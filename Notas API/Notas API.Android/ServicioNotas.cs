using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using Firebase.Messaging;
using Notas_API.Models;
using Notas_API.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(Notas_API.Droid.ServicioNotas))]

namespace Notas_API.Droid
{
    [Service(Exported = false)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class ServicioNotas
    {/*
        public override void OnMessageReceived(RemoteMessage p0)
        {
            Log.Info("SERVICIO CHAT", "RECIBIO MENSAJE");
            try
            {
                NotaRepository repository =
                    new NotaRepository();

                //Log.Info("SERVICIO CHAT", "INYECCION");
                Nota c = null;

                if (p0.Data != null)
                {
                    var datos = p0.Data;

                    if (datos["Accion"] == "Agregar")
                    {
                        Log.Info("SERVICIO CHAT", "AGREGAR");

                        c = new Nota()
                        {
                            Titulo = p0.Data["Titulo"],
                            Descripcion = datos["Descripcion"],
                            Timestamp =
                           DateTime.ParseExact(datos["Timestamp"],
                           "dd/MM/yyyy hh:mm:ss tt",
                           new CultureInfo("es-MX"))
                        };

                        repository.Insert(c);
                    }
                    else if (datos["Accion"] == "Eliminar")
                    {

                        Log.Info("SERVICIO CHAT", "ELIMINAR");

                        var id = int.Parse(datos["Id"]);
                        var m = repository.Get(id);

                        if (m != null)
                        {
                            m.Mensaje = "";
                            m.Eliminado = true;
                            repository.Update(m);
                        }
                    }
                    else if (datos["Accion"] == "Editar")
                    {
                        Log.Info("SERVICIO CHAT", "Editar");

                        var id = int.Parse(datos["Id"]);
                        var m = repository.Get(id);

                        if (m != null)
                        {
                            m.Mensaje = p0.Data["Mensaje"];
                            m.Fecha =
                            DateTime.ParseExact(datos["Fecha"],
                            "dd/MM/yyyy hh:mm:ss tt",
                            new CultureInfo("es-MX"));
                            m.Editado = true;
                            repository.Update(m);
                        }
                    }

                    if (App.Current == null)
                    {
                        if (c != null)
                        {
                            ShowNotification(c.Id, c.Remitente, c.Mensaje);
                        }
                    }
                    else
                    {
                        App.Actualizar();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error("SERVICIO CHAT", ex.Message);
            }

            Log.Info("SERVICIO CHAT", "PROCESO EL MENSAJE");

            base.OnMessageReceived(p0);
        }
        */

        public static string NombreCanal = "PRUEBANOTIFICACIONES";
        public void ShowNotification(string title, string text)
        {
            NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context,
                NombreCanal)
                .SetContentTitle(title)
                .SetContentText(text)
                .SetPriority(NotificationCompat.PriorityMax)
                .SetShowWhen(true)
                .SetSmallIcon(Resource.Drawable.ic_mtrl_checked_circle);

            NotificationManager manager = Application.Context.GetSystemService(Application.NotificationService)
                as NotificationManager;

            manager.Notify(1, builder.Build());

        }
    }
}