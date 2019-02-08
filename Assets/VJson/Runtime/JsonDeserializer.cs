using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace VJson {
    public class JsonDeserializer
    {
        private Type _expectedInitialType = null;

        public JsonDeserializer(Type type)
        {
            _expectedInitialType = type;
        }

        public object Deserialize(TextReader textReader)
        {
            var reader = new JsonReader(textReader);
            var node = reader.Read();

            return DeserializeValue(node, _expectedInitialType);
        }

        object DeserializeValue(INode node, Type expectedType)
        {
            Console.WriteLine("Node: " + node.Kind + " / " + expectedType);
            var expectedKind = Node.KindOfType(expectedType);
            Console.WriteLine("expected: " + expectedKind);

            return DeserializeValueAs(node, expectedKind, expectedType);
        }

        object DeserializeValueAs(INode node, NodeKind targetKind, Type targetType)
        {
            switch (targetKind)
            {
                case NodeKind.Boolean:
                    return DeserializeToBoolean(node, targetKind, targetType);

                case NodeKind.Integer:
                case NodeKind.Float:
                    return DeserializeToNumber(node, targetKind, targetType);

                case NodeKind.String:
                    return DeserializeToString(node, targetKind, targetType);

                case NodeKind.Array:
                    return DeserializeToArray(node, targetKind, targetType);

                case NodeKind.Object:
                    if (node.Kind != NodeKind.Object) {
                        return DeserializeValueAs(node, node.Kind, typeof(object));
                    }

                    return DeserializeToObject(node, targetKind, targetType);

                case NodeKind.Null:
                    return DeserializeToNull(node, targetKind, targetType);

                default:
                    throw new NotImplementedException("default");
            }
        }

        object DeserializeToBoolean(INode node, NodeKind targetKind, Type targetType)
        {
            if (node is NullNode) {
                // TODO: type check of targetType
                return default(bool);
            }

            if (node is BooleanNode bNode) {
                // TODO: type check of targetType
                return bNode.Value;
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }

        object DeserializeToNumber(INode node, NodeKind targetKind, Type targetType)
        {
            if (node is NullNode) {
                // TODO: type check of targetType
                return default(int);
            }

            if (node is IntegerNode iNode) {
                // TODO: type check of targetType
                return iNode.Value;
            }

            if (node is FloatNode fNode) {
                // TODO: type check of targetType
                return fNode.Value;
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }

        object DeserializeToString(INode node, NodeKind targetKind, Type targetType)
        {
            if (node is NullNode) {
                // TODO: type check of targetType
                return default(string);
            }

            if (node is StringNode sNode) {
                // TODO: type check of targetType
                return sNode.Value;
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }

        object DeserializeToArray(INode node, NodeKind targetKind, Type targetType)
        {
            if (node is NullNode) {
                // TODO: type check of targetType
                return null;
            }

            if (node is ArrayNode aNode) {
                // TODO: type check of targetType

                if (targetType.IsArray) {
                    var container = (Array)Activator.CreateInstance(targetType, new object[] { aNode.Elems.Count });
                    var elementType = targetType.GetElementType();

                    for(int i=0; i<aNode.Elems.Count; ++i) {
                        var v = DeserializeValue(aNode.Elems[i], elementType);
                        container.SetValue(v, i);
                    }

                    return container;

                } else {
                    // container = Activator.CreateInstance(expectedType);
                    throw new NotImplementedException();
                }
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }

        object DeserializeToObject(INode node, NodeKind targetKind, Type targetType)
        {
            if (node is NullNode) {
                // TODO: type check of targetType
                return null;
            }

            if (node is ObjectNode oNode) {
                // TODO: type check of targetType

                // TODO: support dictionary

                Console.WriteLine("node: " + node);
                Console.WriteLine("targetKind: " + targetKind);
                Console.WriteLine("targetType: " + targetType);

                var container = Activator.CreateInstance(targetType);
                foreach(var elem in oNode.Elems) {
                    Console.WriteLine("Key: " + elem.Key);

                    var elementInfo = targetType.GetField(elem.Key, BindingFlags.Public | BindingFlags.Instance);
                    if (elementInfo == null) {
                        // TODO: ignore or raise errors?
                        Console.WriteLine("Ignored: " + elem.Key);
                        continue;
                    }
                    var elementType = elementInfo.FieldType;

                    var v = DeserializeValue(elem.Value, elementType);
                    elementInfo.SetValue(container, v);
                }

                return container;
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }

        object DeserializeToNull(INode node, NodeKind targetKind, Type targetType)
        {
            if (node is NullNode) {
                // TODO: type check of targetType
                return null;
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }
    }
}
