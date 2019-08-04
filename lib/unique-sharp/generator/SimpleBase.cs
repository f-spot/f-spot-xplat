// GtkSharp.Generation.SimpleBase.cs - base class for marshaling non-generated types.
//
// Author: Mike Kestner <mkestner@novell.com>
//
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

	public abstract class SimpleBase : IGeneratable  {
		readonly string ns = string.Empty;
		readonly string default_value = string.Empty;

		public SimpleBase (string ctype, string type, string default_value)
		{
			string[] toks = type.Split('.');
			CName = ctype;
			Name = toks[toks.Length - 1];
			if (toks.Length > 2)
				ns = string.Join (".", toks, 0, toks.Length - 1);
			else if (toks.Length == 2)
				ns = toks[0];
			this.default_value = default_value;
		}

        public string CName { get; }

        public string Name { get; }

        public string QualifiedName {
			get {
				return ns == string.Empty ? Name : ns + "." + Name;
			}
		}

		public virtual string MarshalType {
			get {
				return QualifiedName;
			}
		}

		public virtual string MarshalReturnType {
			get {
				return MarshalType;
			}
		}

		public virtual string DefaultValue {
			get {
				return default_value;
			}
		}

		public virtual string ToNativeReturnType {
			get {
				return MarshalType;
			}
		}

		public virtual string CallByName (string var)
		{
			return var;
		}
		
		public virtual string FromNative(string var)
		{
			return var;
		}
		
		public virtual string FromNativeReturn(string var)
		{
			return FromNative (var);
		}

		public virtual string ToNativeReturn(string var)
		{
			return CallByName (var);
		}

		public bool Validate ()
		{
			return true;
		}

		public void Generate ()
		{
		}
		
		public void Generate (GenerationInfo gen_info)
		{
		}
	}
}

