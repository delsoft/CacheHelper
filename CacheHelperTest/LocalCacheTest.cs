﻿using Newtonsoft.Json;
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
            var tmpa = stringCache.Fetch(() =>
            {
                return "jkl";
            });

            var tmp = (new LocalCache<string>()).Load();

            Assert.Equal(tmpa, tmp);
        }

        [Fact]
        public void should_cache_lists()
        {

            var objs = new MockList()
            {
                new MockCacheableClass{  Data = "123"},
                new MockCacheableClass{  Data = "456"}

            };

            var cache = new LocalCache<MockList>();
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
            var cache1 = new LocalCache<MockCacheableClass, MockCacheFilter>(filter1);
            var cache2 = new LocalCache<MockCacheableClass, MockCacheFilter>(filter2);

            // prepare caches
            cache1.Clear();
            cache2.Clear();
            
            
            cache1.Save(obj1);

            var data1 = cache1.Load();
            var data2 = cache2.Load();
            
            Assert.NotNull(data1);
            Assert.Equal(data1,obj1);
            
            Assert.Null(data2);

            //  save second data and filter
            cache2.Save(obj2);

            data1 = cache1.Load();
            data2 = cache2.Load();

            Assert.NotNull(data1);
            Assert.NotNull(data2);
            Assert.Equal(data1, obj1);
            Assert.Equal(data2, obj2);
            Assert.NotEqual(data2, data1);


        }
    }
}
