using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHistory.Configuration
{
    public class BaseProductConfiguration : EntityTypeConfiguration<BaseProduct>
    {
        public BaseProductConfiguration()
        {
            HasKey(p => new { p.Id, p.ValidFrom, p.ValidTo });
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.ValidFrom).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.ValidTo).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}