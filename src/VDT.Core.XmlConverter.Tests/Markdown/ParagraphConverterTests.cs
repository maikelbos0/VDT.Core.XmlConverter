﻿using System.Collections.Generic;
using System.IO;
using VDT.Core.XmlConverter.Markdown;
using Xunit;

namespace VDT.Core.XmlConverter.Tests.Markdown {
    public class ParagraphConverterTests {
        [Theory]
        [InlineData("p", true)]
        [InlineData("P", true)]
        [InlineData("baz", false)]
        public void IsValidFor(string elementName, bool expectedIsValid) {
            var converter = new ParagraphConverter();

            Assert.Equal(expectedIsValid, converter.IsValidFor(ElementDataHelper.Create(elementName)));
        }

        [Theory]
        [InlineData(0, "\r\n\r\n")]
        [InlineData(1, "\r\n")]
        [InlineData(2, "")]
        public void RenderStart(int trailingNewLineCount, string expectedOutput) {
            using var writer = new StringWriter();

            var converter = new ParagraphConverter();
            var elementData = ElementDataHelper.Create(
                "p",
                additionalData: new Dictionary<string, object?> {
                    { nameof(ContentTracker.TrailingNewLineCount), trailingNewLineCount }
                }
            );

            converter.RenderStart(elementData, writer);

            Assert.Equal(expectedOutput, writer.ToString(), ignoreLineEndingDifferences: true);
        }

        [Theory]
        [InlineData(0, "\r\n\r\n")]
        [InlineData(1, "\r\n")]
        [InlineData(2, "")]
        public void RenderEnd(int trailingNewLineCount, string expectedOutput) {
            using var writer = new StringWriter();

            var converter = new ParagraphConverter();
            var elementData = ElementDataHelper.Create(
                "p",
                additionalData: new Dictionary<string, object?> {
                    { nameof(ContentTracker.TrailingNewLineCount), trailingNewLineCount }
                }
            );

            converter.RenderEnd(elementData, writer);

            Assert.Equal(expectedOutput, writer.ToString(), ignoreLineEndingDifferences: true);
        }
    }
}
