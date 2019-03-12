using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHistory
{
    public class Order : BaseModel
    {
        public string Number { get; set; }

        public virtual ICollection<BaseProduct> Products { get; set; }
    }
}
