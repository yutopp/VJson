//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections;
using System.Reflection;

namespace VJson
{
    public enum EnumConversionType
    {
        AsInt,
        AsString,
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
    public sealed class JsonAttribute : System.Attribute
    {
        public bool ImplicitConstructable; // Only for classes
        public EnumConversionType EnumConversion; // Only for enums
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class JsonFieldAttribute : System.Attribute
    {
        public string Name;
        public int Order = 0;
        public Type[] TypeHints;

        public static string FieldName(JsonFieldAttribute f, FieldInfo fi)
        {
            if (f != null && f.Name != null)
            {
                return f.Name;
            }

            return fi.Name;
        }

        public static int FieldOrder(JsonFieldAttribute f)
        {
            if (f != null)
            {
                return f.Order;
            }

            return 0;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class JsonFieldIgnorableAttribute : System.Attribute
    {
        public object WhenValueIs;
        public int WhenLengthIs;

        public static bool IsIgnorable<T>(JsonFieldIgnorableAttribute f, T o)
        {
            if (f == null)
            {
                return false;
            }

            // Value
            if (Object.Equals(o, f.WhenValueIs))
            {
                return true;
            }

            // Length
            var a = o as Array;
            if (a != null)
            {
                return a.Length == f.WhenLengthIs;
            }

            var l = o as IList;
            if (l != null)
            {
                return l.Count == f.WhenLengthIs;
            }

            // Others
            return false;
        }
    }
}
