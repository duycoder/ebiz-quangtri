using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.DUNGLUONGLUUTRU;
using Business.CommonModel.EFILECHIASE;
using Business.CommonModel.TAILIEUDINHKEM;
using Business.CommonModel.THUMUCLUUTRU;
using CommonHelper;
using Model.Entities;
using Newtonsoft.Json;
using NUnrar.Archive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml.Linq;
//using GramMultiThread.Controllers;
using Web.Areas.THUMUCLUUTRUArea.Models;
using Web.Common;
using Web.Common.Elastic;
using Web.Custom;
using Web.FwCore;
using Web.Models;

using MongoDB.Bson;
using MongoDB.Driver;
using NAudio.Wave;
using Nest;
using FormCollection = System.Web.Mvc.FormCollection;

namespace Web.Areas.THUMUCLUUTRUArea.Controllers
{
    public class THUMUCLUUTRUController : BaseController
    {
        // GET: THUMUCLUUTRUArea/THUMUCLUUTRU
        #region Khai báo biến
        private THUMUC_LUUTRUBusiness THUMUC_LUUTRUBusiness;
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private TAILIEUDINHKEM_VERSIONBusiness TAILIEUDINHKEM_VERSIONBusiness;
        private List<THUMUC_LUUTRU_BO> lstFolder = new List<THUMUC_LUUTRU_BO>();
        private List<THUMUC_LUUTRU> lstPath = new List<THUMUC_LUUTRU>();
        //private List<THUMUC_LUUTRU> thumuc = new List<THUMUC_LUUTRU>();
        private List<string> arrFolder = new List<string>();
        private string URL_FOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private string URL_TRASH = WebConfigurationManager.AppSettings["FileUploadTrash"];
        private int MaxSize = int.Parse(WebConfigurationManager.AppSettings["FILEMAXSIZE"]);
        private string Extension = WebConfigurationManager.AppSettings["FILEEXTENSION"];
        private string UrlNavigation = WebConfigurationManager.AppSettings["URLNAVIGATION"];
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private DM_NHOMDANHMUCBusiness DM_NHOMDANHMUCBusiness;
        private LOAITAILIEU_THUOCTINHBusiness LOAITAILIEU_THUOCTINHBusiness;
        private TAILIEU_THUOCTINHBusiness TAILIEU_THUOCTINHBusiness;
        private EFILE_CHIASEBusiness EFILE_CHIASEBusiness;
        private EFILE_CHIASE_NGUOIDUNGBusiness EFILE_CHIASE_NGUOIDUNGBusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private DUNGLUONG_LUUTRUBusiness DUNGLUONG_LUUTRUBusiness;
        #endregion

        public string TestMergingMp3()
        {
            string dirPath = @"E:/03.HiNET_Project/2018/Doji/SOURCE/Web/Uploads/ListSpeech/";
            FileInfo[] files = new DirectoryInfo(dirPath).GetFiles(); // 6 audio files to merge
            foreach (var file in files)
            {
                using (var f = file.Open(FileMode.Open))
                {
                    byte[] bytes = new byte[f.Length];
                    using (FileStream isfs = new FileStream(dirPath + "test.mp3", FileMode.OpenOrCreate))
                    {
                        isfs.Seek(0, SeekOrigin.End); //go to the last byte
                        while (f.Read(bytes, 0, bytes.Length) > 0)
                        {
                            isfs.Write(bytes, 0, bytes.Length);
                            isfs.Flush();
                        }
                        isfs.Close();
                    }
                }
            }
            return "Done";
        }
        //public async System.Threading.Tasks.Task<ActionResult> TestMongoDB()
        //{
        //    //var testcontent = GetDocXInnerText("E:/03.HiNET_Project/2018/Doji/SOURCE/Web/Uploads/test.pdf");
        //    string connectionString = "mongodb://localhost"; 
        //    MongoClient client = new MongoClient(connectionString);
        //    IMongoDatabase db = client.GetDatabase("detaikhoahoc");
        //    IMongoCollection<ElasticBCAModel> collection = db.GetCollection<ElasticBCAModel>("detaikhoahoc");
        //    GramController model = new GramController();
        //    var lstngram = model.GetNGramFromFile("E:/03.HiNET_Project/2018/Doji/SOURCE/Web/Uploads/test.docx");
        //    BsonDocument query_project = new BsonDocument
        //    {
        //        {
        //            "$project", new BsonDocument
        //            {
        //                {
        //                    "counttotal", new BsonDocument
        //                    {
        //                        {
        //                            "$size",new BsonDocument
        //                            {
        //                                { "$setIntersection", new BsonArray
        //                                    {   "$lstngram",
        //                                        new BsonArray(lstngram.ToArray())
        //                                    }
        //                                }
        //                            }
        //                        }
                                
        //                    }
        //                },
        //                { "iddetai", "$detaiid" }
        //            }
        //        }
        //    };
        //    BsonDocument query_sort = new BsonDocument
        //    {
        //        {
        //            "$sort", new BsonDocument
        //            {
        //                { "counttotal",-1}
        //            }
        //        }
        //    };
        //    BsonDocument query_limit = new BsonDocument
        //    {
        //        {
        //            "$limit", 10
        //        }
        //    };
        //    var pipeline = new[] { query_project, query_sort, query_limit };

        //    var lstDedicateDocuments = collection.Aggregate<BsonDocument>(pipeline).ToList();
            
        //    return Redirect("/");
        //}
        public ActionResult Index()
        {
            SessionManager.Remove("ListThuMuc");
            SessionManager.Remove("ThuMucTable");
            ThuMucLuuTruModel model = new ThuMucLuuTruModel();
            var lstAllThuMuc = initDataByPage(null);
            var lstThuMuc = lstAllThuMuc.ListItem;
            SessionManager.SetValue("ThuMucTable", lstThuMuc);
            SessionManager.SetValue("ListThuMuc", lstThuMuc);
            model.ListThuMuc = lstAllThuMuc;
            model.ListAccessModifier = initAccessModifier(0);
            model.ListFolderPermission = initFolderPermission(0);
            model.userInfoBo = GetUserInfo();
            model.AccessModifer = GetAccessFile(model.userInfoBo);
            return View(model);
        }

