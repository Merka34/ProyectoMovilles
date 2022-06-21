using ProyectoMovilles.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoMovilles.Repositories
{
    public class NotasRepository : Repository<Nota>
    {
        public NotasRepository(itesrcne_181g0262Context context) : base(context)
        {

        }


        public override void Insert(Nota entity)
        {
            entity.Timestamp = DateTime.Now.ToMexicoTime();
            base.Insert(entity);
        }

        public override IEnumerable<Nota> GetAll()
        {
            return base.GetAll().Where(x => x.Eliminado == 0).OrderBy(x => x.Titulo);
        }

        public IEnumerable<Nota> GetAllSinceDate(DateTime timestamp)
        {
            var cambiados = base.GetAll().Where(x => x.Eliminado == 0 && x.Timestamp > timestamp);
            var eliminados = base.GetAll().Where(x => x.Eliminado == 1 && x.Timestamp > timestamp)
                .Select(x => new Nota { Id = x.Id });

            return cambiados.Concat(eliminados);
        }

        public override void Update(Nota entity)
        {
            entity.Timestamp = DateTime.Now.ToMexicoTime();
            base.Update(entity);
        }

        public override void Delete(Nota entity)
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


        public override bool IsValid(Nota entity, out List<string> validationErrors)
        {
            validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(entity.Titulo))
                validationErrors.Add("La marca del celular se encuentra vacio.");


            if (Context.Set<Nota>().Any(x => x.Titulo == entity.Titulo && x.Descripcion == entity.Descripcion && x.Eliminado == 0 && x.Id != entity.Id))
            {
                validationErrors.Add("El nombre del contacto ya se encuentra en la lista.");
            }

            return validationErrors.Count == 0;
        }
    }
}
