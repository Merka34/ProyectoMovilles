using Newtonsoft.Json;
using PerfilApp.Helpers;
using PerfilApp.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PerfilApp.Services
{
    public class UserService
    {

        public async Task<string> GetToken()
        {
            var x = await SecureStorage.GetAsync("MiToken");
            return x;
        }

        public DateTime ExpirationDate { get; set; }

        public bool EstoyAutenticado
        {
            get
            {
                var result = GetToken().Result;

                if (result == null) return false;

                var thandler = new JwtSecurityTokenHandler();
                var descriptor = thandler.ReadJwtToken(result);

                Identity = new ClaimsIdentity(descriptor.Claims);
                ExpirationDate = descriptor.ValidTo;
                var salida = result != null && DateTime.UtcNow < ExpirationDate;
                return salida;
            }
        }


        public ClaimsIdentity Identity { get; set; }

        public async Task<bool> Renovar()
        {
            string usuario = await SecureStorage.GetAsync("User");
            string contra = await SecureStorage.GetAsync("Password");

            if (usuario != null && contra != null)
            {
                App.username = usuario;
                return await IniciarSesion(new SesionModel
                {
                    Usuario = usuario,
                    Contraseña = contra
                });
            }

            return false;
        }


        public async Task<bool> IniciarSesion(SesionModel lm)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://181g0262.81g.itesrc.net/");

            string json = JsonConvert.SerializeObject(lm);

            HttpResponseMessage result = await client.PostAsync("api/login",
                new StringContent(json, Encoding.UTF8, "application/json"));

            SecureStorage.RemoveAll();


            if (result.IsSuccessStatusCode)
            {
                string token = await result.Content.ReadAsStringAsync();
                await SecureStorage.SetAsync("MiToken", token);
                await SecureStorage.SetAsync("User", lm.Usuario);
                await SecureStorage.SetAsync("Password", lm.Contraseña);

                JwtSecurityTokenHandler thandler = new JwtSecurityTokenHandler();
                JwtSecurityToken descriptor = thandler.ReadJwtToken(token);

                Identity = new ClaimsIdentity(descriptor.Claims);
                ExpirationDate = descriptor.ValidTo;

                return true;

            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return false;
            }
            return true;
        }

        public void RedirigirPrincipal()
        {
            if (EstoyAutenticado)
                App.Current.MainPage = new Views.PerfilView();
            else
                App.Current.MainPage = new Views.SesionView();
            
        }

        public void CerrarSesion()
        {
            SecureStorage.RemoveAll();
            Identity = null;
            ExpirationDate = DateTime.MinValue;
            App.Current.MainPage = new Views.SesionView();

        }
    }
}
