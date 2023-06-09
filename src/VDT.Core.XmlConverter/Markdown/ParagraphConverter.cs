﻿using System.IO;

namespace VDT.Core.XmlConverter.Markdown {
    /// <summary>
    /// Converter for rendering paragraphs as Markdown
    /// </summary>
    public class ParagraphConverter : BaseElementConverter {
        /// <summary>
        /// Construct an instance of a Markdown paragraph converter
        /// </summary>
        public ParagraphConverter() : base("", "p") { }

        /// <inheritdoc/>
        public override void RenderStart(ElementData elementData, TextWriter writer) {
            var tracker = elementData.GetContentTracker();

            while (tracker.TrailingNewLineCount < 2) {
                tracker.WriteLine(writer);
            }
        }

        /// <inheritdoc/>
        public override void RenderEnd(ElementData elementData, TextWriter writer) {
            var tracker = elementData.GetContentTracker();

            while (tracker.TrailingNewLineCount < 2) {
                tracker.WriteLine(writer);
            }
        }
    }
}
