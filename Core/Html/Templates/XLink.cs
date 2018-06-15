using System.Linq;
using System.Xml.Linq;

namespace Woof.Core.Html.Templates {

    /// <summary>
    /// Special <see cref="XElement"/> with HTML link properties and constructors.
    /// </summary>
    public class XLink : XElement {

        /// <summary>
        /// Gets or sets CSS class of the element.
        /// </summary>
        public string Class {
            get => Attribute(_class).Value; set => SetAttributeValue(_class, value);
        }

        /// <summary>
        /// Gets or sets element text.
        /// </summary>
        public string Text {
            get => Value; set => Value = value;
        }

        /// <summary>
        /// Gets or sets element hyperlink reference.
        /// </summary>
        public string Href {
            get => Attribute(_href).Value; set => SetAttributeValue(_href, value);
        }

        /// <summary>
        /// Gets a Font Awesome icon name (without prefix) or adds such icon to the link with set.
        /// </summary>
        public string Icon {
            get {
                var icon = Element("span");
                if (icon == null) return null;
                return icon.Classes()?.FirstOrDefault(i => i.StartsWith("fa-"))?.Substring(3);
            }
            set {
                var icon = Element("span");
                if (icon == null) Add(icon = new XElement("span") { Value = " " });
                icon.SetAttributeValue("class", $"fa fa-{value}");
            }
        }

        /// <summary>
        /// Gets or sets element identifier.
        /// </summary>
        public string Id {
            get => Attribute(_id).Value; set => SetAttributeValue(_id, value);
        }

        /// <summary>
        /// Gets or sets element rel attribute.
        /// </summary>
        public string Rel {
            get => Attribute(_rel).Value; set => SetAttributeValue(_rel, value);
        }

        /// <summary>
        /// Gets or sets element target attribute.
        /// </summary>
        public string Target {
            get => Attribute(_target).Value; set => SetAttributeValue(_target, value);
        }

        /// <summary>
        /// Creates empty (invalid) link.
        /// </summary>
        public XLink() : base(_a) { }

        /// <summary>
        /// Creates a HTML link or span if href attribute is not set.
        /// </summary>
        /// <param name="text">Element text.</param>
        /// <param name="href">Element hyperlink reference.</param>
        public XLink(string text, string href = null) : base(href != null ? _a : _span) {
            if (href != null) Href = href;
            if (text != null) Text = text;
        }

        const string _a = "a";
        const string _class = "class";
        const string _href = "href";
        const string _id = "id";
        const string _rel = "rel";
        const string _span = "span";
        const string _target = "target";

    }

}