using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProyectoMovilles.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace ProyectoMovilles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        public itesrcne_181g0262Context Context { get; set; }

        public LoginController(IConfiguration configuration, itesrcne_181g0262Context context)
        {
            Configuration = configuration;
            Context = context;
        }
        [HttpPost]
        public IActionResult Post(SesionModel model)
        {
            var usuario = Context.Usuario.FirstOrDefault(x=>x.NombreUsu==model.Usuario);
            if (usuario!=null && usuario.NombreUsu==model.Usuario && usuario.Clave==model.Contraseña)  
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, "Admin"));
                claims.Add(new Claim("Id", "1"));
                claims.Add(new Claim(ClaimTypes.Role, "1"));
                claims.Add(new Claim(JwtRegisteredClaimNames.Iss,
                    Configuration["JwtAuth:Issuer"]));
                claims.Add(new Claim(JwtRegisteredClaimNames.Exp,
                    DateTime.UtcNow.AddMinutes(3).ToString()));


                var handler = new JwtSecurityTokenHandler();


                SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor();
                descriptor.Issuer = Configuration["JwtAuth:Issuer"];
                descriptor.Audience = Configuration["JwtAuth:Audience"];
                descriptor.Subject = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                descriptor.Expires = DateTime.UtcNow.AddMinutes(3);
                descriptor.SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(Configuration["JwtAuth:Key"])),
                    SecurityAlgorithms.HmacSha256);

                var token = handler.CreateToken(descriptor);
                var tokenString = handler.WriteToken(token);
                return Ok(tokenString);

            }
            else
            {
                return Unauthorized("El usuario o la contraseña son incorrectos.");
            }
        }
    }
}