        public string GetDocXInnerText(string docxFilepath)
        {
            string folder = Path.GetDirectoryName(docxFilepath);
            string extractionFolder = folder + "\\extraction";

            if (Directory.Exists(extractionFolder))
                Directory.Delete(extractionFolder, true);

            System.IO.Compression.ZipFile.ExtractToDirectory(docxFilepath, extractionFolder);
            string xmlFilepath = extractionFolder + "\\word\\document.xml";

            var xmldoc = new XmlDocument();
            xmldoc.Load(xmlFilepath);

            return xmldoc.DocumentElement.InnerText;
        }
        public FileResult DownloadFilePath(long ID)
        {
            TAILIEUDINHKEM_VERSIONBusiness = Get<TAILIEUDINHKEM_VERSIONBusiness>();
            TAILIEUDINHKEM_VERSION VERSION = TAILIEUDINHKEM_VERSIONBusiness.Find(ID);
            if (VERSION != null)
            {
                string contentType = VERSION.DINHDANG_FILE;
                string path = URL_FOLDER + VERSION.DUONGDAN_FILE;
                if (System.IO.File.Exists(path))
                {
                    var filename = path.Split('\\');
                    string fileSave = filename[filename.Count() - 1];
                    return File(path, contentType, fileSave);
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        public FileResult DownloadZipFile(long? id)
        {
            //THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            //THUMUC_LUUTRU folder = THUMUC_LUUTRUBusiness.Find(id);
            //UserInfoBO userInfo = (UserInfoBO)SessionManager.GetUserInfo();
            ////Kiểm tra quyền thao tác với thư mục
            //if (folder != null)
            //{
            //    EFILE_CHIASEBusiness = Get<EFILE_CHIASEBusiness>();
            //    var ListChiaSe = EFILE_CHIASEBusiness.GetFolderByUserId(userInfo.ID);
            //    List<long> Ids = THUMUC_LUUTRUBusiness.GetAllChildren(id.HasValue ? id.Value : 0).Select(x => x.ID).ToList();
            //    Ids.Add(folder.ID);
            //    if (folder.USER_ID != userInfo.ID && !Ids.Contains(folder.ID))
            //    {
            //        return null;
            //    }
            //    UploadFileTool tool = new UploadFileTool();
            //    string uploads = URL_FOLDER + "\\";
            //    string pathname = "";
            //    CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            //    if (folder.USER_ID != userInfo.ID)
            //    {
            //        DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            //        DM_NGUOIDUNG NguoiDung = DM_NGUOIDUNGBusiness.Find(folder.USER_ID);
            //        if (NguoiDung == null)
            //        {
            //            return null;
            //        }
            //        List<CCTC_THANHPHAN> ListDonVi = CCTC_THANHPHANBusiness.GetDSParent(NguoiDung.DM_PHONGBAN_ID.HasValue ? NguoiDung.DM_PHONGBAN_ID.Value : 0);
            //        ListDonVi.Reverse();
            //        foreach (var item in ListDonVi)
            //        {
            //            pathname += item.NAME + "\\";
            //        }
            //        pathname += (NguoiDung != null ? NguoiDung.TENDANGNHAP : userInfo.TENDANGNHAP) + "\\eFile\\";
            //    }
            //    else
            //    {
            //        List<CCTC_THANHPHAN> ListDonVi = CCTC_THANHPHANBusiness.GetDSParent(userInfo.DM_PHONGBAN_ID.HasValue ? userInfo.DM_PHONGBAN_ID.Value : 0);
            //        ListDonVi.Reverse();
            //        foreach (var item in ListDonVi)
            //        {
            //            pathname += item.NAME + "\\";
            //        }
            //        pathname += userInfo.TENDANGNHAP + "\\eFile\\";
            //    }
            //    //string virtualPath = tool.GetPath(userInfo.DonViID, userInfo.PhongBanID, userInfo.CoSoID) + "\\eFile\\";
            //    uploads += pathname;
            //    //Đường dẫn đến thư mục hiện tại
            //    var lstFolderPath = GetAllParent(folder.PARENT_ID.HasValue ? folder.PARENT_ID.Value : 0);
            //    lstFolderPath.Reverse();
            //    foreach (var item in lstFolderPath)
            //    {
            //        uploads += item;
            //    }
            //    uploads += "\\" + folder.TENTHUMUC + "\\";
            //    ZipFile zip = new ZipFile();
            //    //zip.UseUnicodeAsNecessary = true;
            //    //zip.ProvisionalAlternateEncoding = System.Text.Encoding.GetEncoding(866);
            //    zip.AlternateEncodingUsage = ZipOption.Always;
            //    zip.AlternateEncoding = Encoding.UTF8;
            //    zip.AddDirectory(uploads);
            //    var archive = Server.MapPath("~/" + folder.TENTHUMUC + ".zip");
            //    if (System.IO.File.Exists(archive))
            //    {
            //        System.IO.File.Delete(archive);
            //    }
            //    zip.Save(archive);
            //    return File(archive, "application/zip", folder.TENTHUMUC + ".zip");
            //}
            return null;
        }
        public ActionResult Trash()
        {
            var lstAllThuMuc = initDataByPage(null);
            return View(lstAllThuMuc.ListItem);
        }
        #region Các hàm private
        private List<THUMUC_LUUTRU_BO> GetAllThuMuc(int Nam)
        {
            UserInfoBO user = GetUserInfo();
            List<THUMUC_LUUTRU_BO> all = new List<THUMUC_LUUTRU_BO>();
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            all = THUMUC_LUUTRUBusiness.GetDataByPhong(user.ID, Nam);
            all = all.OrderBy(x => x.TENTHUMUC).ToList();
            return all;
        }
        private List<string> GetAllChildren(long? ID, long USER_ID)
        {
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            List<THUMUC_LUUTRU> thumuc = THUMUC_LUUTRUBusiness.GetChildren(ID);
            if (thumuc.Count > 0)
            {
                arrFolder.Add(thumuc.Count.ToString());
            }
            return arrFolder;
        }
        private List<string> GetAllParent(long ID)
        {
            if (ID > 0)
            {
                THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
                List<THUMUC_LUUTRU> ListThuMuc = THUMUC_LUUTRUBusiness.GetAllParent(ID);
                ListThuMuc.Reverse();
                //THUMUC_LUUTRU ThuMuc = THUMUC_LUUTRUBusiness.Find(ID);
                //if (ThuMuc != null)
                //{
                //    arrFolder.Add(ThuMuc.TENTHUMUC);
                //}
                foreach (var item in ListThuMuc)
                {
                    arrFolder.Add(item.TENTHUMUC);
                }
            }
            return arrFolder;
        }
        private PageListResultBO<THUMUC_LUUTRU_BO> initDataByPage(THUMUC_LUUTRU_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            return THUMUC_LUUTRUBusiness.GetDataByPage(searchModel, pageIndex, pageSize);
        }
        private PageListResultBO<THUMUC_LUUTRU_BO> initTrashByPage(THUMUC_LUUTRU_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            return THUMUC_LUUTRUBusiness.GetDataByPage(searchModel, pageIndex, pageSize);
        }
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
        private List<SelectListItem> initAccessModifier(int accessId)
        {
            List<SelectListItem> lstAccess = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            #region Cấp hệ thống
            item.Text = "Thuộc tập đoàn";
            item.Value = AccessModifier.ALL_SYSTEM.ToString();
            item.Selected = AccessModifier.ALL_SYSTEM == accessId;
            lstAccess.Add(item);
            #endregion
            #region Cấp cơ quan
            item = new SelectListItem();
            item.Text = "Thuộc đơn vị";
            item.Value = AccessModifier.ALL_UNIT.ToString();
            item.Selected = AccessModifier.ALL_UNIT == accessId;
            lstAccess.Add(item);
            #endregion
            #region Cấp phòng
            item = new SelectListItem();
            item.Text = "Thuộc phòng ban";
            item.Value = AccessModifier.ALL_DEPARTMENT.ToString();
            item.Selected = AccessModifier.ALL_DEPARTMENT == accessId;
            lstAccess.Add(item);
            #endregion
            #region Cá nhân hóa
            item = new SelectListItem();
            item.Text = "Cá nhân";
            item.Value = AccessModifier.PRIVATE.ToString();
            item.Selected = AccessModifier.PRIVATE == accessId;
            lstAccess.Add(item);
            #endregion
            switch (accessId)
            {
                case AccessModifier.PRIVATE:
                    lstAccess = lstAccess.Where(x => x.Value.Equals(AccessModifier.PRIVATE.ToString())).ToList();
                    break;
                case AccessModifier.ALL_DEPARTMENT:
                    lstAccess = lstAccess.Where(x => x.Value.Equals(AccessModifier.PRIVATE.ToString())
                    || x.Value.Equals(AccessModifier.ALL_DEPARTMENT.ToString())).ToList();
                    break;
                case AccessModifier.ALL_UNIT:
                    lstAccess = lstAccess.Where(x => x.Value.Equals(AccessModifier.PRIVATE.ToString())
                    || x.Value.Equals(AccessModifier.ALL_DEPARTMENT.ToString())
                    || x.Value.Equals(AccessModifier.ALL_UNIT.ToString())).ToList();
                    break;
            }
            int AccessFolder = GetAccessFile(GetUserInfo());
            switch (AccessFolder)
            {
                case AccessModifier.ALL_DEPARTMENT:
                    lstAccess = lstAccess.Where(x => x.Value.Equals(AccessModifier.PRIVATE.ToString())
                    || x.Value.Equals(AccessModifier.ALL_DEPARTMENT.ToString())).ToList();
                    break;
                case AccessModifier.ALL_UNIT:
                    lstAccess = lstAccess.Where(x => x.Value.Equals(AccessModifier.PRIVATE.ToString())
                    || x.Value.Equals(AccessModifier.ALL_DEPARTMENT.ToString())
                    || x.Value.Equals(AccessModifier.ALL_UNIT.ToString())).ToList();
                    break;
                case AccessModifier.ALL_SYSTEM:
                    lstAccess = lstAccess.Where(x => x.Value.Equals(AccessModifier.PRIVATE.ToString())
                    || x.Value.Equals(AccessModifier.ALL_SYSTEM.ToString())
                    || x.Value.Equals(AccessModifier.ALL_DEPARTMENT.ToString())
                    || x.Value.Equals(AccessModifier.ALL_UNIT.ToString())).ToList();
                    break;
                default:
                    lstAccess = lstAccess.Where(x => x.Value.Equals(AccessModifier.PRIVATE.ToString())).ToList();
                    break;
            }
            return lstAccess;
        }
        private List<DM_DANHMUC_DATA> initLoaiTaiLieu()
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            return DM_DANHMUC_DATABusiness.GetData(ThuMucLuuTruConstant.LoaiTaiLieu);
        }
        private List<SelectListItem> initFolderPermission(int folderPermission)
        {
            List<SelectListItem> lstPermission = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "Chỉ đọc";
            item.Value = FolderPermission.CAN_READ.ToString();
            item.Selected = FolderPermission.CAN_READ == folderPermission;
            lstPermission.Add(item);
            item = new SelectListItem();
            item.Text = "Chỉ ghi";
            item.Value = FolderPermission.CAN_WRITE.ToString();
            item.Selected = FolderPermission.CAN_WRITE == folderPermission;
            lstPermission.Add(item);
            item = new SelectListItem();
            item.Text = "Đọc và ghi";
            item.Value = FolderPermission.BOTH.ToString();
            item.Selected = (FolderPermission.BOTH == folderPermission || (folderPermission == 0));
            lstPermission.Add(item);
            return lstPermission;
        }
        private static List<long> initDefaultId()
        {
            List<long> Ids = new List<long>();
            Ids.Add(ThuMucLuuTruConstant.DefaultCongViec);
            Ids.Add(ThuMucLuuTruConstant.DefaultDept);
            Ids.Add(ThuMucLuuTruConstant.DefaultPrivate);
            Ids.Add(ThuMucLuuTruConstant.DefaultSys);
            Ids.Add(ThuMucLuuTruConstant.DefaultUnit);
            Ids.Add(ThuMucLuuTruConstant.DefaultVanban);
            Ids.Add(ThuMucLuuTruConstant.DefaultVbDen);
            return Ids;
        }
        public List<string> initClass()
        {
            List<string> ListClass = new List<string>();
            ListClass.Add(CommonFolder.IF_FOLDER_BLUE);
            ListClass.Add(CommonFolder.IF_FOLDER_CHECKED);
            ListClass.Add(CommonFolder.IF_FOLDER_CLOSE);
            ListClass.Add(CommonFolder.IF_FOLDER_GREEN);
            ListClass.Add(CommonFolder.IF_FOLDER_HOUSE);
            ListClass.Add(CommonFolder.IF_FOLDER_INFORMATION);
            ListClass.Add(CommonFolder.IF_FOLDER_LOCKEDC);
            ListClass.Add(CommonFolder.IF_FOLDER_MUSIC);
            ListClass.Add(CommonFolder.IF_FOLDER_OPEN);
            ListClass.Add(CommonFolder.IF_FOLDER_PICTURE);
            ListClass.Add(CommonFolder.IF_FOLDER_REMOVE);
            ListClass.Add(CommonFolder.IF_FOLDER_SEARCH);
            ListClass.Add(CommonFolder.IF_FOLDER_VIDEO);
            return ListClass;
        }
        private string initFolderPath(UserInfoBO userInfo, long parentId)
        {
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            string pathname = "";
            List<CCTC_THANHPHAN> ListDonVi = CCTC_THANHPHANBusiness.GetDSParent(userInfo.DM_PHONGBAN_ID.HasValue ? userInfo.DM_PHONGBAN_ID.Value : 0);
            ListDonVi.Reverse();
            foreach (var item in ListDonVi)
            {
                pathname += item.NAME + "\\";
            }
            pathname += userInfo.TENDANGNHAP + "\\eFile";
            if (parentId > 0)
            {
                arrFolder.Clear();
                arrFolder = GetAllParent(parentId);
                arrFolder.Reverse();
                for (int i = 0; i < arrFolder.Count; i++)
                {
                    pathname += "\\" + arrFolder[i];
                }
            }
            return pathname;
        }
        private DUNGLUONG_LUUTRU_BO initDungLuong(UserInfoBO userInfo)
        {
            DUNGLUONG_LUUTRUBusiness = Get<DUNGLUONG_LUUTRUBusiness>();
            DUNGLUONG_LUUTRU DungLuong = DUNGLUONG_LUUTRUBusiness.GetDataByUser(userInfo.ID);
            if (DungLuong == null)
            {
                DungLuong = new DUNGLUONG_LUUTRU();
                DungLuong.DUNGLUONG = ThuMucLuuTruConstant.DefaultStorage;
                DungLuong.TYPE = ThuMucLuuTruConstant.DetaultType;
            }
            DUNGLUONG_LUUTRU_BO DungLuongBO = new DUNGLUONG_LUUTRU_BO();
            DungLuongBO.CONLAI = 0;
            DungLuongBO.DUNGLUONG = DungLuong.DUNGLUONG;
            DungLuongBO.NGAYTAO = DungLuong.NGAYTAO;
            DungLuongBO.ID = DungLuong.ID;
            DungLuongBO.NGUOITAO = DungLuong.NGUOITAO;
            DungLuongBO.TEN_DONVI = "";
            DungLuongBO.TEN_NHANVIEN = "";
            DungLuongBO.TRANGTHAI = DungLuong.TRANGTHAI;
            DungLuongBO.TYPE = DungLuong.TYPE;
            DungLuongBO.USER_ID = DungLuong.USER_ID;
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            List<long> Ids = THUMUC_LUUTRUBusiness.GetIdsByUserId(ThuMucLuuTruConstant.DefaultPrivate, userInfo.ID);
            //Mặc định kích cỡ file được lưu là Kilobyte
            long DaDung = TAILIEUDINHKEMBusiness.CountStorageSize(Ids) / 1024;
            long Capacity = DungLuong.DUNGLUONG.HasValue ? DungLuong.DUNGLUONG.Value : ThuMucLuuTruConstant.DefaultStorage;
            #region Quy đổi dung lượng lưu trữ ra đơn vị tính là Mb
            switch (DungLuong.TYPE)
            {
                case ThuMucLuuTruConstant.Terabyte:
                    Capacity = Capacity * 1048576;
                    break;
                case ThuMucLuuTruConstant.Gigabyte:
                    Capacity = Capacity * 1024;
                    break;
                //case ThuMucLuuTruConstant.Megabyte:
                //    Capacity = Capacity;
                //    break;
                case ThuMucLuuTruConstant.Kilobyte:
                    Capacity = Capacity / 1024;
                    break;
            }
            #endregion
            DungLuongBO.CONLAI = Capacity - DaDung;
            DungLuongBO.DUNGLUONG = Capacity;
            return DungLuongBO;
        }
        private int GetAccessFile(UserInfoBO userInfo)
        {
            if (IsInActivities(userInfo.ListThaoTac, AccessModifier.ACCESS_SYSTEM))
            {
                return AccessModifier.ALL_SYSTEM;
            }
            else if (IsInActivities(userInfo.ListThaoTac, AccessModifier.ACCESS_UNIT))
            {
                return AccessModifier.ALL_UNIT;
            }
            else if (IsInActivities(userInfo.ListThaoTac, AccessModifier.ACCESS_DEPT))
            {
                return AccessModifier.ALL_DEPARTMENT;
            }
            return AccessModifier.PRIVATE;
        }
        public List<long> GetAllPrivateChild()
        {
            return new List<long>();
            //THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            //return THUMUC_LUUTRUBusiness.GetAllChildren(ThuMucLuuTruConstant.DefaultPrivate).Select(x => x.ID).ToList();
        }
        #endregion
        #region Các hàm partialview
        public PartialViewResult EditFolder(long id, bool isEdit)
        {
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            ThuMucLuuTruModel model = new ThuMucLuuTruModel();
            THUMUC_LUUTRU ThuMuc = THUMUC_LUUTRUBusiness.Find(id);
            if (ThuMuc == null)
            {
                ThuMuc = new THUMUC_LUUTRU();
            }
            if (!isEdit)
            {
                ThuMuc.ID = 0;
                ThuMuc.PARENT_ID = id;
                ThuMuc.TENTHUMUC = "";
            }
            ThuMuc.IS_DELETE = false;
            model.ThuMuc = ThuMuc;
            List<long> Ids = initDefaultId();
            if (Ids.Contains(id))
            {
                model.ListAccessModifier = initAccessModifier(0);
            }
            else
            {
                model.ListAccessModifier = initAccessModifier(ThuMuc.ACCESS_MODIFIER.HasValue ? ThuMuc.ACCESS_MODIFIER.Value : 0);
            }
            model.ListLoaiTaiLieu = initLoaiTaiLieu();
            model.ListFolderPermission = initFolderPermission(ThuMuc.PERMISSION.HasValue ? ThuMuc.PERMISSION.Value : 0);
            model.userInfoBo = GetUserInfo();
            model.ListClass = initClass();
            model.Ids = initDefaultId();
            return PartialView("_CreateFolder", model);
        }
        public PartialViewResult ReloadTree()
        {
            var lstAllThuMuc = initDataByPage(null);
            var lstThuMuc = lstAllThuMuc.ListItem;
            SessionManager.SetValue("ThuMucTable", lstThuMuc);
            return PartialView("_ThuMucLeft");
        }
        public PartialViewResult UploadFileSingle(string FOLDER_ID)
        {
            //TmLoaiVanBanBusiness = Get<TmLoaiVanBanBusiness>();
            //TaiLieuDungLuongLuuTruBusiness = Get<TaiLieuDungLuongLuuTruBusiness>();
            ThuMucLuuTruModel model = new ThuMucLuuTruModel();
            //model.TAILIEU = TmLoaiVanBanBusiness.GetData().Select(x => new SelectListItem
            //{
            //    Value = x.ID.ToString(),
            //    Text = x.LOAITAILIEU
            //}).ToList();
            ViewData["FOLDER_ID"] = FOLDER_ID;
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(FOLDER_ID.ToLongOrZero());
            if (THUMUC != null)
            {
                model.ThuMuc = THUMUC;
            }
            model.FileSize = MaxSize;
            model.Extension = Extension;
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            model.ListLoaiTaiLieu = initLoaiTaiLieu();
            //model.ListTrangThai = InitTrangThaiTaiLieu();
            model.DungLuong = initDungLuong(user);
            return PartialView("_UploadFileSingle", model);
        }
        public PartialViewResult RenameFile(long ID)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            TAILIEUDINHKEM model = null;
            model = TAILIEUDINHKEMBusiness.Find(ID);
            if (model == null)
            {
                model = new TAILIEUDINHKEM();
            }
            return PartialView("_RenameFile", model);
        }
        public PartialViewResult UploadFile(string FOLDER_ID)
        {
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            try
            {
                //SessionManager.SetValue("ListDanhMuc", null);
                //TmLoaiVanBanBusiness = Get<TmLoaiVanBanBusiness>();
                ThuMucLuuTruModel model = new ThuMucLuuTruModel();
                //model.TAILIEU = TmLoaiVanBanBusiness.GetData().Select(x => new SelectListItem
                //{
                //    Value = x.ID.ToString(),
                //    Text = x.LOAITAILIEU
                //}).ToList();
                ViewData["FOLDER_ID"] = FOLDER_ID;
                THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
                THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(FOLDER_ID.ToLongOrZero());
                if (THUMUC != null)
                {
                    model.ThuMuc = THUMUC;
                }
                else
                {
                    return null;
                }
                model.FileSize = MaxSize;
                model.Extension = Extension;
                model.DungLuong = initDungLuong(user);
                return PartialView("_UploadFile", model);
            }
            catch
            {
                return null;
            }
        }
        public PartialViewResult UploadZipFile(long folderId)
        {
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            ThuMucLuuTruModel model = new ThuMucLuuTruModel();
            model.Extension = Extension;
            model.FileSize = (this.MaxSize / 1048676);
            model.FolderId = folderId;
            THUMUC_LUUTRU folder = THUMUC_LUUTRUBusiness.Find(folderId);
            if (folder == null || folder.USER_ID != user.ID)
            {
                return null;
            }
            //model.lstAccessModifier = initAccessModifierChild(folder.ACCESS_MODIFIER_ID);
            model.ThuMuc = folder;
            model.DungLuong = initDungLuong(user);
            return PartialView("_UploadZipFile", model);
        }
        public PartialViewResult DragDropFile(long folderId)
        {
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            ThuMucLuuTruModel model = new ThuMucLuuTruModel();
            model.Extension = Extension;
            model.FileSize = (MaxSize / 1048676);
            model.FolderId = folderId;
            THUMUC_LUUTRU folder = THUMUC_LUUTRUBusiness.Find(folderId);
            if (folder == null)
            {
                return null;
            }
            model.DungLuong = initDungLuong(GetUserInfo());
            //model.lstAccessModifier = initAccessModifierChild(folder.ACCESS_MODIFIER_ID);
            return PartialView("_DragDropFiles", model);
        }
        public PartialViewResult FileDetail(string TAILIEU, string OPTION)
        {
            try
            {
                long TAILIEU_ID = TAILIEU.ToLongOrZero();
                TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
                UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
                ThuMucLuuTruModel model = new ThuMucLuuTruModel();
                TAILIEUDINHKEM TAILIEUDK = TAILIEUDINHKEMBusiness.Find(TAILIEU_ID);
                if (TAILIEUDK == null)
                {
                    TAILIEUDK = new TAILIEUDINHKEM();
                }
                model.TaiLieu = TAILIEUDK;
                model.UrlNavigation = UrlNavigation;
                TAILIEUDINHKEM_VERSIONBusiness = Get<TAILIEUDINHKEM_VERSIONBusiness>();
                model.ListVersion = TAILIEUDINHKEM_VERSIONBusiness.FindDataByTaiLieu(TAILIEU_ID);
                TAILIEU_THUOCTINHBusiness = Get<TAILIEU_THUOCTINHBusiness>();
                if (OPTION == "chitiet")
                {
                    //Khi chỉ xem
                    model.IsDetail = true;
                    model.ListThuocTinhBO = TAILIEU_THUOCTINHBusiness.GetDataBO(TAILIEU_ID);
                    DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
                    DM_DANHMUC_DATA LoaiTaiLieu = DM_DANHMUC_DATABusiness.Find(TAILIEUDK.DM_LOAITAILIEU_ID);
                    model.LoaiTaiLieu = LoaiTaiLieu;
                }
                else
                {
                    model.IsDetail = false;
                    model.ListThuocTinhBO = TAILIEU_THUOCTINHBusiness.GetDataBO(TAILIEU_ID);
                    model.ListLoaiTaiLieu = initLoaiTaiLieu();
                }
                ViewData["FOLDER_ID"] = TAILIEU;
                THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(TAILIEUDK.FOLDER_ID);
                if (THUMUC == null)
                {
                    THUMUC = new THUMUC_LUUTRU();
                }
                model.ThuMuc = THUMUC;
                return PartialView("_FileDetail", model);
            }
            catch
            {
                return PartialView("_FileDetail");
            }
        }
        public PartialViewResult VersionFile(long TAILIEU)
        {
            TAILIEUDINHKEM_VERSIONBusiness = Get<TAILIEUDINHKEM_VERSIONBusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            ThuMucLuuTruModel model = new ThuMucLuuTruModel();
            model.ListVersion = TAILIEUDINHKEM_VERSIONBusiness.FindDataByTaiLieu(TAILIEU);
            model.TaiLieu = TAILIEUDINHKEMBusiness.Find(TAILIEU);
            model.DungLuong = initDungLuong(GetUserInfo());
            return PartialView("_QuanLyPhienBan", model);
        }
        /// <summary>
        /// Lấy danh sách thuộc tính của tài liệu theo loại tài liệu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult LoadThuocTinh(int id)
        {
            LOAITAILIEU_THUOCTINHBusiness = Get<LOAITAILIEU_THUOCTINHBusiness>();
            TaiLieuThuocTinhModel model = new TaiLieuThuocTinhModel();
            model.ListThuocTinh = LOAITAILIEU_THUOCTINHBusiness.GetData(id);
            return PartialView("_DanhSachThuocTinh", model);
        }
        /// <summary>
        /// Hàm chia sẻ file hoặc folder
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        public PartialViewResult Sharing(long id, bool isFolder)
        {
            UserInfoBO userInfoBO = GetUserInfo();
            ThuMucLuuTruModel model = new ThuMucLuuTruModel();
            EFILE_CHIASEBusiness = Get<EFILE_CHIASEBusiness>();
            EFILE_CHIASE_NGUOIDUNGBusiness = Get<EFILE_CHIASE_NGUOIDUNGBusiness>();
            if (isFolder)
            {
                THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
                THUMUC_LUUTRU ThuMuc = THUMUC_LUUTRUBusiness.Find(id);
                if (ThuMuc == null)
                {
                    ThuMuc = new THUMUC_LUUTRU();
                }
                model.ThuMuc = ThuMuc;
                model.ListFolderPermission = initFolderPermission(ThuMuc.PERMISSION.HasValue ? ThuMuc.PERMISSION.Value : 0);
                model.TaiLieu = new TAILIEUDINHKEM();
            }
            else
            {
                TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                TAILIEUDINHKEM TaiLieu = TAILIEUDINHKEMBusiness.Find(id);
                if (TaiLieu == null)
                {
                    TaiLieu = new TAILIEUDINHKEM();
                }
                model.ThuMuc = new THUMUC_LUUTRU();
                model.ListFolderPermission = initFolderPermission(TaiLieu.PERMISSION.HasValue ? TaiLieu.PERMISSION.Value : 0);
                model.TaiLieu = TaiLieu;
            }
            model.DONVI_ID = userInfoBO.DM_PHONGBAN_ID.HasValue ? userInfoBO.DM_PHONGBAN_ID.Value : 0;
            model.IsFolder = isFolder;
            EFILE_CHIASEBusiness = Get<EFILE_CHIASEBusiness>();
            model.ListChiaSe = EFILE_CHIASEBusiness.GetData(id, isFolder);
            model.userInfoBo = userInfoBO;
            return PartialView("_Sharing", model);
        }
        public PartialViewResult EditShare(long id)
        {
            ThuMucLuuTruModel model = new ThuMucLuuTruModel();
            EFILE_CHIASEBusiness = Get<EFILE_CHIASEBusiness>();
            EFILE_CHIASE ChiaSe = EFILE_CHIASEBusiness.Find(id);
            if (ChiaSe == null)
            {
                ChiaSe = new EFILE_CHIASE();
            }
            model.ChiaSe = ChiaSe;
            if (ChiaSe.IS_FOLDER.HasValue && ChiaSe.IS_FOLDER.Value)
            {
                THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
                THUMUC_LUUTRU ThuMuc = THUMUC_LUUTRUBusiness.Find(ChiaSe.ITEM_ID);
                if (ThuMuc == null)
                {
                    ThuMuc = new THUMUC_LUUTRU();
                }
                model.ThuMuc = ThuMuc;
                model.TaiLieu = new TAILIEUDINHKEM();
            }
            else
            {
                TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                TAILIEUDINHKEM TaiLieu = TAILIEUDINHKEMBusiness.Find(id);
                if (TaiLieu == null)
                {
                    TaiLieu = new TAILIEUDINHKEM();
                }
                model.ThuMuc = new THUMUC_LUUTRU();
                model.TaiLieu = TaiLieu;
            }
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            DM_NGUOIDUNG NguoiDung = DM_NGUOIDUNGBusiness.Find(ChiaSe.USER_ID);
            if (NguoiDung == null)
            {
                NguoiDung = new DM_NGUOIDUNG();
            }
            model.NguoiDung = NguoiDung;
            model.ListFolderPermission = initFolderPermission(ChiaSe.PERMISSION.HasValue ? ChiaSe.PERMISSION.Value : 0);
            return PartialView("_EditShare", model);
        }
        //public PartialViewResult UpdatePriovity(long id)
        //{
        //    ThuMucLuuTruModel model = new ThuMucLuuTruModel();
        //    return PartialView("_UpdatePriovity", model);
        //}
        #endregion
        #region Các hàm json
        [ValidateInput(false)]
        public JsonResult FolderChecking(string name, long? parentID, long id)
        {
            AssignUserInfo();
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            bool canInsert = true;
            switch (parentID)
            {
                case ThuMucLuuTruConstant.DefaultPrivate:
                case ThuMucLuuTruConstant.DefaultSys:
                case ThuMucLuuTruConstant.DefaultDept:
                case ThuMucLuuTruConstant.DefaultUnit:
                    //Nếu là thư mục to nhất thì check theo người dùng
                    canInsert = THUMUC_LUUTRUBusiness.CanInsert(name, parentID.HasValue ? parentID.Value : 0, id, currentUser.ID);
                    break;
                default:
                    //còn lại ko check
                    canInsert = THUMUC_LUUTRUBusiness.CanInsert(name, parentID.HasValue ? parentID.Value : 0, id, currentUser.ID);
                    break;
            }
            if (canInsert)
            {
                return Json(new { Type = "SUCCESS", Message = "Chưa có thư mục nào được tạo với tên như này" });
            }
            return Json(new { Type = "ERROR", Message = "Tên thư mục này đã được sử dụng" });
        }
        public JsonResult SaveThuMuc(THUMUC_LUUTRU ThuMuc, FormCollection col)
        {
            if (!ThuMuc.PARENT_ID.HasValue)
            {
                return Json(new { Type = "ERROR", Message = "Bạn chưa chọn phạm vi truy cập cho thư mục" }, JsonRequestBehavior.AllowGet);
            }
            List<long> Ids = initDefaultId();
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            UserInfoBO userInfoBO = GetUserInfo();
            //if (!ThuMuc.USER_ID.HasValue || ThuMuc.USER_ID != userInfoBO.ID)
            //{
            //    return Json(new { Type = "ERROR", Message = "Bạn không có quyền thực hiện chức năng này" }, JsonRequestBehavior.AllowGet);
            //}
            arrFolder.Clear();
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            FileUltilities file = new FileUltilities();
            string msg = "";
            string pathname = "";
            List<CCTC_THANHPHAN> ListDonVi = CCTC_THANHPHANBusiness.GetDSParent(userInfoBO.DM_PHONGBAN_ID.HasValue ? userInfoBO.DM_PHONGBAN_ID.Value : 0);
            ListDonVi.Reverse();
            foreach (var item in ListDonVi)
            {
                pathname += item.NAME + "\\";
            }
            pathname += userInfoBO.TENDANGNHAP + "\\eFile";
            if (ThuMuc.ID > 0)
            {
                #region Cập nhật thư mục
                msg = "Cập nhật thư mục thành công";
                var result = THUMUC_LUUTRUBusiness.Find(ThuMuc.ID);
                if (result == null)
                {
                    return Json(new { Type = "ERROR", Message = "Không tìm thấy thư mục cần cập nhật" }, JsonRequestBehavior.AllowGet);
                }
                if (result.USER_ID.HasValue && result.USER_ID.Value != userInfoBO.ID && result.PERMISSION.HasValue
                    && result.PERMISSION.Value == FolderPermission.CAN_READ)
                {
                    return Json(new { Type = "ERROR", Message = "Bạn không có quyền thao tác đến thư mục này" }, JsonRequestBehavior.AllowGet);
                }
                result.TENTHUMUC = string.IsNullOrEmpty(ThuMuc.TENTHUMUC) ? ThuMuc.TENTHUMUC : ThuMuc.TENTHUMUC.Trim();
                result.ACCESS_MODIFIER = ThuMuc.ACCESS_MODIFIER;
                result.IS_FIXED = false;
                result.PERMISSION = ThuMuc.PERMISSION;
                result.CLASS = ThuMuc.CLASS;
                THUMUC_LUUTRUBusiness.Save(result);
                #endregion
            }
            else
            {
                #region Thêm mới thư mục

                if (ThuMuc.PARENT_ID.HasValue && Ids.Contains(ThuMuc.PARENT_ID.Value))
                {
                    switch (ThuMuc.ACCESS_MODIFIER)
                    {
                        case AccessModifier.PRIVATE:
                            ThuMuc.PARENT_ID = ThuMucLuuTruConstant.DefaultPrivate;
                            ThuMuc.FIXED_FOLDER_ID = ThuMucLuuTruConstant.DefaultPrivate;
                            break;
                        case AccessModifier.ALL_DEPARTMENT:
                            ThuMuc.PARENT_ID = ThuMucLuuTruConstant.DefaultDept;
                            ThuMuc.FIXED_FOLDER_ID = ThuMucLuuTruConstant.DefaultDept;
                            break;
                        case AccessModifier.ALL_SYSTEM:
                            ThuMuc.PARENT_ID = ThuMucLuuTruConstant.DefaultSys;
                            ThuMuc.FIXED_FOLDER_ID = ThuMucLuuTruConstant.DefaultSys;
                            break;
                        case AccessModifier.ALL_UNIT:
                            ThuMuc.PARENT_ID = ThuMucLuuTruConstant.DefaultUnit;
                            ThuMuc.FIXED_FOLDER_ID = ThuMucLuuTruConstant.DefaultUnit;
                            break;
                    }
                }
                else if (ThuMuc.PARENT_ID.HasValue || ThuMuc.PARENT_ID.Value == 0)
                {
                    if (ThuMuc.PARENT_ID.Value == 0)
                    {
                        switch (ThuMuc.ACCESS_MODIFIER)
                        {
                            case AccessModifier.PRIVATE:
                                ThuMuc.PARENT_ID = ThuMucLuuTruConstant.DefaultPrivate;
                                break;
                            case AccessModifier.ALL_DEPARTMENT:
                                ThuMuc.PARENT_ID = ThuMucLuuTruConstant.DefaultDept;
                                break;
                            case AccessModifier.ALL_SYSTEM:
                                ThuMuc.PARENT_ID = ThuMucLuuTruConstant.DefaultSys;
                                break;
                            case AccessModifier.ALL_UNIT:
                                ThuMuc.PARENT_ID = ThuMucLuuTruConstant.DefaultUnit;
                                break;
                        }
                    }
                    THUMUC_LUUTRU Parent = THUMUC_LUUTRUBusiness.Find(ThuMuc.PARENT_ID);
                    if (Parent == null)
                    {
                        return Json(new { Type = "ERROR", Message = "Không tìm thấy thư mục cha" });
                    }
                    ThuMuc.FIXED_FOLDER_ID = Parent.FIXED_FOLDER_ID;
                }
                if (ThuMuc.PARENT_ID > 0)
                {
                    arrFolder = GetAllParent(ThuMuc.PARENT_ID.HasValue ? ThuMuc.PARENT_ID.Value : 0);
                    //arrFolder.Reverse();
                    foreach (string s in arrFolder)
                    {
                        pathname += "\\" + s;
                    }
                    file.CreateFolder(URL_FOLDER + "\\" + pathname + "\\" + ThuMuc.TENTHUMUC.Trim());
                }
                else
                {
                    file.CreateFolder(URL_FOLDER + "\\" + pathname + "\\" + ThuMuc.TENTHUMUC.Trim());
                }
                msg = "Thêm mới thư mục thành công";
                ThuMuc.NGAYTAO = DateTime.Now;
                ThuMuc.USER_ID = userInfoBO.ID;
                ThuMuc.DONVI_ID = userInfoBO.DM_PHONGBAN_ID;
                ThuMuc.IS_DELETE = false;
                ThuMuc.NAM = DateTime.Now.Year;
                //ThuMuc.PERMISSION = FolderParent.PERMISSION;
                //ThuMuc.ACCESS_MODIFIER = FolderParent.ACCESS_MODIFIER;
                ThuMuc.THUMUC_AO = string.IsNullOrEmpty(ThuMuc.TENTHUMUC) ? ThuMuc.TENTHUMUC : ThuMuc.TENTHUMUC.Trim();
                THUMUC_LUUTRUBusiness.Save(ThuMuc);
                #endregion
            }
            return Json(new { Type = "SUCCESS", Message = msg }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(long id)
        {
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            THUMUC_LUUTRU ThuMuc = THUMUC_LUUTRUBusiness.Find(id);
            if (ThuMuc == null)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy thư mục cần xóa" });
            }
            arrFolder.Clear();
            UserInfoBO user = GetUserInfo();
            if (!ThuMuc.USER_ID.HasValue || user.ID != ThuMuc.USER_ID)
            {
                return Json(new { Type = "ERROR", Message = "Bạn không có quyền thực hiện chức năng này" });
            }
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            string pathname = "";
            List<CCTC_THANHPHAN> ListDonVi = CCTC_THANHPHANBusiness.GetDSParent(user.DM_PHONGBAN_ID.HasValue ? user.DM_PHONGBAN_ID.Value : 0);
            ListDonVi.Reverse();
            foreach (var item in ListDonVi)
            {
                pathname += item.NAME + "\\";
            }
            pathname += user.TENDANGNHAP + "\\eFile";
            FileUltilities file = new FileUltilities();
            var ListThuMuc = THUMUC_LUUTRUBusiness.GetAllChildren(id);
            foreach (var item in ListThuMuc)
            {
                item.IS_DELETE = true;
            }
            THUMUC_LUUTRUBusiness.Save(ListThuMuc);
            arrFolder = GetAllParent(ThuMuc.PARENT_ID.HasValue ? ThuMuc.PARENT_ID.Value : 0);
            //arrFolder.Reverse();
            for (int i = 0; i < arrFolder.Count; i++)
            {
                pathname += "\\" + arrFolder[i];
            }
            string toFolder = URL_TRASH + "\\Folder\\" + ThuMuc.ID + "\\" + ThuMuc.TENTHUMUC;
            file.MoveDirectory(URL_FOLDER + "\\" + pathname + "\\" + ThuMuc.TENTHUMUC, toFolder);
            ThuMuc.TENTHUMUC = ThuMuc.TENTHUMUC;
            ThuMuc.IS_DELETE = true;
            THUMUC_LUUTRUBusiness.Save(ThuMuc);
            return Json(new { Type = "SUCCESS", Message = "Xóa thư mục thành công" });
        }
        [ValidateInput(false)]
        public JsonResult SaveSingleFile(FormCollection col, string[] filename, string[] filecode,
            string[] LOAITAILIEU_ID, IEnumerable<HttpPostedFileBase> filebase,
            IEnumerable<HttpPostedFileBase> imagecover, List<TAILIEU_THUOCTINH> ListThuocTinh)
        {
            UploadFileTool uploadfile = new UploadFileTool();
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(col["FOLDER_ID"].ToLongOrZero());
            try
            {
                if (THUMUC != null)
                {
                    List<long> ThuMucId = GetAllPrivateChild();
                    if (filebase != null && ThuMucId.Contains(THUMUC.ID))
                    {
                        DUNGLUONG_LUUTRU_BO DungLuong = initDungLuong(user);
                        if (DungLuong.CONLAI.HasValue && DungLuong.CONLAI.Value < ((float)filebase.ElementAt(0).ContentLength / 1024))
                        {
                            return Json(new { Type = "ERROR", Message = "Không đủ bộ nhớ để lưu thêm" });
                        }
                    }
                    List<long> Ids = initDefaultId();
                    if (Ids.Contains(THUMUC.ID))
                    {
                        return Json("Không tìm thấy thư mục cần upload tài liệu");
                    }
                    string TACGIA = col["AUTHOR"];
                    string s = col["NGAYPHATHANH"];
                    DateTime? NGAYPHATHANH = string.IsNullOrEmpty(col["NGAYPHATHANH"]) ? null : col["NGAYPHATHANH"].ToDateTime();
                    #region Cập nhật tài liệu
                    if (col["TAILIEU_ID"].ToLongOrZero() > 0)
                    {
                        //cập nhật
                        TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                        TAILIEUDINHKEM TAILIEU = TAILIEUDINHKEMBusiness.Find(col["TAILIEU_ID"].ToLongOrZero());
                        if (TAILIEU != null)
                        {

                            TAILIEU.DM_LOAITAILIEU_ID = LOAITAILIEU_ID[0].ToIntOrZero();
                            if (col["DUYETTAILIEU"].ToIntOrZero() > 0)
                            {
                                TAILIEU.IS_PHEDUYET = col["DUYETTAILIEU"].ToIntOrZero();
                            }
                            TAILIEU.USER_ID = user.ID;
                            TAILIEU.NGAYTAO = DateTime.Now;
                            TAILIEU.TRANGTHAI = col["TRANGTHAI"].ToIntOrZero();
                            //TAILIEU.CONTENT_CHANGE = col["CONTENT_CHANGE"];
                            #region Kiểm tra chế độ quản ly phiên bản + Thêm mới version nếu có
                            //Nếu đang bật chế độ quản lý phiên bản
                            if (TAILIEU.IS_QLPHIENBAN.Value)
                            {
                                if (filebase != null && filebase.ElementAt(0) != null)
                                {
                                    #region Cập nhật tài liệu
                                    FileUltilities file = new FileUltilities();
                                    List<string> arrFolder = new List<string>();
                                    string old_file = URL_FOLDER + TAILIEU.DUONGDAN_FILE;
                                    string new_file = filebase.ElementAt(0).FileName;
                                    var file_name = old_file.Split('\\');
                                    string url_file = "";
                                    string old_url;
                                    TAILIEUDINHKEM_VERSION VERSION = new TAILIEUDINHKEM_VERSION();
                                    TAILIEUDINHKEM_VERSIONBusiness = Get<TAILIEUDINHKEM_VERSIONBusiness>();
                                    VERSION.TAILIEU_ID = TAILIEU.TAILIEU_ID;
                                    VERSION.DINHDANG_FILE = TAILIEU.DINHDANG_FILE;
                                    VERSION.NGAYTAI = DateTime.Now;
                                    VERSION.NGUOITAI = user.ID;
                                    VERSION.TEN_TAILIEU = TAILIEU.TENTAILIEU;
                                    VERSION.DUONGDAN_FILE = TAILIEU.DUONGDAN_FILE;
                                    VERSION.MOTA = TAILIEU.MOTA;
                                    VERSION.VERSION = TAILIEU.VERSION;
                                    TAILIEUDINHKEM_VERSIONBusiness.Save(VERSION);
                                    //Kiểm tra file đã tồn tại hay chưa
                                    if (file_name[file_name.Count() - 1].SequenceEqual(new_file))
                                    {
                                        for (int i = 0; i < file_name.Count() - 1; i++)
                                        {
                                            url_file += file_name[i] + "\\";
                                        }
                                        old_url = url_file;
                                        new_file = DateTime.Now.ToString("dd-MM-yyyy hh-mm") + filebase.ElementAt(0).FileName;
                                        url_file += new_file;
                                        file.MoveFile(old_file, url_file);
                                        filebase.ElementAt(0).SaveAs(old_url + filebase.ElementAt(0).FileName);
                                        url_file = "\\";
                                        for (int i = 0; i < TAILIEU.DUONGDAN_FILE.Split('\\').Count() - 1; i++)
                                        {
                                            url_file += file_name[i] + "\\";
                                        }
                                        TAILIEU.DINHDANG_FILE = filebase.ElementAt(0).ContentType;
                                        TAILIEU.DUONGDAN_FILE = url_file + filebase.ElementAt(0).FileName;
                                    }
                                    else
                                    {
                                        file_name = TAILIEU.DUONGDAN_FILE.Split('\\');
                                        for (int i = 0; i < file_name.Count() - 1; i++)
                                        {
                                            url_file += file_name[i] + "\\";
                                        }
                                        if (!System.IO.File.Exists(url_file))
                                        {
                                            FileUltilities fileultil = new FileUltilities();
                                            fileultil.CreateFolder(URL_FOLDER + url_file);
                                        }
                                        url_file = URL_FOLDER + url_file + new_file;
                                        filebase.ElementAt(0).SaveAs(url_file);
                                        //thuc hien luu lai ten moi cho tai lieu
                                        file_name = TAILIEU.DUONGDAN_FILE.Split('\\');
                                        url_file = "\\";
                                        for (int i = 0; i < file_name.Count() - 1; i++)
                                        {
                                            url_file += file_name[i] + "\\";
                                        }
                                        TAILIEU.DUONGDAN_FILE = url_file + filebase.ElementAt(0).FileName;
                                        TAILIEU.DINHDANG_FILE = filebase.ElementAt(0).ContentType;
                                    }
                                    if (!string.IsNullOrEmpty(filename[0]))
                                    {
                                        TAILIEU.TENTAILIEU = filename[0];
                                    }
                                    else
                                    {
                                        TAILIEU.TENTAILIEU = filebase.ElementAt(0).FileName;
                                    }
                                    if (!string.IsNullOrEmpty(col["filedescription"]))
                                    {
                                        TAILIEU.MOTA = col["filedescription"];
                                    }
                                    else
                                    {
                                        TAILIEU.TENTAILIEU = filebase.ElementAt(0).FileName;
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                if (filebase != null && filebase.ElementAt(0) != null)
                                {
                                    FileUltilities fileutil = new FileUltilities();
                                    fileutil.RemoveFile(URL_FOLDER + TAILIEU.DUONGDAN_FILE);
                                    string old_file = URL_FOLDER + TAILIEU.DUONGDAN_FILE;
                                    var file_name = old_file.Split('\\');
                                    string url_file = "";
                                    for (int i = 0; i < file_name.Count() - 1; i++)
                                    {
                                        url_file += file_name[i] + "\\";
                                    }
                                    filebase.ElementAt(0).SaveAs(url_file += filebase.ElementAt(0).FileName);
                                    url_file = "\\";
                                    for (int i = 2; i < file_name.Count() - 1; i++)
                                    {
                                        url_file += file_name[i] + "\\";
                                    }
                                    TAILIEU.DUONGDAN_FILE = url_file + filebase.ElementAt(0).FileName;
                                }
                            }
                            TAILIEU.MOTA = col["filedescription"];
                            TAILIEU.TENTACGIA = TACGIA;
                            TAILIEU.NGAYPHATHANH = NGAYPHATHANH;
                            #endregion
                            TAILIEU.VERSION = col["VERSION"];
                            TAILIEUDINHKEMBusiness.Save(TAILIEU);
                            #region Thêm mới + cập nhật thuộc tính
                            foreach (var item in ListThuocTinh)
                            {
                                item.TAILIEU_ID = TAILIEU.TAILIEU_ID;
                            }
                            TAILIEU_THUOCTINHBusiness.Save(ListThuocTinh);
                            #endregion
                        }
                        return Json("Cập nhật tài liệu thành công");
                    }
                    #endregion
                    #region Thêm mới tài liệu
                    else
                    {
                        TAILIEU_THUOCTINHBusiness = Get<TAILIEU_THUOCTINHBusiness>();
                        string DUYETTAILIEU = col["DUYETTAILIEU"];
                        var ListTaiLieu = uploadfile.UploadCustomFileVer2(DUYETTAILIEU.ToIntOrZero(), filebase, true, Extension, URL_FOLDER, MaxSize, col["FOLDER_ID"].ToLongOrZero(), filename, col["FOLDER_ID"].ToLongOrZero(), LOAITAILIEU_ID, col["filedescription"], filecode, TACGIA, NGAYPHATHANH, false, col["VERSION"], col["TRANGTHAI"].ToIntOrZero(), THUMUC.PERMISSION, LOAITAILIEU.TM_UPLOAD, "eFile");
                        if (ListTaiLieu.Count > 0)
                        {
                            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                            TAILIEUDINHKEM TAILIEUDK = TAILIEUDINHKEMBusiness.Find(ListTaiLieu[0]);
                            if (TAILIEUDK == null)
                            {
                                TAILIEUDK = new TAILIEUDINHKEM();
                            }
                            #region Thêm  thuộc tính
                            if (TAILIEUDK.TAILIEU_ID > 0 && ListThuocTinh != null)
                            {
                                foreach (var item in ListThuocTinh)
                                {
                                    item.TAILIEU_ID = TAILIEUDK.TAILIEU_ID;
                                }
                                TAILIEU_THUOCTINHBusiness.Save(ListThuocTinh);
                            }
                            if (!THUMUC.HAS_FILES.HasValue || (THUMUC.HAS_FILES.HasValue && !THUMUC.HAS_FILES.Value))
                            {
                                THUMUC.HAS_FILES = true;
                                THUMUC_LUUTRUBusiness.Save(THUMUC);
                            }
                            #region Insert dữ liệu sang mongodb và elastic
                            //TODOIT
                            //var fullPath = Server.MapPath("~/Uploads/" + TAILIEUDK.DUONGDAN_FILE);
                            // Lấy toàn bộ nội dung file
                            //var fullPathTaiLieu = Server.MapPath("~/Uploads/" + TAILIEUDK.DUONGDAN_FILE);
                            //XElement HtmlString = DocxProvider.GetHTMLString(fullPathTaiLieu, TAILIEUDK.TENTAILIEU);
                            //XmlDocument doc = new XmlDocument();
                            //doc.LoadXml(HtmlString.ToString());
                            //string totalText = "";
                            //string totalText = HtmlString.ToString();
                            //XmlNodeList lstSpan = doc.GetElementsByTagName("span");
                            //foreach (XmlNode item in lstSpan)
                            //{
                            //    totalText += item.InnerText + " ";
                            //}

                                //var totalText = GetDocXInnerText(fullPath);
                                #region Insert dữ liệu vào elasticsearch
                            //ElasticBCAModel modelElastic = new ElasticBCAModel();
                            //modelElastic.tieude = TAILIEUDK.TENTAILIEU;
                            //modelElastic.detaiid = TAILIEUDK.TAILIEU_ID;
                            //modelElastic.danhmuc = TAILIEUDK.LOAI_TAILIEU.Value;
                            //modelElastic.tacgia = TAILIEUDK.TENTACGIA;
                            //modelElastic.duongdan = TAILIEUDK.DUONGDAN_FILE;
                            //modelElastic.nam = 2018;
                            //Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(totalText);
                            //string fileContent = Convert.ToBase64String(bytes);
                            //modelElastic.file = new Attachment();
                            //modelElastic.file.Content = fileContent;
                            //ElasticSearch.insertDocumentBCA(modelElastic, modelElastic.detaiid.ToString(), "BCA");
                            #endregion
                            #region Insert dữ liệu vào mongo
                            //string connectionString = "mongodb://localhost";
                            //MongoClient client = new MongoClient(connectionString);
                            //IMongoDatabase db = client.GetDatabase("detaikhoahoc");
                            //IMongoCollection<GramMongo> collection = db.GetCollection<GramMongo>("detaikhoahoc");
                            //GramController gram = new GramController();
                            //var lstngram = gram.GetNGramFromFullText(totalText);
                            //GramMongo gramModel = new GramMongo();
                            //gramModel.lstngram = lstngram;
                            //gramModel.detaiid = TAILIEUDK.TAILIEU_ID;
                            //gramModel.danhmuc = TAILIEUDK.LOAI_TAILIEU.Value;
                            //gramModel.filepath = TAILIEUDK.DUONGDAN_FILE;
                            //collection.InsertOne(gramModel);
                            #endregion
                            #endregion
                            #endregion
                            return Json("Tải lên tài liệu thành công");
                        }
                        else
                        {
                            return Json("Không tìm thấy tài liệu được chọn để tải lên.");
                        }
                    }
                    #endregion
                }
                else
                {
                    return Json("Không tìm thấy thư mục cần tải lên file");
                }
            }
            catch (Exception ex)
            {
                return Json("Có lỗi xảy ra trong quá trình tải lên");
            }
        }

        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            var searchModel = SessionManager.GetValue("ThuMucLuuTruSearch") as THUMUC_LUUTRU_SEARCHBO;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new THUMUC_LUUTRU_SEARCHBO();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("ThuMucLuuTruSearch", searchModel);
            }
            var data = THUMUC_LUUTRUBusiness.GetDataByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }
        public JsonResult KiemTraXoaThuMuc(long THUMUC_ID)
        {
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            THUMUC_LUUTRU ThuMuc = THUMUC_LUUTRUBusiness.Find(THUMUC_ID);
            if (ThuMuc != null && ThuMuc.USER_ID.HasValue && ThuMuc.USER_ID == user.ID)
            {
                return Json("Co");
            }
            else
            {
                return Json("Khong");
            }
        }
        [ValidateInput(false)]
        public JsonResult SaveFileName(TAILIEUDINHKEM TAILIEu)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            TAILIEUDINHKEM TaiLieu = TAILIEUDINHKEMBusiness.Find(TAILIEu.TAILIEU_ID);
            if (TaiLieu != null)
            {
                UserInfoBO user = GetUserInfo();
                if (!TaiLieu.USER_ID.HasValue || user.ID != TaiLieu.USER_ID)
                {
                    return Json(new { Type = "ERROR", Message = "Bạn không có quyền thực hiện chức năng này" });
                }
                TaiLieu.TENTAILIEU = TAILIEu.TENTAILIEU;
                TAILIEUDINHKEMBusiness.Save(TaiLieu);
            }
            else
            {
                return Json("Không tìm thấy file cần đổi tên");
            }
            return Json("Đổi tên file thành công");
        }
        [ValidateInput(false)]
        public JsonResult SaveUploadFile(FormCollection col, string[] filename, string[] filecode, string[] LOAITAILIEU_ID, IEnumerable<HttpPostedFileBase> filebase, string[] DANHMUC_ID)
        {
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(col["FOLDER_ID"].ToLongOrZero());
            UserInfoBO user = GetUserInfo();
            int? ACCESS_MODIFIER = col["ACCESS_MODIFIER_ID"].ToIntOrNULL();
            List<long> Ids = initDefaultId();
            if (THUMUC != null)
            {
                List<long> ThuMucId = GetAllPrivateChild();
                if (filebase != null && ThuMucId.Contains(THUMUC.ID))
                {
                    DUNGLUONG_LUUTRU_BO DungLuong = initDungLuong(user);
                    float fileLength = 0;
                    foreach (var item in filebase)
                    {
                        if (item != null)
                        {
                            fileLength += (float)item.ContentLength / 1024;
                        }
                    }
                    if (DungLuong.CONLAI.HasValue && DungLuong.CONLAI.Value < fileLength)
                    {
                        return Json("Không đủ bộ nhớ để lưu thêm");
                    }
                }
                if (Ids.Contains(THUMUC.ID))
                {
                    return Json("Không tìm thấy thư mục cần upload tài liệu");
                }
                UploadFileTool uploadfile = new UploadFileTool();
                try
                {
                    List<long> ListTaiLieu = uploadfile.UploadCustomFileVer2(col["DUYETTAILIEU"].ToIntOrZero(),
                        filebase, true, Extension, URL_FOLDER, MaxSize, col["FOLDER_ID"].ToLongOrZero(), filename,
                        col["FOLDER_ID"].ToLongOrZero(), LOAITAILIEU_ID, "", filecode, "", null, false, col["VERSION"],
                        1, THUMUC.PERMISSION, LOAITAILIEU.TM_UPLOAD, "eFile");
                    if (ListTaiLieu.Count > 0)
                    {
                        if (!THUMUC.HAS_FILES.HasValue || (THUMUC.HAS_FILES.HasValue && !THUMUC.HAS_FILES.Value))
                        {
                            THUMUC.HAS_FILES = true;
                            THUMUC_LUUTRUBusiness.Save(THUMUC);
                        }
                        return Json("Tải lên tài liệu thành công");
                    }
                    else
                    {
                        return Json("Không tìm thấy tài liệu được chọn để tải lên.");
                    }
                }
                catch
                {
                    return Json("Có lỗi xảy ra trong quá trình tải lên");
                }
            }
            else
            {
                return Json("Không tìm thấy thư mục cần tải lên file");
            }
        }
        public JsonResult saveZipFile(FormCollection col, HttpPostedFileBase files)
        {
            try
            {
                if (files != null)
                {
                    UserInfoBO user = GetUserInfo();
                    THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
                    CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
                    string isArchiveHere = col["rdArchive"];
                    string folderName = col["TENTHUMUC"];
                    int? accessModifierId = col["ACCESS_MODIFIER_ID"].ToIntOrNULL();
                    THUMUC_LUUTRU thumuc = THUMUC_LUUTRUBusiness.Find(long.Parse(col["FOLDER_ID"]));
                    if (thumuc == null || thumuc.USER_ID != user.ID)
                    {
                        return Json("Không tìm thấy thư mục cần giải nén tài liệu");
                    }
                    List<long> ThuMucId = GetAllPrivateChild();
                    if (ThuMucId.Contains(thumuc.ID))
                    {
                        DUNGLUONG_LUUTRU_BO DungLuong = initDungLuong(user);
                        float fileLength = (float)files.ContentLength / 1024;
                        if (DungLuong.CONLAI.HasValue && DungLuong.CONLAI.Value < fileLength)
                        {
                            return Json("Không đủ bộ nhớ để lưu thêm");
                        }
                    }
                    List<long> Ids = initDefaultId();
                    if (Ids.Contains(thumuc.ID))
                    {
                        return Json("Không tìm thấy thư mục cần upload tài liệu");
                    }
                    UploadFileTool tool = new UploadFileTool();
                    string uploads = "";
                    string virtualPath = "\\";
                    List<CCTC_THANHPHAN> ListDonVi = CCTC_THANHPHANBusiness.GetDSParent(user.DM_PHONGBAN_ID.HasValue ? user.DM_PHONGBAN_ID.Value : 0);
                    ListDonVi.Reverse();
                    foreach (var item in ListDonVi)
                    {
                        virtualPath += item.NAME + "\\";
                    }
                    virtualPath += user.TENDANGNHAP + "\\eFile";
                    #region Tạo mới thư mục nếu người dùng muốn giải nén tài liệu ở 1 nơi khác, không phải thư mục đang chọn
                    if (!isArchiveHere.ToLower().Equals("true"))
                    {
                        THUMUC_LUUTRU newFolder = new THUMUC_LUUTRU();
                        newFolder.USER_ID = thumuc.USER_ID;
                        newFolder.DONVI_ID = thumuc.DONVI_ID;
                        newFolder.IS_DELETE = false;
                        newFolder.NAM = thumuc.NAM;
                        newFolder.NGAYTAO = DateTime.Now;
                        newFolder.PARENT_ID = thumuc.ID;
                        newFolder.TENTHUMUC = folderName;
                        newFolder.THUMUC_AO = folderName;
                        newFolder.PERMISSION = thumuc.PERMISSION;
                        newFolder.ACCESS_MODIFIER = thumuc.ACCESS_MODIFIER;
                        THUMUC_LUUTRUBusiness.Save(newFolder);
                        thumuc = new THUMUC_LUUTRU();
                        thumuc = newFolder;
                        #region Thêm mới thư mục trong ổ cứng
                        FileUltilities fileUtil = new FileUltilities();
                        if (newFolder.PARENT_ID > 0)
                        {
                            arrFolder = GetAllParent(newFolder.PARENT_ID.HasValue ? newFolder.PARENT_ID.Value : 0);
                            arrFolder.Reverse();
                            foreach (string s in arrFolder)
                            {
                                virtualPath += "\\" + s;
                            }
                            fileUtil.CreateFolder(URL_FOLDER + "\\" + virtualPath + "\\" + newFolder.TENTHUMUC.Trim());
                        }
                        else
                        {
                            fileUtil.CreateFolder(URL_FOLDER + "\\" + virtualPath + "\\" + newFolder.TENTHUMUC.Trim());
                        }
                        uploads = URL_FOLDER + "\\" + virtualPath + "\\" + newFolder.TENTHUMUC.Trim();
                        virtualPath += "\\" + newFolder.TENTHUMUC.Trim();
                        #endregion
                    }
                    else
                    {
                        uploads = URL_FOLDER + "\\" + initFolderPath(user, thumuc.PARENT_ID.HasValue ? thumuc.PARENT_ID.Value : 0) + "\\" + thumuc.TENTHUMUC.Trim();
                        virtualPath += "\\" + thumuc.TENTHUMUC.Trim();
                    }
                    #endregion
                    TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                    //Nếu không là file zip
                    int countFolder = 0;
                    if (files.ContentType != "application/x-zip-compressed")
                    {
                        #region Giải nén file rar
                        RarArchive rarFile = RarArchive.Open(files.InputStream);
                        int i = -1;
                        foreach (RarArchiveEntry item in rarFile.Entries)
                        {

                            //Nếu entry này không phải là thư mục
                            if (!item.IsDirectory)
                            {
                                i++;
                                string fileName = Path.GetFileName(item.FilePath);
                                string path = Path.Combine(uploads, fileName);
                                if (System.IO.File.Exists(path) && item != null)
                                {

                                    fileName = Path.GetFileNameWithoutExtension(path) + DateTime.Now.ToString("dd-MM-yyyy HH:mm") + Path.GetExtension(path);
                                }
                                #region Thêm mới tài liệu
                                TAILIEUDINHKEM TAILIEU = new TAILIEUDINHKEM();
                                TAILIEU.TENTAILIEU = fileName;
                                TAILIEU.LOAI_TAILIEU = LOAITAILIEU.TM_UPLOAD;
                                TAILIEU.MOTA = "";
                                TAILIEU.DUONGDAN_FILE = virtualPath;
                                TAILIEU.DINHDANG_FILE = MimeMapping.GetMimeMapping(fileName);
                                TAILIEU.USER_ID = (long)user.ID;
                                TAILIEU.FOLDER_ID = thumuc.ID;
                                TAILIEU.IS_ACTIVE = 1;
                                TAILIEU.SOLUONG_DOWNLOAD = 0;
                                TAILIEU.ITEM_ID = thumuc.ID;
                                TAILIEU.NGAYTAO = DateTime.Now;
                                TAILIEU.DM_LOAITAILIEU_ID = 0;
                                TAILIEU.IS_LOCK = false;
                                TAILIEU.NGUOI_LOCK = 0;
                                TAILIEU.IS_QLPHIENBAN = false;
                                TAILIEU.KICHCO = item.Size;
                                TAILIEU.PERMISSION = thumuc.PERMISSION;
                                TAILIEUDINHKEMBusiness.Save(TAILIEU);
                                #endregion
                                string urlPath = Path.Combine(uploads, item.FilePath);
                                using (FileStream output = System.IO.File.OpenWrite(urlPath))
                                {
                                    item.WriteTo(output);
                                }
                            }
                            else
                            {
                                countFolder++;
                            }
                        }
                        if ((!thumuc.HAS_FILES.HasValue || (thumuc.HAS_FILES.HasValue && !thumuc.HAS_FILES.Value)) && i > 0)
                        {
                            thumuc.HAS_FILES = true;
                            THUMUC_LUUTRUBusiness.Save(thumuc);
                        }
                        #endregion
                    }
                    else
                    {
                        int count = 0;
                        #region Giải nén file zip
                        //using (ZipFile zip = ZipFile.Read(files.InputStream))
                        //{
                        //    List<string> lstFileName = new List<string>();
                        //    foreach (ZipEntry item in zip.Entries)
                        //    {
                        //        //Nếu entry này không phải là thư mục
                        //        if (!item.IsDirectory)
                        //        {
                        //            count++;
                        //            #region Thêm mới tài liệu
                        //            string path = Path.Combine(uploads, item.FileName);
                        //            if (System.IO.File.Exists(path))
                        //            {
                        //                lstFileName.Add(Path.GetFileNameWithoutExtension(path) + DateTime.Now.ToString("dd-MM-yyyy HH:mm") + Path.GetExtension(path));
                        //            }
                        //            else
                        //            {
                        //                lstFileName.Add(item.FileName);
                        //            }
                        //            TAILIEUDINHKEM TAILIEU = new TAILIEUDINHKEM();
                        //            TAILIEU.TENTAILIEU = item.FileName;
                        //            TAILIEU.LOAI_TAILIEU = LOAITAILIEU.TM_UPLOAD;
                        //            TAILIEU.MOTA = "";
                        //            TAILIEU.DUONGDAN_FILE = virtualPath + "\\" + item.FileName;
                        //            TAILIEU.DINHDANG_FILE = MimeMapping.GetMimeMapping(item.FileName);
                        //            TAILIEU.USER_ID = (long)user.ID;
                        //            TAILIEU.FOLDER_ID = thumuc.ID;
                        //            TAILIEU.IS_ACTIVE = 1;
                        //            TAILIEU.SOLUONG_DOWNLOAD = 0;
                        //            TAILIEU.ITEM_ID = thumuc.ID;
                        //            TAILIEU.NGAYTAO = DateTime.Now;
                        //            TAILIEU.DM_LOAITAILIEU_ID = 0;
                        //            TAILIEU.IS_LOCK = false;
                        //            TAILIEU.NGUOI_LOCK = 0;
                        //            TAILIEU.IS_QLPHIENBAN = false;
                        //            TAILIEU.KICHCO = item.UncompressedSize;
                        //            TAILIEU.PERMISSION = thumuc.PERMISSION;
                        //            TAILIEUDINHKEMBusiness.Save(TAILIEU);
                        //            #endregion
                        //        }
                        //        else
                        //        {
                        //            countFolder++;
                        //        }
                        //    }
                        //    for (int i = 0; i < lstFileName.Count; i++)
                        //    {
                        //        if (zip.ElementAt(i) != null)
                        //        {
                        //            zip.ElementAt(i).FileName = lstFileName[i];
                        //        }
                        //    }
                        //    zip.ExtractAll(uploads, ExtractExistingFileAction.OverwriteSilently);
                        //}
                        if ((!thumuc.HAS_FILES.HasValue || (thumuc.HAS_FILES.HasValue && !thumuc.HAS_FILES.Value)) && count > 0)
                        {
                            thumuc.HAS_FILES = true;
                            THUMUC_LUUTRUBusiness.Save(thumuc);
                        }
                        #endregion
                    }
                    if (countFolder > 0)
                    {
                        return Json("Giải nén file thành công. Có " + countFolder + " không được giải nén");
                    }
                    else
                    {
                        return Json("Giải nén file thành công");
                    }
                }
                else
                {
                    return Json("Không tìm thấy file được tải lên");
                }
            }
            catch
            {
                return Json("Không thể thực hiện hành động này");
            }
        }
        public JsonResult KiemTraXoaTaiLieu(long TAILIEU)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            UserInfoBO user = GetUserInfo();
            TAILIEUDINHKEM TaiLieu = TAILIEUDINHKEMBusiness.Find(TAILIEU);
            if (TaiLieu != null && TaiLieu.USER_ID == user.ID)
            {
                return Json("Co");
            }
            else
            {
                return Json("Khong");
            }
        }
        public JsonResult XoaTaiLieu(long TAILIEU)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            TAILIEUDINHKEM_VERSIONBusiness = Get<TAILIEUDINHKEM_VERSIONBusiness>();
            TAILIEUDINHKEM TAILIEUDK = TAILIEUDINHKEMBusiness.Find(TAILIEU);
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            if (TAILIEUDK != null || TAILIEUDK.USER_ID != (long)user.ID)
            {
                TAILIEUDINHKEM TAILIEU_DINHKEM = TAILIEUDINHKEMBusiness.Find(TAILIEU);
                string duongdan = URL_FOLDER + TAILIEU_DINHKEM.DUONGDAN_FILE;
                string toFolder = URL_TRASH + "\\Files\\" + TAILIEU + "\\";
                FileUltilities file = new FileUltilities();
                if (!Directory.Exists(toFolder))
                {
                    System.IO.Directory.CreateDirectory(toFolder);
                }
                toFolder += Path.GetFileName(duongdan);
                file.MoveFile(duongdan, toFolder);
                //TAILIEU_DINHKEM.IS_DELETE = true;
                TAILIEUDINHKEMBusiness.Save(TAILIEU_DINHKEM);
                #region Xóa các phiên bản của tài liệu
                var ListVersion = TAILIEUDINHKEM_VERSIONBusiness.GetDataByTaiLieuID(TAILIEU);
                foreach (var item in ListVersion)
                {
                    file.RemoveFile(URL_FOLDER + item.DUONGDAN_FILE);
                    TAILIEUDINHKEM_VERSIONBusiness.repository.Delete(item.ID);
                }
                TAILIEUDK.IS_DELETE = true;
                TAILIEUDINHKEMBusiness.Save(TAILIEUDK);
                TAILIEUDINHKEM_VERSIONBusiness.Save();
                #endregion
                return Json("Xóa tài liệu thành công");
            }
            else
            {
                return Json("Không tìm thấy tài liệu cần xóa");
            }
        }
        public JsonResult CheckkingFile(long ID)
        {

            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            TAILIEUDINHKEM taikieu = TAILIEUDINHKEMBusiness.Find(ID);
            UserInfoBO user = GetUserInfo();
            if (taikieu != null)
            {
                string path = URL_FOLDER + taikieu.DUONGDAN_FILE;
                if (System.IO.File.Exists(path))
                {
                    return Json("Co");
                }
                else
                {
                    return Json("Khong");
                }
            }
            return Json("Khong");
        }
        public JsonResult CheckkingVersion(long ID)
        {
            TAILIEUDINHKEM_VERSIONBusiness = Get<TAILIEUDINHKEM_VERSIONBusiness>();

            TAILIEUDINHKEM_VERSION taikieu = TAILIEUDINHKEM_VERSIONBusiness.Find(ID);

            if (taikieu != null)
            {
                string path = URL_FOLDER + taikieu.DUONGDAN_FILE;
                if (System.IO.File.Exists(path))
                {
                    return Json("Co");
                }
                else
                {
                    return Json("Khong");
                }
            }
            return Json("Khong");
        }
        public JsonResult AddNewVersion(FormCollection col, IEnumerable<HttpPostedFileBase> filebase)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            UserInfoBO user = GetUserInfo();
            TAILIEUDINHKEM TAILIEU = TAILIEUDINHKEMBusiness.Find(col["TAILIEU_ID"].ToLongOrZero());
            if (TAILIEU == null || TAILIEU.USER_ID != user.ID)
            {
                return Json("Không tìm thấy tài liệu cần thêm phiên bản mới");
            }
            if (TAILIEU.IS_QLPHIENBAN.Value)
            {
                if (filebase != null && filebase.ElementAt(0) != null)
                {
                    #region Cập nhật tài liệu
                    FileUltilities file = new FileUltilities();
                    List<string> arrFolder = new List<string>();
                    string old_file = URL_FOLDER + TAILIEU.DUONGDAN_FILE;
                    string new_file = filebase.ElementAt(0).FileName;
                    var file_name = old_file.Split('\\');
                    string url_file = "";
                    string old_url;
                    TAILIEUDINHKEM_VERSION VERSION = new TAILIEUDINHKEM_VERSION();
                    TAILIEUDINHKEM_VERSIONBusiness = Get<TAILIEUDINHKEM_VERSIONBusiness>();
                    VERSION.TAILIEU_ID = TAILIEU.TAILIEU_ID;
                    VERSION.DINHDANG_FILE = TAILIEU.DINHDANG_FILE;
                    VERSION.NGAYTAI = DateTime.Now;
                    VERSION.NGUOITAI = user.ID;
                    VERSION.TEN_TAILIEU = TAILIEU.TENTAILIEU;
                    VERSION.DUONGDAN_FILE = TAILIEU.DUONGDAN_FILE;
                    VERSION.MOTA = TAILIEU.MOTA;
                    VERSION.VERSION = col["VERSION"];
                    TAILIEUDINHKEM_VERSIONBusiness.Save(VERSION);
                    //Kiểm tra file đã tồn tại hay chưa
                    if (file_name[file_name.Count() - 1].SequenceEqual(new_file))
                    {
                        url_file = "";
                        for (int i = 0; i < file_name.Count() - 1; i++)
                        {
                            if (!string.IsNullOrEmpty(file_name[i]))
                            {
                                url_file += file_name[i] + "\\";
                            }
                        }
                        old_url = url_file;
                        new_file = DateTime.Now.ToString("dd-MM-yyyy hh-mm") + filebase.ElementAt(0).FileName;
                        url_file += new_file;
                        file.MoveFile(old_file, url_file);
                        filebase.ElementAt(0).SaveAs(old_url + filebase.ElementAt(0).FileName);
                        url_file = "\\";
                        var fileArr = TAILIEU.DUONGDAN_FILE.Split('\\');
                        for (int i = 0; i < fileArr.Count() - 1; i++)
                        {
                            if (!string.IsNullOrEmpty(fileArr[i]))
                            {
                                url_file += file_name[i] + "\\";
                            }
                        }
                        TAILIEU.DINHDANG_FILE = filebase.ElementAt(0).ContentType;
                        TAILIEU.DUONGDAN_FILE = url_file + filebase.ElementAt(0).FileName;
                    }
                    else
                    {
                        file_name = TAILIEU.DUONGDAN_FILE.Split('\\');
                        for (int i = 0; i < file_name.Count() - 1; i++)
                        {
                            url_file += file_name[i] + "\\";
                        }
                        if (!System.IO.File.Exists(url_file))
                        {
                            FileUltilities fileultil = new FileUltilities();
                            fileultil.CreateFolder(URL_FOLDER + url_file);
                        }
                        url_file = URL_FOLDER + url_file + new_file;
                        filebase.ElementAt(0).SaveAs(url_file);
                        //thuc hien luu lai ten moi cho tai lieu
                        file_name = TAILIEU.DUONGDAN_FILE.Split('\\');
                        url_file = "\\";
                        for (int i = 0; i < file_name.Count() - 1; i++)
                        {
                            url_file += file_name[i] + "\\";
                        }
                        TAILIEU.DUONGDAN_FILE = url_file + filebase.ElementAt(0).FileName;
                        TAILIEU.DINHDANG_FILE = filebase.ElementAt(0).ContentType;
                    }
                    TAILIEU.TENTAILIEU = filebase.ElementAt(0).FileName;
                    TAILIEUDINHKEMBusiness.Save(TAILIEU);
                    #endregion
                }
            }
            else
            {
                return Json("Vui lòng bật chết độ quản lý phiên bản");
            }
            return Json("Đã thêm mới phiên bản tài liệu thành công");
        }
        public JsonResult XoaFileVersion(long ID)
        {
            TAILIEUDINHKEM_VERSIONBusiness = Get<TAILIEUDINHKEM_VERSIONBusiness>();
            if (TAILIEUDINHKEM_VERSIONBusiness.Find(ID) != null)
            {
                try
                {
                    TAILIEUDINHKEM_VERSION VERSION = TAILIEUDINHKEM_VERSIONBusiness.Find(ID);
                    string duongdan = URL_FOLDER + VERSION.DUONGDAN_FILE;
                    FileUltilities file = new FileUltilities();
                    file.RemoveFile(duongdan);
                    TAILIEUDINHKEM_VERSIONBusiness.repository.Delete(ID);
                    TAILIEUDINHKEM_VERSIONBusiness.Save();
                    return Json("Xóa tài liệu thành công");
                }
                catch
                {
                    return Json("Có lỗi trong quá trình xóa");
                }
            }
            else
            {
                return Json("Không tìm thấy tài liệu cần xóa");
            }
        }
        public JsonResult PreviousVersionFIle(long ID, long TAILIEU_ID)
        {
            TAILIEUDINHKEM_VERSIONBusiness = Get<TAILIEUDINHKEM_VERSIONBusiness>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            UserInfoBO user = GetUserInfo();
            TAILIEUDINHKEM TAILIEU = TAILIEUDINHKEMBusiness.Find(TAILIEU_ID);
            TAILIEUDINHKEM TAILIEU1 = TAILIEUDINHKEMBusiness.Find(TAILIEU_ID);
            TAILIEUDINHKEM_VERSION VERSION = TAILIEUDINHKEM_VERSIONBusiness.Find(ID);
            if (TAILIEU != null && VERSION != null)
            {
                TAILIEU.DUONGDAN_FILE = VERSION.DUONGDAN_FILE;
                TAILIEUDINHKEMBusiness.Save(TAILIEU);
                //TmManagerVersionBusiness.Delete(ID);
                VERSION = new TAILIEUDINHKEM_VERSION();
                VERSION.DUONGDAN_FILE = TAILIEU1.DUONGDAN_FILE;
                VERSION.DINHDANG_FILE = TAILIEU1.DINHDANG_FILE;
                VERSION.MOTA = TAILIEU1.MOTA;
                VERSION.NGAYTAI = DateTime.Now;
                VERSION.NGUOITAI = (long)user.ID;
                VERSION.TAILIEU_ID = TAILIEU1.TAILIEU_ID;
                VERSION.TEN_TAILIEU = TAILIEU1.TENTAILIEU;
                TAILIEUDINHKEM_VERSIONBusiness.Save(VERSION);
            }
            return Json("Đã khôi phục tài liệu thành công1");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(FormCollection form)
        {
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            UserInfoBO userInfoBO = GetUserInfo();
            var searchModel = SessionManager.GetValue("ThuMucLuuTruSearch") as THUMUC_LUUTRU_SEARCHBO;

            if (searchModel == null)
            {
                searchModel = new THUMUC_LUUTRU_SEARCHBO();
                searchModel.pageSize = 20;
            }
            searchModel.ACCESS_MODIFIER = form["ACCESS_MODIFIER"].ToIntOrNULL();
            searchModel.TEN_THUMUC = !string.IsNullOrEmpty(form["TEN_THUMUC"]) ? form["TEN_THUMUC"].Trim() : form["TEN_THUMUC"];
            searchModel.TEN_TAILIEU = !string.IsNullOrEmpty(form["TEN_TAILIEU"]) ? form["TEN_TAILIEU"].Trim() : form["TEN_TAILIEU"];
            searchModel.FOLDER_ID = form["FOLDER_ID"].ToLongOrNULL();
            searchModel.FOLDER_PERMISSION = form["PERMISSION"].ToIntOrNULL();
            if (searchModel.FOLDER_ID == ThuMucLuuTruConstant.DefaultPrivate)
            {
                searchModel.USER_ID = userInfoBO.ID;
            }
            List<long> Ids = THUMUC_LUUTRUBusiness.GetAllParent(searchModel.FOLDER_ID.HasValue ? searchModel.FOLDER_ID.Value : 0).Select(x => x.ID).ToList();
            if (Ids.Contains(ThuMucLuuTruConstant.DefaultPrivate))
            {
                searchModel.USER_ID = userInfoBO.ID;
            }
            SessionManager.SetValue("ThuMucLuuTruSearch", searchModel);
            if (form["FIND_BY"].ToIntOrZero() == 1)
            {
                //Find by folder
                var data = THUMUC_LUUTRUBusiness.GetDataByPage(searchModel, 1, searchModel.pageSize);
                return Json(data);
            }
            else
            {
                THUMUC_LUUTRU ThuMuc = THUMUC_LUUTRUBusiness.Find(searchModel.FOLDER_ID);
                if (ThuMuc == null)
                {
                    ThuMuc = new THUMUC_LUUTRU();
                }
                CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(ThuMuc.DONVI_ID);

                var data = THUMUC_LUUTRUBusiness.GetFileByPage(searchModel, 1, searchModel.pageSize, DonVi != null ? DonVi.NAME : "", ThuMuc);
                return Json(data);
            }
        }
        public JsonResult SaveSharing(EFILE_CHIASE Sharing, FormCollection col)
        {
            var Ids = col["NGUOIDUOC_CHIASE"].ToListLong(',');
            DateTime? TUNGAY = col["TUNGAY"].ToDateTime();
            DateTime? DENNGAY = col["DENNGAY"].ToDateTime();
            Sharing.TUNGAY = TUNGAY;
            Sharing.DENNGAY = DENNGAY;
            EFILE_CHIASEBusiness = Get<EFILE_CHIASEBusiness>();
            UserInfoBO userInfoBO = GetUserInfo();
            List<EFILE_CHIASE_BO> ListChiaSe = EFILE_CHIASEBusiness.GetData(Sharing.ITEM_ID.HasValue ? Sharing.ITEM_ID.Value : 0,
                Sharing.IS_FOLDER.HasValue ? Sharing.IS_FOLDER.Value : false);
            int? DONVI_ID = 0;
            #region Lấy đơn vị của thư mục hay tài liệu
            if (Sharing.IS_FOLDER.HasValue && Sharing.IS_FOLDER.Value)
            {
                THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
                THUMUC_LUUTRU ThuMuc = THUMUC_LUUTRUBusiness.Find(Sharing.ITEM_ID);
                if (ThuMuc != null)
                {
                    DONVI_ID = ThuMuc.DONVI_ID;
                }
            }
            else if (Sharing.IS_FOLDER.HasValue && !Sharing.IS_FOLDER.Value)
            {
                TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                TAILIEUDINHKEM TaiLieu = TAILIEUDINHKEMBusiness.Find(Sharing.ITEM_ID);
                if (TaiLieu != null)
                {
                    DONVI_ID = TaiLieu.DONVI_ID;
                }
            }
            #endregion
            foreach (var item in Ids)
            {
                EFILE_CHIASE temp = ListChiaSe.Where(x => x.USER_ID.HasValue && x.USER_ID == item).FirstOrDefault();
                if (temp == null)
                {
                    temp = new EFILE_CHIASE();
                }
                temp.DENNGAY = Sharing.DENNGAY;
                temp.GHICHU = Sharing.GHICHU;
                temp.IS_FOLDER = Sharing.IS_FOLDER;
                temp.ITEM_ID = Sharing.ITEM_ID;
                temp.NGAY_CHIASE = DateTime.Now;
                temp.TUNGAY = Sharing.TUNGAY;
                temp.USER_ID = item;
                temp.SHARING_BY = userInfoBO.ID;
                temp.DONVI_ID = DONVI_ID;
                temp.PERMISSION = Sharing.PERMISSION;
                EFILE_CHIASEBusiness.Save(temp);
            }
            return Json(new { Type = "SUCCESS", Message = "Chia sẻ tài liệu thành công" });
        }
        public JsonResult DeleteSharing(long id)
        {
            EFILE_CHIASEBusiness = Get<EFILE_CHIASEBusiness>();
            EFILE_CHIASE ChiaSe = EFILE_CHIASEBusiness.Find(id);
            UserInfoBO userInfo = GetUserInfo();
            if (ChiaSe == null || (ChiaSe.SHARING_BY.HasValue && ChiaSe.SHARING_BY.Value != userInfo.ID))
            {
                return Json(new { Type = "SUCCESS", Message = "Không thể thực hiện thao tác này" });
            }
            EFILE_CHIASEBusiness.repository.Delete(id);
            EFILE_CHIASEBusiness.Save();
            return Json(new { Type = "SUCCESS", Message = "Ngừng chia sẻ thành công" });
        }
        public JsonResult SaveEditShare(EFILE_CHIASE ChiaSe, FormCollection col)
        {
            EFILE_CHIASEBusiness = Get<EFILE_CHIASEBusiness>();
            if (ChiaSe.ID == 0)
            {
                return Json(new { Type = "ERROR", Message = "Không thể thực hiện thao tác này" }, JsonRequestBehavior.AllowGet);
            }
            var result = EFILE_CHIASEBusiness.Find(ChiaSe.ID);
            if (result == null)
            {
                return Json(new { Type = "ERROR", Message = "Không tìm thấy chia sẻ cần cập nhật" }, JsonRequestBehavior.AllowGet);
            }
            result.TUNGAY = col["TUNGAY"].ToDateTime();
            result.DENNGAY = col["DENNGAY"].ToDateTime();
            result.GHICHU = ChiaSe.GHICHU;
            EFILE_CHIASEBusiness.Save(result);
            return Json(new { Type = "SUCCESS", Message = "Cập nhật chia sẻ file thành công" }, JsonRequestBehavior.AllowGet);
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
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            List<THUMUC_LUUTRU_BO> subMenu = new List<THUMUC_LUUTRU_BO>();
            List<TAILIEUDINHKEM_BO> lstTaiLieu = new List<TAILIEUDINHKEM_BO>();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            int pID = pid.ToIntOrZero();
            THUMUC_LUUTRUBusiness = Get<THUMUC_LUUTRUBusiness>();
            if (pID == 0)
            {
                ViewData["Search"] = "0";
                subMenu = this.GetAllThuMuc(DateTime.Now.Year);
                var allChild = JsonConvert.SerializeObject(subMenu);
                return allChild;
            }
            THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(pID);
            if (THUMUC != null)
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
                if ((ThuMucLuuTruConstant.DefaultVanban == THUMUC.FIXED_FOLDER_ID || ThuMucLuuTruConstant.DefaultVbDen == THUMUC.FIXED_FOLDER_ID || ThuMucLuuTruConstant.DefaultCongViec == THUMUC.FIXED_FOLDER_ID)
                    && 0 < THUMUC.PARENT_ID)
                {
                    switch (THUMUC.FIXED_FOLDER_ID)
                    {
                        case ThuMucLuuTruConstant.DefaultVanban:
                            subMenu = TAILIEUDINHKEMBusiness.getListFileByFolder(THUMUC.THUMUC_AO.ToLongOrZero(), LOAITAILIEU.VANBAN, user.ID, DONVI.NAME, THUMUC);
                            break;
                        case ThuMucLuuTruConstant.DefaultVbDen:
                            subMenu = TAILIEUDINHKEMBusiness.getListFileByFolder(THUMUC.THUMUC_AO.ToLongOrZero(), LOAITAILIEU.VANBANDEN, user.ID, DONVI.NAME, THUMUC);
                            break;
                        case ThuMucLuuTruConstant.DefaultCongViec:
                            subMenu = TAILIEUDINHKEMBusiness.getListFileByFolder(THUMUC.TENTHUMUC.ToLongOrZero(), LOAITAILIEU.CONGVIEC, user.ID, DONVI.NAME, THUMUC);
                            break;
                    }
                    return JsonConvert.SerializeObject(subMenu);
                }
                else
                {
                    switch (THUMUC.ACCESS_MODIFIER)
                    {
                        case AccessModifier.ALL_SYSTEM:
                        case AccessModifier.ALL_UNIT:
                        case AccessModifier.ALL_DEPARTMENT:
                            subMenu = TAILIEUDINHKEMBusiness.getListFileByFolder(pID, LOAITAILIEU.TM_UPLOAD, 0, DONVI.NAME, THUMUC);
                            break;
                        case AccessModifier.PRIVATE:
                            subMenu = TAILIEUDINHKEMBusiness.getListFileByFolder(pID, LOAITAILIEU.TM_UPLOAD, user.ID, DONVI.NAME, THUMUC);
                            break;
                    }
                }
                if ((ThuMucLuuTruConstant.DefaultVanban == THUMUC.FIXED_FOLDER_ID || ThuMucLuuTruConstant.DefaultVbDen == THUMUC.FIXED_FOLDER_ID || ThuMucLuuTruConstant.DefaultCongViec == THUMUC.FIXED_FOLDER_ID))
                {
                    subMenu.AddRange(THUMUC_LUUTRUBusiness.GetThuMucByParent(pID, user.ID, Ids));
                    return JsonConvert.SerializeObject(subMenu);
                }
                CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
                switch (THUMUC.ACCESS_MODIFIER)
                {
                    case AccessModifier.ALL_SYSTEM:
                        subMenu.AddRange(THUMUC_LUUTRUBusiness.GetThuMucByParent(pID, 0, Ids));
                        break;
                    case AccessModifier.ALL_UNIT:
                        Ids = CCTC_THANHPHANBusiness.GetDSChild(user.DM_PHONGBAN_ID.Value).Select(x => x.ID).ToList();
                        Ids.Add(user.DM_PHONGBAN_ID.Value);
                        subMenu.AddRange(THUMUC_LUUTRUBusiness.GetThuMucByParent(pID, 0, Ids));
                        break;
                    case AccessModifier.ALL_DEPARTMENT:
                        Ids.Add(user.DM_PHONGBAN_ID.Value);
                        subMenu.AddRange(THUMUC_LUUTRUBusiness.GetThuMucByParent(pID, 0, Ids));
                        break;
                    case AccessModifier.PRIVATE:
                        subMenu.AddRange(THUMUC_LUUTRUBusiness.GetThuMucByParent(pID, user.ID, Ids));
                        break;
                }
                //subMenu = subMenu.Where(x => (x.USER_ID == user.ID)).ToList();
                #endregion
            }
            return JsonConvert.SerializeObject(subMenu);
        }

        public FileResult DownloadFile(long ID)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            TAILIEUDINHKEM taikieu = TAILIEUDINHKEMBusiness.Find(ID);
            if (taikieu != null)
            {
                TAILIEUDINHKEM files = TAILIEUDINHKEMBusiness.Find(taikieu.TAILIEU_ID);
                if (files != null)
                {
                    files.SOLUONG_DOWNLOAD += 1;
                    string contentType = taikieu.DINHDANG_FILE;
                    string path = URL_FOLDER + taikieu.DUONGDAN_FILE;
                    if (System.IO.File.Exists(path))
                    {
                        var filename = path.Split('\\');
                        string fileSave = filename[filename.Count() - 1];
                        TAILIEUDINHKEMBusiness.Save(files);
                        return File(path, contentType, fileSave);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }
        #endregion

        
    }
}