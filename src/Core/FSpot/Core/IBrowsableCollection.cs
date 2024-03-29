//
// IBrowsableCollection.cs
//
// Author:
//   Stephane Delcroix <stephane@delcroix.org>
//   Ruben Vermeersch <ruben@savanne.be>
//
// Copyright (C) 2008-2010 Novell, Inc.
// Copyright (C) 2008 Stephane Delcroix
// Copyright (C) 2010 Ruben Vermeersch
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
// THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Collections.Generic;

namespace FSpot.Core
{
	// FIXME, This should probably become an ObservableCollection instead?
	public delegate void IBrowsableCollectionChangedHandler (IBrowsableCollection collection);
	public delegate void IBrowsableCollectionItemsChangedHandler (IBrowsableCollection collection, BrowsableEventArgs args);

	public interface IBrowsableCollection
	{
		IEnumerable<IPhoto> Items {
			get;
		}

		int IndexOf (IPhoto item);

		IPhoto this [int index] {
			get;
		}

		int Count {
			get;
		}

		bool Contains (IPhoto item);

		// FIXME the Changed event needs to pass along information
		// about the items that actually changed if possible.  For things like
		// TrayView everything has to be redrawn when a single
		// item has been added or removed which adds too much
		// overhead.
		event IBrowsableCollectionChangedHandler Changed;
		event IBrowsableCollectionItemsChangedHandler ItemsChanged;

		void MarkChanged (int index, IBrowsableItemChanges changes);
	}
}
