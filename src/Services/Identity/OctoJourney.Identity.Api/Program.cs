using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using OctoJourney.Identity.Api;
using OctoJourney.Identity.Api.Identity;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

var identityConnectionString = configuration.GetConnectionString(ConnectionStringsKeys.IdentityConfiguration) ??
    throw new ApplicationException($"No {nameof(ConnectionStringsKeys.IdentityConfiguration)} connection string.");
var persistedGrantConnectionString = configuration.GetConnectionString(ConnectionStringsKeys.IdentityPersistedGrant) ??
    throw new ApplicationException($"No {nameof(ConnectionStringsKeys.IdentityPersistedGrant)} connection string.");

var migrationAssemblyName = typeof(Program).Assembly.GetName().Name;

services.AddIdentityServer()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = cfg =>
            cfg.UseNpgsql(identityConnectionString,
                sql => sql.MigrationsAssembly(migrationAssemblyName));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = cfg =>
            cfg.UseNpgsql(persistedGrantConnectionString,
                sql => sql.MigrationsAssembly(migrationAssemblyName));
    }).AddDeveloperSigningCredential();

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

await app.InitializeIdentityServerDb();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var option = new RewriteOptions()
        .AddRedirect("^$", "swagger");
    app.UseRewriter(option);
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseIdentityServer();

app.MapControllers();

app.Run();