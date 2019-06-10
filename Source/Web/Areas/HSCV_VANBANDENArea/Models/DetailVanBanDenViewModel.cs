using Business.CommonBusiness;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 
namespace Web.Areas.HSCV_VANBANDENArea.Models
{
    public class DetailVanBanDenViewModel
    {
        public HSCV_VANBANDEN entityVanBanDen { set; get; }
        public long currentUserId { set; get; }
        public string nameOfDonViGui { set; get; }
        public string nameOfLoaiVanBan { set; get; }
        public string nameOfLinhVucVanBan { set; get; }
        public string nameOfDoKhan { set; get; }
        public string nameOfDoUuTien { set; get; }
        public string nameOfLoaiCoQuan { set; get; }
        public string nameOfThongTinLoaiBan { set; get; }
        public string nameOfCongVanDen { set; get; }
        public List<TAILIEUDINHKEM> groupOfTaiLieuDinhKems { set; get; }
        public List<UserComment> groupOfComments { set; get; }
        public List<TAILIEUDINHKEM> groupOfCommentAttachments { set; get; }
        public int typeOfVanBanDen { set; get; }
        public bool isDuplicateCalendar { set; get; } //bị trùng lịch công tác
        public bool canCreateCalendar { set; get; } //có thể tạo lịch công tác
        public bool hasSteps { set; get; } //kiểm tra có bước xử lý văn bản hay không
    }
}