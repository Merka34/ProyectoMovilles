using System;
using System.Collections.Generic;

#nullable disable

namespace ProyectoMovilles.Models
{
    public partial class Celular
    {
        public int Id { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public string Red { get; set; }
        public int? MemoriaRam { get; set; }
        public decimal? Precio { get; set; }
        public DateTime? Timestamp { get; set; }
        public ulong Eliminado { get; set; }
    }
}
