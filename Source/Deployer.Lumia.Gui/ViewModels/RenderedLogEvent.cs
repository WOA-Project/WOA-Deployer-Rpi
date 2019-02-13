using Serilog.Events;

namespace Deployer.Lumia.Gui.ViewModels
{
    public class RenderedLogEvent
    {
        public string Message { get; set; }
        public LogEventLevel Level { get; set; }
    }
}