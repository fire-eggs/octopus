/* 
 * MIT License. See license.txt for details.
 * 
 * Copyright © 2018 by github.com/fire-eggs.
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;

// TODO seriously consider markers around parts (discs,folders,files,lfolds)

namespace BlueMirrorIndexer
{
    public class BinFmt
    {
        private int tick;

        public void logit(string msg, bool first = false)
        {
            int delta = 0;
            if (first)
                tick = Environment.TickCount;
            else
                delta = Environment.TickCount - tick;
            using (var f = File.Open("octopus.log", FileMode.Append))
            using (var sw = new StreamWriter(f))
            {
                sw.WriteLine("{0}|{1}", msg, delta);
            }
        }

        private const int COMPRESSED_FLAG = 0x1000000;

        public static VolumeDatabase ReadFromBin(string binFile)
        {
            var bf = new BinFmt();
            bf.logit("ReadFromBin", true);
            //string binFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Indexer.bin");
            if (!File.Exists(binFile))
                return null;

            VolumeDatabase mem;
            using (var f = File.Open(binFile, FileMode.Open))
            using (var r = new BinaryReader(f))
            {
                if (!ReadHeader(r))
                    return null; // Not a compatible octopus file
                mem = ReadData(r);
            }
            bf.logit("ReadFromBin-Done");
            return mem;
        }

        public static void WriteToBin(string binFile, VolumeDatabase mem)
        {
            var bf = new BinFmt();
            bf.logit("WriteToBin",true);

            //string binFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Indexer.bin");
            if (File.Exists(binFile))
            {
                File.Delete(binFile);
            }
            using (var f = File.Open(binFile, FileMode.Create))
            using (var w = new BinaryWriter(f))
            {
                WriteHeader(w);
                // Logical folders have reference to data items using DbId
                WriteLogicalFolders(w, mem);
                WriteData(w, mem);
            }

            bf.logit("WriteToBin complete");
        }

        #region Write Data

        private static uint _databaseId;

        private static void WriteHeader(BinaryWriter f)
        {
            // TODO fetch version # from someplace
            f.Write("|Octopus_3.0.0.1|");

            _databaseId = 0; // Fresh assignment of ids
        }

        private static void WriteData(BinaryWriter w, VolumeDatabase mem)
        {
            var discList = mem.GetDiscs();
            int totDiscs = discList.Count;
            w.Write(totDiscs);
            for (int discCount=0; discCount < totDiscs; discCount++)
            {
                var disc = discList[discCount];
                WriteDisk(w, disc);
                WriteFiles(w, ((IFolder)disc).Files);
                WriteFolders(w, ((IFolder)disc).Folders);
            }
        }

        private static void WriteDisk(BinaryWriter w, DiscInDatabase disc)
        {
            w.Write("|Disk|");

            //disc.DbId = ++_databaseId;
            w.Write(disc.DbId);

            w.Write(disc.DriveFormat);
            w.Write((int) disc.DriveType);
            w.Write(disc.TotalFreeSpace);
            w.Write(disc.TotalSize);
            w.Write(disc.VolumeLabel);
            w.Write(disc.Scanned.ToUniversalTime().Ticks);
            w.Write(disc.SerialNumber);
            w.Write(disc.PhysicalLocation);
            w.Write(disc.FromDrive);
            w.Write(disc.Name);
            w.Write(disc.Keywords);
            w.Write(disc.Flags);
            w.Write(disc.Description);
            w.Write(disc.ClusterSize);
        }

        private static void WriteFiles(BinaryWriter w, FileInDatabase[] files)
        {
            int totFiles = files.Length;
            w.Write(totFiles);
            for (int fileDex = 0; fileDex < totFiles; fileDex++)
            {
                var afile = files[fileDex];

                //afile.DbId = ++_databaseId;
                w.Write(afile.DbId);

                w.Write(afile.Name);
                w.Write(afile.Extension);
                w.Write(afile.FullName);
                w.Write((int)afile.Attributes);
                w.Write(afile.Length);
                w.Write(afile.CreationTime.ToUniversalTime().Ticks);
                w.Write(afile.LastAccessTime.ToUniversalTime().Ticks);
                w.Write(afile.LastWriteTime.ToUniversalTime().Ticks);
                w.Write(afile.Keywords);
                w.Write(afile.Description ?? ""); // Null for within compressed files
                w.Write(afile.CRC);
            }
        }

        private static void WriteFolders(BinaryWriter w, IFolder[] folds)
        {
            int totFolds = folds.Length;
            w.Write(totFolds);
            for (int foldDex = 0; foldDex < totFolds; foldDex++)
            {
                var afold = folds[foldDex];
                var afile = afold as ItemInDatabase;
                if (afile == null)
                    continue; // TODO BUG file count is now wrong!

                //afile.DbId = ++_databaseId;
                w.Write(afile.DbId);

                w.Write(afile.Name);
                w.Write(afile.Extension);
                w.Write(afile.FullName);

                // A Q&D hack: mark a 'compressed file' folder with a special Attribute value. Must be removed on read!!!
                int attrib = (int)afile.Attributes;
                if (afold is CompressedFile)
                    attrib |= COMPRESSED_FLAG;

                w.Write(attrib);
                w.Write(afile.CreationTime.ToUniversalTime().Ticks);
                w.Write(afile.LastAccessTime.ToUniversalTime().Ticks);
                w.Write(afile.LastWriteTime.ToUniversalTime().Ticks);
                w.Write(afile.Keywords);
                w.Write(afile.Description ?? ""); // Null within compressed

                WriteFiles(w, afold.Files);
                WriteFolders(w, afold.Folders);
            }
        }

        private static void WriteLogicalFolders(BinaryWriter w, VolumeDatabase mem)
        {
            w.Write("|LFold|");
            var lfolds = mem.GetLogicalFolders();
            w.Write(lfolds.Count);
            foreach (var lfold in lfolds)
            {
                WriteLogicalFolder(w, lfold);
            }
        }

        private static void WriteLogicalFolder(BinaryWriter w, LogicalFolder f)
        {
            w.Write(f.Name);
            w.Write(f.Description);
            w.Write((int)f.FolderType);

            w.Write(f.Items.Length);
            foreach (var iid in f.Items)
            {
                // Only those items referenced by logical folder will get an id
                if (iid.DbId == 0)
                    iid.DbId = ++_databaseId;
                w.Write(iid.DbId);
            }

            w.Write(f.GetSubFolders().Count);
            foreach (var subFolder in f.GetSubFolders())
            {
                WriteLogicalFolder(w, subFolder);
            }
        }

        #endregion

        #region Read Data

        private static bool ReadHeader(BinaryReader r)
        {
            var head = r.ReadString();
            return head == "|Octopus_3.0.0.1|";

            // TODO version handling
        }

        private static VolumeDatabase ReadData(BinaryReader r)
        {
            VolumeDatabase mem = new VolumeDatabase();

            // Logical folders first: they have reference to DbIds ahead
            ReadLFolds(r, mem);
            
            int discCount = r.ReadInt32();
            for (uint dex = 0; dex < discCount; dex++)
            {
                var did = ReadDisc(r);
                mem.AddDisc(did);
                ReadFiles(r, did);
                ReadFolds(r, did);
            }

            return mem;
        }

        private static DiscInDatabase ReadDisc(BinaryReader r)
        {
            var head = r.ReadString();

            var dbId = r.ReadUInt32();
            DiscInDatabase did = new DiscInDatabase(dbId);

            HookLogicalFolder(dbId, did);

            did.DriveFormat = r.ReadString();
            did.DriveType= (DriveType)r.ReadInt32();
            did.TotalFreeSpace= r.ReadInt64();
            did.TotalSize= r.ReadInt64();
            did.VolumeLabel= r.ReadString();
            var ticks = r.ReadInt64();
            did.Scanned = new DateTime(ticks);
            did.SerialNumber = r.ReadString();
            did.PhysicalLocation = r.ReadString();
            did.FromDrive = r.ReadString();
            did.Name = r.ReadString();
            did.Keywords = r.ReadString();
            did.Flags = r.ReadInt32();
            did.Description = r.ReadString();
            did.ClusterSize= r.ReadUInt32();

            return did;
        }

        private static void ReadFiles(BinaryReader r, IFolder did)
        {
            int totFiles = r.ReadInt32();
            for (uint dex = 0; dex < totFiles; dex++)
            {
                uint dbId = r.ReadUInt32();

                string name = r.ReadString();
                string ext = r.ReadString();
                string fname = r.ReadString();
                FileAttributes fa = (FileAttributes) r.ReadInt32();
                long len = r.ReadInt64();
                long cTicks = r.ReadInt64();
                long aTicks = r.ReadInt64();
                long wTicks = r.ReadInt64();
                string keyw = r.ReadString();
                string desc = r.ReadString();
                uint crc = r.ReadUInt32();

                FileInDatabase afile = new FileInDatabase(dbId, name, ext, fname, fa, len, keyw);
                afile.SetTimes(cTicks, aTicks, wTicks);
                afile.Description = desc;
                afile.CRC = crc;

                did.AddToFiles(afile);

                HookLogicalFolder(dbId, afile);
            }
        }

        private static void ReadFolds(BinaryReader r, IFolder did)
        {
            int totFolds = r.ReadInt32();
            for (uint dex = 0; dex < totFolds; dex++)
            {
                uint dbId = r.ReadUInt32();
                var name = r.ReadString();
                var ext = r.ReadString();
                var full = r.ReadString();
                var attrib = r.ReadInt32();

                // TODO COMPRESSED_FLAG hack: should write attributes first
                ItemInDatabase afile;
                if ((attrib & COMPRESSED_FLAG) != 0)
                    afile = new CompressedFile(dbId,did);
                else
                    afile = new FolderInDatabase(dbId, did);

                afile.Name = name;
                afile.Extension = ext;
                afile.FullName = full;
                afile.Attributes = (FileAttributes) (attrib & ~COMPRESSED_FLAG);
                var ticks = r.ReadInt64();
                afile.CreationTime = new DateTime(ticks);
                ticks = r.ReadInt64();
                afile.LastAccessTime = new DateTime(ticks);
                ticks = r.ReadInt64();
                afile.LastWriteTime = new DateTime(ticks);
                afile.Keywords = r.ReadString();
                afile.Description = r.ReadString();

                did.AddToFolders(afile as IFolder);

                HookLogicalFolder(dbId, afile);

                ReadFiles(r, afile as IFolder);
                ReadFolds(r, afile as IFolder);
            }
        }

        // TODO dictionary is overkill? array, index on dbid sufficient?
        private static Dictionary<uint, List<LogicalFolder>> _lfoldMap;

        private static void AddToLFoldMap(uint dbid, LogicalFolder refer)
        {
            List<LogicalFolder> lfolds;
            if (_lfoldMap.TryGetValue(dbid, out lfolds))
            {
                lfolds.Add(refer);
            }
            else
            {
                lfolds = new List<LogicalFolder>();
                lfolds.Add(refer);
                _lfoldMap.Add(dbid, lfolds);
            }
        }

        // Determine if an entity needs to be connected to a logical folder
        private static void HookLogicalFolder(uint dbid, ItemInDatabase iid)
        {
            if (dbid == 0 || iid == null)
                return;
            var lfolds = _lfoldMap[dbid];
            foreach (var logicalFolder in lfolds)
            {
                logicalFolder.AddItem(iid);
            }
        }

        private static void ReadLFolds(BinaryReader r, VolumeDatabase mem)
        {
            var head = r.ReadString();

            _lfoldMap = new Dictionary<uint, List<LogicalFolder>>();

            int count = r.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var lf = ReadLogFold(r);
                mem.AddLogicalFolder(lf);
            }
        }

        private static LogicalFolder ReadLogFold(BinaryReader r)
        {
            LogicalFolder lf = new LogicalFolder();
            lf.Name = r.ReadString();
            lf.Description = r.ReadString();
            lf.FolderType = (LogicalFolderType)r.ReadInt32();

            int itemCount = r.ReadInt32();
            for (int j = 0; j < itemCount; j++)
            {
                var dbid = r.ReadUInt32();
                AddToLFoldMap(dbid, lf);
            }

            int subfolds = r.ReadInt32();
            for (int j = 0; j < subfolds; j++)
            {
                var slf = ReadLogFold(r);
                lf.GetSubFolders().Add(slf);
            }
            return lf;
        }

        #endregion
    }
}
