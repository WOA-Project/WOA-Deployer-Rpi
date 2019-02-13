namespace Deployer.Raspberry
{
    public interface IPhoneModelReader
    {
        PhoneModel GetPhoneModel(uint diskNumber);
    }
}