using DatapacTechnicalAssignment.Controllers;
using DatapacTechnicalAssignment.Models;
using DatapacTechnicalAssignment.Controllers.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatapacTechnicalAssignmentUnitTests
{
    public class LoansControllerTests
    {
        private readonly LoansController _controller;
        private readonly ApplicationDbContext _context;

        public LoansControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new LoansController(_context);
        }

        [Fact]
        public async Task CreateLoan_ReturnsCreatedAtActionResult_WhenLoanIsValid()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var loanDto = new LoanDto
            {
                UserId = userId,
                BookId = bookId,
            };

            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                Author = "Author Name",
                Quantity = 5 ,// Initial quantity
                AvailableQuantity = 5
            };

            _context.Books.Add(book);
            _context.Users.Add(new User { Id = userId, Name = "Name", Email = "Email" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.CreateLoan(loanDto) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);

            // Verify the book quantity is decremented
            var updatedBook = await _context.Books.FindAsync(bookId);
            Assert.Equal(4, updatedBook.AvailableQuantity);
        }

        [Fact]
        public async Task ReturnBook_ReturnsOK_WhenSuccessful()
        {
            // Arrange
            var loanId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Name", Email = "Email" };
            var loan = new Loan
            {
                Id = loanId,
                BookId = bookId,
                UserId = userId,
                BorrowedDate = DateTime.UtcNow,
                ReturnedDate = null,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                Author = "Author Name",
                Quantity = 5, // Quantity after being borrowed
                AvailableQuantity = 4
            };

            _context.Loans.Add(loan);
            _context.Books.Add(book);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ReturnBook(loanId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            // Verify the book quantity is incremented
            var updatedBook = await _context.Books.FindAsync(bookId);
            Assert.Equal(5, updatedBook.AvailableQuantity);
        }

        [Fact]
        public async Task GetLoan_ReturnsNotFound_WhenLoanDoesNotExist()
        {
            // Arrange
            var loanId = Guid.NewGuid();

            // Act
            var result = await _controller.GetLoan(loanId) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }
    }
}