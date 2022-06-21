using NotasNotificaciones.Models;
using NotasNotificaciones.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.FirebasePushNotification;

namespace NotasNotificaciones
{
    public partial class App : Application
    {
        public static CatalogoNota Catalogo { get; set; } = new CatalogoNota();
        public static SincronizadorService Sincronizador { get; set; }
        public static event Action Actualizacion;
        public static void Actualizar()
        {
            Actualizacion?.Invoke();
        }
        public App()
        {
            InitializeComponent();
            Sincronizador = new SincronizadorService(Catalogo);
            MainPage = new NavigationPage(new Views.NotasView());
        }

        private void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {  
           
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
