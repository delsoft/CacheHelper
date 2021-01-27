using System;
using System.Collections.Generic;
using System.Text;

namespace Salomon.Common.Helper.Utils
{
    public class TagName
    {

        public TagName(string tagName)
        {
            Value = tagName;
        }

        public string Value { get; private set; }

        public static implicit operator TagName(string tagName)
        {
            return new TagName(tagName);
        }

        public static implicit operator String(TagName tagName)
        {
            return tagName.Value;
        }
    }
}
