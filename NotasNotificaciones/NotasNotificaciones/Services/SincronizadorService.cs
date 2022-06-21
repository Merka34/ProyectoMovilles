using Newtonsoft.Json;
using NotasNotificaciones.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NotasNotificaciones.Services
{
    public class SincronizadorService
    {
        public static event Action ActualizacionRealizada;

        public List<NotaBuffer> Buffer { get; set; }



        static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("https://181g0262.81g.itesrc.net/")
        };

        public CatalogoNota Catalogo { get; set; }

        public SincronizadorService(CatalogoNota c)
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            Catalogo = c;
            App.Actualizacion += App_Actualizacion;
            DeserializarBuffer();

            //Verificar si ya se ha descargado algo antes

            if (!Preferences.ContainsKey("FechaUltimaActualizacion"))
            { //Si no: Verifico si tengo conexion y descargo toda la bd, inicio el hilo de sincronizacion

                _ = DescargarPrimeraVezAsync();
                Sincronizar();
            }
            else
            {
                Sincronizar();
                //Si: inicio el hilo de sincronización
            }
        }

        private async void App_Actualizacion()
        {
            await Descargar();
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {

            await Descargar();

        }

        async Task DescargarPrimeraVezAsync()
        {
            //Verificar si existe conexión a internet
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var fecha = DateTime.Now;
                var result = await client.GetAsync("api/notas");
                if (result.IsSuccessStatusCode)
                {
                    Preferences.Set("FechaUltimaActualizacion", fecha);
                    string json = await result.Content.ReadAsStringAsync();
                    List<Nota> contactos = JsonConvert.DeserializeObject<List<Nota>>(json);
                    contactos.ForEach(x => Catalogo.InsertOrReplace(x));
                    if (contactos.Count > 0)
                        LanzarEvento();
                }

            }

        }

        //Thread hs;
        async void Sincronizar()
        {
            await Descargar();
            //     hs = new Thread(new ThreadStart(hiloSincronizador));
            //     hs.Start();
        }

        async void hiloSincronizador()
        {
            while (true)
            {

                await Descargar();

         //       Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        async Task CargarBuffer()
        {

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {

                //si hay algo en el buffer
                if (Buffer.Count > 0)
                {
                    Console.WriteLine("CargarBuffer Iniciado");

                    foreach (var ce in Buffer.ToArray())
                    {
                        switch (ce.Estado)
                        {
                            case Estado.Agregar:
                                var result = await EnviarAPI(ce.Nota, HttpMethod.Post);
                                //manejar el error
                                if (result != null) Buffer.Remove(ce);

                                break;
                            case Estado.Editar:
                                await EnviarAPI(ce.Nota, HttpMethod.Put);
                                //manejar el error
                                Buffer.Remove(ce);
                                break;
                            case Estado.Eliminar:
                                await EnviarAPI(ce.Nota, HttpMethod.Delete);
                                //manejar el error
                                Buffer.Remove(ce);
                                break;
                        }
                    }

                    SerializarBuffer();

                    Console.WriteLine("CargarBuffer Terminado");

                }


            }


        }
        private async Task Descargar()
        {

            if (Connectivity.NetworkAccess == NetworkAccess.Internet
                && Preferences.ContainsKey("FechaUltimaActualizacion")
                )
            {
                await CargarBuffer();

                Console.WriteLine("Descargar Iniciado");


                var json = JsonConvert.SerializeObject(
                Preferences.Get("FechaUltimaActualizacion",
                DateTime.MinValue));

                var fecha = DateTime.Now;


                var response = await client.PostAsync("api/notas/sincronizar",
                    new StringContent(json, Encoding.UTF8, "application/json"));


                if (response.IsSuccessStatusCode)
                {
                    var datos = await response.Content.ReadAsStringAsync();

                    List<Nota> lista = JsonConvert.DeserializeObject<List<Nota>>(datos);

                    foreach (var item in lista)
                    {
                        Catalogo.InsertOrReplace(item);
                    }

                    Preferences.Set("FechaUltimaActualizacion", fecha);

                    if (lista.Count > 0)
                        LanzarEvento();

                }

                Console.WriteLine("Descargar Terminado");


            }

        }

        public async Task<List<string>> Agregar(Nota c)
        {
            //Validar
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                return await EnviarAPI(c, HttpMethod.Post);
            }
            else
            {
                NotaBuffer ce = new NotaBuffer();
                ce.Nota = c;
                ce.Estado = Estado.Agregar;

                Buffer.Add(ce);
                SerializarBuffer();

                LanzarEvento();

                return null;
            }
        }

        public async Task<List<string>> Editar(Nota c)
        {
            //Validar
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                return await EnviarAPI(c, HttpMethod.Put);
            }
            else
            {
                NotaBuffer ce = new NotaBuffer();
                ce.Nota = c;
                ce.Estado = Estado.Editar;

                Buffer.Add(ce);
                SerializarBuffer();
                LanzarEvento();
                return null;
            }
        }

        public async Task<List<string>> Eliminar(Nota c)
        {
            //Validar
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                return await EnviarAPI(c, HttpMethod.Delete);
            }
            else
            {
                NotaBuffer ce = new NotaBuffer();
                ce.Nota = c;
                ce.Estado = Estado.Eliminar;

                Buffer.Add(ce);
                SerializarBuffer();
                LanzarEvento();
                return null;
            }
        }

        private async Task<List<string>> EnviarAPI(Nota c, HttpMethod method)
        {
            List<string> errores = new List<string>();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    //Serializar el contacto y mandarlo a la api
                    string json = JsonConvert.SerializeObject(c);

                    HttpRequestMessage request = new HttpRequestMessage();
                    request.Method = method;
                    request.RequestUri = new Uri(client.BaseAddress + "api/notas");
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");


                    var result = await client.SendAsync(request);

                    //Forzar actualizar, si fue exitoso
                    if (result.IsSuccessStatusCode)
                    {
                        await Descargar();
                        return null;
                    }
                    //Si no fue exitoso, agregar errores
                    if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        json = await result.Content.ReadAsStringAsync();
                        var lista = JsonConvert.DeserializeObject<List<string>>(json);
                        return lista;
                    }

                    if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        errores.Add("No se ha encontrado el contacto indicado.");
                        return errores;
                    }

                    if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        json = await result.Content.ReadAsStringAsync();
                        var mensaje = JsonConvert.DeserializeObject<string>(json);
                        errores.Add(mensaje);
                        return errores;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    errores.Add(ex.Message);
                    return errores;
                }

            }
            else
            {
                //trabajo desconectado
                errores.Add("No hay conexión a internet.");
                return null;
            }


        }


        private void LanzarEvento()
        {
            Xamarin.Forms.Application.Current.Dispatcher.BeginInvokeOnMainThread(() =>
            {
                ActualizacionRealizada?.Invoke();
            });
        }

        void SerializarBuffer()
        {
            var json = JsonConvert.SerializeObject(Buffer);
            var ruta = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/buffer.json";
            File.WriteAllText(ruta, json);
        }

        void DeserializarBuffer()
        {
            var ruta = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/buffer.json";
            try
            {
                if (File.Exists(ruta))
                {
                    var json = File.ReadAllText(ruta);
                    Buffer = JsonConvert.DeserializeObject<List<NotaBuffer>>(json);
                }
                else
                {
                    Buffer = new List<NotaBuffer>();
                }
            }
            catch
            {
                Buffer = new List<NotaBuffer>();
            }
        }
    }
}
