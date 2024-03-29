// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Collections;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public class FileAttributeInfoList : GLib.Opaque {

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_file_attribute_info_list_get_type();

		public static GLib.GType GType { 
			get {
				var raw_ret = g_file_attribute_info_list_get_type();
				var ret = new GLib.GType(raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_file_attribute_info_list_dup(IntPtr raw);

		public FileAttributeInfoList Dup () {
			var raw_ret = g_file_attribute_info_list_dup(Handle);
			FileAttributeInfoList ret = raw_ret == IntPtr.Zero ? null : (FileAttributeInfoList) GLib.Opaque.GetOpaque (raw_ret, typeof (FileAttributeInfoList), false);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_file_attribute_info_list_lookup(IntPtr raw, IntPtr name);

		public FileAttributeInfo Lookup (string name) {
			IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (name);
			var raw_ret = g_file_attribute_info_list_lookup(Handle, native_name);
			var ret = GLib.FileAttributeInfo.New (raw_ret);
			GLib.Marshaller.Free (native_name);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_file_attribute_info_list_add(IntPtr raw, IntPtr name, int type, int flags);

		public void Add(string name, FileAttributeType type, FileAttributeInfoFlags flags) {
			IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (name);
			g_file_attribute_info_list_add(Handle, native_name, (int) type, (int) flags);
			GLib.Marshaller.Free (native_name);
		}

		public FileAttributeInfoList(IntPtr raw) : base(raw) {}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_file_attribute_info_list_new();

		public FileAttributeInfoList () 
		{
			Raw = g_file_attribute_info_list_new();
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_file_attribute_info_list_ref(IntPtr raw);

		protected override void Ref (IntPtr raw)
		{
			if (!Owned) {
				g_file_attribute_info_list_ref (raw);
				Owned = true;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_file_attribute_info_list_unref(IntPtr raw);

		protected override void Unref (IntPtr raw)
		{
			if (Owned) {
				g_file_attribute_info_list_unref (raw);
				Owned = false;
			}
		}

		class FinalizerInfo {
			readonly IntPtr handle;

			public FinalizerInfo (IntPtr handle)
			{
				this.handle = handle;
			}

			public bool Handler ()
			{
				g_file_attribute_info_list_unref (handle);
				return false;
			}
		}

		~FileAttributeInfoList ()
		{
			if (!Owned)
				return;
			var info = new FinalizerInfo (Handle);
			GLib.Timeout.Add (50, new GLib.TimeoutHandler (info.Handler));
		}

#endregion
	}
}
