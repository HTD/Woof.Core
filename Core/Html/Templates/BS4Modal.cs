using System.Xml.Linq;

namespace Woof.Core.Html.Templates {

    /// <summary>
    /// Bootstrap 4 modal template class.
    /// </summary>
    public class BS4Modal : XTemplate {

        /// <summary>
        /// CSS class of the main element.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Modal identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Modal title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Text to display in modal when body content is not provided.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Header part of the dialog (use for custom header content, do not touch for default title and "close" element)
        /// </summary>
        public XElement Header { get; private set; }

        /// <summary>
        /// Body element, place modal content here.
        /// </summary>
        public XElement Body { get; private set; }

        /// <summary>
        /// Footer element, place buttons here, leave blank for no footer.
        /// </summary>
        public XElement Footer { get; private set; }

        /// <summary>
        /// Creates and initializes Bootstrap 4 modal window.
        /// </summary>
        public BS4Modal() : base(Template) {
            if (UidSequence == 0) UidSequence = GetHashCode(); else UidSequence++;
            Header = Target("header");
            Body = Target("body");
            Footer = Target("footer");
        }

        /// <summary>
        /// Renders element using preset properties.
        /// </summary>
        /// <returns>Rendered element.</returns>
        public XElement Render() {
            if (RenderingState > 0) return Root;
            Root.Attr("aria-labelledby", Uid);
            if (Id != null) Root.SetAttributeValue("id", Id);
            if (Class != null) Root.AddClass(Class);
            var title = Target("title");
            title.SetAttributeValue("id", Uid);
            UidSequence++;
            if (Title != null) title.Value = Title;
            if (Text != null) Body.Value = Text;
            if (Header.IsEmpty) Header.Remove();
            if (Footer.IsEmpty) Footer.Remove();
            RenderingState++;
            return Root;
        }

        static readonly XDocument Template = new Resource("Templates/Bootstrap4/Modal.html").Document;
        static int UidSequence;
        string Uid => $"bs4modal-{UidSequence}";
        int RenderingState;

    }

}