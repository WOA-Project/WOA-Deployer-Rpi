using Serilog.Events;

namespace Deployer.Raspberry.Gui.ViewModels
{
    public class RenderedLogEvent
    {
        public string Message { get; set; }
        public LogEventLevel Level { get; set; }
    }
}