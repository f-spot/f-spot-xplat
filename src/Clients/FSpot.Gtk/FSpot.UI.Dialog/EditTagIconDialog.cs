//
// EditTagIconDialog.cs
//
// Author:
//   Stephane Delcroix <stephane@delcroix.org>
//   Ruben Vermeersch <ruben@savanne.be>
//
// Copyright (C) 2009-2010 Novell, Inc.
// Copyright (C) 2009 Stephane Delcroix
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

using Gtk;

using FSpot.Core;
using FSpot.Database;
using FSpot.Imaging;
using FSpot.Query;
using FSpot.Settings;
using FSpot.Translations;
using FSpot.Utils;
using FSpot.Widgets;

using Hyena;
using Hyena.Widgets;

namespace FSpot.UI.Dialog
{
	public class EditTagIconDialog : BuilderDialog
	{
		readonly PhotoQuery query;
		readonly PhotoImageView image_view;
		Gtk.IconView icon_view;
		ListStore icon_store;
		string icon_name;
		Gtk.FileChooserButton external_photo_chooser;

#pragma warning disable 649
		[GtkBeans.Builder.Object] Gtk.Image preview_image;
		[GtkBeans.Builder.Object] Gtk.ScrolledWindow photo_scrolled_window;
		[GtkBeans.Builder.Object] Gtk.ScrolledWindow icon_scrolled_window;
		[GtkBeans.Builder.Object] Label photo_label;
		[GtkBeans.Builder.Object] Label from_photo_label;
		[GtkBeans.Builder.Object] SpinButton photo_spin_button;
		[GtkBeans.Builder.Object] HBox external_photo_chooser_hbox;
#pragma warning restore 649

		public EditTagIconDialog (Db db, Tag t, Gtk.Window parent_window) : base ("EditTagIconDialog.ui", "edit_tag_icon_dialog")
		{
			TransientFor = parent_window;
			Title = Strings.EditIconForTagName (t.Name);

			// FIXME, Icon
			//preview_pixbuf = t.Icon;
			if (preview_pixbuf != null && ColorManagement.Profiles.TryGetValue (Preferences.Get<string> (Preferences.COLOR_MANAGEMENT_DISPLAY_PROFILE), out var screen_profile)) {
				preview_image.Pixbuf = preview_pixbuf.Copy ();
				ColorManagement.ApplyProfile (preview_image.Pixbuf, screen_profile);
			} else
				preview_image.Pixbuf = preview_pixbuf;

			query = new PhotoQuery (db.Photos);

			if (db.Tags.Hidden != null)
				query.Terms = OrTerm.FromTags (new[] { t });
			else
				query.Terms = new Literal (t);

			image_view = new PhotoImageView (query) { CropHelpers = false };
			image_view.SelectionXyRatio = 1.0;
			image_view.SelectionChanged += HandleSelectionChanged;
			image_view.PhotoChanged += HandlePhotoChanged;

			external_photo_chooser = new Gtk.FileChooserButton (Strings.SelectPhotoFromFile, Gtk.FileChooserAction.Open) {
				Filter = new FileFilter ()
			};
			external_photo_chooser.Filter.AddPixbufFormats ();
			external_photo_chooser.LocalOnly = false;
			external_photo_chooser_hbox.PackStart (external_photo_chooser);
			external_photo_chooser.Show ();
			external_photo_chooser.SelectionChanged += HandleExternalFileSelectionChanged;

			photo_scrolled_window.Add (image_view);

			if (query.Count > 0) {
				photo_spin_button.Wrap = true;
				photo_spin_button.Adjustment.Lower = 1.0;
				photo_spin_button.Adjustment.Upper = query.Count;
				photo_spin_button.Adjustment.StepIncrement = 1.0;
				photo_spin_button.ValueChanged += HandleSpinButtonChanged;

				image_view.Item.Index = 0;
			} else {
				from_photo_label.Markup = string.Format ($"\n<b>{Strings.FromPhoto}</b>\n{Strings.TagFromPhotoLabelMessage}", t.Name);
				photo_scrolled_window.Visible = false;
				photo_label.Visible = false;
				photo_spin_button.Visible = false;
			}

			icon_store = new ListStore (typeof (string), typeof (Gdk.Pixbuf));

			icon_view = new Gtk.IconView (icon_store) {
				PixbufColumn = 1,
				SelectionMode = SelectionMode.Single
			};
			icon_view.SelectionChanged += HandleIconSelectionChanged;

			icon_scrolled_window.Add (icon_view);

			icon_view.Show ();

			image_view.Show ();

			//var fill_delay = new DelayedOperation (FillIconView);
			//fill_delay.Start ();
		}

