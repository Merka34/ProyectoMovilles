using System;
using System.Collections.Generic;


namespace ProyectoMovilles.Models
{
    public partial class Perfil
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string NoControl { get; set; }
        public string Carrera { get; set; }
    }
}
