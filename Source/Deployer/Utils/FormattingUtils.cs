using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Deployer.Utils
{
    public static class FormattingUtils
    {
        public static Guid GetGuid(string str)
        {
            var guids = Regex.Matches(str, @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}");
            var match = guids.Cast<Match>().First(x => x.Success).Value;
            return Guid.Parse(match);
        }
    }
}