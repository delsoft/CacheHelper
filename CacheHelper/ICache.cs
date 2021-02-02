using System;

namespace Salomon.Common.Helper
{
    public interface ICache
    {
        bool Valid { get; }

        void Save(object obj);

        object Load();

        object Fetch(Func<object> method);

        void Clear();
    }

}
