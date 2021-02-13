using System;
using System.IO;
using System.Text;
using VJson;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    public class Deserializer
    {
        private readonly byte[] i = Encoding.UTF8.GetBytes("255");

        [Benchmark]
        public object IntegerToByte() {
            using(var ms = new MemoryStream(i))
            {
                var d = new JsonDeserializer(typeof(byte));
                return d.Deserialize(ms);
            }
        }

        [Benchmark]
        public object IntegerToLong() {
            using(var ms = new MemoryStream(i))
            {
                var d = new JsonDeserializer(typeof(long));
                return d.Deserialize(ms);
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Deserializer>();
        }
    }
}
