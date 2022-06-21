using Microsoft.EntityFrameworkCore;
using ProyectoMovilles.Models;
using System.Collections.Generic;

namespace ProyectoMovilles.Repositories
{

    public class Repository<T> where T : class
    {
        public DbContext Context { get; set; }

        public Repository(DbContext context)
        {
            Context = context;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Context.Set<T>();
        }

        public virtual T Get(object id)
        {
            return Context.Find<T>(id);
        }

        public virtual void Insert(T entidad)
        {
            Context.Add(entidad);
            Context.SaveChanges();
        }

        public virtual void Update(T entidad)
        {
            Context.Update(entidad);
            Context.SaveChanges();
        }

        public virtual void Delete(T entitdad)
        {
            Context.Remove(entitdad);
            Context.SaveChanges();
        }

        public virtual bool IsValid(T entitdad, out List<string> validationErrors)
        {
            validationErrors = new List<string>();
            return true;
        }
    }
}
