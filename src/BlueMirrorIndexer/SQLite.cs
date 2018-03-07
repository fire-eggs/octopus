using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueMirrorIndexer
{
    public class SQLite
    {
        private static string DiscCreate = @"CREATE TABLE IF NOT EXISTS [Discs] (
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
        private static string FileCreate = @"CREATE TABLE IF NOT EXISTS [Files] (
[ID] INTEGER NOT NULL PRIMARY KEY,
[Owner] INTEGER NOT NULL,
[Name] TEXT,
[Ext] TEXT,
[FullName] TEXT,
[Attributes] INTEGER,
[Length] INTEGER,
[CreateT] TEXT,
[AccessT] TEXT,
[WriteT] TEXT
)";

        private static string FoldCreate = @"CREATE TABLE IF NOT EXISTS [Folds] (
[ID] INTEGER NOT NULL PRIMARY KEY,
[Owner] INTEGER NOT NULL,
[Name] TEXT,
[Ext] TEXT,
[FullName] TEXT,
[Attributes] INTEGER,
[Length] INTEGER,
[CreateT] TEXT,
[AccessT] TEXT,
[WriteT] TEXT
)";

        public static void WriteToDb(VolumeDatabase mem)
        {
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
                cmd.CommandText = "insert into discs (Format, Type, Free, Size) values (";
                cmd.CommandText += "'" + disc.DriveFormat + "',";
                cmd.CommandText += "'" + (int)disc.DriveType + "',";
                cmd.CommandText += "'" + disc.TotalFreeSpace + "',";
                cmd.CommandText += "'" + disc.TotalSize + "')";

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
            string start = "insert into Folds (Owner, Name, Ext, FullName, Attributes, Length, CreateT, AccessT, WriteT) VALUES ('" + owner + "',";

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
                    cmd.CommandText += "'" + afile.Length + "',";
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
            string start = "insert into Files (Owner, Name, Ext, FullName, Attributes, Length, CreateT, AccessT, WriteT) VALUES ('"+owner+"',";
            using (var cmd = conn.CreateCommand())
            {
                foreach (var afile in files)
                {
                    try
                    {
                        cmd.CommandText = start;
                        cmd.CommandText += "'" + afile.Name.Replace("'", "''") + "',";
                        cmd.CommandText += "'" + afile.Extension + "',";
                        cmd.CommandText += "'" + afile.FullName.Replace("'", "''") + "',";
                        cmd.CommandText += "'" + (int)afile.Attributes + "',";
                        cmd.CommandText += "'" + afile.Length + "',";
                        cmd.CommandText += "'" + afile.CreationTime.ToUniversalTime() + "',";
                        cmd.CommandText += "'" + afile.LastAccessTime.ToUniversalTime() + "',";
                        cmd.CommandText += "'" + afile.LastWriteTime.ToUniversalTime() + "')";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                    }

                }


            }
        }
    }
}
