//
// ExceptionDialog.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2005-2007 Novell, Inc.
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
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Gtk;
using FSpot.Translations;

namespace Hyena.Gui.Dialogs
{
    public class ExceptionDialog : Dialog
    {
        AccelGroup accel_group;
		readonly string debugInfo;

        public ExceptionDialog(Exception e) : base()
        {
            debugInfo = BuildExceptionMessage(e);

            HasSeparator = false;
            BorderWidth = 5;
            Resizable = false;
            //Translators: {0} is substituted with the application name
            Title = string.Format(Catalog.GetString("{0} Encountered a Fatal Error"),
                                  ApplicationContext.ApplicationName);

            VBox.Spacing = 12;
            ActionArea.Layout = ButtonBoxStyle.End;

            accel_group = new AccelGroup();
            AddAccelGroup(accel_group);

			var hbox = new HBox (false, 12) {
				BorderWidth = 5
			};
			VBox.PackStart(hbox, false, false, 0);

			var image = new Image (Stock.DialogError, IconSize.Dialog) {
				Yalign = 0.0f
			};
			hbox.PackStart(image, true, true, 0);

			var label_vbox = new VBox (false, 0) {
				Spacing = 12
			};
			hbox.PackStart(label_vbox, false, false, 0);

			var label = new Label (string.Format ("<b><big>{0}</big></b>", GLib.Markup.EscapeText (Title))) {
				UseMarkup = true,
				Justify = Justification.Left,
				LineWrap = true
			};
			label.SetAlignment(0.0f, 0.5f);
            label_vbox.PackStart(label, false, false, 0);

			label = new Label (e.Message) {
				UseMarkup = true,
				UseUnderline = false,
				Justify = Gtk.Justification.Left,
				LineWrap = true,
				Selectable = true
			};
			label.SetAlignment(0.0f, 0.5f);
            label_vbox.PackStart(label, false, false, 0);

			var details_label = new Label (string.Format ("<b>{0}</b>",
				GLib.Markup.EscapeText (Catalog.GetString ("Error Details")))) {
				UseMarkup = true
			};
			var details_expander = new Expander ("Details") {
				LabelWidget = details_label
			};
			label_vbox.PackStart(details_expander, true, true, 0);

            var scroll = new ScrolledWindow();
            var view = new TextView();

            scroll.HscrollbarPolicy = PolicyType.Automatic;
            scroll.VscrollbarPolicy = PolicyType.Automatic;
            scroll.AddWithViewport(view);

            scroll.SetSizeRequest(450, 250);

            view.Editable = false;
            view.Buffer.Text = debugInfo;

            details_expander.Add(scroll);

            hbox.ShowAll();

            AddButton(Stock.Close, ResponseType.Close, true);
        }

        void AddButton(string stock_id, Gtk.ResponseType response, bool is_default)
        {
			var button = new Button (stock_id) {
				CanDefault = true
			};
			button.Show ();

            AddActionWidget(button, response);

            if(is_default) {
                DefaultResponse = response;
                button.AddAccelerator("activate", accel_group, (uint)Gdk.Key.Return,
                    0, AccelFlags.Visible);
            }
        }

        string BuildExceptionMessage(Exception e)
        {
            var msg = new System.Text.StringBuilder();

            msg.Append(Catalog.GetString("An unhandled exception was thrown: "));

            var exception_chain = new Stack<Exception> ();

            while (e != null) {
                exception_chain.Push (e);
                e = e.InnerException;
            }

            while (exception_chain.Count > 0) {
                e = exception_chain.Pop ();
                msg.AppendFormat ("{0}\n\n{1}\n", e.Message, e.StackTrace);
            };

            msg.Append("\n");
            msg.AppendFormat(".NET Version: {0}\n", Environment.Version);
            msg.AppendFormat("OS Version: {0}\n", Environment.OSVersion);
            msg.Append("\nAssembly Version Information:\n\n");

            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies()) {
				var name = asm.GetName();
                msg.AppendFormat("{0} ({1})\n", name.Name, name.Version);
            }

            if(Environment.OSVersion.Platform != PlatformID.Unix) {
                return msg.ToString();
            }

            try {
                msg.AppendFormat("\nPlatform Information: {0}", BuildPlatformString());

                msg.Append("\n\nDisribution Information:\n\n");

				var lsb = LsbVersionInfo.Harvest;

                foreach(string lsbfile in lsb.Keys) {
                    msg.AppendFormat("[{0}]\n", lsbfile);
                    msg.AppendFormat("{0}\n", lsb[lsbfile]);
                }
            } catch {
            }

            return msg.ToString();
        }

        string BuildPlatformString()
        {
			var startInfo = new ProcessStartInfo {
				Arguments = "-sirom",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false
			};

			foreach (string unameprog in new string [] {
                "/usr/bin/uname", "/bin/uname", "/usr/local/bin/uname",
                "/sbin/uname", "/usr/sbin/uname", "/usr/local/sbin/uname"}) {
                try {
                    startInfo.FileName = unameprog;
                    var uname = Process.Start(startInfo);
                    return uname.StandardOutput.ReadLine().Trim();
                } catch(Exception) {
                    continue;
                }
            }

            return null;
        }

        class LsbVersionInfo
        {
			readonly string [] filesToCheck = {
                "*-release",
                "slackware-version",
                "debian_version"
            };

			public LsbVersionInfo ()
            {
                foreach(string pattern in filesToCheck) {
                    foreach(string filename in Directory.GetFiles("/etc/", pattern)) {
                        using(var fs = File.OpenRead(filename)) {
                            Findings[filename] = (new StreamReader(fs)).ReadToEnd();
                        }
                    }
                }
            }

            public Dictionary<string, string> Findings { get; } = new Dictionary<string, string>();

			public static Dictionary<string, string> Harvest {
                get { return (new LsbVersionInfo()).Findings; }
            }
        }
    }
}
