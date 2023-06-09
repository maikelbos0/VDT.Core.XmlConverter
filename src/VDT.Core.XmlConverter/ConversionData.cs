﻿using System;
using System.Collections.Generic;
using System.Xml;

namespace VDT.Core.XmlConverter {
    internal class ConversionData {
        internal Stack<ElementData> Ancestors { get; } = new Stack<ElementData>();
        internal Dictionary<string, object?> AdditionalData { get; } = new Dictionary<string, object?>();
        internal INodeData CurrentNodeData { get; set; } = new NullNodeData();

        internal void ReadNode(XmlReader reader) {
            var isFirstChild = CurrentNodeData is NullNodeData || reader.Depth > Ancestors.Count;

            while (Ancestors.Count > reader.Depth) {
                Ancestors.Pop();
            }

            if (reader.Depth > Ancestors.Count) {
                Ancestors.Push(CurrentNodeData as ElementData ?? throw new InvalidOperationException($"Expected parent node to be an {nameof(ElementData)} but found {CurrentNodeData.GetType().FullName}"));
            }

            if (reader.NodeType == XmlNodeType.Element) {
                CurrentNodeData = new ElementData(
                    reader.Name,
                    reader.GetAttributes(),
                    reader.IsEmptyElement,
                    Ancestors.ToArray(),
                    isFirstChild,
                    AdditionalData
                );
            }
            else {
                CurrentNodeData = new NodeData(
                    reader.NodeType,
                    reader.Name,
                    reader.Value,
                    Ancestors.ToArray(),
                    isFirstChild,
                    AdditionalData
                );
            }
        }
    }
}