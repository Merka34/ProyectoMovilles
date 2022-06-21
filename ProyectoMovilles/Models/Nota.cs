using System;
using System.Collections.Generic;

#nullable disable

namespace ProyectoMovilles.Models
{
    public partial class Nota
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? Timestamp { get; set; }
        public ulong Eliminado { get; set; }
    }
}
