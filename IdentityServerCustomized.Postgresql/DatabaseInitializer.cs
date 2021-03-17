using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace IdentityServerCustomized.Postgresql
{
    public class DatabaseInitializer
    {
        public static void Initialize(IApplicationBuilder app, IConfiguration configuration)
        {
            InitializeTokenServerConfigurationDatabase(app, configuration);
        }

        private static void InitializeTokenServerConfigurationDatabase(IApplicationBuilder app, IConfiguration configuration)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()
                   .CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>()
                    .Database.Migrate();

                var context = scope.ServiceProvider
                    .GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

#if DEBUG
                context.Clients.RemoveRange(context.Clients);
                context.ApiScopes.RemoveRange(context.ApiScopes);
                context.IdentityResources.RemoveRange(context.IdentityResources);
                context.ApiResources.RemoveRange(context.ApiResources);
                context.SaveChanges();
#endif

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients(configuration))
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var item in Config.GetApiScopes(configuration))
                    {
                        context.ApiScopes.Add(item.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources(configuration))
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources(configuration))
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
