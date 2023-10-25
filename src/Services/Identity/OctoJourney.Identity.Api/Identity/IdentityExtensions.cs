using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace OctoJourney.Identity.Api.Identity;

public static class IdentityExtensions
{
    public static async Task<IApplicationBuilder> InitializeIdentityServerDb(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        await using var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        await using var persistedGrantsContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

        await configurationContext.Database.MigrateAsync();
        await persistedGrantsContext.Database.MigrateAsync();
        
        return app;
    }
}