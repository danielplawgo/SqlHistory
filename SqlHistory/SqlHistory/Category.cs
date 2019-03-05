using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHistory
{
    public class Category : BaseModel
    {
        public string Name { get; set; }

        public virtual ICollection<BaseProduct> Products { get; set; }
    }
}
