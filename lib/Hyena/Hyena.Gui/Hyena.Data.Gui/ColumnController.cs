//
// ColumnController.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
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
using System.Collections;
using System.Collections.Generic;

namespace Hyena.Data.Gui
{
    public class ColumnController : IEnumerable<Column>
    {
		ISortableColumn default_sort_column;
        ISortableColumn sort_column;

        protected List<Column> Columns { get; } = new List<Column>();

		public event EventHandler Updated;

        protected virtual void OnVisibilitiesChanged ()
        {
            OnUpdated ();
        }

        protected virtual void OnWidthsChanged ()
        {
        }

        protected void OnUpdated ()
        {
			Updated?.Invoke (this, EventArgs.Empty);
		}

        public void Clear ()
        {
            lock (this) {
                foreach (var column in Columns) {
                    column.VisibilityChanged -= OnColumnVisibilityChanged;
                    column.WidthChanged -= OnColumnWidthChanged;
                }
                Columns.Clear ();
            }

            OnUpdated ();
        }

        public void AddRange (params Column [] range)
        {
            lock (this) {
                foreach (var column in range) {
                    column.VisibilityChanged += OnColumnVisibilityChanged;
                    column.WidthChanged += OnColumnWidthChanged;
                }
                Columns.AddRange (range);
            }

            OnUpdated ();
        }

        public void Add (Column column)
        {
            lock (this) {
                column.VisibilityChanged += OnColumnVisibilityChanged;
                column.WidthChanged += OnColumnWidthChanged;
                Columns.Add (column);
            }

            OnUpdated ();
        }

        public void Insert (Column column, int index)
        {
            lock (this) {
                column.VisibilityChanged += OnColumnVisibilityChanged;
                column.WidthChanged += OnColumnWidthChanged;
                Columns.Insert (index, column);
            }

            OnUpdated ();
        }

        public void Remove (Column column)
        {
            lock (this) {
                column.VisibilityChanged -= OnColumnVisibilityChanged;
                column.WidthChanged -= OnColumnWidthChanged;
                Columns.Remove (column);
            }

            OnUpdated ();
        }

        public void Remove (int index)
        {
            lock (this) {
				var column = Columns[index];
                column.VisibilityChanged -= OnColumnVisibilityChanged;
                column.WidthChanged -= OnColumnWidthChanged;
                Columns.RemoveAt (index);
            }

            OnUpdated ();
        }

        public void Reorder (int index, int newIndex)
        {
            lock (this) {
				var column = Columns[index];
                Columns.RemoveAt (index);
                Columns.Insert (newIndex, column);
            }

            OnUpdated ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return Columns.GetEnumerator ();
        }

        IEnumerator<Column> IEnumerable<Column>.GetEnumerator ()
        {
            return Columns.GetEnumerator ();
        }

        public int IndexOf (Column column)
        {
            lock (this) {
                return Columns.IndexOf (column);
            }
        }

        public Column [] ToArray ()
        {
            return Columns.ToArray ();
        }

        void OnColumnVisibilityChanged (object o, EventArgs args)
        {
            OnVisibilitiesChanged ();
        }

        void OnColumnWidthChanged (object o, EventArgs args)
        {
            OnWidthsChanged ();
        }

        public Column this[int index] {
            get { return Columns[index]; }
        }

        public ISortableColumn DefaultSortColumn {
            get { return default_sort_column; }
            set { default_sort_column = value; }
        }

        public virtual ISortableColumn SortColumn {
            get { return sort_column; }
            set { sort_column = value;}
        }

        public int Count {
            get { return Columns.Count; }
        }

        public virtual bool EnableColumnMenu {
            get { return false; }
        }
    }
}
