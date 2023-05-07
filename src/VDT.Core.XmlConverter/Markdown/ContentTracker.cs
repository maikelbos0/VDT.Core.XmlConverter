﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VDT.Core.XmlConverter.Markdown {
    /// <summary>
    /// Tracks Markdown content being written to a <see cref="TextWriter"/>; used to determine later output
    /// </summary>
    public class ContentTracker {
        private static readonly Regex newLineFinder = new Regex("\r\n?|\n", RegexOptions.Compiled);

        private readonly INodeData nodeData;

        /// <summary>
        /// Amount of trailing line terminators
        /// </summary>
        public int TrailingNewLineCount {
            get {
                if (nodeData.AdditionalData.TryGetValue(nameof(TrailingNewLineCount), out var countObj) && countObj is int count) {
                    return count;
                }

                return 0;
            }
            private set {
                nodeData.AdditionalData[nameof(TrailingNewLineCount)] = value;
            }
        }

        /// <summary>
        /// <see langword="true"/> if the last things written ended in a trailing line terminator; otherwise <see langword="false"/>
        /// </summary>
        public bool HasTrailingNewLine => TrailingNewLineCount > 0;

        /// <summary>
        /// Prefixes to add to all lines
        /// </summary>
        public Stack<string> Prefixes {
            get {
                if (!(nodeData.AdditionalData.TryGetValue(nameof(Prefixes), out var prefixesObj) && prefixesObj is Stack<string> prefixes)) {
                    prefixes = new Stack<string>();
                    nodeData.AdditionalData[nameof(Prefixes)] = prefixes;
                }

                return prefixes;
            }
        }

        /// <summary>
        /// Construct a Markdown content tracker
        /// </summary>
        /// <param name="nodeData">Node currently being converted</param>
        public ContentTracker(INodeData nodeData) {
            this.nodeData = nodeData;
        }

        /// <summary>
        /// Writes a string to the text stream
        /// </summary>
        /// <param name="writer">Writer to write the string to</param>
        /// <param name="value">String to write</param>
        public void Write(TextWriter writer, string value) {
            WriteInternal(writer, value, false);
        }

        /// <summary>
        /// Writes a string to the text stream, followed by a line terminator
        /// </summary>
        /// <param name="writer">Writer to write the string to</param>
        /// <param name="value">String to write</param>
        public void WriteLine(TextWriter writer, string value) {
            WriteInternal(writer, value, true);
        }

        /// <summary>
        /// Writes a line terminator to the text stream
        /// </summary>
        /// <param name="writer">Writer to write the line terminator to</param>
        public void WriteLine(TextWriter writer) {
            WriteInternal(writer, null, true);
        }

        private void WriteInternal(TextWriter writer, string? value, bool addNewLine) {
            var prefix = string.Join("", Prefixes.Reverse());

            if (HasTrailingNewLine) {
                writer.Write(prefix);
            }

            if (value != null) {
                var lines = newLineFinder.Split(value);
                var newLineCount = lines.Reverse().TakeWhile(s => string.IsNullOrEmpty(s)).Count();
                var newLine = $"{Environment.NewLine}{prefix}";

                if (newLineCount == lines.Length) {
                    TrailingNewLineCount += newLineCount - 1;
                }
                else {
                    TrailingNewLineCount = newLineCount;
                }

                writer.Write(string.Join(newLine, lines));
            }

            if (addNewLine) {
                writer.WriteLine();
                TrailingNewLineCount++;
            }
        }
    }
}
