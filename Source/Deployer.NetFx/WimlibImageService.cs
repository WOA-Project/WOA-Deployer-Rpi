using System;
using System.Threading.Tasks;
using Deployer.FileSystem;
using ManagedWimLib;

namespace Deployer.Filesystem.FullFx
{
    public class WimlibImageService : ImageServiceBase
    {
        public override async Task ApplyImage(Volume volume, string imagePath, int imageIndex = 1, bool useCompact = false, IObserver<double> progressObserver = null)
        {
            EnsureValidParameters(volume, imagePath, imageIndex);

            await Task.Run(() =>
            {
                using (var wim = Wim.OpenWim(imagePath, OpenFlags.DEFAULT, (msg, info, callback) => UpdatedStatusCallback(msg, info, callback, progressObserver)))
                {
                    wim.ExtractImage(imageIndex, volume.RootDir.Name, ExtractFlags.DEFAULT);
                }
            });
        }

        private static CallbackStatus UpdatedStatusCallback(ProgressMsg msg, object info, object progctx,
            IObserver<double> progressObserver)
        {
            if (info is ProgressInfo_Extract m)
            {
                ulong percentComplete = 0;

                switch (msg)
                {
                    case ProgressMsg.EXTRACT_FILE_STRUCTURE:
                        
                        if (0 < m.EndFileCount)
                        {
                            percentComplete = m.CurrentFileCount * 10 / m.EndFileCount;
                        }

                        break;
                    case ProgressMsg.EXTRACT_STREAMS:
                        
                        if (0 < m.TotalBytes)
                        {
                            percentComplete = 10 + m.CompletedBytes * 80 / m.TotalBytes;
                        }

                        break;
                    case ProgressMsg.EXTRACT_METADATA:
                        
                        if (0 < m.EndFileCount)
                        {
                            percentComplete = 90 + m.CurrentFileCount * 10 / m.EndFileCount;
                        }

                        break;
                }

                progressObserver.OnNext((double)percentComplete / 100);
            }


            return CallbackStatus.CONTINUE;
        }
    }
}