		public BrowsablePointer Item {
			get { return image_view.Item; }
		}

		Gdk.Pixbuf preview_pixbuf;
		public Gdk.Pixbuf PreviewPixbuf {
			get { return preview_pixbuf; }
			set {
				icon_name = null;
				preview_pixbuf = value;
				if (value != null && ColorManagement.Profiles.TryGetValue (Preferences.Get<string> (Preferences.COLOR_MANAGEMENT_DISPLAY_PROFILE), out var screen_profile)) {
					preview_image.Pixbuf = value.Copy ();
					ColorManagement.ApplyProfile (preview_image.Pixbuf, screen_profile);
				} else
					preview_image.Pixbuf = value;

			}

		}

		public string ThemeIconName {
			get { return icon_name; }
			set {
				icon_name = value;
				// FIXME, Theme icon
				//PreviewPixbuf = GtkUtil.TryLoadIcon (Global.IconTheme, value, 48, (IconLookupFlags)0);
			}

		}

		void HandleSpinButtonChanged (object sender, EventArgs args)
		{
			int value = photo_spin_button.ValueAsInt - 1;

			image_view.Item.Index = value;
		}

		void HandleExternalFileSelectionChanged (object sender, EventArgs args)
		{   //Note: The filter on the FileChooserButton's dialog means that we will have a Pixbuf compatible uri here
			CreateTagIconFromExternalPhoto ();
		}

		void CreateTagIconFromExternalPhoto ()
		{
			try {
				using (var img = App.Instance.Container.Resolve<IImageFileFactory> ().Create (new Uri (external_photo_chooser.Uri, true))) {
					using (Gdk.Pixbuf external_image = img.Load ()) {
						PreviewPixbuf = PixbufUtils.TagIconFromPixbuf (external_image);
					}
				}
			} catch (Exception) {
				string caption = Strings.UnableToLoadImage;
				string message = string.Format (Strings.UnableToLoadPhotoAsIconForTheTag, external_photo_chooser.Uri);
				var md = new HigMessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close,
					caption, message);
				md.Run ();
				md.Destroy ();
			}
		}

		void HandleSelectionChanged (object sender, EventArgs e)
		{
			int x = image_view.Selection.X;
			int y = image_view.Selection.Y;
			int width = image_view.Selection.Width;
			int height = image_view.Selection.Height;

			if (image_view.Pixbuf != null) {
				if (image_view.Selection != Gdk.Rectangle.Zero) {
					using (var tmp = new Gdk.Pixbuf (image_view.Pixbuf, x, y, width, height)) {
						Gdk.Pixbuf transformed = Utils.PixbufUtils.TransformOrientation (tmp, image_view.PixbufOrientation);
						PreviewPixbuf = PixbufUtils.TagIconFromPixbuf (transformed);
						transformed.Dispose ();
					}
				} else {
					Gdk.Pixbuf transformed = Utils.PixbufUtils.TransformOrientation (image_view.Pixbuf, image_view.PixbufOrientation);
					PreviewPixbuf = PixbufUtils.TagIconFromPixbuf (transformed);
					transformed.Dispose ();
				}
			}
		}

		public void HandlePhotoChanged (object sender, EventArgs e)
		{
			int item = image_view.Item.Index;
			photo_label.Text = Strings.PhotoPhotoOutOfPhotos (item + 1, query.Count);

			photo_spin_button.Value = item + 1;
			HandleSelectionChanged (null, null);
		}

		public void HandleIconSelectionChanged (object o, EventArgs args)
		{
			if (icon_view.SelectedItems.Length == 0)
				return;

			icon_store.GetIter (out var iter, icon_view.SelectedItems[0]);
			ThemeIconName = (string)icon_store.GetValue (iter, 0);
		}

		public bool FillIconView ()
		{
			icon_store.Clear ();
			// FIXME, Theme icon
			//string[] icon_list = Global.IconTheme.ListIcons ("Emblems");
			//foreach (string item_name in icon_list)
			//	icon_store.AppendValues (item_name, GtkUtil.TryLoadIcon (Global.IconTheme, item_name, 32, (IconLookupFlags)0));
			return false;
		}
	}
}
