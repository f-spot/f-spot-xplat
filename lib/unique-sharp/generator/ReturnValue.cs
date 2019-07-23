// GtkSharp.Generation.ReturnValue.cs - The ReturnValue Generatable.
//
// Author: Mike Kestner <mkestner@novell.com>
//
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

	public class ReturnValue  {
		readonly bool is_null_term;
		readonly bool is_array;
		readonly bool elements_owned;
		readonly bool owned;
		readonly string element_ctype = string.Empty;

		public ReturnValue (XmlElement elem) 
		{
			if (elem != null) {
				is_null_term = elem.HasAttribute ("null_term_array");
				is_array = elem.HasAttribute ("array");
				elements_owned = elem.GetAttribute ("elements_owned") == "true";
				owned = elem.GetAttribute ("owned") == "true";
				CType = elem.GetAttribute("type");
				element_ctype = elem.GetAttribute ("element_type");
			}
		}

        public string CType { get; } = string.Empty;

		public string CSType {
			get {
				if (IGen == null)
					return string.Empty;

				if (ElementType != string.Empty)
					return ElementType + "[]";

				return IGen.QualifiedName + (is_array || is_null_term ? "[]" : string.Empty);
			}
		}

		public string DefaultValue {
			get {
				if (IGen == null)
					return string.Empty;
				return IGen.DefaultValue;
			}
		}

		string ElementType {
			get {
				if (element_ctype.Length > 0)
					return SymbolTable.Table.GetCSType (element_ctype);

				return string.Empty;
			}
		}

		IGeneratable igen;
		IGeneratable IGen {
			get {
				if (igen == null)
					igen = SymbolTable.Table [CType];
				return igen;
			}
		}

		public bool IsVoid {
			get {
				return CSType == "void";
			}
		}

		public string MarshalType {
			get {
				if (IGen == null)
					return string.Empty;
				else if (is_null_term)
					return "IntPtr";
				return IGen.MarshalReturnType + (is_array ? "[]" : string.Empty);
			}
		}

		public string ToNativeType {
			get {
				if (IGen == null)
					return string.Empty;
				else if (is_null_term)
					return "IntPtr"; //FIXME
				return IGen.ToNativeReturnType + (is_array ? "[]" : string.Empty);
			}
		}

		public string FromNative (string var)
		{
			if (IGen == null)
				return string.Empty;

			if (ElementType != string.Empty) {
				string args = (owned ? "true" : "false") + ", " + (elements_owned ? "true" : "false");
				if (IGen.QualifiedName == "GLib.PtrArray")
					return string.Format ("({0}[]) GLib.Marshaller.PtrArrayToArray ({1}, {2}, typeof({0}))", ElementType, var, args);
				else
					return string.Format ("({0}[]) GLib.Marshaller.ListPtrToArray ({1}, typeof({2}), {3}, typeof({0}))", ElementType, var, IGen.QualifiedName, args);
			} else if (IGen is HandleBase)
				return ((HandleBase)IGen).FromNative (var, owned);
			else if (is_null_term)
				return string.Format ("GLib.Marshaller.NullTermPtrToStringArray ({0}, {1})", var, owned ? "true" : "false");
			else
				return IGen.FromNativeReturn (var);
		}
			
		public string ToNative (string var)
		{
			if (IGen == null)
				return string.Empty;

			if (ElementType.Length > 0) {
				string args = ", typeof (" + ElementType + "), " + (owned ? "true" : "false") + ", " + (elements_owned ? "true" : "false");
				var = "new " + IGen.QualifiedName + "(" + var + args + ")";
			} else if (is_null_term)
				return string.Format ("GLib.Marshaller.StringArrayToNullTermPtr ({0})", var);

			if (IGen is IManualMarshaler)
				return (IGen as IManualMarshaler).AllocNative (var);
			else if (IGen is ObjectGen && owned)
				return var + " == null ? IntPtr.Zero : " + var + ".OwnedHandle";
			else if (IGen is OpaqueGen && owned)
				return var + " == null ? IntPtr.Zero : " + var + ".OwnedCopy";
			else
				return IGen.ToNativeReturn (var);
		}

		public bool Validate ()
		{
			if (MarshalType == "" || CSType == "") {
				Console.Write("rettype: " + CType);
				return false;
			}

			return true;
		}
	}
}

