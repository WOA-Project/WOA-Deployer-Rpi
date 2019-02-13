namespace Deployer.Lumia.NetFx
{
    public class TestPhoneModelReader : IPhoneModelReader
    {
        public PhoneModel GetPhoneModel(uint diskNumber)
        {
            return PhoneModel.Cityman;
        }
    }
}