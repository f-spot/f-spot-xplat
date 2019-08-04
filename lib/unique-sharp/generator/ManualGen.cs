// GtkSharp.Generation.ManualGen.cs - Ungenerated handle type Generatable.
//
// Author: Mike Kestner <mkestner@novell.com>
//
// Copyright (c) 2003 Mike Kestner
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

	public class ManualGen : SimpleBase {
		readonly string from_fmt;

		public ManualGen (string ctype, string type) : base (ctype, type, "null") 
		{
			from_fmt = "new " + QualifiedName + "({0})";
		}

		public ManualGen (string ctype, string type, string from_fmt) : base (ctype, type, "null")
		{
			this.from_fmt = from_fmt;
		}
		
		public override string MarshalType {
			get {
				return "IntPtr";
			}
		}

		public override string CallByName (string var_name)
		{
			return var_name + " == null ? IntPtr.Zero : " + var_name + ".Handle";
		}
		
		public override string FromNative(string var)
		{
			return string.Format (from_fmt, var);
		}
	}
}

