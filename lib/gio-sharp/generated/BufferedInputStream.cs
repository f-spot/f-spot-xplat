// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Collections;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public class BufferedInputStream : FilterInputStream
	{

		[Obsolete]
		protected BufferedInputStream(GLib.GType gtype) : base(gtype) {}
		public BufferedInputStream(IntPtr raw) : base(raw) {}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_buffered_input_stream_new(IntPtr base_stream);

		public BufferedInputStream (InputStream base_stream) : base (IntPtr.Zero)
		{
			if (GetType () != typeof (BufferedInputStream)) {
				var vals = new ArrayList();
				var names = new ArrayList();
				if (base_stream != null) {
					names.Add ("base_stream");
					vals.Add (new GLib.Value (base_stream));
				}
				CreateNativeObject ((string[])names.ToArray (typeof (string)), (GLib.Value[])vals.ToArray (typeof (GLib.Value)));
				return;
			}
			Raw = g_buffered_input_stream_new(base_stream == null ? IntPtr.Zero : base_stream.Handle);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_buffered_input_stream_new_sized(IntPtr base_stream, UIntPtr size);

		public BufferedInputStream (InputStream base_stream, ulong size) : base (IntPtr.Zero)
		{
			if (GetType () != typeof (BufferedInputStream)) {
				throw new InvalidOperationException ("Can't override this constructor.");
			}
			Raw = g_buffered_input_stream_new_sized(base_stream == null ? IntPtr.Zero : base_stream.Handle, new UIntPtr (size));
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern UIntPtr g_buffered_input_stream_get_buffer_size(IntPtr raw);

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_buffered_input_stream_set_buffer_size(IntPtr raw, UIntPtr size);

		[GLib.Property ("buffer-size")]
		public ulong BufferSize {
			get  {
				var raw_ret = g_buffered_input_stream_get_buffer_size(Handle);
				ulong ret = (ulong) raw_ret;
				return ret;
			}
			set  {
				g_buffered_input_stream_set_buffer_size(Handle, new UIntPtr (value));
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_buffered_input_stream_get_type();

		public static new GLib.GType GType { 
			get {
				var raw_ret = g_buffered_input_stream_get_type();
				var ret = new GLib.GType(raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern unsafe int g_buffered_input_stream_read_byte(IntPtr raw, IntPtr cancellable, out IntPtr error);

		public unsafe int ReadByte(Cancellable cancellable) {
			var error = IntPtr.Zero;
			int raw_ret = g_buffered_input_stream_read_byte(Handle, cancellable == null ? IntPtr.Zero : cancellable.Handle, out error);
			int ret = raw_ret;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern unsafe IntPtr g_buffered_input_stream_fill_finish(IntPtr raw, IntPtr result, out IntPtr error);

		public unsafe long FillFinish(AsyncResult result) {
			var error = IntPtr.Zero;
			var raw_ret = g_buffered_input_stream_fill_finish(Handle, result == null ? IntPtr.Zero : result.Handle, out error);
			long ret = (long) raw_ret;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_buffered_input_stream_fill_async(IntPtr raw, IntPtr count, int io_priority, IntPtr cancellable, GLibSharp.AsyncReadyCallbackNative cb, IntPtr user_data);

		public void FillAsync(long count, int io_priority, Cancellable cancellable, AsyncReadyCallback cb) {
			var cb_wrapper = new GLibSharp.AsyncReadyCallbackWrapper (cb);
			cb_wrapper.PersistUntilCalled ();
			g_buffered_input_stream_fill_async(Handle, new IntPtr (count), io_priority, cancellable == null ? IntPtr.Zero : cancellable.Handle, cb_wrapper.NativeDelegate, IntPtr.Zero);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern UIntPtr g_buffered_input_stream_peek(IntPtr raw, IntPtr buffer, UIntPtr offset, UIntPtr count);

		public ulong Peek(IntPtr buffer, ulong offset, ulong count) {
			var raw_ret = g_buffered_input_stream_peek(Handle, buffer, new UIntPtr (offset), new UIntPtr (count));
			ulong ret = (ulong) raw_ret;
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern UIntPtr g_buffered_input_stream_get_available(IntPtr raw);

		public ulong Available { 
			get {
				var raw_ret = g_buffered_input_stream_get_available(Handle);
				ulong ret = (ulong) raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_buffered_input_stream_peek_buffer(IntPtr raw, out UIntPtr count);

		public IntPtr PeekBuffer(out ulong count) {
			UIntPtr native_count;
			var raw_ret = g_buffered_input_stream_peek_buffer(Handle, out native_count);
			var ret = raw_ret;
			count = (ulong) native_count;
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern unsafe IntPtr g_buffered_input_stream_fill(IntPtr raw, IntPtr count, IntPtr cancellable, out IntPtr error);

		public unsafe long Fill(long count, Cancellable cancellable) {
			var error = IntPtr.Zero;
			var raw_ret = g_buffered_input_stream_fill(Handle, new IntPtr (count), cancellable == null ? IntPtr.Zero : cancellable.Handle, out error);
			long ret = (long) raw_ret;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

#endregion
	}
}