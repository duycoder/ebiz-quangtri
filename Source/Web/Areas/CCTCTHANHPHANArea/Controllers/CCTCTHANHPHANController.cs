using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonHelper;
using Business.Business;
using Business.CommonBusiness;
using Model.Entities;
using Web.Custom;
using Web.FwCore;
using Business.CommonModel.CCTCTHANHPHAN;
using Web.Areas.CCTCTHANHPHANArea.Models;
using Web.Areas.CoCauToChucArea.Models;
using Business.CommonModel.DMNguoiDung;
using Web.Filter;
using Business.CommonModel.CONSTANT;
using System.IO;
using System.Web.Configuration;
using CommonHelper.Upload;
using CommonHelper.Excel;

namespace Web.Areas.CCTCTHANHPHANArea.Controllers
{
    public class CCTCTHANHPHANController : BaseController
    {
        #region KhaiBao
        CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        DMLoaiDonViBusiness DMLoaiDonViBusiness;
        DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        DM_NHOMDANHMUCBusiness DM_NHOMDANHMUCBusiness;

        private string UPLOADFOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private string HostUpload = "/Uploads";
        #endregion

        public ActionResult TaoSoVanBanTheoNam(int year, string code)
        {
            AssignUserInfo();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            var Nhom = DM_NHOMDANHMUCBusiness.repository.All().Where(x => x.GROUP_CODE == code).FirstOrDefault();
            if (Nhom != null)
            {
                string TenSo = Nhom.GROUP_NAME + " " + year.ToString();
                var LstTypeLabel = CCTC_THANHPHANBusiness.repository.All().Where(x => x.TYPE == 10).ToList();
                foreach (var item in LstTypeLabel)
                {
                    var checkExist = DM_DANHMUC_DATABusiness.repository.All().Where(x => x.DEPTID == item.ID && x.DATA == year && x.TEXT == TenSo).FirstOrDefault();
                    if (checkExist == null)
                    {
                        DM_DANHMUC_DATA model = new DM_DANHMUC_DATA();
                        model.DM_NHOM_ID = Nhom.ID;
                        model.TEXT = TenSo;
                        model.DATA = year;
                        model.DEPTID = item.ID;
                        DM_DANHMUC_DATABusiness.Save(model);
                    }
                }
            }
            return Redirect("/");
        }
        //[CodeAllowAccess(Code = "DsCCTC")]

        public ActionResult Index()
        {
            AssignUserInfo();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DMLoaiDonViBusiness = Get<DMLoaiDonViBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();

            CCTC_THANHPHAN_SEARCHBO searchModel = new CCTC_THANHPHAN_SEARCHBO();
            SessionManager.SetValue("CctcThanhPhanSearch", searchModel);

            CoCauToChucIndexModel viewModel = new CoCauToChucIndexModel()
            {
                GroupData = CCTC_THANHPHANBusiness.GetDataByPage(searchModel, currentUser),
                DS_TYPE = DMLoaiDonViBusiness.DSLoaiDonVi(),
                DS_CATEGORY = DM_DANHMUC_DATABusiness.DsByMaNhom("DMCAPPHONGBAN", currentUser.ID)
            };

            return View(viewModel);
        }

        /// <summary>
        /// @author:duynn
        /// @description: tìm kiếm phòng ban
        /// @since: 03/06/2019
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchData(FormCollection form)
        {
            AssignUserInfo();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var searchModel = (CCTC_THANHPHAN_SEARCHBO)SessionManager.GetValue("CctcThanhPhanSearch") ?? new CCTC_THANHPHAN_SEARCHBO();
            searchModel.pageSize = 20;
            searchModel.QR_MAPHONGBAN = form["QR_MAPHONGBAN"];
            searchModel.QR_LOAIPHONGBAN = form["QR_LOAIPHONGBAN"].ToIntOrNULL();
            searchModel.QR_CAPPHONGBAN = form["QR_CAPPHONGBAN"].ToIntOrNULL();
            searchModel.QR_TENPHONGBAN = form["QR_TENPHONGBAN"];
            SessionManager.SetValue("CctcThanhPhanSearch", searchModel);
            var data = CCTC_THANHPHANBusiness.GetDataByPage(searchModel, currentUser, 1, searchModel.pageSize);
            return Json(data);
        }

