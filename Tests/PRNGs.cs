using System;
using System.Linq;
using Woof.Algorithms;
using Xunit;
using Xunit.Abstractions;

namespace Woof.Core.Tests {

    public class PRNGs {

        //public PRNGs(ITestOutputHelper testOutputHelper) => Output = testOutputHelper;

        //private readonly ITestOutputHelper Output;

        [/*Fun*/Fact]
        public void SysRandom_Next() => Next(new SysRandom());

        [/*Fun*/Fact]
        public void XorShift32_Next() => Next(new XorShift32());

        [/*Fun*/Fact]
        public void XorShift32T_Next() => Next(new XorShift32T());

        [/*Fun*/Fact]
        public void SysRandom_NextDouble() => NextDouble(new SysRandom());

        [/*Fun*/Fact]
        public void XorShif32_NextDouble() => NextDouble(new XorShift32());

        [/*Fun*/Fact]
        public void XorShift32T_NextDouble() => NextDouble(new XorShift32T());

        [/*Fun*/Fact]
        public void SysRandom_NextIntMax() => NextIntMax(new SysRandom());

        [/*Fun*/Fact]
        public void XorShif32_NextIntMax() => NextIntMax(new XorShift32());

        [/*Fun*/Fact]
        public void XorShift32T_NextIntMax() => NextIntMax(new XorShift32T());

        [/*Fun*/Fact]
        public void SysRandom_NextIntMinMax() => NextIntMinMax(new SysRandom());

        [/*Fun*/Fact]
        public void XorShif32_NextIntMinMax() => NextIntMinMax(new XorShift32());

        [/*Fun*/Fact]
        public void XorShift32T_NextIntMinMax() => NextIntMinMax(new XorShift32T());

        [/*Fun*/Fact]
        public void SysRandom_NextBytes() => NextBytes(new SysRandom());

        [/*Fun*/Fact]
        public void XorShif32_NextBytes() => NextBytes(new XorShift32());

        [/*Fun*/Fact]
        public void XorShift32T_NextBytes() => NextBytes(new XorShift32T());

        [/*Fun*/Fact]
        public void SysRandom_BoxMuller() => BoxMuller(new SysRandom());

        [/*Fun*/Fact]
        public void XorShift32_BoxMuller() => BoxMuller(new XorShift32());

        [/*Fun*/Fact]
        public void XorShift32T_BoxMuller() => BoxMuller(new XorShift32T());


        private void BoxMuller(IPseudoRandomNumberGenerator prng) {
            var sampleSize = 128000;
            {
                var u = new double[sampleSize];
                var n = new double[sampleSize];
                for (int i = 0; i < sampleSize; i++) {
                    u[i] = prng.NextDouble();
                    n[i] = prng.NextNormal();
                }
                var uMin = u.Min();
                var uAvg = u.Average();
                var uMax = u.Max();
                var min = n.Min();
                var avg = n.Average();
                var max = n.Max();
                Assert.InRange(Math.Abs(min - uMin), 0.0, 0.2);
                Assert.InRange(Math.Abs(avg - uAvg), 0.0, 0.01);
                Assert.InRange(Math.Abs(max - uMax), 0.0, 0.2);
            }
            {
                var u = new int[sampleSize];
                var n = new int[sampleSize];
                for (int i = 0; i < sampleSize; i++) {
                    u[i] = prng.Next(-1000, 1001);
                    n[i] = prng.NextNormal(-1000, 1001);
                }
                var uMin = u.Min();
                var uAvg = u.Average();
                var uMax = u.Max();
                var min = n.Min();
                var avg = n.Average();
                var max = n.Max();
                Assert.InRange(Math.Abs(min - uMin), 0, 1);
                Assert.InRange(Math.Abs(avg - uAvg), 0, 10);
                Assert.InRange(Math.Abs(max - uMax), 0, 1);
            }
        }

        private void NextDouble(IPseudoRandomNumberGenerator prng) {
            const int sampleSize = 256000;
            var sample = new double[sampleSize];
            for (int i = 0; i < sampleSize; i++) sample[i] = prng.NextDouble();
            var min = sample.Min();
            var max = sample.Max();
            var avg = sample.Average();
            Assert.InRange(min, 0, 0.0001);
            Assert.InRange(max, 0.9999, 1);
            Assert.InRange(avg, 0.49, 0.51);
        }

        private void NextIntMax(IPseudoRandomNumberGenerator prng) {
            const int sampleSize = 256000;
            var sample = new int[sampleSize];
            for (int i = 0; i < sampleSize; i++) sample[i] = prng.Next(1001);
            var min = sample.Min();
            var max = sample.Max();
            var avg = sample.Average();
            Assert.Equal(0, min);
            Assert.InRange(avg, 490, 510);
            Assert.Equal(1000, max);
        }

        private void NextIntMinMax(IPseudoRandomNumberGenerator prng) {
            const int sampleSize = 256000;
            var sample = new int[sampleSize];
            for (int i = 0; i < sampleSize; i++) sample[i] = prng.Next(-1000, 1001);
            var min = sample.Min();
            var max = sample.Max();
            var avg = sample.Average();
            Assert.Equal(-1000, min);
            Assert.InRange(avg, -10, 10);
            Assert.Equal(1000, max);
        }

        private void NextBytes(IPseudoRandomNumberGenerator prng) {
            const int sampleSize = 256000;
            var sample = new byte[sampleSize];
            prng.NextBytes(sample);
            var counter = new int[256];
            foreach (var b in sample) counter[b]++;
            foreach (var c in counter) Assert.InRange(c, 850, 1150);
        }

        private void Next(IPseudoRandomNumberGenerator prng) {
            const int sampleSize = 256000;
            var sample = new int[sampleSize];
            for (int i = 0; i < sampleSize; i++) sample[i] = prng.Next();
            var min = sample.Min();
            var max = sample.Max();
            Assert.True(min >= 0);
            Assert.True(max > sampleSize);
        }



    }

}