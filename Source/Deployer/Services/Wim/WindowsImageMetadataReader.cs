using System;
using System.IO;
using System.Linq;
using Serilog;

namespace Deployer.Services.Wim
{
    public class WindowsImageMetadataReader : WindowsImageMetadataReaderBase
    {
        private static long ToInt64LittleEndian(byte[] buffer, int offset)
        {
            return (long)ToUInt64LittleEndian(buffer, offset);
        }

        private static uint ToUInt32LittleEndian(byte[] buffer, int offset)
        {
            var a = (buffer[offset + 3] << 24) & 0xFF000000U;
            var b = (buffer[offset + 2] << 16) & 0x00FF0000U;
            var c = (buffer[offset + 1] << 8) & 0x0000FF00U;
            var d = (buffer[offset + 0] << 0) & 0x000000FFU;

            return (uint)(a | b | c | d);
        }


        private static ulong ToUInt64LittleEndian(byte[] buffer, int offset)
        {
            return ((ulong)ToUInt32LittleEndian(buffer, offset + 4) << 32) | ToUInt32LittleEndian(buffer, offset + 0);
        }
        
        //
        // https://stackoverflow.com/questions/1471975/best-way-to-find-position-in-the-stream-where-given-byte-sequence-starts
        //
        public static long FindPosition(Stream stream, byte[] byteSequence)
        {
            if (byteSequence.Length > stream.Length)
            {
                return -1;
            }

            var buffer = new byte[byteSequence.Length];

            var bufStream = new BufferedStream(stream, byteSequence.Length);
            int i;

            while ((i = bufStream.Read(buffer, 0, byteSequence.Length)) == byteSequence.Length)
            {
                if (byteSequence.SequenceEqual(buffer))
                {
                    return bufStream.Position - byteSequence.Length;
                }

                bufStream.Position -= byteSequence.Length - PadLeftSequence(buffer, byteSequence);
            }

            return -1;
        }

        private static int PadLeftSequence(byte[] bytes, byte[] seqBytes)
        {
            var i = 1;
            while (i < bytes.Length)
            {
                var n = bytes.Length - i;
                var aux1 = new byte[n];
                var aux2 = new byte[n];
                Array.Copy(bytes, i, aux1, 0, n);
                Array.Copy(seqBytes, aux2, n);
                if (aux1.SequenceEqual(aux2))
                {
                    return i;
                }

                i++;
            }

            return i;
        }

        protected override Stream GetXmlMetadataStream(Stream wim)
        {
            var outputstream = new MemoryStream();
            var wimwriter = new BinaryWriter(outputstream);
            using (var wimsecstream = wim)
            {
                using (var wimsecreader = new BinaryReader(wimsecstream))
                {
                    var bytes = new byte[]
                    {
                        0x4D, 0x53, 0x57, 0x49, 0x4D
                    };

                    Log.Verbose("(WIM) Finding Magic Bytes...");

                    var start = WindowsImageMetadataReader.FindPosition(wimsecstream, bytes);

                    Log.Verbose("(WIM) Found Magic Bytes at " + start);

                    Log.Verbose("(WIM) Finding WIM XML Data...");

                    var endbytes = new byte[]
                    {
                        0x3C, 0x00, 0x2F, 0x00, 0x57, 0x00, 0x49, 0x00, 0x4D, 0x00, 0x3E, 0x00
                    };

                    wimsecstream.Seek(start + 72, SeekOrigin.Begin);
                    var buffer = new byte[24];
                    wimsecstream.Read(buffer, 0, 24);
                    var may = WindowsImageMetadataReader.ToInt64LittleEndian(buffer, 8);
                    wimsecstream.Seek(start, SeekOrigin.Begin);

                    Log.Verbose("(WIM) Found WIM XML Data at " + start + may + 2);

                    wimsecstream.Seek(start + may + 2, SeekOrigin.Begin);

                    for (var i = wimsecstream.Position; i < wimsecstream.Length - endbytes.Length; i++)
                    {
                        if (BitConverter.ToString(wimsecreader.ReadBytes(12)) == BitConverter.ToString(endbytes))
                        {
                            wimwriter.Write(endbytes);
                            break;
                        }

                        wimsecstream.Seek(-12, SeekOrigin.Current);
                        wimwriter.Write(wimsecreader.ReadBytes(1));
                    }
                }
            }

            outputstream.Seek(0, SeekOrigin.Begin);
            return outputstream;
        }
    }    
}