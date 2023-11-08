using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace OctoJourney.Identity.Api.Identity;

public static class IdentityExtensions
{
    public static async Task<IApplicationBuilder> InitializeIdentityServerDb(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
        await using var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        await using var persistedGrantsContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

        await configurationContext.Database.MigrateAsync();
        await persistedGrantsContext.Database.MigrateAsync();

        if (!await configurationContext.Clients.AnyAsync())
        {
            var clients = IdentityConfiguration.Clients.Select(c => c.ToEntity());
            
            await configurationContext.Clients
                .AddRangeAsync(clients);
        }

        if (!await configurationContext.ApiScopes.AnyAsync())
        {
            await configurationContext.ApiScopes
                .AddRangeAsync(IdentityConfiguration.ApiScopes.Select(c => c.ToEntity()));
        }

        if (!await configurationContext.IdentityResources.AnyAsync())
        {
            await configurationContext.IdentityResources
                .AddRangeAsync(IdentityConfiguration.IdentityResources.Select(c => c.ToEntity()));
        }

        await configurationContext.SaveChangesAsync();
        
        return app;
    }
}