using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QLNGUOINHANVANBAN
{
    public class QL_NGUOINHAN_VANBAN_BO : QL_NGUOINHAN_VANBAN
    {
        public string TEN_PHONGBAN { get; set; }
        public List<DM_NGUOIDUNG> Users { set; get; }
        public string Members { set; get; }
    }
}
