using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using HashCode = Woof.Algorithms.HashCode;

namespace Woof.Core.Tests {

    public class Others {

        [/*Fun*/Fact]
        public void HashCodes() {
            var rng = new Random();
            char randomChar() => (char)rng.Next(32, 126);
            string randomString() {
                var _b = new StringBuilder();
                for (int i = 0, n = rng.Next(1, 16); i < n; i++) _b.Append(randomChar());
                return _b.ToString();
            }
            string[] randomStringArray(int count) {
                var list = new List<string>();
                for (int i = 0; i < count; i++) list.Add(randomString());
                return list.ToArray();
            }
            static string[] copyStringArray(string[] original) => original.Select(i => i).ToArray();
            {
                var a = randomStringArray(255);
                var b = copyStringArray(a);
                var c = randomStringArray(255);
                int h1 = new HashCode().GetFromCollection(a);
                int h2 = new HashCode().GetFromCollection(b);
                int h3 = new HashCode().GetFromCollection(c);
                Assert.Equal(h1, h2);
                Assert.NotEqual(h1, h3);
            }
            {
                var x = randomStringArray(255);
                var y = randomStringArray(255);
                var a = new Tuple<string[], string[]>(x, y);
                var b = new Tuple<string[], string[]>(copyStringArray(x), copyStringArray(y));
                var c = new Tuple<string[], string[]>(x, copyStringArray(x));
                var h1 = new HashCode().GetFromComponents(a.Item1, a.Item2);
                var h2 = new HashCode().GetFromComponents(b.Item1, b.Item2);
                var h3 = new HashCode().GetFromComponents(c.Item1, c.Item2);
                Assert.Equal(h1, h2);
                Assert.NotEqual(h1, h3);
            }
        }

    }

}