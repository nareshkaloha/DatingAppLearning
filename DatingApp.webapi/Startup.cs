using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DatingApp.webapi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using DatingApp.webapi.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using DatingApp.webapi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DatingApp.webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt=> 
            {
                opt.UseLazyLoadingProxies();
                opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnectionString"));                
            });         

            ConfigureServices(services);
        }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt=> 
            {
                opt.UseLazyLoadingProxies();
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString")); 
            });

            ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<DataContext>(opt=> opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString")));

            IdentityBuilder builder = services.AddIdentityCore<User>(opt => 
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

             services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateAudience = false,
                        ValidateIssuer = false
                    };
                });

            services.AddAuthorization(opt => 
            {
                opt.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("AdminOrModeratorPhotoOnly", policy => policy.RequireRole("Admin", "Moderator"));
                opt.AddPolicy("VIPOnly", policy => policy.RequireRole("VIP"));
            });

            services.AddControllers(options => 
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();     

                options.Filters.Add( new AuthorizeFilter(policy));
            }
            )
            .AddNewtonsoftJson(opt => 
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddCors();
            services.AddAutoMapper(typeof(DatingRepository).Assembly);
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddScoped<LogUserLastActivity>();           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }
            // else
            // {
            //     app.UseExceptionHandler(builder=> {
            //        builder.Run(async context=> {
            //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //            var error = context.Features.Get<IExceptionHandlerFeature>();

            //            if(error!=null)
            //            {
            //                context.Response.AddApplicationError("please try later ..");
            //                await context.Response.WriteAsync("error happended on server , its logged , you dont need to know what went wrong");
            //            }
            //        });
            //     });
            // }

            app.UseDeveloperExceptionPage();
            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(x=> x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
