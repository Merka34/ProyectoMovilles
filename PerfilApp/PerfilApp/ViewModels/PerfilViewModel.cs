using Newtonsoft.Json;
using ProyectoMovilles.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PerfilApp.ViewModels
{
    public class PerfilViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Lista { get; set; } = new ObservableCollection<string>();

        public ICommand CerrarSesionCommand { get; set; }

        private Perfil _perfil;

        public Perfil Perfil
        {
            get { return _perfil; }
            set { _perfil = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Perfil")); }
        }


        public PerfilViewModel()
        {
            CerrarSesionCommand = new Command(CerrarSesion);
            CargarDatos();
        }

        async void CargarDatos()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://181g0262.81g.itesrc.net/");
            //client.BaseAddress = new Uri("https://proyectomovilles.conveyor.cloud/");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await App.User.GetToken()}");
            string usName = await SecureStorage.GetAsync("User");
            var result = await client.GetAsync($"api/perfil?name={usName}");

            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                Perfil = JsonConvert.DeserializeObject<Perfil>(json);
            }
            else
            {
                App.User.CerrarSesion();
            }
        }

        private void CerrarSesion()
        {
            App.User.CerrarSesion();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void Lanzar(string propiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }
    }
}
