using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace OctoJourney.Identity.Api.Identity;

public static class IdentityExtensions
{
    /// <summary>
    /// Проводит миграции и инициализирует БД недостающими
    /// данными конфигурации Identity Server из памяти.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task<IApplicationBuilder> InitializeIdentityServerDb(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
        await using var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        await using var persistedGrantsContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

        await configurationContext.Database.MigrateAsync();
        await persistedGrantsContext.Database.MigrateAsync();

        var clients = IdentityConfiguration.Clients.Select(c => c.ToEntity());
        var apiScopes = IdentityConfiguration.ApiScopes.Select(a => a.ToEntity());
        var identityResources = IdentityConfiguration.IdentityResources.Select(i => i.ToEntity());
        
        // ReSharper disable AccessToDisposedClosure
        var clientsDiff = clients.Except(configurationContext.Clients).
            Where(c => configurationContext.Clients.FirstOrDefault(cl => cl.ClientId.Equals(c.ClientId)) is null);

        var apiScopesDiff = apiScopes.Except(configurationContext.ApiScopes).
            Where(a => configurationContext.ApiScopes.FirstOrDefault(ap => ap.Name.Equals(a.Name)) is null);

        var identityResourcesDiff = identityResources.Except(configurationContext.IdentityResources).
            Where(i => configurationContext.IdentityResources.FirstOrDefault(ir => ir.Name.Equals(i.Name)) is null);
        // ReSharper restore AccessToDisposedClosure
        
        await configurationContext.Clients.AddRangeAsync(clientsDiff);
        await configurationContext.ApiScopes.AddRangeAsync(apiScopesDiff);
        await configurationContext.IdentityResources.AddRangeAsync(identityResourcesDiff);

        await configurationContext.SaveChangesAsync();
        
        return app;
    }
}