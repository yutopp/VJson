//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VJson
{
    public class JsonDeserializer
    {
        private Type _expectedInitialType = null;

        public JsonDeserializer(Type type)
        {
            _expectedInitialType = type;
        }

        public object Deserialize(string text)
        {
            using (var s = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return Deserialize(s);
            }
        }

        public object Deserialize(Stream s)
        {
            using (var r = new JsonReader(s))
            {
                var node = r.Read();
                return Deserialize(node);
            }
        }

        public object Deserialize(INode node)
        {
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
                    return DeserializeToObject(node, targetKind, targetType);

                case NodeKind.Null:
                    return DeserializeToNull(node, targetKind, targetType);

                default:
                    throw new NotImplementedException("default");
            }
        }

        object DeserializeToBoolean(INode node, NodeKind targetKind, Type targetType)
        {
            if (node is NullNode)
            {
                if (!(targetType is object))
                {
                    throw new NotImplementedException();
                }
                return null;
            }

            var bNode = node as BooleanNode;
            if (bNode != null)
            {
                return CreateInstanceIfConstrucutable<bool>(targetType, bNode.Value);
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }

        object DeserializeToNumber(INode node, NodeKind targetKind, Type targetType)
        {
            if (node is NullNode)
            {
                if (!(targetType is object))
                {
                    throw new NotImplementedException();
                }
                return null;
            }

            var iNode = node as IntegerNode;
            if (iNode != null)
            {
                return CreateInstanceIfConstrucutable<long>(targetType, iNode.Value);
            }

            var fNode = node as FloatNode;
            if (fNode != null)
            {
                return CreateInstanceIfConstrucutable<double>(targetType, fNode.Value);
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }

        object DeserializeToString(INode node, NodeKind targetKind, Type targetType)
        {
            if (node is NullNode)
            {
                // TODO: type check of targetType
                return default(string);
            }

            var sNode = node as StringNode;
            if (sNode != null)
            {
                // TODO: type check of targetType
                return sNode.Value;
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }

        object DeserializeToArray(INode node, NodeKind targetKind, Type targetType)
        {
            bool isConvertible =
                targetType == typeof(object)
                || (targetType.IsArray)
                || (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
                ;
            if (!isConvertible)
            {
                // TODO: raise suitable errors
                throw new NotImplementedException();
            }

            if (node is NullNode)
            {
                return null;
            }

            var aNode = node as ArrayNode;
            if (aNode != null)
            {
                if (targetType.IsArray || targetType == typeof(object))
                {
                    // To Array
                    var conteinerTy = targetType;
                    if (conteinerTy == typeof(object))
                    {
                        conteinerTy = typeof(object[]);
                    }

                    var len = aNode.Elems != null ? aNode.Elems.Count : 0;
                    var container = (Array)Activator.CreateInstance(conteinerTy, new object[] { len });

                    var elemType = conteinerTy.GetElementType();
                    for (int i = 0; i < len; ++i)
                    {
                        var v = DeserializeValue(aNode.Elems[i], elemType);
                        container.SetValue(v, i);
                    }

                    return container;

                }
                else
                {
                    // To List
                    var conteinerTy = targetType;

                    var len = aNode.Elems != null ? aNode.Elems.Count : 0;
                    var container = (IList)Activator.CreateInstance(conteinerTy);

                    var elemType = conteinerTy.GetGenericArguments()[0];
                    for (int i = 0; i < len; ++i)
                    {
                        var v = DeserializeValue(aNode.Elems[i], elemType);
                        container.Add(v);
                    }

                    return container;
                }
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }

        object DeserializeToObject(INode node, NodeKind targetKind, Type targetType)
        {
            if (targetKind != NodeKind.Object)
            {
                // TODO: raise suitable errors
                throw new NotImplementedException();
            }

            if (node is NullNode)
            {
                return null;
            }

            var oNode = node as ObjectNode;
            if (oNode != null)
            {
                bool asDictionary =
                    targetType == typeof(object)
                    || (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    ;
                if (asDictionary)
                {
                    // To Dictionary
                    Type containerTy = targetType;
                    if (containerTy == typeof(object))
                    {
                        containerTy = typeof(Dictionary<string, object>);
                    }

                    var keyType = containerTy.GetGenericArguments()[0];
                    if (keyType != typeof(string))
                    {
                        throw new NotImplementedException();
                    }

                    var container = (IDictionary)Activator.CreateInstance(containerTy);

                    if (oNode.Elems == null)
                    {
                        goto dictionaryDecoded;
                    }

                    var elemType = containerTy.GetGenericArguments()[1];
                    foreach (var elem in oNode.Elems)
                    {
                        // TODO: duplication check
                        var v = DeserializeValue(elem.Value, elemType);
                        container.Add(elem.Key, v);
                    }

                dictionaryDecoded:
                    return container;

                }
                else
                {
                    // Mapping to the structure

                    // TODO: add type check
                    var container = Activator.CreateInstance(targetType);
                    var fields = targetType.GetFields();
                    foreach (var field in fields)
                    {
                        var attr = (JsonField)Attribute.GetCustomAttribute(field, typeof(JsonField));

                        // TODO: duplication check
                        var elemName = JsonField.FieldName(attr, field);

                        INode elem = null;
                        if (oNode.Elems == null || !oNode.Elems.TryGetValue(elemName, out elem))
                        {
                            // TODO: ignore or raise errors?
                            continue;
                        }

                        if (attr != null && attr.TypeHints != null)
                        {
                            Exception lastEx = null;
                            bool resolved = false;
                            foreach (var hint in attr.TypeHints)
                            {
                                var elemType = hint;
                                try
                                {
                                    var v = DeserializeValue(elem, elemType);
                                    field.SetValue(container, v);

                                    resolved = true;
                                    break;
                                }
                                catch (Exception e)
                                {
                                    lastEx = e;
                                }
                            }
                            if (!resolved)
                            {
                                throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            var elemType = field.FieldType;

                            var v = DeserializeValue(elem, elemType);
                            field.SetValue(container, v);
                        }
                    }

                    return container;
                }
            }

            // A json node type is NOT an object but the target type is an object.
            // Thus, change a target kind and retry.
            return DeserializeValueAs(node, node.Kind, targetType);
        }

        object DeserializeToNull(INode node, NodeKind targetKind, Type targetType)
        {
            if (node is NullNode)
            {
                // TODO: type check of targetType
                return null;
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }

        static object CreateInstanceIfConstrucutable<T>(Type targetType, T value)
        {
            // Raw
            if (targetType == typeof(object))
            {
                return value;
            }

            var convFunc = TypeHelper.GetConverter(typeof(T), targetType);
            if (convFunc != null) {
                return convFunc(value);
            }

            // Try to convert value implicitly
            var attr = (Json)Attribute.GetCustomAttribute(targetType, typeof(Json));
            if (attr == null)
            {
                throw new NotImplementedException(targetType.ToString());
            }

            if (!attr.ImplicitConstructable)
            {
                throw new NotImplementedException(targetType.ToString());
            }

            var ctor = targetType.GetConstructor(new[] { typeof(T) });
            if (ctor == null)
            {
                throw new NotImplementedException();
            }

            return ctor.Invoke(new object[] { value });
        }
    }
}
