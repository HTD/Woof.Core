using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;

namespace Woof.Core {

    /// <summary>
    /// Represents an attachment to an email created from an assembly resource.
    /// </summary>
    public class ResourceAttachment : Attachment {

        /// <summary>
        /// Creates new image attachment from specified assembly.
        /// </summary>
        /// <param name="assembly">Containing assembly.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="contentId">Content identifier.</param>
        /// <param name="disposition">Content disposition: "attachment" or "inline".</param>
        public ResourceAttachment(Assembly assembly, string name, string contentId = null, string disposition = null)
            : base(new Resource(assembly, name).Stream, new ContentType(MimeMapping.GetMimeMapping(name))) {
            ContentDisposition.DispositionType = disposition ?? DispositionTypeNames.Attachment;
            ContentId = contentId;
        }

        /// <summary>
        /// Creates new image attachment from entry assembly.
        /// </summary>
        /// <param name="name">Resource name.</param>
        /// <param name="contentId">Content identifier.</param>
        /// <param name="disposition">Content disposition: "attachment" or "inline".</param>
        public ResourceAttachment(string name, string contentId = null, string disposition = null) : this(Assembly.GetEntryAssembly(), name, contentId, disposition) { }

    }

}