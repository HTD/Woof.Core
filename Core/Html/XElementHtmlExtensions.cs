using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Woof.Core.Html {

    /// <summary>
    /// Additional XElement methods for HTML elements.
    /// </summary>
    public static class XElementHtmlExtensions {

        /// <summary>
        /// Gets all CSS classes of this element or null if class attribute is not set.
        /// </summary>
        /// <param name="element">HTML element.</param>
        /// <returns>Classes array or null.</returns>
        public static string[] Classes(this XElement element) => element.Attribute(_class)?.Value?.Split(' ');

        /// <summary>
        /// Adds a CSS class to this element if it doesn't already exist.
        /// </summary>
        /// <param name="element">HTML element.</param>
        /// <param name="classValue">Class name.</param>
        /// <returns>This element.</returns>
        public static XElement AddClass(this XElement element, string classValue) {
            if (classValue == null) return element;
            var itsClass = element.Attribute(_class)?.Value;
            if (itsClass == null) { element.SetAttributeValue(_class, classValue); return element; }
            var items = itsClass.Split(' ');
            if (items.Contains(classValue)) return element;
            element.SetAttributeValue(_class, String.Join(" ", items.Concat(new[] { classValue })));
            return element;
        }

        /// <summary>
        /// Removes a CSS class from this element.
        /// </summary>
        /// <param name="element">HTML element.</param>
        /// <param name="classValue">Class name.</param>
        /// <returns>This element.</returns>
        public static XElement RemoveClass(this XElement element, string classValue) {
            if (classValue == null) return element;
            var itsClass = element.Attribute(_class)?.Value;
            if (itsClass == null) return element;
            var items = itsClass.Split(' ');
            if (!items.Contains(classValue)) return element;
            element.SetAttributeValue(_class, String.Join(" ", items.Where(i => i != classValue)));
            return element;
        }

        /// <summary>
        /// Shortcut to <see cref="XElement.SetAttributeValue(XName, object)"/>.
        /// </summary>
        /// <param name="element">HTML element.</param>
        /// <param name="name">Attribute name.</param>
        /// <param name="value">Attribute value.</param>
        /// <returns>This element.</returns>
        public static XElement Attr(this XElement element, string name, string value) {
            if (value == null) return element;
            element.SetAttributeValue(name, value);
            return element;
        }

        /// <summary>
        /// Selects all descendant elements matching the name specified.
        /// </summary>
        /// <param name="element">HTML element.</param>
        /// <param name="name">Element name.</param>
        /// <returns>Matched element collection.</returns>
        public static IEnumerable<XElement> Select(this XElement element, string name)
            => element.Descendants().Where(i => i.Name == name);

        /// <summary>
        /// Selects first descendant of specified tag name, or throws an exception if none found.
        /// </summary>
        /// <param name="element">This element.</param>
        /// <param name="name">Element name to select.</param>
        /// <returns>Matched element.</returns>
        public static XElement First(this XElement element, string name)
            => element.Descendants().First(i => i.Name == name);

        /// <summary>
        /// Selects single descendant of specified tag name, or throws an exception if there is no exactly one such element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement Single(this XElement element, string name)
            => element.Descendants().Single(i => i.Name == name);

        /// <summary>
        /// Removes the element but not its descendants. All descendant elemnts are moved to this element parent.
        /// </summary>
        /// <param name="element">This element.</param>
        public static void Pop(this XElement element) {
            foreach (var node in element.Nodes()) element.AddBeforeSelf(node);
            element.Remove();
        }

        /// <summary>
        /// Merges source element into target element by overriding target's element attributes and value.
        /// The class attribute is special: source classes are added to target classes.
        /// </summary>
        /// <param name="target">Element whose attributes and value are changed.</param>
        /// <param name="source">Element used as source.</param>
        public static void Merge(this XElement target, XElement source) {
            target.RemoveNodes();
            foreach (var node in source.Nodes()) target.Add(node);
            foreach (var a in source.Attributes()) {
                if (a.Name != "class") target.SetAttributeValue(a.Name, a.Value);
                else target.AddClass(a.Value);
            }
        }

        const string _class = "class";

    }

}