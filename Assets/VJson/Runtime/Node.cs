using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
		public string Span;

		public NodeKind Kind => NodeKind.Boolean;

        public bool Value {
            get {
                return Boolean.Parse(Span);
            }
        }

		public BooleanNode(string span)
		{
			Span = span;
		}

		public override bool Equals(object rhsObj)
		{
			if (rhsObj == null)
			{
				return false;
			}

			if (!(rhsObj is BooleanNode rhs))
			{
				return false;
			}

			return Span.Equals(rhs.Span);
		}

		public override int GetHashCode()
		{
			return Span.GetHashCode();
		}

		public override string ToString()
		{
			return "BOOLEAN: " + Span;
		}
	}

	public class NullNode : INode
	{
		public NodeKind Kind => NodeKind.Null;

		public override bool Equals(object rhsObj)
		{
			if (rhsObj == null)
			{
				return false;
			}

			if (!(rhsObj is NullNode rhs))
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
		public string Span;

		public NodeKind Kind => NodeKind.Integer;

        public int Value {
            get {
                return Int32.Parse(Span); // TODO: Fix for large numbers
            }
        }

		public IntegerNode(string span)
		{
			Span = span;
		}

		public override bool Equals(object rhsObj)
		{
			if (rhsObj == null)
			{
				return false;
			}

			if (!(rhsObj is IntegerNode rhs))
			{
				return false;
			}

			return Span.Equals(rhs.Span);
		}

		public override int GetHashCode()
		{
			return Span.GetHashCode();
		}

		public override string ToString()
		{
			return "NUMBER(Int): " + Span;
		}
	}

	public class FloatNode : INode
	{
		public string Span;

		public NodeKind Kind => NodeKind.Float;

        public float Value {
            get {
                return Single.Parse(Span); // TODO: Fix for large numbers
            }
        }

		public FloatNode(string span)
		{
			Span = span;
		}

		public override bool Equals(object rhsObj)
		{
			if (rhsObj == null)
			{
				return false;
			}

			if (!(rhsObj is FloatNode rhs))
			{
				return false;
			}

			return Span.Equals(rhs.Span);
		}

		public override int GetHashCode()
		{
			return Span.GetHashCode();
		}

		public override string ToString()
		{
			return "NUMBER(Float): " + Span;
		}
	}

	public class StringNode : INode
	{
		public string Span;

		public NodeKind Kind => NodeKind.String;

        public string Value {
            get {
                return Regex.Unescape(Span.Trim());
            }
        }

		public StringNode(string span)
		{
			Span = span;
		}

		public override bool Equals(object rhsObj)
		{
			if (rhsObj == null)
			{
				return false;
			}

			if (!(rhsObj is StringNode rhs))
			{
				return false;
			}

			return Span.Equals(rhs.Span);
		}

		public override int GetHashCode()
		{
			return Span.GetHashCode();
		}

		public override string ToString()
		{
			return "STRING: " + Span;
		}
	}

	public class ObjectNode : INode
	{
		public Dictionary<string, INode> Elems;

		public NodeKind Kind => NodeKind.Object;

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
			if (rhsObj == null)
			{
				return false;
			}

			if (!(rhsObj is ObjectNode rhs))
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

		public NodeKind Kind => NodeKind.Array;

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
			if (rhsObj == null)
			{
				return false;
			}

			if (!(rhsObj is ArrayNode rhs))
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

			return "ARRAY: " + String.Join("; ", Elems);
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

            // Arrays
			if (ty.IsArray)
			{
				return NodeKind.Array;
			}
            if (ty.IsGenericType) {
                var containerTy = ty.GetGenericTypeDefinition();
                if (containerTy == typeof(List<>)) {
                    return NodeKind.Array;
                }
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
