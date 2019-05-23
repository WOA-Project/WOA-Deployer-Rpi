using System.Collections.Generic;
using Deployer.UI;
using Deployer.UI.ViewModels;
using Grace.DependencyInjection;

namespace Deployer.Raspberry.Gui.ViewModels
{
    public class MainViewModel : MainViewModelBase
    {
        public MainViewModel(IList<Meta<ISection>> sections, IList<IBusy> busies) : base(sections, busies)
        {
        }

        protected override string DonationLink => "https://github.com/WOA-Project/WOA-Deployer-Rpi/blob/master/Docs/Donations.md";
        protected override string HelpLink => "https://github.com/WOA-Project/WOA-Deployer-Rpi#need-help";
        public override string Title => AppProperties.AppTitle;
    }
}