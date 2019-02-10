using System;
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
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class JsonFieldIgnore : System.Attribute
    {
        public object WhenValueIs;
        public int WhenLengthIs;
    }
}
