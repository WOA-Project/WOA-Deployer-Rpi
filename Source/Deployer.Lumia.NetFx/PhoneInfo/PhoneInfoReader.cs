using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Deployer.Lumia.NetFx.PhoneInfo.Streams;
using DiscUtils.Complete;
using DiscUtils.Fat;
using Serilog;

namespace Deployer.Lumia.NetFx.PhoneInfo
{
    public class PhoneInfoReader : IPhoneInfoReader
    {
        public Lumia.PhoneInfo GetPhoneInfo(uint diskNumber)
        {
            var time = DateTime.Now;

            Log.Debug("Initializing Device Stream...");

            var deviceName = @"\\.\PhysicalDrive" + diskNumber;

            var diskSectorSize = (int) GetDiskSize.GetDiskSectorSize(deviceName);
            byte[] platconfig;
            byte[] dppconfig;
            QualcommPartition part;
            using (var devicestream = new DeviceStream(deviceName))
            {
                ulong platStart = 0;
                ulong platEnd = 0;
                ulong dppStart = 0;
                ulong dppEnd = 0;

                ulong sbl1Start = 0;
                ulong sbl1End = 0;

                Log.Debug("Reading device GPT...");

                // Code to find the PLAT and DPP FAT partition offsets
                devicestream.Seek(0, SeekOrigin.Begin);
                var buffer = new byte[diskSectorSize];

                while (Encoding.ASCII.GetString(buffer, 0, 8) != "EFI PART")
                {
                    devicestream.Read(buffer, 0, diskSectorSize);
                }

                var partentrycount = BitConverter.ToUInt32(buffer, 0x50);
                var partentrysize = BitConverter.ToUInt32(buffer, 0x54);
                var bytestoread = (int) Math.Round(partentrycount * partentrysize / (double) diskSectorSize,
                                      MidpointRounding.AwayFromZero) * diskSectorSize;
                var partarray = new byte[bytestoread];
                devicestream.Read(partarray, 0, bytestoread);
                devicestream.Seek(0, SeekOrigin.Begin);

                using (var br = new BinaryReader(new MemoryStream(partarray)))
                {
                    var name = new byte[72]; // fixed name size
                    while (true)
                    {
                        var type = new Guid(br.ReadBytes(16));
                        if (type == Guid.Empty)
                        {
                            break;
                        }

                        br.BaseStream.Seek(16, SeekOrigin.Current);
                        var firstLba = br.ReadUInt64();
                        var lastLba = br.ReadUInt64();
                        br.BaseStream.Seek(0x8, SeekOrigin.Current);
                        name = br.ReadBytes(name.Length);

                        var convname = Encoding.Unicode.GetString(name).TrimEnd('\0');
                        var diskstartoffset = firstLba * (uint) diskSectorSize;
                        var diskendoffset = lastLba * (uint) diskSectorSize;

                        if (convname == "PLAT")
                        {
                            Log.Debug("Found PLAT");

                            platStart = diskstartoffset;
                            platEnd = diskendoffset;
                        }

                        if (convname == "DPP")
                        {
                            Log.Debug("Found DPP");

                            dppStart = diskstartoffset;
                            dppEnd = diskendoffset;
                        }

                        if (convname == "SBL1")
                        {
                            Log.Debug("Found SBL1");

                            sbl1Start = diskstartoffset;
                            sbl1End = diskendoffset;
                        }
                    }
                }

                var sbl1Partition = new byte[platEnd - platStart];

                Log.Debug("Reading SBL1 Partition");
                ChunkReader(devicestream, sbl1Partition, sbl1Start, sbl1End,
                    (int) sbl1End - (int) sbl1Start); // We can just read the whole thing in a single run from testing

                part = new QualcommPartition(sbl1Partition);

                // Initialize DiscUtils
                SetupHelper.SetupComplete();

                Log.Debug("Reading pconf.bin from file system...");

                using (var platFileSystem =
                    new FatFileSystem(new PartialStream(devicestream, (long) platStart, (long) platEnd))
                ) //new MemoryStream(PLATPartition)))
                {
                    using (Stream platConf = platFileSystem.OpenFile(@"pconf.bin", FileMode.Open, FileAccess.Read))
                    using (var platConfigStream = new MemoryStream())
                    {
                        platConf.CopyTo(platConfigStream);
                        platconfig = platConfigStream.ToArray();
                    }
                }

                Log.Debug("Reading product.dat from file system...");

                using (var dppFileSystem =
                    new FatFileSystem(new PartialStream(devicestream, (long) dppStart, (long) dppEnd))
                ) //new MemoryStream(DPPPartition)))
                {
                    var isMmo = dppFileSystem.DirectoryExists("MMO");
                    Log.Debug("Is the device a MMO device: " + isMmo);

                    // Properly handle earlier 950s/RX130s with Nokia folders.
                    using (Stream dppConf = dppFileSystem.OpenFile(isMmo ? @"MMO\product.dat" : @"Nokia\product.dat",
                        FileMode.Open, FileAccess.Read))
                    using (var dppConfigStream = new MemoryStream())
                    {
                        dppConf.CopyTo(dppConfigStream);
                        dppconfig = dppConfigStream.ToArray();
                    }
                }
            }

            var ndate = DateTime.Now - time;

            Log.Debug("Finished in: " + ndate.TotalSeconds + " seconds.");

            Log.Debug("Drumroll...");
            Log.Debug("//////////////////");
            Log.Debug("product.dat");
            Log.Debug("//////////////////");

            // We now did read both product.dat and pconf.bin, you want to detect a device with pconf and check for the ID, if needed you can also read product.dat
            // Below are samples of pconf, you want to read NAME property. This is the device platform identifier.
            // 
            // pconf.bin
            //
            // NAME=P6148
            // PKEY = 3
            // SWVERSION = 03030.00000.14256.02000

            var ddp = ParseDpp(Encoding.ASCII.GetString(dppconfig));
            var plat = ParsePlat(Encoding.ASCII.GetString(platconfig));

            Log.Debug(BitConverter.ToString(part.RootKeyHash));

            return new Lumia.PhoneInfo(ddp, plat, part.RootKeyHash);
        }

