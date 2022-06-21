using System;
using System.Collections.Generic;

#nullable disable

namespace ProyectoMovilles.Models
{
    public partial class Dispositivo
    {
        public int Id { get; set; }
        public int? TipoDispositivo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int? MemoriaRam { get; set; }
        public string Procesador { get; set; }
        public string TarjetaGrafica { get; set; }
        public int? CapacidadDisco { get; set; }
        public int? TipoDisco { get; set; }
        public decimal? Precio { get; set; }
        public DateTime? Timestamp { get; set; }
        public ulong Eliminado { get; set; }
    }
}
