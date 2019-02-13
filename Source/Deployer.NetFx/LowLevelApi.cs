using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.FileSystem;
using Serilog;

namespace Deployer.Filesystem.FullFx
{
    public class LowLevelApi : ILowLevelApi
    {
        private readonly PowerShell ps;

        public LowLevelApi()
        {
            ps = PowerShell.Create();
        }

        public async Task<ICollection<Disk>> GetDisks()
        {
            ps.Commands.Clear();
            ps.AddCommand("Get-Disk");

            var results = await Task.Factory.FromAsync(ps.BeginInvoke(), r => ps.EndInvoke(r));

            var disks = results
                .Select(x => x.ImmediateBaseObject)
                .Select(x => ToDisk(this, x));

            return disks.ToList();
        }

        public async Task<ICollection<DriverMetadata>> GetDrivers(string path)
        {
            ps.Commands.Clear();
            ps.AddCommand($"Get-WindowsDriver");
            ps.AddParameter("Path", path);

            var results = await Task.Factory.FromAsync(ps.BeginInvoke(), r => ps.EndInvoke(r));

            var disks = results
                .Select(ToDriverMetadata);

            return disks.ToList();
        }
        
        public async Task<IList<Volume>> GetVolumes(Disk disk)
        {
            var partitions = await GetPartitions(disk);

            if (!partitions.Any())
            {
                Log.Verbose("Couldn't find any partition in {Disk}. Updating Storage Cache...", disk);
                await UpdateStorageCache();
                Log.Verbose("Retrying Get Partitions...", disk);
                partitions = await GetPartitions(disk);
            }

            var partitionsObs = partitions.ToObservable();

            var volumes = partitionsObs
                .Select(x => Observable.FromAsync(async () =>
                {
                    try
                    {
                        return await GetVolume(x);
                    }
                    catch (Exception)
                    {
                        Log.Warning($"Cannot get volume for partition {x}");
                        return null;
                    }
                }))
                .Merge(1)
                .Where(v => v != null)
                .ToList();

            return await volumes;
        }

        public Task UpdateStorageCache()
        {
            ps.Commands.Clear();
            ps.AddCommand($@"Update-HostStorageCache");

            return Task.Factory.FromAsync(ps.BeginInvoke(), x => ps.EndInvoke(x));
        }

        public Task RemovePartition(Partition partition)
        {
            ps.Commands.Clear();
            var cmd = $@"Remove-Partition -DiskNumber {partition.Disk.Number} -PartitionNumber {partition.Number} -Confirm:$false";
            ps.AddScript(cmd);

            return Task.Factory.FromAsync(ps.BeginInvoke(), x => ps.EndInvoke(x));
        }

        private static Disk ToDisk(ILowLevelApi lowLevelApi, object disk)
        {
            var number = (uint)disk.GetPropertyValue("Number");
            var size = new ByteSize((ulong)disk.GetPropertyValue("Size"));
            var allocatedSize = new ByteSize((ulong)disk.GetPropertyValue("AllocatedSize"));

            var diskProps = new DiskInfo
            {
                Number = number,
                Size = size,
                AllocatedSize = allocatedSize,
                FriendlyName = (string)disk.GetPropertyValue("FriendlyName"),
                IsSystem = (bool)disk.GetPropertyValue("IsSystem"),
                IsBoot = (bool)disk.GetPropertyValue("IsBoot"),
                IsOffline = (bool)disk.GetPropertyValue("IsOffline"),
                IsReadOnly = (bool)disk.GetPropertyValue("IsReadOnly"),
            };

            return new Disk(lowLevelApi, diskProps);
        }


        public async Task ResizePartition(Partition partition, ByteSize size)
        {
            ps.Commands.Clear();

            if (size.MegaBytes < 0)
            {
                throw new InvalidOperationException($"The partition size cannot be negative: {size}");
            }

            var sizeBytes = (ulong)size.Bytes;
            Log.Verbose("Resizing partition {Partition} to {Size}", partition, size);

            ps.AddCommand("Resize-Partition")
                .AddParameter("DiskNumber", partition.Disk.Number)
                .AddParameter("PartitionNumber", partition.Number)
                .AddParameter("Size", sizeBytes);

            await Task.Factory.FromAsync(ps.BeginInvoke(), x => ps.EndInvoke(x));
            if (ps.HadErrors)
            {
                Throw("The resize operation has failed");
            }
        }

        private void Throw(string message)
        {
            var errors = string.Join(",", ps.Streams.Error.ReadAll());

            var invalidOperationException = new InvalidOperationException($@"{message}. Details: {errors}");
            Log.Error(invalidOperationException, message);

            throw invalidOperationException;
        }

        public async Task<List<Partition>> GetPartitions(Disk disk)
        {
            ps.Commands.Clear();
            ps.AddScript($"Get-Disk -Number {disk.Number} | Get-Partition");

            var results = await Task.Factory.FromAsync(ps.BeginInvoke(), x => ps.EndInvoke(x));

            var partitions = results
                .Select(x => x.ImmediateBaseObject)
                .Select(x => ToPartition(disk, x))
                .ToList();

            return partitions;
        }

