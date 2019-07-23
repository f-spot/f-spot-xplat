// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Collections;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public class FilterInputStream : InputStream
	{

		[Obsolete]
		protected FilterInputStream(GLib.GType gtype) : base(gtype) {}
		public FilterInputStream(IntPtr raw) : base(raw) {}

		protected FilterInputStream() : base(IntPtr.Zero)
		{
			CreateNativeObject (new string [0], new GLib.Value [0]);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_filter_input_stream_get_base_stream(IntPtr raw);

		[GLib.Property ("base-stream")]
		public InputStream BaseStream {
			get  {
				var raw_ret = g_filter_input_stream_get_base_stream(Handle);
				var ret = GLib.Object.GetObject (raw_ret) as InputStream;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_filter_input_stream_get_close_base_stream(IntPtr raw);

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_filter_input_stream_set_close_base_stream(IntPtr raw, bool close_base);

		[GLib.Property ("close-base-stream")]
		public bool CloseBaseStream {
			get  {
				bool raw_ret = g_filter_input_stream_get_close_base_stream(Handle);
				bool ret = raw_ret;
				return ret;
			}
			set  {
				g_filter_input_stream_set_close_base_stream(Handle, value);
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_filter_input_stream_get_type();

		public static new GLib.GType GType { 
			get {
				var raw_ret = g_filter_input_stream_get_type();
				var ret = new GLib.GType(raw_ret);
				return ret;
			}
		}

#endregion
	}
}
