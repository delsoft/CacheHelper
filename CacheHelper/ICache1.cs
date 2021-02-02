using System;

namespace Salomon.Common.Helper
{
    public interface ICache<TOwner> : ICache
    {

        void Save(TOwner obj);
        
        new TOwner Load();

        TOwner Fetch(Func<TOwner> method);

    }

}
