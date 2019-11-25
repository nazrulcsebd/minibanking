using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MiniBanking.Core.Helper;
using MiniBanking.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using CodeBonds.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CodeBonds
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfig = configuration;
        }
        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfig { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Constants.SqlServer = Configuration["DB:Server"];
            var initialCatelog = Configuration["DB:InitialCatelog"];
            Constants.SqlServerUserId = Configuration["DB:UserId"];
            Constants.SqlServerPassword = Configuration["DB:Password"];

            Constants.DefaultConnectionString = $"Server={Constants.SqlServer};Initial Catalog={initialCatelog};Persist Security Info=False;User ID={Constants.SqlServerUserId};Password={Constants.SqlServerPassword};MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True;Connection Timeout=3000;";
            services.AddDbContext<MiniBankingDbContext>(options => options.UseSqlServer(Constants.DefaultConnectionString));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IRazorViewRenderService, RazorViewRenderService>();

            services
                .AddMvc(options => { options.EnableEndpointRouting = false; })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver(); --- for Pascal Case Request Serialization
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            app.UseAuthentication();

            app.UseMvc();
            app.UseStaticFiles();
        }
    }
}
