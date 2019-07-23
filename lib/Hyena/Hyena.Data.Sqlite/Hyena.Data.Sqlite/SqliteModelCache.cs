//
// SqliteModelCache.cs
//
// Authors:
//   Gabriel Burt <gburt@novell.com>
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2007-2008 Novell, Inc.
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

using System;
using System.Collections.Generic;
using System.Text;

using Hyena.Query;

namespace Hyena.Data.Sqlite
{
    public class SqliteModelCache<T> : DictionaryModelCache<T> where T : ICacheableItem, new ()
    {
		readonly HyenaSqliteConnection connection;
		readonly ICacheableDatabaseModel model;
		readonly SqliteModelProvider<T> provider;
		readonly HyenaSqliteCommand select_range_command;
		readonly HyenaSqliteCommand select_single_command;
		readonly HyenaSqliteCommand select_first_command;
		readonly HyenaSqliteCommand count_command;
		readonly HyenaSqliteCommand delete_selection_command;
		readonly HyenaSqliteCommand save_selection_command;
		readonly HyenaSqliteCommand get_selection_command;
        HyenaSqliteCommand indexof_command;
		readonly string select_str;
		readonly string reload_sql;
        string last_indexof_where_fragment;
		readonly long selection_uid;

		// private bool warm;
		long first_order_id;

        public event Action<IDataReader> AggregatesUpdated;

