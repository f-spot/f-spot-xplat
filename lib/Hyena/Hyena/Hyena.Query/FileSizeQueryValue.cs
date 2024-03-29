//
// FileSizeQueryValue.cs
//
// Authors:
//   Gabriel Burt <gburt@novell.com>
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2007-2008 Novell, Inc.
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
using System.Xml;

namespace Hyena.Query
{
	public enum FileSizeFactor : long {
        None = 1,
        KB = 1024,
        MB = 1048576,
        GB = 1073741824,
        TB = 1099511627776,
        PB = 1125899906842624
    }

    public class FileSizeQueryValue : IntegerQueryValue
    {
		public FileSizeFactor Factor { get; private set; } = FileSizeFactor.None;

		public FileSizeQueryValue ()
        {
        }

        public FileSizeQueryValue (long bytes)
        {
            value = bytes;
            IsEmpty = false;
            DetermineFactor ();
        }

        public double FactoredValue {
            get { return (double)value / (double)Factor; }
        }

        public override void ParseUserQuery (string input)
        {
            if (input.Length > 1 && (input[input.Length - 1] == 'b' || input[input.Length - 1] == 'B')) {
                input = input.Substring (0, input.Length - 1);
            }

			IsEmpty = !double.TryParse (input, out var double_value);

			if (IsEmpty && input.Length > 1) {
                IsEmpty = !double.TryParse (input.Substring (0, input.Length - 1), out double_value);
            }

            if (!IsEmpty) {
                switch (input[input.Length - 1]) {
                    case 'k': case 'K': Factor = FileSizeFactor.KB; break;
                    case 'm': case 'M': Factor = FileSizeFactor.MB; break;
                    case 'g': case 'G': Factor = FileSizeFactor.GB; break;
                    case 't': case 'T': Factor = FileSizeFactor.TB; break;
                    case 'p': case 'P': Factor = FileSizeFactor.PB; break;
                    default : Factor = FileSizeFactor.None; break;
                }
                value = (long)((double)Factor * double_value);
            }
        }

        public override void ParseXml (XmlElement node)
        {
            base.ParseUserQuery (node.InnerText);
            if (node.HasAttribute ("factor")) {
                Factor = (FileSizeFactor) Enum.Parse (typeof(FileSizeFactor), node.GetAttribute ("factor"));
            } else {
                DetermineFactor ();
            }
        }

        public override void AppendXml (XmlElement node)
        {
            base.AppendXml (node);
            node.SetAttribute ("factor", Factor.ToString ());
        }

        public void SetValue (double value, FileSizeFactor factor)
        {
            this.value = (long)(value * (double)factor);
            Factor = factor;
            IsEmpty = false;
        }

        protected void DetermineFactor ()
        {
            if (!IsEmpty && value != 0) {
                foreach (FileSizeFactor factor in Enum.GetValues (typeof(FileSizeFactor))) {
                    if (value >= (double)factor) {
                        Factor = factor;
                    }
                }
            }
        }

        public override string ToUserQuery ()
        {
            return ToUserQuery (false);
        }

        public string ToUserQuery (bool always_decimal)
        {
            if (Factor != FileSizeFactor.None) {
                return string.Format ("{0} {1}",
                    IntValue == 0
                        ? "0"
                        : StringUtil.DoubleToTenthsPrecision (((double)IntValue / (double)Factor), always_decimal),
                    Factor.ToString ()
                );
            } else {
                return base.ToUserQuery ();
            }
        }
    }
}
