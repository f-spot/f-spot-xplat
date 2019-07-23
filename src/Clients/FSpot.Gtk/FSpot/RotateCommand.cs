//
// RotateCommand.cs
//
// Author:
//   Larry Ewing <lewing@novell.com>
//   Gabriel Burt <gabriel.burt@gmail.com>
//   Ruben Vermeersch <ruben@savanne.be>
//
// Copyright (C) 2004-2010 Novell, Inc.
// Copyright (C) 2004-2006 Larry Ewing
// Copyright (C) 2008 Gabriel Burt
// Copyright (C) 2009-2010 Ruben Vermeersch
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
using System.IO;

using Gtk;

using FSpot.Core;
using FSpot.Settings;
using FSpot.Thumbnail;
using FSpot.Translations;
using FSpot.UI.Dialog;
using FSpot.Utils;

using Hyena;
using Hyena.Widgets;

namespace FSpot
{
	public class RotateException : ApplicationException
	{
		public bool ReadOnly;
		public string Path { get; private set; }

		public RotateException (string msg, string path) : this (msg, path, false) { }

		public RotateException (string msg, string path, bool ro) : base (msg)
		{
			Path = path;
			ReadOnly = ro;
		}
	}

	public enum RotateDirection
	{
		Clockwise,
		Counterclockwise,
	}

	public class RotateOperation
	{
		readonly IPhoto item;
		readonly RotateDirection direction;
		bool done;

		public RotateOperation (IPhoto item, RotateDirection direction)
		{
			this.item = item;
			this.direction = direction;
			done = false;
		}

		static void RotateOrientation (string originalPath, RotateDirection direction)
		{
			try {
				var uri = new Uri (originalPath);
				using (var metadata = Metadata.Parse (uri)) {
					metadata.EnsureAvailableTags ();
					var tag = metadata.ImageTag;
					var orientation = direction == RotateDirection.Clockwise
						? Utils.PixbufUtils.Rotate90 (tag.Orientation)
						: Utils.PixbufUtils.Rotate270 (tag.Orientation);

					tag.Orientation = orientation;
					var alwaysSidecar = Preferences.Get<bool> (Preferences.METADATA_ALWAYS_USE_SIDECAR);
					metadata.SaveSafely (uri, alwaysSidecar);
					App.Instance.Container.Resolve<IThumbnailService> ().DeleteThumbnails (uri);
				}
			} catch (Exception e) {
				Log.DebugException (e);
				throw new RotateException (Strings.UnableToRotateThisTypeOfPhoto, originalPath);
			}
		}

		void Rotate (string originalPath, RotateDirection dir)
		{
			RotateOrientation (originalPath, dir);
		}

		public bool Step ()
		{
			if (done)
				return false;

			var info = new FileInfo (item.DefaultVersion.Uri.AbsolutePath);
			if (info.IsReadOnly)
				throw new RotateException (Strings.UnableToRotateReadonlyFile, item.DefaultVersion.Uri.AbsoluteUri, true);

			Rotate (item.DefaultVersion.Uri.AbsoluteUri, direction);

			done = true;
			return !done;
		}
	}

	public class RotateMultiple
	{
		readonly RotateDirection direction;
		RotateOperation op;

		public int Index { get; private set; }

		public IPhoto[] Items { get; private set; }

		public RotateMultiple (IPhoto[] items, RotateDirection direction)
		{
			this.direction = direction;
			Items = items;
			Index = 0;
		}

		public bool Step ()
		{
			if (Index >= Items.Length)
				return false;

			if (op == null)
				op = new RotateOperation (Items[Index], direction);

			if (op.Step ())
				return true;
			else {
				Index++;
				op = null;
			}

			return (Index < Items.Length);
		}
	}

	public class RotateCommand
	{
		readonly Gtk.Window parentWindow;

		public RotateCommand (Gtk.Window parent_window)
		{
			parentWindow = parent_window;
		}

		public bool Execute (RotateDirection direction, IPhoto[] items)
		{
			ProgressDialog progress_dialog = null;

			if (items.Length > 1)
				progress_dialog = new ProgressDialog (Strings.RotatingPhotos, ProgressDialog.CancelButtonType.Stop, items.Length, parentWindow);

			var op = new RotateMultiple (items, direction);
			int readonly_count = 0;
			bool done = false;
			int index = 0;

			while (!done) {
				if (progress_dialog != null && op.Index != -1 && index < items.Length)
					if (progress_dialog.Update (Strings.RotatingPhotoX (op.Items[op.Index].Name)))
						break;

				try {
					done = !op.Step ();
				} catch (RotateException re) {
					if (!re.ReadOnly)
						RunGenericError (re, re.Path, re.Message);
					else
						readonly_count++;
				} catch (GLib.GException) {
					readonly_count++;
				} catch (DirectoryNotFoundException e) {
					RunGenericError (e, op.Items[op.Index].DefaultVersion.Uri.LocalPath, Strings.DirectoryNotFound);
				} catch (FileNotFoundException e) {
					RunGenericError (e, op.Items[op.Index].DefaultVersion.Uri.LocalPath, Strings.FileNotFound);
				} catch (Exception e) {
					RunGenericError (e, op.Items[op.Index].DefaultVersion.Uri.LocalPath);
				}
				index++;
			}

			progress_dialog?.Destroy ();

			if (readonly_count > 0)
				RunReadonlyError (readonly_count);

			return true;
		}

		void RunReadonlyError (int readonly_count)
		{
			string notice = Catalog.GetPluralString (Strings.UnableToRotatePhoto, Strings.UnableToRotateXPhotos (readonly_count), readonly_count);
			string desc = Catalog.GetPluralString (Strings.ThePhotoCouldNotBeRotatedBecauseItIsOnAReadonlyFileSystemOrMedia,
				Strings.XPhotosCouldNotBeRotatedBecauseItIsOnAReadonlyFileSystemOrMedia (readonly_count), readonly_count);

			var md = new HigMessageDialog (parentWindow, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, notice, desc);
			md.Run ();
			md.Destroy ();
		}

		// FIXME shouldn't need this method, should catch all exceptions explicitly
		// so can present translated error messages.
		void RunGenericError (Exception e, string path)
		{
			RunGenericError (e, path, e.Message);
		}

		void RunGenericError (Exception e, string path, string msg)
		{
			string longmsg = Strings.ReceivedErrorXWhileAttemptingToRotateY (msg, Path.GetFileName (path));

			var md = new HigMessageDialog (parentWindow, DialogFlags.DestroyWithParent,
				MessageType.Warning, ButtonsType.Ok, Strings.ErrorWhileRotatingPhoto, longmsg);

			md.Run ();
			md.Destroy ();
		}
	}
}