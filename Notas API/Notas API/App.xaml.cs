using Microsoft.Extensions.DependencyInjection;
using Notas_API.Repositories;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notas_API
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        void SetupServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<NotaRepository>();
            ServiceProvider = services.BuildServiceProvider();

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
