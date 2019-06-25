using System;
using AuthenticationSample.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<SampleIdentityDbContext>(options => options.UseSqlite(new SqliteConnectionStringBuilder
            {
                DataSource = "identity.db"
            }.ToString()));
            services.AddDbContext<TicketDbContext>(options => options.UseSqlite(new SqliteConnectionStringBuilder
            {
                DataSource = "ticket.db"
            }.ToString()));
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.SessionStore = services.BuildServiceProvider().GetService<TicketDbContext>();
            });
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequiredLength = 8;
                    })
                    .AddEntityFrameworkStores<SampleIdentityDbContext>()
                    .AddDefaultTokenProviders();
            services.AddDataProtection().SetApplicationName("CookieAuthSample");
            services.AddAuthentication().AddCookie().AddJwtBearer(options =>
            {
                options.Audience = AppConfig.SiteUrl;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = AppConfig.SiteUrl,
                    IssuerSigningKey = new SymmetricSecurityKey(AppConfig.SecretKey),
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
