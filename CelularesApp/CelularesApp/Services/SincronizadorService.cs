using CelularesApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CelularesApp.Services
{
    public class SincronizadorService
    {
        public static event Action ActualizacionRealizada;

        public List<CelularBuffer> Buffer { get; set; }

        static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("https://181g0262.81g.itesrc.net/")
        };

        public CatalogoCelular Catalogo { get; set; }

        public SincronizadorService(CatalogoCelular c)
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            Catalogo = c;
            DeserializarBuffer();
            if (!Preferences.ContainsKey("LastUpdateDate"))
            {
                _ = DescargarPrimeraVezAsync();
                Sincronizar();
            }
            else
            {
                Sincronizar();
            }
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            await Descargar();
        }
        public bool isCargarBuffer { get; set; } = false;
        async Task CargarBuffer()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                isCargarBuffer = true;
                if (Buffer.Count > 0)
                {
                    foreach (var disBuf in Buffer.ToArray())
                    {
                        switch (disBuf.Estado)
                        {
                            case Estado.Agregar:
                                var result = await EnviarAPI(disBuf.Celular, HttpMethod.Post);
                                if (result == null) Buffer.Remove(disBuf);
                                break;
                            case Estado.Editar:
                                await EnviarAPI(disBuf.Celular, HttpMethod.Put);
                                Buffer.Remove(disBuf);
                                break;
                            case Estado.Eliminar:
                                await EnviarAPI(disBuf.Celular, HttpMethod.Delete);
                                Buffer.Remove(disBuf);
                                break;
                        }
                    }
                    SerializarBuffer();
                }
                isCargarBuffer = false;
            }
        }

        void SerializarBuffer()
        {
            var json = JsonConvert.SerializeObject(Buffer);
            var ruta = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/buffer.json";
            System.IO.File.WriteAllText(ruta, json);
        }

        void DeserializarBuffer()
        {
            var ruta = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/buffer.json";
            try
            {
                if (System.IO.File.Exists(ruta))
                {
                    var json = System.IO.File.ReadAllText(ruta);
                    Buffer = JsonConvert.DeserializeObject<List<CelularBuffer>>(json);
                }
                else
                {
                    Buffer = new List<CelularBuffer>();
                }
            }
            catch
            {
                Buffer = new List<CelularBuffer>();
            }
        }

        async Task DescargarPrimeraVezAsync()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                DateTime date = DateTime.Now;
                var result = await client.GetAsync("api/celulares");
                if (result.IsSuccessStatusCode)
                {
                    Preferences.Set("LastUpdateDate", date);
                    string json = await result.Content.ReadAsStringAsync();
                    List<Celular> celulares = JsonConvert.DeserializeObject<List<Celular>>(json);
                    celulares.ForEach(x => Catalogo.InsertOrReplace(x));
                    if (celulares.Count > 0)
                        LanzarEvento();
                }
            }
        }

        private void LanzarEvento()
        {
            Xamarin.Forms.Application.Current.Dispatcher.BeginInvokeOnMainThread(() =>
            {
                ActualizacionRealizada?.Invoke();
            });
        }

        Thread tS;
        private void Sincronizar()
        {
            tS = new Thread(new ThreadStart(threadSincronizar));
            tS.Start();
        }

        private async void threadSincronizar()
        {
            while (true)
            {
                await Descargar();
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        private async Task Descargar()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet && Preferences.ContainsKey("LastUpdateDate"))
            {
                if (!isCargarBuffer)
                    await CargarBuffer();

                string json = JsonConvert.SerializeObject(
                    Preferences.Get("LastUpdateDate", DateTime.MinValue));
                HttpRequestMessage httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(client.BaseAddress.AbsoluteUri + "api/celulares/sincronizar"),
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                Task<HttpResponseMessage> response = client.SendAsync(httpRequest);
                response.Wait();
                if (response.Result.IsSuccessStatusCode)
                {
                    Task<string> datos = response.Result.Content.ReadAsStringAsync();
                    datos.Wait();
                    List<CelularEliminarModel> celularesNull = JsonConvert.DeserializeObject<List<CelularEliminarModel>>(datos.Result);
                    List<Celular> celulares = new List<Celular>();
                    foreach (CelularEliminarModel item in celularesNull)
                    {
                        Celular cel = new Celular();
                        if (!string.IsNullOrWhiteSpace(item.Precio) && item.Precio != "(null)")
                            cel.Precio = int.Parse(item.Precio);
                        if (!string.IsNullOrWhiteSpace(item.MemoriaRam) && item.MemoriaRam != "(null)")
                            cel.MemoriaRam = int.Parse(item.MemoriaRam);
                        cel.Id = item.Id;
                        cel.Marca = item.Marca;
                        cel.Red = item.Red;
                        cel.Modelo = item.Modelo;
                        celulares.Add(cel);
                    }
                    foreach (Celular celular in celulares)
                    {
                        Catalogo.InsertOrReplace(celular);
                    }
                    if (celulares.Count > 0)
                        LanzarEvento();
                }
            }
        }

        private async Task<List<string>> EnviarAPI(Celular c, HttpMethod method)
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
                    request.RequestUri = new Uri(client.BaseAddress + "api/celulares");
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
                errores.Add("No hay conexión a internet.");
                return null;
            }
        }

        public async Task<List<string>> Agregar(Celular c)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                return await EnviarAPI(c, HttpMethod.Post);
            }
            else
            {
                CelularBuffer disBuf = new CelularBuffer();
                disBuf.Celular = c;
                disBuf.Estado = Estado.Agregar;
                Buffer.Add(disBuf);
                SerializarBuffer();
                LanzarEvento();
                return null;
            }
        }

        public async Task<List<string>> Editar(Celular d)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                return await EnviarAPI(d, HttpMethod.Put);
            }
            else
            {
                CelularBuffer disBuf = new CelularBuffer();
                disBuf.Celular = d;
                disBuf.Estado = Estado.Editar;
                Buffer.Add(disBuf);
                SerializarBuffer();
                LanzarEvento();
                return null;
            }
        }

        public async Task<List<string>> Eliminar(Celular d)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                return await EnviarAPI(d, HttpMethod.Delete);
            }
            else
            {
                CelularBuffer disBuf = new CelularBuffer();
                disBuf.Celular = d;
                disBuf.Estado = Estado.Eliminar;
                Buffer.Add(disBuf);
                SerializarBuffer();
                LanzarEvento();
                return null;
            }
        }

        class CelularEliminarModel
        {
            public int Id { get; set; }
            public string Marca { get; set; }
            public string Modelo { get; set; }
            public string Red { get; set; }
            public string MemoriaRam { get; set; }
            public string Precio { get; set; }
        }
    }
}