        private static DppInfo ParseDpp(string str)
        {
            var dict = FormattedStrToDictionary(str, ':');

            return new DppInfo
            {
                PhoneType = dict["TYPE"],
                Lpsn = dict["LPSN"],
                Mc = dict["MC"],
                Hwid = dict["HWID"],
                Btr = dict["BTR"],
                Ctr = dict["CTR"]
            };
        }

        private static Dictionary<string, string> FormattedStrToDictionary(string getString, char separator)
        {
            var chunks = from line in getString.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
                let chunk = line.Split(separator)
                select new {Tag = chunk[0], Value = chunk[1]};

            return chunks.ToDictionary(x => x.Tag, x => x.Value);
        }

        private static PlatInfo ParsePlat(string str)
        {
            str = str.Replace("\r", "");

            var dict = FormattedStrToDictionary(str, '=');

            return new PlatInfo
            {
                Name = dict["NAME"],
                SwVersion = dict["SWVERSION"],
                AkVersion = dict["AKVERSION"]
            };
        }

        private static void ChunkReader(Stream stream, byte[] buffer, ulong startOffset, ulong endOffset,
            int sectorSize)
        {
            var numberofsectors =
                (long) (endOffset - startOffset) / sectorSize; // The number of sectors we need to read.
            stream.Seek((long) startOffset,
                SeekOrigin.Begin); // Place the stream at the correct position for reading purposes.

            Log.Debug("Starting reading operation");
            Log.Debug("Reading from " + startOffset + " to " + endOffset);

            for (long i = 0; i < numberofsectors; i++)
            {
                stream.Read(buffer, (int) (i * sectorSize), sectorSize); // Read sector per sector for safety (for loop boundaries should be ok)
            }
        }
    }
}