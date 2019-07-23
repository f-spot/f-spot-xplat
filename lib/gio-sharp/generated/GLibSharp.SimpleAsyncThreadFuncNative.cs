// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLibSharp {

	using System;
	using System.Runtime.InteropServices;

#region Autogenerated code
	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
delegate void SimpleAsyncThreadFuncNative(IntPtr res, IntPtr objekt, IntPtr cancellable);

	class SimpleAsyncThreadFuncInvoker {
		readonly SimpleAsyncThreadFuncNative native_cb;
		readonly IntPtr __data;
		GLib.DestroyNotify __notify;

		~SimpleAsyncThreadFuncInvoker ()
		{
			if (__notify == null)
				return;
			__notify (__data);
		}

		internal SimpleAsyncThreadFuncInvoker (SimpleAsyncThreadFuncNative native_cb) : this (native_cb, IntPtr.Zero, null) {}

		internal SimpleAsyncThreadFuncInvoker (SimpleAsyncThreadFuncNative native_cb, IntPtr data) : this (native_cb, data, null) {}

		internal SimpleAsyncThreadFuncInvoker (SimpleAsyncThreadFuncNative native_cb, IntPtr data, GLib.DestroyNotify notify)
		{
			this.native_cb = native_cb;
			__data = data;
			__notify = notify;
		}

		internal GLib.SimpleAsyncThreadFunc Handler {
			get {
				return new GLib.SimpleAsyncThreadFunc(InvokeNative);
			}
		}

		void InvokeNative (GLib.SimpleAsyncResult res, GLib.Object objekt, GLib.Cancellable cancellable)
		{
			native_cb (res == null ? IntPtr.Zero : res.Handle, objekt == null ? IntPtr.Zero : objekt.Handle, cancellable == null ? IntPtr.Zero : cancellable.Handle);
		}
	}

	class SimpleAsyncThreadFuncWrapper {

		public void NativeCallback (IntPtr res, IntPtr objekt, IntPtr cancellable)
		{
			try {
				managed (GLib.Object.GetObject(res) as GLib.SimpleAsyncResult, GLib.Object.GetObject (objekt), GLib.Object.GetObject(cancellable) as GLib.Cancellable);
				if (release_on_call)
					gch.Free ();
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		bool release_on_call = false;
		GCHandle gch;

		public void PersistUntilCalled ()
		{
			release_on_call = true;
			gch = GCHandle.Alloc (this);
		}

		internal SimpleAsyncThreadFuncNative NativeDelegate;
		readonly GLib.SimpleAsyncThreadFunc managed;

		public SimpleAsyncThreadFuncWrapper (GLib.SimpleAsyncThreadFunc managed)
		{
			this.managed = managed;
			if (managed != null)
				NativeDelegate = new SimpleAsyncThreadFuncNative (NativeCallback);
		}

		public static GLib.SimpleAsyncThreadFunc GetManagedDelegate (SimpleAsyncThreadFuncNative native)
		{
			if (native == null)
				return null;
			var wrapper = (SimpleAsyncThreadFuncWrapper) native.Target;
			if (wrapper == null)
				return null;
			return wrapper.managed;
		}
	}
#endregion
}
