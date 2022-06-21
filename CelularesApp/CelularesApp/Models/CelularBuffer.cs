using System;
using System.Collections.Generic;
using System.Text;

namespace CelularesApp.Models
{
    public enum Estado { Agregar, Editar, Eliminar }
    public class CelularBuffer
    {
        public Celular Celular { get; set; }
        public Estado Estado { get; set; }

    }

}
