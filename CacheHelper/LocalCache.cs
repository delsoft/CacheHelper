﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Salomon.Common.Helper
{

    public class LocalCache<TOwner> //: ISubordinated<TOwner>
    {
        #region private 

        private CacheManifest _manifest;

        private string Filename => Manifest.Filename;

        private bool GetValid()
        {
            var fileVer = FileVersion;
            return DateTime.Compare(fileVer.Add(Options.Timeout), DateTime.UtcNow) > 0 && fileVer.Year > 1970;
        }

        private DateTime FileVersion => File.GetLastWriteTimeUtc(Filename);

        private CacheManifest GetManifest()
        {
            return new CacheManifest(Filter, typeof(TOwner));
        }

        private TOwner _fetch(LocalCacheOptions options, Func<TOwner> method)
        {
            var cache = this; 

            if (cache.Valid) return cache.Load();

            var data = method.Invoke();
            cache.Save(data);

            return data;
        }
        
        private object Filter { get; set; }

        #endregion

        #region protected

        protected LocalCacheOptions Options { get; }

        protected CacheManifest Manifest => _manifest ?? (_manifest = GetManifest());

        #endregion

        #region ctor
        public LocalCache(LocalCacheOptions options = null)
        {
            this.Options = options ?? new LocalCacheOptions();
        }

        public LocalCache(object filter, LocalCacheOptions options = null) : this(options)
        {
            this.Filter = filter;
        }

        #endregion

        #region public
        
        /// <summary>
        ///  return true if cache is up to date
        /// </summary>
        public bool Valid => GetValid();

        /// <summary>
        ///  Save object to local cache
        /// </summary>
        /// <param name="obj"></param>
        public void Save(TOwner data)
        {

            var path = Path.GetDirectoryName(Filename);
            Directory.CreateDirectory(path);

            FileHelper.SaveToFile(data, Filename);
        }

        /// <summary>
        /// Load object from local cache
        /// </summary>
        /// <returns></returns>
        public TOwner Load()
        {
            if (!File.Exists(Filename)) return default(TOwner);
            return FileHelper.LoadFromFile<TOwner>(Filename);
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
        public TOwner Fetch(LocalCacheOptions options, Func<TOwner> method)
        {
            var stage = (options as LocalCacheOptionsEx)?.Stage ?? ApplicationStage.Development;
            
            ApplicationStage currentStage = default(ApplicationStage);

            if (System.Diagnostics.Debugger.IsAttached)
                currentStage = ApplicationStage.Development;
            else
                currentStage = ApplicationStage.Production;

            if (stage == currentStage)
                return _fetch(options, method);
            else
                return method.Invoke();
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
        #endregion

    }

    public class LocalCache
    {

        public static LocalCache<T> Invoke<T>()
        {
            return new LocalCache<T>();
        }

        public static LocalCache<T> Invoke<T>(object filter)
        {
            return new LocalCache<T>(filter);
        }

        public static void ClearAll()
        {
            var fn = FileHelper.CachePath();
            Directory.Delete(fn, true);
        }
    }
}
