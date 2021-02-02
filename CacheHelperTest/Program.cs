using Newtonsoft.Json;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace CacheHelperTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var a = SHA1.Create();
            var t = Encoding.Default.GetBytes("abc");
            var x = a.ComputeHash(t);

            var b = Convert.ToBase64String(x);


        }
    }
}
