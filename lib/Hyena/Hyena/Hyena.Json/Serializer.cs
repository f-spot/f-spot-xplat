//
// Serializer.cs
//
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
//
// Authors:
//   Sandy Armstrong <sanfordarmstrong@gmail.com>
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Hyena.Json
{
	public class Serializer
    {
        const string serializedNull = "null";
        const string serializedTrue = "true";
        const string serializedFalse = "false";

        object input;

        public Serializer () { }
        public Serializer (object input) { SetInput (input); }

        public void SetInput (object input)
        {
            this.input = input;
        }

        // TODO: Support serialize to stream?

        public string Serialize ()
        {
            return Serialize (input);
        }

        string SerializeBool (bool val)
        {
            return val ? serializedTrue : serializedFalse;
        }

        string SerializeInt (int val)
        {
            return val.ToString ();
        }

        string SerializeDouble (double val)
        {
            return val.ToString (System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }

        // TODO: exponent stuff

        string SerializeString (string val)
        {
            // TODO: More work, escaping, etc
            return "\"" +
                val.Replace ("\\", "\\\\").
                    Replace ("\"", "\\\"").
                    Replace ("\b", "\\b").
                    Replace ("\f", "\\f").
                    Replace ("\n", "\\n").
                    Replace ("\r", "\\r").
                    Replace ("\t", "\\t") +
                "\"";
        }

        string SerializeEnumerable (IEnumerable array)
        {
            var builder = new StringBuilder ("[");
            int i = 0;
            foreach (var obj in array) {
                builder.Append (Serialize (obj));
                builder.Append (",");
                i++;
            }
            // Get rid of trailing comma
            if (i > 0)
                builder.Remove (builder.Length - 1, 1);
            builder.Append ("]");
            return builder.ToString ();
        }

        string SerializeDictionary (Dictionary<string, object> obj)
        {
            var builder = new StringBuilder ("{");
            foreach (var pair in obj) {
                builder.Append (SerializeString (pair.Key));
                builder.Append (":");
                builder.Append (Serialize (pair.Value));
                builder.Append (",");
            }
            // Get rid of trailing comma
            if (obj.Count > 0)
                builder.Remove (builder.Length - 1, 1);
            builder.Append ("}");
            return builder.ToString ();
        }

        string Serialize (object unknownObj)
        {
            if (unknownObj == null)
                return serializedNull;

            bool? b = unknownObj as bool?;
            if (b.HasValue)
                return SerializeBool (b.Value);

            int? i = unknownObj as int?;
            if (i.HasValue)
                return SerializeInt (i.Value);

            double? d = unknownObj as double?;
            if (d.HasValue)
                return SerializeDouble (d.Value);

			if (unknownObj is string s)
				return SerializeString (s);

			if (unknownObj is Dictionary<string, object> o)
				return SerializeDictionary (o);

			if (unknownObj is IEnumerable a)
				return SerializeEnumerable (a);

			throw new ArgumentException ("Cannot serialize anything but doubles, integers, strings, JsonObjects, and JsonArrays");
        }
    }
}
