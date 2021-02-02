using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salomon.Common.Helper
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class POCO<T>
    {
        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        public T FromJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public override bool Equals(object obj)
        {
            if (obj is POCO<T>)
                return this.ToJSON() == (obj as POCO<T>).ToJSON();
            else
                return base.Equals(obj);
        }

    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
