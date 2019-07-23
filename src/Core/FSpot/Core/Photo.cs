//
// Photo.cs
//
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
//   Stephane Delcroix <sdelcroix@src.gnome.org>
//   Stephen Shaw <sshaw@decriptor.com>
//
// Copyright (C) 2008-2010 Novell, Inc.
// Copyright (C) 2010 Ruben Vermeersch
// Copyright (C) 2008-2009 Stephane Delcroix
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
// THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using FSpot.Core;
using FSpot.Imaging;
using FSpot.Settings;
using FSpot.Thumbnail;
using FSpot.Translations;
using FSpot.Utils;

using Hyena;
using SixLabors.ImageSharp;

namespace FSpot
{
    public class Photo : DbItem, IComparable<Photo>, IPhoto, IPhotoVersionable
    {
        readonly IImageFileFactory imageFileFactory;
        readonly IThumbnailService thumbnailService;

        PhotoChanges changes = new PhotoChanges();
        public PhotoChanges Changes
        {
            get { return changes; }
            set
            {
                if (value != null)
                    throw new ArgumentException("The only valid value is null");

                changes = new PhotoChanges();
            }
        }

        // The time is always in UTC.
        DateTime time;
        public DateTime Time
        {
            get { return time; }
            set
            {
                if (time == value)
                    return;
                time = value;
                changes.TimeChanged = true;
            }
        }

        public string Name
        {
            get { return Uri.UnescapeDataString(Path.GetFileName(VersionUri(OriginalVersionId).AbsolutePath)); }
        }

        readonly List<Tag> tags;
        public Tag[] Tags
        {
            get
            {
                return tags.ToArray();
            }
        }

        bool all_versions_loaded = false;
        public bool AllVersionsLoaded
        {
            get { return all_versions_loaded; }
            set
            {
                if (value)
                    if (DefaultVersionId != OriginalVersionId && !versions.ContainsKey(DefaultVersionId))
                        DefaultVersionId = OriginalVersionId;
                all_versions_loaded = value;
            }
        }

        string description;
        public string Description
        {
            get { return description; }
            set
            {
                if (description == value)
                    return;
                description = value;
                changes.DescriptionChanged = true;
            }
        }

        uint roll_id = 0;
        public uint RollId
        {
            get { return roll_id; }
            set
            {
                if (roll_id == value)
                    return;
                roll_id = value;
                changes.RollIdChanged = true;
            }
        }

        uint rating;
        public uint Rating
        {
            get { return rating; }
            set
            {
                if (rating == value || value > 5)
                    return;
                rating = value;
                changes.RatingChanged = true;
            }
        }

        public const int OriginalVersionId = 1;
        uint highestVersionId;
        readonly Dictionary<uint, PhotoVersion> versions = new Dictionary<uint, PhotoVersion>();

        public IEnumerable<IPhotoVersion> Versions
        {
            get
            {
                foreach (var version in versions.Values)
                {
                    yield return version;
                }
            }
        }

        public uint[] VersionIds
        {
            get
            {
                if (versions == null)
                    return new uint[0];

                uint[] ids = new uint[versions.Count];
                versions.Keys.CopyTo(ids, 0);
                Array.Sort(ids);
                return ids;
            }
        }

        uint default_version_id = OriginalVersionId;

        public uint DefaultVersionId
        {
            get { return default_version_id; }
            set
            {
                if (default_version_id == value)
                    return;
                default_version_id = value;
                changes.DefaultVersionIdChanged = true;
            }
        }

        public Photo(IImageFileFactory imageFactory, IThumbnailService thumbnailService, uint id, long unixTime) : base(id)
        {
            imageFileFactory = imageFactory;
            this.thumbnailService = thumbnailService;

            time = DateTimeUtil.ToDateTime(unixTime);
            tags = new List<Tag>();

            description = string.Empty;
            rating = 0;
        }

        public PhotoVersion GetVersion(uint version_id)
        {
            if (versions == null)
                return null;

            return versions[version_id];
        }

        // This doesn't check if a version of that name already exists,
        // it's supposed to be used only within the Photo and PhotoStore classes.
        public void AddVersionUnsafely(uint version_id, Uri base_uri, string filename, string import_md5, string name, bool is_protected)
        {
            versions[version_id] = new PhotoVersion(this, version_id, base_uri, filename, import_md5, name, is_protected);

            highestVersionId = Math.Max(version_id, highestVersionId);
            changes.AddVersion(version_id);
        }

        public uint AddVersion(Uri base_uri, string filename, string name)
        {
            return AddVersion(base_uri, filename, name, false);
        }

        public uint AddVersion(Uri base_uri, string filename, string name, bool is_protected)
        {
            if (VersionNameExists(name))
                throw new ApplicationException("A version with that name already exists");

            highestVersionId++;
            string import_md5 = string.Empty; // Modified version

            versions[highestVersionId] = new PhotoVersion(this, highestVersionId, base_uri, filename, import_md5, name, is_protected);

            changes.AddVersion(highestVersionId);
            return highestVersionId;
        }

