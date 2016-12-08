using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public sealed class WebsiteType
    {
        public const string Dealsdirect = "Dealsdirect";
        public const string Onlyonline = "Onlyonline";
    }

    public sealed class WebsiteURL
    {
        public const string Onlyonline = "http://search.oo.com.au/search?w={0}&ts=ajax";
        public const string Dealsdirect = "http://search.dealsdirect.com.au/search?asug=&w={0}";
    }

}
