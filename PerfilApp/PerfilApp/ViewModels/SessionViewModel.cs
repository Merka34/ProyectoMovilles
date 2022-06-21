using PerfilApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PerfilApp.ViewModels
{
    public class SessionViewModel : INotifyPropertyChanged
    {
        public SesionModel SessionModel { get; set; }
        public bool Indicador { get; set; }
        public string Error { get; set; }
        public ICommand IniciarSesionCommand { get; set; }
        

        public SessionViewModel()
        {
            SessionModel = new SesionModel();
            IniciarSesionCommand = new Command(IniciarSesion);
        }

            

        private async void IniciarSesion()
        {
            Indicador = true;
            if (string.IsNullOrWhiteSpace(SessionModel.Usuario) || string.IsNullOrWhiteSpace(SessionModel.Contraseña))
            {
                Error = "Por favor ingrese su nombre de usuario y la constraseña para iniciar sesion";
            }
            else
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    Error = "No hay conexión a Internet, por favor verifique su conectiidad";
                }
                else
                {
                    var result = await App.User.IniciarSesion(SessionModel);

                    if (!result)
                    {
                        Error = "El nombre de usuario o la constraseña son erroneas, por favor verifique";
                    }
                    else
                    {
                        App.username = SessionModel.Usuario;
                        App.User.RedirigirPrincipal();
                    }
                }
                
            }
            Indicador = false;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

