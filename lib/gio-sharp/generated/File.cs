// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;

#region Autogenerated code
	public interface File : GLib.IWrapper {

		bool Copy(GLib.File destination, GLib.FileCopyFlags flags, GLib.Cancellable cancellable, GLib.FileProgressCallback progress_callback);
		string Basename { 
			get;
		}
		GLib.FileOutputStream Create(GLib.FileCreateFlags flags, GLib.Cancellable cancellable);
		void EjectMountableWithOperation(GLib.MountUnmountFlags flags, GLib.MountOperation mount_operation, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		void LoadContentsAsync(GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		GLib.FileIOStream OpenReadwriteFinish(GLib.AsyncResult res);
		GLib.File SetDisplayName(string display_name, GLib.Cancellable cancellable);
		GLib.FileAttributeInfoList QueryWritableNamespaces(GLib.Cancellable cancellable);
		void LoadPartialContentsAsync(GLib.Cancellable cancellable, GLib.FileReadMoreCallback read_more_callback, GLib.AsyncReadyCallback cb);
		void ReadAsync(int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		void SetAttributesAsync(GLib.FileInfo info, GLib.FileQueryInfoFlags flags, int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		GLib.FileIOStream CreateReadwriteFinish(GLib.AsyncResult res);
		bool EjectMountableFinish(GLib.AsyncResult result);
		GLib.AppInfo QueryDefaultHandler(GLib.Cancellable cancellable);
		string Path { 
			get;
		}
		bool Equal(GLib.File file2);
		bool SetAttributesFromInfo(GLib.FileInfo info, GLib.FileQueryInfoFlags flags, GLib.Cancellable cancellable);
		bool HasPrefix(GLib.File prefix);
		GLib.FileInfo QueryInfoFinish(GLib.AsyncResult res);
		GLib.File SetDisplayNameFinish(GLib.AsyncResult res);
		bool MakeDirectory(GLib.Cancellable cancellable);
		GLib.FileOutputStream AppendToFinish(GLib.AsyncResult res);
		bool LoadContents(GLib.Cancellable cancellable, string contents, out ulong length, string etag_out);
		GLib.File Parent { 
			get;
		}
		void EjectMountable(GLib.MountUnmountFlags flags, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		bool Delete(GLib.Cancellable cancellable);
		GLib.FileOutputStream AppendTo(GLib.FileCreateFlags flags, GLib.Cancellable cancellable);
		GLib.FileType QueryFileType(GLib.FileQueryInfoFlags flags, GLib.Cancellable cancellable);
		bool EjectMountableWithOperationFinish(GLib.AsyncResult result);
		string GetRelativePath(GLib.File descendant);
		bool Move(GLib.File destination, GLib.FileCopyFlags flags, GLib.Cancellable cancellable, GLib.FileProgressCallback progress_callback);
		GLib.FileEnumerator EnumerateChildren(string attributes, GLib.FileQueryInfoFlags flags, GLib.Cancellable cancellable);
		void FindEnclosingMountAsync(int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		bool CopyFinish(GLib.AsyncResult res);
		GLib.FileInfo QueryInfo(string attributes, GLib.FileQueryInfoFlags flags, GLib.Cancellable cancellable);
		bool PollMountableFinish(GLib.AsyncResult result);
		void SetDisplayNameAsync(string display_name, int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		bool SetAttributeInt64(string attribute, long value, GLib.FileQueryInfoFlags flags, GLib.Cancellable cancellable);
		void UnmountMountableWithOperation(GLib.MountUnmountFlags flags, GLib.MountOperation mount_operation, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		void ReplaceReadwriteAsync(string etag, bool make_backup, GLib.FileCreateFlags flags, int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		GLib.File GetChildForDisplayName(string display_name);
		void ReplaceContentsAsync(string contents, string etag, bool make_backup, GLib.FileCreateFlags flags, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		void StopMountable(GLib.MountUnmountFlags flags, GLib.MountOperation mount_operation, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		bool StopMountableFinish(GLib.AsyncResult result);
		void CreateAsync(GLib.FileCreateFlags flags, int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		GLib.FileMonitor Monitor(GLib.FileMonitorFlags flags, GLib.Cancellable cancellable);
		void QueryInfoAsync(string attributes, GLib.FileQueryInfoFlags flags, int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		bool IsNative { 
			get;
		}
		bool QueryExists(GLib.Cancellable cancellable);
		void CreateReadwriteAsync(GLib.FileCreateFlags flags, int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		bool SetAttribute(string attribute, GLib.FileAttributeType type, IntPtr value_p, GLib.FileQueryInfoFlags flags, GLib.Cancellable cancellable);
		bool UnmountMountableWithOperationFinish(GLib.AsyncResult result);
		bool Trash(GLib.Cancellable cancellable);
		GLib.FileIOStream ReplaceReadwriteFinish(GLib.AsyncResult res);
		GLib.File GetChild(string name);
		bool LoadPartialContentsFinish(GLib.AsyncResult res, string contents, out ulong length, string etag_out);
		bool MakeDirectoryWithParents(GLib.Cancellable cancellable);
		GLib.File Dup();
		GLib.FileIOStream CreateReadwrite(GLib.FileCreateFlags flags, GLib.Cancellable cancellable);
		GLib.Mount FindEnclosingMountFinish(GLib.AsyncResult res);
		GLib.FileInputStream Read(GLib.Cancellable cancellable);
		bool LoadContentsFinish(GLib.AsyncResult res, string contents, out ulong length, string etag_out);
		void QueryFilesystemInfoAsync(string attributes, int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		GLib.File ResolveRelativePath(string relative_path);
		GLib.FileOutputStream CreateFinish(GLib.AsyncResult res);
		void MountMountable(GLib.MountMountFlags flags, GLib.MountOperation mount_operation, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		GLib.FileOutputStream ReplaceFinish(GLib.AsyncResult res);
		GLib.File MountMountableFinish(GLib.AsyncResult result);
		void PollMountable(GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		GLib.FileEnumerator EnumerateChildrenFinish(GLib.AsyncResult res);
		string UriScheme { 
			get;
		}
		bool HasUriScheme(string uri_scheme);
		GLib.FileInfo QueryFilesystemInfoFinish(GLib.AsyncResult res);
		bool StartMountableFinish(GLib.AsyncResult result);
		string ParsedName { 
			get;
		}
		GLib.FileIOStream OpenReadwrite(GLib.Cancellable cancellable);
		void OpenReadwriteAsync(int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		bool SupportsThreadContexts();
		bool UnmountMountableFinish(GLib.AsyncResult result);
		GLib.Mount FindEnclosingMount(GLib.Cancellable cancellable);
		void ReplaceAsync(string etag, bool make_backup, GLib.FileCreateFlags flags, int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		void StartMountable(GLib.DriveStartFlags flags, GLib.MountOperation start_operation, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		void CopyAsync(GLib.File destination, GLib.FileCopyFlags flags, int io_priority, GLib.Cancellable cancellable, GLib.FileProgressCallback progress_callback, GLib.AsyncReadyCallback cb);
		GLib.FileAttributeInfoList QuerySettableAttributes(GLib.Cancellable cancellable);
		bool SetAttributesFinish(GLib.AsyncResult result, GLib.FileInfo info);
		void MountEnclosingVolume(GLib.MountMountFlags flags, GLib.MountOperation mount_operation, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		bool ReplaceContentsFinish(GLib.AsyncResult res, string new_etag);
		bool SetAttributeUint32(string attribute, uint value, GLib.FileQueryInfoFlags flags, GLib.Cancellable cancellable);
		void AppendToAsync(GLib.FileCreateFlags flags, int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		bool SetAttributeInt32(string attribute, int value, GLib.FileQueryInfoFlags flags, GLib.Cancellable cancellable);
		bool MountEnclosingVolumeFinish(GLib.AsyncResult result);
		bool SetAttributeByteString(string attribute, string value, GLib.FileQueryInfoFlags flags, GLib.Cancellable cancellable);
		bool CopyAttributes(GLib.File destination, GLib.FileCopyFlags flags, GLib.Cancellable cancellable);
		bool SetAttributeString(string attribute, string value, GLib.FileQueryInfoFlags flags, GLib.Cancellable cancellable);
		GLib.FileOutputStream Replace(string etag, bool make_backup, GLib.FileCreateFlags flags, GLib.Cancellable cancellable);
		void EnumerateChildrenAsync(string attributes, GLib.FileQueryInfoFlags flags, int io_priority, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		bool SetAttributeUint64(string attribute, ulong value, GLib.FileQueryInfoFlags flags, GLib.Cancellable cancellable);
		bool MakeSymbolicLink(string symlink_value, GLib.Cancellable cancellable);
		bool ReplaceContents(string contents, string etag, bool make_backup, GLib.FileCreateFlags flags, string new_etag, GLib.Cancellable cancellable);
		void UnmountMountable(GLib.MountUnmountFlags flags, GLib.Cancellable cancellable, GLib.AsyncReadyCallback cb);
		GLib.FileInfo QueryFilesystemInfo(string attributes, GLib.Cancellable cancellable);
		GLib.FileInputStream ReadFinish(GLib.AsyncResult res);
		GLib.FileIOStream ReplaceReadwrite(string etag, bool make_backup, GLib.FileCreateFlags flags, GLib.Cancellable cancellable);
#region Customized extensions
#line 1 "File.custom"
// File.custom - customizations to GLib.File
//
// Authors: Stephane Delcroix  <stephane@delcroix.org>
//
// Copyright (C) 2008 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

bool Exists
{
	get;
}

		Uri Uri
{
	get;
}

bool Delete ();

void Dispose ();

#endregion
	}

	[GLib.GInterface (typeof (FileAdapter))]
	public interface FileImplementor : GLib.IWrapper {

		File Dup ();
		uint Hash ();
		bool Equal (File file2);
		bool IsNative { get; }
		bool HasUriScheme (string uri_scheme);
		string UriScheme { get; }
		string Basename { get; }
		string Path { get; }
		string Uri { get; }
		string ParseName { get; }
		File Parent { get; }
		bool PrefixMatches (File file);
		string GetRelativePath (File descendant);
		File ResolveRelativePath (string relative_path);
		File GetChildForDisplayName (string display_name);
		FileEnumerator EnumerateChildren (string attributes, FileQueryInfoFlags flags, Cancellable cancellable);
		void EnumerateChildrenAsync (string attributes, FileQueryInfoFlags flags, int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		FileEnumerator EnumerateChildrenFinish (AsyncResult res);
		FileInfo QueryInfo (string attributes, FileQueryInfoFlags flags, Cancellable cancellable);
		void QueryInfoAsync (string attributes, FileQueryInfoFlags flags, int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		FileInfo QueryInfoFinish (AsyncResult res);
		FileInfo QueryFilesystemInfo (string attributes, Cancellable cancellable);
		void QueryFilesystemInfoAsync (string attributes, int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		FileInfo QueryFilesystemInfoFinish (AsyncResult res);
		Mount FindEnclosingMount (Cancellable cancellable);
		void FindEnclosingMountAsync (int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		Mount FindEnclosingMountFinish (AsyncResult res);
		File SetDisplayName (string display_name, Cancellable cancellable);
		void SetDisplayNameAsync (string display_name, int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		File SetDisplayNameFinish (AsyncResult res);
		FileAttributeInfoList QuerySettableAttributes (Cancellable cancellable);
		void QuerySettableAttributesAsync ();
		void QuerySettableAttributesFinish ();
		FileAttributeInfoList QueryWritableNamespaces (Cancellable cancellable);
		void QueryWritableNamespacesAsync ();
		void QueryWritableNamespacesFinish ();
		bool SetAttribute (string attribute, FileAttributeType type, IntPtr value_p, FileQueryInfoFlags flags, Cancellable cancellable);
		bool SetAttributesFromInfo (FileInfo info, FileQueryInfoFlags flags, Cancellable cancellable);
		void SetAttributesAsync (FileInfo info, FileQueryInfoFlags flags, int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		bool SetAttributesFinish (AsyncResult result, FileInfo info);
		FileInputStream ReadFn (Cancellable cancellable);
		void ReadAsync (int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		FileInputStream ReadFinish (AsyncResult res);
		FileOutputStream AppendTo (FileCreateFlags flags, Cancellable cancellable);
		void AppendToAsync (FileCreateFlags flags, int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		FileOutputStream AppendToFinish (AsyncResult res);
		FileOutputStream Create (FileCreateFlags flags, Cancellable cancellable);
		void CreateAsync (FileCreateFlags flags, int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		FileOutputStream CreateFinish (AsyncResult res);
		FileOutputStream Replace (string etag, bool make_backup, FileCreateFlags flags, Cancellable cancellable);
		void ReplaceAsync (string etag, bool make_backup, FileCreateFlags flags, int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		FileOutputStream ReplaceFinish (AsyncResult res);
		bool DeleteFile (Cancellable cancellable);
		void DeleteFileAsync ();
		void DeleteFileFinish ();
		bool Trash (Cancellable cancellable);
		void TrashAsync ();
		void TrashFinish ();
		bool MakeDirectory (Cancellable cancellable);
		void MakeDirectoryAsync ();
		void MakeDirectoryFinish ();
		bool MakeSymbolicLink (string symlink_value, Cancellable cancellable);
		void MakeSymbolicLinkAsync ();
		void MakeSymbolicLinkFinish ();
		bool Copy (File destination, FileCopyFlags flags, Cancellable cancellable, FileProgressCallback progress_callback);
		void CopyAsync (File destination, FileCopyFlags flags, int io_priority, Cancellable cancellable, FileProgressCallback progress_callback, AsyncReadyCallback cb);
		bool CopyFinish (AsyncResult res);
		bool Move (File destination, FileCopyFlags flags, Cancellable cancellable, FileProgressCallback progress_callback);
		void MoveAsync ();
		void MoveFinish ();
		void MountMountable (MountMountFlags flags, MountOperation mount_operation, Cancellable cancellable, AsyncReadyCallback cb);
		File MountMountableFinish (AsyncResult result);
		void UnmountMountable (MountUnmountFlags flags, Cancellable cancellable, AsyncReadyCallback cb);
		bool UnmountMountableFinish (AsyncResult result);
		void EjectMountable (MountUnmountFlags flags, Cancellable cancellable, AsyncReadyCallback cb);
		bool EjectMountableFinish (AsyncResult result);
		void MountEnclosingVolume (MountMountFlags flags, MountOperation mount_operation, Cancellable cancellable, AsyncReadyCallback cb);
		bool MountEnclosingVolumeFinish (AsyncResult result);
		FileMonitor MonitorDir (FileMonitorFlags flags, Cancellable cancellable);
		FileMonitor MonitorFile (FileMonitorFlags flags, Cancellable cancellable);
		FileIOStream OpenReadwrite (Cancellable cancellable);
		void OpenReadwriteAsync (int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		FileIOStream OpenReadwriteFinish (AsyncResult res);
		FileIOStream CreateReadwrite (FileCreateFlags flags, Cancellable cancellable);
		void CreateReadwriteAsync (FileCreateFlags flags, int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		FileIOStream CreateReadwriteFinish (AsyncResult res);
		FileIOStream ReplaceReadwrite (string etag, bool make_backup, FileCreateFlags flags, Cancellable cancellable);
		void ReplaceReadwriteAsync (string etag, bool make_backup, FileCreateFlags flags, int io_priority, Cancellable cancellable, AsyncReadyCallback cb);
		FileIOStream ReplaceReadwriteFinish (AsyncResult res);
		void StartMountable (DriveStartFlags flags, MountOperation start_operation, Cancellable cancellable, AsyncReadyCallback cb);
		bool StartMountableFinish (AsyncResult result);
		void StopMountable (MountUnmountFlags flags, MountOperation mount_operation, Cancellable cancellable, AsyncReadyCallback cb);
		bool StopMountableFinish (AsyncResult result);
		void UnmountMountableWithOperation (MountUnmountFlags flags, MountOperation mount_operation, Cancellable cancellable, AsyncReadyCallback cb);
		bool UnmountMountableWithOperationFinish (AsyncResult result);
		void EjectMountableWithOperation (MountUnmountFlags flags, MountOperation mount_operation, Cancellable cancellable, AsyncReadyCallback cb);
		bool EjectMountableWithOperationFinish (AsyncResult result);
		void PollMountable (Cancellable cancellable, AsyncReadyCallback cb);
		bool PollMountableFinish (AsyncResult result);
	}
#endregion
}