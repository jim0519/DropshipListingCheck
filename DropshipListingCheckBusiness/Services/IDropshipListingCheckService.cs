using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace DropshipListingCheckBusiness.Services
{
    public interface IDropshipListingCheckService
    {
        IList<OnlyonlineData> GetOnlyonlineData();

        IList<DealsdirectData> GetDealsdirectData();
    }
}
