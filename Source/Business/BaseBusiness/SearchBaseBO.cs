using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BaseBusiness
{
    public class SearchBaseBO
    {
        public string sortQuery { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }

    }
}
