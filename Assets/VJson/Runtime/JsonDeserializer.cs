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
using System.Linq;

namespace VJson
{
    using Internal;

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
            return DeserializeValue(node, _expectedInitialType, new State());
        }

        object DeserializeValue(INode node, Type expectedType, State state)
        {
            var expectedKind = Node.KindOfType(expectedType);

            return DeserializeValueAs(node, expectedKind, expectedType, state);
        }

        object DeserializeValueAs(INode node, NodeKind targetKind, Type targetType, State state)
        {
            switch (targetKind)
            {
                case NodeKind.Boolean:
                    return DeserializeToBoolean(node, targetKind, targetType, state);

                case NodeKind.Integer:
                case NodeKind.Float:
                    return DeserializeToNumber(node, targetKind, targetType, state);

                case NodeKind.String:
                    return DeserializeToString(node, targetKind, targetType, state);

                case NodeKind.Array:
                    return DeserializeToArray(node, targetKind, targetType, state);

                case NodeKind.Object:
                    return DeserializeToObject(node, targetKind, targetType, state);

                case NodeKind.Null:
                    return DeserializeToNull(node, targetKind, targetType, state);

                default:
                    throw new NotImplementedException("default: " + targetKind);
            }
        }

        object DeserializeToBoolean(INode node, NodeKind targetKind, Type targetType, State state)
        {
            if (node is NullNode)
            {
                if (!TypeHelper.TypeWrap(targetType).IsClass)
                {
                    var msg = state.CreateMessage("Null cannot convert to non-nullable value({0})", targetType);
                    throw new DeserializeFailureException(msg);
                }

                return null;
            }

            var bNode = node as BooleanNode;
            if (bNode != null)
            {
                return CreateInstanceIfConstrucutable<bool>(targetType, bNode.Value, state);
            }

            // TODO: Should raise error?
            var msg0 = state.CreateMessage("{0} cannot convert to {1}", node.Kind, targetType);
            throw new DeserializeFailureException(msg0);
        }

        object DeserializeToNumber(INode node, NodeKind targetKind, Type targetType, State state)
        {
            if (node is NullNode)
            {
                if (!TypeHelper.TypeWrap(targetType).IsClass)
                {
                    var msg = state.CreateMessage("Null cannot convert to non-nullable value({0})", targetType);
                    throw new DeserializeFailureException(msg);
                }

                return null;
            }

            var iNode = node as IntegerNode;
            if (iNode != null)
            {
                return CreateInstanceIfConstrucutable<long>(targetType, iNode.Value, state);
            }

            var fNode = node as FloatNode;
            if (fNode != null)
            {
                return CreateInstanceIfConstrucutable<double>(targetType, fNode.Value, state);
            }

            // TODO: Should raise error?
            var msg0 = state.CreateMessage("{0} cannot convert to {1}", node.Kind, targetType);
            throw new DeserializeFailureException(msg0);
        }

        object DeserializeToString(INode node, NodeKind targetKind, Type targetType, State state)
        {
            if (node is NullNode)
            {
                if (!TypeHelper.TypeWrap(targetType).IsClass)
                {
                    var msg = state.CreateMessage("Null cannot convert to non-nullable value({0})", targetType);
                    throw new DeserializeFailureException(msg);
                }

                return null;
            }

            var sNode = node as StringNode;
            if (sNode != null)
            {
                return CreateInstanceIfConstrucutable<string>(targetType, sNode.Value, state);
            }

            // TODO: Should raise error?
            var msg0 = state.CreateMessage("{0} cannot convert to {1}", node.Kind, targetType);
            throw new DeserializeFailureException(msg0);
        }

        object DeserializeToArray(INode node, NodeKind targetKind, Type targetType, State state)
        {
            bool isConvertible =
                targetType == typeof(object)
                || (targetType.IsArray)
                || (TypeHelper.TypeWrap(targetType).IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
                ;
            if (!isConvertible)
            {
                var msg = state.CreateMessage("Array cannot convert to non-iterable value({0})", targetType);
                throw new DeserializeFailureException(msg);
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
                        var v = DeserializeValue(aNode.Elems[i], elemType, state.NestAsElem(i));
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

                    var elemType = TypeHelper.TypeWrap(conteinerTy).GetGenericArguments()[0];
                    for (int i = 0; i < len; ++i)
                    {
                        var v = DeserializeValue(aNode.Elems[i], elemType, state.NestAsElem(i));
                        container.Add(v);
                    }

                    return container;
                }
            }

            // TODO: Should raise error?
            var msg0 = state.CreateMessage("{0} cannot convert to {1}", node.Kind, targetType);
            throw new DeserializeFailureException(msg0);
        }

        object DeserializeToObject(INode node, NodeKind targetKind, Type targetType, State state)
        {
            if (targetKind != NodeKind.Object)
            {
                var msg0 = state.CreateMessage("{0} cannot convert to {1}", node.Kind, targetType);
                throw new DeserializeFailureException(msg0);
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
                    || (TypeHelper.TypeWrap(targetType).IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    ;
                if (asDictionary)
                {
                    // To Dictionary
                    Type containerTy = targetType;
                    if (containerTy == typeof(object))
                    {
                        containerTy = typeof(Dictionary<string, object>);
                    }

                    var keyType = TypeHelper.TypeWrap(containerTy).GetGenericArguments()[0];
                    if (keyType != typeof(string))
                    {
                        throw new NotImplementedException();
                    }

                    var container = (IDictionary)Activator.CreateInstance(containerTy);

                    if (oNode.Elems == null)
                    {
                        goto dictionaryDecoded;
                    }

                    var elemType = TypeHelper.TypeWrap(containerTy).GetGenericArguments()[1];
                    foreach (var elem in oNode.Elems)
                    {
                        // TODO: duplication check
                        var v = DeserializeValue(elem.Value, elemType, state.NestAsElem(elem.Key));
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
                    var fields = TypeHelper.TypeWrap(targetType).GetFields();
                    foreach (var field in fields)
                    {
                        var attr = TypeHelper.GetCustomAttribute<JsonField>(field);

                        // TODO: duplication check
                        var elemName = JsonField.FieldName(attr, field);

                        INode elem = null;
                        if (oNode.Elems == null || !oNode.Elems.TryGetValue(elemName, out elem))
                        {
                            // TODO: ignore or raise errors?
                            continue;
                        }

                        var elemState = state.NestAsElem(elemName);

                        if (attr != null && attr.TypeHints != null)
                        {
                            bool resolved = false;
                            foreach (var hint in attr.TypeHints)
                            {
                                var elemType = hint;
                                try
                                {
                                    var v = DeserializeValue(elem, elemType, elemState);
                                    field.SetValue(container, v);

                                    resolved = true;
                                    break;
                                }
                                catch (Exception)
                                {
                                }
                            }
                            if (!resolved)
                            {
                                var msg = elemState.CreateMessage("{0} cannot convert to one of [{1}]",
                                                                  elem.Kind,
                                                                  string.Join(", ", attr.TypeHints.Select(t => t.ToString()).ToArray()));
                                throw new DeserializeFailureException(msg);
                            }
                        }
                        else
                        {
                            var elemType = field.FieldType;

                            var v = DeserializeValue(elem, elemType, elemState);
                            field.SetValue(container, v);
                        }
                    }

                    return container;
                }
            }

            // A json node type is NOT an object but the target type is an object.
            // Thus, change a target kind and retry.
            return DeserializeValueAs(node, node.Kind, targetType, state);
        }

        object DeserializeToNull(INode node, NodeKind targetKind, Type targetType, State state)
        {
            if (node is NullNode)
            {
                // TODO: type check of targetType
                return null;
            }

            // TODO: Should raise error?
            throw new NotImplementedException();
        }

        static object CreateInstanceIfConstrucutable<T>(Type targetType, T value, State state)
        {
            // Raw
            if (targetType == typeof(object))
            {
                return value;
            }

            var convFunc = TypeHelper.GetConverter(typeof(T), targetType);
            if (convFunc != null)
            {
                return convFunc(value);
            }

            // Try to convert value implicitly
            var attr = TypeHelper.GetCustomAttribute<Json>(targetType);
            if (attr == null)
            {
                var msg = state.CreateMessage("{0} cannot convert to {1}", typeof(T), targetType);
                throw new DeserializeFailureException(msg);
            }

            if (!attr.ImplicitConstructable)
            {
                throw new NotImplementedException(targetType.ToString());
            }

            var ctor = TypeHelper.TypeWrap(targetType).GetConstructor(new[] { typeof(T) });
            if (ctor == null)
            {
                var msg = state.CreateMessage("{0} cannot convert implicitly to {1}", typeof(T), targetType);
                throw new DeserializeFailureException(msg);
            }

            return ctor.Invoke(new object[] { value });
        }
    }

    public class DeserializeFailureException : Exception
    {
        public DeserializeFailureException(string message)
            : base(message)
        {
        }

        public DeserializeFailureException(string message, DeserializeFailureException inner)
            : base(String.Format("{0}.{1}", message, inner.Message))
        {
        }
    }
}
