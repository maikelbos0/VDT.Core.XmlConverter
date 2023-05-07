﻿using System.IO;
using VDT.Core.XmlConverter.Markdown;
using Xunit;

namespace VDT.Core.XmlConverter.Tests.Markdown {
    public class FencedPreConverterTests {
        [Theory]
        [InlineData("pre", true)]
        [InlineData("PRE", true)]
        [InlineData("foo", false)]
        public void IsValidFor(string elementName, bool expectedIsValid) {
            var converter = new FencedPreConverter();

            Assert.Equal(expectedIsValid, converter.IsValidFor(ElementDataHelper.Create(elementName)));
        }

        [Fact]
        public void RenderStart() {
            using var writer = new StringWriter();

            var converter = new FencedPreConverter();

            converter.RenderStart(ElementDataHelper.Create("pre"), writer);

            Assert.Equal("\r\n```", writer.ToString(), ignoreLineEndingDifferences: true);
        }

        [Fact]
        public void RenderEnd() {
            using var writer = new StringWriter();

            var converter = new FencedPreConverter();

            converter.RenderEnd(ElementDataHelper.Create("pre"), writer);

            Assert.Equal("\r\n```\r\n", writer.ToString(), ignoreLineEndingDifferences: true);
        }
    }
}
