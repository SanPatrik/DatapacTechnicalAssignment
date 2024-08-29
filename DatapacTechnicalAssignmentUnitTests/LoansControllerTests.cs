using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DatapacTechnicalAssignment.Controllers;
using DatapacTechnicalAssignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DatapacTechnicalAssignmentUnitTests
{
    public class LoansControllerTests
    {
        private readonly LoansController _controller;
        private readonly Mock<DbSet<Loan>> _mockLoans;
        private readonly Mock<DbSet<Book>> _mockBooks;
        private readonly Mock<DbSet<User>> _mockUsers;
        private readonly Mock<ApplicationDbContext> _mockContext;

        public LoansControllerTests()
        {
            _mockLoans = new Mock<DbSet<Loan>>();
            _mockBooks = new Mock<DbSet<Book>>();
            _mockUsers = new Mock<DbSet<User>>();
            _mockContext = new Mock<ApplicationDbContext>();

            _mockContext.Setup(c => c.Loans).Returns(_mockLoans.Object);
            _mockContext.Setup(c => c.Books).Returns(_mockBooks.Object);
            _mockContext.Setup(c => c.Users).Returns(_mockUsers.Object);

            _controller = new LoansController(_mockContext.Object);
        }

        [Fact]
        public async Task CreateLoan_ReturnsCreatedAtActionResult_WhenLoanIsValid()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                BorrowedDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30)
            };

            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                Author = "Author Name",
                Quantity = 5 // Initial quantity
            };

            _mockBooks.Setup(b => b.FindAsync(bookId)).ReturnsAsync(book);
            _mockUsers.Setup(u => u.FindAsync(userId)).ReturnsAsync(new User { Id = userId });

            _mockContext.Setup(c => c.Loans.Add(It.IsAny<Loan>())).Verifiable();
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateLoan(loan) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Verify the book quantity is decremented
            Assert.Equal(4, book.Quantity);
        }

        [Fact]
        public async Task ReturnBook_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var loanId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var loan = new Loan
            {
                Id = loanId,
                BookId = bookId,
                BorrowedDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30)
            };

            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                Author = "Author Name",
                Quantity = 4 // Quantity after being borrowed
            };

            _mockContext.Setup(c => c.Loans.Include(l => l.Book)
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Loan, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(loan);

            _mockBooks.Setup(b => b.FindAsync(bookId)).ReturnsAsync(book);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.ReturnBook(loanId) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Verify the book quantity is incremented
            Assert.Equal(5, book.Quantity);
        }

        [Fact]
        public async Task GetLoan_ReturnsNotFound_WhenLoanDoesNotExist()
        {
            // Arrange
            var loanId = Guid.NewGuid();
            _mockContext.Setup(c => c.Loans.Include(l => l.Book).Include(l => l.User)
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Loan, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Loan)null);

            // Act
            var result = await _controller.GetLoan(loanId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }
    }
}
