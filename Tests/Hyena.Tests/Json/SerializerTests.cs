//
// SerializerTests.cs
//
// Authors:
//   Sandy Armstrong <sanfordarmstrong@gmail.com>
//
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

using Hyena.Json;

using NUnit.Framework;

namespace Hyena.Json.Tests
{
    [TestFixture]
    public class SerializerTests : Hyena.Tests.TestBase
    {
        [Test]
        public void SerializeBoolTest ()
        {
            var ser = new Serializer (true);
            Assert.AreEqual ("true", ser.Serialize ());
            ser.SetInput (false);
            Assert.AreEqual ("false", ser.Serialize ());
        }

        [Test]
        public void SerializeIntTest ()
        {
            var ser = new Serializer (12);
            Assert.AreEqual ("12", ser.Serialize ());
            ser.SetInput (-658);
            Assert.AreEqual ("-658", ser.Serialize ());
            ser.SetInput (0);
            Assert.AreEqual ("0", ser.Serialize ());
        }

        [Test]
        public void SerializeDoubleTest ()
        {
            var ser = new Serializer (12.5);
            Assert.AreEqual ("12.5", ser.Serialize ());
            ser.SetInput (-658.1);
            Assert.AreEqual ("-658.1", ser.Serialize ());
            ser.SetInput (0.0);
            Assert.AreEqual ("0", ser.Serialize ());
            ser.SetInput (0.1);
            Assert.AreEqual ("0.1", ser.Serialize ());
        }

        [Test]
        public void SerializeStringTest ()
        {
            VerifyString ("The cat\njumped \"over\" the rat! We escape with \\\"",
                          @"""The cat\njumped \""over\"" the rat! We escape with \\\""""");
        }

        [Test]
        public void EscapedCharactersTest ()
        {
            VerifyString ("\\", @"""\\""");
            VerifyString ("\b", @"""\b""");
            VerifyString ("\f", @"""\f""");
            VerifyString ("\n", @"""\n""");
            VerifyString ("\r", @"""\r""");
            VerifyString ("\t", @"""\t""");
            VerifyString ("\u2022", "\"\u2022\"");
        }

        void VerifyString (string original, string expectedSerialized)
        {
            var ser = new Serializer (original);
            string output = ser.Serialize ();
            Assert.AreEqual (expectedSerialized, output, "Serialized Output");
            Assert.AreEqual (original, new Deserializer (output).Deserialize (),
                             "Input should be identical after serialized then deserialized back to string");
        }

        [Test]
        public void SerializeNullTest ()
        {
            var ser = new Serializer (null);
            Assert.AreEqual ("null", ser.Serialize ());
        }

        [Test]
        public void SerializeArrayTest ()
        {
			var simpleArray = new JsonArray {
				1,
				"text",
				0.1,
				"5",
				false,
				null
			};

			var ser = new Serializer (simpleArray);
            Assert.AreEqual ("[1,\"text\",0.1,\"5\",false,null]",
                             ser.Serialize ());

            var emptyArray = new JsonArray ();
            ser.SetInput (emptyArray);
            Assert.AreEqual ("[]", ser.Serialize ());
        }

        // TODO: Test arrays/objects in each other, various levels deep

        [Test]
        public void SerializeObjectTest ()
        {
			var obj1 = new JsonObject {
				["intfield"] = 1,
				["text field"] = "text",
				["double-field"] = 0.1,
				["Boolean, Field"] = true,
				["\"Null\"\nfield"] = null
			};

			var ser = new Serializer (obj1);
            // Test object equality, since order not guaranteed
            string output = ser.Serialize ();
            var reconstitutedObj1 = (JsonObject) new Deserializer (output).Deserialize ();
            AssertJsonObjectsEqual (obj1, reconstitutedObj1);

			var obj2 = new JsonObject {
				["double-field"] = 0.1,
				["\"Null\"\nfield"] = null
			};
			ser.SetInput (obj2);
            output = ser.Serialize ();
            Assert.IsTrue ((output == "{\"double-field\":0.1,\"\\\"Null\\\"\\nfield\":null}") ||
                           (output == "{\"\\\"Null\\\"\\nfield\":null,\"double-field\":0.1}"),
                           "Serialized output format");
        }

        void AssertJsonObjectsEqual (JsonObject expectedObj, JsonObject actualObj)
        {
            Assert.AreEqual (expectedObj.Count, actualObj.Count, "Field count");
            foreach (var expectedPair in expectedObj)
                Assert.AreEqual (expectedPair.Value,
                                 actualObj [expectedPair.Key],
                                 expectedPair.Key + " field");
        }
    }
}
