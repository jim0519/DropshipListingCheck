using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public partial class OnlyonlineData : BaseEntity
    {
        public OnlyonlineData()
        {
        }

        public Int64 RowID { get; set; }
        [CsvColumn(Name = "SKU")]
        public string SKU { get; set; }
        public DateTime AddTime { get; set; }
        public string SourceFile { get; set; }
        public int Stock { get; set; }
        [CsvColumn(Name = "SupplierSKU")]
        public string NewAimSKU { get; set; }

        
    }
}
