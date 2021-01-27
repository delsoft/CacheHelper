using Salomon.Common.Helper;
using Xunit;


namespace CommonTest
{
    public class MockCacheableClass
    {
        //    private LocalCache<MockCacheableClass> _cache = null;

        //    public void Save() { Cache.Save(this); }
        //    public MockCacheableClass Load() { return Cache.Load(); }

        //    public LocalCache<MockCacheableClass> Cache => _cache ?? (_cache = new LocalCache<MockCacheableClass>());

        public string Data { get; set; }

    }

    public class LocalCacheTest
    {
        [Fact]
        public void should_cache_object_locally()
        {
            var cache = new LocalCache<MockCacheableClass>();
            var obj = cache.Load() ?? new MockCacheableClass();

            obj.Data = "jaca";
            cache.Save(obj);

            var cache2 = new LocalCache<MockCacheableClass>();
            var obj2 = cache2.Load();

            Assert.Equal(obj.Data, obj2.Data);

        }

        [Fact]
        public void should_cache_string_locally()
        {
            var cache = new LocalCache<string>();
            var abc = cache.Load() ?? "abc";

            abc = abc + 'd';

            cache.Save(abc);

            var abc2 = cache.Load();

            Assert.Equal(abc, abc2);
        }
        
        [Fact]
        public void should_use_lambda_cache()
        {
            var stringCache = new LocalCache<string>();
            var tmpa = stringCache.Fetch(() => {
                return "jkl";
            });

            var tmp = (new LocalCache<string>()).Load();

            Assert.Equal(tmpa,tmp);
        }
    }
}
