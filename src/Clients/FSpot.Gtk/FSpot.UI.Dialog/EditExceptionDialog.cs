//
// EditExceptionDialog.cs
//
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
//   Stephane Delcroix <sdelcroix@novell.com>
//
// Copyright (C) 2008-2010 Novell, Inc.
// Copyright (C) 2010 Ruben Vermeersch
// Copyright (C) 2008 Stephane Delcroix
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

using System;

using Gtk;

using Hyena;
using Hyena.Widgets;

using FSpot.Core;
using FSpot.Translations;

namespace FSpot.UI.Dialog
{
	public class EditException : Exception
	{
		public IPhoto Item { get; private set; }

		public EditException (IPhoto item, Exception e) : base (Strings.ReceivedExceptionXUnableToSavePhotoName (e.Message, item.Name), e)
		{
			Item = item;
		}
	}

	public class EditExceptionDialog : HigMessageDialog
	{
		const int MaxErrors = 10;

		public EditExceptionDialog (Gtk.Window parent, Exception [] errors) : base (parent, DialogFlags.DestroyWithParent,
									Gtk.MessageType.Error, ButtonsType.Ok, Strings.ErrorEditingPhoto, GenerateMessage (errors))
		{
			foreach (var e in errors)
				Log.Exception (e);
		}

		public EditExceptionDialog (Gtk.Window parent, Exception e, IPhoto item) : this (parent, new EditException (item, e))
		{
		}

		public EditExceptionDialog (Gtk.Window parent, Exception e) : this (parent, new Exception [] { e })
		{
		}

		static string GenerateMessage (Exception [] errors)
		{
			string desc = string.Empty;
			for (int i = 0; i < errors.Length && i < MaxErrors; i++) {
				var e = errors [i];
				desc += e.Message + Environment.NewLine;
			}
			return desc;
		}
	}
}
