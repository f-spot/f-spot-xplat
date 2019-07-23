//
// DatabaseColumnAttribute.cs
//
// Author:
//   Scott Peterson  <lunchtimemama@gmail.com>
//
// Copyright (C) 2007 Novell, Inc.
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
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hyena.Data.Sqlite
{
    [Flags]
    public enum DatabaseColumnConstraints
    {
        NotNull = 1,
        PrimaryKey = 2,
        Unique = 4
    }

    public abstract class AbstractDatabaseColumnAttribute : Attribute
    {
		bool select = true;

        public AbstractDatabaseColumnAttribute ()
        {
        }

        public AbstractDatabaseColumnAttribute (string column_name)
        {
            ColumnName = column_name;
        }

        public string ColumnName { get; }

        public bool Select {
            get { return select; }
            set { select = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DatabaseColumnAttribute : AbstractDatabaseColumnAttribute
    {
		string default_value;

		public DatabaseColumnAttribute ()
        {
        }

        public DatabaseColumnAttribute (string column_name) : base (column_name)
        {
        }

        public DatabaseColumnConstraints Constraints { get; set; }

        public string DefaultValue {
            get { return default_value; }
            set { default_value = value; }
        }

        public string Index { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class VirtualDatabaseColumnAttribute : AbstractDatabaseColumnAttribute
    {
		readonly string foreign_key;

        public VirtualDatabaseColumnAttribute (string column_name, string target_table, string local_key, string foreign_key)
            : base (column_name)
        {
            TargetTable = target_table;
            LocalKey = local_key;
            this.foreign_key = foreign_key;
        }

        public string TargetTable { get; }

        public string LocalKey { get; }

        public string ForeignKey {
            get { return foreign_key; }
        }
    }
}
