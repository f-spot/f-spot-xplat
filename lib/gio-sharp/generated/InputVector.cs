// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Collections;
	using System.Runtime.InteropServices;

#region Autogenerated code
	[StructLayout(LayoutKind.Sequential)]
	public struct InputVector {
		readonly IntPtr _buffer;
		UIntPtr size;
		public ulong Size {
			get {
				return (ulong) size;
			}
			set {
				size = new UIntPtr (value);
			}
		}

		public static InputVector Zero = new InputVector ();

		public static InputVector New (IntPtr raw) {
			if (raw == IntPtr.Zero)
				return GLib.InputVector.Zero;
			return (InputVector) Marshal.PtrToStructure (raw, typeof (InputVector));
		}

		static GLib.GType GType {
			get { return GLib.GType.Pointer; }
		}
#endregion
	}
}