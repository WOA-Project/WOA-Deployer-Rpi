using System;
using System.Collections.Generic;

namespace Deployer.FileSystem
{
    public class PartitionType
    {
        private static readonly Guid EspGuid = Guid.Parse("C12A7328-F81F-11D2-BA4B-00A0C93EC93B");
        private static readonly Guid BasicGuid = Guid.Parse("EBD0A0A2-B9E5-4433-87C0-68B6B72699C7");
        private static readonly Guid ReservedGuid = Guid.Parse("E3C9E316-0B5C-4DB8-817D-F92DF00215AE");

        public string Name { get; }
        public Guid Guid { get; }

        public static readonly PartitionType Reserved = new PartitionType("Reserved", ReservedGuid);
        public static readonly PartitionType Esp = new PartitionType("EFI System Partition", EspGuid);
        public static readonly PartitionType Basic = new PartitionType("Basic", BasicGuid);

        private static readonly IDictionary<Guid, PartitionType> PartitionTypes = new Dictionary<Guid, PartitionType>()
        {
            { EspGuid, Esp},
            { BasicGuid, Basic },
            { ReservedGuid, Reserved },
        };

        private PartitionType(string name, Guid guid)
        {
            Name = name;
            Guid = guid;
        }

        public static PartitionType FromGuid(Guid guid)
        {
            if (PartitionTypes.TryGetValue(guid, out var type))
            {
                return type;
            }

            //Log.Warning("The partition type {Guid} is unknown", guid);
            return null;
        }

        protected bool Equals(PartitionType other)
        {
            return Guid.Equals(other.Guid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((PartitionType) obj);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(Guid)}: {Guid}";
        }
    }
}