        public SqliteModelCache (HyenaSqliteConnection connection,
                           string uuid,
                           ICacheableDatabaseModel model,
                           SqliteModelProvider<T> provider)
            : base (model)
        {
            this.connection = connection;
            this.model = model;
            this.provider = provider;

            CheckCacheTable ();

            if (model.SelectAggregates != null) {
                if (model.CachesJoinTableEntries) {
                    count_command = new HyenaSqliteCommand (string.Format (@"
                        SELECT count(*), {0} FROM {1}{2} j
                        WHERE j.{4} IN (SELECT ItemID FROM {3} WHERE ModelID = ?)
                        AND {5} = j.{6}",
                        model.SelectAggregates, // eg sum(FileSize), sum(Duration)
                        provider.TableName,     // eg CoreTracks
                        model.JoinFragment,     // eg , CorePlaylistEntries
                        CacheTableName,         // eg CoreCache
                        model.JoinPrimaryKey,   // eg EntryID
                        provider.PrimaryKey,    // eg CoreTracks.TrackID
                        model.JoinColumn        // eg TrackID
                    ));
                } else {
                    count_command = new HyenaSqliteCommand (string.Format (
                        "SELECT count(*), {0} FROM {1} WHERE {2} IN (SELECT ItemID FROM {3} WHERE ModelID = ?)",
                        model.SelectAggregates, provider.TableName, provider.PrimaryKey, CacheTableName
                    ));
                }
            } else {
                count_command = new HyenaSqliteCommand (string.Format (
                    "SELECT COUNT(*) FROM {0} WHERE ModelID = ?", CacheTableName
                ));
            }

            CacheId = FindOrCreateCacheModelId (string.Format ("{0}-{1}", uuid, typeof(T).Name));

            if (model.Selection != null) {
                selection_uid = FindOrCreateCacheModelId (string.Format ("{0}-{1}-Selection", uuid, typeof(T).Name));
            }

            if (model.CachesJoinTableEntries) {
                select_str = string.Format (
                    @"SELECT {0} {10}, OrderID, {5}.ItemID  FROM {1}
                        INNER JOIN {2}
                            ON {3} = {2}.{4}
                        INNER JOIN {5}
                            ON {2}.{6} = {5}.ItemID {11}
                        WHERE
                            {5}.ModelID = {7} {8} {9}",
                    provider.Select, provider.From,
                    model.JoinTable, provider.PrimaryKey, model.JoinColumn,
                    CacheTableName, model.JoinPrimaryKey, CacheId,
					string.IsNullOrEmpty (provider.Where) ? null : "AND",
                    provider.Where, "{0}", "{1}"
                );

                reload_sql = string.Format (@"
                    DELETE FROM {0} WHERE ModelID = {1};
                        INSERT INTO {0} (ModelID, ItemID) SELECT {1}, {2} ",
                    CacheTableName, CacheId, model.JoinPrimaryKey
                );
            } else if (model.CachesValues) {
                // The duplication of OrderID/ItemID in the select below is intentional!
                // The first is used construct the QueryFilterInfo value, the last internally
                // to this class
                select_str = string.Format (
                    @"SELECT OrderID, ItemID {2}, OrderID, ItemID FROM {0} {3} WHERE {0}.ModelID = {1}",
                    CacheTableName, CacheId, "{0}", "{1}"
                );

                reload_sql = string.Format (@"
                    DELETE FROM {0} WHERE ModelID = {1};
                    INSERT INTO {0} (ModelID, ItemID) SELECT DISTINCT {1}, {2} ",
                    CacheTableName, CacheId, provider.Select
                );
            } else {
                select_str = string.Format (
                    @"SELECT {0} {7}, OrderID, {2}.ItemID FROM {1}
                        INNER JOIN {2}
                            ON {3} = {2}.ItemID {8}
                        WHERE
                            {2}.ModelID = {4} {5} {6}",
                    provider.Select, provider.From, CacheTableName,
                    provider.PrimaryKey, CacheId,
					string.IsNullOrEmpty (provider.Where) ? string.Empty : "AND",
                    provider.Where, "{0}", "{1}"
                );

                reload_sql = string.Format (@"
                    DELETE FROM {0} WHERE ModelID = {1};
                        INSERT INTO {0} (ModelID, ItemID) SELECT {1}, {2} ",
                    CacheTableName, CacheId, provider.PrimaryKey
                );
            }

            select_single_command = new HyenaSqliteCommand (
				string.Format (@"
                    SELECT OrderID FROM {0}
                        WHERE
                            ModelID = {1} AND
                            ItemID = ?",
                    CacheTableName, CacheId
                )
            );

            select_range_command = new HyenaSqliteCommand (
				string.Format (string.Format ("{0} {1}", select_str, "LIMIT ?, ?"), null, null)
            );

            select_first_command = new HyenaSqliteCommand (
				string.Format (
                    "SELECT OrderID FROM {0} WHERE ModelID = {1} LIMIT 1",
                    CacheTableName, CacheId
                )
            );

            if (model.Selection != null) {
                delete_selection_command = new HyenaSqliteCommand (string.Format (
                    "DELETE FROM {0} WHERE ModelID = {1}", CacheTableName, selection_uid
                ));

                save_selection_command = new HyenaSqliteCommand (string.Format (
                    "INSERT INTO {0} (ModelID, ItemID) SELECT {1}, ItemID FROM {0} WHERE ModelID = {2} LIMIT ?, ?",
                    CacheTableName, selection_uid, CacheId
                ));

                get_selection_command = new HyenaSqliteCommand (string.Format (
                    "SELECT OrderID FROM {0} WHERE ModelID = {1} AND ItemID IN (SELECT ItemID FROM {0} WHERE ModelID = {2})",
                    CacheTableName, CacheId, selection_uid
                ));
            }
        }

        public bool HasSelectAllItem { get; set; } = false;

		// The idea behind this was we could preserve the CoreCache table across restarts of Banshee,
		// and indicate whether the cache was already primed via this property.  It's not used, and may never be.
		public bool Warm {
            //get { return warm; }
            get { return false; }
        }

        public long Count { get; private set; }

        public long CacheId { get; }

        protected virtual string CacheModelsTableName {
            get { return "HyenaCacheModels"; }
        }

        protected virtual string CacheTableName {
            get { return "HyenaCache"; }
        }

        long FirstOrderId {
            get {
                lock (this) {
                    if (first_order_id == -1) {
                        first_order_id = connection.Query<long> (select_first_command);
                    }
                    return first_order_id;
                }
             }
        }

