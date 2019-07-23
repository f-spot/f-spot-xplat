// GtkSharp.Generation.ManagedCallString.cs - The ManagedCallString Class.
//
// Author: Mike Kestner <mkestner@speakeasy.net>
//
// Copyright (c) 2003 Mike Kestner
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

	public class ManagedCallString {
		readonly ArrayList parms = new ArrayList ();
		readonly ArrayList special = new ArrayList ();
		readonly string error_param = null;
		readonly string user_data_param = null;
		readonly string destroy_param = null;

		public ManagedCallString (Parameters parms, bool drop_first)
		{
			for (int i = drop_first ? 1 : 0; i < parms.Count; i ++) {
				var p = parms [i];
				if (p.IsLength && i > 0 && parms [i-1].IsString) 
					continue;
				else if (p.Scope == "notified") {
					user_data_param = parms[i+1].Name;
					destroy_param = parms[i+2].Name;
					i += 2;
				} else if (p.IsUserData && parms.IsHidden (p)) {
					user_data_param = p.Name;
					continue;
				} else if (p is ErrorParameter) {
					error_param = p.Name;
					continue;
				}
				this.parms.Add (p);

				if (p.PassAs != string.Empty && (p.Name != p.FromNative (p.Name)))
					special.Add (true);
				else if (p.Generatable is CallbackGen)
					special.Add (true);
				else
					special.Add (false);
			}
		}

		public bool HasOutParam {
			get {
				foreach (Parameter p in parms) {
					if (p.PassAs == "out")
						return true;
				}
				return false;
			}
		}

		public string Unconditional (string indent) {
			string ret = "";
			if (error_param != null)
				ret = indent + error_param + " = IntPtr.Zero;\n";
			return ret;
		}

		public string Setup (string indent)
		{
			string ret = "";

			for (int i = 0; i < parms.Count; i ++) {
				if ((bool)special[i] == false)
					continue;

				var p = parms [i] as Parameter;
				var igen = p.Generatable;

				if (igen is CallbackGen) {
					if (user_data_param == null)
						ret += indent + string.Format ("{0} {1}_invoker = new {0} ({1});\n", (igen as CallbackGen).InvokerName, p.Name);
					else if (destroy_param == null)
						ret += indent + string.Format ("{0} {1}_invoker = new {0} ({1}, {2});\n", (igen as CallbackGen).InvokerName, p.Name, user_data_param);
					else
						ret += indent + string.Format ("{0} {1}_invoker = new {0} ({1}, {2}, {3});\n", (igen as CallbackGen).InvokerName, p.Name, user_data_param, destroy_param);
				} else {
					ret += indent + igen.QualifiedName + " my" + p.Name;
					if (p.PassAs == "ref")
						ret += " = " + p.FromNative (p.Name);
					ret += ";\n";
				}
			}

			return ret;
		}

		public override string ToString ()
		{
			if (parms.Count < 1)
				return "";

			string[] result = new string [parms.Count];

			for (int i = 0; i < parms.Count; i ++) {
				var p = parms [i] as Parameter;
				result [i] = p.PassAs == "" ? "" : p.PassAs + " ";
				if (p.Generatable is CallbackGen)
					result [i] += p.Name + "_invoker.Handler";
				else
					result [i] += ((bool)special[i]) ? "my" + p.Name : p.FromNative (p.Name);
			}

			return string.Join (", ", result);
		}

		public string Finish (string indent)
		{
			string ret = "";

			for (int i = 0; i < parms.Count; i ++) {
				if ((bool)special[i] == false)
					continue;

				var p = parms [i] as Parameter;
				var igen = p.Generatable;

				if (igen is CallbackGen)
					continue;
				else if (igen is StructBase || igen is ByRefGen)
					ret += indent + string.Format ("if ({0} != IntPtr.Zero) System.Runtime.InteropServices.Marshal.StructureToPtr (my{0}, {0}, false);\n", p.Name);
				else
					ret += indent + p.Name + " = " + igen.ToNativeReturn ("my" + p.Name) + ";\n";
			}

			return ret;
		}
	}
}

