//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.IO;
using System.Text;

namespace VJson
{
    public class JsonSerializer
    {
        private Type _type;

        public JsonSerializer(Type type)
        {
            this._type = type;
        }

        #region Serializer

        public void Serialize<T>(Stream s, T o)
        {
            using (var w = new JsonWriter(s))
            {
                SerializeValue(w, o);
            }
        }

        public string Serialize<T>(T o)
        {
            using (var s = new MemoryStream())
            {
                Serialize(s, o);
                return Encoding.UTF8.GetString(s.ToArray());
            }
        }

        void SerializeValue<T>(JsonWriter writer, T o)
        {
            var kind = Node.KindOfValue(o);

            switch (kind)
            {
                case NodeKind.String:
                case NodeKind.Integer:
                case NodeKind.Float:
                case NodeKind.Boolean:
                    SerializePrimitive(writer, o);
                    return;
                case NodeKind.Array:
                    SerializeArray(writer, o);
                    return;
                case NodeKind.Object:
                    SerializeObject(writer, o);
                    return;
                case NodeKind.Null:
                    SerializeNull(writer, o);
                    return;
            }
        }

        void SerializePrimitive<T>(JsonWriter writer, T o)
        {
            if (TypeHelper.TypeWrap(o.GetType()).IsEnum)
            {
                var attr = TypeHelper.GetCustomAttribute<JsonAttribute>(o.GetType());
                switch (attr != null ? attr.EnumConversion : EnumConversionType.AsInt)
                {
                    case EnumConversionType.AsInt:
                        // Convert to simple integer
                        SerializeValue(writer, Convert.ChangeType(o, Enum.GetUnderlyingType(o.GetType())));
                        break;

                    case EnumConversionType.AsString:
                        SerializeValue(writer, TypeHelper.GetStringEnumNameOf(o));
                        break;
                }

                return;
            }

            var write = TypeHelper.TypeWrap(typeof(JsonWriter)).GetMethod("WriteValue", new[] { o.GetType() });
            write.Invoke(writer, new object[] { o });
        }

        void SerializeArray<T>(JsonWriter writer, T o)
        {
            writer.WriteArrayStart();

            foreach (var elem in TypeHelper.ToIEnumerable(o))
            {
                SerializeValue(writer, elem);
            }

            writer.WriteArrayEnd();
        }

        void SerializeObject<T>(JsonWriter writer, T o)
        {
            writer.WriteObjectStart();

            foreach (var kv in TypeHelper.ToKeyValues(o))
            {
                writer.WriteObjectKey(kv.Key);
                SerializeValue(writer, kv.Value);
            }

            writer.WriteObjectEnd();
        }

        void SerializeNull<T>(JsonWriter writer, T o)
        {
            writer.WriteValueNull();
        }
        #endregion

        #region Deserializer
        public object Deserialize(String text)
        {
            var d = new JsonDeserializer(_type);
            return d.Deserialize(text);
        }

        public object Deserialize(Stream s)
        {
            var d = new JsonDeserializer(_type);
            return d.Deserialize(s);
        }
        #endregion
    }
}
