using System;
using System.Collections.Generic;

namespace Deployer.Lumia.NetFx.PhoneInfo
{
    public class PhoneModelReader : IPhoneModelReader
    {
        private readonly IPhoneInfoReader reader;

        public PhoneModelReader(IPhoneInfoReader reader)
        {
            this.reader = reader;
        }

        public PhoneModel GetPhoneModel(uint diskNumber)
        {
            var dict = new Dictionary<(string, string), PhoneModel>()
            {
                {("P6211", "42-7D-8F-D5-A7-F2-27-82-0D-5B-11-BF-8C-6F-76-70-C0-A0-62-2C-C6-1B-A9-5A-AE-E1-8F-75-17-FC-0B-77"), PhoneModel.Cityman },
                {("P6170", "42-7D-8F-D5-A7-F2-27-82-0D-5B-11-BF-8C-6F-76-70-C0-A0-62-2C-C6-1B-A9-5A-AE-E1-8F-75-17-FC-0B-77"), PhoneModel.Hapanero },
                {("P6218", "9C-FA-9A-DB-10-1C-E4-1E-C5-E0-B4-BF-58-6B-CD-37-A4-BA-93-1F-D9-75-F9-99-52-48-5F-EF-0E-7B-DF-A4"), PhoneModel.Talkman },
            };

            var info = reader.GetPhoneInfo(diskNumber);

            var rkhStr = BitConverter.ToString(info.Rkh);
            var name = info.Plat.Name.Replace("_ATT", "");

            return dict[(name, rkhStr)];
        }
    }
}