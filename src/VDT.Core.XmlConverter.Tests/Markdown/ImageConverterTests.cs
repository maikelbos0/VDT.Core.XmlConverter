﻿using System.Collections.Generic;
using System.IO;
using VDT.Core.XmlConverter.Markdown;
using Xunit;

namespace VDT.Core.XmlConverter.Tests.Markdown {
    public class ImageConverterTests {
        [Theory]
        [InlineData("img", true)]
        [InlineData("IMG", true)]
        [InlineData("foo", false)]
        public void IsValidFor(string elementName, bool expectedIsValid) {
            var converter = new ImageConverter();

            Assert.Equal(expectedIsValid, converter.IsValidFor(ElementDataHelper.Create(elementName)));
        }

        [Theory]
        [InlineData(null, null, null, "![]()")]
        [InlineData("A picture", null, null, "![A picture]()")]
        [InlineData("A picture", "https://picsum.photos/200", null, "![A picture](https://picsum.photos/200)")]
        [InlineData(null, "https://picsum.photos/200", null, "![](https://picsum.photos/200)")]
        [InlineData("A picture", "https://picsum.photos/200", "The title", "![A picture](https://picsum.photos/200 \"The title\")")]
        [InlineData(null, "https://picsum.photos/200", "The title", "![](https://picsum.photos/200 \"The title\")")]
        [InlineData(null, null, "The title", "![]( \"The title\")")]
        public void RenderStart(string? alt, string? src, string? title, string expectedOutput) {
            using var writer = new StringWriter();

            var converter = new ImageConverter();
            var attributes = new Dictionary<string, string>();

            if (src != null) {
                attributes["src"] = src;
            }

            if (alt != null) {
                attributes["alt"] = alt;
            }

            if (title != null) {
                attributes["title"] = title;
            }

            converter.RenderStart(ElementDataHelper.Create("img", attributes: attributes), writer);

            Assert.Equal(expectedOutput, writer.ToString());
        }

        [Fact]
        public void ShouldRenderContent_Returns_False() {
            var converter = new ImageConverter();

            Assert.False(converter.ShouldRenderContent(ElementDataHelper.Create("bar")));
        }

        [Fact]
        public void RenderEnd() {
            using var writer = new StringWriter();

            var converter = new ImageConverter();

            converter.RenderEnd(ElementDataHelper.Create("bar"), writer);

            Assert.Equal("", writer.ToString());
        }
    }
}
