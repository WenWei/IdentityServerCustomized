using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServerCustomized.Postgresql
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
#if DEBUG
                new IdentityResource
                {
                    Name = "rc.scope",
                    UserClaims =
                    {
                        "rc.garndma"
                    }
                }
#endif
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var scopes = new List<ApiScope>();
            configuration.GetSection("IdentityServer:ApiScopes").Bind(scopes);

#if DEBUG
            scopes.AddRange( new List<ApiScope>
            {
                new ApiScope("ApiOne"),
                new ApiScope("ApiTwo"),
                new ApiScope("api1.read"),
                new ApiScope("api1.write")
            });
#endif
            return scopes;
        }

        // scopes define the API resources in your system
        public static IEnumerable<ApiResource> GetApiResources(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var resources = new List<ApiResource>();
            configuration.GetSection("IdentityServer:ApiResources").Bind(resources);

#if DEBUG
            resources.AddRange( new List<ApiResource>
            {
                new ApiResource("ApiOne"),
                new ApiResource("ApiTwo", new string[] { "rc.api.garndma" }),
                new ApiResource("api1", "API sample")
                {
                    Scopes =
                    {
                        "api1.read",
                        "api1.write",
                    }
                }
            });
#endif

            return resources;
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            // client credentials client
            var clients = new List<Client>();
            configuration.GetSection("IdentityServer:Clients").Bind(clients);

#if DEBUG
            clients.AddRange( new List<Client> {
                // resource owner password grant client
                new Client
                {
                    ClientId = "client1",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowOfflineAccess = true,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        StandardScopes.OfflineAccess,
                        "api1",
                        "api1.read",
                        },
                    AllowAccessTokensViaBrowser = true
                },
                 new Client
                {
                    ClientId = "client2",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowOfflineAccess = true,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    SlidingRefreshTokenLifetime = 1296000,
                    AccessTokenLifetime = 60,
                    AlwaysSendClientClaims = true,
                    Enabled = true,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1",
                        "api1.read",
                        "api1.write",
                        "ApiOne",
                        "ApiTwo",
                        },
                    AllowAccessTokensViaBrowser = true
                 },
            });
#endif
            return clients;
        }
    }
}
