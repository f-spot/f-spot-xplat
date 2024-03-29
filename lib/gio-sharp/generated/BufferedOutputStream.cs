// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Collections;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public class BufferedOutputStream : FilterOutputStream
	{

		[Obsolete]
		protected BufferedOutputStream(GLib.GType gtype) : base(gtype) {}
		public BufferedOutputStream(IntPtr raw) : base(raw) {}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_buffered_output_stream_new(IntPtr base_stream);

		public BufferedOutputStream (OutputStream base_stream) : base (IntPtr.Zero)
		{
			if (GetType () != typeof (BufferedOutputStream)) {
				var vals = new ArrayList();
				var names = new ArrayList();
				if (base_stream != null) {
					names.Add ("base_stream");
					vals.Add (new GLib.Value (base_stream));
				}
				CreateNativeObject ((string[])names.ToArray (typeof (string)), (GLib.Value[])vals.ToArray (typeof (GLib.Value)));
				return;
			}
			Raw = g_buffered_output_stream_new(base_stream == null ? IntPtr.Zero : base_stream.Handle);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_buffered_output_stream_new_sized(IntPtr base_stream, UIntPtr size);

		public BufferedOutputStream (OutputStream base_stream, ulong size) : base (IntPtr.Zero)
		{
			if (GetType () != typeof (BufferedOutputStream)) {
				throw new InvalidOperationException ("Can't override this constructor.");
			}
			Raw = g_buffered_output_stream_new_sized(base_stream == null ? IntPtr.Zero : base_stream.Handle, new UIntPtr (size));
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_buffered_output_stream_get_auto_grow(IntPtr raw);

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_buffered_output_stream_set_auto_grow(IntPtr raw, bool auto_grow);

		[GLib.Property ("auto-grow")]
		public bool AutoGrow {
			get  {
				bool raw_ret = g_buffered_output_stream_get_auto_grow(Handle);
				bool ret = raw_ret;
				return ret;
			}
			set  {
				g_buffered_output_stream_set_auto_grow(Handle, value);
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern UIntPtr g_buffered_output_stream_get_buffer_size(IntPtr raw);

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_buffered_output_stream_set_buffer_size(IntPtr raw, UIntPtr size);

		[GLib.Property ("buffer-size")]
		public ulong BufferSize {
			get  {
				var raw_ret = g_buffered_output_stream_get_buffer_size(Handle);
				ulong ret = (ulong) raw_ret;
				return ret;
			}
			set  {
				g_buffered_output_stream_set_buffer_size(Handle, new UIntPtr (value));
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_buffered_output_stream_get_type();

		public static new GLib.GType GType { 
			get {
				var raw_ret = g_buffered_output_stream_get_type();
				var ret = new GLib.GType(raw_ret);
				return ret;
			}
		}

#endregion
	}
}
