using System;
using System.Globalization;
using System.IO;
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
    public class ImageFlasher : IImageFlasher
    {
        private readonly Regex percentRegex = new Regex(@"(\d*.\d*)%");

        public async Task Flash(Disk disk, string imagePath, IObserver<double> progressObserver = null)
        {
            ISubject<string> outputSubject = new Subject<string>();
            IDisposable stdOutputSubscription = null;
            bool isValidating = false;
            if (progressObserver != null)
            {
                stdOutputSubscription = outputSubject
                    .Do(s =>
                    {
                        if (!isValidating && CultureInfo.CurrentCulture.CompareInfo.IndexOf(s, "validating", 0, CompareOptions.IgnoreCase) != -1)
                        {
                            Log.Verbose("Validating flashed image...");
                            isValidating = true;
                        }
                    })
                    .Select(GetPercentage)
                    .Where(d => !double.IsNaN(d))
                    .Subscribe(progressObserver);
            }
            
            //etcher.exe -d \\.\PHYSICALDRIVE3 "..\Tutorial Googulator\gpt.zip" --yes
            var gptSchemeImagePath = Path.Combine("Files", "Core", "Gpt.zip");

            var platformSuffix = Environment.Is64BitProcess ? "x64" : "x86";
            var etcherPath = Path.Combine("Files", "Tools", "Etcher-Cli", platformSuffix, "Etcher");
            var args = $@"-d \\.\PHYSICALDRIVE{disk.Number} ""{gptSchemeImagePath}"" --yes --no-unmount";
            Log.Verbose("We are about to run Etcher: {ExecName} {Parameters}", etcherPath, args);
            var resultCode = await ProcessUtils.RunProcessAsync(etcherPath, args, outputObserver: outputSubject);
            if (resultCode != 0)
            {
                throw new DeploymentException($"There has been a problem during deployment: Etcher exited with code {resultCode}.");
            }

            progressObserver?.OnNext(double.NaN);

            stdOutputSubscription?.Dispose();
        }

        private double GetPercentage(string output)
        {
            if (output == null)
            {
                return double.NaN;
            }

            var matches = percentRegex.Match(output);

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