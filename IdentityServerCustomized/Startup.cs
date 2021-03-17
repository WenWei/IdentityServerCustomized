using IdentityServerCustomized.Postgresql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServerCustomized
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webEnvironment)
        {
            Configuration = configuration;
            WebEnvironment = webEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityServerConfigure(services, WebEnvironment);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            DatabaseInitializer.Initialize(app);
            app.UseIdentityServer();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void IdentityServerConfigure(IServiceCollection services, IWebHostEnvironment env)
        {
            var builder = services.AddIdentityServer();

#if DEBUG
            builder.AddDeveloperSigningCredential();
#else
            var rsa = new RsaKeyService(env, TimeSpan.FromDays(30));
            services.AddSingleton<RsaKeyService>(provider => rsa);
            builder.AddSigningCredential(rsa.GetKey(), RsaSigningAlgorithm.RS256);
#endif

            builder.AddConfigurationStore(option =>
                          option.ConfigureDbContext = builder => builder.UseNpgsql(Configuration.GetConnectionString("IdentityServerConnection"), options =>
                          options.MigrationsAssembly("IdentityServerCustomized.Postgresql")))
                   .AddOperationalStore(option =>
                          option.ConfigureDbContext = builder => builder.UseNpgsql(Configuration.GetConnectionString("IdentityServerConnection"), options =>
                          options.MigrationsAssembly("IdentityServerCustomized.Postgresql")));
        }
    }
}
