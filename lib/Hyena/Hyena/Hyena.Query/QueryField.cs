//
// QueryField.cs
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
using System.Text;
using System.Collections.Generic;

namespace Hyena.Query
{
    public class QueryField : IAliasedObject
    {
		readonly bool no_custom_format;
		readonly bool column_lowered;

        public Type[] ValueTypes { get; }

        public string Name { get; set; }

        public string PropertyName { get; set; }

        public string Label { get; set; }

        string short_label;
        public string ShortLabel {
            get { return short_label ?? Label; }
            set { short_label = value; }
        }

        public string[] Aliases { get; }

        public string PrimaryAlias {
            get { return Aliases[0]; }
        }

        public string Column { get; }

        readonly bool is_default;
        public bool IsDefault {
            get { return is_default; }
        }

        public QueryField (string name, string propertyName, string label, string column, params string [] aliases)
            : this (name, propertyName, label, column, false, aliases)
        {
        }

        public QueryField (string name, string propertyName, string label, string column, bool isDefault, params string [] aliases)
            : this (name, propertyName, label, column, new Type [] {typeof(StringQueryValue)}, isDefault, aliases)
        {
        }

        public QueryField (string name, string propertyName, string label, string column, Type valueType, params string [] aliases)
            : this (name, propertyName, label, column, new Type [] {valueType}, false, aliases)
        {
        }

        public QueryField (string name, string propertyName, string label, string column, Type [] valueTypes, params string [] aliases)
            : this (name, propertyName, label, column, valueTypes, false, aliases)
        {
        }

        public QueryField (string name, string propertyName, string label, string column, Type [] valueTypes, bool isDefault, params string [] aliases)
        {
            Name = name;
            PropertyName = propertyName;
            Label = label;
            Column = column;
            ValueTypes = valueTypes;
            is_default = isDefault;
            Aliases = aliases;

            no_custom_format = (Column.IndexOf ("{0}") == -1 && Column.IndexOf ("{1}") == -1);
            column_lowered = (Column.IndexOf ("Lowered") != -1);

            if (!no_custom_format) {
                // Ensure we have parens around any custom 'columns' that may be an OR of two columns
                Column = string.Format ("({0})", Column);
            }

            foreach (var value_type in valueTypes) {
                QueryValue.AddValueType (value_type);
            }
        }

        public IEnumerable<QueryValue> CreateQueryValues ()
        {
            foreach (var type in ValueTypes) {
                yield return Activator.CreateInstance (type) as QueryValue;
            }
        }

        public string ToTermString (string op, string value)
        {
            return ToTermString (PrimaryAlias, op, value);
        }

        public string ToSql (Operator op, QueryValue qv)
        {
            string value = qv.ToSql (op) ?? string.Empty;

            if (op == null) op = qv.OperatorSet.First;

            var sb = new StringBuilder ();

            if (no_custom_format) {
                string column_with_key = Column;
                if (qv is StringQueryValue && !(column_lowered || qv is ExactStringQueryValue)) {
                    column_with_key = string.Format ("HYENA_SEARCH_KEY({0})", Column);
                }
                sb.AppendFormat ("{0} {1}", column_with_key, string.Format (op.SqlFormat, value));

                if (op.IsNot) {
                    return string.Format ("({0} IS NULL OR {1})", Column, sb.ToString ());
                } else {
                    return string.Format ("({0} IS NOT NULL AND {1})", Column, sb.ToString ());
                }
            } else {
                sb.AppendFormat (
                    Column, string.Format (op.SqlFormat, value),
                    value, op.IsNot ? "NOT" : null
                );
            }

            return sb.ToString ();
        }

        public static string ToTermString (string alias, string op, string value)
        {
            if (!string.IsNullOrEmpty (value)) {
                value = string.Format (
                    "{1}{0}{1}",
                    value, value.IndexOf (" ") == -1 ? string.Empty : "\""
                );
            } else {
                value = string.Empty;
            }
            return string.IsNullOrEmpty (alias)
                ? value
                : string.Format ("{0}{1}{2}", alias, op, value);
        }
    }
}
