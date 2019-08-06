using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Woof.Core;
using Xunit;
using Xunit.Abstractions;
using A = Woof.Algorithms;

public class UnitTests {

    public UnitTests(ITestOutputHelper testOutputHelper) => Output = testOutputHelper;
    private readonly ITestOutputHelper Output;

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
            int h1 = new A.HashCode().GetFromCollection(a);
            int h2 = new A.HashCode().GetFromCollection(b);
            int h3 = new A.HashCode().GetFromCollection(c);
            Assert.Equal(h1, h2);
            Assert.NotEqual(h1, h3);
        }
        {
            var x = randomStringArray(255);
            var y = randomStringArray(255);
            var a = new Tuple<string[], string[]>(x, y);
            var b = new Tuple<string[], string[]>(copyStringArray(x), copyStringArray(y));
            var c = new Tuple<string[], string[]>(x, copyStringArray(x));
            var h1 = new A.HashCode().GetFromComponents(a.Item1, a.Item2);
            var h2 = new A.HashCode().GetFromComponents(b.Item1, b.Item2);
            var h3 = new A.HashCode().GetFromComponents(c.Item1, c.Item2);
            Assert.Equal(h1, h2);
            Assert.NotEqual(h1, h3);
        }
    }

    [/*Fun*/Fact]
    public void PRNG() {
        var generators = new A.IPseudoRandomNumberGenerator[] {
            new A.SysRandom(),
            new A.XorShift32(),
            new A.XorShift32T()
        };
        foreach (var prng in generators) {
            PRNG_NextDouble(prng);
            PRNG_NextBytes(prng);
            PRNG_NextRange(prng);
        }
    }

    private void PRNG_NextDouble(A.IPseudoRandomNumberGenerator prng) {
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

    private void PRNG_NextBytes(A.IPseudoRandomNumberGenerator prng) {
        const int sampleSize = 256000;
        var sample = new byte[sampleSize];
        prng.NextBytes(sample);
        var counter = new int[256];
        foreach (var b in sample) counter[b]++;
        foreach (var c in counter) Assert.InRange(c, 850, 1150);
    }

    private void PRNG_NextRange(A.IPseudoRandomNumberGenerator prng) {
        const int sampleSize = 100000;
        var sample = new int[sampleSize];
        for (int i = 0; i < sampleSize; i++) sample[i] = prng.Next(1, 101);
        var counter = new int[100];
        foreach (var b in sample) counter[b - 1]++;
        foreach (var c in counter) Assert.InRange(c, 850, 1150);
    }

    [/*Fun*/Fact]
    public void ResourceExistence() {
        var assembly = GetType().Assembly;
        var rightPath = "Resources\\Logo.png";
        var wrongPath = "Nowhere\\Logo.png";
        Assert.True(Resource.Exists(assembly, rightPath));
        Assert.False(Resource.Exists(assembly, wrongPath));
    }

    [/*Fun*/Fact]
    public void ResourceEnumeration() {
        var assembly = GetType().Assembly;
        var pattern1 = "Resources\\*.png";
        var pattern2 = "Resources\\*.jpg";
        var results1 = Resource.Enumerate(assembly, pattern1).ToArray();
        var results2 = Resource.Enumerate(assembly, pattern2).ToArray();
        Assert.Single(results1);
        Assert.Empty(results2);
    }

    [/*Fun*/Fact]
    public void ResourceAttachment() {
        MailAddress spamTarget = null; // insert your e-mail address here!
        using var message = new MailMessage() {
            Sender = new MailAddress("🐕 <it@codedog.pl>"),
            From = new MailAddress("🐕 <it@codedog.pl>"),
            Subject = $"⚠ It's {DateTime.Now}. WOOF!",
            Body = "<h1>Hello.</h1><p>Woof! Here, take this CodeDog logo!</p><p><img src='cid:testImage'/>",
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = true
        };
        message.Attachments.Add(new ResourceAttachment(GetType().Assembly, @"Resources\Logo.png", "testImage", DispositionTypeNames.Inline));
        if (spamTarget != null) {
            using var client = new SmtpClient("smtp.gmail.com", 587) {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential("beta63465@gmail.com", "Dn5x,&DMOx5p6N#C.]v9hcSvHxaBy>_T") // throwaway account, but please...
            };
            message.To.Add(spamTarget);
            client.Send(message);
        }

    }

}