using System;
using System.IO;
using System.Reflection;

namespace VJson {
    public class JsonSerializer {
        public JsonSerializer(Type type) {
        }

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
                case NodeKind.Object:
                    SerializeObject(writer, o, v);
                    return;
                case NodeKind.Array:
                    SerializeArray(writer, o, v);
                    return;
                case NodeKind.String:
                case NodeKind.Number:
                case NodeKind.Boolean:
                    SerializePrimitive(writer, o, v);
                    return;
                case NodeKind.Null:
                    SerializeNull(writer, o, v);
                    return;
            }
        }

        void SerializeObject<T>(JsonWriter writer, T o, IValidator v)
        {
            writer.WriteObjectStart();

            var ty = o.GetType();
            FieldInfo[] fields = ty.GetFields();
            foreach (var field in fields)
            {
                var elemName = field.Name;
                var elemValue = field.GetValue(o);

                writer.WriteObjectKey(elemName);
                SerializeValue(writer, elemValue, v);
            }

            writer.WriteObjectEnd();
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

            writer.WriteArrayEnd();
        }

        void SerializePrimitive<T>(JsonWriter writer, T o, IValidator v)
        {
            var write = typeof(JsonWriter).GetMethod("WriteValue", new []{o.GetType()});
            write.Invoke(writer, new object[] {o});
        }

        void SerializeNull<T>(JsonWriter writer, T o, IValidator v)
        {
            writer.WriteValueNull();
        }
    }
}
