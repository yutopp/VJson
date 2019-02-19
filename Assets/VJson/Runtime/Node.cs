//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
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
    }

    public interface INode
    {
        NodeKind Kind { get; }
    }

    public class BooleanNode : INode
    {
        public NodeKind Kind
        {
            get { return NodeKind.Boolean; }
        }

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

    public class IntegerNode : INode
    {
        public NodeKind Kind
        {
            get { return NodeKind.Integer; }
        }

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

    public class ObjectNode : INode
    {
        public Dictionary<string, INode> Elems;

        public NodeKind Kind
        {
            get { return NodeKind.Object; }
        }

        public void AddElement(string key, INode elem)
        {
            if (Elems == null)
            {
                Elems = new Dictionary<string, INode>();
            }

            Elems.Add(key, elem); // TODO: check duplication
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

    public class ArrayNode : INode
    {
        public List<INode> Elems;

        public NodeKind Kind
        {
            get { return NodeKind.Array; }
        }

        public void AddElement(INode elem)
        {
            if (Elems == null)
            {
                Elems = new List<INode>();
            }

            Elems.Add(elem); // TODO: check duplication
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
            NodeKind k;
            if (_primitiveTable.TryGetValue(ty, out k))
            {
                return k;
            }

            // Enum(integer or string)
            if (TypeHelper.TypeWrap(ty).IsEnum)
            {
                // TODO: support string
                return NodeKind.Integer;
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
