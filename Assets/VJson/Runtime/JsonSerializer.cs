using System;
using System.IO;

namespace VJson {
    public interface IValidator {
    }

    public class JsonSerializer {
        public JsonSerializer(Type type) {
        }

        public void Serialize(TextWriter stream, object o, IValidator v = null) {
            stream.Write("WIP");
        }

        public int Test() {
            return 0;
        }
    }
}
