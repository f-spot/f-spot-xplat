// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Runtime.InteropServices;

#region Autogenerated code
	[GLib.GType (typeof (DriveStartStopTypeGType))]
	public enum DriveStartStopType {

		Unknown,
		Shutdown,
		Network,
		Multidisk,
		Password,
	}

	class DriveStartStopTypeGType {
		[DllImport ("libgio-2.0-0.dll")]
		static extern IntPtr g_drive_start_stop_type_get_type ();

		public static GLib.GType GType {
			get {
				return new GLib.GType (g_drive_start_stop_type_get_type ());
			}
		}
	}
#endregion
}
