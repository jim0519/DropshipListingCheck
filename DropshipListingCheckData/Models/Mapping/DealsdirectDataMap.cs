using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Common.Models;

namespace DropshipListingCheckData.Models.Mapping
{
    public class DealsdirectDataMap : EntityTypeConfiguration<DealsdirectData>
    {
        public DealsdirectDataMap()
        {

            // Table & Column Mappings
            this.ToTable("Import_CSV_DealsdirectFeed");

            this.HasKey(t => t.RowID);

            // Properties
            this.Property(t => t.SKU).HasColumnName("Product_Code")
                .IsRequired();

            //this.Property(t => t.SourceFile)
            //    .IsRequired();

            this.Property(t => t.AddTime).HasColumnName("DateImport");

            

            

            //Relationship

            
        }
    }
}
