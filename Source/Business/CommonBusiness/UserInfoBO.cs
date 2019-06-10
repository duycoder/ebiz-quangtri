using System;
using System.Collections.Generic;
using Model.Entities;
using AE.Net.Mail;
using Business.CommonModel.DMNguoiDung;
using Business.CommonModel.DMCHUCNANG;

namespace Business.CommonBusiness
{
    [Serializable]
    public class UserInfoBO : DM_NGUOIDUNG_BO
    {
        public List<DM_CHUCNANG> ListChucNang;
        public List<DM_VAITRO> ListVaiTro;
        public List<DM_THAOTAC> ListThaoTac;
        public List<DM_CHUCNANG_BO> ListChucNangMenu { get; set; }
        public int? DeptParentID { get; set; }
        public List<UserBO> ListUserBO;
        public string ListUserName { get; set; }
        public bool CanSendSMS { set; get; } //gửi tin nhắn hệ thống
    }
}
