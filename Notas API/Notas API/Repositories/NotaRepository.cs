using Notas_API.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notas_API.Repositories
{
    public class NotaRepository
    {

        public SQLiteConnection Conexion { get; set; }

        public NotaRepository()
        {
            var ruta = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/notas.db3";
            Conexion = new SQLiteConnection(ruta);

            Conexion.CreateTable<Nota>();
        }

        public IEnumerable<Nota> GetAll()
        {
            return Conexion.Table<Nota>().ToList().OrderBy(x => x.Titulo);
        }


        public void InsertOrReplace(Nota c)
        {
            //Tomar la decisión:

            var contacto = Conexion.Find<Nota>(c.Id);

            //Insertar si no existia el id
            if (contacto == null)
            {
                //pero tiene nombre
                if (c.Titulo != null) Conexion.Insert(c);

                //si no tiene nombre, no hace nada
            }
            else if (c.Titulo != null)
            {
                //y tiene nombre, edito el nombre
                contacto.Titulo = c.Titulo;
                Conexion.Update(contacto);
            }
            else
            {
                //si no tiene nombre, lo elimino
                Conexion.Delete(contacto);
            }
        }

    }
}
