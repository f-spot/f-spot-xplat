// GtkSharp.Generation.MethodBase.cs - function element base class.
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
	using System.Xml;

	public abstract class MethodBase  {
		readonly XmlElement elem;
		protected ClassBase container_type;
		bool is_static = false;
		string protection = "public";

		protected MethodBase (XmlElement elem, ClassBase container_type) 
		{
			this.elem = elem;
			this.container_type = container_type;
			Name = elem.GetAttribute ("name");
			Parameters = new Parameters (elem ["parameters"]);
			IsStatic = elem.GetAttribute ("shared") == "true";
			if (elem.HasAttribute ("new_flag"))
				Modifiers = "new ";
			if (elem.HasAttribute ("accessibility")) {
				string attr = elem.GetAttribute ("accessibility");
				switch (attr) {
					case "public":
					case "protected":
					case "internal":
					case "private":
					case "protected internal":
						protection = attr;
						break;
				}
			}
		}

		protected string BaseName {
			get {
				string name = Name;
				int idx = Name.LastIndexOf (".");
				if (idx > 0)
					name = Name.Substring (idx + 1);
				return name;
			}
		}

		MethodBody body;
		public MethodBody Body {
			get {
				if (body == null)
					body = new MethodBody (Parameters);
				return body;
			}
		}

		public string CName {
			get {
				return SymbolTable.Table.MangleName (elem.GetAttribute ("cname"));
			}
		}

		protected bool HasGetterName {
			get {
				string name = BaseName;
				if (name.Length <= 3)
					return false;
				if (name.StartsWith ("Get") || name.StartsWith ("Has"))
					return char.IsUpper (name [3]);
				else if (name.StartsWith ("Is"))
					return char.IsUpper (name [2]);
				else
					return false;
			}
		}

		protected bool HasSetterName {
			get {
				string name = BaseName;
				if (name.Length <= 3)
					return false;

				return name.StartsWith ("Set") && char.IsUpper (name [3]);
			}
		}

		public bool IsStatic {
			get {
				return is_static;
			}
			set {
				is_static = value;
				Parameters.Static = value;
			}
		}

		public string LibraryName {
			get {
				if (elem.HasAttribute ("library"))
					return elem.GetAttribute ("library");
				return container_type.LibraryName;
			}
		}

        public string Modifiers { get; set; } = string.Empty;

        public string Name { get; set; }

        public Parameters Parameters { get; }


        public string Protection {
			get { return protection; }
			set { protection = value; }
		}

		protected string Safety {
			get {
				return Body.ThrowsException && !(container_type is InterfaceGen) ? "unsafe " : "";
			}
		}

		Signature sig;
		public Signature Signature {
			get {
				if (sig == null)
					sig = new Signature (Parameters);
				return sig;
			}
		}

		public virtual bool Validate ()
		{
			if (!Parameters.Validate ()) {
				Console.Write("in " + CName + " ");
				Statistics.ThrottledCount++;
				return false;
			}

			return true;
		}
	}
}

