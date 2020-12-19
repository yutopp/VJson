//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace VJson.UnitTests
{
    class JsonReaderPassTests
    {
        [Test]
        [TestCaseSource("FixtureArgs")]
        public void ReadTest(INode expected, string src)
        {
            using (var s = new MemoryStream(Encoding.UTF8.GetBytes(src)))
            using (var r = new JsonReader(s))
            {
                var actual = r.Read();

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        //
        static object[] FixtureArgs = {
            // Boolean
			new object[] {
                new BooleanNode(true),
                @"true",
            },
            new object[] {
                new BooleanNode(false),
                @"false",
            },
            new object[] {
                new BooleanNode(true),
                @"  true  ",
            },
            new object[] {
                new BooleanNode(false),
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
                new IntegerNode(123),
                @"123",
            },
            new object[] {
                new IntegerNode(123),
                @"  123  ",
            },
            new object[] {
                new IntegerNode(0),
                @"0",
            },
            new object[] {
                new IntegerNode(9),
                @"9",
            },
            new object[] {
                new IntegerNode(-123),
                @"-123",
            },
            new object[] {
                new IntegerNode(-0),
                @"-0",
            },
            new object[] {
                new IntegerNode(-9),
                @"-9",
            },
            new object[] {
                new FloatNode(-9.0),
                @"-9.0",
            },
            new object[] {
                new FloatNode(-9.0e0),
                @"-9.0e0",
            },
            new object[] {
                new FloatNode(-9.0e+0),
                @"-9.0e+0",
            },
            new object[] {
                new FloatNode(-9.0e-0),
                @"-9.0e-0",
            },
            new object[] {
                new FloatNode(-9e0),
                @"-9e0",
            },
            new object[] {
                new FloatNode(-9e+0),
                @"-9e+0",
            },
            new object[] {
                new FloatNode(-9e-0),
                @"-9e-0",
            },
            new object[] {
                new FloatNode(3.14),
                @"3.14",
            },
            new object[] {
                new FloatNode(-3.14),
                @"-3.14",
            },
            new object[] {
                new FloatNode(3.14e12),
                @"3.14e12",
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
                new StringNode("\""),
                "\"\\\"\"",
            },
            new object[] {
                new StringNode("\\"),
                "\"\\\\\"",
            },
            new object[] {
                new StringNode("/"),
                "\"\\/\"",
            },
            new object[] {
                new StringNode("/"),
                "\"/\"",
            },
            new object[] {
                new StringNode("\b"),
                @"""\b""",
            },
            new object[] {
                new StringNode("\n"),
                @"""\n""",
            },
            new object[] {
                new StringNode("\r"),
                @"""\r""",
            },
            new object[] {
                new StringNode("\t"),
                @"""\t""",
            },
            new object[] {
                new StringNode("あ"),
                @"""\u3042""",
            },
            new object[] {
                new StringNode("http://"),
                @"""http:\/\/""",
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
                        {"abc", new IntegerNode(1)},
                    }
                },
                @"{""abc"":1}",
            },
            new object[] {
                new ObjectNode() {
                    Elems = new Dictionary<string, INode>
                    {
                        {"abc", new IntegerNode(1)},
                        {"def", new IntegerNode(2)},
                    }
                },
                @"{""abc"":1,""def"":2}",
            },
            new object[] {
                new ObjectNode() {
                    Elems = new Dictionary<string, INode>
                    {
                        {"abc", new IntegerNode(1)},
                        {"def", new IntegerNode(2)},
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
                        new IntegerNode(1),
                    }
                },
                @"[""abc"",1]",
            },
            new object[] {
                new ArrayNode() {
                    Elems = new List<INode>
                    {
                        new StringNode("abc"),
                        new IntegerNode(1),
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
                                                        new IntegerNode(1),
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

    class JsonReaderFailTests
    {
        [Test]
        [TestCaseSource("FixtureArgs")]
        public void ReadTest(string expectedMsg, string src)
        {
            using (var s = new MemoryStream(Encoding.UTF8.GetBytes(src)))
            using (var r = new JsonReader(s))
            {
                var ex = Assert.Throws<ParseFailedException>(() => r.Read());
                Assert.AreEqual(expectedMsg, ex.Message);
            }
        }

        //
        static object[] FixtureArgs = {
            new object[] {
                "A node \"value\" is expected but '<EOS>' is provided (at position 0)",
                "",
            },

			new object[] {
                "A node \"EOS\" is expected but '1' is provided (at position 3)",
                @"  012  ",
            },
        };
    }
}
