using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.EFILECHIASE
{
    public class EFILE_CHIASE_SEARCHBO : SearchBaseBO
    {
        public long? ITEM_ID { get; set; }
        public string TEN_THUMUC { get; set; }
        public string TEN_TAILIEU { get; set; }
        public long USER_ID { get; set; }
    }
}
