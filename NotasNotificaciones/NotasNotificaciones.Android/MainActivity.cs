using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Gms.Extensions;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Common;
using Xamarin.Essentials;

namespace NotasNotificaciones.Droid
{
    [Activity(Label = "NotasNotificaciones", Icon = "@mipmap/icon", Theme = "@style/MainTheme", 
        MainLauncher = false,
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            RegistrarmeATema();
            
            NotificationManager manager = Application.Context.GetSystemService(Application.NotificationService)
               as NotificationManager;

            var channel = new NotificationChannel(ClaseNotificaciones.NombreCanal,
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
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
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