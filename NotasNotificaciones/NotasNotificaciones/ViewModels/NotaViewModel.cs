using NotasNotificaciones.Droid;
using NotasNotificaciones.Models;
using NotasNotificaciones.Services;
using NotasNotificaciones.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Extensions;

namespace NotasNotificaciones.ViewModels
{
    public class NotaViewModel : INotifyPropertyChanged
    {
        private Nota _nota;
        IToast Mensaje;
        public Nota Nota
        {
            get { return _nota; }
            set
            {
                _nota = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nota)));
            }
        }
        AgregarNotaView agregarView;
        EditarNotaView editarView;
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
        public ObservableCollection<Nota> Notas { get; set; } = new ObservableCollection<Nota>();

        //Commands
        public Command VistaAgregarCommand { get; set; }
        public Command VistaEditarCommand { get; set; }
        public Command AgregarCommand { get; set; }
        public Command EditarCommand { get; set; }
        public Command EliminarCommand { get; set; }
        public Command CancelarCommand { get; set; }

        //Actions
        public NotaViewModel()
        {
            AgregarCommand = new Command(Agregar);
            EditarCommand = new Command(Editar);
            EliminarCommand = new Command(Eliminar);
            VistaAgregarCommand = new Command(VerAgregarAsync);
            VistaEditarCommand = new Command(VerEditarAsync);
            SincronizadorService.ActualizacionRealizada += SincronizadorService_ActualizacionRealizada;
            SincronizadorService_ActualizacionRealizada();
            Mensaje = DependencyService.Get<IToast>();
        }

        private async void VerEditarAsync(object d)
        {
            if (editarView == null) editarView = new EditarNotaView() { BindingContext = this };

            Nota = d as Nota;
            Errores = null;
            await Application.Current.MainPage.Navigation.PushAsync(editarView);
        }

        private async void VerAgregarAsync()
        {
            if (agregarView == null) agregarView = new AgregarNotaView() { BindingContext = this };

            this.Nota = new Nota();
            Errores = null;
            await Application.Current.MainPage.Navigation.PushAsync(agregarView);
        }

        private async void Agregar()
        {
            var result = await App.Sincronizador.Agregar(Nota);
            if (result == null)
            {
                //Mensaje.EnviarMensaje($"Nota {Nota.Titulo} agregada");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                Errores = result.Select(x => new Error { Message = x }).ToList();
            }
        }

        private async void Editar()
        {
            var result = await App.Sincronizador.Editar(Nota);

            if (result == null)
            {
                Mensaje.EnviarMensaje($"Nota {Nota.Titulo} editada");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                Errores = result.Select(x => new Error { Message = x }).ToList();
            }
        }

        private async void Eliminar(object d)
        {
            var nota = d as Nota;
            var x = await Application.Current.MainPage.DisplayAlert("Confirme:",
                $"¿Está seguro de eliminar '{nota.Titulo} ?", "Si", "No");
            if (x)
            {
                var result = await App.Sincronizador.Eliminar(nota);

                if (result != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Ha ocurrido un error:",
                        String.Join("\n", result), "Aceptar"

                        );
                }
                else
                {
                    Mensaje.EnviarMensaje($"Nota {nota.Titulo} eliminada");
                }
            }

        }

        private void SincronizadorService_ActualizacionRealizada()
        {
            List<Nota> celulares = App.Catalogo.GetAll().ToList();

            foreach (var buf in App.Sincronizador.Buffer)
            {
                if (buf.Estado == Estado.Agregar)
                {
                    celulares.Add(buf.Nota);
                }
                else if (buf.Estado == Estado.Editar)
                {
                    var c = celulares.FirstOrDefault(x => x.Id == buf.Nota.Id);
                    if (c != null)
                    {
                        c.Titulo = buf.Nota.Titulo;
                        c.Descripcion = buf.Nota.Descripcion;
                    }
                }
                else
                {
                    var d = celulares.FirstOrDefault(x => x.Id == buf.Nota.Id);
                    if (d != null)
                    {
                        celulares.Remove(d);
                    }
                }
            }
            Notas.Clear();
            foreach (Nota celular in celulares.OrderBy(x => x.Titulo))
            {
                Notas.Add(celular);
            }
        }
    }
}
