﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHistory.Configuration
{
    public class ProductHistoryConfiguration : EntityTypeConfiguration<ProductHistory>
    {
        public ProductHistoryConfiguration()
        {
            Map(m => { m.ToTable("dbo.ProductsHistory"); m.MapInheritedProperties(); });
        }
    }
}
