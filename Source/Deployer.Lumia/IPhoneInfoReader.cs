namespace Deployer.Lumia
{
    public interface IPhoneInfoReader
    {
        PhoneInfo GetPhoneInfo(uint diskNumber);
    }
}