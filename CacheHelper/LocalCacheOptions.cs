using System;

namespace Salomon.Common.Helper
{
    [Flags]
    public enum ApplicationStage
    {
        Debug,
        Development,
        Test,
        Homolog,
        Production
    }

    public class LocalCacheOptions
    {
        public String TagName { get; set; } = "default";

        public TimeSpan Timeout { get; set; } = TimeSpan.FromDays(1);

    }

    public class LocalCacheOptionsEx : LocalCacheOptions
    {
        public ApplicationStage Stage { get; set; } = ApplicationStage.Debug;

    }

}
