using System;
using System.Linq;

namespace Salomon.Common.Helper
{
    public static class StringExtensions
    {
        /// <summary>
        /// like oracle translate function.
        ///  example:  "abc".Translate("abc","123") => "123"
        /// </summary>
        /// <param name="source">source characters</param>
        /// <param name="target">target characters</param>
        /// <returns></returns>
        public static string Translate(this string original, string source, string target)
        {
            if (source.Length != target.Length) throw new ApplicationException($"parameters aren't the same length");

            Func<char, char> xx = (w) =>
            {
                var idx = source.IndexOf(w);
                return idx > -1 ? target[idx] : w;
            };

            return String.Concat(original.Select(xx));
        }

        /// <summary>
        ///  friendly string conversion to integer
        /// </summary>
        /// <param name="base">numeric base (default 10)</param>
        /// <returns></returns>
        public static int ToInt(this string source, int @base = 10)
        {

            if (source.Empty()) return default(Int32);
            return Convert.ToInt32(source, @base);
        }

        /// <summary>
        ///  checks if the string contains at least one character other than white space
        /// </summary>
        /// <returns>true if string not empty</returns>
        public static bool Present(this string source)
        {
            return !source.Empty();
        }

        /// <summary>
        /// checks if the string contains only white space or is null
        /// </summary>
        /// <returns>true if string is empty</returns>
        public static bool Empty(this string source)
        {
            return String.IsNullOrEmpty(source) || String.IsNullOrWhiteSpace(source);
        }


    }

}
