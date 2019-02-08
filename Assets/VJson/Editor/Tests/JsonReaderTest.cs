using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;

namespace VJson.UnitTests
{
	[TestFixtureSource("FixtureArgs")]
	class JsonReaderPassTests
	{
		INode _expected;
		string _src;

		public JsonReaderPassTests(INode expected, string src) {
			_expected = expected;
			_src = src;
		}

		[Test]
		public void ReadTest()
		{
			using(var textReader = new StringReader(_src))
			{
				var reader = new JsonReader(textReader);
				var actual = reader.Read();

				Assert.That(actual, Is.EqualTo(_expected));
			}
		}

		//
		static object [] FixtureArgs = {
            // Boolean
			new object[] {
				new BooleanNode("true"),
				@"true",
			},
            new object[] {
				new BooleanNode("false"),
				@"false",
			},
			new object[] {
				new BooleanNode("true"),
				@"  true  ",
			},
            new object[] {
				new BooleanNode("false"),
				@"  false  ",
			},

            // Null
			new object[] {
				new NullNode(),
				@"null",
			},
			new object[] {
				new NullNode(),
				@"  null  ",
			},

			// Numbers
			new object[] {
				new IntegerNode("123"),
				@"123",
			},
			new object[] {
				new IntegerNode("123"),
				@"  123  ",
			},
			new object[] {
				new IntegerNode("0"),
				@"0",
			},
			new object[] {
				new IntegerNode("9"),
				@"9",
			},
			new object[] {
				new IntegerNode("-123"),
				@"-123",
			},
			new object[] {
				new IntegerNode("-0"),
				@"-0",
			},
			new object[] {
				new IntegerNode("-9"),
				@"-9",
			},
			new object[] {
				new FloatNode("-9.0"),
				@"-9.0",
			},
			new object[] {
				new FloatNode("-9.0e0"),
				@"-9.0e0",
			},
			new object[] {
				new FloatNode("-9.0e+0"),
				@"-9.0e+0",
			},
            new object[] {
                new FloatNode("-9.0e-0"),
				@"-9.0e-0",
			},
			new object[] {
				new FloatNode("-9e0"),
				@"-9e0",
			},
			new object[] {
				new FloatNode("-9e+0"),
				@"-9e+0",
			},
            new object[] {
                new FloatNode("-9e-0"),
				@"-9e-0",
			},

			// Strings
			new object[] {
				new StringNode("abc"),
				@"""abc""",
			},
			new object[] {
				new StringNode(""),
				@"""""",
			},
			new object[] {
				new StringNode("abc"),
				@"  ""abc""  ",
			},
            new object[] {
				new StringNode("あいうえお"),
				@"""あいうえお""",
			},
            new object[] {
				new StringNode("\\n"),
				@"""\n""",
			},
            new object[] {
				new StringNode("\\u3042"),
				@"""\u3042""",
			},
            new object[] {
				new StringNode("🍣"),
				@"""🍣""",
			},

			// Objects
			new object[] {
				new ObjectNode(),
				@"{}",
			},
			new object[] {
				new ObjectNode()
				{
					Elems = new Dictionary<string, INode>
					{
						{"abc", new IntegerNode("1")},
					}
				},
				@"{""abc"":1}",
			},
			new object[] {
				new ObjectNode() {
                    Elems = new Dictionary<string, INode>
                    {
                        {"abc", new IntegerNode("1")},
                        {"def", new IntegerNode("2")},
                    }
                },
				@"{""abc"":1,""def"":2}",
			},
			new object[] {
				new ObjectNode() {
                    Elems = new Dictionary<string, INode>
                    {
                        {"abc", new IntegerNode("1")},
                        {"def", new IntegerNode("2")},
                    }
                },
				@"  {  ""abc""  :  1  ,  ""def""  :  2  }  ",
			},

            // Arrays
			new object[] {
				new ArrayNode(),
				@"[]",
			},
			new object[] {
				new ArrayNode()
				{
					Elems = new List<INode>
					{
						new StringNode("abc"),
					}
				},
				@"[""abc""]",
			},
			new object[] {
				new ArrayNode() {
                    Elems = new List<INode>
					{
						new StringNode("abc"),
                        new IntegerNode("1"),
					}
                },
				@"[""abc"",1]",
			},
			new object[] {
				new ArrayNode() {
                    Elems = new List<INode>
					{
						new StringNode("abc"),
                        new IntegerNode("1"),
					}
                },
				@"  [  ""abc""  ,  1   ]  ",
			},

            // Compound
			new object[] {
				new ArrayNode() {
                    Elems = new List<INode>{
                        new ObjectNode(),
                        new ObjectNode {
                            Elems = new Dictionary<string, INode>{
                                {"a", new ObjectNode {
                                        Elems = new Dictionary<string, INode>{
                                            {"b",  new ArrayNode {
                                                    Elems = new List<INode>{
                                                        new NullNode(),
                                                        new IntegerNode("1"),
                                                        new StringNode("🍣"),
                                                    },
                                                }
                                            },
                                        },
                                    }
                                },
                            },
                        },
                    },
                },
				@"[{}, {""a"": {""b"": [null, 1, ""🍣""]}}]",
			},
		};
	}

	[TestFixtureSource("FixtureArgs")]
	class JsonReaderFailTests
	{
		INode _expected;
		string _src;

		public JsonReaderFailTests(INode expected, string src) {
			_expected = expected;
			_src = src;
		}

		[Test]
		public void ReadTest()
		{
			using(var textReader = new StringReader(_src))
			{
				var reader = new JsonReader(textReader);
                var ex = Assert.Throws<Exception>(() => reader.Read());

                // TODO: test exceptions
			}
		}

		//
		static object [] FixtureArgs = {
			// Numbers
			new object[] {
				new IntegerNode("123"),
				@"  012  ",
			},
		};
	}
}
