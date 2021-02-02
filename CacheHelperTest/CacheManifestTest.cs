using Newtonsoft.Json;
using Salomon.Common.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CommonHelperTest
{
    namespace jaca.xx.y
    {
        public class MockObject
        {
            public string Name { get; set; }
        }
    }

    public class CacheManifestTest
    {

        [Fact]
        public void should_()
        {
            var xx = new jaca.xx.y.MockObject() { Name = "abc" };
            var qq = new CacheManifestItem(xx);
            var json = JsonConvert.SerializeObject(xx);

            Assert.Equal("commonhelpertest.jaca.xx.y", qq.Namespace);
            Assert.Equal("mockobject", qq.TypeName);
            Assert.Equal(json, qq.Data);
            Assert.Equal(json.Length, qq.Length);
            Assert.NotNull(qq.HashCode);
        }
    }

}
