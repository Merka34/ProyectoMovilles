using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NotasNotificaciones.Droid
{
    public interface IToast
    {
        void EnviarMensaje(string mensaje);
    }
}