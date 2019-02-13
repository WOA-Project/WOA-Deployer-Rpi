namespace Deployer.Lumia
{
    public class PhoneInfo
    {
        public DppInfo Ddp { get; }
        public PlatInfo Plat { get; }
        public byte[] Rkh { get; }

        public PhoneInfo(DppInfo ddp, PlatInfo plat, byte[] rkh)
        {
            Ddp = ddp;
            Plat = plat;
            Rkh = rkh;
        }
    }
}