using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StaticSample.Data;

namespace StaticSample;

public class UserRegistrationCleanupService(IServiceProvider ServiceProvider, ILogger<UserRegistrationCleanupService> Logger) : IHostedService
{
    private const int CleanupIntervalMinutes = 5;
    private const int DeleteAfterMinutes = 30;

    private Task? _executingTask;
    private readonly CancellationTokenSource _cts = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Staring cleanup service");

        _executingTask = ExecuteAsync(_cts.Token);

        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMinutes(CleanupIntervalMinutes));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await CleanupAsync(stoppingToken);
            }
        }
        catch (Exception)
        {
            Logger.LogInformation("User Registration Cleanup Service has been cancelled");
        }
    }

    private async Task CleanupAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var deleteAt = DateTime.UtcNow.AddMinutes(DeleteAfterMinutes);
            var usersToDelete = await dbContext.Users.Where(u => u.CreatedAt < deleteAt).ToListAsync(stoppingToken);

            Logger.LogInformation("Found {count} users to be deleted", usersToDelete.Count);

            foreach (var user in usersToDelete)
            {
                if (user is { Email: ApplicationUser.DefaultEmail }) continue;

                Logger.LogInformation("Deleting user {email}", user.Email);
                await userManager.DeleteAsync(user);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while cleaning up user registrations");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("User Registration Cleanup Service is stopping");

        if (_executingTask is null) return;

        try
        {
            _cts.Cancel();
        }
        finally 
        {
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }
}
