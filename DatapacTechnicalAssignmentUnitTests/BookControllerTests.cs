using DatapacTechnicalAssignment.Controllers;
using DatapacTechnicalAssignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DatapacTechnicalAssignmentUnitTests;

public class BookControllerTests
{
    private readonly BooksController _controller;
    private readonly Mock<DbSet<Book>> _mockBooks;
    private readonly Mock<ApplicationDbContext> _mockContext;

    public BookControllerTests()
    {
        _mockBooks = new Mock<DbSet<Book>>();
        _mockContext = new Mock<ApplicationDbContext>();
        _mockContext.Setup(c => c.Books).Returns(_mockBooks.Object);

        _controller = new BooksController(_mockContext.Object);
    }

    [Fact]
    public async Task CreateBook_ReturnsCreatedAtActionResult_WhenBookIsValid()
    {
        // Arrange
        var book = new Book { Title = "Test Book", Author = "Author" };

        _mockContext.Setup(c => c.Books.Add(book)).Verifiable();
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

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
        _mockContext.Setup(c => c.Books.FindAsync(bookId)).ReturnsAsync((Book)null);

        // Act
        var result = await _controller.GetBook(bookId) as NotFoundResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    // Add additional tests for UpdateBook and DeleteBook
}
