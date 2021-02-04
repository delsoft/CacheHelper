using Newtonsoft.Json;
using Salomon.Common.Helper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace CommonTest
{
    public class MockList : List<MockCacheableClass>
    {

    }

    public class MockCacheableClass : POCO<MockCacheableClass>
    {
        public string Data { get; set; }
    }

    public class MockCacheFilter
    {
        public string FilterName { get; set; }
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
            
            stringCache.Clear();
            var xx = false;
            var dt = "jkl";
            var tmpa = stringCache.Fetch(() =>
            {
                xx = true;
                return dt;
            });

            //var tmp = (new LocalCache<string>()).Load();
            Assert.True(xx);
            Assert.Equal(tmpa, dt);
        }

        [Fact]
        public void should_cache_lists()
        {

            var objs = new MockList()
            {
                new MockCacheableClass{  Data = "123"},
                new MockCacheableClass{  Data = "456"}

            };

            var cache = LocalCache.Invoke<MockList>();
            var tmpa = cache.Fetch(() =>
            {
                return objs;
            });

            var tmp = (new LocalCache<MockList>()).Load();

            Assert.Equal(JsonConvert.SerializeObject(tmpa), JsonConvert.SerializeObject(tmp));
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


        [Fact]
        public void should_cache_with_filters()
        {
            var filter1 = new MockCacheFilter
            {
                FilterName = "João"
            };

            var obj1 = new MockCacheableClass
            {
                Data = "abc"
            };

            var filter2 = new MockCacheFilter
            {
                FilterName = "João 2"
            };

            var obj2 = new MockCacheableClass
            {
                Data = "def"
            };

            // create caches
            var cache1 = LocalCache.Invoke<MockCacheableClass>(filter1);
            var cache2 = new LocalCache<MockCacheableClass>(filter2);

            // prepare caches
            cache1.Clear();
            cache2.Clear();
            
            var data1 = cache1.Fetch(() => obj1);
            var data2 = cache2.Fetch(()=> obj2);
            
            Assert.NotEqual(data2, data1);

        }
    }
}
