//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System.IO;
using NUnit.Framework;

namespace VJson.UnitTests
{
    public class JsonWriterPrimitiveValueTests
    {
        [Test]
        public void ValueWriteTest()
        {
            using(var textWriter = new StringWriter())
            {
                var f = new JsonWriter(textWriter);
                f.WriteValue(1);

                var actual = textWriter.ToString();
                Assert.AreEqual("1", actual);
            }
        }
    }

    public class JsonWriterObjectTests
    {
        [Test]
        public void EmptyTest()
        {
            using (var textWriter = new StringWriter())
            {
                var f = new JsonWriter(textWriter);
                f.WriteObjectStart();
                f.WriteObjectEnd();

                var actual = textWriter.ToString();
                Assert.AreEqual(@"{}", actual);
            }
        }

        [Test]
        public void SingleTest()
        {
            using (var textWriter = new StringWriter())
            {
                var f = new JsonWriter(textWriter);
                f.WriteObjectStart();
                f.WriteObjectKey("foo");
                f.WriteValue(42);
                f.WriteObjectEnd();

                var actual = textWriter.ToString();
                Assert.AreEqual(@"{""foo"":42}", actual);
            }
        }

        [Test]
        public void MultiTest()
        {
            using (var textWriter = new StringWriter())
            {
                var f = new JsonWriter(textWriter);
                f.WriteObjectStart();
                f.WriteObjectKey("foo");
                f.WriteValue(42);

                f.WriteObjectKey("bar");
                f.WriteValue(84);
                f.WriteObjectEnd();

                var actual = textWriter.ToString();
                Assert.AreEqual(@"{""foo"":42,""bar"":84}", actual);
            }
        }

        [Test]
        public void NestedTest()
        {
            using (var textWriter = new StringWriter())
            {
                var f = new JsonWriter(textWriter);
                f.WriteObjectStart();
                f.WriteObjectKey("foo");

                f.WriteObjectStart();
                f.WriteObjectKey("bar");
                f.WriteValue(84);
                f.WriteObjectEnd();

                f.WriteObjectEnd();

                var actual = textWriter.ToString();
                Assert.AreEqual(@"{""foo"":{""bar"":84}}", actual);
            }
        }
    }

    public class JsonWriterArrayTests
    {
        [Test]
        public void EmptyTest()
        {
            using (var textWriter = new StringWriter())
            {
                var f = new JsonWriter(textWriter);
                f.WriteArrayStart();
                f.WriteArrayEnd();

                var actual = textWriter.ToString();
                Assert.AreEqual(@"[]", actual);
            }
        }

        [Test]
        public void SingleTest()
        {
            using(var textWriter = new StringWriter())
            {
                var f = new JsonWriter(textWriter);
                f.WriteArrayStart();
                f.WriteValue(42);
                f.WriteArrayEnd();

                var actual = textWriter.ToString();
                Assert.AreEqual(@"[42]", actual);
            }
        }

        [Test]
        public void MultiTest()
        {
            using(var textWriter = new StringWriter())
            {
                var f = new JsonWriter(textWriter);
                f.WriteArrayStart();
                f.WriteValue(42);
                f.WriteValue("aaa");
                f.WriteArrayEnd();

                var actual = textWriter.ToString();
                Assert.AreEqual(@"[42,""aaa""]", actual);
            }
        }

        [Test]
        public void NestedTest()
        {
            using(var textWriter = new StringWriter())
            {
                var f = new JsonWriter(textWriter);
                f.WriteArrayStart();
                f.WriteValue(42);
                f.WriteArrayStart();
                f.WriteValue("aaa");
                f.WriteArrayEnd();
                f.WriteArrayEnd();

                var actual = textWriter.ToString();
                Assert.AreEqual(@"[42,[""aaa""]]", actual);
            }
        }
    }

    public class JsonWriterCompoundTests
    {
        [Test]
        public void CompoundTest()
        {
            using (var textWriter = new StringWriter())
            {
                var f = new JsonWriter(textWriter);
                f.WriteArrayStart();

                f.WriteObjectStart();
                f.WriteObjectKey("foo");
                f.WriteValue(42);
                f.WriteObjectEnd();

                f.WriteObjectStart();
                f.WriteObjectKey("foo");
                f.WriteArrayStart();
                f.WriteValue(84);
                f.WriteArrayEnd();
                f.WriteObjectEnd();

                f.WriteArrayEnd();

                var actual = textWriter.ToString();
                Assert.AreEqual(@"[{""foo"":42},{""foo"":[84]}]", actual);
            }
        }
    }
}
