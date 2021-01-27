using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Salomon.Common.Helper
{

    public static class TaskHelper
    {
        #region Mutex
        public static void Mutex(Object object0, string resourceName, string tag, Action method)
        {
            var f = Path.GetFileName(FileHelper.ObjectCacheFilename(object0.GetType()).Split('.').First());
            f += '_' + resourceName;
            Mutex(f, 60000, tag, method);
        }

        public static void Mutex(string mutexName, string tag, Action method)
        {
            Mutex(mutexName, 60000, tag, method);
        }

        public static void Mutex(string mutexName, int millisecondsTimeout, string tag, Action method)
        {

            var mtx = new Mutex(false, mutexName);

            if (!mtx.WaitOne(millisecondsTimeout))
            {
                if (tag != null) tag = $"'{tag}' ";
                throw new ApplicationException($"{tag}mutex timeout {millisecondsTimeout}");
            }
            try
            {
                method.Invoke();
            }
            finally
            {
                mtx.ReleaseMutex();
            }
        }

        public static Mutex EnterMutex(string mutexName, int millisecondsTimeout)
        {
            var mtx = new Mutex(false, mutexName);
            if (!mtx.WaitOne(millisecondsTimeout))
                throw new ApplicationException($"mutex timeout {millisecondsTimeout}");
            return mtx;
        }

        #endregion

        #region Semaphore
        public static void Semaphore(string semaphoreName, int maxCount, int millisecondsTimeout, string tag, Action method)
        {
         
            var mtx = new Semaphore(maxCount, maxCount, semaphoreName);

            if (!mtx.WaitOne(millisecondsTimeout))
            {
                if (tag != null) tag = $"'{tag}' ";
                throw new ApplicationException($"{tag} semaphore timeout {millisecondsTimeout}");
            }

            try
            {
                method.Invoke();
            }
            finally
            {
                mtx.Release();
            }

          
        }

        public static Semaphore EnterSemaphore(string semaphoreName, int maxCount, int millisecondsTimeout)
        {
            var mtx = new Semaphore(maxCount, maxCount, semaphoreName);

            if (!mtx.WaitOne(millisecondsTimeout))
                throw new ApplicationException($"mutex timeout {millisecondsTimeout}");
            return mtx;
        }
        #endregion
    }

}
