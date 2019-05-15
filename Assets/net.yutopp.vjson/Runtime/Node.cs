//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VJson
{
    public enum NodeKind
    {
        Object,
        Array,
        String,
        Integer, // Number
        Float,   // Number
        Boolean,
        Null,

        Undefined,
    }

    public interface INode
    {
        NodeKind Kind { get; }

        INode this[int index] { get; }
        INode this[string key] { get; }
    }

    public class BooleanNode : INode
    {
        public NodeKind Kind
        {
            get { return NodeKind.Boolean; }
        }

        public INode this[int index] { get { return UndefinedNode.Undef; } }
        public INode this[string key] { get { return UndefinedNode.Undef; } }

        public bool Value { get; private set; }

        public BooleanNode(bool v)
        {
            Value = v;
        }

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as BooleanNode;
            if (rhs == null)
            {
                return false;
            }

            return Value.Equals(rhs.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return "BOOLEAN: " + Value;
        }
    }

    public class NullNode : INode
    {
        public NodeKind Kind
        {
            get { return NodeKind.Null; }
        }

        public INode this[int index] { get { return UndefinedNode.Undef; } }
        public INode this[string key] { get { return UndefinedNode.Undef; } }

        public static readonly INode Null = new NullNode();

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as NullNode;
            if (rhs == null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "NULL";
        }
    }

    public class UndefinedNode : INode
    {
        public NodeKind Kind
        {
            get { return NodeKind.Undefined; }
        }

        public INode this[int index] { get { return Undef; } }
        public INode this[string key] { get { return Undef; } }

        public static readonly INode Undef = new UndefinedNode();

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as UndefinedNode;
            if (rhs == null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "UNDEFINED";
        }
    }

    public class IntegerNode : INode
    {
        public NodeKind Kind
        {
            get { return NodeKind.Integer; }
        }

        public INode this[int index] { get { return UndefinedNode.Undef; } }
        public INode this[string key] { get { return UndefinedNode.Undef; } }

        public long Value { get; private set; }

        public IntegerNode(long v)
        {
            Value = v;
        }

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as IntegerNode;
            if (rhs == null)
            {
                return false;
            }

            return Value.Equals(rhs.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return "NUMBER(Int): " + Value;
        }
    }

    public class FloatNode : INode
    {
        public NodeKind Kind
        {
            get { return NodeKind.Float; }
        }

        public INode this[int index] { get { return UndefinedNode.Undef; } }
        public INode this[string key] { get { return UndefinedNode.Undef; } }

        public double Value { get; private set; }

        public FloatNode(double v)
        {
            Value = v;
        }

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as FloatNode;
            if (rhs == null)
            {
                return false;
            }

            return Value.Equals(rhs.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return "NUMBER(Float): " + Value;
        }
    }

    public class StringNode : INode
    {
        public NodeKind Kind
        {
            get { return NodeKind.String; }
        }

        public INode this[int index] { get { return UndefinedNode.Undef; } }
        public INode this[string key] { get { return UndefinedNode.Undef; } }

        public string Value { get; private set; }

        public StringNode(string v)
        {
            Value = v;
        }

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as StringNode;
            if (rhs == null)
            {
                return false;
            }

            return Value.Equals(rhs.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return "STRING: " + Value;
        }
    }

    public class ObjectNode : INode, IEnumerable<KeyValuePair<string, INode>>
    {
        public Dictionary<string, INode> Elems;

        public NodeKind Kind
        {
            get { return NodeKind.Object; }
        }

        public INode this[int index] { get { return UndefinedNode.Undef; } }
        public INode this[string key]
        {
            get
            {
                INode n = null;
                if (Elems != null)
                {
                    Elems.TryGetValue(key, out n);
                }

                return n != null ? n : UndefinedNode.Undef;
            }
        }

        public void AddElement(string key, INode elem)
        {
            if (Elems == null)
            {
                Elems = new Dictionary<string, INode>();
            }

            Elems.Add(key, elem); // TODO: check duplication
        }

        public void RemoveElement(string key)
        {
            if (Elems == null)
            {
                return;
            }

            Elems.Remove(key);
        }

        public IEnumerator<KeyValuePair<string, INode>> GetEnumerator()
        {
            if (Elems != null)
            {
                return Elems.GetEnumerator();
            }

            return Enumerable.Empty<KeyValuePair<string, INode>>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as ObjectNode;
            if (rhs == null)
            {
                return false;
            }

            if (Elems == null)
            {
                return rhs.Elems == null;
            }

            return Elems.OrderBy(p => p.Key).SequenceEqual(rhs.Elems.OrderBy(p => p.Key));
        }

        public override int GetHashCode()
        {
            if (Elems == null)
            {
                return 0;
            }

            return Elems.GetHashCode();
        }

        public override string ToString()
        {
            if (Elems == null)
            {
                return "OBJECT: {}";
            }

            return "OBJECT: " + String.Join("; ", Elems.Select(p => p.Key + " = " + p.Value).ToArray());
        }
    }

    public class ArrayNode : INode, IEnumerable<INode>
    {
        public List<INode> Elems;

        public NodeKind Kind
        {
            get { return NodeKind.Array; }
        }

        public INode this[int index]
        {
            get
            {
                var elem = Elems != null ? Elems.ElementAtOrDefault(index) : null;
                return elem != null ? elem : UndefinedNode.Undef;
            }
        }
        public INode this[string key] { get { return UndefinedNode.Undef; } }

        public void AddElement(INode elem)
        {
            if (Elems == null)
            {
                Elems = new List<INode>();
            }

            Elems.Add(elem); // TODO: check duplication
        }

        public void RemoveElementAt(int index)
        {
            if (Elems == null)
            {
                return;
            }

            if (index >= 0 && index < Elems.Count)
            {
                Elems.RemoveAt(index);
            }
        }

        public IEnumerator<INode> GetEnumerator()
        {
            if (Elems != null)
            {
                return Elems.GetEnumerator();
            }

            return Enumerable.Empty<INode>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as ArrayNode;
            if (rhs == null)
            {
                return false;
            }

            if (Elems == null)
            {
                return rhs.Elems == null;
            }

            return Elems.SequenceEqual(rhs.Elems);
        }

        public override int GetHashCode()
        {
            if (Elems == null)
            {
                return 0;
            }

            return Elems.GetHashCode();
        }

        public override string ToString()
        {
            if (Elems == null)
            {
                return "ARRAY: []";
            }

            return "ARRAY: " + String.Join("; ", Elems.Select(e => e.ToString()).ToArray());
        }
    }

    public static class Node
    {
        // TODO: optimize
        public static NodeKind KindOfValue<T>(T o)
        {
            if (o == null)
            {
                return NodeKind.Null;
            }

            var ty = o.GetType();
            return KindOfType(ty);
        }

        public static NodeKind KindOfType(Type ty)
        {
            // Unwrap all Nullable<T>s
            // Any class values are nullable, however this library does not treat them as nullables.
            // Thus we adjust logic of Nullable<T> to as same as class values. Nullable<T> will be treated as T.
            if (TypeHelper.TypeWrap(ty).IsGenericType && ty.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return KindOfType(TypeHelper.TypeWrap(ty).GetGenericArguments()[0]);
            }

            NodeKind k;
            if (_primitiveTable.TryGetValue(ty, out k))
            {
                return k;
            }

            // Enum(integer or string)
            if (TypeHelper.TypeWrap(ty).IsEnum)
            {
                var attr = TypeHelper.GetCustomAttribute<JsonAttribute>(ty);
                return attr != null && attr.EnumConversion == EnumConversionType.AsString
                    ? NodeKind.String
                    : NodeKind.Integer;
            }

            // Arrays
            // If elem type exists, it can treat as Array(IEnumerable)
            var elemTy = TypeHelper.ElemTypeOfIEnumerable(ty);
            if (elemTy != null)
            {
                return NodeKind.Array;
            }

            // Others
            return NodeKind.Object;
        }

        static Dictionary<Type, NodeKind> _primitiveTable = new Dictionary<Type, NodeKind>
        {
            {typeof(bool), NodeKind.Boolean},
            {typeof(byte), NodeKind.Integer},
            {typeof(sbyte), NodeKind.Integer},
            {typeof(char), NodeKind.Integer},
            {typeof(decimal), NodeKind.Integer},
            {typeof(double), NodeKind.Float},
            {typeof(float), NodeKind.Float},
            {typeof(int), NodeKind.Integer},
            {typeof(uint), NodeKind.Integer},
            {typeof(long), NodeKind.Integer},
            {typeof(ulong), NodeKind.Integer},
            {typeof(short), NodeKind.Integer},
            {typeof(ushort), NodeKind.Integer},
            {typeof(string), NodeKind.String},
        };
    }
}
