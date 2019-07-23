// GtkSharp.Generation.EnumGen.cs - The Enumeration Generatable.
//
// Author: Mike Kestner <mkestner@speakeasy.net>
//
// Copyright (c) 2001 Mike Kestner
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

	public class EnumGen : GenBase {
		readonly string enum_type = string.Empty;
		readonly ArrayList members = new ArrayList ();

		public EnumGen (XmlElement ns, XmlElement elem) : base (ns, elem) 
		{
			foreach (XmlElement member in elem.ChildNodes) {
				if (member.Name != "member")
					continue;

				string result = "\t\t" + member.GetAttribute("name");
				if (member.HasAttribute("value")) {
					string value = member.GetAttribute("value");
					if (value.EndsWith("U")) {
						enum_type = " : uint";
						value = value.TrimEnd('U');
					}
					result += " = " + value;
				}
				members.Add (result + ",");
			}
		}

		public override bool Validate ()
		{
			return true;
		}

		public override string DefaultValue {
			get {
				return "(" + QualifiedName + ") 0";
			}
		}

		public override string MarshalType {
			get {
				return "int";
			}
		}

		public override string CallByName (string var_name)
		{
			return "(int) " + var_name;
		}
		
		public override string FromNative(string var)
		{
			return "(" + QualifiedName + ") " + var;
		}
		
		public override void Generate (GenerationInfo gen_info)
		{
			var sw = gen_info.OpenStream (Name);

			sw.WriteLine ("namespace " + NS + " {");
			sw.WriteLine ();
			sw.WriteLine ("\tusing System;");
			sw.WriteLine ("\tusing System.Runtime.InteropServices;");
			sw.WriteLine ();

			sw.WriteLine ("#region Autogenerated code");
					
			if (Elem.GetAttribute("type") == "flags")
				sw.WriteLine ("\t[Flags]");
			if (Elem.HasAttribute("gtype"))
				sw.WriteLine ("\t[GLib.GType (typeof (" + NS + "." + Name + "GType))]");

			string access = IsInternal ? "internal" : "public";
			sw.WriteLine ("\t" + access + " enum " + Name + enum_type + " {");
			sw.WriteLine ();
				
			foreach (string member in members)
				sw.WriteLine (member);

			sw.WriteLine ("\t}");

			if (Elem.HasAttribute ("gtype")) {
				sw.WriteLine ();
				sw.WriteLine ("\tinternal class " + Name + "GType {");
				sw.WriteLine ("\t\t[DllImport (\"" + LibraryName + "\")]");
				sw.WriteLine ("\t\tstatic extern IntPtr " + Elem.GetAttribute ("gtype") + " ();");
				sw.WriteLine ();
				sw.WriteLine ("\t\tpublic static GLib.GType GType {");
				sw.WriteLine ("\t\t\tget {");
				sw.WriteLine ("\t\t\t\treturn new GLib.GType (" + Elem.GetAttribute ("gtype") + " ());");
				sw.WriteLine ("\t\t\t}");
				sw.WriteLine ("\t\t}");
				sw.WriteLine ("\t}");
			}

			sw.WriteLine ("#endregion");
			sw.WriteLine ("}");
			sw.Close ();
			Statistics.EnumCount++;
		}
	}
}

