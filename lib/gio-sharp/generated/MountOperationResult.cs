// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Runtime.InteropServices;

#region Autogenerated code
	[GLib.GType (typeof (MountOperationResultGType))]
	public enum MountOperationResult {

		Handled,
		Aborted,
		Unhandled,
	}

	class MountOperationResultGType {
		[DllImport ("libgio-2.0-0.dll")]
		static extern IntPtr g_mount_operation_result_get_type ();

		public static GLib.GType GType {
			get {
				return new GLib.GType (g_mount_operation_result_get_type ());
			}
		}
	}
#endregion
}
