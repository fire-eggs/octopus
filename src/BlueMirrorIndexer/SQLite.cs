﻿/* 
 * Copyright © 2018 by Kevin Routley.
 * 
 */
using System;
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
[Desc] TEXT
)
";
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
[Desc] TEXT
)";

        private const string FoldCreate = @"CREATE TABLE IF NOT EXISTS [Folds] (
[ID] INTEGER NOT NULL PRIMARY KEY,
[Owner] INTEGER NOT NULL,
[Name] TEXT,
[Ext] TEXT,
[FullName] TEXT,
[Attributes] INTEGER,
[CreateT] TEXT,
[AccessT] TEXT,
[WriteT] TEXT
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
                                  "Keywords, Flags, Desc) values (";
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
                cmd.CommandText += "'" + disc.Description.Replace("'", "''") + "'";
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

        private static void WriteFolders(SQLiteConnection conn, int owner, IFolder[] folders)
        {
            string start = "insert into Folds (Owner, Name, Ext, FullName, Attributes, CreateT, AccessT, WriteT) VALUES ('" + owner + "',";

            using (var cmd = conn.CreateCommand())
            {

                foreach (var afold in folders)
                {
                    FolderInDatabase afile = afold as FolderInDatabase;
                    if (afile == null)
                        continue;

                    cmd.CommandText = start;
                    cmd.CommandText += "'" + afile.Name.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + afile.Extension.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + afile.FullName.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + (int)afile.Attributes + "',";
                    cmd.CommandText += "'" + afile.CreationTime.ToUniversalTime() + "',";
                    cmd.CommandText += "'" + afile.LastAccessTime.ToUniversalTime() + "',";
                    cmd.CommandText += "'" + afile.LastWriteTime.ToUniversalTime() + "')";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "select last_insert_rowid()";
                    Int64 lastRowId64 = (Int64)cmd.ExecuteScalar();
                    int lastRowId = (int)lastRowId64;

                    WriteFiles(conn, lastRowId, afold.Files);
                    WriteFolders(conn, lastRowId, afold.Folders);
                }

            }
        }

        private static void WriteFiles(SQLiteConnection conn, int owner, FileInDatabase[] files)
        {
            string start = "insert into Files (Owner, Name, Ext, FullName, Attributes, Length, CreateT, AccessT, WriteT," + 
                           "Keywords, Desc) VALUES ('"+owner+"',";
            using (var cmd = conn.CreateCommand())
            {
                foreach (var afile in files)
                {
                    cmd.CommandText = start;
                    cmd.CommandText += "'" + afile.Name.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + afile.Extension.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + afile.FullName.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + (int)afile.Attributes + "',";
                    cmd.CommandText += "'" + afile.Length + "',";
                    cmd.CommandText += "'" + afile.CreationTime.ToUniversalTime().Ticks + "',";
                    cmd.CommandText += "'" + afile.LastAccessTime.ToUniversalTime().Ticks + "',";
                    cmd.CommandText += "'" + afile.LastWriteTime.ToUniversalTime().Ticks + "',";
                    cmd.CommandText += "'" + afile.Keywords.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + afile.Description.Replace("'", "''") + "'";
                    cmd.CommandText += ")";
                    cmd.ExecuteNonQuery();
                }
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

                _readFoldCmd.Dispose();
                _readFileCmd.Dispose();

                conn.Close();
            }
            // TODO KBR exception, e.g. not a valid db file

            SQLiteConnection.ClearAllPools();
            return mem;
        }

        private static VolumeDatabase ReadData(SQLiteConnection conn)
        {
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

                        mem.AddDisc(did);
                    }
                }
            }

            foreach (var discInDatabase in mem.GetDiscs())
            {
                ReadFiles(conn, discInDatabase);
                ReadFolders(conn, discInDatabase);
            }

            ReadLogicalFolders(conn, mem);

            return mem;
        }

        private static SQLiteCommand _readFoldCmd;

        private static void ReadFolders(SQLiteConnection conn, FolderInDatabase did)
        {
            if (_readFoldCmd == null)
            {
                _readFoldCmd = new SQLiteCommand(conn);
                _readFoldCmd.CommandText = "select * from [Folds] WHERE Owner = @own";
            }

            _readFoldCmd.Parameters.Clear();
            _readFoldCmd.Parameters.AddWithValue("@own", did.DbId);

//            string txt = "select * from Folds where Owner = " + did.DbId;
//            using (SQLiteCommand cmd = new SQLiteCommand(txt, conn))
//            {
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
                        [CreateT] TEXT,
                        [AccessT] TEXT,
                        [WriteT] TEXT
                        */
                        //"insert into Folds (Owner, Name, Ext, FullName, Attributes, Length, CreateT, AccessT, WriteT) VALUES ('" + owner + "',";
                        long dbid = rdr.GetInt64(0);
                        FolderInDatabase afile = new FolderInDatabase((int) dbid, did);
                        afile.Name = rdr.GetString(2);
                        afile.Extension = rdr.GetString(3);
                        afile.FullName = rdr.GetString(4);
                        afile.Attributes = (FileAttributes)rdr.GetInt64(5);
                        //afile.FullName = rdr["FullName"] as string;
                        //afile.Extension = rdr["Ext"] as string;
                        //afile.Name = rdr["Name"] as string;
                        //afile.Attributes = (FileAttributes) ((long) rdr["Attributes"]);

                        afile.CreationTime = DateTime.Parse(rdr.GetString(6));
                        afile.LastAccessTime = DateTime.Parse(rdr.GetString(7));
                        afile.LastWriteTime = DateTime.Parse(rdr.GetString(8));
                        //string tmp = rdr["CreateT"] as string;
                        //afile.CreationTime = DateTime.Parse(tmp);
                        //tmp = rdr["AccessT"] as string;
                        //afile.LastAccessTime = DateTime.Parse(tmp);
                        //tmp = rdr["WriteT"] as string;
                        //afile.LastWriteTime = DateTime.Parse(tmp);

                        ((IFolder) did).AddToFolders(afile);
                    }
                }
