using System;

namespace Salomon.Common.Helper
{
    public class CacheManifest
    {
        private CacheManifestItem Filter = null;
        private CacheManifestItem Data = null;
        private string[] Search(string @namespace, string typeName)
        {
            return new string[] { Filter.HashCode, @namespace.ToLower(), typeName.ToLower() };
        }

        #region ctor
        public CacheManifest(object filter, Type data)
        {
            Filter = new CacheManifestItem(filter);

            HashCode = HashGenerator.Calc(Search(data.Namespace, data.Name));

            Filename = FileHelper.CachePath() + HashCode + "._ch";
        }

        public CacheManifest(object data) : this(null, data)
        {

        }

        public CacheManifest(object filter, object data): this(filter, data?.GetType())
        {
        }

        #endregion

        public string HashCode { get; private set; }

        public string Filename { get; private set; }
    }



}
