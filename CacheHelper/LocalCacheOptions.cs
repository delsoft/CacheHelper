using System;

namespace Salomon.Common.Helper
{
    public class LocalCacheOptions
    {
        public String TagName { get; set; } = "default";

        public TimeSpan Timeout { get; set; } = TimeSpan.FromDays(1);
    }

}
