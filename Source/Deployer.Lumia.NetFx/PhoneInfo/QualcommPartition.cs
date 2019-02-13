// Copyright (c) 2018, Rene Lergner - wpinternals.net - @Heathcliff74xda
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Security.Cryptography;

namespace Deployer.Lumia.NetFx.PhoneInfo
{
    internal class QualcommPartition
    {
        internal byte[] Binary;
        internal uint HeaderOffset;
        internal QualcommPartitionHeaderType HeaderType;
        internal uint ImageOffset;
        internal uint ImageAddress;
        internal uint ImageSize;
        internal uint CodeSize;
        internal uint SignatureAddress;
        internal uint SignatureSize;
        internal uint SignatureOffset;
        internal uint CertificatesAddress;
        internal uint CertificatesSize;
        internal uint CertificatesOffset;
        internal byte[] RootKeyHash = null;

        internal QualcommPartition(string Path) : this(File.ReadAllBytes(Path)) { }

        internal QualcommPartition(byte[] Binary, uint Offset = 0)
        {
#if DEBUG
            System.Diagnostics.Debug.Print("Loader: " + Converter.ConvertHexToString(new SHA256Managed().ComputeHash(Binary, 0, Binary.Length), ""));
#endif

            this.Binary = Binary;

            byte[] LongHeaderPattern = new byte[] { 0xD1, 0xDC, 0x4B, 0x84, 0x34, 0x10, 0xD7, 0x73, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] LongHeaderMask = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            if (ByteOperations.FindPattern(Binary, Offset, 4, new byte[] { 0x7F, 0x45, 0x4C, 0x46 }, new byte[] { 0x00, 0x00, 0x00, 0x00 }, null) == 0)
            {
                // This is an ELF image
                // First program header is a reference to the elf-header
                // Second program header is a reference to the signed hash-table
                HeaderType = QualcommPartitionHeaderType.Short;
                UInt32 ProgramHeaderOffset;
                UInt16 ProgramHeaderEntrySize;
                UInt32 HashTableProgramHeaderOffset;
                if (Binary[Offset + 0x04] == 1)
                {
                    // 32-bit elf image
                    ProgramHeaderOffset = Offset + ByteOperations.ReadUInt32(Binary, Offset + 0x1c);
                    ProgramHeaderEntrySize = ByteOperations.ReadUInt16(Binary, Offset + 0x2a);
                    HashTableProgramHeaderOffset = ProgramHeaderOffset + ProgramHeaderEntrySize;
                    ImageOffset = Offset + ByteOperations.ReadUInt32(Binary, HashTableProgramHeaderOffset + 0x04);
                    HeaderOffset = ImageOffset + 8;
                }
                else if (Binary[Offset + 0x04] == 2)
                {
                    // 64-bit elf image
                    ProgramHeaderOffset = Offset + ByteOperations.ReadUInt32(Binary, Offset + 0x20);
                    ProgramHeaderEntrySize = ByteOperations.ReadUInt16(Binary, Offset + 0x36);
                    HashTableProgramHeaderOffset = ProgramHeaderOffset + ProgramHeaderEntrySize;
                    ImageOffset = Offset + (UInt32)ByteOperations.ReadUInt64(Binary, HashTableProgramHeaderOffset + 0x08);
                    HeaderOffset = ImageOffset + 8;
                }
                else
                    throw new Exception("Invalid programmer");
            }
            else if (ByteOperations.FindPattern(Binary, Offset, (uint)LongHeaderPattern.Length, LongHeaderPattern, LongHeaderMask, null) == null)
            {
                HeaderType = QualcommPartitionHeaderType.Short;
                ImageOffset = Offset;
                HeaderOffset = ImageOffset + 8;
            }
            else
            {
                HeaderType = QualcommPartitionHeaderType.Long;
                ImageOffset = Offset;
                HeaderOffset = ImageOffset + (uint)LongHeaderPattern.Length;
            }

            if (ByteOperations.ReadUInt32(Binary, HeaderOffset + 0X00) != 0)
                ImageOffset = ByteOperations.ReadUInt32(Binary, HeaderOffset + 0X00);
            else if (HeaderType == QualcommPartitionHeaderType.Short)
                ImageOffset += 0x28;
            else
                ImageOffset += 0x50;

            ImageAddress = ByteOperations.ReadUInt32(Binary, HeaderOffset + 0X04);
            ImageSize = ByteOperations.ReadUInt32(Binary, HeaderOffset + 0X08);
            CodeSize = ByteOperations.ReadUInt32(Binary, HeaderOffset + 0X0C);
            SignatureAddress = ByteOperations.ReadUInt32(Binary, HeaderOffset + 0X10);
            SignatureSize = ByteOperations.ReadUInt32(Binary, HeaderOffset + 0X14);
            SignatureOffset = SignatureAddress - ImageAddress + ImageOffset;
            CertificatesAddress = ByteOperations.ReadUInt32(Binary, HeaderOffset + 0X18);
            CertificatesSize = ByteOperations.ReadUInt32(Binary, HeaderOffset + 0X1C);
            CertificatesOffset = CertificatesAddress - ImageAddress + ImageOffset;

            uint CurrentCertificateOffset = CertificatesOffset;
            uint CertificateSize = 0;
            while (CurrentCertificateOffset < (CertificatesOffset + CertificatesSize))
            {
                if ((Binary[CurrentCertificateOffset] == 0x30) && (Binary[CurrentCertificateOffset + 1] == 0x82))
                {
                    CertificateSize = (uint)(Binary[CurrentCertificateOffset + 2] * 0x100) + Binary[CurrentCertificateOffset + 3] + 4; // Big endian!

                    if ((CurrentCertificateOffset + CertificateSize) == (CertificatesOffset + CertificatesSize))
                    {
                        // This is the last certificate. So this is the root key.
                        RootKeyHash = new SHA256Managed().ComputeHash(Binary, (int)CurrentCertificateOffset, (int)CertificateSize);

#if DEBUG
                        System.Diagnostics.Debug.Print("RKH: " + Converter.ConvertHexToString(RootKeyHash, ""));
#endif
                    }
#if DEBUG
                    else
                    {
                        System.Diagnostics.Debug.Print("Cert: " + Converter.ConvertHexToString(new SHA256Managed().ComputeHash(Binary, (int)CurrentCertificateOffset, (int)CertificateSize), ""));
                    }
#endif
                    CurrentCertificateOffset += CertificateSize;
                }
                else
                {
                    if ((RootKeyHash == null) && (CurrentCertificateOffset > CertificatesOffset))
                    {
                        CurrentCertificateOffset -= CertificateSize;

                        // This is the last certificate. So this is the root key.
                        RootKeyHash = new SHA256Managed().ComputeHash(Binary, (int)CurrentCertificateOffset, (int)CertificateSize);

#if DEBUG
                        System.Diagnostics.Debug.Print("RKH: " + Converter.ConvertHexToString(RootKeyHash, ""));
#endif
                    }
                    break;
                }
            }
        }
    }
}