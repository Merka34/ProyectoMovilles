using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ProyectoMovilles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoMovilles
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            llave = Configuration["JwtAuth:Key"];
            issuer = Configuration["JwtAuth:Issuer"];
            aud = Configuration["JwtAuth:Audience"];
        }

        public IConfiguration Configuration { get; }

        string llave = "";
        string issuer = "";
        string aud = "";


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string path = "server=204.93.216.11;user=itesrcne_daniel;database=itesrcne_181g0262;password=181G0262";
            services.AddDbContext<itesrcne_181g0262Context>(optionsBuilder =>
                           optionsBuilder.UseMySql(path, Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.3.29-mariadb")));
            services.AddControllers();
            services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {

            ValidAudience = aud,
            ValidIssuer = issuer,
            IssuerSigningKey =
             new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(llave)),
            ValidateIssuerSigningKey = true

        };
    });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRequestLocalization("es-MX");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
