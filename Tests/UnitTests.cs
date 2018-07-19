using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Woof.Core;
using Xunit;

public class UnitTests {

    [Fact]
    public void ResourceExistence() {
        var assembly = GetType().Assembly;
        var rightPath = "Resources\\Logo.png";
        var wrongPath = "Nowhere\\Logo.png";
        Assert.True(Resource.Exists(assembly, rightPath));
        Assert.False(Resource.Exists(assembly, wrongPath));
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