        public long IndexOf (string where_fragment, long offset)
        {
            if (string.IsNullOrEmpty (where_fragment)) {
                return -1;
            }

            if (!where_fragment.Equals (last_indexof_where_fragment)) {
                last_indexof_where_fragment = where_fragment;

                if (!where_fragment.Trim ().ToLower ().StartsWith ("and ")) {
                    where_fragment = " AND " + where_fragment;
                }

                string sql = string.Format ("{0} {1} LIMIT ?, 1", select_str, where_fragment);
                indexof_command = new HyenaSqliteCommand (string.Format (sql, null, null));
            }

            lock (this) {
                using (var reader = connection.Query (indexof_command, offset)) {
                    if (reader.Read ()) {
                        long target_id = (long) reader[reader.FieldCount - 2];
                        if (target_id == 0) {
                            return -1;
                        }
                        return target_id - FirstOrderId;
                    }
                }

                return -1;
            }
        }

        public long IndexOf (ICacheableItem item)
        {
            if (item == null || item.CacheModelId != CacheId) {
                return -1;
            }

            return IndexOf (item.CacheEntryId);
        }

        public long IndexOf (object item_id)
        {
            lock (this) {
                if (Count == 0) {
                    return -1;
                }

                long target_id = connection.Query<long> (select_single_command, item_id);
                if (target_id == 0) {
                    return -1;
                }
                return target_id - FirstOrderId;
            }
        }

        public T GetSingleWhere (string conditionOrderFragment, params object [] args)
        {
            return GetSingle (null, null, conditionOrderFragment, args);
        }

        HyenaSqliteCommand get_single_command;
        string last_get_single_select_fragment, last_get_single_condition_fragment, last_get_single_from_fragment;
        public T GetSingle (string selectFragment, string fromFragment, string conditionOrderFragment, params object [] args)
        {
            if (selectFragment != last_get_single_select_fragment
                || conditionOrderFragment != last_get_single_condition_fragment
                ||fromFragment != last_get_single_from_fragment
                || get_single_command == null)
            {
                last_get_single_select_fragment = selectFragment;
                last_get_single_condition_fragment = conditionOrderFragment;
                last_get_single_from_fragment = fromFragment;
                get_single_command = new HyenaSqliteCommand (string.Format (string.Format (
                        "{0} {1} {2}", select_str, conditionOrderFragment, "LIMIT 1"), selectFragment, fromFragment
                ));
            }

            using (var reader = connection.Query (get_single_command, args)) {
                if (reader.Read ()) {
					var item = provider.Load (reader);
                    item.CacheEntryId = reader[reader.FieldCount - 1];
                    item.CacheModelId = CacheId;
                    return item;
                }
            }
            return default(T);
        }

        HyenaSqliteCommand last_reload_command;
        string last_reload_fragment;

        public override void Reload ()
        {
            lock (this) {
                if (last_reload_fragment != model.ReloadFragment || last_reload_command == null) {
                    last_reload_fragment = model.ReloadFragment;
                    last_reload_command = new HyenaSqliteCommand (string.Format ("{0}{1}", reload_sql, last_reload_fragment));
                }

                Clear ();
                //Log.DebugFormat ("Reloading {0} with {1}", model, last_reload_command.Text);
                connection.Execute (last_reload_command);
            }
        }

        public override void Clear ()
        {
            lock (this) {
                base.Clear ();
                first_order_id = -1;
            }
        }

        bool saved_selection = false;
        ICacheableItem saved_focus_item = null;
        public void SaveSelection ()
        {
            if (model.Selection != null && model.Selection.Count > 0 &&
                !(HasSelectAllItem && model.Selection.AllSelected))
            {
                connection.Execute (delete_selection_command);
                saved_selection = true;

                if (!HasSelectAllItem && model.Selection.FocusedIndex != -1) {
					var item = GetValue (model.Selection.FocusedIndex);
                    if (item != null) {
                        saved_focus_item = GetValue (model.Selection.FocusedIndex);
                    }
                }

                long start, end;
                foreach (var range in model.Selection.Ranges) {
                    start = range.Start;
                    end = range.End;

                    // Compensate for the first, fake 'All *' item
                    if (HasSelectAllItem) {
                        start -= 1;
                        end -= 1;
                    }

                    connection.Execute (save_selection_command, start, end - start + 1);
                }
            } else {
                saved_selection = false;
            }
        }

