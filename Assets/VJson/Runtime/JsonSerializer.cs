//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace VJson {
    public class JsonSerializer
    {
        private Type _type;

        public JsonSerializer(Type type)
        {
            this._type = type;
        }

        #region Serializer
        public void Serialize<T>(TextWriter textWriter, T o)
        {
            var writer = new JsonWriter(textWriter);
            SerializeValue(writer, o);
        }

        public string Serialize<T>(T o)
        {
            using(var textWriter = new StringWriter()) {
                Serialize(textWriter, o);
                return textWriter.ToString();
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
            var write = typeof(JsonWriter).GetMethod("WriteValue", new []{o.GetType()});
            write.Invoke(writer, new object[] {o});
        }

        void SerializeArray<T>(JsonWriter writer, T o)
        {
            writer.WriteArrayStart();

            var ty = o.GetType();
			if (ty.IsArray)
			{
                foreach (var elem in o as Array)
                {
                    SerializeValue(writer, elem);
                }
			}
            if (ty.IsGenericType) {
                var containerTy = ty.GetGenericTypeDefinition();
                if (containerTy != typeof(List<>)) {
                    throw new NotImplementedException();
                }
                foreach(var elem in (IList)o)
                {
                    SerializeValue(writer, elem);
                }
            }

            writer.WriteArrayEnd();
        }

        void SerializeObject<T>(JsonWriter writer, T o)
        {
            writer.WriteObjectStart();

            var ty = o.GetType();
            if (ty.IsGenericType) {
                var containerTy = ty.GetGenericTypeDefinition();
                if (containerTy != typeof(Dictionary<,>)) {
                    throw new NotImplementedException();
                }

                var keyType = ty.GetGenericArguments()[0];
                if (keyType != typeof(string)) {
                    // TODO: Should allow them and call `ToString`?
                    throw new NotImplementedException();
                }

                foreach (DictionaryEntry elem in (IDictionary)o)
                {
                    writer.WriteObjectKey((string)elem.Key);
                    SerializeValue(writer, elem.Value);
                }

                goto encoded;
            }

            // Traverse fields
            FieldInfo[] fields = ty.GetFields();
            foreach (var field in fields)
            {
                var fieldAttr = (JsonField)Attribute.GetCustomAttribute(field, typeof(JsonField));

                // TODO: duplication check
                var elemName = JsonField.FieldName(fieldAttr, field);
                var elemValue = field.GetValue(o);

                var fieldIgnoreAttr =
                    (JsonFieldIgnore)Attribute.GetCustomAttribute(field, typeof(JsonFieldIgnore));
                if (JsonFieldIgnore.IsIgnorable(fieldIgnoreAttr, elemValue)) {
                    continue;
                }

                writer.WriteObjectKey(elemName);
                SerializeValue(writer, elemValue);
            }

        encoded:
            writer.WriteObjectEnd();
        }

        void SerializeNull<T>(JsonWriter writer, T o)
        {
            writer.WriteValueNull();
        }
        #endregion

        #region Deserializer
        public object Deserialize(TextReader textReader)
        {
            var d = new JsonDeserializer(_type);
            return d.Deserialize(textReader);
        }

        public object Deserialize(String text)
        {
            var d = new JsonDeserializer(_type);
            return d.Deserialize(text);
        }
        #endregion
    }
}
