using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Deployer.Exceptions;
using Deployer.FileSystem;
using Deployer.Services;
using Deployer.Utils;
using Serilog;

namespace Deployer.Filesystem.FullFx
{
    public class DismImageService : ImageServiceBase
    {
        private readonly Regex percentRegex = new Regex(@"(\d*.\d*)%");

        public override async Task ApplyImage(Volume volume, string imagePath, int imageIndex = 1, bool useCompact = false, IObserver<double> progressObserver = null)
        {
            EnsureValidParameters(volume, imagePath, imageIndex);

            ISubject<string> outputSubject = new Subject<string>();
            IDisposable stdOutputSubscription = null;
            if (progressObserver != null)
            {
                stdOutputSubscription = outputSubject
                    .Select(GetPercentage)
                    .Where(d => !double.IsNaN(d))
                    .Subscribe(progressObserver);
            }
            
            var dismName = WindowsCommandLineUtils.Dism;

            var compact = useCompact ? "/compact" : "";

            var args = $@"/Apply-Image {compact} /ImageFile:""{imagePath}"" /Index:{imageIndex} /ApplyDir:{volume.RootDir.Name}";
            
            Log.Verbose("We are about to run DISM: {ExecName} {Parameters}", dismName, args);
            var resultCode = await ProcessUtils.RunProcessAsync(dismName, args, outputObserver: outputSubject);

            progressObserver?.OnNext(double.NaN);

            if (resultCode != 0)
            {
                throw new DeploymentException($"There has been a problem during deployment: DISM exited with code {resultCode}.");
            }

            stdOutputSubscription?.Dispose();
        }

        private double GetPercentage(string dismOutput)
        {
            if (dismOutput == null)
            {
                return double.NaN;
            }

            var matches = percentRegex.Match(dismOutput);

            if (matches.Success)
            {
                var value = matches.Groups[1].Value;
                try
                {
                    var percentage = double.Parse(value, CultureInfo.InvariantCulture) / 100D;
                    return percentage;
                }
                catch (FormatException)
                {
                    Log.Warning($"Cannot convert {value} to double");
                }
            }

            return double.NaN;
        }
    }
}