        //FIXME: store versions next to originals. will crash on ro locations.
        string GetFilenameForVersionName(string version_name, string extension)
        {
            string name_without_extension = Path.GetFileNameWithoutExtension(Name);

            return name_without_extension + " (" +
                UriUtils.EscapeString(version_name, true, true, true)
                + ")" + extension;
        }

        public bool VersionNameExists(string version_name)
        {
            return Versions.Any(v => v.Name == version_name);
        }

        public Uri VersionUri(uint version_id)
        {
            if (!versions.ContainsKey(version_id))
                return null;

            var v = versions[version_id];
            return v?.Uri;
        }

        public IPhotoVersion DefaultVersion
        {
            get
            {
                if (!versions.ContainsKey(DefaultVersionId))
                    throw new Exception("Something is horribly wrong, this should never happen: no default version!");

                return versions[DefaultVersionId];
            }
        }

        public void SetDefaultVersion(IPhotoVersion version)
        {
            if (!(version is PhotoVersion photoVersion))
                throw new ArgumentException("Not a valid version for this photo");

            DefaultVersionId = photoVersion.VersionId;
        }


		//FIXME: won't work on non file uris
		public uint SaveVersion(IImage buffer, bool createVersion)
        {
            if (buffer == null)
                throw new ApplicationException("invalid (null) image");

            uint version = DefaultVersionId;
            using (var img = imageFileFactory.Create(DefaultVersion.Uri))
            {
                // Always create a version if the source is not a jpeg for now.
                createVersion = createVersion || imageFileFactory.IsJpeg(DefaultVersion.Uri.GetExtension ());

                if (createVersion)
                    version = CreateDefaultModifiedVersion(DefaultVersionId, false);

                try
                {
                    var versionUri = VersionUri(version);

                    ImageSharpUtils.CreateDerivedVersion(DefaultVersion.Uri, versionUri, 95, buffer);
                    GetVersion(version).ImportMD5 = HashUtils.GenerateMD5(VersionUri(version));
                    DefaultVersionId = version;
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                    if (createVersion)
                        DeleteVersion(version);

                    throw e;
                }
            }

            return version;
        }

        public void DeleteVersion(uint versionId, bool removeOriginal = false, bool keepFile = false)
        {
            if (versionId == OriginalVersionId && !removeOriginal)
                throw new Exception("Cannot delete original version");

            var uri = VersionUri(versionId);
            var file = new FileInfo(uri.AbsolutePath);

            if (!keepFile && file.Exists)
                file.Delete();

            try
            {
                thumbnailService.DeleteThumbnails(uri);
            }
            catch
            {
                // ignore an error here we don't really care.
            }

            var directory = Directory.GetParent(uri.AbsolutePath);
            if (directory.Exists)
                DeleteEmptyDirectory(directory);

            versions.Remove(versionId);
            changes.RemoveVersion(versionId);

            for (versionId = highestVersionId; versionId >= OriginalVersionId; versionId--)
            {
                if (versions.ContainsKey(versionId))
                {
                    DefaultVersionId = versionId;
                    break;
                }
            }
        }

        // FIXME, We need to validate this!
        void DeleteEmptyDirectory(DirectoryInfo directory)
        {
            // if the directory we're dealing with is not in the
            // F-Spot photos directory, don't delete anything,
            // even if it is empty
            string photoUri = Global.PhotoUri.GetFilename();
            bool path_matched = directory.FullName.IndexOf(photoUri) > -1;

            if (directory.Name.Equals(photoUri) || !path_matched)
                return;

            if (!IsDirectoryEmpty(directory))
                return;

            try
            {
                Log.DebugFormat($"Removing empty directory: {directory}");
                directory.Delete();
            }
            catch (Exception e)
            {
                // silently log the exception, but don't re-throw it
                // as to not annoy the user
                Log.Exception(e);
            }

            // check to see if the parent is empty
            DeleteEmptyDirectory(directory.Parent);
        }

        bool IsDirectoryEmpty(DirectoryInfo directory)
            => !directory.EnumerateFileSystemInfos().Any();

        public uint CreateVersion(string name, uint baseVersionId, bool create)
            => CreateVersion(name, null, baseVersionId, create, false);

        uint CreateVersion(string name, string extension, uint baseVersionId, bool create)
            => CreateVersion(name, extension, baseVersionId, create, false);

        uint CreateVersion(string name, string extension, uint baseVersionId, bool create, bool isProtected)
        {
            extension = extension ?? VersionUri(baseVersionId).GetExtension();
            var newBaseUri = DefaultVersion.BaseUri;
            string filename = GetFilenameForVersionName(name, extension);
            var originalUri = VersionUri(baseVersionId);
            var newUri = newBaseUri.Append(filename);
            string importMd5 = DefaultVersion.ImportMD5;

            if (VersionNameExists(name))
                throw new Exception("This version name already exists");

            if (create)
            {
                var destination = newUri.AbsolutePath;
                if (File.Exists(destination))
                    throw new Exception(string.Format("An object at this uri {0} already exists", newUri));

                File.Copy(originalUri.AbsolutePath, destination);
            }

            highestVersionId++;

            versions[highestVersionId] = new PhotoVersion(this, highestVersionId, newBaseUri, filename, importMd5, name, isProtected);

            changes.AddVersion(highestVersionId);

            return highestVersionId;
        }

