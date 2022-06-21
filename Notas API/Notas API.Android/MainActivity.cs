using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Firebase.Messaging;
using Android.Gms.Extensions;

namespace Notas_API.Droid
{
    [Activity(Label = "Notas_API", Icon = "@mipmap/icon", Theme = "@style/MainTheme", 
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static Activity CurrentActivity { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            CurrentActivity = this;
            RegistrarmeATema();

            NotificationManager manager = Application.Context.GetSystemService(Application.NotificationService)
               as NotificationManager;


            var channel = new NotificationChannel(ServicioNotas.NombreCanal,
                          "Canal de Pruebas de Notificaciones Locales", NotificationImportance.Default);

            channel.SetAllowBubbles(true);
            channel.SetShowBadge(true);
            channel.EnableLights(true);
            channel.EnableVibration(true);

            manager.CreateNotificationChannel(channel);

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        async void RegistrarmeATema()
        {
            await Firebase.Messaging.FirebaseMessaging.Instance.SubscribeToTopic("nota");
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}