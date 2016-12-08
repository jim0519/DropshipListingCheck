using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Common.Models
{
    public partial class DealsdirectData : BaseEntity
    {
        public DealsdirectData()
        {
        }

        public Int64 RowID { get; set; }
        [CsvColumn(Name = "Product_Code")]
        public string SKU { get; set; }
        public DateTime AddTime { get; set; }
        public string SourceFile { get; set; }
        public string Available { get; set; }
        public int Stock { get; set; }

        
    }
}