        /// <summary>
        /// @author:duynn
        /// @description: tìm kiếm cơ cấu tổ chức thành phần
        /// @since: 14/06/2019
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetData(int pageIndex, string sortQuery, int pageSize)
        {
            AssignUserInfo();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var searchModel = (CCTC_THANHPHAN_SEARCHBO)SessionManager.GetValue("CctcThanhPhanSearch") ?? new CCTC_THANHPHAN_SEARCHBO();
            if (!string.IsNullOrEmpty(sortQuery))
            {
                searchModel.sortQuery = sortQuery;
            }
            if (pageSize > 0)
            {
                searchModel.pageSize = pageSize;
            }
            SessionManager.SetValue("CctcThanhPhanSearch", searchModel);
            var data = CCTC_THANHPHANBusiness.GetDataByPage(searchModel, currentUser, pageIndex, pageSize);
            return Json(data);
        }

        /// <summary>
        /// @author:duynn
        /// @description: xóa phòng ban
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delele(int id)
        {
            JsonResultBO result = new JsonResultBO(true);
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            try
            {
                CCTC_THANHPHANBusiness.repository.Delete(id);
                CCTC_THANHPHANBusiness.Save();
                return Json(result);
            }
            catch
            {
                result.Status = false;
                result.Message = "Xóa phòng ban thành công";
                return Json(result);
            }
        }


        /// <summary>
        /// @author:duynn
        /// @description: gửi phòng ban
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(FormCollection form)
        {
            AssignUserInfo();
            JsonResultBO result = new JsonResultBO(true);
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            try
            {
                var id = form["ID"].ToIntOrZero();
                var dept = CCTC_THANHPHANBusiness.Find(id) ?? new CCTC_THANHPHAN();
                var parent = CCTC_THANHPHANBusiness.Find(dept.PARENT_ID);
                dept.PARENT_ID = form["PARENT_ID"].ToIntOrNULL();
                dept.NAME = form["NAME"]?.Trim();
                dept.CODE = form["CODE"]?.Trim();

                var existCode = CCTC_THANHPHANBusiness.CheckExistCode(dept.CODE, id);
                if (existCode)
                {
                    result.Status = false;
                    result.Message = "Mã phòng ban đã tồn tại";
                    return Json(result);
                }

                var existName = CCTC_THANHPHANBusiness.CheckExistName(dept.NAME, id);
                if (existName)
                {
                    result.Status = false;
                    result.Message = "Tên phòng ban đã tồn tại";
                    return Json(result);
                }
                
                dept.TYPE = form["TYPE"].ToIntOrZero();
                dept.CATEGORY = form["CATEGORY"].ToIntOrZero();

                dept.THUTU = form["THUTU"].ToIntOrZero();
                dept.ITEM_LEVEL = parent.ITEM_LEVEL + 1;
                dept.NGUOITAO = currentUser.ID;
                dept.NGAYTAO = DateTime.Now;
                if (dept.TYPE == 10)
                {
                    dept.CAN_SEND_SMS = form["CAN_SEND_SMS"].ToIntOrZero() > 0;
                }
                CCTC_THANHPHANBusiness.Save(dept);
                return Json(result);
            }
            catch
            {
                result.Status = false;
                result.Message = "Cập nhật phòng ban không thành công";
                return Json(result);
            }
        }

        /// <summary>
        /// @author:duynn
        /// @description: lấy danh sách người dùng thuộc phòng ban
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult GetListUsers(int deptId)
        {
            //DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            //var searchModel = new DM_NGUOIDUNG_

            //var data = DM_NGUOIDUNGBusiness.GetUser
            return PartialView("_ListUsers");
        }


