using System.IO;
using NUnit.Framework;

namespace VJson.UnitTests
{
	public class NodeKindTests
	{
		[TestCase(true, NodeKind.Boolean)]
		[TestCase(false, NodeKind.Boolean)]
		[TestCase(1, NodeKind.Integer)]
		[TestCase("", NodeKind.String)]
		public void KindTest<T>(T value, NodeKind expected)
		{
			var actual = Node.KindOfValue(value);
			Assert.AreEqual(expected, actual);
		}
	}

	[TestFixtureSource("FixtureArgs")]
	public class NodeComparisonTests
	{
		IntegerNode _lhs;
		IntegerNode _rhs;
		bool _expected;

		public NodeComparisonTests(IntegerNode lhs, IntegerNode rhs, bool expected) {
			_lhs = lhs;
			_rhs = rhs;
			_expected = expected;
		}

		public void EqualityTest()
		{
			var actual = _lhs.Equals(_rhs);
			Assert.AreEqual(_expected, actual);
		}

		//
		static object [] FixtureArgs = {
            new object[] {
                new IntegerNode("a"),
             	new IntegerNode("a"),
             	true
            },
			new object[] {
				new IntegerNode("a"),
				new IntegerNode("b"),
				false
			},
			new object[] {
				new IntegerNode("a"),
				null,
				false
			}
		};
	}
}