//            }

            foreach (var afold in ((IFolder) did).Folders)
            {
                var fold = afold as FolderInDatabase;
                ReadFiles(conn, fold);
                ReadFolders(conn, fold);
            }
        }

        private static SQLiteCommand _readFileCmd;

        private static void ReadFiles(SQLiteConnection conn, FolderInDatabase did)
        {
            if (_readFileCmd == null)
            {
                _readFileCmd = new SQLiteCommand(conn);
                _readFileCmd.CommandText = "select * from [Files] WHERE Owner = @own";
            }

            _readFileCmd.Parameters.Clear();
            _readFileCmd.Parameters.AddWithValue("@own", did.DbId);

//            string txt = "select * from Files where Owner = " + did.DbId;

//            using (SQLiteCommand cmd = new SQLiteCommand(txt, conn))
//            {
            using (SQLiteDataReader rdr = _readFileCmd.ExecuteReader())
                {
                    while (rdr.Read())
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
                [Desc] TEXT
                */
                        //string start = "insert into Files (Owner, Name, Ext, FullName, Attributes, Length, CreateT, AccessT, WriteT," +
//               "Keywords, Desc, FileDesc, FileVers) VALUES ('" + owner + "',";
                        //afile.Parent = did;
//                        FileInDatabase afile = new FileInDatabase(did);
                        FileInDatabase afile = new FileInDatabase(did, rdr.GetString(3));
                        afile.DbId = rdr.GetInt32(0);
                        afile.Name = rdr.GetString(2);
                        //afile.Extension = rdr.GetString(3);
                        afile.FullName = rdr.GetString(4);
                        afile.Attributes = (FileAttributes) rdr.GetInt64(5);
                        afile.Length = rdr.GetInt64(6);

                        afile.CreationTime = new DateTime(rdr.GetInt64(7));
                        //string tmp = rdr.GetString(7);
                        //afile.CreationTime = DateTime.Parse(tmp);
                        //tmp = rdr.GetString(8);
                        afile.LastAccessTime = new DateTime(rdr.GetInt64(8));
                        //afile.LastAccessTime = DateTime.Parse(tmp);
                        //tmp = rdr.GetString(9);
                        afile.LastWriteTime = new DateTime(rdr.GetInt64(9));
                        //afile.LastWriteTime = DateTime.Parse(tmp);

                        afile.Keywords = rdr.GetString(10);
                        afile.Description = rdr.GetString(11);

                        ((IFolder) did).AddToFiles(afile);
                    }
                }
//            }
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

    }

}
