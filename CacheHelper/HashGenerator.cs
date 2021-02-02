using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Salomon.Common.Helper
{
    public class HashGenerator
    {
        
        public string ComputeHash(params string[] data)
        {
            var txt = String.Concat(data.OrderBy(p => p).Select(p => $"{p}@"));
            return TxtToHash(txt);

        }

        private string TxtToHash(string txt)
        {
            var dataArray = Encoding.Default.GetBytes(txt);
            var hashValue = hashGenerator.ComputeHash(dataArray);
            var ret = BitConverter.ToString(hashValue).Replace("-", "");
            Trace.WriteLine(ret);
            Trace.WriteLine(txt);
            Trace.WriteLine("=======================");
            return ret;
        }

        public string ComputeHash(params object[] data)
        {
            StringBuilder buffer = new StringBuilder();
            List<string> qq = new List<string>();

            foreach (var item in data)
            {
                buffer.Clear();
                var json = JsonConvert.SerializeObject(item);
                buffer.Append(Normalize(json));
                buffer.Append($"|{json.Length}|");
                var xx = buffer.ToString();
                qq.Add(TxtToHash(xx));
            }

            return ComputeHash(qq.ToArray());

        }

        public static string Calc(params object[] data)
        {
            return (new HashGenerator()).ComputeHash(data);
        }

        private HashAlgorithm hashGenerator = SHA1.Create();

        private char[] Normalize(string data)
        {
            return Regex.Replace(data.ToLower(), "\\ *", " ").Trim().ToCharArray();
        }
    }

}
