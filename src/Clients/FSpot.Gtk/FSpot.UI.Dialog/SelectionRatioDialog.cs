//
// SelectionRatioDialog.cs
//
// Author:
//   Stephane Delcroix <sdelcroix@src.gnome.org>
//
// Copyright (C) 2008 Novell, Inc.
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
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

using FSpot.Settings;
using FSpot.Translations;

using Gtk;

using Hyena;

namespace FSpot.UI.Dialog
{
	public class SelectionRatioDialog : BuilderDialog
	{
		[Serializable]
		public struct SelectionConstraint
		{
            public string Label { get; set; }

            double ratio;
			public double XyRatio {
				get { return ratio; }
				set { ratio = value; }
			}

			public SelectionConstraint (string label, double ratio)
			{
				Label = label;
				this.ratio = ratio;
			}
		}

#pragma warning disable 649
		[GtkBeans.Builder.Object] Button close_button;
		[GtkBeans.Builder.Object] Button add_button;
		[GtkBeans.Builder.Object] Button delete_button;
		[GtkBeans.Builder.Object] Button up_button;
		[GtkBeans.Builder.Object] Button down_button;
		[GtkBeans.Builder.Object] TreeView content_treeview;
#pragma warning restore 649

		ListStore constraints_store;

		public SelectionRatioDialog () : base ("SelectionRatioDialog.ui", "customratio_dialog")
		{
			close_button.Clicked += (o, e) => {
				SavePrefs ();
				Destroy ();
			};

			add_button.Clicked += (o, e) => {constraints_store.AppendValues (Strings.NewSelection, 1.0); };
			delete_button.Clicked += DeleteSelectedRows;
			up_button.Clicked += MoveUp;
			down_button.Clicked += MoveDown;
			var text_renderer = new CellRendererText {
				Editable = true
			};
			text_renderer.Edited += HandleLabelEdited;
			content_treeview.AppendColumn (Strings.Label, text_renderer, "text", 0);
			text_renderer = new CellRendererText {
				Editable = true
			};
			text_renderer.Edited += HandleRatioEdited;
			content_treeview.AppendColumn (Strings.Ratio, text_renderer, "text", 1);

			LoadPreference (Preferences.CUSTOM_CROP_RATIOS);
			Preferences.SettingChanged += OnPreferencesChanged;
		}

		void Populate ()
		{
			constraints_store = new ListStore (typeof (string), typeof (double));
			content_treeview.Model = constraints_store;
			var serializer = new XmlSerializer (typeof(SelectionConstraint));
			string [] vals = Preferences.Get<string []> (Preferences.CUSTOM_CROP_RATIOS);
			if (vals != null)
				foreach (string xml in vals) {
					var constraint = (SelectionConstraint)serializer.Deserialize (new StringReader (xml));
					constraints_store.AppendValues (constraint.Label, constraint.XyRatio);
				}
		}

		void OnPreferencesChanged (object sender, NotifyEventArgs args)
		{
			LoadPreference (args.Key);
		}

		void LoadPreference (string key)
		{
			switch (key) {
			case Preferences.CUSTOM_CROP_RATIOS:
				Populate ();
				break;
			}
		}

		void SavePrefs ()
		{
			var prefs = new List<string> ();
			var serializer = new XmlSerializer (typeof (SelectionConstraint));
			foreach (object[] row in constraints_store) {
				var sw = new StringWriter ();
				serializer.Serialize (sw, new SelectionConstraint ((string)row[0], (double)row[1]));
				sw.Close ();
				prefs.Add (sw.ToString ());
			}

#if !GCONF_SHARP_2_18
			if (prefs.Count != 0)
#endif
				Preferences.Set (Preferences.CUSTOM_CROP_RATIOS, prefs.ToArray());
#if !GCONF_SHARP_2_18
			else
				Preferences.Set (Preferences.CUSTOM_CROP_RATIOS, -1);
#endif
		}

		public void HandleLabelEdited (object sender, EditedArgs args)
		{
			args.RetVal = false;
			if (!constraints_store.GetIterFromString (out var iter, args.Path))
				return;

			using (var val = new GLib.Value (args.NewText))
				constraints_store.SetValue (iter, 0, val);

			args.RetVal = true;
		}

		public void HandleRatioEdited (object sender, EditedArgs args)
		{
			args.RetVal = false;
			if (!constraints_store.GetIterFromString (out var iter, args.Path))
				return;

			double ratio;
			try {
				ratio = ParseRatio (args.NewText);
			} catch (FormatException fe) {
				Log.Exception (fe);
				return;
			}
			if (ratio < 1.0)
				ratio = 1.0 / ratio;

			using (var val = new GLib.Value (ratio))
				constraints_store.SetValue (iter, 1, val);

			args.RetVal = true;
		}

		double ParseRatio (string text)
		{
			try {
				return Convert.ToDouble (text);
			} catch (FormatException) {
				char [] separators = {'/', ':'};
				foreach (char c in separators) {
					if (text.IndexOf (c) != -1) {
						double ratio = Convert.ToDouble (text.Substring (0, text.IndexOf (c)));
						ratio /= Convert.ToDouble (text.Substring (text.IndexOf (c) + 1));
						return ratio;
					}
				}
				throw new FormatException ($"unable to parse {text}");
			}
		}

		void DeleteSelectedRows (object o, EventArgs e)
		{
			if (content_treeview.Selection.GetSelected (out var model, out var iter))
				(model as ListStore).Remove (ref iter);
		}

		void MoveUp (object o, EventArgs e)
		{
			if (content_treeview.Selection.GetSelected (out var model, out var selected)) {
				//no IterPrev :(
				TreePath path = model.GetPath (selected);
				if (path.Prev ())
					if (model.GetIter (out var prev, path))
						(model as ListStore).Swap (prev, selected);
			}
		}

		void MoveDown (object o, EventArgs e)
		{
			if (content_treeview.Selection.GetSelected (out var model, out var current)) {
				TreeIter next = current;
				if ((model as ListStore).IterNext (ref next))
					(model as ListStore).Swap (current, next);
			}
		}
	}
}
