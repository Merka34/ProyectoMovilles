using CelularesApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CelularesApp.Models
{
    public class CatalogoCelular
    {

        public SQLiteConnection Connection { get; set; }
        public CatalogoCelular()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/celulares.db3";
            Connection = new SQLiteConnection(path);
            Connection.CreateTable<Celular>();
        }

        public IEnumerable<Celular> GetAll()
        {
            return Connection.Table<Celular>().OrderBy(x => x.Marca);
        }

        public void Insert(Celular c)
        {
            Connection.Insert(c);
        }

        public void Update(Celular c)
        {
            Connection.Update(c);
        }

        public void Delete(Celular c)
        {
            Connection.Delete(c);
        }

        public IEnumerable<Celular> GetPersonaById(string nombre)
        {

            // string query = "SELECT * FROM medicamentos1 WHERE nombre_medicamento LIKE '%' + @nombre + '%'";
            // SqlCommand consulta = new SqlCommand(query, conexion);
            var x = GetAll();

            var coincidencias = (from cel in x
                                 where cel.Marca.ToUpper().Contains(nombre)
                                 select new Celular()
                                 {
                                     Id = cel.Id,
                                     Marca = cel.Marca,
                                     Red = cel.Red,
                                     Modelo = cel.Modelo,
                                     Precio = cel.Precio
                                 }).ToList();
            return coincidencias;
        }

        public void InsertOrReplace(Celular c)
        {
            Celular celular = Connection.Find<Celular>(c.Id);
            if (celular == null)
            {
                if (c.Marca != null) Connection.Insert(c);
            }
            else if (!string.IsNullOrEmpty(c.Marca) && !string.IsNullOrEmpty(c.Modelo))
            {
                celular.Marca = c.Marca;
                celular.MemoriaRam = c.MemoriaRam;
                celular.Modelo = c.Modelo;
                celular.Red = c.Red;
                celular.Precio = c.Precio;
                Connection.Update(celular);
            }
            else
            {
                Connection.Delete(celular);
            }
        }

    }
}
