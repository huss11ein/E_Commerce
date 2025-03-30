using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Extensions
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }
} 