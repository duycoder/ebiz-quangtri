using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.EFILECHIASE;
using Business.CommonModel.TAILIEUDINHKEM;
using Business.CommonModel.THUMUCLUUTRU;
using CommonHelper;
using Model.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.THUMUCLUUTRUArea.Models;
using Web.Custom;
using Web.FwCore;

namespace Web.Areas.THUMUCLUUTRUArea.Controllers
{
    public class FollowingController : BaseController
    {
        //Danh sách tài liệu đang được chia sẻ
        // GET: THUMUCLUUTRUArea/Following
        private EFILE_CHIASEBusiness EFILE_CHIASEBusiness;
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private THUMUC_LUUTRUBusiness THUMUC_LUUTRUBusiness;
        private List<THUMUC_LUUTRU> lstPath = new List<THUMUC_LUUTRU>();
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        public ActionResult Index()
        {
            FollowingModel model = new FollowingModel();
            EFILE_CHIASE_SEARCHBO searchModel = new EFILE_CHIASE_SEARCHBO();
            searchModel.USER_ID = GetUserInfo().ID;
            EFILE_CHIASEBusiness = Get<EFILE_CHIASEBusiness>();
            model.ListChiaSe = EFILE_CHIASEBusiness.GetDaTaByPage(searchModel);
            SessionManager.SetValue("FollowingSearch", searchModel);
            return View(model);
        }
        #region Các hàm private
        private List<THUMUC_LUUTRU> GetPathURL(long? ID)
        {
            if (ID > 0)
            {
                THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
                List<THUMUC_LUUTRU> ListThuMuc = THUMUC_LUUTRUBusiness.GetAllParent(ID.HasValue ? ID.Value : 0);
                //ListThuMuc.Reverse();
                foreach (var item in ListThuMuc)
                {
                    lstPath.Add(item);
                }
            }
            return lstPath;
        }
        #endregion
        #region Các hàm partialview
        #endregion
        #region Các hàm Json
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            EFILE_CHIASEBusiness = Get<EFILE_CHIASEBusiness>();
            var searchModel = SessionManager.GetValue("FollowingSearch") as EFILE_CHIASE_SEARCHBO;
            if (searchModel == null)
            {
                searchModel = new EFILE_CHIASE_SEARCHBO();
            }
            if (!string.IsNullOrEmpty(sortQuery))
            {
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("FollowingSearch", searchModel);
            }
            searchModel.USER_ID = GetUserInfo().ID;
            var data = EFILE_CHIASEBusiness.GetDaTaByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }
        #endregion
        #region Các hàm khác
        public string GetURLBar(string pID)
        {
            lstPath.Clear();
            int ID = 0;
            int.TryParse(pID, out ID);
            lstPath = this.GetPathURL(ID);
            var allParent = JsonConvert.SerializeObject(lstPath);
            return allParent;
        }
        public string GetChild(string pid, string sort)
        {
            UserInfoBO user = GetUserInfo();
            List<THUMUC_LUUTRU_BO> subMenu = new List<THUMUC_LUUTRU_BO>();
            List<TAILIEUDINHKEM_BO> lstTaiLieu = new List<TAILIEUDINHKEM_BO>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            int pID = pid.ToIntOrZero();
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            EFILE_CHIASEBusiness = Get<EFILE_CHIASEBusiness>();
            THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(pID);
            if (THUMUC != null)
            {
                if (THUMUC.PERMISSION.HasValue && THUMUC.PERMISSION.Value == FolderPermission.CAN_WRITE)
                {
                    subMenu = new List<THUMUC_LUUTRU_BO>();
                }
                else
                {
                    CCTC_THANHPHAN DONVI = new CCTC_THANHPHAN();
                    if (THUMUC != null)
                    {
                        DONVI = CCTC_THANHPHANBusiness.Find(THUMUC.DONVI_ID);
                    }
                    else
                    {
                        DONVI = CCTC_THANHPHANBusiness.Find(user.DM_PHONGBAN_ID);
                    }
                    List<int> Ids = new List<int>();
                    #region Lấy tài liệu + thư mục con
                    subMenu = TAILIEUDINHKEMBusiness.getListFileByFolder(pID, LOAITAILIEU.TM_UPLOAD, 0, DONVI.NAME,THUMUC);
                    CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
                    subMenu.AddRange(THUMUC_LUUTRUBusiness.GetThuMucByParent(pID, 0, Ids));
                    #endregion
                }
            }
            return JsonConvert.SerializeObject(subMenu);
        }
        #endregion
    }
}