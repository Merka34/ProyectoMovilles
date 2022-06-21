using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Notas_API.Helpers
{

    public class HttpClientHelper<T> where T : class
    {
        public HttpClient HttpClient { get; private set; }
        public Uri Uri { get; }

        public HttpClientHelper(Uri uri)
        {
            Uri = uri;
            HttpClient = new HttpClient();
        }

        async Task Send(HttpMethod method, T model)
        {
            var json = JsonConvert.SerializeObject(model);
            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                Method = method,
                RequestUri = Uri,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await HttpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                await ThrowException(response);
                return;
            }
        }

        private static async Task ThrowException(HttpResponseMessage response)
        {
            if (response.Content.Headers.ContentLength > 0)
            {
                var json = await response.Content.ReadAsStringAsync();
                object o = JsonConvert.DeserializeObject(json);

                throw new HttpException(response.StatusCode, o);
            }

            else
            {
                throw new HttpException(response.StatusCode);
            }
        }

        public async Task Post(T model)
        {
            await Send(HttpMethod.Post, model);
        }

        public async Task Put(T model)
        {
            await Send(HttpMethod.Put, model);
        }

        public async Task Delete(T Model)
        {
            await Send(HttpMethod.Delete, Model);
        }

        public async Task Delete(object id)
        {
            var response = await HttpClient.DeleteAsync(Uri + "/" + id);

            await ThrowException(response);
        }

        public async Task<T> Get(object id)
        {
            var response = await HttpClient.GetAsync(Uri + "/" + id);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                return default;
            }

        }

        public async Task<IEnumerable<T>> Get()
        {
            HttpRequestMessage rm = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = Uri
            };

            var response = await HttpClient.SendAsync(rm);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<T>>(json);
            }
            else
            {
                await ThrowException(response);
            }
            return null;

        }

    }

    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public HttpException(HttpStatusCode statusCode, object @object)
        {
            StatusCode = statusCode;
            Object = @object;
        }

        public HttpException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public object Object { get; set; }
    }
}
