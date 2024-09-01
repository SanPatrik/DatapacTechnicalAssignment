using DatapacTechnicalAssignment.Controllers.Dtos;
using DatapacTechnicalAssignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatapacTechnicalAssignment.Controllers
{
    /// <summary>
    /// Controller pre správu výpožičiek kníh.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Vytvorí novú výpožičku.
        /// </summary>
        /// <param name="loanDto">Detaily výpožičky.</param>
        /// <returns>HTTP 201 Created ak bola výpožička úspešne vytvorená.</returns>
        /// <response code="400">Vracia sa, ak sú vstupy neplatné.</response>
        /// <response code="404">Vracia sa, ak používateľ alebo kniha neexistuje.</response>
        /// <response code="200">Vracia sa, ak kniha nie je dostupná.</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Loan), 201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> CreateLoan([FromBody] LoanDto loanDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FindAsync(loanDto.UserId);
            if (user == null)
                return NotFound("User not found.");
            
            var book = await _context.Books.FindAsync(loanDto.BookId);
            
            if (book == null)
                return NotFound("Book not found.");
            
            if (book.AvailableQuantity <= 0)
                return Ok(new { Message = "Book is not available." });
            
            book.AvailableQuantity--;

            var loan = new Loan
            {
                UserId = loanDto.UserId,
                BookId = loanDto.BookId,
                BorrowedDate = DateTime.Now,
                ReturnedDate = null,
                DueDate = DateTime.Now.AddDays(1)
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan);
        }

        /// <summary>
        /// Vráti knihu podľa ID výpožičky.
        /// </summary>
        /// <param name="id">ID výpožičky.</param>
        /// <returns>HTTP 200 OK ak bola kniha úspešne vrátená, inak HTTP 404 Not Found.</returns>
        /// <response code="404">Vracia sa, ak výpožička neexistuje.</response>
        [HttpPatch("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> ReturnBook(Guid id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);
            if (loan == null) return NotFound();

            loan.Book.AvailableQuantity++;
            loan.ReturnedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Book is returned." });
        }


        /// <summary>
        /// Získa detaily existujúcej výpožičky podľa ID.
        /// </summary>
        /// <param name="id">ID výpožičky.</param>
        /// <returns>HTTP 200 OK ak výpožička existuje, inak HTTP 404 Not Found.</returns>
        /// <response code="404">Vracia sa, ak výpožička neexistuje.</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Loan), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetLoan([FromRoute] Guid id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null) return NotFound($"Loan with ID {id} not found.");
            
            return Ok(loan);
        }
    }
}
