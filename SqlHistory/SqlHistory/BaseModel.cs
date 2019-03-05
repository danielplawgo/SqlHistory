using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHistory
{
    public class BaseModel
    {
        public BaseModel()
        {
            IsActive = true;
        }

        public int Id { get; set; }

        public bool IsActive { get; set; }
    }
}
