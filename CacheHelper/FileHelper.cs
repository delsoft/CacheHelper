using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Salomon.Common.Helper
{
    public class FileHelper
    {
        private static string _convertVirtualPath(string virtualPath, int counter = 5)
        {
            if (counter <= 0) throw new ApplicationException("ConvertVirtualPath stack overflow");

            Match m = null;
            //MatchCollection mc = null;

            //if (virtualPath.Contains("{config:") && (mc = Regex.Matches(virtualPath, @"\{config:(?<key>[^\}]+)\}")).Any())
            //{
            //    foreach (Match mi in mc)
            //    {
            //        var value = ConfigHelper.Instance.MustSetting(mi.Groups["key"].Value);
            //        virtualPath = Regex.Replace(virtualPath, mi.Value, value);
            //        return _convertVirtualPath(virtualPath, counter--);
            //    }

            //}

#if DEBUG
            if (virtualPath.Contains("{build_path}"))
                virtualPath = virtualPath.Replace("{build_path}", "bin\\Debug");
            string stage = "Dev";

#else
            string stage = "Prod";
            if (virtualPath.Contains("{build_path}"))
                virtualPath = virtualPath.Replace("{build_path}", "bin\\Release");

#endif
            //if (virtualPath.Contains("{debug_path}"))
            //    virtualPath = virtualPath.Replace("{debug_path}", SysHelper.DebugPath());
            //if (virtualPath.Contains("{root_path}"))
            //    virtualPath = virtualPath.Replace("{root_path}", SysHelper.RootPath());
            //if (virtualPath.Contains("{exe_path}"))
            //    virtualPath = virtualPath.Replace("{exe_path}", SysHelper.ExePath());
            //if (virtualPath.Contains("{bin_path}"))
            //    virtualPath = virtualPath.Replace("{bin_path}", SysHelper.ExePath());
            //if (virtualPath.Contains("{main_path}"))
            //    virtualPath = virtualPath.Replace("{main_path}", SysHelper.MainRootPath());
            if (virtualPath.Contains("{stage}"))
                virtualPath = virtualPath.Replace("{stage}", stage);
            if ((m = Regex.Match(virtualPath, @"%localappdata%|\{local_app_data\}|\{localappdata\}")).Success)
            {
                string localAppDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                virtualPath = Regex.Replace(virtualPath, m.Value, localAppDataPath);
            }
            if ((m = Regex.Match(virtualPath, @"%appdata%|\{app_data\}|\{appdata\}")).Success)
            {
                string appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                virtualPath = Regex.Replace(virtualPath, m.Value, appData);
            }
            if ((m = Regex.Match(virtualPath, @"%localappdata%|\{local_app_data\}|\{localappdata\}")).Success)
            {
                string appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                virtualPath = Regex.Replace(virtualPath, m.Value, appData);
            }
            if ((m = Regex.Match(virtualPath, @"\{ansi_date\}|\{ansi_date_time\}")).Success)
            {
                string appData = DateTime.Now.ToString("yyyyMMddhhmmss");
                virtualPath = Regex.Replace(virtualPath, m.Value, appData);
            }
            if ((m = Regex.Match(virtualPath, @"\{shortansidatetime\}|\{short_ansi_date_time\}")).Success)
            {
                string appData = DateTime.Now.ToString("yyMMddhhmmss");
                virtualPath = Regex.Replace(virtualPath, m.Value, appData);
            }
            if ((m = Regex.Match(virtualPath, @"\{shortansidate\}|\{short_ansi_date\}")).Success)
            {
                string appData = DateTime.Now.ToString("yyMMdd");
                virtualPath = Regex.Replace(virtualPath, m.Value, appData);
            }
            if ((m = Regex.Match(virtualPath, @"\{(shorttime|short_time|time)\}")).Success)
            {
                string appData = DateTime.Now.ToString("hhmmss");
                virtualPath = Regex.Replace(virtualPath, m.Value, appData);
            }
            if ((m = Regex.Match(virtualPath, @"\{process_id\}|\{processid\}")).Success)
            {
                string appData = $"{Process.GetCurrentProcess().Id}";
                virtualPath = Regex.Replace(virtualPath, m.Value, appData);
            }

            return virtualPath;
        }

        public static string SafePath(string path)
        {
            var s = Path.DirectorySeparatorChar;
            return path.Translate("\\/", $"{s}{s}");
        }

        public static string ConvertVirtualPath(string virtualPath)
        {
            return SafePath(_convertVirtualPath(virtualPath));
        }

        private static string _cachePath = null;

        public static string CachePath(string tag = null)
        {
            if (_cachePath == null)
            {
                var cachePath = "sl_cache\\{stage}";
                var sep = Path.DirectorySeparatorChar;
                _cachePath = ConvertVirtualPath($"{Path.GetTempPath().TrimEnd(sep)}\\{cachePath}\\");
            }
            return _cachePath;
        }

        public static string ObjectCacheFilename(Type type, string tag = null)
        {
            var cachePath = "sl_cache\\{stage}";

            var sep = Path.DirectorySeparatorChar;
            var path = type.FullName.Replace('.', sep).ToLower();
            //path = Regex.Replace(path, "[\\[,\\]]", "_");
            return ConvertVirtualPath($"{Path.GetTempPath().TrimEnd(sep)}\\{cachePath}\\{path}{tag}._ch");
        }

        public static T LoadFromGlobalCache<T>(string tag = null)
        {
            T ret = default(T);
            var f = ObjectCacheFilename(typeof(T), tag);
            if (File.Exists(f))
            {
                var mtx = typeof(T).FullName;
                TaskHelper.Mutex(mtx, "FileHelper#LoadFronGlobalCache", () =>
                {
                    ret = LoadFromFile<T>(f);
                });
            }

            return ret;
        }

        public static T LoadFromFile<T>(string filename)
        {
            //var json = File.ReadAllText(FileHelper.ConvertVirtualPath(filename));
            var json = LoadFromFile(filename);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string LoadFromFile(string filename)
        {

            return File.ReadAllText(FileHelper.ConvertVirtualPath(filename));
        }

        public static void SaveToGlobalCache(object Object)
        {
            SaveToGlobalCache(Object, null, null);
        }

        public static void SaveToGlobalCache(object Object, string tag = null, Action refreshAction = null)
        {
            var mtx = Object.GetType().FullName;
            TaskHelper.Mutex(mtx, $"FileHelper#SaveToGlobalCache_{mtx}_{tag}", () =>
            {
                var fl = ObjectCacheFilename(Object.GetType(), tag);
                var path = Path.GetDirectoryName(fl);
                Directory.CreateDirectory(path);
                refreshAction?.Invoke();
                SaveToFile(Object, fl);
            });
        }

        public static void SaveToFile(object Object, string filename)
        {
            string json = null;

            json = JsonConvert.SerializeObject(Object);

            SaveToFile(json, filename);
        }

        public static void SaveToFile(string data, string filename)
        {
            using (var f = new StreamWriter(FileHelper.ConvertVirtualPath(filename)))
                f.Write(data);

        }
    }

}
