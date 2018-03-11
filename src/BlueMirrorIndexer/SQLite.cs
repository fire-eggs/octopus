using System;
using System.Data.SQLite;
using System.IO;

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
[FromDrive] TEXT
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
[CreateT] TEXT,
[AccessT] TEXT,
[WriteT] TEXT,
[Keywords] TEXT,
[Desc] TEXT,
[FileDesc] TEXT,
[FileVers] TEXT
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

                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = FileDex;
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = FoldDex;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void CreateTables(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = DiscCreate;
                cmd.ExecuteNonQuery();

                // folders
                cmd.CommandText = FoldCreate;
                cmd.ExecuteNonQuery();

                // files
                cmd.CommandText = FileCreate;
                cmd.ExecuteNonQuery();
            }
        }

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
        }

        private static void WriteDisc(SQLiteConnection conn, DiscInDatabase disc)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "insert into Discs (Format, Type, Free, Size, Label, ScanTime, Serial, PhysicalLocation, FromDrive) values (";
                cmd.CommandText += "'" + disc.DriveFormat + "',";
                cmd.CommandText += "'" + (int)disc.DriveType + "',";
                cmd.CommandText += "'" + disc.TotalFreeSpace + "',";
                cmd.CommandText += "'" + disc.TotalSize + "',";
                cmd.CommandText += "'" + disc.VolumeLabel + "',";
                cmd.CommandText += "'" + disc.Scanned.ToUniversalTime() + "',";
                cmd.CommandText += "'" + disc.SerialNumber + "',";
                cmd.CommandText += "'" + disc.PhysicalLocation + "',";
                cmd.CommandText += "'" + disc.FromDrive + "'";
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
                    cmd.CommandText += "'" + afile.Extension + "',";
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
                           "Keywords, Desc, FileDesc, FileVers) VALUES ('"+owner+"',";
            using (var cmd = conn.CreateCommand())
            {
                foreach (var afile in files)
                {
                    cmd.CommandText = start;
                    cmd.CommandText += "'" + afile.Name.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + afile.Extension + "',";
                    cmd.CommandText += "'" + afile.FullName.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + (int)afile.Attributes + "',";
                    cmd.CommandText += "'" + afile.Length + "',";
                    cmd.CommandText += "'" + afile.CreationTime.ToUniversalTime() + "',";
                    cmd.CommandText += "'" + afile.LastAccessTime.ToUniversalTime() + "',";
                    cmd.CommandText += "'" + afile.LastWriteTime.ToUniversalTime() + "',";
                    cmd.CommandText += "'" + afile.Keywords.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + afile.Description.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + afile.FileDescription.Replace("'", "''") + "',";
                    cmd.CommandText += "'" + afile.FileVersion.Replace("'", "''") + "'";
                    cmd.CommandText += ")";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static VolumeDatabase ReadFromDb(string dbpath)
        {
            // TODO KBR allow user to name file, location

            string dbName = "Indexer.db";
            string dbFile = AppDomain.CurrentDomain.BaseDirectory + dbName;
            string dbSrc = string.Format("Data Source={0};Version=3;", dbName);

            if (!File.Exists(dbFile))
                return null;

            using (var conn = new SQLiteConnection(dbSrc))
            {
                conn.Open();
                VolumeDatabase mem = ReadData(conn);
                return mem;
            }
            // TODO KBR exception, e.g. not a valid db file
        }

        private static VolumeDatabase ReadData(SQLiteConnection conn)
        {
            VolumeDatabase mem = new VolumeDatabase();

            string txt = "select * from Discs";
            SQLiteCommand cmd = new SQLiteCommand(txt, conn);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var dbid = (long)rdr["ID"]; // The top folder has this as the owner id

                DiscInDatabase did = new DiscInDatabase(-(int)dbid);
//"insert into Discs (Format, Type, Free, Size, Label, ScanTime, Serial, PhysicalLocation, FromDrive) values (";

                did.DriveFormat = rdr["Format"] as string;
                did.DriveType = (DriveType) ((long) rdr["Type"]);
                did.TotalFreeSpace = (long) rdr["Free"];
                did.TotalSize = (long) rdr["Size"];
                did.Scanned = DateTime.Parse((string) rdr["ScanTime"]);
                did.SerialNumber = rdr["Serial"] as string;
                did.PhysicalLocation = rdr["PhysicalLocation"] as string;
                did.FromDrive = rdr["FromDrive"] as string;

                mem.AddDisc(did);
            }

            foreach (var discInDatabase in mem.GetDiscs())
            {
                ReadFiles(conn, discInDatabase);
                ReadFolders(conn, discInDatabase);
            }
            return mem;
        }

        private static void ReadFolders(SQLiteConnection conn, FolderInDatabase did)
        {
            string txt = "select * from Folds where Owner = " + did.DbId;
            SQLiteCommand cmd = new SQLiteCommand(txt, conn);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
//"insert into Folds (Owner, Name, Ext, FullName, Attributes, Length, CreateT, AccessT, WriteT) VALUES ('" + owner + "',";
                var dbid = (long)rdr["ID"]; // The top folder has this as the owner id
                FolderInDatabase afile = new FolderInDatabase((int)dbid, did);
                afile.FullName = rdr["FullName"] as string;
                afile.Extension = rdr["Ext"] as string;
                afile.Name = rdr["Name"] as string;
                afile.Attributes = (FileAttributes)((long)rdr["Attributes"]);

                string tmp = rdr["CreateT"] as string;
                afile.CreationTime = DateTime.Parse(tmp);
                tmp = rdr["AccessT"] as string;
                afile.LastAccessTime = DateTime.Parse(tmp);
                tmp = rdr["WriteT"] as string;
                afile.LastWriteTime = DateTime.Parse(tmp);

                ((IFolder)did).AddToFolders(afile);
            }

            foreach (var afold in ((IFolder) did).Folders)
            {
                var fold = afold as FolderInDatabase;
                ReadFiles(conn, fold);
                ReadFolders(conn, fold);
            }
        }

        private static void ReadFiles(SQLiteConnection conn, FolderInDatabase did)
        {
            string txt = "select * from Files where Owner = " + did.DbId;
            SQLiteCommand cmd = new SQLiteCommand(txt, conn);
            SQLiteDataReader rdr = cmd.ExecuteReader();
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
                [Desc] TEXT,
                [FileDesc] TEXT,
                [FileVers] TEXT
                */
                //string start = "insert into Files (Owner, Name, Ext, FullName, Attributes, Length, CreateT, AccessT, WriteT," +
//               "Keywords, Desc, FileDesc, FileVers) VALUES ('" + owner + "',";
                FileInDatabase afile = new FileInDatabase(did);
                afile.Name = rdr.GetString(2);
                afile.Extension = rdr.GetString(3);
                afile.FullName = rdr.GetString(4);
                afile.Attributes = (FileAttributes)rdr.GetInt64(5);
                afile.Length = rdr.GetInt64(6);

                string tmp = rdr.GetString(7);
                afile.CreationTime = DateTime.Parse(tmp);
                tmp = rdr.GetString(8);
                afile.LastAccessTime = DateTime.Parse(tmp);
                tmp = rdr.GetString(9);
                afile.LastWriteTime = DateTime.Parse(tmp);

                afile.Keywords = rdr.GetString(10);
                afile.Description = rdr.GetString(11);
                afile.FileDescription = rdr.GetString(12);
                afile.FileVersion = rdr.GetString(13);
#if false
                afile.FullName = rdr["FullName"] as string;
                afile.Extension = rdr["Ext"] as string;
                afile.Name = rdr["Name"] as string;
                afile.Attributes = (FileAttributes)((long)rdr["Attributes"]);
                afile.Length = (long) rdr["Length"];

                string tmp = rdr["CreateT"] as string;
                afile.CreationTime = DateTime.Parse(tmp);
                tmp = rdr["AccessT"] as string;
                afile.LastAccessTime = DateTime.Parse(tmp);
                tmp = rdr["WriteT"] as string;
                afile.LastWriteTime = DateTime.Parse(tmp);

                afile.Keywords = rdr["Keywords"] as string;
                afile.Description = rdr["Desc"] as string;
                afile.FileDescription = rdr["FileDesc"] as string;
                afile.FileVersion = rdr["FileVers"] as string;
#endif
                ((IFolder)did).AddToFiles(afile);
            }
        }

    }

}
