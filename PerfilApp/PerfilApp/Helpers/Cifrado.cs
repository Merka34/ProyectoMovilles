using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PerfilApp.Helpers
{
    public static class Cifrado
    {
        public static string GetHash(string cadena)
        {
            SHA512 algoritmo = SHA512.Create();
            byte[] arreglo = Encoding.UTF8.GetBytes(cadena);
            byte[] hash = algoritmo.ComputeHash(arreglo, 0, arreglo.Length);
            string key = Encoding.UTF8.GetString(hash);
            //return string.Join("", hash);
            return key;
        }
    }
}
