namespace Deployer.Raspberry
{
    public interface IPhoneInfoReader
    {
        PhoneInfo GetPhoneInfo(uint diskNumber);
    }
}