using System.Linq;
using System.Xml.Linq;

namespace Woof.Core {

    /// <summary>
    /// XML template document.
    /// </summary>
    public class XTemplate : XDocument {

        /// <summary>
        /// Attribute used to target elements of the template.
        /// </summary>
        const string TemplateTarget = "x-target";

        /// <summary>
        /// Creates a blank template document.
        /// </summary>
        public XTemplate() : base() { }

        /// <summary>
        /// Create a template document from embedded resource.
        /// </summary>
        /// <param name="path">Embedded resource path.</param>
        public XTemplate(string path) : base(new Resource(path).Document) { }

        /// <summary>
        /// Creates a template document from another document.
        /// </summary>
        /// <param name="document">Other document.</param>
        public XTemplate(XDocument document) : base(document) { }

        /// <summary>
        /// Implicit to XElement conversion.
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator XElement(XTemplate t) => new XElement(t.Root);

        /// <summary>
        /// Targets elemt by <see cref="TemplateTarget"/> attribute. The attribute is then removed from the template.
        /// </summary>
        /// <param name="name">Attribute value to search.</param>
        /// <returns>Targetted element.</returns>
        public XElement Target(string name) {
            var target = Root.Descendants().First(i => i.Attribute(TemplateTarget)?.Value == name);
            target.Attribute(TemplateTarget).Remove();
            return target;
        }

        /// <summary>
        /// Targets element by <see cref="TemplateTarget"/> attribute. The attribute is then removed from the template.
        /// </summary>
        /// <param name="name">Attribute value to search.</param>
        /// <param name="root">Root element.</param>
        /// <returns></returns>
        public static XElement Target(string name, XElement root) {
            var target = root.Descendants().First(i => i.Attribute(TemplateTarget)?.Value == name);
            target.Attribute(TemplateTarget).Remove();
            return target;
        }

        /// <summary>
        /// Targets element by <see cref="TemplateTarget"/> attribute. Takes the targetted element out from the template.
        /// For use with dynamic elements like containers and items.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XElement TakeTarget(string name) {
            var element = Target(name);
            var copy = new XElement(element);
            element.Remove();
            return copy;
        }

        /// <summary>
        /// Targets element by <see cref="TemplateTarget"/> attribute. Takes the targetted element out from the template.
        /// For use with dynamic elements like containers and items.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static XElement TakeTarget(string name, XElement root) {
            var element = Target(name, root);
            var copy = new XElement(element);
            element.Remove();
            return copy;
        }

    }

}