using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace DropshipListingCheckBusiness.BusinessModels
{
    public partial class DropshipListingCheckBM 
    {
        public DropshipListingCheckBM()
        {
           
        }

        public string ExternalSKU { get; set; }
        public string Website { get; set; }
        public bool ListingAvailable { get; set; }
        public string InternalSKU { get; set; }

        
    }

  
}
