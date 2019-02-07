using System.IO;
using NUnit.Framework;

namespace VJson.UnitTests
{
	public class NodeKindTests
	{
		[TestCase(true, NodeKind.Boolean)]
		[TestCase(false, NodeKind.Boolean)]
		[TestCase(1, NodeKind.Number)]
		[TestCase("", NodeKind.String)]
		public void KindTest<T>(T value, NodeKind expected)
		{
			var actual = Node.KindOfValue(value);
			Assert.AreEqual(expected, actual);
		}
	}
}
