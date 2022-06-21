using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notas_API.Models
{
    public class Nota
    {
        [PrimaryKey,NotNull]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
