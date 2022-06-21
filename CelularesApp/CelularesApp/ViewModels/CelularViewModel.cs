using CelularesApp.Models;
using CelularesApp.Services;
using CelularesApp.Views;
using MarcTron.Plugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace CelularesApp.ViewModels
{
    public class CelularViewModel : INotifyPropertyChanged
    {
        public int MostrarAnuncio { get; set; }
        private bool _recompensaDisponible = false;

        public bool RecompensaDisponible
        {
            get { return _recompensaDisponible; }
            set { _recompensaDisponible = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecompensaDisponible))); }
        }

        private Celular _celular;
        public Celular Celular
        {
            get { return _celular; }
            set
            {
                _celular = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Celular)));
            }
        }

        private string _celularesBusqueda = "";
        public string CelularesBusqueda
        {
            get { return _celularesBusqueda; }
            set
            {
                _celularesBusqueda = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CelularesBusqueda)));
            }
        }

        AgregarCelularView agregarView;
        EditarCelularView editarView;
        public event PropertyChangedEventHandler PropertyChanged;
        private List<Error> _errores;

        public List<Error> Errores
        {
            get { return _errores; }
            set
            {
                _errores = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Errores)));
            }
        }


        //Models
        public ObservableCollection<Celular> Celulares { get; set; } = new ObservableCollection<Celular>();

        //Commands
        public Command VistaAgregarCommand { get; set; }
        public Command VistaEditarCommand { get; set; }
        public Command AgregarCommand { get; set; }
        public Command BusquedaCelularCommand { get; set; }
        public Command RecompensarCommand { get; set; }
        public Command EditarCommand { get; set; }
        public Command EliminarCommand { get; set; }
        public Command CancelarCommand { get; set; }

        //Actions
        public CelularViewModel()
        {
            MostrarAnuncio = 0;
            AgregarCommand = new Command(Agregar);
            EditarCommand = new Command(Editar);
            RecompensarCommand = new Command(Recompensar);
            EliminarCommand = new Command(Eliminar);
            BusquedaCelularCommand = new Command(BuscarCelulares);
            VistaAgregarCommand = new Command(VerAgregarAsync);
            VistaEditarCommand = new Command(VerEditarAsync);
            CrossMTAdmob.Current.LoadInterstitial("ca-app-pub-9712565769296684/4383820109");
            CrossMTAdmob.Current.LoadRewardedVideo("ca-app-pub-9712565769296684/4961037622");
            SincronizadorService.ActualizacionRealizada += SincronizadorService_ActualizacionRealizada;
            SincronizadorService_ActualizacionRealizada();
            CrossMTAdmob.Current.OnRewardedVideoAdLoaded += Current_OnRewardedVideoAdLoaded;
            CrossMTAdmob.Current.OnRewardedVideoAdCompleted += Current_OnRewardedVideoAdCompleted;
            CrossMTAdmob.Current.OnRewardedVideoAdClosed += Current_OnRewardedVideoAdClosed;
        }

        private void Current_OnRewardedVideoAdLoaded(object sender, EventArgs e)
        {
            RecompensaDisponible = true;
        }

        private void Current_OnRewardedVideoAdClosed(object sender, EventArgs e)
        {
            if (!CrossMTAdmob.Current.IsRewardedVideoLoaded())
            {
                RecompensaDisponible = false;
            }
        }

        private void Current_OnRewardedVideoAdCompleted(object sender, EventArgs e)
        {
            
            if (!CrossMTAdmob.Current.IsRewardedVideoLoaded())
            {
                CrossMTAdmob.Current.LoadRewardedVideo("ca-app-pub-9712565769296684/4961037622");
                RecompensaDisponible = false;
            }
        }

        private void Recompensar(object obj)
        {
           if(CrossMTAdmob.Current.IsRewardedVideoLoaded())
                CrossMTAdmob.Current.ShowRewardedVideo();
            
        }

        private async void VerEditarAsync(object d)
        {
            if (editarView == null) editarView = new EditarCelularView() { BindingContext = this };
            MostrarAnuncio++;
            if (CrossMTAdmob.Current.IsInterstitialLoaded()&&MostrarAnuncio==3)
            {
                MostrarAnuncio = 0;
                CrossMTAdmob.Current.ShowInterstitial();
                CrossMTAdmob.Current.LoadInterstitial("ca-app-pub-9712565769296684/4383820109");
            }
                

            Celular = d as Celular;
            Errores = null;
            await Application.Current.MainPage.Navigation.PushAsync(editarView);
            CrossMTAdmob.Current.LoadInterstitial("ca-app-pub-9712565769296684/4383820109");
        }

        private async void VerAgregarAsync()
        {
            if (agregarView == null) agregarView = new AgregarCelularView() { BindingContext = this };
            MostrarAnuncio++;
            if(CrossMTAdmob.Current.IsInterstitialLoaded()&&MostrarAnuncio==3)
            {
                MostrarAnuncio = 0;
                CrossMTAdmob.Current.ShowInterstitial();
                CrossMTAdmob.Current.LoadInterstitial("ca-app-pub-9712565769296684/4383820109");
            }

            this.Celular = new Celular();
            Errores = null;
            await Application.Current.MainPage.Navigation.PushAsync(agregarView);
        }

        private async void Agregar()
        {
            var result = await App.Sincronizador.Agregar(Celular);
            if (result == null)
            {
                await Application.Current.MainPage.Navigation.PopAsync();
                CrossMTAdmob.Current.LoadInterstitial("ca-app-pub-9712565769296684/4383820109");
            }
            else
            {
                Errores = result.Select(x => new Error { Message = x }).ToList();
            }
        }

        private async void Editar()
        {
            var result = await App.Sincronizador.Editar(Celular);

            if (result == null)
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                Errores = result.Select(x => new Error { Message = x }).ToList();
            }
        }

        private async void Eliminar(object d)
        {
            var celular = d as Celular;
            var x = await Application.Current.MainPage.DisplayAlert("Confirme:",
                $"¿Está seguro de eliminar '{celular.Marca} {celular.Modelo}'?", "Si", "No");
            if (x)
            {
                var result = await App.Sincronizador.Eliminar(celular);

                if (result != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Ha ocurrido un error:",
                        String.Join("\n", result), "Aceptar"

                        );
                }
            }
        }

        private async void BuscarCelulares()
        {
            var p = App.Catalogo.GetPersonaById(CelularesBusqueda.ToUpper());
            if (p.Count() >= 1)
            {
                Celulares.Clear();
                foreach (var item in p)
                {
                    Celulares.Add(item);
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Atención 🚫", $"No se encontraron resultados mediante '{CelularesBusqueda}'", "OK");
            }
        }

        private void SincronizadorService_ActualizacionRealizada()
        {
            List<Celular> celulares = App.Catalogo.GetAll().ToList();

            foreach (var buf in App.Sincronizador.Buffer)
            {
                if (buf.Estado == Estado.Agregar)
                {
                    celulares.Add(buf.Celular);
                }
                else if (buf.Estado == Estado.Editar)
                {
                    var c = celulares.FirstOrDefault(x => x.Id == buf.Celular.Id);
                    if (c != null)
                    {
                        c.Marca = buf.Celular.Marca;
                        c.Modelo = buf.Celular.Modelo;
                        c.MemoriaRam = buf.Celular.MemoriaRam;
                        c.Precio = buf.Celular.Precio;
                        c.Red = buf.Celular.Red;
                    }
                }
                else
                {
                    var d = celulares.FirstOrDefault(x => x.Id == buf.Celular.Id);
                    if (d != null)
                    {
                        celulares.Remove(d);
                    }
                }
            }
            Celulares.Clear();
            foreach (Celular celular in celulares.OrderBy(x => x.Marca))
            {
               Celulares.Add(celular);
            }
        }
    }
}
