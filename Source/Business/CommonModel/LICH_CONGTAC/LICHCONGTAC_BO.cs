using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
namespace Business.CommonModel.LICH_CONGTAC
{
    public class LICHCONGTAC_BO : LICHCONGTAC
    {
        public string TEN_NGUOICHUTRI { set; get; }
        public string TEN_LANHDAO { set; get; }
        public string NGAY_CONGTAC_TEXT { set; get; }
        public bool IS_OLD_WEEK { set; get; }
        public bool IS_REGISTERED_CAR { set; get; } //đã đăng ký xe
    }
}
