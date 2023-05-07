﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using VDT.Core.XmlConverter.Markdown;
using Xunit;

namespace VDT.Core.XmlConverter.Tests.Markdown {
    public class UnorderedListItemConverterTests {
        [Theory]
        [InlineData(false, "div")]
        [InlineData(false, "div", "ol")]
        [InlineData(true, "li")]
        [InlineData(true, "li", "ul")]
        [InlineData(false, "li", "ol")]
        [InlineData(true, "li", "ul", "ol")]
        [InlineData(false, "li", "ol", "ul")]
        public void IsValidFor(bool expectedIsValidFor, string elementName, params string[] ancestorElementNames) {
            var converter = new UnorderedListItemConverter();
            var elementData = ElementDataHelper.Create(
                elementName,
                ancestorElementNames.Select(n => ElementDataHelper.Create(n))
            );

            Assert.Equal(expectedIsValidFor, converter.IsValidFor(elementData));
        }

        [Fact]
        public void RenderStart() {
            using var writer = new StringWriter();

            var converter = new UnorderedListItemConverter();
            var elementData = ElementDataHelper.Create("li");

            converter.RenderStart(elementData, writer);

            Assert.Equal("\r\n- ", writer.ToString(), ignoreLineEndingDifferences: true);
            Assert.Equal("\t", Assert.Single(Assert.IsType<Stack<string>>(elementData.AdditionalData[nameof(ContentTracker.Prefixes)])));
        }

        [Fact]
        public void RenderEnd() {
            using var writer = new StringWriter();

            var converter = new UnorderedListItemConverter();
            var prefixes = new Stack<string>();
            var elementData = ElementDataHelper.Create(
                "li",
                additionalData: new Dictionary<string, object?>() {
                    { nameof(ContentTracker.Prefixes), prefixes }
                }
            );

            prefixes.Push("> ");
            prefixes.Push("\t");

            converter.RenderEnd(elementData, writer);

            Assert.Equal("\r\n", writer.ToString(), ignoreLineEndingDifferences: true);
            Assert.Equal("> ", Assert.Single(Assert.IsType<Stack<string>>(elementData.AdditionalData[nameof(ContentTracker.Prefixes)])));
        }
    }
}
