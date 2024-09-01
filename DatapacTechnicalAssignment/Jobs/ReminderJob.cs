using DatapacTechnicalAssignment.Models;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace DatapacTechnicalAssignment.Jobs;

public class ReminderJob : IJob
{
    private readonly ILogger<ReminderJob> _logger;
    private readonly ApplicationDbContext _context;

    public ReminderJob(ILogger<ReminderJob> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var loans = _context.Loans.Include(l => l.User).Include(l => l.Book)
            .Where(l => l.DueDate <= DateTime.Now.AddDays(1) && l.ReturnedDate == null).ToList();

        if (loans.Count == 0)   
        {
            _logger.LogInformation("No loans due tomorrow.");
        }
        foreach (var loan in loans)
        {
            // Mock sending email
            _logger.LogInformation("Sending reminder email to {userEmail} for book {bookTitle} due on {dueDate}",
                loan.User.Email, loan.Book.Title, loan.DueDate);
        }

        await Task.CompletedTask;
    }
}