        public uint CreateReparentedVersion(PhotoVersion version, bool isProtected = false)
        {
            // Try to derive version name from its filename
            string filename = Uri.UnescapeDataString(Path.GetFileNameWithoutExtension(version.Uri.AbsolutePath));
            string parentFilename = Path.GetFileNameWithoutExtension(Name);
            string name = null;
            if (filename.StartsWith(parentFilename))
                name = filename.Substring(parentFilename.Length).Replace("(", "").Replace(")", "").Replace("_", " ").Trim();

            if (string.IsNullOrEmpty(name))
            {
                string rep = name = Resources.Reparented;
                for (int num = 1; VersionNameExists(name); num++)
                {
                    name = $"rep ({num})";
                }
            }
            highestVersionId++;
            versions[highestVersionId] = new PhotoVersion(this, highestVersionId, version.BaseUri, version.Filename, version.ImportMD5, name, isProtected);

            changes.AddVersion(highestVersionId);

            return highestVersionId;
        }

        public uint CreateDefaultModifiedVersion(uint baseVersionId, bool createFile)
        {
            int num = 1;

            while (true)
            {
                string name = Catalog.GetPluralString(Strings.Modified, Strings.ModifiedNumber(num), num);
                string filename = GetFilenameForVersionName(name, Path.GetExtension(versions[baseVersionId].Filename));
                var filePath = DefaultVersion.BaseUri.Append(filename).AbsolutePath;

                if (!VersionNameExists(name) && !File.Exists(filePath))
                    return CreateVersion(name, baseVersionId, createFile);

                num++;
            }
        }

        public uint CreateNamedVersion(string name, string extension, uint baseVersionId, bool createFile)
        {
            int num = 1;

            string finalName;
            while (true)
            {
                finalName = (num == 1) ? Strings.ModifiedInName(num) : Strings.ModifiedInNameNumber(name, num);

                string filename = GetFilenameForVersionName(name, Path.GetExtension(versions[baseVersionId].Filename));
                var filePath = DefaultVersion.BaseUri.Append(filename).AbsolutePath;

                if (!VersionNameExists(finalName) && !File.Exists(filePath))
                    return CreateVersion(finalName, extension, baseVersionId, createFile);

                num++;
            }
        }

        public void RenameVersion(uint versionId, string new_name)
        {
            if (versionId == OriginalVersionId)
                throw new Exception("Cannot rename original version");

            if (VersionNameExists(new_name))
                throw new Exception("This name already exists");


            GetVersion(versionId).Name = new_name;
            changes.ChangeVersion(versionId);

            //TODO: rename file too ???

            //		if (System.IO.File.Exists (new_path))
            //			throw new Exception ("File with this name already exists");
            //
            //		File.Move (old_path, new_path);
            //		PhotoStore.MoveThumbnail (old_path, new_path);
        }

        public void CopyAttributesFrom(Photo that)
        {
            Time = that.Time;
            Description = that.Description;
            Rating = that.Rating;
            AddTag(that.Tags);
        }

        // This doesn't check if the tag is already there, use with caution.
        public void AddTagUnsafely(Tag tag)
        {
            tags.Add(tag);
            changes.AddTag(tag);
        }

        // This on the other hand does, but is O(n) with n being the number of existing tags.
        public void AddTag(Tag tag)
        {
            if (!tags.Contains(tag))
                AddTagUnsafely(tag);
        }

        public void AddTag(IEnumerable<Tag> taglist)
        {
            /*
			 * FIXME need a better naming convention here, perhaps just
			 * plain Add.
			 *
			 * tags.AddRange (taglist);
			 *     but, AddTag calls AddTagUnsafely which
			 *     adds and calls changes.AddTag on each tag?
			 *     Need to investigate that.
			 */
            foreach (var tag in taglist)
                AddTag(tag);
        }

        public void RemoveTag(Tag tag)
        {
            if (!tags.Contains(tag))
                return;

            tags.Remove(tag);
            changes.RemoveTag(tag);
        }

        public void RemoveTag(Tag[] taglist)
        {
            foreach (var tag in taglist)
                RemoveTag(tag);
        }

        public void RemoveCategory(IList<Tag> taglist)
        {
            foreach (var tag in taglist)
            {
                if (tag is Category cat)
                    RemoveCategory(cat.Children);

                RemoveTag(tag);
            }
        }

        // FIXME: This should be removed (I think)
        public bool HasTag(Tag tag)
        {
            return tags.Contains(tag);
        }

        static readonly IDictionary<Uri, string> md5Cache = new Dictionary<Uri, string>();

        public static void ResetMD5Cache()
        {
            md5Cache?.Clear();
        }

        public int CompareTo(Photo photo)
        {
            int result = Id.CompareTo(photo.Id);

            if (result == 0)
                return 0;

            return this.Compare(photo);
        }
    }
}
