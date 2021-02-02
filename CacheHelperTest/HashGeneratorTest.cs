using Salomon.Common.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CommonHelperTest
{
    public class HashgeneratorTest
    {

        [Fact]
        public void should_return_different_hash_codes_for_different_objects()
        {
            var a = new { Name = "abc" };
            var b = new { Name = "abD" };

            var ha = HashGenerator.Calc(a);
            var hb = HashGenerator.Calc(b);

            Assert.NotEqual(ha, hb);
        }

        [Fact]
        public void given_the_same_object_should_return_the_same_hash_code()
        {
            var a = new { Name = "abc" };
            var b = new { Name = "abc" };

            var ha = HashGenerator.Calc(a);
            var hb = HashGenerator.Calc(b);

            Assert.Equal(ha, hb);
        }

        [Fact]
        public void given_an_unordered_object_list_should_return_the_same_hash_code()
        {
            var a = new { Name = "abc" };
            var b = new { Value = 123 };
            var c = new { Nome = "XXX!", Value = 999 };

            var g = new HashGenerator();
            var x = g.ComputeHash(b, a, c);

            var h = g.ComputeHash(b, a, c);

            Assert.Equal(x, h);

            h = g.ComputeHash(a, b, c);
            Assert.Equal(x, h);

            h = g.ComputeHash(a, c, b);
            Assert.Equal(x, h);

        }


    }
}
