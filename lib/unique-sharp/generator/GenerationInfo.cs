// GtkSharp.Generation.GenerationInfo.cs - Generation information class.
//
// Author: Mike Kestner <mkestner@ximian.com>
//
// Copyright (c) 2003-2008 Novell Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the GNU General Public
// License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
// Boston, MA 02110-1301


namespace GtkSharp.Generation {

	using System;
	using System.Collections;
	using System.IO;
	using System.Xml;

	public class GenerationInfo {
		readonly string gluelib_name;
		StreamWriter glue_sw;

		public GenerationInfo (XmlElement ns)
		{
			string ns_name = ns.GetAttribute ("name");
			char sep = Path.DirectorySeparatorChar;
			Dir = ".." + sep + ns_name.ToLower () + sep + "generated";
			CustomDir = ".." + sep + ns_name.ToLower ();
			AssemblyName = ns_name.ToLower () + "-sharp";
		}

		public GenerationInfo (string dir, string assembly_name) : this (dir, dir, assembly_name, "", "", "") {}

		public GenerationInfo (string dir, string custom_dir, string assembly_name, string glue_filename, string glue_includes, string gluelib_name)
		{
			Dir = dir;
			CustomDir = custom_dir;
			AssemblyName = assembly_name;
			this.gluelib_name = gluelib_name;
			InitializeGlue (glue_filename, glue_includes, gluelib_name);
		}

		void InitializeGlue (string glue_filename, string glue_includes, string gluelib_name)
		{
			if (gluelib_name != string.Empty && glue_filename != string.Empty) {
				FileStream stream;
				try {
					stream = new FileStream (glue_filename, FileMode.Create, FileAccess.Write);
				} catch (Exception) {
					Console.Error.WriteLine ("Unable to create specified glue file.  Glue will not be generated.");
					return;
				}

				glue_sw = new StreamWriter (stream);
			
				glue_sw.WriteLine ("// This file was generated by the Gtk# code generator.");
				glue_sw.WriteLine ("// Any changes made will be lost if regenerated.");
				glue_sw.WriteLine ();

				if (glue_includes != "") {
					foreach (string header in glue_includes.Split (new char[] {',', ' '})) {
						if (header != "")
							glue_sw.WriteLine ("#include <{0}>", header);
					}
					glue_sw.WriteLine ("");
				}
				glue_sw.WriteLine ("const gchar *__prefix = \"__gtksharp_\";\n");
				glue_sw.WriteLine ("#define HAS_PREFIX(a) (*((guint64 *)(a)) == *((guint64 *) __prefix))\n");
				glue_sw.WriteLine ("static GObjectClass *");
				glue_sw.WriteLine ("get_threshold_class (GObject *obj)");
				glue_sw.WriteLine ("{");
				glue_sw.WriteLine ("\tGType gtype = G_TYPE_FROM_INSTANCE (obj);");
				glue_sw.WriteLine ("\twhile (HAS_PREFIX (g_type_name (gtype)))");
				glue_sw.WriteLine ("\t\tgtype = g_type_parent (gtype);");
				glue_sw.WriteLine ("\tGObjectClass *klass = g_type_class_peek (gtype);");
				glue_sw.WriteLine ("\tif (klass == NULL) klass = g_type_class_ref (gtype);");
				glue_sw.WriteLine ("\treturn klass;");
				glue_sw.WriteLine ("}\n");
				GlueEnabled = true;
			}
		}

        public string AssemblyName { get; }

        public string CustomDir { get; }

        public string Dir { get; }

        public string GluelibName {
			get {
				return gluelib_name;
			}
		}

        public bool GlueEnabled { get; private set; }

        public StreamWriter GlueWriter {
			get {
				return glue_sw;
			}
		}

        public StreamWriter Writer { get; set; }

        public void CloseGlueWriter ()
		{
			if (glue_sw != null)
				glue_sw.Close ();
		}

		string member;
		public string CurrentMember {
			get {
				return CurrentType + "." + member;
			}
			set {
				member = value;
			}
		}

        public string CurrentType { get; set; }

        public StreamWriter OpenStream (string name) 
		{
			char sep = Path.DirectorySeparatorChar;
			if (!Directory.Exists(Dir))
				Directory.CreateDirectory(Dir);
			string filename = Dir + sep + name + ".cs";
			
			var stream = new FileStream (filename, FileMode.Create, FileAccess.Write);
			var sw = new StreamWriter (stream);
			
			sw.WriteLine ("// This file was generated by the Gtk# code generator.");
			sw.WriteLine ("// Any changes made will be lost if regenerated.");
			sw.WriteLine ();

			return sw;
		}
	}
}

