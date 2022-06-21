using SQLite;
using System;
using System.Collections.Generic;

namespace CelularesApp.Models
{
    [Table("Celular")]
    public class Celular
    {
        [PrimaryKey,NotNull]
        public int Id { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public string Red { get; set; }
        public int MemoriaRam { get; set; }
        public decimal Precio { get; set; }
    }
}
