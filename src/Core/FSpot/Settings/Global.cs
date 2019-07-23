//
// Global.cs
//
// Author:
//   Stephane Delcroix <sdelcroix@novell.com>
//   Ruben Vermeersch <ruben@savanne.be>
//   Larry Ewing <lewing@novell.com>
//
// Copyright (C) 2004-2010 Novell, Inc.
// Copyright (C) 2007-2008 Stephane Delcroix
// Copyright (C) 2010 Ruben Vermeersch
// Copyright (C) 2004-2007 Larry Ewing
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

namespace FSpot.Settings
{
	// FIXME, Clean up the Global page
	public static class Global
	{
		public static string HomeDirectory {
			get { return Path.Combine (Environment.GetEnvironmentVariable ("HOME"), string.Empty); }
		}

		//$XDG_CONFIG_HOME/f-spot or $HOME/.config/f-spot
		static readonly string xdg_config_home = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);

        public static string BaseDirectory { get; set; } = Path.Combine(xdg_config_home, "f-spot");

		public static Uri PhotoUri { get; set; }

		public static string HelpDirectory {
			get {
				// path is relative
				return "f-spot";
			}
		}

		public static Cms.Profile DisplayProfile { get; set; }

		public static Cms.Profile DestinationProfile { get; set; }

		// FIXME, Gtk Icon Theme
		//static Gtk.IconTheme icon_theme;
		//public static Gtk.IconTheme IconTheme {
		//	get {
		//		if (icon_theme == null) {
		//			icon_theme = Gtk.IconTheme.Default;
		//			icon_theme.AppendSearchPath (Path.Combine (Defines.APP_DATA_DIR, "icons"));
		//		}
		//		return icon_theme;
		//	}
		//}

		//static string [] default_rc_files;
		//public static string [] DefaultRcFiles {
		//	get {
		//		if (default_rc_files == null)
		//			default_rc_files = Gtk.Rc.DefaultFiles;
		//		return default_rc_files;
		//	}
		//	set { default_rc_files = value; }
		//}

			// FIXME, PageSetup
		public static object PageSetup { get; set; }
		//public static Gtk.PageSetup PageSetup { get; set; }
	}
}
