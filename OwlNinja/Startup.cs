using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using OwlNinja.Controllers;

namespace OwlNinja
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
            services.AddMvc();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => {
                        options.TokenValidationParameters =
                             new TokenValidationParameters
                             {
                                 ValidateIssuer = true,
                                 ValidateAudience = true,
                                 ValidateLifetime = true,
                                 ValidateIssuerSigningKey = true,

                                 ValidIssuer = "OwlNinja.Security.Bearer",
                                 ValidAudience = "OwlNinja.Security.Bearer",
                                 IssuerSigningKey = JwtSecurityKey.Create()
                             };
                    });

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<ValidateReCaptchaAttribute>();
        }

      
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvc();
        }

       
    }

    public static class JwtSecurityKey
    {
        public static SymmetricSecurityKey Create()
        {
            if (String.IsNullOrWhiteSpace(Result))
            {
                Random rand = new Random();
                int[] NoDuplicates = InitializeArrayWithNoDuplicates(rand.Next(10, 100), rand);
                Result = "JrN4IeSBZDIh$" + string.Join("", NoDuplicates) + "hNjC)6~ULlfL5nwdp%";
            }            
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Result));
        }

        private static string Result { get; set; }

        public static int[] InitializeArrayWithNoDuplicates(int size,Random rand)
        {
            return Enumerable.Repeat<int>(0, size).Select((x, i) => new { i = i, rand = rand.Next() }).OrderBy(x => x.rand).Select(x => x.i).ToArray();
        }
    }
}
