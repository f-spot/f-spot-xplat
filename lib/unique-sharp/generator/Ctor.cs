// GtkSharp.Generation.Ctor.cs - The Constructor Generation Class.
//
// Author: Mike Kestner <mkestner@novell.com>
//
// Copyright (c) 2001-2003 Mike Kestner
// Copyright (c) 2004-2005 Novell, Inc.
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

	public class Ctor : MethodBase  {
		readonly string name;
		readonly bool needs_chaining = false;

		public Ctor (XmlElement elem, ClassBase implementor) : base (elem, implementor) 
		{
			if (elem.HasAttribute ("preferred"))
				Preferred = true;
			if (implementor is ObjectGen)
				needs_chaining = true;
			name = implementor.Name;
		}

        public bool Preferred { get; set; }

        public string StaticName {
			get {
				if (!IsStatic)
					return string.Empty;

				string[] toks = CName.Substring(CName.IndexOf("new")).Split ('_');
				string result = string.Empty;

				foreach (string tok in toks)
					result += tok.Substring(0,1).ToUpper() + tok.Substring(1);
				return result;
			}
		}

		void GenerateImport (StreamWriter sw)
		{
			sw.WriteLine("\t\t[DllImport(\"" + LibraryName + "\")]");
			sw.WriteLine("\t\tstatic extern " + Safety + "IntPtr " + CName + "(" + Parameters.ImportSignature + ");");
			sw.WriteLine();
		}

		void GenerateStatic (GenerationInfo gen_info)
		{
			var sw = gen_info.Writer;
			sw.WriteLine("\t\t" + Protection + " static " + Safety + Modifiers +  name + " " + StaticName + "(" + Signature + ")");
			sw.WriteLine("\t\t{");

			Body.Initialize(gen_info, false, false, ""); 

			sw.Write("\t\t\t" + name + " result = ");
			if (container_type is StructBase)
				sw.Write ("{0}.New (", name);
			else
				sw.Write ("new {0} (", name);
			sw.WriteLine (CName + "(" + Body.GetCallString (false) + "));");
			Body.Finish (sw, ""); 
			Body.HandleException (sw, ""); 
			sw.WriteLine ("\t\t\treturn result;");
		}

		public void Generate (GenerationInfo gen_info)
		{
			if (!Validate ())
				return;

			var sw = gen_info.Writer;
			gen_info.CurrentMember = CName;

			GenerateImport (sw);
			
			if (IsStatic)
				GenerateStatic (gen_info);
			else {
				sw.WriteLine("\t\t{0} {1}{2} ({3}) {4}", Protection, Safety, name, Signature.ToString(), needs_chaining ? ": base (IntPtr.Zero)" : "");
				sw.WriteLine("\t\t{");

				if (needs_chaining) {
					sw.WriteLine ("\t\t\tif (GetType () != typeof (" + name + ")) {");
					
					if (Parameters.Count == 0) {
						sw.WriteLine ("\t\t\t\tCreateNativeObject (new string [0], new GLib.Value[0]);");
						sw.WriteLine ("\t\t\t\treturn;");
					} else {
						var names = new ArrayList ();
						var values = new ArrayList ();
						for (int i = 0; i < Parameters.Count; i++) {
							var p = Parameters[i];
							if (container_type.GetPropertyRecursively (p.StudlyName) != null) {
								names.Add (p.Name);
								values.Add (p.Name);
							} else if (p.PropertyName != string.Empty) {
								names.Add (p.PropertyName);
								values.Add (p.Name);
							}
						}

						if (names.Count == Parameters.Count) {
							sw.WriteLine ("\t\t\t\tArrayList vals = new ArrayList();");
							sw.WriteLine ("\t\t\t\tArrayList names = new ArrayList();");
							for (int i = 0; i < names.Count; i++) {
								var p = Parameters [i];
								string indent = "\t\t\t\t";
								if (p.Generatable is ClassBase && !(p.Generatable is StructBase)) {
									sw.WriteLine (indent + "if (" + p.Name + " != null) {");
									indent += "\t";
								}
								sw.WriteLine (indent + "names.Add (\"" + names [i] + "\");");
								sw.WriteLine (indent + "vals.Add (new GLib.Value (" + values[i] + "));");

								if (p.Generatable is ClassBase && !(p.Generatable is StructBase))
									sw.WriteLine ("\t\t\t\t}");
							}

							sw.WriteLine ("\t\t\t\tCreateNativeObject ((string[])names.ToArray (typeof (string)), (GLib.Value[])vals.ToArray (typeof (GLib.Value)));");
							sw.WriteLine ("\t\t\t\treturn;");
						} else
							sw.WriteLine ("\t\t\t\tthrow new InvalidOperationException (\"Can't override this constructor.\");");
					}
					
					sw.WriteLine ("\t\t\t}");
				}
	
				Body.Initialize(gen_info, false, false, ""); 
				sw.WriteLine("\t\t\t{0} = {1}({2});", container_type.AssignToName, CName, Body.GetCallString (false));
				Body.Finish (sw, "");
				Body.HandleException (sw, "");
			}
			
			sw.WriteLine("\t\t}");
			sw.WriteLine();
			
			Statistics.CtorCount++;
		}
	}
}

