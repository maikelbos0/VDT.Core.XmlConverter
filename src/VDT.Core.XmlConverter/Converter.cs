﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace VDT.Core.XmlConverter {
    /// <summary>
    /// Converts XML documents into other text-based document formats
    /// </summary>
    public class Converter {
        /// <summary>
        /// Options to use when calling <see cref="Convert(XmlReader, TextWriter)"/> or any of its overloads
        /// </summary>
        public ConverterOptions Options { get; set; }

        /// <summary>
        /// Construct an instance of a converter
        /// </summary>
        public Converter() : this(new ConverterOptions()) {
        }

        /// <summary>
        /// Construct an instance of a converter with the provided <see cref="ConverterOptions"/>
        /// </summary>
        /// <param name="options">Options to use when calling <see cref="Convert(XmlReader, TextWriter)"/> or any of its overloads</param>
        public Converter(ConverterOptions options) {
            Options = options;
        }

        /// <summary>
        /// Convert an XML document string using the provided <see cref="ConverterOptions"/>
        /// </summary>
        /// <param name="xml">XML document to convert</param>
        /// <returns>Converted document</returns>
        public string Convert(string xml) {
            using var writer = new StringWriter();

            Convert(xml, writer);

            return writer.ToString();
        }

        /// <summary>
        /// Convert an XML document string using the provided <see cref="ConverterOptions"/>
        /// </summary>
        /// <param name="xml">XML document to convert</param>
        /// <param name="writer">Converted document is written to this <see cref="TextWriter"/></param>
        public void Convert(string xml, TextWriter writer) {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

            Convert(stream, writer);
        }

        /// <summary>
        /// Convert a stream containing an XML document using the provided <see cref="ConverterOptions"/>
        /// </summary>
        /// <param name="stream">XML document to convert</param>
        /// <returns>Converted document</returns>
        public string Convert(Stream stream) {
            using var writer = new StringWriter();

            Convert(stream, writer);

            return writer.ToString();
        }

        /// <summary>
        /// Convert a stream containing an XML document using the provided <see cref="ConverterOptions"/>
        /// </summary>
        /// <param name="stream">XML document to convert</param>
        /// <param name="writer">Converted document is written to this <see cref="TextWriter"/></param>
        public void Convert(Stream stream, TextWriter writer) {
            using var reader = XmlReader.Create(stream, new XmlReaderSettings() {
                ConformanceLevel = ConformanceLevel.Fragment
            });

            Convert(reader, writer);
        }

        /// <summary>
        /// Convert an XML document using the provided <see cref="ConverterOptions"/>
        /// </summary>
        /// <param name="reader">XML document to convert</param>
        /// <returns>Converted document</returns>
        public string Convert(XmlReader reader) {
            using var writer = new StringWriter();

            Convert(reader, writer);

            return writer.ToString();
        }

        /// <summary>
        /// Convert an XML document using the provided <see cref="ConverterOptions"/>
        /// </summary>
        /// <param name="reader">XML document to convert</param>
        /// <param name="writer">Converted document is written to this <see cref="TextWriter"/></param>
        public void Convert(XmlReader reader, TextWriter writer) {
            var data = new ConversionData();

            if (reader.NodeType != XmlNodeType.None) {
                throw new UnexpectedNodeTypeException($"Node type '{reader.NodeType}' was not expected; ensure {nameof(reader)} is in starting position before calling {nameof(Convert)}", reader.NodeType);
            }

            while (reader.Read()) {
                ConvertNode(reader, writer, data);
            }
        }

        internal void ConvertNode(XmlReader reader, TextWriter writer, ConversionData data) {
            data.ReadNode(reader);

            if (data.CurrentNodeData is NodeData nodeData) {
                ConvertNode(reader, writer, nodeData);
            }
            else if (data.CurrentNodeData is ElementData elementData) {
                ConvertElement(reader, writer, data, elementData);
            }
            else {
                throw new InvalidOperationException($"Found unhandled implementation {data.CurrentNodeData.GetType().FullName} of {nameof(INodeData)}");
            }
        }

        internal void ConvertNode(XmlReader reader, TextWriter writer, NodeData nodeData) {
            switch (nodeData.NodeType) {
                case XmlNodeType.Text:
                    Options.TextConverter.Convert(writer, nodeData);
                    break;
                case XmlNodeType.CDATA:
                    Options.CDataConverter.Convert(writer, nodeData);
                    break;
                case XmlNodeType.Comment:
                    Options.CommentConverter.Convert(writer, nodeData);
                    break;
                case XmlNodeType.XmlDeclaration:
                    Options.XmlDeclarationConverter.Convert(writer, nodeData);
                    break;
                case XmlNodeType.Whitespace:
                    Options.WhitespaceConverter.Convert(writer, nodeData);
                    break;
                case XmlNodeType.SignificantWhitespace:
                    Options.SignificantWhitespaceConverter.Convert(writer, nodeData);
                    break;
                case XmlNodeType.DocumentType:
                    Options.DocumentTypeConverter.Convert(writer, nodeData);
                    break;
                case XmlNodeType.ProcessingInstruction:
                    Options.ProcessingInstructionConverter.Convert(writer, nodeData);
                    break;
                case XmlNodeType.EndElement:
                case XmlNodeType.Attribute:
                    throw new UnexpectedNodeTypeException($"Node type '{nodeData.NodeType}' was not expected; ensure {nameof(reader)} is in starting position before calling {nameof(Convert)}", nodeData.NodeType);
                case XmlNodeType.Document:
                case XmlNodeType.DocumentFragment:
                case XmlNodeType.EndEntity:
                case XmlNodeType.Entity:
                case XmlNodeType.EntityReference:
                case XmlNodeType.Notation:
                default:
                    throw new UnexpectedNodeTypeException($"Node type '{nodeData.NodeType}' is currently not supported", nodeData.NodeType);
            }
        }

        internal void ConvertElement(XmlReader reader, TextWriter writer, ConversionData data, ElementData elementData) {
            var depth = reader.Depth;
            var elementConverter = Options.ElementConverters.FirstOrDefault(c => c.IsValidFor(elementData)) ?? Options.DefaultElementConverter;
            var shouldRenderContent = elementConverter.ShouldRenderContent(elementData);

            elementConverter.RenderStart(elementData, writer);

            if (!elementData.IsSelfClosing) {
                while (reader.Read() && reader.Depth > depth) {
                    if (shouldRenderContent) {
                        ConvertNode(reader, writer, data);
                    }
                }
            }

            elementConverter.RenderEnd(elementData, writer);
        }
    }
}