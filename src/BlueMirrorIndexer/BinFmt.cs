using System;
using System.IO;

namespace BlueMirrorIndexer
{
    public class BinFmt
    {
        private int tick;
        private void logit(string msg, bool first = false)
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

        public static VolumeDatabase ReadFromBin()
        {
            var bf = new BinFmt();
            bf.logit("ReadFromBin", true);
            string binFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Indexer.bin");
            if (!File.Exists(binFile))
                return null;

            VolumeDatabase mem;
            using (var f = File.Open(binFile, FileMode.Open))
            using (var r = new BinaryReader(f))
            {
                mem = ReadData(r);
            }
            bf.logit("ReadFromBin-Done");
            return mem;
        }

        public static void WriteToBin(VolumeDatabase mem)
        {
            var bf = new BinFmt();
            bf.logit("WriteToBin",true);

            string binFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Indexer.bin");
            if (File.Exists(binFile))
            {
                File.Delete(binFile);
            }
            using (var f = File.Open(binFile, FileMode.Create))
            using (var w = new BinaryWriter(f))
            {
                WriteData(w, mem);
            }

            bf.logit("WriteToBin complete");
        }

        #region Write Data
        public static void WriteData(BinaryWriter w, VolumeDatabase mem)
        {
            // TODO version header

            var discList = mem.GetDiscs();
            int totDiscs = discList.Count;
            w.Write(totDiscs);
            for (int discCount=0; discCount < totDiscs; discCount++)
            {
                var disc = discList[discCount];
                WriteDisk(w, disc);
                WriteFiles(w, -discCount, ((IFolder)disc).Files);
                WriteFolders(w, -discCount, ((IFolder)disc).Folders);
            }
        }

        private static void WriteDisk(BinaryWriter w, DiscInDatabase disc)
        {
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

        private static void WriteFiles(BinaryWriter w, int owner, FileInDatabase[] files)
        {
            int totFiles = files.Length;
            w.Write(totFiles);
            for (int fileDex = 0; fileDex < totFiles; fileDex++)
            {
                var afile = files[fileDex];
                w.Write(owner);

                w.Write(afile.Name);
                w.Write(afile.Extension);
                w.Write(afile.FullName);
                w.Write((int)afile.Attributes);
                w.Write(afile.Length);
                w.Write(afile.CreationTime.ToUniversalTime().Ticks);
                w.Write(afile.LastAccessTime.ToUniversalTime().Ticks);
                w.Write(afile.LastWriteTime.ToUniversalTime().Ticks);
                w.Write(afile.Keywords);
                w.Write(afile.Description);
                w.Write(afile.CRC);
            }
            
        }

        private static void WriteFolders(BinaryWriter w, int owner, IFolder[] folds)
        {
            int totFolds = folds.Length;
            w.Write(totFolds);
            for (int foldDex = 0; foldDex < totFolds; foldDex++)
            {
                var afold = folds[foldDex];
                var afile = afold as ItemInDatabase;
                if (afile == null)
                    continue; // TODO BUG file count is now wrong!

                w.Write(owner);

                w.Write(afile.Name);
                w.Write(afile.Extension);
                w.Write(afile.FullName);
                w.Write((int)afile.Attributes); // TODO compressed file hack
                w.Write(afile.CreationTime.ToUniversalTime().Ticks);
                w.Write(afile.LastAccessTime.ToUniversalTime().Ticks);
                w.Write(afile.LastWriteTime.ToUniversalTime().Ticks);
                w.Write(afile.Keywords);
                w.Write(afile.Description);

                WriteFiles(w, foldDex, afold.Files);
                WriteFolders(w, foldDex, afold.Folders);
            }
        }

        #endregion

        #region Read Data

        private static VolumeDatabase ReadData(BinaryReader r)
        {
            VolumeDatabase mem = new VolumeDatabase();

            int discCount = r.ReadInt32();
            for (int dex = 0; dex < discCount; dex++)
            {
                var did = ReadDisc(r, dex);
                mem.AddDisc(did);
                ReadFiles(r, did);
                ReadFolds(r, did);
            }

            return mem;
        }

        private static DiscInDatabase ReadDisc(BinaryReader r, int dbId)
        {
            DiscInDatabase did = new DiscInDatabase(dbId);

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
            for (int dex = 0; dex < totFiles; dex++)
            {
                r.ReadInt32(); // TODO skip owner?

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

                FileInDatabase afile = new FileInDatabase(dex, name, ext, fname, fa, len, keyw);
                afile.SetTimes(cTicks, aTicks, wTicks);
                afile.Description = desc;
                afile.CRC = crc;

                did.AddToFiles(afile);
            }
        }

        private static void ReadFolds(BinaryReader r, IFolder did)
        {
            int totFolds = r.ReadInt32();
            for (int dex = 0; dex < totFolds; dex++)
            {
                // TODO COMPRESSED_FLAG hack: should write attributes first
                ItemInDatabase afile = new FolderInDatabase(dex, did);
                r.ReadInt32(); // TODO skip owner?
                afile.Name = r.ReadString();
                afile.Extension = r.ReadString();
                afile.FullName = r.ReadString();
                afile.Attributes = (FileAttributes) r.ReadInt32();
                var ticks = r.ReadInt64();
                afile.CreationTime = new DateTime(ticks);
                ticks = r.ReadInt64();
                afile.LastAccessTime = new DateTime(ticks);
                ticks = r.ReadInt64();
                afile.LastWriteTime = new DateTime(ticks);
                afile.Keywords = r.ReadString();
                afile.Description = r.ReadString();

                did.AddToFolders(afile as IFolder);

                ReadFiles(r, afile as IFolder);
                ReadFolds(r, afile as IFolder);
            }
        }

        #endregion
    }
}
