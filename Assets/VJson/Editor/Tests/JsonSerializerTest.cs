using NUnit.Framework;
using VJson;

namespace VJson.UnitTests
{
    static class JsonSerializerTests
    {
        [Test]
        static public void ReturnFalseGivenValueOf1()
        {
            var serializer = new VJson.JsonSerializer(typeof(int));

            Assert.IsFalse(serializer.Test() == 0, "");
        }
    }
}
