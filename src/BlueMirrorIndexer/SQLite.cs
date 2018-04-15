/* 
 * MIT License. See license.txt for details.
 * 
 * Copyright © 2018 by github.com/fire-eggs.
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

// ReSharper disable InconsistentNaming

// TODO KBR investigate automapper?
// TODO KBR optional disc fields
// TODO KBR logical folders

namespace BlueMirrorIndexer
{
    public class SQLite
    {
        private const string DiscCreate = @"CREATE TABLE IF NOT EXISTS [Discs] (
[ID] INTEGER NOT NULL PRIMARY KEY,
[Format] TEXT NULL,
[Type] INTEGER,
[Free] INTEGER,
[Size] INTEGER,
[Label] TEXT,
[ScanTime] TEXT,
[Serial] TEXT,
[PhysicalLocation] TEXT,
[FromDrive] TEXT,
[Name] TEXT,
[Keywords] TEXT,
[Flags] INTEGER,
[Desc] TEXT,
[ClusterSize] INTEGER
)";
        private const string FileCreate = @"CREATE TABLE IF NOT EXISTS [Files] (
[ID] INTEGER NOT NULL PRIMARY KEY,
[Owner] INTEGER NOT NULL,
[Name] TEXT,
[Ext] TEXT,
[FullName] TEXT,
[Attributes] INTEGER,
[Length] INTEGER,
[CreateT] INTEGER,
[AccessT] INTEGER,
[WriteT] INTEGER,
[Keywords] TEXT,
[Desc] TEXT,
[Hash] TEXT
)";

        private const string FoldCreate = @"CREATE TABLE IF NOT EXISTS [Folds] (
[ID] INTEGER NOT NULL PRIMARY KEY,
[Owner] INTEGER NOT NULL,
[Name] TEXT,
[Ext] TEXT,
[FullName] TEXT,
[Attributes] INTEGER,
[CreateT] INTEGER,
[AccessT] INTEGER,
[WriteT] INTEGER
)";

        private const string LFoldCreate = @"CREATE TABLE IF NOT EXISTS [LFold] (
[ID] INTEGER NOT NULL PRIMARY KEY,
[Owner] INTEGER NOT NULL,
[Name] TEXT,
[Desc] TEXT,
[Type] INTEGER
)";

        private const string FileDex = @"CREATE INDEX owner_dex1 ON Files(Owner)";
        private const string FoldDex = @"CREATE INDEX owner_dex2 ON Folds(Owner)";

        public static void WriteToDb(VolumeDatabase mem)
        {
            // TODO KBR allow user to name file, location
            SQLite sdb = new SQLite();
            sdb.logit("SQLite-DBWrite", true);

            string dbName = "Indexer.db";
            string dbFile = AppDomain.CurrentDomain.BaseDirectory + dbName;
            string dbSrc = string.Format("Data Source={0};Version=3;", dbName);

            if (File.Exists(dbFile))
            {
                File.Delete(dbFile);
                SQLiteConnection.CreateFile(dbFile);
            }

            using (var conn = new SQLiteConnection(dbSrc))
            {
                conn.Open();
                CreateTables(conn);
                WriteData(conn, mem);
                CreateIndices(conn);
            }
            sdb.logit("SQLite-DBWrite");
        }

        private static void CreateTables(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand(conn))
            {
                // discs
                cmd.CommandText = DiscCreate;
                cmd.ExecuteNonQuery();

                // folders
                cmd.CommandText = FoldCreate;
                cmd.ExecuteNonQuery();

                // files
                cmd.CommandText = FileCreate;
                cmd.ExecuteNonQuery();

                // Logical Folders
                cmd.CommandText = LFoldCreate;
                cmd.ExecuteNonQuery();

                // Logical Folder <> item mappings
            }
        }

        private static void CreateIndices(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = FileDex;
                cmd.ExecuteNonQuery();
                cmd.CommandText = FoldDex;
                cmd.ExecuteNonQuery();
            }
        }

        #region Write data
        private static void WriteData(SQLiteConnection conn, VolumeDatabase mem)
        {
            foreach (var disc in mem.GetDiscs())
            {
                using (var tx = conn.BeginTransaction())
                {
                    WriteDisc(conn, disc);
                    tx.Commit();
                }
            }

            using (var tx = conn.BeginTransaction())
            {
                foreach (var lFold in mem.GetLogicalFolders())
                {
                    WriteLFold(conn, lFold);
                }
                tx.Commit();
            }
        }

        private static void WriteDisc(SQLiteConnection conn, DiscInDatabase disc)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "insert into Discs (Format, Type, Free, Size, Label, "+
                                  "ScanTime, Serial, PhysicalLocation, FromDrive, Name, "+
                                  "Keywords, Flags, Desc, ClusterSize) values (";
                cmd.CommandText += "'" + disc.DriveFormat + "',";
                cmd.CommandText += "'" + (int)disc.DriveType + "',";
                cmd.CommandText += "'" + disc.TotalFreeSpace + "',";
                cmd.CommandText += "'" + disc.TotalSize + "',";
                cmd.CommandText += "'" + disc.VolumeLabel.Replace("'", "''") + "',";
                cmd.CommandText += "'" + disc.Scanned.ToUniversalTime() + "',";
                cmd.CommandText += "'" + disc.SerialNumber + "',";
                cmd.CommandText += "'" + disc.PhysicalLocation.Replace("'", "''") + "',";
                cmd.CommandText += "'" + disc.FromDrive + "',";
                cmd.CommandText += "'" + disc.Name + "',";
                cmd.CommandText += "'" + disc.Keywords.Replace("'", "''") + "',";
                cmd.CommandText += "'" + disc.Flags + "',";
                cmd.CommandText += "'" + disc.Description.Replace("'", "''") + "',";
                cmd.CommandText += "'" + disc.ClusterSize + "'";
                cmd.CommandText += ")";

                cmd.ExecuteNonQuery();

                // pass disc id for folders
                cmd.CommandText = "select last_insert_rowid()";
                Int64 lastRowId64 = (Int64) cmd.ExecuteScalar();
                int lastRowId = (int) lastRowId64;

                WriteFiles(conn, -lastRowId, ((IFolder) disc).Files);
                WriteFolders(conn, -lastRowId, ((IFolder)disc).Folders);
            }
        }

        private const int COMPRESSED_FLAG = 0x1000000;

        private static void WriteFolders(SQLiteConnection conn, int owner, IFolder[] folders)
        {
            string start = "insert into Folds (Owner, Name, Ext, FullName, Attributes, CreateT, AccessT, WriteT) VALUES ('" + owner + "',";

            using (var cmd = conn.CreateCommand())
            {

                foreach (var afold in folders)
                {
                    var afile = afold as ItemInDatabase; // FolderInDatabase;
                    if (afile == null)
                        continue;

                    cmd.CommandText = start;
                    cmd.CommandText += "'" + afile.Name.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + afile.Extension.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + afile.FullName.Replace("'", "''") + "',";

                    // A Q&D hack: mark a 'compressed file' folder with a special Attribute value. Must be removed on read!!!
                    int attribs = (int) afile.Attributes;
                    if (afold is CompressedFile)
                        attribs |= COMPRESSED_FLAG;

                    cmd.CommandText += "'" + attribs + "',";
                    cmd.CommandText += "'" + afile.CreationTime.ToUniversalTime().Ticks + "',";
                    cmd.CommandText += "'" + afile.LastAccessTime.ToUniversalTime().Ticks + "',";
                    cmd.CommandText += "'" + afile.LastWriteTime.ToUniversalTime().Ticks + "')";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "select last_insert_rowid()";
                    Int64 lastRowId64 = (Int64)cmd.ExecuteScalar();
                    int lastRowId = (int)lastRowId64;

                    WriteFiles(conn, lastRowId, afold.Files);
                    WriteFolders(conn, lastRowId, afold.Folders);
                }

            }
        }

        private static SQLiteCommand _writeFileCmd;

        private static void WriteFiles(SQLiteConnection conn, int owner, FileInDatabase[] files)
        {
            if (_writeFileCmd == null)
            {
                _writeFileCmd = new SQLiteCommand(conn);
                string sql = "insert into Files (Owner, Name, Ext, FullName, Attributes, Length, CreateT, AccessT, WriteT," +
                               "Keywords, Desc, Hash) VALUES (@own,@name,@ext,@fname,@attrib,@len,@ctime, @atime, @wtime, @keyw,@desc,@hash)";
                _writeFileCmd.CommandText = sql;
            }

            foreach (var afile in files)
            {
                _writeFileCmd.Parameters.Clear();

                _writeFileCmd.Parameters.AddWithValue("@own", owner);
                _writeFileCmd.Parameters.AddWithValue("@name", afile.Name);
                _writeFileCmd.Parameters.AddWithValue("@ext", afile.Extension);
                _writeFileCmd.Parameters.AddWithValue("@fname", afile.FullName);
                _writeFileCmd.Parameters.AddWithValue("@attrib", (int) afile.Attributes);
                _writeFileCmd.Parameters.AddWithValue("@len", afile.Length);
                _writeFileCmd.Parameters.AddWithValue("@ctime", afile.CreationTime.ToUniversalTime().Ticks);
                _writeFileCmd.Parameters.AddWithValue("@atime", afile.LastAccessTime.ToUniversalTime().Ticks);
                _writeFileCmd.Parameters.AddWithValue("@wtime", afile.LastWriteTime.ToUniversalTime().Ticks);
                _writeFileCmd.Parameters.AddWithValue("@keyw", afile.Keywords);
                _writeFileCmd.Parameters.AddWithValue("@desc", afile.Description);
                _writeFileCmd.Parameters.AddWithValue("@hash", afile.Hash.ToString());
                _writeFileCmd.ExecuteNonQuery();
            }
        }

        private static void WriteLFold(SQLiteConnection conn, LogicalFolder lfold, int parent=0)
        {
            using (var cmd = conn.CreateCommand())
            {
                /*
            [ID] INTEGER NOT NULL PRIMARY KEY,
            [Owner] INTEGER NOT NULL,
            [Name] TEXT,
            [Desc] TEXT,
            [Type] INTEGER
             */
                cmd.CommandText = "INSERT INTO [LFold] (Owner, Name, Desc, Type) VALUES (";
                cmd.CommandText += "'" + parent + "',";
                cmd.CommandText += "'" + lfold.Name.Replace("'", "''") + "',";
                cmd.CommandText += "'" + lfold.Description.Replace("'", "''") + "',";
                cmd.CommandText += "'" + (int) lfold.FolderType + "')";

                cmd.ExecuteNonQuery();

                // pass disc id for folders
                cmd.CommandText = "select last_insert_rowid()";
                Int64 lastRowId64 = (Int64)cmd.ExecuteScalar();
                int lastRowId = (int)lastRowId64;

                foreach (var subFold in lfold.GetSubFolders())
                {
                    WriteLFold(conn, subFold, lastRowId);
                }
            }
        }

