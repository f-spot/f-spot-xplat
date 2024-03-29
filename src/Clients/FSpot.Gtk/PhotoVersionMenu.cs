//
// PhotoVersionMenu.cs
//
// Author:
//   Ettore Perazzoli <ettore@src.gnome.org>
//   Mike Gemünde <mike@gemuende.de>
//
// Copyright (C) 2003-2010 Novell, Inc.
// Copyright (C) 2003 Ettore Perazzoli
// Copyright (C) 2010 Mike Gemünde
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
using System.Collections.Generic;

using Gtk;

using FSpot.Core;
using FSpot.Translations;

public class PhotoVersionMenu : Menu
{
	public IPhotoVersion Version { get; private set; }

	public delegate void VersionChangedHandler (PhotoVersionMenu menu);
	public event VersionChangedHandler VersionChanged;

	readonly Dictionary<MenuItem, IPhotoVersion> versionMapping;

	void HandleMenuItemActivated (object sender, EventArgs args)
	{
		if (sender is MenuItem item && versionMapping.ContainsKey (item)) {
			Version = versionMapping[item];
			VersionChanged (this);
		}
	}

	public PhotoVersionMenu (IPhoto photo)
	{
		Version = photo.DefaultVersion;

		versionMapping = new Dictionary<MenuItem, IPhotoVersion> ();

		foreach (var version in photo.Versions) {
			var menu_item = new MenuItem (version.Name);
			menu_item.Show ();
			menu_item.Sensitive = true;
			var child = ((Gtk.Label)menu_item.Child);

			if (version == photo.DefaultVersion) {
				child.UseMarkup = true;
				child.Markup = $"<b>{version.Name}</b>";
			}

			versionMapping.Add (menu_item, version);

			Append (menu_item);
		}

		if (versionMapping.Count == 1) {
			var no_edits_menu_item = new MenuItem (Strings.ParenNoEditsParen);
			no_edits_menu_item.Show ();
			no_edits_menu_item.Sensitive = false;
			Append (no_edits_menu_item);
		}
	}
}
