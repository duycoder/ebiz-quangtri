using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.SYSTINNHAN
{
    public class SYS_TINNHAN_BO : SYS_TINNHAN
    {
        public string TEN_NGUOIGUI { get; set; }
        public string TEN_NGUOINHAN { get; set; }
        public SYS_TINNHAN ToModel()
        {
            SYS_TINNHAN TinNhan = new SYS_TINNHAN();
            TinNhan.FROM_USERNAME = this.FROM_USERNAME;
            TinNhan.FROM_USER_ID = this.FROM_USER_ID;
            TinNhan.ID = this.ID;
            TinNhan.IS_READ = this.IS_READ;
            TinNhan.NGAYTAO = this.NGAYTAO;
            TinNhan.NGUOITAO = this.NGUOITAO;
            TinNhan.NOIDUNG = this.NOIDUNG;
            TinNhan.TIEUDE = this.TIEUDE;
            TinNhan.TO_USERNAME = this.TO_USERNAME;
            TinNhan.TO_USER_ID = this.TO_USER_ID;
            TinNhan.URL = this.URL;
            return TinNhan;
        }
    }
}
