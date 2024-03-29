//
// QueryDebugger.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2008 Novell, Inc.
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
using System.IO;
using System.Xml;
using System.Reflection;
using Gtk;

using Hyena.Gui;

namespace Hyena.Query.Gui
{
    [TestModule ("Query Debugger")]
    public class QueryDebugger : Window
    {
        TextView input;
        TextView sql;
        TextView xml;

        QueryFieldSet query_field_set;

        public QueryDebugger () : base ("Hyena.Query Debugger")
        {
            SetDefaultSize (800, 600);

			var input_box = new VBox {
				Spacing = 8
			};
			var sw = new ScrolledWindow {
				ShadowType = ShadowType.In,
				HscrollbarPolicy = PolicyType.Never
			};
			input = new TextView {
				AcceptsTab = false
			};
			input.KeyReleaseEvent += delegate (object o, KeyReleaseEventArgs args) {
                if (args.Event.Key == Gdk.Key.Return || args.Event.Key == Gdk.Key.KP_Enter) {
                    input.Buffer.Text = input.Buffer.Text.Trim ();
                    OnParseUserQuery (null, EventArgs.Empty);
                }
            };
            input.WrapMode = WrapMode.Word;
            sw.Add (input);
            input_box.PackStart (sw, true, true, 0);
            var button_box = new HBox ();
            var parse = new Button ("Parse as User Query");
            parse.Clicked += OnParseUserQuery;
            button_box.PackStart (parse, false, false, 0);
            input_box.PackStart (button_box, false, false, 0);

			var output_box = new HBox {
				Spacing = 8
			};
			sw = new ScrolledWindow {
				ShadowType = ShadowType.In,
				HscrollbarPolicy = PolicyType.Never
			};
			sql = new TextView {
				WrapMode = WrapMode.Word
			};
			sw.Add (sql);
            output_box.PackStart (sw, true, true, 0);
			sw = new ScrolledWindow {
				ShadowType = ShadowType.In,
				HscrollbarPolicy = PolicyType.Never
			};
			xml = new TextView {
				WrapMode = WrapMode.Word
			};
			sw.Add (xml);
            output_box.PackStart (sw, true, true, 0);

            var pane = new VPaned ();
            pane.Add1 (input_box);
            pane.Add2 (output_box);
            pane.Position = 100;

            Add (pane);
            pane.ShowAll ();

            input.HasFocus = true;

            LoadQueryFieldSet ();
        }

        void LoadQueryFieldSet ()
        {
            var asm = Assembly.LoadFile ("Banshee.Services.dll");
			var t = asm.GetType ("Banshee.Query.BansheeQuery");
			var f = t.GetField ("FieldSet", BindingFlags.Public | BindingFlags.Static);
            query_field_set = (QueryFieldSet)f.GetValue (null);
        }

        StreamReader StringToStream (string s)
        {
            return new StreamReader (new MemoryStream (System.Text.Encoding.UTF8.GetBytes (s)));
        }

        void OnParseUserQuery (object o, EventArgs args)
        {
			var parser = new UserQueryParser {
				InputReader = StringToStream (input.Buffer.Text)
			};
			var node = parser.BuildTree (query_field_set);

            sql.Buffer.Text = node.ToSql (query_field_set) ?? string.Empty;

            var doc = new XmlDocument ();
            doc.LoadXml (node.ToXml (query_field_set));

            var s = new MemoryStream ();
			var w = new XmlTextWriter (s, System.Text.Encoding.UTF8) {
				Formatting = Formatting.Indented
			};
			doc.WriteContentTo (w);
            w.Flush ();
            s.Flush ();
            s.Position = 0;
            xml.Buffer.Text = new StreamReader (s).ReadToEnd ();
        }
    }
}
