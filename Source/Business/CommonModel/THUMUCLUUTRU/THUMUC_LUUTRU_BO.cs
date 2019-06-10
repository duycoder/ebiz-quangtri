using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.THUMUCLUUTRU
{
    public class THUMUC_LUUTRU_BO : THUMUC_LUUTRU
    {
        public string TEN_DONVI { get; set; }
        public string TEN_NGUOITAO { get; set; }
        public bool IS_THUMUC { get; set; }
        public string THUMUCCHA { get; set; }
        public int SLTHUMUC { get; set; }
    }
}
