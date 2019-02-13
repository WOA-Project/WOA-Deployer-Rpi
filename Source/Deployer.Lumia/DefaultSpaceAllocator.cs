using System;
using System.Threading.Tasks;
using ByteSizeLib;
using Serilog;

namespace Deployer.Lumia
{
    public class DefaultSpaceAllocator : ISpaceAllocator<IPhone>
    {
        public async Task<bool> TryAllocate(IPhone phone, ByteSize requiredSpace)
        {
            Log.Verbose("Shrinking Data partition...");

            var dataVolume = await phone.GetDataVolume();

            if (dataVolume == null)
            {
                return false;
            }

            var phoneDisk = await phone.GetDeviceDisk();
            var data = dataVolume.Size;
            var allocated = phoneDisk.AllocatedSize;
            var available = phoneDisk.AvailableSize;
            var newData =  data - (requiredSpace - available);



            Log.Verbose("Total size allocated: {Size}", allocated);
            Log.Verbose("Space available: {Size}", available);
            Log.Verbose("Space needed: {Size}", requiredSpace);
            Log.Verbose("'Data' size: {Size}", data);
            Log.Verbose("Calculated new size for the 'Data' partition: {Size}", newData);
          
            Log.Verbose("Resizing 'Data' to {Size}", newData);

            await dataVolume.Partition.Resize(newData);

            Log.Verbose("Resize operation completed successfully");

            return await phone.IsThereEnoughSpace(requiredSpace);
        }     
    }
}