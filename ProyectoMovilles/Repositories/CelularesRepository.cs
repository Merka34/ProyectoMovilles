using ProyectoMovilles.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoMovilles.Repositories
{
    public class CelularesRepository : Repository<Celular>
    {
        public CelularesRepository(itesrcne_181g0262Context context) : base(context)
        { 

        }

        public override void Insert(Celular entity)
        {
            entity.Timestamp = DateTime.Now.ToMexicoTime();
            base.Insert(entity);
        }

        public override IEnumerable<Celular> GetAll()
        {
            return base.GetAll().Where(x => x.Eliminado == 0).OrderBy(x => x.Marca);
        }

        public IEnumerable<Celular> GetAllSinceDate(DateTime timestamp)
        {
            var cambiados = base.GetAll().Where(x => x.Eliminado == 0 && x.Timestamp > timestamp);
            var eliminados = base.GetAll().Where(x => x.Eliminado == 1 && x.Timestamp > timestamp)
                .Select(x => new Celular { Id = x.Id });

            return cambiados.Concat(eliminados);
        }

        public override void Update(Celular entity)
        {
            entity.Timestamp = DateTime.Now.ToMexicoTime();
            base.Update(entity);
        }

        public override void Delete(Celular entity)
        {
            if (entity.Eliminado == 0)
            {
                entity.Timestamp = DateTime.Now.ToMexicoTime();
                entity.Eliminado = 1;
                base.Update(entity);
            }

            int TTL = 5; //5 dias

            //LinqToObjects
            //LinqToSql


            var fechaEliminar = DateTime.Now.ToMexicoTime().Subtract(TimeSpan.FromDays(TTL));

            var porEliminar = base.GetAll().Where(x => x.Eliminado == 1
            && x.Timestamp < fechaEliminar);

            foreach (var item in porEliminar)
            {
                Context.Remove(item);
            }
            Context.SaveChanges();


        }


        public override bool IsValid(Celular entity, out List<string> validationErrors)
        {
            validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(entity.Marca))
                validationErrors.Add("La marca del celular se encuentra vacio.");
            

            if (Context.Set<Celular>().Any(x => x.Marca == entity.Marca && x.Modelo == entity.Modelo && x.Eliminado == 0 && x.Id != entity.Id))
            {
                validationErrors.Add("El nombre del contacto ya se encuentra en la lista.");
            }

            return validationErrors.Count == 0;
        }

    }
}
