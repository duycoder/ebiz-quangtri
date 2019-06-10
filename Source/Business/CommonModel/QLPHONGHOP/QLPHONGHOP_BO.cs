using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Business.CommonModel.QLPHONGHOP
{
    public class QLPHONGHOP_BO:QUANLY_PHONGHOP
    {
        public string TENPHONG { set; get; }
        public string TEN_LANHDAO { set; get; }
        public string TEN_NGUOITAO { set; get; }
    }
}
