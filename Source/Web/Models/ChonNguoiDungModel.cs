using Business.CommonBusiness;
using Business.CommonModel.DMNguoiDung;
using System.Collections.Generic;

namespace Web.Models
{
    public class ChonNguoiDungModel
    {
        public int PHONGBAN_ID { get; set; }
        public string TEXT_ID { get; set; }
        public string VALUE_ID { get; set; }
        public string ID_CLICK { get; set; }
        public int IS_MULTICHOICE { get; set; }
        public string[] IDS { get; set; }
        public string CALLBACK_FUNCTION { get; set; }
        public string TITLE { get; set; }
        public int INDEX { get; set; }
        public string SHOW_CHUC_VU_ID { get; set; }
        public List<DM_NGUOIDUNG_BO> LstNguoiDung { get; set; }
        public List<DM_NGUOIDUNG_BO> LstNguoiDungSearch { get; set; }
        public Dictionary<int, string> DictChucVu { get; set; }
        public string EXCLUDE_IDS { get; set; }
        public CCTCItemTreeBO TreeDonVi { get; set; }
        /// <summary>
        /// Dùng để loại trừ 1 đơn vị nào đó
        /// </summary>
        public int? EXCEPT_DEPT { get; set; }
        /// <summary>
        /// Lấy theo danh sách quyền
        /// </summary>
        public string ROLE { get; set; }
    }
}