        public async Task<Volume> GetVolume(Partition partition)
        {
            ps.Commands.Clear();
            ps.AddScript($@"Get-Partition -UniqueId ""{partition.Id}"" | Get-Volume", true);

            var results = await Task.Factory.FromAsync(ps.BeginInvoke(), x => ps.EndInvoke(x));
            var volume = results.FirstOrDefault()?.ImmediateBaseObject;

            if (volume == null)
            {
                return null;
            }

            return new Volume(partition)
            {
                Partition = partition,
                Size = new ByteSize(Convert.ToUInt64(volume.GetPropertyValue("Size"))),
                Label = (string)volume.GetPropertyValue("FileSystemLabel"),
                Letter = (char?)volume.GetPropertyValue("DriveLetter")
            };
        }

        public async Task<Partition> CreateReservedPartition(Disk disk, ulong sizeInBytes)
        {
            ps.Commands.Clear();
            ps.AddCommand("New-Partition")
                .AddParameter("DiskNumber", disk.Number)
                .AddParameter("GptType", "{e3c9e316-0b5c-4db8-817d-f92df00215ae}")
                .AddParameter("Size", sizeInBytes);

            var results = await Task.Factory.FromAsync(ps.BeginInvoke(), x => ps.EndInvoke(x));
            var partition = results.First().ImmediateBaseObject;

            return ToPartition(disk, partition);
        }

        public async Task<Partition> CreatePartition(Disk disk, ulong sizeInBytes)
        {
            ps.Commands.Clear();
            ps.AddCommand("New-Partition")
                .AddParameter("DiskNumber", disk.Number);

            if (sizeInBytes == ulong.MaxValue)
            {
                ps.AddParameter("UseMaximumSize");
            }
            else
            {
                ps.AddParameter("Size", sizeInBytes);
            }

            var results = await Task.Factory.FromAsync(ps.BeginInvoke(), x => ps.EndInvoke(x));
            var partition = results.First().ImmediateBaseObject;

            return ToPartition(disk, partition);
        }

        private static DriverMetadata ToDriverMetadata(PSObject driverMetadata)
        {
            return new DriverMetadata
            {
                Driver = (string)driverMetadata.Properties["Driver"].Value,
                OriginalFileName = (string)driverMetadata.Properties["OriginalFileName"].Value,
                Inbox = (bool)driverMetadata.Properties["Inbox"].Value,
                BootCritical = (bool)driverMetadata.Properties["BootCritical"].Value,
                ProviderName = (string)driverMetadata.Properties["ProviderName"].Value,
                Date = (DateTime)driverMetadata.Properties["Date"].Value,
            };
        }

        private static Partition ToPartition(Disk disk, object partition)
        {
            string guid = (string)partition.GetPropertyValue("GptType");
            var partitionType = guid != null ? PartitionType.FromGuid(Guid.Parse(guid)) : null;

            return new Partition(disk)
            {
                Number = (uint)partition.GetPropertyValue("PartitionNumber"),
                Id = (string)partition.GetPropertyValue("UniqueId"),
                Letter = (char?)partition.GetPropertyValue("DriveLetter"),
                PartitionType = partitionType,
            };
        }

        public Task SetPartitionType(Partition partition, PartitionType partitionType)
        {
            ps.Commands.Clear();
            var cmd = $@"Set-Partition -PartitionNumber {partition.Number} -DiskNumber {partition.Disk.Number} -GptType ""{{{partitionType.Guid}}}""";
            ps.AddScript(cmd);

            var result = Task.Factory.FromAsync(ps.BeginInvoke(), x => ps.EndInvoke(x));

            if (ps.HadErrors)
            {
                Throw($"Cannot set the partition type {partitionType} to {partition}");
            }

            return result;
        }

        public Task Format(Volume volume, FileSystemFormat fileSystemFormat, string fileSystemLabel)
        {
            ps.Commands.Clear();
            var cmd = $@"Get-Partition -UniqueId ""{volume.Partition.Id}"" | Get-Volume | Format-Volume -FileSystem {fileSystemFormat.Moniker} -NewFileSystemLabel ""{fileSystemLabel}"" -Force -Confirm:$false";
            ps.AddScript(cmd);

            return Task.Factory.FromAsync(ps.BeginInvoke(), x => ps.EndInvoke(x));
        }

        public Task AssignDriveLetter(Volume volume, char driverLetter)
        {
            ps.Commands.Clear();
            var cmd = $@"Set-Partition -DiskNumber {volume.Partition.Disk.Number} -PartitionNumber {volume.Partition.Number} -NewDriveLetter {driverLetter}";
            ps.AddScript(cmd);

            return Task.Factory.FromAsync(ps.BeginInvoke(), x => ps.EndInvoke(x));
        }

        public char GetFreeDriveLetter()
        {
            var drives = Enumerable.Range('C', 'Z').Select(i => (char)i);
            var usedDrives = DriveInfo.GetDrives().Select(x => char.ToUpper(x.Name[0]));

            var available = drives.Except(usedDrives);

            return available.First();
        }
    }
}