using Salomon.Common.Helper.Utils;
using System;
using System.IO;

namespace Salomon.Common.Helper
{
    public class LocalCacheOptions
    {
        public TagName TagName { get; set; } = "default";

        public string DataMask { get; set; } = "yyMMdd";
    }

    public class LocalCache<TOwner> //: ISubordinated<TOwner>
    {
        #region private 
        //private static string _dataMask = "yyMMdd";
        private string _filename = null;

        private string Filename => _filename ?? (_filename = FileHelper.ObjectCacheFilename(typeof(TOwner), Options.TagName));
        private int FileVersion => Version(File.GetLastWriteTimeUtc(Filename));
        private int CurrentVersion => Version(DateTime.UtcNow);

        private int Version(DateTime datetime)
        {
            return datetime.ToString(Options.DataMask).ToInt();
        }

        private bool Valid => CurrentVersion == FileVersion;

        #endregion

        #region ctor
        public LocalCache(LocalCacheOptions options=null)
        {
            this.Options = options ?? new LocalCacheOptions();
        }

        //public LocalCache(string dataMask, TagName tagName = null) : this(tagName)
        //{
        //    _dataMask = dataMask;
        //}

        private LocalCacheOptions Options { get; }

        #endregion

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
        public TOwner Fetch(Func<TOwner> method) {
            return Fetch(this.Options, method);
        }


    }

}
