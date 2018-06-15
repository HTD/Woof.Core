using System.Xml.Linq;

namespace Woof.Core.Html.Templates {

    /// <summary>
    /// Provides Bootstrap button element shortcut.
    /// </summary>
    class BS4Button : XElement {

        /// <summary>
        /// Gets or sets button identifier, needed to be set for quick access from script.
        /// </summary>
        public string Id { get => Attribute(_id).Value; set => SetElementValue(_id, value); }

        /// <summary>
        /// Gets or sets CSS class of the button (beside btn).
        /// </summary>
        public string Class { get => Attribute(_class).Value; set { SetAttributeValue(_class, _btn); this.AddClass(value); } }

        /// <summary>
        /// Gets or sets data-target attribute for a reference for other Bootstrap interactive elements.
        /// </summary>
        public string DataTarget { get => Attribute(_data_target).Value; set => SetElementValue(_data_target, value); }

        /// <summary>
        /// Gets or sets button type. Set default to "button" for ajax.
        /// </summary>
        public string Type { get => Attribute(_type).Value; set => SetElementValue(_type, value); }

        /// <summary>
        /// Creates new empty button element.
        /// </summary>
        public BS4Button() : base(_button) {
            Type = _button;
            SetAttributeValue(_class, _btn);
        }

        /// <summary>
        /// Creates complete button element.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="label">Label to be displayed.</param>
        /// <param name="cssClass">CSS class (beside btn).</param>
        public BS4Button(string label, string cssClass = null, string id = null) : base(_button) {
            Class = cssClass;
            if (id != null) Id = id;
            Value = label;
        }

        const string _btn = "btn";
        const string _button = "button";
        const string _class = "class";
        const string _data_target = "data-target";
        const string _id = "id";
        const string _type = "type";

    }

}