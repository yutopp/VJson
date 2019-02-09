using System;
using System.Collections;
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
            var expectedKind = Node.KindOfType(expectedType);

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

                    var elemType = targetType.GetElementType();
                    for(int i=0; i<aNode.Elems.Count; ++i) {
                        var v = DeserializeValue(aNode.Elems[i], elemType);
                        container.SetValue(v, i);
                    }

                    return container;

                } else {
                    if (targetType.IsGenericType) {
                        var containerTy = targetType.GetGenericTypeDefinition();
                        if (containerTy == typeof(List<>)) {
                            var container = (IList)Activator.CreateInstance(targetType);

                            var elemType = targetType.GetGenericArguments()[0];
                            for(int i=0; i<aNode.Elems.Count; ++i) {
                                var v = DeserializeValue(aNode.Elems[i], elemType);
                                container.Add(v);
                            }

                            return container;
                        }
                    }

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

                if (targetType.IsGenericType) {
                    var containerTy = targetType.GetGenericTypeDefinition();
                    if (containerTy != typeof(Dictionary<,>)) {
                        goto mapping;
                    }

                    var keyType = targetType.GetGenericArguments()[0];
                    if (keyType != typeof(string)) {
                        throw new NotImplementedException();
                    }

                    var container0 = (IDictionary)Activator.CreateInstance(targetType);

                    var elemType = targetType.GetGenericArguments()[1];
                    foreach(var elem in oNode.Elems) {
                        // TODO: duplication check
                        var v = DeserializeValue(elem.Value, elemType);
                        container0.Add(elem.Key, v);
                    }

                    return container0;
                }

            mapping:
                var container = Activator.CreateInstance(targetType);
                foreach(var elem in oNode.Elems) {
                    var elementInfo = targetType.GetField(elem.Key, BindingFlags.Public | BindingFlags.Instance);
                    if (elementInfo == null) {
                        // TODO: ignore or raise errors?
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
