using DatapacTechnicalAssignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DatapacTechnicalAssignment.Controllers
{
    /// <summary>
    /// Controller pre správu kníh v knižnici.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Vytvorí novú knihu.
        /// </summary>
        /// <param name="book">Model knihy na vytvorenie.</param>
        /// <returns>HTTP 201 Created ak bola kniha úspešne vytvorená.</returns>
        /// <response code="400">Vracia sa, ak sú vstupy neplatné.</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Book), 201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        /// <summary>
        /// Získa detaily knihy podľa ID.
        /// </summary>
        /// <param name="id">ID knihy.</param>
        /// <returns>HTTP 200 OK ak kniha existuje, inak HTTP 404 Not Found.</returns>
        /// <response code="404">Vracia sa, ak kniha neexistuje.</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Book), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetBook([FromRoute] Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        /// <summary>
        /// Aktualizuje existujúcu knihu podľa ID.
        /// </summary>
        /// <param name="id">ID knihy na aktualizáciu.</param>
        /// <param name="book">Model knihy s novými údajmi.</param>
        /// <returns>HTTP 204 No Content ak bola kniha úspešne aktualizovaná, inak HTTP 400 Bad Request.</returns>
        /// <response code="400">Vracia sa, ak ID v URL neodpovedá ID v tele požiadavky.</response>
        /// <response code="404">Vracia sa, ak kniha neexistuje.</response>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> UpdateBook([FromRoute] Guid id, [FromBody] Book book)
        {
            if (id != book.Id) return BadRequest("ID in request body does not match ID in URL.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null) return NotFound();

            _context.Entry(existingBook).CurrentValues.SetValues(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Odstráni knihu podľa ID.
        /// </summary>
        /// <param name="id">ID knihy na odstránenie.</param>
        /// <returns>HTTP 204 No Content ak bola kniha úspešne odstránená, inak HTTP 404 Not Found.</returns>
        /// <response code="404">Vracia sa, ak kniha neexistuje.</response>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> DeleteBook([FromRoute] Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
