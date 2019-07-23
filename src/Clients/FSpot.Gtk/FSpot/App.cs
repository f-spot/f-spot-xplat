//
// App.cs
//
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
//   Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (C) 2009-2010 Novell, Inc.
// Copyright (C) 2010 Ruben Vermeersch
// Copyright (C) 2009-2010 Stephane Delcroix
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Unique;

using Hyena;

using FSpot.Core;
using FSpot.Database;
using FSpot.Imaging;
using FSpot.Settings;
using FSpot.Thumbnail;
using FSpot.Translations;
using FSpot.Utils;

namespace FSpot
{
	public class App : Unique.App
	{
		static readonly object sync_handle = new object ();

		static App app;
		public static App Instance {
			get {
				lock (sync_handle) {
					if (app == null)
						app = new App ();
				}
				return app;
			}
		}

		Thread constructing_organizer = null;

		public MainWindow Organizer {
			get {
				lock (sync_handle) {
					if (organizer == null) {
						if (constructing_organizer == Thread.CurrentThread) {
							throw new Exception ("Recursively tried to acquire App.Organizer!");
						}

						constructing_organizer = Thread.CurrentThread;
						organizer = new MainWindow (Database);
						Register (organizer.Window);
					}
				}
				return organizer;
			}
		}

		public Db Database {
			get {
				lock (sync_handle) {
					if (db == null) {
						if (!File.Exists (Global.BaseDirectory))
							Directory.CreateDirectory (Global.BaseDirectory);

						db = new Db (Container.Resolve<IImageFileFactory> (), Container.Resolve<IThumbnailService> (), new UpdaterUI ());

						try {
							db.Init (Path.Combine (Global.BaseDirectory, "photos.db"), true);
						} catch (Exception e) {
							new UI.Dialog.RepairDbDialog (e, db.Repair (), null);
							db.Init (Path.Combine (Global.BaseDirectory, "photos.db"), true);
						}
					}
				}
				return db;
			}
		}

		public TinyIoCContainer Container {
			get { return TinyIoCContainer.Current; }
		}

		public void Import (string path)
		{
			if (IsRunning) {
				var md = new MessageData {
					Text = path
				};
				SendMessage (Command.Import, md);
				return;
			}
			HandleImport (path);
		}

		public void Organize ()
		{
			if (IsRunning) {
				SendMessage (Command.Organize, null);
				return;
			}
			HandleOrganize ();
		}

		public void Shutdown ()
		{
			if (IsRunning) {
				SendMessage (Command.Shutdown, null);
				return;
			}
			HandleShutdown ();
		}

		public void Slideshow (string tagname)
		{
			if (IsRunning) {
				var md = new MessageData {
					Text = tagname ?? string.Empty
				};
				SendMessage (Command.Slideshow, md);

				return;
			}
			HandleSlideshow (tagname);
		}

		public void View (Uri uri)
		{
			View (new[] { uri });
		}

		public void View (IEnumerable<Uri> uris)
		{
			var uri_s = from uri in uris select uri.ToString ();
			View (uri_s);
		}

		public void View (string uri)
		{
			View (new[] { uri });
		}

		public void View (IEnumerable<string> uris)
		{
			if (IsRunning) {
				var md = new MessageData {
					Uris = uris.ToArray ()
				};
				SendMessage (Command.View, md);
				return;
			}
			HandleView (uris.ToArray ());
		}
		enum Command
		{
			Invalid = 0,
			Import,
			View,
			Organize,
			Shutdown,
			Version,
			Slideshow,
		}

		readonly List<Gtk.Window> toplevels;
		MainWindow organizer;
		Db db;

		App () : base ("org.gnome.FSpot.Core", null,
				  "Import", Command.Import,
				  "View", Command.View,
				  "Organize", Command.Organize,
				  "Shutdown", Command.Shutdown,
				  "Slideshow", Command.Slideshow)
		{
			toplevels = new List<Gtk.Window> ();
			if (IsRunning) {
				Log.Information ("Found active FSpot process");
			} else {
				MessageReceived += HandleMessageReceived;
			}

			FileSystem.ModuleController.RegisterTypes (Container);
			Imaging.ModuleController.RegisterTypes (Container);
			FSpot.Import.ModuleController.RegisterTypes (Container);
			Thumbnail.ModuleController.RegisterTypes (Container);
		}

		void SendMessage (Command command, MessageData md)
		{
			SendMessage ((Unique.Command)command, md);
		}

		void HandleMessageReceived (object sender, MessageReceivedArgs e)
		{
			switch ((Command)e.Command) {
			case Command.Import:
				HandleImport (e.MessageData.Text);
				e.RetVal = Response.Ok;
				break;
			case Command.Organize:
				HandleOrganize ();
				e.RetVal = Response.Ok;
				break;
			case Command.Shutdown:
				HandleShutdown ();
				e.RetVal = Response.Ok;
				break;
			case Command.Slideshow:
				HandleSlideshow (e.MessageData.Text);
				e.RetVal = Response.Ok;
				break;
			case Command.View:
				HandleView (e.MessageData.Uris);
				e.RetVal = Response.Ok;
				break;
			case Command.Invalid:
			default:
				Log.Debug ("Wrong command received");
				break;
			}
		}

