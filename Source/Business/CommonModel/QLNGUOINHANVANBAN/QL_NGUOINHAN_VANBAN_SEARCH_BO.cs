using Business.BaseBusiness;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QLNGUOINHANVANBAN
{
    public class QL_NGUOINHAN_VANBAN_SEARCH_BO : SearchBaseBO
    {
        public int? QueryDeptId { set; get; }
        public string QueryName { set; get; }
        public List<DM_NGUOIDUNG> users { set; get; }
    }
}
