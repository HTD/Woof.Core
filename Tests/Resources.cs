using System;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Xunit;

namespace Woof.Core.Tests {

    public class Resources {

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
            MailAddress spamTarget = new MailAddress("it@codedog.pl"); // insert your e-mail address here!
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

}