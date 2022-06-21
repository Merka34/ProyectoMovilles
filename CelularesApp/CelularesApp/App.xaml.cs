using CelularesApp.Models;
using CelularesApp.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CelularesApp
{
    public partial class App : Application
    {
        
        public static CatalogoCelular Catalogo { get; set; } = new CatalogoCelular();
        public static SincronizadorService Sincronizador { get; set; }

        public App()
        {
            InitializeComponent();
            Sincronizador = new SincronizadorService(Catalogo);
            MainPage = new NavigationPage(new Views.CelularesView());
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
