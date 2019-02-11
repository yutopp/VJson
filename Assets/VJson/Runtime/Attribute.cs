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
    [AttributeUsage(AttributeTargets.Class)]
    public class Json : System.Attribute
    {
        public bool ImplicitConstructable;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class JsonField : System.Attribute
    {
        public Type[] TypeHints;
        public string Name;

        public static string FieldName(JsonField f, FieldInfo fi)
        {
            if (f != null && f.Name != null) {
                return f.Name;
            }

            return fi.Name;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class JsonFieldIgnore : System.Attribute
    {
        public object WhenValueIs;
        public int WhenLengthIs;

        public static bool IsIgnorable<T>(JsonFieldIgnore f, T o)
        {
            if (f == null) {
                return false;
            }

            // Value
            if (Object.Equals(o, f.WhenValueIs)) {
                return true;
            }

            // Length
            if (o is Array a) {
                return a.Length == f.WhenLengthIs;
            } else if (o is IList l) {
                return l.Count == f.WhenLengthIs;
            }

            // Others
            return false;
        }
    }
}
