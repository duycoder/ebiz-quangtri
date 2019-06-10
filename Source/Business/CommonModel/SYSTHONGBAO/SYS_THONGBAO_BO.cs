using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.SYSTHONGBAO
{
    public class SYS_THONGBAO_BO:THONGBAO
    {
        public DM_NGUOIDUNG InfoNguoiGui { get; set; }
        public DM_NGUOIDUNG InfoNguoiNhan { get; set; }
    }
}
