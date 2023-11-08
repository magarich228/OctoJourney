using IdentityModel;
using IdentityServer4.Models;

namespace OctoJourney.Identity.Api.Identity;

public static class IdentityConfiguration
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>()
        {
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Email(),
            new IdentityResources.OpenId()
        };
    
    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>()
        {
            new ApiScope("Api1")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "OctoJourney.Razor.Client",
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                ClientSecrets = { new Secret("temp".ToSha256()) },
                AllowedScopes = { "Api1" }
            }
        };
}