        //public JsonResult ReloadPage()
        //{
        //    var user = (UserInfoBO)SessionManager.GetUserInfo();
        //    CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
        //    var dataTree = CCTC_THANHPHANBusiness.GetTree(user.DM_PHONGBAN_ID.GetValueOrDefault(0));
        //    return Json(dataTree, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetNode(int id)
        {
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var node = CCTC_THANHPHANBusiness.Find(id);
            int order = CCTC_THANHPHANBusiness.repository.All().Max(x => x.THUTU).GetValueOrDefault();
            order++;
            return Json(new { node = node, order = order }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetUser(int id)
        //{
        //    DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
        //    CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
        //    var model = new CoCauToChucNguoiDungModel();
        //    var lstUser = DM_NGUOIDUNGBusiness.GetByPhongBan(id);

        //    var node = CCTC_THANHPHANBusiness.Find(id);
        //    model.Item = node;
        //    model.ListNguoiDung = lstUser;
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        public PartialViewResult GetUserPhongBan(int id)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var model = new CoCauToChucNguoiDungModel();
            var lstUser = DM_NGUOIDUNGBusiness.GetByPhongBan(id);

            var node = CCTC_THANHPHANBusiness.Find(id);
            model.Item = node;
            model.ListNguoiDung = lstUser;
            return PartialView("_DsNguoiDungPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchNguoiDung(FormCollection form)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var searchModel = SessionManager.GetValue("NguoiDungSearchModel") as DM_NGUOIDUNG_SEARCHBO;

            if (searchModel == null)
            {
                searchModel = new DM_NGUOIDUNG_SEARCHBO();
                searchModel.pageSize = 20;
            }
            searchModel.sea_HoTen = form["sea_HOTEN"];
            searchModel.sea_TenDangNhap = form["sea_TENDANGNHAP"];
            searchModel.sea_Email = form["sea_EMAIL"];
            searchModel.sea_DienThoai = form["sea_DIENTHOAI"];
            SessionManager.SetValue("NguoiDungSearchModel", searchModel);

            var data = DM_NGUOIDUNGBusiness.GetUserTuDoByPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }

        //Lấy dữ liệu người dùng
        public JsonResult getDataND(int indexPage, string sortQuery, int pageSize)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var searchModel = SessionManager.GetValue("NguoiDungSearchModel") as DM_NGUOIDUNG_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new DM_NGUOIDUNG_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("NguoiDungSearchModel", searchModel);
            }
            var data = DM_NGUOIDUNGBusiness.GetUserTuDoByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }

        public PartialViewResult ChuyenPhongBanModal(long id)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var ngdung = DM_NGUOIDUNGBusiness.Find(id);
            var model = new ChuyenPhongBanVM();
            model.nguoiDung = ngdung;
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            model.listPhongBan = CCTC_THANHPHANBusiness.GetDropDownList();
            return PartialView("_chuyenPhongModel", model);
        }


        [HttpPost]
        public JsonResult ChuyenBan(long id, int idphongban)
        {
            var result = new JsonResultBO(true);
            try
            {
                DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                var ngdung = DM_NGUOIDUNGBusiness.Find(id);
                ngdung.DM_PHONGBAN_ID = idphongban;
                DM_NGUOIDUNGBusiness.Save(ngdung);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không thể chuyển phòng ban nhân viên này.";
            }
            return Json(result);
        }


        [HttpPost]
        public JsonResult xoaNgDungPhongBan(long id)
        {
            var result = new JsonResultBO(true);
            try
            {
                DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                var ngdung = DM_NGUOIDUNGBusiness.Find(id);
                ngdung.DM_PHONGBAN_ID = null;
                DM_NGUOIDUNGBusiness.Save(ngdung);

            }
            catch
            {
                result.Status = false;
                result.Message = "Không thể chuyển phòng ban nhân viên này.";
            }
            return Json(result);
        }


        [HttpPost]
        public JsonResult saveUserPhongBan(List<long> lstUser, int idphongban)
        {
            var result = new JsonResultBO(true);
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            foreach (var item in lstUser)
            {
                var obj = DM_NGUOIDUNGBusiness.Find(item);
                if (obj != null)
                {
                    obj.DM_PHONGBAN_ID = idphongban;
                    DM_NGUOIDUNGBusiness.Save(obj);
                }
            }
            return Json(result);
        }


