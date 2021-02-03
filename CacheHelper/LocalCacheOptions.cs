using System;

namespace Salomon.Common.Helper
{    
    public enum ApplicationStage
    {
        Development,
        Production
    }

    public class LocalCacheOptions
    {
        public String TagName { get; set; } = "default";

        public TimeSpan Timeout { get; set; } = TimeSpan.FromDays(1);

    }

    public class LocalCacheOptionsEx : LocalCacheOptions
    {
        public ApplicationStage Stage { get; set; } = ApplicationStage.Development;

    }

}
