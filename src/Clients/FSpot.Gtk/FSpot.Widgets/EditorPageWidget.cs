//
//  EditorPageWidget.cs
//
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
//   Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (C) 2008-2010 Novell, Inc.
// Copyright (C) 2008, 2010 Ruben Vermeersch
// Copyright (C) 2008-2010 Stephane Delcroix
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
using System.Linq;

using FSpot.Editors;
using FSpot.Translations;
using FSpot.UI.Dialog;
using FSpot.Utils;

using Gtk;

using Mono.Addins;

using Hyena;
using Hyena.Widgets;

namespace FSpot.Widgets
{
	public class EditorPageWidget : Gtk.ScrolledWindow
	{
		VBox widgets;
		VButtonBox buttons;
		Widget active_editor;
		readonly List<Editor> editors;
		Editor current_editor;

		// Used to make buttons insensitive when selecting multiple images.
		readonly Dictionary<Editor, Button> editor_buttons;

		EditorPage page;
		internal EditorPage Page {
			get { return page; }
			set { page = value; ChangeButtonVisibility (); }
		}

		public EditorPageWidget ()
		{
			editors = new List<Editor> ();
			editor_buttons = new Dictionary<Editor, Button> ();
			ShowTools ();
			AddinManager.AddExtensionNodeHandler ("/FSpot/Editors", OnExtensionChanged);
		}

		void OnExtensionChanged (object s, ExtensionNodeEventArgs args)
		{
			// FIXME: We do not do run-time removal of editors yet!
			if (args.Change == ExtensionChange.Add) {
				var editor = (args.ExtensionNode as EditorNode).GetEditor ();
				editor.ProcessingStarted += OnProcessingStarted;
				editor.ProcessingStep += OnProcessingStep;
				editor.ProcessingFinished += OnProcessingFinished;
				editors.Add (editor);
				PackButton (editor);
			}
		}

		ProgressDialog progress;

		void OnProcessingStarted (string name, int count)
		{
			progress = new ProgressDialog (name, ProgressDialog.CancelButtonType.None, count, App.Instance.Organizer.Window);
		}

		void OnProcessingStep (int done)
		{
			if (progress != null)
				progress.Update (string.Empty);
		}

		void OnProcessingFinished ()
		{
			if (progress != null) {
				progress.Destroy ();
				progress = null;
			}
		}

		internal void ChangeButtonVisibility ()
		{
			foreach (var editor in editors) {
				if (editor_buttons.TryGetValue (editor, out var button))
					button.Visible = Page.InPhotoView || editor.CanHandleMultiple;
			}
		}

		void PackButton (Editor editor)
		{
			var button = new Button (editor.Label);
			// FIXME, theme icon
			//if (editor.IconName != null)
			//	button.Image = new Image (GtkUtil.TryLoadIcon (FSpot.Settings.Global.IconTheme, editor.IconName, 22, (Gtk.IconLookupFlags)0));
			button.Clicked += (o, e) => { ChooseEditor (editor); };
			button.Show ();
			buttons.Add (button);
			editor_buttons.Add (editor, button);
		}

		public void ShowTools ()
		{
			// Remove any open editor, if present.
			if (current_editor != null) {
				active_editor.Hide ();
				widgets.Remove (active_editor);
				active_editor = null;
				current_editor.Restore ();
				current_editor = null;
			}

			// No need to build the widget twice.
			if (buttons != null) {
				buttons.Show ();
				return;
			}

			if (widgets == null) {
				widgets = new VBox (false, 0) {
					NoShowAll = true
				};
				widgets.Show ();
				var widgets_port = new Viewport ();
				widgets_port.Add (widgets);
				Add (widgets_port);
				widgets_port.ShowAll ();
			}

			// Build the widget (first time we call this method).
			buttons = new VButtonBox {
				BorderWidth = 5,
				Spacing = 5,
				LayoutStyle = ButtonBoxStyle.Start
			};

			foreach (var editor in editors)
				PackButton (editor);

			buttons.Show ();
			widgets.Add (buttons);
		}

		void ChooseEditor (Editor editor)
		{
			SetupEditor (editor);

			if (!editor.CanBeApplied || editor.HasSettings)
				ShowEditor (editor);
			else
				Apply (editor); // Instant apply
		}

		bool SetupEditor (Editor editor)
		{
			var state = editor.CreateState ();

			var photo_view = App.Instance.Organizer.PhotoView.View;

			if (Page.InPhotoView && photo_view != null) {
				state.Selection = photo_view.Selection;
				state.PhotoImageView = photo_view;
			} else {
				state.Selection = Gdk.Rectangle.Zero;
				state.PhotoImageView = null;
			}
			if ((Page.Sidebar as Sidebar).Selection == null)
				return false;
			state.Items = (Page.Sidebar as Sidebar).Selection.Items.ToArray ();

			editor.Initialize (state);
			return true;
		}

		void Apply (Editor editor)
		{
			if (!SetupEditor (editor))
				return;

			if (!editor.CanBeApplied) {
				string msg = Strings.NoSelectionAvailable;
				string desc = Strings.ThisToolRequiresAnActiveSelection;

				var md = new HigMessageDialog (App.Instance.Organizer.Window, DialogFlags.DestroyWithParent, Gtk.MessageType.Error, ButtonsType.Ok,
					msg, desc);

				md.Run ();
				md.Destroy ();
				return;
			}

			// TODO: Might need to do some nicer things for multiple selections (progress?)
			try {
				editor.Apply ();
			} catch (Exception e) {
				Log.DebugException (e);
				string msg = Catalog.GetPluralString (Strings.ErrorSavingAdjustedPhoto, Strings.ErrorSavingAdjustedPhotos, editor.State.Items.Length);
				string desc = Strings.ReceivedExceptionExNoteThatYouHaveToDevelopRawFilesInto (e.Message);

				var md = new HigMessageDialog (App.Instance.Organizer.Window, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Ok,
					msg, desc);
				md.Run ();
				md.Destroy ();
			}
			ShowTools ();
		}

		void ShowEditor (Editor editor)
		{
			SetupEditor (editor);
			current_editor = editor;

			buttons.Hide ();

			// Top label
			var vbox = new VBox (false, 4);
			var label = new Label {
				Markup = $"<big><b>{editor.Label}</b></big>"
			};
			vbox.PackStart (label, false, false, 5);

			// Optional config widget
			var config = editor.ConfigurationWidget ();
			if (config != null) {
				// This is necessary because GtkBuilder widgets need to be
				// reparented.
				if (config.Parent != null) {
					config.Reparent (vbox);
				} else {
					vbox.PackStart (config, false, false, 0);
				}
			}

			// Apply / Cancel buttons
			var tool_buttons = new HButtonBox {
				LayoutStyle = ButtonBoxStyle.End,
				Spacing = 5,
				BorderWidth = 5,
				Homogeneous = false
			};

			var cancel = new Button (Stock.Cancel);
			cancel.Clicked += HandleCancel;
			tool_buttons.Add (cancel);

			var apply = new Button (editor.ApplyLabel) {
				// FIXME, Theme icon
				//Image = new Image (GtkUtil.TryLoadIcon (Settings.Global.IconTheme, editor.IconName, 22, 0))
			};
			apply.Clicked += (s, e) => { Apply (editor); };
			tool_buttons.Add (apply);

			// Pack it all together
			vbox.PackEnd (tool_buttons, false, false, 0);
			active_editor = vbox;
			widgets.Add (active_editor);
			active_editor.ShowAll ();
		}

		void HandleCancel (object sender, EventArgs args)
		{
			ShowTools ();
		}
	}
}
