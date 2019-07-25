using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Woof.Core;
using Xunit;
using A = Woof.Algorithms;

public class UnitTests {

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