using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoMovilles.Models;
using ProyectoMovilles.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoMovilles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotasController : ControllerBase
    {
        public itesrcne_181g0262Context Context { get; set; }
        NotasRepository repository;

        public NotasController(itesrcne_181g0262Context context)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile("notas.json")
                });
            }
            Context = context;
            repository = new NotasRepository(context);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var notas = repository.GetAll();
            return Ok(notas.Select(x => new
            {
                x.Id,
                x.Titulo,
                x.Descripcion
            }));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var nota = repository.Get(id);
            if (nota == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(new
                {
                    nota.Id,
                    nota.Titulo,
                    nota.Descripcion
                });
            }
        }


        [HttpPost("sincronizar")]
        public IActionResult Post([FromBody] DateTime fecha)
        {
            var notas = repository.GetAllSinceDate(fecha.ToMexicoTime());
            return Ok(notas.Select(x => new
            {
                x.Id,
                x.Titulo,
                x.Descripcion
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Nota n)
        {
            try
            {
                Message message = new Message();
                message.Topic = "nota";
                message.Data = new Dictionary<string, string>
                    {
                        {"Titulo",n.Titulo},
                        {"Descripcion", n.Descripcion},
                        {"Accion","Agregar" }
                    };
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
                n.Id = 0;
                if (repository.IsValid(n, out List<string> errores))
                {
                    repository.Insert(n);
                    return Ok();
                }
                else
                {
                    return BadRequest(errores);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) return StatusCode(500, ex.InnerException.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Nota n)
        {
            try
            {
                Message message = new Message();
                message.Topic = "nota";
                message.Data = new Dictionary<string, string>
                    {
                        {"Id", n.Id.ToString()},
                        {"Titulo", n.Titulo},
                        {"Descripcion", n.Descripcion},
                        {"Accion","Editar" }
                    };
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
                var nota = repository.Get(n.Id);
                if (nota == null)
                    return NotFound();

                if (repository.IsValid(n, out List<string> errores))
                {
                    nota.Titulo = n.Titulo;
                    nota.Descripcion = n.Descripcion;
                    repository.Update(nota);
                    return Ok();
                }
                else
                {
                    return BadRequest(errores);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) return StatusCode(500, ex.InnerException.Message);

                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] Celular c)
        {
            try
            {
                Message message = new Message();
                message.Topic = "nota";
                message.Data = new Dictionary<string, string>
                    {
                        {"Accion","Eliminar" },
                        {"Id",c.Id.ToString() }
                    };
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
                var celular = repository.Get(c.Id);
                if (celular == null)
                    return NotFound();

                repository.Delete(celular);
                return Ok();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) return StatusCode(500, ex.InnerException.Message);

                return StatusCode(500, ex.Message);
            }
        }
    }
}
