using Business.CommonBusiness;
using Business.CommonModel.HSCVVANBANDEN;
using Business.CommonModel.HSCVVANBANDENDONVINHAN;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.HSCV_VANBANPHATHANHArea.Models
{
    public class HscvVanBanPhatHanhModel
    {
        public List<HSCV_VANBANDI> ListVanBan { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieu { get; set; }
        public HSCV_VANBANDEN VanBan { get; set; }
        public HSCV_VANBANDEN_BO VanBanBO { get; set; }
        public List<DM_DANHMUC_DATA> ListDoKhan { get; set; }
        public List<DM_DANHMUC_DATA> ListDoMat { get; set; }
        public List<DM_DANHMUC_DATA> ListDoUuTien { get; set; }
        public List<DM_DANHMUC_DATA> ListLinhVucVanBan { get; set; }
        public List<DM_DANHMUC_DATA> ListLoaiVanBan { get; set; }
        public string Extension { get; set; }
        public int MaxSize { get; set; }
        public PageListResultBO<HSCV_VANBANDEN_BO> ListResult { get; set; }
        public CCTCItemTreeBO TreeData { get; set; }
        public UserInfoBO UserInfoBO { get; set; }
        public List<CCTC_THANHPHAN> ListDonVi { get; set; }
        public CCTCItemTreeBO TreeDonVi { get; set; }
    }
    public class DonViNhanModel
    {
        public List<HSCV_VANBANDEN_DONVINHAN_BO> ListDonVi { get; set; }
        public HSCV_VANBANDEN VanBan { get; set; }
    }

}