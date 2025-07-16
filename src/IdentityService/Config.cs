using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> apiScopes => new ApiScope[]{
        new ApiScope("auctionApp" , "Auction app full access")
    };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("scope1"),
            new ApiScope("scope2"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
           new Client{
            ClientId = "postman",
            ClientName = "Postman" ,
            AllowedScopes = {"openId","profile"},
            RedirectUris = {"https://www.getpostman.com/oauth2/callback"},
            ClientSecrets = new[] {new Secret("NotSecret".Sha256())},
            AllowedGrantTypes = {GrantType.ResourceOwnerPassword} ,
            
           }
        };
}
