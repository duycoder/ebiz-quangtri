using Business.Business;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.THUMUCLUUTRUArea.Models;
using Web.Custom;

namespace Web.Areas.THUMUCLUUTRUArea.Controllers
{
    public class DUNGLUONGLUUTRUController : BaseController
    {
        // GET: THUMUCLUUTRUArea/DUNGLUONGLUUTRU
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        private DUNGLUONG_LUUTRUBusiness DUNGLUONG_LUUTRUBusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private DMLoaiDonViBusiness DMLoaiDonViBusiness;
        public ActionResult Index()
        {
            var user = GetUserInfo();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DungLuongLuuTruModel model = new DungLuongLuuTruModel();
            var dataTree = CCTC_THANHPHANBusiness.GetTree(user.DM_PHONGBAN_ID.GetValueOrDefault(0));
            model.TreeData = dataTree;
            DMLoaiDonViBusiness = Get<DMLoaiDonViBusiness>();
            model.DS_TYPE = DMLoaiDonViBusiness.DSLoaiDonVi();
            return View(model);
        }
        #region  Các hàm private
        //private List<>
        #endregion
        #region Các hàm partialview
        public PartialViewResult GetUserPhongBan(int id)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var model = new DungLuongLuuTruModel();
            List<int> Ids = CCTC_THANHPHANBusiness.GetDSChild(id).Select(x => x.ID).ToList();
            Ids.Add(id);
            var lstUser = DM_NGUOIDUNGBusiness.GetByPhongBan(Ids, 0,new List<int>());
            var node = CCTC_THANHPHANBusiness.Find(id);
            model.Item = node;
            model.ListNguoiDung = lstUser;
            return PartialView("_DsNguoiDungPartial", model);
        }
        public PartialViewResult EditStorage(long id)
        {
            DungLuongLuuTruModel model = new DungLuongLuuTruModel();
            DUNGLUONG_LUUTRUBusiness = Get<DUNGLUONG_LUUTRUBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DUNGLUONG_LUUTRU Storage = DUNGLUONG_LUUTRUBusiness.GetDataByUser(id);
            if (Storage == null)
            {
                Storage = new DUNGLUONG_LUUTRU();
                Storage.DUNGLUONG = ThuMucLuuTruConstant.DefaultStorage;
                Storage.TYPE = ThuMucLuuTruConstant.DetaultType;
                Storage.USER_ID = 0;
            }
            model.Storage = Storage;
            DM_NGUOIDUNG NguoiDung = DM_NGUOIDUNGBusiness.Find(id);
            if (NguoiDung == null)
            {
                NguoiDung = new DM_NGUOIDUNG();
            }
            CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(NguoiDung.DM_PHONGBAN_ID);
            if (DonVi == null)
            {
                DonVi = new CCTC_THANHPHAN();
            }
            model.NguoiDung = NguoiDung;
            model.DonVi = DonVi;
            return PartialView("_SetupStorage", model);
        }
        #endregion
        #region Các hàm json
        public JsonResult ReloadPage()
        {
            var user = GetUserInfo();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var dataTree = CCTC_THANHPHANBusiness.GetTree(user.DM_PHONGBAN_ID.GetValueOrDefault(0));
            return Json(dataTree, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save(DUNGLUONG_LUUTRU Storage)
        {
            DUNGLUONG_LUUTRUBusiness = Get<DUNGLUONG_LUUTRUBusiness>();
            if (Storage.ID > 0)
            {
                #region Cập nhật thiết lập
                var result = DUNGLUONG_LUUTRUBusiness.Find(Storage.ID);
                if (result == null)
                {
                    return Json(new { Type = "ERROR", Message = "Không tìm thấy thiết lập cần cập nhật" });
                }
                result.DUNGLUONG = Storage.DUNGLUONG;
                result.TRANGTHAI = Storage.TRANGTHAI;
                result.TYPE = Storage.TYPE;
                DUNGLUONG_LUUTRUBusiness.Save(result);
                #endregion
            }
            else
            {
                #region Thêm mới thiết lập
                Storage.NGAYTAO = DateTime.Now;
                Storage.NGUOITAO = GetUserInfo().ID;
                DUNGLUONG_LUUTRUBusiness.Save(Storage);
                #endregion
            }
            return Json(new { Type = "SUCCESS", Message = "Thiết lập dung lượng lưu trữ thành công" });
        }
        #endregion
    }
}