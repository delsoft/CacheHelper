using Salomon.Common.Helper;
using System;
using System.Threading.Tasks;
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


        [Fact]
        public void should_invalidate_cache()
        {
            var time = 1000;
            var opt = new LocalCacheOptions
            {
                TagName = "invalidate_test",
                Timeout = TimeSpan.FromMilliseconds(time)
            };

            var cache = new LocalCache<string>(opt);
            var objA = "abc";
            
            // clear cache
            cache.Clear();
            Assert.False(cache.Valid);
            
            // save a object to cache
            cache.Save(objA);
            Assert.True(cache.Valid);

            // load same object from cache
            var objB = cache.Load();

            // wait cache invalidate
            Task.Delay(time).Wait();

            // check cache is invalid
            Assert.False(cache.Valid);

            // load object from out of date cache
            var objC = cache.Load();

            //  Data from cache is OK
            Assert.Equal(objA, objB);
            Assert.Equal(objB, objC);
        }
    }
}
