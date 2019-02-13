namespace Deployer.Lumia
{
    public interface IPhoneModelReader
    {
        PhoneModel GetPhoneModel(uint diskNumber);
    }
}