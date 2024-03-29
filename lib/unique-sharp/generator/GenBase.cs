// GtkSharp.Generation.GenBase.cs - The Generatable base class.
//
// Author: Mike Kestner <mkestner@novell.com>
//
// Copyright (c) 2001-2002 Mike Kestner
// Copyright (c) 2004 Novell, Inc.
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
	using System.IO;
	using System.Xml;

	public abstract class GenBase : IGeneratable {
		readonly XmlElement ns;

		protected GenBase (XmlElement ns, XmlElement elem)
		{
			this.ns = ns;
			Elem = elem;
		}

		public string CName {
			get {
				return Elem.GetAttribute ("cname");
			}
		}

        public XmlElement Elem { get; }

        public bool IsInternal {
			get {
				if (Elem.HasAttribute ("internal")) {
					string attr = Elem.GetAttribute ("internal");
					return attr == "1" || attr == "true";
				}
				return false;
			}
		}

		public string LibraryName {
			get {
				return ns.GetAttribute ("library");
			}
		}

		public virtual string MarshalReturnType { 
			get {
				return MarshalType;
			}
		}

		public abstract string MarshalType { get; }

		public string Name {
			get {
				return Elem.GetAttribute ("name");
			}
		}

		public string NS {
			get {
				return ns.GetAttribute ("name");
			}
		}

		public abstract string DefaultValue { get; }

		public string QualifiedName {
			get {
				return NS + "." + Name;
			}
		}

		public virtual string ToNativeReturnType { 
			get {
				return MarshalType;
			}
		}

		protected void AppendCustom (StreamWriter sw, string custom_dir)
		{
			char sep = Path.DirectorySeparatorChar;
			string custom = custom_dir + sep + Name + ".custom";
			if (File.Exists(custom)) {
				sw.WriteLine ("#region Customized extensions");
				sw.WriteLine ("#line 1 \"" + Name + ".custom\"");
				var custstream = new FileStream(custom, FileMode.Open, FileAccess.Read);
				var sr = new StreamReader(custstream);
				sw.WriteLine (sr.ReadToEnd ());
				sw.WriteLine ("#endregion");
				sr.Close ();
			}
		}

		public abstract string CallByName (string var);

		public abstract string FromNative (string var);

		public virtual string FromNativeReturn (string var)
		{
			return FromNative (var);
		}

		public virtual string ToNativeReturn (string var)
		{
			return CallByName (var);
		}

		public abstract bool Validate ();

		public void Generate ()
		{
			var geninfo = new GenerationInfo (ns);
			Generate (geninfo);
		}

		public abstract void Generate (GenerationInfo geninfo);
	}
}

