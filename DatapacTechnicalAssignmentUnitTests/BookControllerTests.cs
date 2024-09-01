using DatapacTechnicalAssignment.Controllers;
using DatapacTechnicalAssignment.Models;
using DatapacTechnicalAssignment.Controllers.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatapacTechnicalAssignmentUnitTests
{
    public class BookControllerTests
    {
        private readonly BooksController _controller;
        private readonly ApplicationDbContext _context;

        public BookControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new BooksController(_context);
        }
        
        [Fact]
        public async Task GetBook_ReturnsOk_WhenBookExists()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Test Book", Author = "Author", Quantity = 3, AvailableQuantity = 3 };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetBook(bookId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
        
        [Fact]
        public async Task CreateBook_ReturnsCreatedAtActionResult_WhenBookIsValid()
        {
            // Arrange
            var book = new BookDto { Title = "Test Book", Author = "Author", Quantity = 3};

            // Act
            var result = await _controller.CreateBook(book) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
        }

        [Fact]
        public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            // Act
            var result = await _controller.GetBook(bookId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task UpdateBook_ReturnsOk_WhenBookIsUpdated()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Test Book", Author = "Author", Quantity = 3, AvailableQuantity = 3};
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var bookDto = new BookDto { Title = "Test Book Updated", Author = "Author Updated", Quantity = 5 };

            // Act
            var result = await _controller.UpdateBook(bookId, bookDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContent_WhenBookIsDeleted()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Test Book", Author = "Author", Quantity = 3, AvailableQuantity = 3 };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteBook(bookId) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }
    }
}