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
    public class OnlyonlineDataMap : EntityTypeConfiguration<OnlyonlineData>
    {
        public OnlyonlineDataMap()
        {

            // Table & Column Mappings
            this.ToTable("Import_CSV_OnlyOnlineFeed");

            this.HasKey(t => t.RowID);
            // Properties
            this.Property(t => t.SKU).IsRequired();

            //this.Property(t => t.SourceFile)
            //    .IsRequired();

            this.Property(t => t.AddTime).HasColumnName("DateImport");

            this.Property(t => t.Stock).HasColumnName("QOH");

            this.Property(t => t.NewAimSKU).HasColumnName("SupplierSKU");

            //Relationship

            
        }
    }
}
