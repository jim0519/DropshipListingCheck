using DropshipListingCheckBusiness;
using DropshipListingCheckBusiness.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropshipListingCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            AppContext.Instance.Initialize();
            var checkHandler = AppContext.Instance.Resolve<IListingCheck>();
            checkHandler.StartCheck();
        }
    }
}