        public PartialViewResult newUserModal(int id)
        {
            var model = new NewNguoiDungVM();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            model.PhongBan = CCTC_THANHPHANBusiness.Find(id);
            model.lstNguoiDung = DM_NGUOIDUNGBusiness.GetUserTuDoByPage(null);
            return PartialView("_NewUserPhongBanPartial", model);
        }

        public JsonResult getDS(int id)
        {
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var node = CCTC_THANHPHANBusiness.repository.All().Where(x => x.ID != id).OrderBy(x => x.ITEM_LEVEL).ThenBy(x => x.ID).ToList();
            return Json(node, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetParent(int id)
        {
            AssignUserInfo();
            CoCauToChucUpdateModel model = new CoCauToChucUpdateModel();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var item = CCTC_THANHPHANBusiness.Find(id);
            var list = CCTC_THANHPHANBusiness.GetAllByLeVelUp(item.ITEM_LEVEL.ToString().ToIntOrZero()).Where(x => x.ID != id).ToList();
            model.DS_PARENT = list;
            model.Item = item;
            DMLoaiDonViBusiness = Get<DMLoaiDonViBusiness>();
            model.DS_TYPE = DMLoaiDonViBusiness.DSLoaiDonVi(item.TYPE);
            model.DS_CATEGORY = DM_DANHMUC_DATABusiness.DsByMaNhom("DMCAPPHONGBAN", currentUser.ID, item.CATEGORY.HasValue ? item.CATEGORY.Value : 0);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public JsonResult CheckHasChild(int id)
        {
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            return Json(CCTC_THANHPHANBusiness.ExistChild(id));
        }

        public PartialViewResult Import(int id)
        {
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DMLoaiDonViBusiness = Get<DMLoaiDonViBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var model = new ImportCCTC();
            model.objectModel = CCTC_THANHPHANBusiness.Find(id);
            model.order = CCTC_THANHPHANBusiness.repository.All().Max(x => x.THUTU).GetValueOrDefault();
            model.order = model.order++;

            model.objectModel = CCTC_THANHPHANBusiness.Find(id);
            model.DS_TYPE = DMLoaiDonViBusiness.DSLoaiDonVi(model.objectModel.TYPE);
            model.DS_CATEGORY = DM_DANHMUC_DATABusiness.DsByMaNhom("DMCAPPHONGBAN", currentUser.ID, model.objectModel.CATEGORY.HasValue ? model.objectModel.CATEGORY.Value : 0);

            model.PathTemplate = Path.Combine(HostUpload, WebConfigurationManager.AppSettings["ImportCCTCTemplate"]);

            return PartialView("_ImportPartial", model);
        }

        [HttpPost]
        public JsonResult CheckImport(HttpPostedFileBase fileImport)
        {
            JsonResultImportBO<DM_CCTC_IMPORTBO> result = new JsonResultImportBO<DM_CCTC_IMPORTBO>(true);

            //Kiểm tra file có tồn tại k?
            if (fileImport == null)
            {
                result.Status = false;
                result.Message = "Không có file đọc dữ liệu";
                return Json(result);
            }
            //Lưu file upload để đọc
            var saveFileResult = UploadProvider.SaveFile(fileImport, null, ".xls,.xlsx", null, "TempImportFile", UPLOADFOLDER);
            if (!saveFileResult.status)
            {
                result.Status = false;
                result.Message = saveFileResult.message;
                return Json(result);
            }
            else
            {
                var importHelper = new ImportExcelHelper<DM_CCTC_IMPORTBO>();
                importHelper.PathTemplate = saveFileResult.fullPath;
                importHelper.ConfigColumn = new List<ConfigModule>();
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "LOAI",
                    require = true,
                    TypeValue = typeof(int).FullName,
                });
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "CAPPHONGBAN",
                    require = true,
                    TypeValue = typeof(int).FullName,
                });
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "TENPHONGBAN",
                    require = true,
                    TypeValue = typeof(string).FullName,
                });
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "MACCTC",
                    require = false,
                    TypeValue = typeof(string).FullName,
                });
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "THUTU",
                    require = false,
                    TypeValue = typeof(int).FullName,
                });

                var rsl = importHelper.Import();
                if (rsl.Status)
                {
                    result.Status = true;
                    result.Message = rsl.Message;
                    result.ListData = rsl.ListTrue;
                    result.ListFalse = rsl.lstFalse;
                }
                else
                {
                    result.Status = false;
                    result.Message = rsl.Message;
                }

            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult SaveImportData(List<List<string>> Data, int id_parent)
        {
            var CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var LstCoCau = CCTC_THANHPHANBusiness.GetDropDownList();
            DMLoaiDonViBusiness = Get<DMLoaiDonViBusiness>();
            var result = new JsonResultBO(true);
            AssignUserInfo();
            var CheckUpdate = false;
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var model = new ImportCCTC();
            model.objectModel = CCTC_THANHPHANBusiness.Find(id_parent);
            model.DS_TYPE = DMLoaiDonViBusiness.DSLoaiDonVi(model.objectModel.TYPE);
            model.DS_CATEGORY = DM_DANHMUC_DATABusiness.DsByMaNhom("DMCAPPHONGBAN", currentUser.ID, model.objectModel.CATEGORY.HasValue ? model.objectModel.CATEGORY.Value : 0);

            model.ListCode = CCTC_THANHPHANBusiness.repository.All().Select(x => new SelectListItem()
            {
                Text = x.CODE,
                Value = x.ID.ToString(),

            }).ToList();

            model.ListTenPhong = CCTC_THANHPHANBusiness.repository.All().Select(x => new SelectListItem()
            {
                Text = x.NAME,
                Value = x.ID.ToString(),

            }).ToList();

            var lstObjSave = new List<CCTC_THANHPHAN>();
            try
            {
                foreach (var item in Data)
                {
                    var tmpCode = item[3];
                    var StrTenPhong = item[2];
                    if (string.IsNullOrEmpty(tmpCode))
                    {
                        break;
                    }
                    var newObj = CCTC_THANHPHANBusiness.repository.All().Where(x => x.CODE == tmpCode.Trim() && x.NAME == StrTenPhong.Trim())
                        .FirstOrDefault();
                    if (newObj == null)
                    {
                        newObj = new CCTC_THANHPHAN();
                    }
                    newObj.TYPE = item[0].ToIntOrZero();
                    newObj.CATEGORY = item[1].ToIntOrZero();
                    newObj.ITEM_LEVEL = model.objectModel.ITEM_LEVEL + 1;
                    newObj.PARENT_ID = id_parent;

                    if (!string.IsNullOrEmpty(StrTenPhong))
                    {
                        var DataTenPhongObj = model.ListTenPhong.Where(x => x.Text.ToLower().Equals(StrTenPhong.ToLower()))
                            .FirstOrDefault();
                        if (DataTenPhongObj == null)
                        {
                            newObj.NAME = StrTenPhong;
                        }
                    }

                    if (!string.IsNullOrEmpty(tmpCode))
                    {
                        var DataCodeObj = model.ListCode.Where(x => x.Text.ToLower().Equals(tmpCode.ToLower()))
                            .FirstOrDefault();
                        if (DataCodeObj == null)
                        {
                            newObj.CODE = tmpCode;
                        }

                    }
                    newObj.THUTU = item[4].ToIntOrZero();

                    if (newObj.ID > 0)
                    {
                        CCTC_THANHPHANBusiness.Save(newObj);
                        CheckUpdate = true;
                    }
                    else
                    {
                        lstObjSave.Add(newObj);
                    }

                }
                if (lstObjSave.Count > 0)
                {
                    result = CCTC_THANHPHANBusiness.saveImport(lstObjSave);
                }
                else
                {
                    if (CheckUpdate == false)
                    {
                        result.Status = false;
                        result.Message = "Không có dữ liệu để lưu";
                    }
                    else
                    {
                        result.Status = true;
                        result.Message = "Cập nhật dữ liệu thành công";
                    }
                }
            }
            catch
            {
                result.Status = false;
                result.Message = "Lỗi dữ liệu, không thể import";
            }

            return Json(result);
        }
    }
}

