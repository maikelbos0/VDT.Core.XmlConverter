﻿using System.IO;

namespace VDT.Core.XmlConverter.Markdown {
    /// <summary>
    /// Converter for rendering images as Markdown
    /// </summary>
    public class ImageConverter : BaseElementConverter {
        /// <summary>
        /// Construct an instance of a Markdown image converter
        /// </summary>
        public ImageConverter() : base("img") { }

        /// <inheritdoc/>
        public override void RenderStart(ElementData elementData, TextWriter writer) {
            var tracker = elementData.GetContentTracker();

            tracker.Write(writer, "![");

            if (elementData.TryGetAttribute("alt", out var alt)) {
                tracker.Write(writer, alt);
            }

            tracker.Write(writer, "](");

            if (elementData.TryGetAttribute("src", out var src)) {
                tracker.Write(writer, src);
            }

            if (elementData.TryGetAttribute("title", out var title)) {
                tracker.Write(writer, " \"");
                tracker.Write(writer, title);
                tracker.Write(writer, "\"");
            }

            tracker.Write(writer, ")");
        }

        /// <inheritdoc/>
        public override bool ShouldRenderContent(ElementData elementData) => false;

        /// <inheritdoc/>
        public override void RenderEnd(ElementData elementData, TextWriter writer) { }
    }
}
