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
        public void Serialize<T>(TextWriter textWriter, T o, IValidator v = null)
        {
            var writer = new JsonWriter(textWriter);
            SerializeValue(writer, o, v);
        }

        void SerializeValue<T>(JsonWriter writer, T o, IValidator v)
        {
            var kind = Node.KindOfValue(o);

            switch (kind)
            {
                case NodeKind.String:
                case NodeKind.Integer:
                case NodeKind.Float:
                case NodeKind.Boolean:
                    SerializePrimitive(writer, o, v);
                    return;
                case NodeKind.Array:
                    SerializeArray(writer, o, v);
                    return;
                case NodeKind.Object:
                    SerializeObject(writer, o, v);
                    return;
                case NodeKind.Null:
                    SerializeNull(writer, o, v);
                    return;
            }
        }

        void SerializePrimitive<T>(JsonWriter writer, T o, IValidator v)
        {
            var write = typeof(JsonWriter).GetMethod("WriteValue", new []{o.GetType()});
            write.Invoke(writer, new object[] {o});
        }

        void SerializeArray<T>(JsonWriter writer, T o, IValidator v)
        {
            writer.WriteArrayStart();

            var ty = o.GetType();
			if (ty.IsArray)
			{
                foreach (var elem in o as Array)
                {
                    SerializeValue(writer, elem, v);
                }
			}
            if (ty.IsGenericType) {
                var containerTy = ty.GetGenericTypeDefinition();
                if (containerTy != typeof(List<>)) {
                    throw new NotImplementedException();
                }
                foreach(var elem in (IList)o)
                {
                    SerializeValue(writer, elem, v);
                }
            }

            writer.WriteArrayEnd();
        }

        void SerializeObject<T>(JsonWriter writer, T o, IValidator v)
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
                    SerializeValue(writer, elem.Value, v);
                }

                goto encoded;
            }

            // Traverse fields
            FieldInfo[] fields = ty.GetFields();
            foreach (var field in fields)
            {
                var elemName = field.Name;
                var elemValue = field.GetValue(o);

                writer.WriteObjectKey(elemName);
                SerializeValue(writer, elemValue, v);
            }

        encoded:
            writer.WriteObjectEnd();
        }

        void SerializeNull<T>(JsonWriter writer, T o, IValidator v)
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
        #endregion
    }
}
