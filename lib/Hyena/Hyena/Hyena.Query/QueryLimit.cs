//
// QueryLimit.cs
//
// Authors:
//   Gabriel Burt <gburt@novell.com>
//
// Copyright (C) 2008 Novell, Inc.
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

namespace Hyena.Query
{
	public class QueryLimit
    {
		public string Name { get; }

        public string Label { get; set; }

        readonly bool row_based;
        public bool RowBased {
            get { return row_based; }
        }

        public int Factor { get; } = 1;

        public string Column { get; }

        public QueryLimit (string name, string label, string column, int factor) : this (name, label, false)
        {
            Column = column;
            Factor = factor;
        }

        public QueryLimit (string name, string label, bool row_based)
        {
            Name = name;
            Label = label;
            this.row_based = row_based;
        }

        public string ToSql (IntegerQueryValue limit_value)
        {
            return RowBased ? string.Format ("LIMIT {0}", limit_value.ToSql ()) : null;
        }
    }
}
