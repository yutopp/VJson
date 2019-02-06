using NUnit.Framework;
using VJson;

namespace VJson.UnitTests
{
    class JsonSerializerTests
    {
        [Test]
        public void SampleTest()
        {
            var serializer = new VJson.JsonSerializer(typeof(int));

            Assert.IsFalse(serializer.Test() == 0, "");
        }
    }
}
