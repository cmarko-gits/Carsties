using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace IdentityService
{
    public static class Config
    {
        // Identity resursi (OpenID Connect standard)
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        // API scopes - definicija dozvola za pristup API-ju
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("auctionApp", "Auction app full access"),
                new ApiScope("scope1"),
                new ApiScope("scope2"),
            };

        // Klijenti koji mogu da koriste IdentityServer
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "postman",
                    ClientName = "Postman",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    // Tajna klijenta
                    ClientSecrets = { new Secret("NotSecret".Sha256()) },

                    // Dozvoljeni scope-ovi koje klijent može da traži
                    AllowedScopes = { "openid", "profile", "auctionApp", "offline_access" },

                    // Omogući refresh tokene
                    AllowOfflineAccess = true,
                } , 

                new Client{
                    ClientId = "nextApp",
                    ClientName = "nextApp" ,
                    ClientSecrets = new Secret[] { new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequirePkce = false,
                    RedirectUris = {"http://localhost:3000/api/auth/callback/id-server"},
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile","auctionApp"},
                    AccessTokenLifetime = 3600*24*30
                }
            };

        public static IEnumerable<ApiResource> ApiResources { get; internal set; }

    }
}
