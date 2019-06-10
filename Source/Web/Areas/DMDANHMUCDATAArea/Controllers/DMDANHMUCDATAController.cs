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
using Business.CommonModel.DMDANHMUCDATA;
using Web.Areas.DMDANHMUCDATAArea.Models;
using Web.Filter;
using System.IO;
using System.Web.Configuration;
using CommonHelper.Upload;
using CommonHelper.Excel;
using Web.Common;
using Business.CommonModel.CONSTANT;
using Web.Areas.DMNguoiDungArea.Models;

namespace Web.Areas.DMDANHMUCDATAArea.Controllers
{
    public class DMDANHMUCDATAController : BaseController
    {
        #region khaibao
        DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        DM_NHOMDANHMUCBusiness DM_NHOMDANHMUCBusiness;
        private string UPLOADFOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private string HostUpload = WebConfigurationManager.AppSettings["HostUpload"];
        private string MstDivisionExtension = WebConfigurationManager.AppSettings["MstDivisionExtension"];
        private string MstDivisionSize = WebConfigurationManager.AppSettings["MstDivisionSize"];
        private string URLNAVIGATION = WebConfigurationManager.AppSettings["URLNAVIGATION"];
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        #endregion
        public ActionResult Index(int id)
        {
            var model = new IndexVM();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            var searchmodel = new DM_DANHMUC_DATA_SEARCHBO();
            SessionManager.SetValue("dmdanhmucdataSearchModel", null);
            model.lstData = DM_DANHMUC_DATABusiness.GetDaTaByPage(id, null);
            model.DanhMuc = DM_NHOMDANHMUCBusiness.Find(id);
            return View(model);
        }
        [HttpPost]
        public JsonResult CheckExistValue(int idDM, int id, int value)
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var result = DM_DANHMUC_DATABusiness.ExistValue(idDM, value, id);
            return Json(result);
        }

