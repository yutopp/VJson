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
using System.Reflection;

namespace VJson
{
    static partial class TypeHelper
    {
#if NET20 || NET35 || NET40 || NET_2_0 || NET_2_0_SUBSET
        public static Type TypeWrap(Type ty)
        {
            return ty;
        }
#else
        public static TypeInfo TypeWrap(Type ty)
        {
            return ty.GetTypeInfo();
        }
#endif

        public static bool IsBoxed(Type ty)
        {
            var ti = TypeWrap(ty);
            if (ti.IsClass) {
                return true;
            }

            return ti.IsGenericType && ty.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static T GetCustomAttribute<T>(FieldInfo fi) where T : Attribute
        {
            return (T)fi.GetCustomAttributes(typeof(T), false)
                .Where(a => a.GetType() == typeof(T))
                .FirstOrDefault();
        }

        public static T GetCustomAttribute<T>(Type ty) where T : Attribute
        {
            return (T)TypeWrap(ty).GetCustomAttributes(typeof(T), false)
                .Where(a => a.GetType() == typeof(T))
                .FirstOrDefault();
        }

        // TODO: implement cache
        public static string[] GetStringEnumNames(Type ty)
        {
            var enumFields = TypeWrap(ty).GetFields(BindingFlags.Static|BindingFlags.Public);
            return enumFields.Select(fi => {
                    var attr = GetCustomAttribute<JsonField>(fi);
                    if (attr != null && attr.Name != null) {
                        return attr.Name;
                    }

                    return fi.Name;
                }).ToArray();
        }

        public static string GetStringEnumNameOf(object e)
        {
            var eTy = e.GetType();
            var enumIndex = Array.IndexOf(Enum.GetValues(eTy), e);

            return GetStringEnumNames(eTy)[enumIndex];
        }

        public static IEnumerable<object> ToIEnumerable(object o)
        {
            var ty = o.GetType();
            if (ty.IsArray)
            {
                if (ty.HasElementType && TypeWrap(ty.GetElementType()).IsClass)
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

            if (TypeWrap(ty).IsGenericType && ty.GetGenericTypeDefinition() == typeof(List<>))
            {
                return TypeWrap(ty).GetGenericArguments()[0];
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
            if (TypeWrap(ty).IsGenericType && ty.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var keyType = TypeWrap(ty).GetGenericArguments()[0];
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
                var fields = TypeWrap(ty).GetFields();
                foreach (var field in fields)
                {
                    var fieldAttr = GetCustomAttribute<JsonField>(field);

                    // TODO: duplication check
                    var elemName = JsonField.FieldName(fieldAttr, field);
                    var elemValue = field.GetValue(o);

                    var fieldIgnoreAttr = GetCustomAttribute<JsonFieldIgnorable>(field);
                    if (JsonFieldIgnorable.IsIgnorable(fieldIgnoreAttr, elemValue))
                    {
                        continue;
                    }

                    yield return new KeyValuePair<string, object>(elemName, elemValue);
                }
            }
        }

        class DeepEqualityComparer : EqualityComparer<object>
        {
            public override bool Equals(object a, object b)
            {
                return DeepEquals(a, b);
            }

            public override int GetHashCode(object a)
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
