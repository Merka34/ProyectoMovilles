using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoMovilles.Models;
using System.Linq;

namespace ProyectoMovilles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PerfilController : ControllerBase
    {
        public itesrcne_181g0262Context Context;

        public PerfilController(itesrcne_181g0262Context context)
        {
            Context = context;
        }
        public IActionResult Get(string name)
        {
            Perfil perfil = Context.Perfil.FirstOrDefault(x=>x.NoControl == name);
            string result = JsonConvert.SerializeObject(perfil);
            return Ok(result);
        }
    }
}
