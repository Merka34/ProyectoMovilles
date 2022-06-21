using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using NotasNotificaciones.Services;
using Firebase.Messaging;
using NotasNotificaciones.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Google.Android.Material.Snackbar;

[assembly: Xamarin.Forms.Dependency(typeof(NotasNotificaciones.Droid.ClaseNotificaciones))]

namespace NotasNotificaciones.Droid
{
    [Service(Exported = false)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class ClaseNotificaciones : FirebaseMessagingService, IToast
    {
        public static string NombreCanal = "PRUEBANOTIFICACIONES";
        public void EnviarMensaje(string mensaje)
        {
            Toast toast = Toast.MakeText(Android.App.Application.Context, mensaje, ToastLength.Long);
            toast.Show();
        }

        public override void OnMessageReceived(RemoteMessage p0)
        {
            Log.Info("NOTIFICACIONES", "RECIBIO MENSAJE");
            /*if (foregrounded())
            {
                if (p0.Data != null)
                {
                    var datos = p0.Data;
                    if (datos["Accion"] == "Agregar")
                    {
                        Log.Info("NOTIFICACIONES", "AGREGAR");
                        EnviarMensaje("Una nueva nota a sido agregada");
                    }
                    else if (datos["Accion"] == "Eliminar")
                    {
                        Log.Info("SERVICIO CHAT", "ELIMINAR");
                        EnviarMensaje("Se elimino una nota");
                    }
                    else if (datos["Accion"] == "Editar")
                    {
                        Log.Info("SERVICIO CHAT", "Editar");
                        EnviarMensaje("Una nota a sido editada");
                    }
                }
                App.Actualizar();
                base.OnMessageReceived(p0);
            }
            else*/
            {
                if (p0.Data != null)
                {
                    var datos = p0.Data;
                    if (datos["Accion"] == "Agregar")
                    {
                        Log.Info("NOTIFICACIONES", "AGREGAR");
                        ShowNotification("Nota Nueva", "Una nueva nota a sido agregada");
                    }
                    else if (datos["Accion"] == "Eliminar")
                    {
                        Log.Info("SERVICIO CHAT", "ELIMINAR");
                        ShowNotification("Nota Eliminada", "Se elimino una nota");
                    }
                    else if (datos["Accion"] == "Editar")
                    {
                        Log.Info("SERVICIO CHAT", "Editar");
                        ShowNotification("Nota Editada", "Una nota a sido editada");
                    }
                }
                App.Actualizar();
                base.OnMessageReceived(p0);

            }
        }

        public bool foregrounded()
        {
            ActivityManager.RunningAppProcessInfo appProcessInfo = new ActivityManager.RunningAppProcessInfo();
            ActivityManager.GetMyMemoryState(appProcessInfo);
            return (appProcessInfo.Importance == Importance.Foreground);
        }


        public void ShowNotification(string title, string text)
        {
            NotificationCompat.Builder builder = new NotificationCompat.Builder(Android.App.Application.Context,
                NombreCanal)
                .SetContentTitle(title)
                .SetContentText(text)
                .SetPriority(NotificationCompat.PriorityMax)
                .SetShowWhen(true)
                .SetSmallIcon(NotasNotificaciones.Droid.Resource.Drawable.material_ic_keyboard_arrow_right_black_24dp);

            NotificationManager manager = Android.App.Application.Context.GetSystemService(Android.App.Application.NotificationService)
                as NotificationManager;

            manager.Notify(1, builder.Build());

        }
    }
}