using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Salomon.Common.Helper
{

    public class CacheManifestItem
    {

        public CacheManifestItem(object @object)
        {
            if (@object == null) return;

            var xx = @object.GetType();
            Namespace = xx.Namespace.ToLower();
            TypeName = xx.Name.ToLower();
            Data = JsonConvert.SerializeObject(@object);
            HashCode = HashGenerator.Calc(@object, Length);
        }

        public string HashCode { get; private set; }

        public string Namespace { get; private set; }

        public string TypeName { get; private set; }

        public string Data { get; private set; }

        public int Length => Data?.Length ?? 0;
        
    }


}
