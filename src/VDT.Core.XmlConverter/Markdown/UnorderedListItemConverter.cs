﻿using System;
using System.IO;
using System.Linq;

namespace VDT.Core.XmlConverter.Markdown {
    /// <summary>
    /// Converter for rendering elements as unordered list items in Markdown
    /// </summary>
    public class UnorderedListItemConverter : BlockElementConverter {
        private const string orderedListName = "ol";

        /// <summary>
        /// Construct an instance of a Markdown unordered list item converter
        /// </summary>
        public UnorderedListItemConverter() : base("- ", "li") { }

        /// <inheritdoc/>
        public override bool IsValidFor(ElementData elementData) 
            => base.IsValidFor(elementData) 
            && !string.Equals(elementData.Ancestors.FirstOrDefault()?.Name, orderedListName, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public override void RenderStart(ElementData elementData, TextWriter writer) {
            base.RenderStart(elementData, writer);
            elementData.GetContentTracker().Prefixes.Push("\t");
        }

        /// <inheritdoc/>
        override public void RenderEnd(ElementData elementData, TextWriter writer) {
            elementData.GetContentTracker().Prefixes.Pop();
            base.RenderEnd(elementData, writer);
        }
    }
}
