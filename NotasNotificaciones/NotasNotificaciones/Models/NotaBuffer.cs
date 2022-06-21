using System;
using System.Collections.Generic;
using System.Text;

namespace NotasNotificaciones.Models
{
    public enum Estado { Agregar, Editar, Eliminar }
    public class NotaBuffer
    {
        public Nota Nota { get; set; }
        public Estado Estado { get; set; }
    }
}
