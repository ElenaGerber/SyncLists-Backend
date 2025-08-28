using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace synclists_backend.Data;

public static class DataExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        var maxAttempts = 10;
        var delay = TimeSpan.FromSeconds(3);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ListStoreContext>();
                await db.Database.MigrateAsync();
                break;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Attempt {attempt} failed: {ex.Message}");
                if (attempt == maxAttempts)
                    throw;
                await Task.Delay(delay);
            }
        }
    }
}