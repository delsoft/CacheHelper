using System;
using System.IO;

namespace Salomon.Common.Helper
{
    public class LocalCache<TOwner> //: ISubordinated<TOwner>
    {
        #region private 
        private string _filename = null;

        private string Filename => _filename ?? (_filename = FileHelper.ObjectCacheFilename(typeof(TOwner), Options.TagName));
        private DateTime FileVersion => File.GetLastWriteTimeUtc(Filename);
        private LocalCacheOptions Options { get; }

        private bool GetValid()
        {
            var fileVer = FileVersion;
            return DateTime.Compare(fileVer.Add(Options.Timeout), DateTime.UtcNow) > 0 && fileVer.Year > 1970;
        }

        #endregion

        #region ctor
        public LocalCache(LocalCacheOptions options = null)
        {
            this.Options = options ?? new LocalCacheOptions();
        }



        #endregion

        /// <summary>
        ///  return true if cache is up to date
        /// </summary>
        public bool Valid => GetValid();


        //protected virtual TagName TagName { get; private set; } = null;
        /// <summary>
        ///  Save object to local cache
        /// </summary>
        /// <param name="obj"></param>
        public void Save(object obj)
        {
            //  RowId = CurrentRowId();
            FileHelper.SaveToGlobalCache(obj, Options.TagName);
        }

        /// <summary>
        /// Load object from local cache
        /// </summary>
        /// <returns></returns>
        public TOwner Load()
        {
            return FileHelper.LoadFromGlobalCache<TOwner>(Options.TagName);
        }

        /// <summary>
        ///   clear local cache
        /// </summary>
        public void Clear()
        {
            if (File.Exists(Filename))
                File.Delete(Filename);
        }

        /// <summary>
        /// Fetch data from cache or update them
        /// </summary>
        /// <param name="options">cache options</param>
        /// <param name="method">return the data to be cached</param>
        /// <returns>cached data</returns>
        public static TOwner Fetch(LocalCacheOptions options, Func<TOwner> method)
        {
            var cache = new LocalCache<TOwner>(options);

            if (cache.Valid) return cache.Load();

            var data = method.Invoke();

            cache.Save(data);

            return data;
        }

        /// <summary>
        /// Fetch data from cache or update them
        /// </summary>
        /// <param name="method">return the data to be cached</param>
        /// <returns>cached data</returns>
        public TOwner Fetch(Func<TOwner> method)
        {
            return Fetch(this.Options, method);
        }


    }

}
