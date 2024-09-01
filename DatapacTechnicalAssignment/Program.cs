using DatapacTechnicalAssignment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Pridanie služieb do kontajnera
builder.Services.AddControllers();

// Nastavenie Entity Framework Core s in-memory databázou
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

// Nastavenie Quartz.NET pre plánovanie úloh
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Definovanie úlohy a jej spúšťania
    var jobKey = new JobKey("ReminderJob");
    q.AddJob<ReminderJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ReminderJob-trigger")
        .WithCronSchedule("0 * * * * ?")); // Cron schedule: každú minútu pre testovacie účely
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Pridanie Swagger/OpenAPI na dokumentáciu API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library Management API", Version = "v1" });
});

var app = builder.Build();

// Použitie Swaggeru v developmente
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management API v1");
    });
}

// Pridanie HTTPS redirection a logovania požiadaviek
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Definícia Quartz Jobu ako samostatnej triedy
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