        [HttpPost]
        public JsonResult getData(int id, int indexPage, string sortQuery, int pageSize)
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var searchModel = SessionManager.GetValue("dmdanhmucdataSearchModel") as DM_DANHMUC_DATA_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new DM_DANHMUC_DATA_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("dmdanhmucdataSearchModel", searchModel);
            }
            var data = DM_DANHMUC_DATABusiness.GetDaTaByPage(id, searchModel, indexPage, pageSize);
            return Json(data);
        }
        /// <summary>
        /// id danh mục
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult Create(int id)
        {
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            var CCBusiness = Get<CCTC_THANHPHANBusiness>();
            var model = new CreateVM();
            model.DanhMuc = DM_NHOMDANHMUCBusiness.Find(id);
            model.LstDept = CCBusiness.repository.All().Where(x => x.TYPE == 10).Select(x => new SelectListItem()
            {
                Value = x.ID.ToString(),
                Text = x.NAME,
            }).ToList();
            return PartialView("_CreatePartial", model);
        }
        [HttpPost]
        public JsonResult Create(FormCollection collection, string[] FOLDER_ID, string[] filename, IEnumerable<HttpPostedFileBase> filebase)
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var myobj = new DM_DANHMUC_DATA();
                myobj.DM_NHOM_ID = collection["DM_NHOM_ID"].ToIntOrZero();
                myobj.DATA = collection["DATA"].ToIntOrZero();
                myobj.TEXT = collection["TEXT"].ToString();
                myobj.CODE = collection["CODE"];
                myobj.COLOR = collection["COLOR"].ToString();
                myobj.GHICHU = collection["GHICHU"].ToString();
                myobj.DEPTID = collection["DEPTID"].ToIntOrNULL();
                DM_DANHMUC_DATABusiness.Save(myobj);
                if (filebase != null)
                {
                    if (filename == null)
                    {
                        filename = new string[filebase.Count()];
                    }
                    UploadFileTool tool = new UploadFileTool();
                    List<string> FileName = new List<string>();
                    List<string> FilePath = new List<string>();
                    List<long> FileId = new List<long>();
                    tool.UploadCustomFileAndOutPath(filebase, false, MstDivisionExtension, UPLOADFOLDER,
                        int.Parse(MstDivisionSize), FOLDER_ID, filename, myobj.ID,
                        out FilePath, out FileName, out FileName, out FileId, LOAITAILIEU.MST_DIVISION,
                        "Danh mục dùng chung");
                    if (FilePath.Any())
                    {
                        myobj.ICON = FilePath[0];
                        DM_DANHMUC_DATABusiness.Save(myobj);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Không thêm mới được";
            }
            return Json(result);
        }

        public PartialViewResult Edit(long id)
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var CCBusiness = Get<CCTC_THANHPHANBusiness>();
            var myModel = new EditVM();
            myModel.objModel = DM_DANHMUC_DATABusiness.repository.Find(id);
            myModel.UrlNavigation = URLNAVIGATION;
            myModel.LstDept = CCBusiness.repository.All().Where(x => x.TYPE == 10).Select(x => new SelectListItem()
            {
                Value = x.ID.ToString(),
                Text = x.NAME,
                Selected = myModel.objModel.DEPTID == x.ID
            }).ToList();
            return PartialView("_EditPartial", myModel);
        }

        public PartialViewResult Detail(int id)
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var myModel = DM_DANHMUC_DATABusiness.GetDaTaByID(id);
            return PartialView("_DetailPartial", myModel);
        }

        public JsonResult deleteIcon(int id)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            var tailieu = TAILIEUDINHKEMBusiness.repository.All()
                .Where(x => x.ITEM_ID == id && x.LOAI_TAILIEU == LOAITAILIEU.MST_DIVISION).FirstOrDefault();
            if (tailieu != null)
            {
                TAILIEUDINHKEMBusiness.repository.Delete(tailieu.TAILIEU_ID);
            }
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var dataObj = DM_DANHMUC_DATABusiness.Find(id);
            if (dataObj != null)
            {
                dataObj.ICON = null;
                DM_DANHMUC_DATABusiness.Save(dataObj);
            }
            var result = new JsonResultBO(true);
            return Json(result);
        }
        [HttpPost]
        public JsonResult Edit(FormCollection collection, string[] FOLDER_ID, string[] filename, IEnumerable<HttpPostedFileBase> filebase)
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var result = new JsonResultBO(true);
            AssignUserInfo();
            try
            {
                var id = collection["ID"].ToIntOrZero();
                var myobj = DM_DANHMUC_DATABusiness.Find(id);
                myobj.DATA = collection["DATA"].ToIntOrZero();
                myobj.TEXT = collection["TEXT"].ToString();
                myobj.CODE = collection["CODE"];
                myobj.GHICHU = collection["GHICHU"].ToString();
                myobj.COLOR = collection["COLOR"].ToString();
                myobj.DEPTID = collection["DEPTID"].ToIntOrNULL();
                if (filebase != null && filebase.FirstOrDefault() != null)
                {
                    FileUltilities file = new FileUltilities();
                    TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                    List<TAILIEUDINHKEM> ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(myobj.ID, LOAITAILIEU.MST_DIVISION);
                    foreach (var item in ListTaiLieu)
                    {
                        file.RemoveFile(UPLOADFOLDER + "/" + item.DUONGDAN_FILE);
                        TAILIEUDINHKEMBusiness.repository.Delete(item.TAILIEU_ID);
                    }
                    TAILIEUDINHKEMBusiness.Save();
                    if (filename == null)
                    {
                        filename = new string[filebase.Count()];
                    }
                    UploadFileTool tool = new UploadFileTool();
                    List<string> FileName = new List<string>();
                    List<string> FilePath = new List<string>();
                    List<long> FileId = new List<long>();
                    tool.UploadCustomFileAndOutPath(filebase, false, MstDivisionExtension, UPLOADFOLDER,
                        int.Parse(MstDivisionSize), FOLDER_ID, filename, myobj.ID,
                        out FilePath, out FileName, out FileName, out FileId, LOAITAILIEU.MST_DIVISION,
                        "Danh mục dùng chung");
                    if (FilePath.Any())
                    {
                        myobj.ICON = FilePath[0];
                    }
                }
                DM_DANHMUC_DATABusiness.Save(myobj);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Không cập nhật được";
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(long id)
        {
            var result = new JsonResultBO(true);
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            DM_DANHMUC_DATABusiness.repository.Delete(id);
            DM_DANHMUC_DATABusiness.Save();
            return Json(result);
        }
        /// <summary>
        /// Import theo nhóm danh mục
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Import(int id)
        {
            ImportVM model = new ImportVM();
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            model.DanhMuc = DM_NHOMDANHMUCBusiness.Find(id);
            model.PathTemplate = Path.Combine(HostUpload, WebConfigurationManager.AppSettings["ImportDanhMucTemplate"]);
            return View(model);
        }

        [HttpPost]
        public JsonResult CheckImport(int idNhomDanhMuc, HttpPostedFileBase fileImport)
        {
            JsonResultImportBO<DM_DANHMUC_DATA> result = new JsonResultImportBO<DM_DANHMUC_DATA>(true);
            DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
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
                var importHelper = new ImportExcelHelper<DM_DANHMUC_DATA>();
                importHelper.PathTemplate = saveFileResult.fullPath;
                importHelper.ConfigColumn = new List<ConfigModule>();
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "TEXT",
                    require = true,
                    TypeValue = typeof(string).FullName,
                });
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "DATA",
                    require = true,
                    TypeValue = typeof(int).FullName,
                });
                //importHelper.ConfigColumn.Add(new ConfigModule()
                //{
                //    columnName = "DEPTID",
                //    require = true,
                //    TypeValue = typeof(int).FullName,
                //});
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "CODE",
                    require = true,
                    TypeValue = typeof(string).FullName,
                });
                //importHelper.ConfigColumn.Add(new ConfigModule()
                //{
                //    columnName = "GHICHU",
                //    require = true,
                //    TypeValue = typeof(string).FullName,
                //});
                //var configTang = new ConfigModule()
                //{
                //    columnName = "GHICHU",
                //    require = false,
                //    TypeValue = typeof(string).FullName,
                //};
                var rsl = importHelper.Import();
                if (rsl.Status)
                {
                    result.Status = true;
                    result.Message = rsl.Message;
                    //foreach (var item in rsl.ListTrue.ToList())
                    //{
                    //    var checkresult = DM_DANHMUC_DATABusiness.ExistValue(idNhomDanhMuc, item.DATA.GetValueOrDefault());
                    //    if (checkresult.Status)
                    //    {
                    //        var listStrErr = new List<string>();
                    //        listStrErr.Add("0");
                    //        listStrErr.Add(item.TEXT);
                    //        listStrErr.Add(item.DATA.ToString());
                    //        listStrErr.Add(item.DEPTID.ToString());
                    //        listStrErr.Add(item.CODE.ToString());
                    //        listStrErr.Add(item.GHICHU.ToString());
                    //        listStrErr.Add("+ " + checkresult.Message);
                    //        rsl.lstFalse.Add(listStrErr);
                    //        rsl.ListTrue.Remove(item);
                    //    }

                    //}
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
        public JsonResult GetExportError(List<List<string>> lstData)
        {
            CommonHelper.ExportExcelHelper<DM_DANHMUC_DATA> exPro = new CommonHelper.ExportExcelHelper<DM_DANHMUC_DATA>();
            exPro.PathStore = Path.Combine(UPLOADFOLDER, "ErrorExport");
            exPro.PathTemplate = Path.Combine(UPLOADFOLDER, WebConfigurationManager.AppSettings["ImportDanhMucTemplate"]);
            exPro.StartRow = 5;
            exPro.StartCol = 2;
            exPro.FileName = "ErrorExportDanhMuc";
            var result = exPro.ExportText(lstData);
            if (result.Status)
            {
                result.PathStore = Path.Combine(HostUpload, "ErrorExport", result.FileName);
            }
            return Json(result);
        }


        [HttpPost]
        public JsonResult SaveImportData(int idNhomDanhMuc, List<List<string>> Data)
        {
            var result = new JsonResultBO(true);
            AssignUserInfo();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var lstObjSave = new List<DM_DANHMUC_DATA>();
            try
            {
                foreach (var item in Data)
                {
                    var newObj = new DM_DANHMUC_DATA();
                    newObj.TEXT = item[0];
                    newObj.DATA = item[1].ToIntOrNULL();
                    if (item[2] != null && item[2] != "null")
                    {
                        newObj.DEPTID = item[2].ToIntOrNULL();
                    }

                    newObj.CODE = item[3];
                    //newObj.GHICHU = item[3];
                    newObj.DM_NHOM_ID = idNhomDanhMuc;
                    lstObjSave.Add(newObj);
                }
                if (lstObjSave.Count > 0)
                {
                    result = DM_DANHMUC_DATABusiness.saveImport(lstObjSave);
                }
                else
                {
                    result.Status = false;
                    result.Message = "Không có dữ liệu để lưu";
                }
            }
            catch
            {
                result.Status = false;
                result.Message = "Lỗi dữ liệu, không thể import";
            }

            return Json(result);
        }
        public PartialViewResult PhanVaiTro(long id)
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            var DataVaiTroBusiness = Get<DM_DANHMUC_DATA_VAITROBusiness>();
            var myModel = new phanVaiTroVM();
            myModel.Data = DM_DANHMUC_DATABusiness.repository.Find(id);
            var listVaiTroData = DataVaiTroBusiness.GetListByData(id);
            myModel.DsVaiTro = DM_VAITROBusiness.DsVaiTro(listVaiTroData.Select(x => x.VAITRO_ID.Value).ToList());
            return PartialView("_phanVaiTro", myModel);
        }
    }
}

