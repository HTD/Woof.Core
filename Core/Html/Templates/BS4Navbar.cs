using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Woof.Core.Html;

namespace Woof.Core.Html.Templates {

    /// <summary>
    /// Bootstrap 4 navbar template class.
    /// </summary>
    public class BS4Navbar : XTemplate {

        /// <summary>
        /// CSS class of the main element of navbar-*, bg-*, etc.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether navbar internal elements are to be wrapped in a positioning container.
        /// </summary>
        public bool InContainer { get; set; }

        /// <summary>
        /// Gets or sets single menu to render, use <see cref="Parts"/> property to set up multiple menus.
        /// </summary>
        public MenuNode Menu { get; set; }

        /// <summary>
        /// Gets or sets multiple menus with optional different alignment.
        /// </summary>
        public List<MenuNode> Parts { get; set; }

        /// <summary>
        /// Creates and initializes Bootstrap 4 navbar template.
        /// </summary>
        public BS4Navbar() : base(Template) {
            if (UidSequence == 0) UidSequence = GetHashCode(); else UidSequence++;
        }

        /// <summary>
        /// Renders element using preset properties.
        /// </summary>
        /// <returns>Rendered element.</returns>
        public XElement Render() {
            if (RenderingState > 0) return Root;
            if (!InContainer) Root.Elements().First().Pop();
            if (Class != null) Root.AddClass(Class);
            var brandLink = Target("brand-link");
            if (Menu != null) {
                if (Menu.Value != null) brandLink.Merge(Menu.Value);
                else brandLink.Remove();
            } else {
                if (Parts?.First()?.Value != null) brandLink.Merge(Parts.First().Value);
                else brandLink.Remove();
            }
            Target("toggler").Attr("data-target", $"#{Uid}");
            var content = Target("content").Attr("id", Uid);
            UidSequence++;
            ContainerTemplate = TakeTarget("container");
            ItemTemplate = TakeTarget("item", ContainerTemplate);
            DropdownTemplate = TakeTarget("dropdown", ContainerTemplate);
            if (Menu != null && Parts != null) throw new InvalidOperationException("Menu and Parts are mutually exclusive.");
            if (Menu != null) content.Add(RenderPart(Menu));
            else if (Parts != null) Parts.ForEach(part => content.Add(RenderPart(part)));
            return Root;
        }

        /// <summary>
        /// Renders one container part of the content.
        /// </summary>
        /// <param name="part">Menu definition.</param>
        /// <returns>Rendered part.</returns>
        private XElement RenderPart(MenuNode part) {
            var container = new XElement(ContainerTemplate);
            if (part.Class != null) {
                if (RenderingState > 0) container.RemoveClass("mr-auto");
                container.AddClass(part.Class);
            }
            if (part.Children != null) part.Children.ForEach(child => {
                if (child.Children == null) { // item
                    var item = new XElement(ItemTemplate);
                    var link = Target("item-link", item);
                    link.Merge(child.Value);
                    container.Add(item);
                }
                else {
                    var dropdown = new XElement(DropdownTemplate);
                    var dropdownText = Target("dropdown-text", dropdown);
                    var dropdownContainer = Target("dropdown-container", dropdown);
                    var dropdownItemTemplate = TakeTarget("dropdown-item", dropdownContainer);
                    dropdownText.Value = child.Value.Text;
                    dropdownText.Attr("id", Uid);
                    dropdownContainer.Attr("aria-labelledby", Uid);
                    UidSequence++;
                    if (child.Class != null) dropdownContainer.AddClass(child.Class);
                    child.Children.ForEach(grandChild => {
                        var item = new XElement(dropdownItemTemplate);
                        var link = Target("dropdown-link", item);
                        link.Merge(grandChild.Value);
                        dropdownContainer.Add(item);
                    });
                    container.Add(dropdown);
                }
            });
            RenderingState++;
            return container;
        }

        static readonly XDocument Template = new Resource("Templates/Bootstrap4/Navbar.html").Document;

        static int UidSequence;
        string Uid => $"bs4navbar-{UidSequence}";
        int RenderingState;
        XElement ContainerTemplate;
        XElement ItemTemplate;
        XElement DropdownTemplate;

    }

}