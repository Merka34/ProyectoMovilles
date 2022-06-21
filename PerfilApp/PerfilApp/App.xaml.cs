using PerfilApp.Services;
using PerfilApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PerfilApp
{
    public partial class App : Application
    {
        public static string username { get; set; }
        public static UserService User { get; private set; } = new UserService();

        public App()
        {
            User.RedirigirPrincipal();
            /*
            InitializeComponent();
            if (User.EstoyAutenticado || User.Renovar().Result)
            {
                User.RedirigirPrincipal();
            }
            else
            {
                MainPage = new SesionView();
            }*/
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
