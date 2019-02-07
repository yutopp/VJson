using System;
using System.Collections.Generic;

namespace VJson
{
	public enum NodeKind
	{
		Object,
		Array,
		String,
		Number,
		Boolean,
		Null,
	}

	public class Node
	{
		// TODO: optimize
		public static NodeKind KindOfValue<T>(T o)
		{
			if (o == null)
			{
				return NodeKind.Null;
			}

			var ty = o.GetType();

			NodeKind k;
			if (_primitiveTable.TryGetValue(ty, out k))
			{
				return k;
			}

			if (ty.IsArray)
			{
				return NodeKind.Array;
			}

			return NodeKind.Object;
		}

		static Dictionary<Type, NodeKind> _primitiveTable = new Dictionary<Type, NodeKind>
		{
			{typeof(bool), NodeKind.Boolean},
			{typeof(byte), NodeKind.Number},
			{typeof(sbyte), NodeKind.Number},
			{typeof(char), NodeKind.Number},
			{typeof(decimal), NodeKind.Number},
			{typeof(double), NodeKind.Number},
			{typeof(float), NodeKind.Number},
			{typeof(int), NodeKind.Number},
			{typeof(uint), NodeKind.Number},
			{typeof(long), NodeKind.Number},
			{typeof(ulong), NodeKind.Number},
			{typeof(short), NodeKind.Number},
			{typeof(ushort), NodeKind.Number},
			{typeof(string), NodeKind.String},
		};
	}
}
