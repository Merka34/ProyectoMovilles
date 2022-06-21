using Microsoft.AspNetCore.Mvc;
using ProyectoMovilles.Models;
using ProyectoMovilles.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoMovilles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CelularesController : Controller
    {
        public itesrcne_181g0262Context Context { get; set; }
        CelularesRepository repository;

        public CelularesController(itesrcne_181g0262Context context)
        {
            Context = context;
            repository = new CelularesRepository(context);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var contactos = repository.GetAll();
            return Ok(contactos.Select(x => new
            {
                x.Id,
                x.Marca,
                x.Precio,
                x.Modelo,
                x.Red,
                x.MemoriaRam
            }));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var celular = repository.Get(id);
            if (celular == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(new
                {
                    celular.Id,
                    celular.Marca,
                    celular.Precio,
                    celular.Modelo,
                    celular.MemoriaRam,
                    celular.Red
                });
            }
        }


        [HttpPost("sincronizar")]
        public IActionResult Post([FromBody] DateTime fecha)
        {
            var celulares = repository.GetAllSinceDate(fecha.ToMexicoTime());
            return Ok(celulares.Select(x => new
            {
                x.Id,
                x.Marca,
                x.Precio,
                x.Modelo,
                x.Red,
                x.MemoriaRam
            }));


        }

        [HttpPost]
        public IActionResult Post([FromBody] Celular c)
        {
            try
            {
                c.Id = 0;
                if (repository.IsValid(c, out List<string> errores))
                {
                    repository.Insert(c);
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
        public IActionResult Put([FromBody] Celular c)
        {
            try
            {
                var celular = repository.Get(c.Id);
                if (celular == null)
                    return NotFound();

                if (repository.IsValid(c, out List<string> errores))
                {
                    celular.Modelo = c.Modelo;
                    celular.Marca = c.Marca;
                    celular.MemoriaRam = c.MemoriaRam;
                    celular.Red = c.Red;
                    celular.Precio = c.Precio;
                    repository.Update(celular);
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
        public IActionResult Delete([FromBody] Celular c)
        {
            try
            {
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
