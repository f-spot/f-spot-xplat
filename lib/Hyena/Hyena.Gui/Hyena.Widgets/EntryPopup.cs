//
// EntryPopup.cs
//
// Author:
//   Neil Loknath <neil.loknath@gmail.com>
//
// Copyright (C) 2009 Neil Loknath
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
using Gtk;

namespace Hyena.Widgets
{
    public class EntryPopup : Gtk.Window
    {
        Entry text_entry;
        HBox hbox;
        uint timeout_id = 0;

        public event EventHandler<EventArgs> Changed;
        public event EventHandler<KeyPressEventArgs> KeyPressed;

        public EntryPopup (string text) : this ()
        {
            Text = text;
        }

        public EntryPopup () : base (Gtk.WindowType.Popup)
        {
            CanFocus = true;
            Resizable = false;
            TypeHint = Gdk.WindowTypeHint.Utility;
            Modal = true;

			var frame = new Frame {
				Shadow = ShadowType.EtchedIn
			};
			Add (frame);

            hbox = new HBox () { Spacing = 6 };
            text_entry = new Entry();
            hbox.PackStart (text_entry, true, true, 0);
            hbox.BorderWidth = 3;

            frame.Add (hbox);
            frame.ShowAll ();

            text_entry.Text = string.Empty;
            text_entry.CanFocus = true;

            //TODO figure out why this event does not get raised
            text_entry.FocusOutEvent += (o, a) => {
                if (HideOnFocusOut) {
                    HidePopup ();
                }
            };

            text_entry.KeyReleaseEvent += delegate (object o, KeyReleaseEventArgs args) {
                if (args.Event.Key == Gdk.Key.Escape ||
                    args.Event.Key == Gdk.Key.Return ||
                    args.Event.Key == Gdk.Key.Tab) {

                    HidePopup ();
                }

                InitializeDelayedHide ();
            };

            text_entry.KeyPressEvent += (o, a) => OnKeyPressed (a);

            text_entry.Changed += (o, a) => {
                if (GdkWindow.IsVisible) {
                    OnChanged (a);
                }
            };
        }

        public new bool HasFocus {
            get { return text_entry.HasFocus; }
            set { text_entry.HasFocus = value; }
        }

        public string Text {
            get { return text_entry.Text; }
            set { text_entry.Text = value; }
        }

        public Entry Entry { get { return text_entry; } }
        public HBox Box { get { return hbox; } }

        public bool HideAfterTimeout { get; set; } = true;

		public uint Timeout { get; set; } = 5000;

		public bool HideOnFocusOut { get; set; } = true;

		public bool ResetOnHide { get; set; } = true;

		public override void Dispose ()
        {
            text_entry.Dispose ();
            base.Dispose ();
        }

        public new void GrabFocus ()
        {
            text_entry.GrabFocus ();
        }

        public void Position (Gdk.Window eventWindow)
        {
            int x, y;
            int widget_x, widget_y;
            int widget_height, widget_width;

            Realize ();

			var widget_window = eventWindow;
            Gdk.Screen widget_screen = widget_window.Screen;

            Gtk.Requisition popup_req;

            widget_window.GetOrigin (out widget_x, out widget_y);
            widget_window.GetSize (out widget_width, out widget_height);

            popup_req = Requisition;

            if (widget_x + widget_width > widget_screen.Width) {
                x = widget_screen.Width - popup_req.Width;
            } else if (widget_x + widget_width - popup_req.Width < 0) {
                x = 0;
            } else {
                x = widget_x + widget_width - popup_req.Width;
            }

            if (widget_y + widget_height + popup_req.Height > widget_screen.Height) {
                y = widget_screen.Height - popup_req.Height;
            } else if (widget_y + widget_height < 0) {
                y = 0;
            } else {
                y = widget_y + widget_height;
            }

            Move (x, y);
        }

        void ResetDelayedHide ()
        {
            if (timeout_id > 0) {
                GLib.Source.Remove (timeout_id);
                timeout_id = 0;
            }
        }

        void InitializeDelayedHide ()
        {
            ResetDelayedHide ();
            timeout_id = GLib.Timeout.Add (Timeout, delegate {
                            HidePopup ();
                            return false;
                        });
        }

        void HidePopup ()
        {
            ResetDelayedHide ();
            Hide ();

            if (ResetOnHide) {
                text_entry.Text = string.Empty;
            }
        }

        protected virtual void OnChanged (EventArgs args)
        {
			Changed?.Invoke (this, EventArgs.Empty);
		}

        protected virtual void OnKeyPressed (KeyPressEventArgs args)
        {
			KeyPressed?.Invoke (this, args);
		}

        //TODO figure out why this event does not get raised
        protected override bool OnFocusOutEvent (Gdk.EventFocus evnt)
        {
            if (HideOnFocusOut) {
                HidePopup ();
                return true;
            }

            return base.OnFocusOutEvent (evnt);
        }

        protected override bool OnExposeEvent (Gdk.EventExpose evnt)
        {
            InitializeDelayedHide ();
            return base.OnExposeEvent (evnt);
        }

        protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
        {
            if (!text_entry.HasFocus && HideOnFocusOut) {
                HidePopup ();
                return true;
            }

            return base.OnButtonReleaseEvent (evnt);
        }

        protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
        {
            if (!text_entry.HasFocus && HideOnFocusOut) {
                HidePopup ();
                return true;
            }

            return base.OnButtonPressEvent (evnt);
        }
    }
}