		void HandleImport (string path)
		{
			// Some users get wonky URIs here, trying to work around below.
			// https://bugzilla.gnome.org/show_bug.cgi?id=629248
			if (path != null && path.StartsWith ("gphoto2:usb:")) {
				path = $"gphoto2://[{path.Substring (8)}]";
			}

			Log.Debug ($"Importing from {path}");
			Organizer.Window.Present ();
			Organizer.ImportFile (path == null ? null : new Uri (path));
		}

		void HandleOrganize ()
		{
			if (Database.Empty)
				HandleImport (null);
			else
				Organizer.Window.Present ();
		}

		void HandleShutdown ()
		{
			try {
				Instance.Organizer.Close ();
			} catch {
				Environment.Exit (0);
			}
		}

		//FIXME move all this in a standalone class
		void HandleSlideshow (string tagname)
		{
			Tag tag;
			Widgets.SlideShow slideshow = null;

			if (!string.IsNullOrEmpty (tagname))
				tag = Database.Tags.GetTagByName (tagname);
			else
				tag = Database.Tags.GetTagById (Preferences.Get<int> (Preferences.SCREENSAVER_TAG));

			IPhoto[] photos;
			if (tag != null)
				photos = ObsoletePhotoQueries.Query (new Tag[] { tag });
			else if (Preferences.Get<int> (Preferences.SCREENSAVER_TAG) == 0)
				photos = ObsoletePhotoQueries.Query (new Tag[] { });
			else
				photos = new IPhoto[0];

			// Minimum delay 1 second; default is 4s
			var delay = Math.Max (1.0, Preferences.Get<double> (Preferences.SCREENSAVER_DELAY));

			var window = new XScreenSaverSlide ();
			window.ModifyFg (Gtk.StateType.Normal, new Gdk.Color (127, 127, 127));
			window.ModifyBg (Gtk.StateType.Normal, new Gdk.Color (0, 0, 0));

			if (photos.Length > 0) {
				Array.Sort (photos, new IPhotoComparer.RandomSort ());
				slideshow = new Widgets.SlideShow (new BrowsablePointer (new PhotoList (photos), 0), (uint)(delay * 1000), true);
				window.Add (slideshow);
			} else {
				var outer = new Gtk.HBox ();
				var hbox = new Gtk.HBox ();
				var vbox = new Gtk.VBox ();

				outer.PackStart (new Gtk.Label (string.Empty));
				outer.PackStart (vbox, false, false, 0);
				vbox.PackStart (new Gtk.Label (string.Empty));
				vbox.PackStart (hbox, false, false, 0);
				hbox.PackStart (new Gtk.Image (Gtk.Stock.DialogWarning, Gtk.IconSize.Dialog),
						false, false, 0);
				outer.PackStart (new Gtk.Label (string.Empty));

				string msg;
				string long_msg;

				if (tag != null) {
					msg = Strings.NoPhotosMatchingXFound (tag.Name);
					long_msg = Strings.TheTagXIsNotAppliedToAnyPhotosLongMsg (tag.Name);
				} else {
					msg = Strings.SearchReturnedNoResults;
					long_msg = Strings.TheTagFSpotIsLookingForDoesNotExistTryLongMsg;
				}

				var label = new Gtk.Label (msg);
				hbox.PackStart (label, false, false, 0);

				var long_label = new Gtk.Label (long_msg) {
					Markup = $"<small>{long_msg}</small>"
				};

				vbox.PackStart (long_label, false, false, 0);
				vbox.PackStart (new Gtk.Label (string.Empty));

				window.Add (outer);
				label.ModifyFg (Gtk.StateType.Normal, new Gdk.Color (127, 127, 127));
				label.ModifyBg (Gtk.StateType.Normal, new Gdk.Color (0, 0, 0));
				long_label.ModifyFg (Gtk.StateType.Normal, new Gdk.Color (127, 127, 127));
				long_label.ModifyBg (Gtk.StateType.Normal, new Gdk.Color (0, 0, 0));
			}
			window.ShowAll ();

			Register (window);
			GLib.Idle.Add (delegate {
				if (slideshow != null)
					slideshow.Start ();
				return false;
			});
		}

		void HandleView (string[] uris)
		{
			var ul = new List<Uri> ();
			foreach (var u in uris)
				ul.Add (new Uri (u));
			try {
				Register (new SingleView (ul.ToArray ()).Window);
			} catch (Exception e) {
				Log.Exception (e);
				Log.Debug ("no real valid path to view from");
			}
		}

		void Register (Gtk.Window window)
		{
			toplevels.Add (window);
			window.Destroyed += HandleDestroyed;
		}

		void HandleDestroyed (object sender, EventArgs e)
		{
			toplevels.Remove (sender as Gtk.Window);
			if (toplevels.Count == 0) {
				Log.Information ("Exiting...");
				Banshee.Kernel.Scheduler.Dispose ();
				Database.Dispose ();
				ImageLoaderThread.CleanAll ();
				Gtk.Application.Quit ();
			}
			if (organizer != null && organizer.Window == sender)
				organizer = null;
		}
	}
}
