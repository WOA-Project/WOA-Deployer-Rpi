namespace Deployer
{
    public class DualBootStatus
    {
        public DualBootStatus(bool canDualBoot, bool isEnabled)
        {
            IsEnabled = isEnabled;
            CanDualBoot = canDualBoot;
        }

        public bool IsEnabled { get; }
        public bool CanDualBoot { get; }
    }
}