#endregion

        #region Data Read
        public static VolumeDatabase ReadFromDb(string dbpath)
        {
            // TODO KBR allow user to name file, location

            string dbName = "Indexer.db";
            string dbFile = AppDomain.CurrentDomain.BaseDirectory + dbName;
            string dbSrc = string.Format("Data Source={0};Version=3;", dbName);

            if (!File.Exists(dbFile))
                return null;

            VolumeDatabase mem;
            using (var conn = new SQLiteConnection(dbSrc))
            {
                conn.Open();
                mem = ReadData(conn);

                // Cleanup
                if (_readFoldCmd != null)
                    _readFoldCmd.Dispose();
                _foldHash = null;

                conn.Close();
            }
            // TODO KBR exception, e.g. not a valid db file

            SQLiteConnection.ClearAllPools();
            return mem;
        }

        private static VolumeDatabase ReadData(SQLiteConnection conn)
        {
            // Track all folders by db-id for fast lookup
            _foldHash = new Hashtable();

            VolumeDatabase mem = new VolumeDatabase();

            string txt = "select * from Discs";
            using (SQLiteCommand cmd = new SQLiteCommand(txt, conn))
            {
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var dbid = (long) rdr["ID"]; // The top folder has this as the owner id

                        DiscInDatabase did = new DiscInDatabase(-(int) dbid);

                        did.DriveFormat = rdr["Format"] as string;
                        did.DriveType = (DriveType) ((long) rdr["Type"]);
                        did.TotalFreeSpace = (long) rdr["Free"];
                        did.TotalSize = (long) rdr["Size"];
                        did.Scanned = DateTime.Parse((string) rdr["ScanTime"]);
                        did.SerialNumber = rdr["Serial"] as string;
                        did.PhysicalLocation = rdr["PhysicalLocation"] as string;
                        did.FromDrive = rdr["FromDrive"] as string;
                        did.Name = rdr.GetString(10);
                        did.Keywords = rdr.GetString(11);
                        did.Flags = rdr.GetInt32(12);
                        did.Description = rdr.GetString(13);
                        did.ClusterSize = (uint)rdr.GetInt32(14);

                        mem.AddDisc(did);
                        _foldHash.Add(-(int)dbid, did);
                    }
                }
            }

            foreach (var discInDatabase in mem.GetDiscs())
            {
                ReadFolders(conn, discInDatabase);
            }

            ReadAllFiles(conn, mem);

            ReadLogicalFolders(conn, mem);

            return mem;
        }

        private static Hashtable _foldHash;
        private static SQLiteCommand _readFoldCmd;

        private static void ReadFolders(SQLiteConnection conn, IFolder did)
        {
            if (_readFoldCmd == null)
            {
                _readFoldCmd = new SQLiteCommand(conn);
                _readFoldCmd.CommandText = "select * from [Folds] WHERE Owner = @own";
            }

            _readFoldCmd.Parameters.Clear();
            _readFoldCmd.Parameters.AddWithValue("@own", (did as ItemInDatabase).DbId);

            using (SQLiteDataReader rdr = _readFoldCmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    /*
                    [ID] INTEGER NOT NULL PRIMARY KEY,
                    [Owner] INTEGER NOT NULL,
                    [Name] TEXT,
                    [Ext] TEXT,
                    [FullName] TEXT,
                    [Attributes] INTEGER,
                    [CreateT] INTEGER,
                    [AccessT] INTEGER,
                    [WriteT] INTEGER
                    */
                    ItemInDatabase afile;
                    int attrib = rdr.GetInt32(5);
                    long dbid = rdr.GetInt64(0);

                    // Check the magic flag for a compressed file, remove it below
                    if ((attrib & COMPRESSED_FLAG) != 0)
                        afile = new CompressedFile((int) dbid, did);
                    else
                        afile = new FolderInDatabase((int) dbid, did);
                    afile.Name = rdr.GetString(2);
                    afile.Extension = rdr.GetString(3);
                    afile.FullName = rdr.GetString(4);
                    afile.Attributes = (FileAttributes) (attrib & ~COMPRESSED_FLAG);

                    afile.CreationTime = new DateTime(rdr.GetInt64(6));
                    afile.LastAccessTime = new DateTime(rdr.GetInt64(7));
                    afile.LastWriteTime = new DateTime(rdr.GetInt64(8));

                    ((IFolder) did).AddToFolders(afile as IFolder);
                    _foldHash.Add((int)dbid, afile);
                }
            }

            foreach (var afold in ((IFolder) did).Folders)
            {
                ReadFolders(conn, afold);
            }
        }

        private static void ReadAllFiles(SQLiteConnection conn, VolumeDatabase mem)
        {
            // All folders have been read.
            // Read ALL the file rows, and push them into the correct folder
            // NOTE: this is *only* faster if folder-by-id lookup is fast enough, which the HashTable gives us

            string txt = "SELECT * FROM [FILES]";
            using (SQLiteCommand cmd = new SQLiteCommand(txt, conn))
            {
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        int ownerId;
                        var afile = FileFromRow(rdr, out ownerId);

                        IFolder fid = _foldHash[ownerId] as IFolder;
                        fid.AddToFiles(afile);
                    }
                }
            }
        }

        private static FileInDatabase FileFromRow(SQLiteDataReader rdr, out int ownerId)
        {
            /*
    [ID] INTEGER NOT NULL PRIMARY KEY,
    [Owner] INTEGER NOT NULL,
    [Name] TEXT,
    [Ext] TEXT,
    [FullName] TEXT,
    [Attributes] INTEGER, // 5
    [Length] INTEGER,
    [CreateT] TEXT,
    [AccessT] TEXT,
    [WriteT] TEXT,
    [Keywords] TEXT, // 10
    [Desc] TEXT,
    [Hash]
    */
            FileInDatabase afile = new FileInDatabase();
            afile.DbId = rdr.GetInt32(0);
            ownerId = rdr.GetInt32(1);
            afile.Name = rdr.GetString(2);
            afile.Extension = rdr.GetString(3);
            afile.FullName = rdr.GetString(4);
            afile.Attributes = (FileAttributes) rdr.GetInt64(5);
            afile.Length = rdr.GetInt64(6);

            afile.CreationTime = new DateTime(rdr.GetInt64(7));
            afile.LastAccessTime = new DateTime(rdr.GetInt64(8));
            afile.LastWriteTime = new DateTime(rdr.GetInt64(9));

            afile.Keywords = rdr.GetString(10);
            object tmp2 = rdr[11];
            if (tmp2 is DBNull)
            {
                // TODO KBR file inside a compressed file
            }
            else
                afile.Description = (string) tmp2; //rdr.GetString(11);

            string tmp = rdr.GetString(12); // SQLite doesn't support unsigned
            afile.Hash = UInt64.Parse(tmp);
            return afile;
        }

        private static void ReadLogicalFolders(SQLiteConnection conn, VolumeDatabase mem)
        {
            var lFoldList = mem.GetLogicalFolders();

            // Note: order by owner insures that sub-folders are after their parents and hookup works
            string txt = "select * from LFold ORDER BY Owner";
            using (SQLiteCommand cmd = new SQLiteCommand(txt, conn))
            {

                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
/*
[ID] INTEGER NOT NULL PRIMARY KEY,
[Owner] INTEGER NOT NULL,
[Name] TEXT,
[Desc] TEXT,
[Type] INTEGER
*/
                        LogicalFolder lfold = new LogicalFolder();
                        lfold.DbId = rdr.GetInt32(0);
                        lfold.Name = rdr.GetString(2);
                        lfold.Description = rdr.GetString(3);
                        lfold.FolderType = (LogicalFolderType) rdr.GetInt32(4);

                        int owner = rdr.GetInt32(1);
                        if (owner != 0)
                            HookupParent(lfold, rdr.GetInt32(1), lFoldList);
                        else
                            lFoldList.Add(lfold);
                    }
                }
            }
        }

        private static void HookupParent(LogicalFolder child, int ownerId, List<LogicalFolder> foldlist)
        {
            // Connect a sub-folder to its parent
            var parent = foldlist.FirstOrDefault(f => f.DbId == ownerId);
            if (parent == null)
                return; // shouldn't happen!
            parent.AddFolder(child);
        }
        #endregion

        private int tick;
        private void logit(string msg, bool first = false)
        {
            int delta = 0;
            if (first)
                tick = Environment.TickCount;
            else
                delta = Environment.TickCount - tick;
            using (var f = File.Open("octopus.log", System.IO.FileMode.Append))
            using (var sw = new StreamWriter(f))
            {
                sw.WriteLine("{0}|{1}", msg, delta);
            }
        }

    }

}
