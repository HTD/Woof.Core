using System;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Woof.Core;
using Woof.TextEx;
using Xunit;

public class UnitTests {

    [Fact]
    public void CsvReader() {
        var assembly = GetType().Assembly;
        string[] specimen1 = null;
        string[] specimen2 = null;
        {
            var reader = new CsvReader { HasHeader = true };
            reader.LinePreprocessor = new LinePreprocessorDelegate((raw) => {
                var p1 = raw.IndexOf(',') + 1;
                var p2 = raw.IndexOf(',', p1) + 2;
                var p3 = raw.LastIndexOf('|');
                return raw.Substring(0, p2) + raw.Substring(p2, p3 - p2 + 1).Replace("\"", "") + raw.Substring(p3 + 1);
            });
            var rows = reader.Read(new Resource(assembly, "Resources\\Test.csv").Text).ToArray();
            specimen1 = rows[1].Cells;
        }
        {
            var reader = new CsvReader { HasHeader = true };
            var rows = reader.Read(new Resource(assembly, "Resources\\Test.csv").Text).ToArray();
            specimen2 = rows[1].Cells;
        }
        Assert.Equal(6, specimen1.Length);
        Assert.Equal(9, specimen2.Length);
    }

    [Fact]
    public void ResourceExistence() {
        var assembly = GetType().Assembly;
        var rightPath = "Resources\\Logo.png";
        var wrongPath = "Nowhere\\Logo.png";
        Assert.True(Resource.Exists(assembly, rightPath));
        Assert.False(Resource.Exists(assembly, wrongPath));
    }

    [Fact]
    public void ResourceEnumeration() {
        var assembly = GetType().Assembly;
        var pattern1 = "Resources\\*.png";
        var pattern2 = "Resources\\*.jpg";
        var results1 = Resource.Enumerate(assembly, pattern1).ToArray();
        var results2 = Resource.Enumerate(assembly, pattern2).ToArray();
        Assert.Single(results1);
        Assert.Empty(results2);
    }

    [Fact]
    public void ResourceAttachment() {
        using (var client = new SmtpClient("smtp.gmail.com", 587) {
            EnableSsl = true,
            Credentials = new System.Net.NetworkCredential("beta63465@gmail.com", "v6GvLcF56zVWAfOFkvqP") // throwaway account, but please...
        }) {
            var image = new ResourceAttachment(GetType().Assembly, "Resources\\Logo.png", "testImage", DispositionTypeNames.Inline);
            var html = "<h1>Hello.</h1><p>Woof! Here, take this CodeDog logo!</p><p><img src='cid:testImage'/>";
            using (var message = new MailMessage() {
                Sender = new MailAddress("🐕 <it@codedog.pl>"),
                From = new MailAddress("🐕 <it@codedog.pl>"),
                Subject = $"⚠ It's {DateTime.Now}. WOOF!",
                Body = html,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            }) {
                message.To.Add(new MailAddress("it@codedog.pl"));
                message.Attachments.Add(image);
                //client.Send(message);
            }
        }
    }
        
}