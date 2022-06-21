using NotasNotificaciones.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotasNotificaciones.Models
{
    public class CatalogoNota
    {
        public SQLiteConnection Connection { get; set; }
        public CatalogoNota()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/notas.db3";
            Connection = new SQLiteConnection(path);
            Connection.CreateTable<Nota>();
        }

        public IEnumerable<Nota> GetAll()
        {
            return Connection.Table<Nota>().OrderBy(x => x.Titulo);
        }

        public void Insert(Nota n)
        {
            Connection.Insert(n);
        }

        public void Update(Nota n)
        {
            Connection.Update(n);
        }

        public void Delete(Nota n)
        {
            Connection.Delete(n);
        }

        public void InsertOrReplace(Nota n)
        {
            Nota nota = Connection.Find<Nota>(n.Id);
            if (nota == null)
            {
                if (n.Titulo != null) Connection.Insert(n);
            }
            else if (!string.IsNullOrEmpty(n.Titulo) && !string.IsNullOrEmpty(n.Descripcion))
            {
                nota.Titulo = n.Titulo;
                nota.Descripcion = n.Descripcion;
                Connection.Update(nota);
            }
            else
            {
                Connection.Delete(nota);
            }
        }

    }
}