        public void RestoreSelection ()
        {
            if (saved_selection) {
				long first_id = FirstOrderId;

                // Compensate for the first, fake 'All *' item
                if (HasSelectAllItem) {
                    first_id -= 1;
                }

                model.Selection.Clear (false);

                foreach (long order_id in connection.QueryEnumerable<long> (get_selection_command)) {
					var selected_id = order_id - first_id;
					model.Selection.QuietSelect ((int)selected_id);
                }

                if (HasSelectAllItem && model.Selection.Count == 0) {
                    model.Selection.QuietSelect (0);
                } if (saved_focus_item != null) {
                    long i = IndexOf (saved_focus_item);
                    if (i != -1) {
                        // TODO get rid of int cast
                        model.Selection.FocusedIndex = (int)i;
                    }
                }
                saved_selection = false;
                saved_focus_item = null;
            }
        }

        protected override void FetchSet (long offset, long limit)
        {
            lock (this) {
                using (var reader = connection.Query (select_range_command, offset, limit)) {
                    T item;
                    while (reader.Read ()) {
                        if (!ContainsKey (offset)) {
                            item = provider.Load (reader);
                            item.CacheEntryId = reader[reader.FieldCount - 1];
                            item.CacheModelId = CacheId;
                            Add (offset, item);
                        }
                        offset++;
                    }
                }
            }
        }

        public void UpdateAggregates ()
        {
            Count = UpdateAggregates (AggregatesUpdated, CacheId);
        }

        public void UpdateSelectionAggregates (Action<IDataReader> handler)
        {
            SaveSelection ();
            UpdateAggregates (handler, selection_uid);
        }

        long UpdateAggregates (Action<IDataReader> handler, long model_id)
        {
            long aggregate_rows = 0;
            using (var reader = connection.Query (count_command, model_id)) {
                if (reader.Read ()) {
                    aggregate_rows = Convert.ToInt64 (reader[0]);

					handler?.Invoke (reader);
				}
            }

            return aggregate_rows;
        }


        long FindOrCreateCacheModelId (string id)
        {
            long model_id = connection.Query<long> (string.Format (
                "SELECT CacheID FROM {0} WHERE ModelID = '{1}'",
                CacheModelsTableName, id
            ));

            if (model_id == 0) {
                //Console.WriteLine ("Didn't find existing cache for {0}, creating", id);
                model_id = connection.Execute (new HyenaSqliteCommand (string.Format (
                    "INSERT INTO {0} (ModelID) VALUES (?)", CacheModelsTableName
                    ), id
                ));
            } else {
                //Console.WriteLine ("Found existing cache for {0}: {1}", id, uid);
                //warm = true;
                //Clear ();
                //UpdateAggregates ();
            }

            return model_id;
        }

        static string checked_cache_table;
        void CheckCacheTable ()
        {
            if (CacheTableName == checked_cache_table) {
                return;
            }

            if (!connection.TableExists (CacheTableName)) {
                connection.Execute (string.Format (@"
                    CREATE TEMP TABLE {0} (
                        OrderID INTEGER PRIMARY KEY,
                        ModelID INTEGER,
                        ItemID TEXT)", CacheTableName
                ));
            }

            if (!connection.TableExists (CacheModelsTableName)) {
                connection.Execute (string.Format (
                    "CREATE TABLE {0} (CacheID INTEGER PRIMARY KEY, ModelID TEXT UNIQUE)",
                    CacheModelsTableName
                ));
            }

            checked_cache_table = CacheTableName;
        }
    }
}
