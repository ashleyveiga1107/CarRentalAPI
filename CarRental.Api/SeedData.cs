using CarRental.Api.Models;

namespace CarRental.Api.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        if (context.Users.Any())
        {
            return;
        }

        var users = new List<User>
        {
            new User
            {
                Username = "Alice",
                Email = "alice@example.com"
            },
            new User
            {
                Username = "Bob",
                Email = "bob@example.com"
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }
}