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
    static class TypeHelper
    {
        public static IEnumerable<object> ToIEnumerable(object o)
        {
            var ty = o.GetType();
            if (ty.IsArray)
            {
                if (ty.HasElementType && ty.GetElementType().IsClass)
                {
                    return ((IEnumerable<object>)o);
                }
                else
                {
                    return ((IEnumerable)o).Cast<object>();
                }
            }
            else
            {
                return ((IEnumerable)o).Cast<object>();
            }
        }

        public static Type ElemTypeOfIEnumerable(Type ty)
        {
            if (ty.IsArray)
            {
                if (ty.HasElementType)
                {
                    return ty.GetElementType();
                }

                return null;
            }

            if (ty.IsGenericType && ty.GetGenericTypeDefinition() == typeof(List<>))
            {
                return ty.GetGenericArguments()[0];
            }

            return null;
        }

        public static IEnumerable<KeyValuePair<string, object>> ToKeyValues(object o)
        {
            return ToKeyValuesUnordered(o).OrderBy(kv => kv.Key);
        }

        public static IEnumerable<KeyValuePair<string, object>> ToKeyValuesUnordered(object o)
        {
            var ty = o.GetType();
            if (ty.IsGenericType && ty.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var keyType = ty.GetGenericArguments()[0];
                if (keyType != typeof(string))
                {
                    // TODO: Should allow them and call `ToString`?
                    throw new NotImplementedException();
                }

                foreach (DictionaryEntry elem in (IDictionary)o)
                {
                    yield return new KeyValuePair<string, object>((string)elem.Key, elem.Value);
                }

            }
            else
            {
                var fields = ty.GetFields();
                foreach (var field in fields)
                {
                    var fieldAttr = (JsonField)Attribute.GetCustomAttribute(field, typeof(JsonField));

                    // TODO: duplication check
                    var elemName = JsonField.FieldName(fieldAttr, field);
                    var elemValue = field.GetValue(o);

                    var fieldIgnoreAttr =
                        (JsonFieldIgnorable)Attribute.GetCustomAttribute(field, typeof(JsonFieldIgnorable));
                    if (JsonFieldIgnorable.IsIgnorable(fieldIgnoreAttr, elemValue))
                    {
                        continue;
                    }

                    yield return new KeyValuePair<string, object>(elemName, elemValue);
                }
            }
        }

        class DeepEqualityComparer : IEqualityComparer<object>
        {
            public bool Equals(object a, object b)
            {
                return DeepEquals(a, b);
            }

            public int GetHashCode(object a)
            {
                return a.GetHashCode();
            }
        }

        public static bool DeepEquals(object lhs, object rhs)
        {
            var lhsKind = Node.KindOfValue(lhs);
            var rhsKind = Node.KindOfValue(rhs);
            if (lhsKind != rhsKind)
            {
                return false;
            }

            switch (lhsKind)
            {
                case NodeKind.Boolean:
                case NodeKind.Integer:
                case NodeKind.Float:
                case NodeKind.String:
                    return Object.Equals(lhs, rhs);

                case NodeKind.Array:
                    var lhsArr = ToIEnumerable(lhs);
                    var rhsArr = ToIEnumerable(rhs);
                    return lhsArr.SequenceEqual(rhsArr, new DeepEqualityComparer());

                case NodeKind.Object:
#if NETCOREAPP2_0
                    var lhsKvs = new Dictionary<string, object>(ToKeyValues(lhs));
                    var rhsKvs = new Dictionary<string, object>(ToKeyValues(rhs));
#else
                    var lhsKvs = new Dictionary<string, object>();
                    foreach (var kv in ToKeyValues(lhs))
                    {
                        lhsKvs.Add(kv.Key, kv.Value);
                    }
                    var rhsKvs = new Dictionary<string, object>();
                    foreach (var kv in ToKeyValues(rhs))
                    {
                        rhsKvs.Add(kv.Key, kv.Value);
                    }
#endif
                    if (!lhsKvs.Keys.SequenceEqual(rhsKvs.Keys))
                    {
                        return false;
                    }
                    return lhsKvs.All(kv => DeepEquals(kv.Value, rhsKvs[kv.Key]));

                case NodeKind.Null:
                    return true;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
