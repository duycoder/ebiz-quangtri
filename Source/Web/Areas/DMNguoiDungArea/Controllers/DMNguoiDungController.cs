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
using Business.CommonModel.DMNguoiDung;
using Web.FwCore;
using Web.Areas.DMNGUOIDUNGArea.Models;
using Web.Areas.DMNguoiDungArea.Models;
using Business.CommonModel.CONSTANT;
using Web.Filter;
using System.IO;
using System.Text;
using Web.Common;
using VTUtils.Excel.Export;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using CommonHelper.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using CommonHelper.DateExtend;
using CommonHelper.Upload;

namespace Web.Areas.DMNguoiDungArea.Controllers
{
    public class DMNguoiDungController : BaseController
    {
        // GET: DMNguoiDungArea/DMNguoiDung
        #region khaibao
        DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        DM_CHUCNANGBusiness DM_CHUCNANGBusiness;
        VAITRO_THAOTACBusiness VAITRO_THAOTACBusiness;
        NGUOIDUNG_VAITROBusiness NGUOIDUNG_VAITROBusiness;
        DM_NGUOIDUNG_THAOTACBusiness DM_NGUOIDUNG_THAOTACBusiness;
        DM_VAITROBusiness DM_VAITROBusiness;
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        private int MaxPerpage = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);
        private string UPLOADFOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private string HostUpload = "/Uploads";
        #endregion
        public string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        public ActionResult TaoPhongBan(int id)
        {
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            string[] lines = System.IO.File.ReadAllLines(@"E:\DuAnCongTy\2018\Tỉnh Ủy Quảng Trị\vanban_quangtri\Source\Web\Uploads\hailang.txt");
            var idx = 1;
            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var deptname = line;
                    var result = deptname.Split(' ').ToList();                    
                    string strCode = String.Join("", result) + "HL";
                    strCode = convertToUnSign3(strCode).ToUpper();
                    var CheckObj =
                        CCTC_THANHPHANBusiness.repository.All().Where(x => x.CODE == strCode && x.ID > id).FirstOrDefault();
                    if (CheckObj == null)
                    {
                        CCTC_THANHPHAN data = new CCTC_THANHPHAN();
                        data.PARENT_ID = id;
                        data.NAME = line;
                        data.CODE = strCode;
                        data.THUTU = idx;
                        data.TYPE = 10;
                        data.ITEM_LEVEL = 5;

                        CCTC_THANHPHANBusiness.Save(data);
                        CCTC_THANHPHAN vanphong = new CCTC_THANHPHAN();
                        vanphong.PARENT_ID = data.ID;
                        vanphong.TYPE = 9;
                        vanphong.ITEM_LEVEL = 5;
                        vanphong.NAME = "VP " + line;
                        vanphong.CODE = "VP" + data.CODE;
                        vanphong.THUTU = 2;
                        CCTC_THANHPHANBusiness.Save(vanphong);

                        CCTC_THANHPHAN banlanhdao = new CCTC_THANHPHAN();
                        banlanhdao.PARENT_ID = data.ID;
                        banlanhdao.TYPE = 2;
                        banlanhdao.ITEM_LEVEL = 5;
                        banlanhdao.NAME = "Ban lãnh đạo " + line;
                        banlanhdao.CODE = "BLD" + data.CODE;
                        banlanhdao.THUTU = 1;
                        CCTC_THANHPHANBusiness.Save(banlanhdao);
                        idx += 1;
                    }
                    
                }
            }
            return RedirectToRoute("index");
        }
        [CodeAllowAccess(Code = "DsNguoiDung")]
        public ActionResult Index()
        {
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            AssignUserInfo();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            //var searchmodel = new DM_NGUOIDUNG_SEARCHBO();
            int DonViId = 0;
            if (currentUser.ListVaiTro.Any(x => x.MA_VAITRO == PermissionVanbanModel.QLHT))
            {
                //Cấp lãnh đạo
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                if (DonVi != null)
                {
                    DonViId = DonVi.PARENT_ID.HasValue ? DonVi.PARENT_ID.Value : DonVi.ID;
                }
            }
            else if (currentUser.ListVaiTro.Any(x => x.MA_VAITRO == PermissionVanbanModel.QLHT_HUYENUY) || currentUser.ListVaiTro.Any(x => x.MA_VAITRO == PermissionVanbanModel.QLHT_XAPHUONG))
            {
                if (currentUser.DeptType == 10)
                {
                    DonViId = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
                }
                else
                {
                    DonViId = currentUser.DeptParentID.GetValueOrDefault();
                }
            }
            else
            {
                DonViId = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
            }
            List<int> Ids = new List<int>();
            Ids.Add(DonViId);
            Ids.AddRange(CCTC_THANHPHANBusiness.GetDSChild(DonViId).Select(x => x.ID).ToList());
            SessionManager.SetValue("ListDonViMan", Ids);
            DM_NGUOIDUNG_SEARCHBO searchModel = new DM_NGUOIDUNG_SEARCHBO();
            if (DonViId == 1)
            {
                searchModel.deptId = null;
            }
            else
            {
                searchModel.deptId = DonViId;
            }
            searchModel.pageSize = MaxPerpage;
            var data = DM_NGUOIDUNGBusiness.GetDaTaByPage(searchModel);
            UserModel model = new UserModel();
            model.ListResult = data;
            model.TreeData = CCTC_THANHPHANBusiness.GetTree(DonViId);
            SessionManager.SetValue("NguoiDungSearchModel", searchModel);
            model.ListChucVu = DM_DANHMUC_DATABusiness.DsByMaNhomNull(DMLOAI_CONSTANT.CHUCVU, 0, true);
            //model.ListPhongBan = CCTC_THANHPHANBusiness.GetDropDownList();
            model.ListPhongBan = CCTC_THANHPHANBusiness.GetDropDownListByListSelect(Ids);

            return View(model);
        }

        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
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

            var data = DM_NGUOIDUNGBusiness.GetDaTaByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchNguoiDung(FormCollection form)
        {
            List<int> ListDeptId = (List<int>)SessionManager.GetValue("ListDonViMan");
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var searchModel = SessionManager.GetValue("NguoiDungSearchModel") as DM_NGUOIDUNG_SEARCHBO;
            if (searchModel == null)
            {
                searchModel = new DM_NGUOIDUNG_SEARCHBO();
                searchModel.pageSize = MaxPerpage;
            }
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            int DONVI_ID = 0;
            AssignUserInfo();
            if (currentUser.ListVaiTro.Any(x => x.MA_VAITRO == PermissionVanbanModel.QLHT))
            {
                DONVI_ID = form["PHONGBAN_ID_sea"].ToIntOrZero();
            }
            if (!string.IsNullOrEmpty(form["PHONGBAN_ID_sea"]))
            {
                var TmpDeptId = form["PHONGBAN_ID_sea"].ToIntOrZero();
                var TmpDeptObj = CCTC_THANHPHANBusiness.Find(TmpDeptId);
                if (TmpDeptObj.TYPE == 10 || TmpDeptObj.TYPE == 11)
                {
                    DONVI_ID = form["PHONGBAN_ID_sea"].ToIntOrZero();
                }
            }
            //if (!ListDeptId.Contains(DONVI_ID))
            //{
            //    DONVI_ID = currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0;
            //}
            //CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(DONVI_ID);
            //if (!DonVi.PARENT_ID.HasValue)
            //{
            //    if (!ListDeptId.Contains(DONVI_ID))
            //    {
            //        DONVI_ID = currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0;
            //    }
            //    searchModel.deptId = DONVI_ID;
            //}
            //else
            //{
            //    searchModel.deptId = null;
            //}
            #region duynt sửa
            //fix luôn id = 1, nếu đơn vị id = nút gốc thì hiển thị hết, ko thì của phòng ban nào thì phòng đó hiển thị            
            if (DONVI_ID == 1 || DONVI_ID == 0)
            {
                searchModel.deptId = null;
            }
            else
            {
                searchModel.deptId = DONVI_ID;
            }
            #endregion
            //if (ListDeptId.IndexOf(DONVI_ID) == 0)
            //{
            //    searchModel.deptId = null;
            //}
            //else
            //{
            //    searchModel.deptId = DONVI_ID;
            //}
            searchModel.sea_HoTen = form["sea_HOTEN"];
            searchModel.sea_TenDangNhap = form["sea_TENDANGNHAP"];
            searchModel.sea_Email = form["sea_EMAIL"];
            searchModel.sea_DienThoai = form["sea_DIENTHOAI"];
            searchModel.sea_CHUCVU_ID = form["CHUCVU_ID_sea"].ToIntOrNULL();
            if (!currentUser.ListVaiTro.Any(x => x.MA_VAITRO == PermissionVanbanModel.QLHT))
            {
                if (DONVI_ID == 0)
                {
                    searchModel.sea_PHONGBAN_ID = form["PHONGBAN_ID_sea"].ToIntOrNULL();
                }
                else
                {
                    searchModel.sea_PHONGBAN_ID = null;
                }

            }
            else
            {
                searchModel.sea_PHONGBAN_ID = null;
            }
            SessionManager.SetValue("NguoiDungSearchModel", searchModel);

            var data = DM_NGUOIDUNGBusiness.GetDaTaByPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }
        public PartialViewResult Edit(long id)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var myModel = new EditVM();
            myModel.objModel = DM_NGUOIDUNGBusiness.repository.Find(id);
            myModel.DsChucVu = DM_DANHMUC_DATABusiness.DsByMaNhomNull(DMLOAI_CONSTANT.CHUCVU, myModel.objModel.CHUCVU_ID.GetValueOrDefault(0), true);
            myModel.LstDonViHienTai = CCTC_THANHPHANBusiness.GetDropDownList(myModel.objModel.DM_PHONGBAN_ID.HasValue ? myModel.objModel.DM_PHONGBAN_ID.Value : 0);
            return PartialView("_EditPartial", myModel);
        }

        [ActionAudit]
        public JsonResult Delete(long id)
        {
            var result = new JsonResultBO(true);
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var userObj = DM_NGUOIDUNGBusiness.Find(id);
            if (userObj != null)
            {
                userObj.IS_ACTIVE = false;
                DM_NGUOIDUNGBusiness.Save(userObj);
                result.Status = true;
                result.Message = "Xóa người dùng thành công";
            }
            else
            {
                result.Status = false;
                result.Message = "Không xóa được người dùng";
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult checkTaiKhoan(string taikhoan)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            return Json(DM_NGUOIDUNGBusiness.CheckExsitTaiKhoan(taikhoan));
        }
        [HttpPost]
        public JsonResult checkEmail(string email, long iduser)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            return Json(DM_NGUOIDUNGBusiness.CheckExsitEmail(email));
        }
        public PartialViewResult Detail(long id)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            NGUOIDUNG_VAITROBusiness = Get<NGUOIDUNG_VAITROBusiness>();
            var model = new DetailVM();
            model.NguoiDung = DM_NGUOIDUNGBusiness.GetDaTaByID(id);
            model.ListVaiTro = NGUOIDUNG_VAITROBusiness.GetVaiTroBYNguoiDung(model.NguoiDung.ID);
            model.ListChucNang = DM_NGUOIDUNGBusiness.GetListThaoTacByNguoiDung(id);
            return PartialView("_DetailPartial", model);
        }
        public PartialViewResult PhanVaiTro(long id)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            NGUOIDUNG_VAITROBusiness = Get<NGUOIDUNG_VAITROBusiness>();
            var myModel = new phanVaiTroVM();
            myModel.NguoiDung = DM_NGUOIDUNGBusiness.repository.Find(id);
            var listVaiTroNgDung = NGUOIDUNG_VAITROBusiness.GetListByNguoiDung(id);
            myModel.DsVaiTro = DM_VAITROBusiness.DsVaiTro(listVaiTroNgDung.Select(x => x.VAITRO_ID.Value).ToList());
            return PartialView("_phanVaiTro", myModel);
        }
        public PartialViewResult resetPass(long id)
        {
            return PartialView("_ChangePass", id);
        }
        [HttpPost]
        [ActionAudit]
        public JsonResult savePass(long taikhoan, string matkhau)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var result = new JsonResultBO(true);
            try
            {
                var ngdung = DM_NGUOIDUNGBusiness.Find(taikhoan);
                ngdung.MAHOA_MK = CommonHelper.MaHoaMatKhau.GenerateRandomString(5);
                ngdung.MATKHAU = CommonHelper.MaHoaMatKhau.Encode_Data(matkhau + ngdung.MAHOA_MK);
                DM_NGUOIDUNGBusiness.Save(ngdung);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không cập nhật được mật khẩu";
            }
            return Json(result);

        }
        public ActionResult PhanQuyen(long id)
        {
            DM_NGUOIDUNG_THAOTACBusiness = Get<DM_NGUOIDUNG_THAOTACBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            DM_CHUCNANGBusiness = Get<DM_CHUCNANGBusiness>();
            VAITRO_THAOTACBusiness = Get<VAITRO_THAOTACBusiness>();
            NGUOIDUNG_VAITROBusiness = Get<NGUOIDUNG_VAITROBusiness>();
            var myModel = new PhanQuyenVM();
            myModel.NguoiDungThaoTac = DM_NGUOIDUNG_THAOTACBusiness.GetByUserID(id);
            myModel.NguoiDung = DM_NGUOIDUNGBusiness.repository.Find(id);
            var listVaiTroNgDung = NGUOIDUNG_VAITROBusiness.GetListByNguoiDung(id);
            myModel.ListAllChucNang = DM_CHUCNANGBusiness.getChucNangThaoTac();

            //myModel.ListChucNangVaiTro = VAITRO_THAOTACBusiness.getChucNangCuaVaiTro(id);
            //myModel. = DM_VAITROBusiness.DsVaiTro(listVaiTroNgDung.Select(x => x.VAITRO_ID.Value).ToList());
            return View(myModel);
        }
        [HttpPost]
        [ActionAudit]
        public JsonResult SaveQuyen(long nguoidungid, List<long> ArrThaoTac, List<int> ArrTrangThai)
        {
            DM_NGUOIDUNG_THAOTACBusiness = Get<DM_NGUOIDUNG_THAOTACBusiness>();
            return Json(DM_NGUOIDUNG_THAOTACBusiness.SaveQuyenNguoiDung(nguoidungid, ArrThaoTac, ArrTrangThai));
        }

        [HttpPost]
        public JsonResult Edit(FormCollection collection)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            AssignUserInfo();
            var result = new JsonResultBO(true);
            try
            {
                var id = collection["ID"].ToLongOrZero(); ;
                var myobj = DM_NGUOIDUNGBusiness.Find(id);
                myobj.NGAYSUA = DateTime.Now;
                myobj.NGAYSINH = collection["NGAYSINH"].ToDateTime();
                myobj.DIENTHOAI = collection["DIENTHOAI"].ToString();
                myobj.NGUOISUA = currentUser.HOTEN;
                myobj.HOTEN = collection["HOTEN"].ToString();
                myobj.EMAIL = collection["EMAIL"].ToString();
                myobj.DIACHI = collection["DIACHI"].ToString();
                myobj.CHUCVU_ID = collection["CHUCVU_ID"].ToIntOrNULL();
                myobj.DM_PHONGBAN_ID = collection["PHONGBAN_ID"].ToIntOrNULL();
                myobj.IS_ACTIVE = true;
                DM_NGUOIDUNGBusiness.Save(myobj);
            }
            catch
            {
                result.Status = false;
                result.Message = "Không cập nhật được";
            }
            return Json(result);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var model = new CreateVM();
            model.DsChucVu = DM_DANHMUC_DATABusiness.DsByMaNhomNull(DMLOAI_CONSTANT.CHUCVU, 0, true);
            model.LstDonViHienTai = CCTC_THANHPHANBusiness.GetDropDownList(1);
            return PartialView("_CreatePartial", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionAudit]
        public JsonResult Create(FormCollection form)
        {
            var result = new JsonResultBO(true);
            try
            {
                DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                var ngdung = new DM_NGUOIDUNG();
                ngdung.ANH_DAIDIEN = form["ANHDAIDIEN"];
                var mk = form["MATKHAU"];
                ngdung.MAHOA_MK = CommonHelper.MaHoaMatKhau.GenerateRandomString(5);
                ngdung.MATKHAU = CommonHelper.MaHoaMatKhau.Encode_Data(mk + ngdung.MAHOA_MK);
                ngdung.DIACHI = form["DIACHI"];
                ngdung.DIENTHOAI = form["DIENTHOAI"];
                ngdung.EMAIL = form["EMAIL"];
                ngdung.HOTEN = form["HOTEN"];
                ngdung.NGAYSINH = form["NGAYSINH"].ToDataTime();
                ngdung.TENDANGNHAP = form["TENDANGNHAP"];
                ngdung.TRANGTHAI = 1;
                ngdung.DM_PHONGBAN_ID = form["ID_PHONGBAN"].ToIntOrZero();
                ngdung.CHUCVU_ID = form["CHUCVU_ID"].ToIntOrNULL();
                ngdung.IS_ACTIVE = true;
                var searchModel = SessionManager.GetValue("NguoiDungSearchModel") as DM_NGUOIDUNG_SEARCHBO;
                if (searchModel != null && searchModel.deptId != null && searchModel.deptId > 1)
                {
                    ngdung.DM_PHONGBAN_ID = searchModel.deptId;
                }

                DM_NGUOIDUNGBusiness.Save(ngdung);
            }
            catch
            {

                result.Status = false;
                result.Message = "Không tạo được người dùng";
            }


            return Json(result);
        }

        //#region Import người dùng
        //public ActionResult Import()
        //{
        //    return View();
        //}

        [HttpGet]
        public ActionResult Export()
        {
            AssignUserInfo();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();

            int DonViId = 0;
            if (currentUser.ListVaiTro.Any(x => x.MA_VAITRO == PermissionVanbanModel.QLHT))
            {
                //Cấp lãnh đạo
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                if (DonVi != null)
                {
                    DonViId = DonVi.PARENT_ID.HasValue ? DonVi.PARENT_ID.Value : DonVi.ID;
                }
            }
            else if (currentUser.ListVaiTro.Any(x => x.MA_VAITRO == PermissionVanbanModel.QLHT_HUYENUY) || currentUser.ListVaiTro.Any(x => x.MA_VAITRO == PermissionVanbanModel.QLHT_XAPHUONG))
            {
                if (currentUser.DeptType == 10)
                {
                    DonViId = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
                }
                else
                {
                    DonViId = currentUser.DeptParentID.GetValueOrDefault();
                }
            }
            else
            {
                DonViId = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
            }

            DM_NGUOIDUNG_SEARCHBO searchModel = SessionManager.GetValue("NguoiDungSearchModel") as DM_NGUOIDUNG_SEARCHBO;

            if (searchModel == null)
            {
                searchModel = new DM_NGUOIDUNG_SEARCHBO();
                if (DonViId == 1)
                {
                    searchModel.deptId = null;
                }
                else
                {
                    searchModel.deptId = DonViId;
                }
            }
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("STT", "STT");
            properties.Add("TENDANGNHAP", "Tên đăng nhập");
            properties.Add("HOTEN", "Họ tên");
            properties.Add("EMAIL", "Email");
            properties.Add("ChucVu", "Chức vụ");
            properties.Add("TEN_VAITRO", "Vai trò người dùng");
            properties.Add("TenPhongBan", "Đơn vị/Phòng ban");

            var users = DM_NGUOIDUNGBusiness.GetDaTaByPage(searchModel, 0, -1);
            EPPlusSupplier<DM_NGUOIDUNG_BO> supplier = new EPPlusSupplier<DM_NGUOIDUNG_BO>();
            supplier.properties = properties;
            supplier.startColumn = 1;
            supplier.startRow = 6;
            supplier.fileName = "Danh sách người dùng";
            supplier.leftAlignColumn = new int[] { 2, 3, 4, 5, 6, 7 };

            var stream = supplier.CreateExcelFile(users.ListItem, FormatWorkSheet);
            var buffer = stream as MemoryStream;

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", supplier.fileName + ".xlsx"));
            Response.BinaryWrite(buffer.ToArray());
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");
        }

        public ExcelWorksheet FormatWorkSheet(ExcelWorksheet sheet, string title)
        {
            sheet.DefaultColWidth = 30;
            sheet.Column(3).Width = 50;
            sheet.Column(1).Width = 10;
            sheet.Column(4).Width = 20;
            sheet.Column(2).Width = 25;
            sheet.Column(5).Width = 25;
            sheet.Column(6).Width = 30;
            sheet.Column(7).Width = 20;
            sheet.Column(8).Width = 20;
            sheet.Column(9).Width = 20;

            sheet.Cells.Style.Font.Name = "Times New Roman";

            ExcelRange titleRange = sheet.SelectedRange["A1:G1"];
            titleRange.Value = null;
            titleRange.Merge = true;
            titleRange.Value = "DANH SÁCH NGƯỜI DÙNG";
            titleRange.Style.Font.Size = 22;
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            titleRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Row(1).Height = 30;


            ExcelRange userRange = sheet.SelectedRange["A3:G3"];
            userRange.Value = null;
            userRange.Merge = true;
            userRange.Value = "Cán bộ thống kê: " + currentUser.HOTEN + " - Loại: " + title;
            userRange.Style.Font.Size = 14;
            userRange.Style.Font.Bold = true;
            userRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            userRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            userRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Row(3).Height = 30;

            ExcelRange timeRange = sheet.SelectedRange["A4:G4"];
            timeRange.Value = null;
            timeRange.Merge = true;
            timeRange.Value = string.Format("(Ngày thống kê: {0})", DateTime.Now.ToVietnameseDateFormat());
            timeRange.Style.Font.Size = 14;
            timeRange.Style.Font.Bold = true;
            timeRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            timeRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            timeRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Row(4).Height = 30;

            return sheet;
        }


        //[ActionAudit]
        //public JsonResult SaveImportUser(FormCollection col)
        //{
        //    string defaultPassowrd = "12345678";
        //    HttpPostedFileBase file = Request.Files["filebase"];
        //    AssignUserInfo();
        //    #region Lưu file xuống ổ cứng
        //    if (file == null)
        //    {
        //        return Json(new { Type = "ERROR", Message = "Không tìm thấy file được tải lên" });
        //    }
        //    List<string> excelExtension = new List<string>();
        //    excelExtension.Add(".XLS");
        //    excelExtension.Add(".XLSX");
        //    var extension = Path.GetExtension(file.FileName);
        //    if (!excelExtension.Contains(extension.ToUpper()))
        //    {
        //        return Json(new { Type = "ERROR", Message = "File không đúng định dạng, bạn hãy chọn file có định dạng .xls hoặc .xlsx" });
        //    }
        //    var fileSize = file.ContentLength;
        //    var fileMB = (fileSize / 1024f) / 1024f;
        //    if (fileMB > 10)
        //    {
        //        return Json(new { Type = "ERROR", Message = "Dung lượng file không được lớn hơn 10MB" });
        //    }
        //    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        //    fileName = fileName + "-" + currentUser.ID + "-" + Guid.NewGuid().ToString() + extension;
        //    var physicalPath = Server.MapPath("~/Uploads/ImportNguoiDung") + "/" + fileName;
        //    FileUltilities file1 = new FileUltilities();
        //    if (!System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads/ImportNguoiDung/"))))
        //    {
        //        file1.CreateFolder(Server.MapPath("~/Uploads/ImportNguoiDung/"));
        //    }
        //    file.SaveAs(physicalPath);
        //    #endregion
        //    #region Đọc file import
        //    IVTWorkbook oBook = VTExport.OpenWorkbook(physicalPath);
        //    IVTWorksheet sheet = oBook.GetSheet(1);
        //    string error = "";
        //    int i = 2;
        //    var numberRegex = new Regex("[0-9]+");
        //    var emailRegex = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
        //    List<string> ListValue = new List<string>();
        //    string fullName;
        //    string email;
        //    string phone;
        //    string address;
        //    string birthdate;
        //    string position;
        //    string department;
        //    string userName = "";
        //    string vaitro = "";
        //    DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
        //    NGUOIDUNG_VAITROBusiness = Get<NGUOIDUNG_VAITROBusiness>();
        //    DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
        //    CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
        //    DM_VAITROBusiness = Get<DM_VAITROBusiness>();
        //    List<DM_VAITRO> ListVaiTro = DM_VAITROBusiness.repository.All().ToList();
        //    List<CCTC_THANHPHAN> ListDonVi = CCTC_THANHPHANBusiness.GetData();
        //    List<DM_NGUOIDUNG> ListNguoiDung = DM_NGUOIDUNGBusiness.GetData();
        //    List<DM_DANHMUC_DATA> ListChucVu = DM_DANHMUC_DATABusiness.GetDataByCode(DMLOAI_CONSTANT.CHUCVU);
        //    List<DM_NGUOIDUNG_BO> ListSuccess = new List<DM_NGUOIDUNG_BO>();
        //    DM_DANHMUC_DATA ChucVu = null;
        //    CCTC_THANHPHAN DonVi = null;
        //    DM_VAITRO VaiTro = null;
        //    do
        //    {
        //        ChucVu = null;
        //        DonVi = null;
        //        VaiTro = null;
        //        error = "";
        //        DM_NGUOIDUNG_BO NguoiDung = new DM_NGUOIDUNG_BO();

        //        userName = sheet.GetCellValue(i, 1) != null ? sheet.GetCellValue(i, 1).ToString().Trim() : "";
        //        vaitro = sheet.GetCellValue(i, 2) != null ? sheet.GetCellValue(i, 2).ToString().Trim() : "";
        //        fullName = sheet.GetCellValue(i, 3) != null ? sheet.GetCellValue(i, 3).ToString().Trim() : "";
        //        email = sheet.GetCellValue(i, 4) != null ? sheet.GetCellValue(i, 4).ToString().Trim() : "";
        //        phone = sheet.GetCellValue(i, 5) != null ? sheet.GetCellValue(i, 5).ToString().Trim() : "";
        //        address = sheet.GetCellValue(i, 6) != null ? sheet.GetCellValue(i, 6).ToString().Trim() : "";
        //        birthdate = sheet.GetCellValue(i, 7) != null ? sheet.GetCellValue(i, 7).ToString().Trim() : "";
        //        position = sheet.GetCellValue(i, 8) != null ? sheet.GetCellValue(i, 8).ToString().Trim() : "";
        //        department = sheet.GetCellValue(i, 9) != null ? sheet.GetCellValue(i, 9).ToString().Trim() : "";
        //        i++;
        //        if (string.IsNullOrEmpty(fullName))
        //        {
        //            error += "<li>Bạn chưa nhập họ và tên</li>";
        //        }
        //        else
        //        {
        //            fullName = fullName.Trim();
        //        }
        //        if (string.IsNullOrEmpty(userName))
        //        {
        //            error += "<li>Bạn chưa nhập tên đăng nhập</li>";
        //        }
        //        else
        //        {
        //            userName = GenUserName(userName.Trim(), ListNguoiDung);
        //        }
        //        if (string.IsNullOrEmpty(email))
        //        {
        //            error += "<li>Bạn chưa nhập email</li>";
        //        }
        //        else if (!emailRegex.IsMatch(email))
        //        {
        //            error += "<li>Email không đúng định dạng</li>";
        //        }
        //        else
        //        {
        //            email = email.Trim();
        //        }
        //        if (string.IsNullOrEmpty(position))
        //        {
        //            error += "<li>Bạn chưa chọn chức vụ</li>";
        //        }
        //        else
        //        {
        //            ChucVu = ListChucVu.Where(x => x.TEXT.ToLower().Equals(position.Trim().ToLower())).FirstOrDefault();
        //            if (ChucVu == null)
        //            {
        //                error += "<li>Chức vụ không tồn tại</li>";
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(phone) && !numberRegex.IsMatch(phone))
        //        {
        //            error += "<li>Số điện thoại phải là số</li>";
        //        }
        //        if (!string.IsNullOrEmpty(department))
        //        {
        //            DonVi = ListDonVi.Where(x => x.CODE.ToLower().Equals(department.ToLower())).FirstOrDefault();
        //            if (DonVi == null)
        //            {
        //                error += "<li>Đơn vị không tồn tại</li>";
        //            }
        //        }
        //        NguoiDung.EMAIL = email;
        //        NguoiDung.ChucVu = position;
        //        NguoiDung.CHUCVU_ID = ChucVu != null ? ChucVu.ID : 0;
        //        NguoiDung.DIENTHOAI = phone;
        //        NguoiDung.DIACHI = address;
        //        NguoiDung.HOTEN = fullName;
        //        NguoiDung.MAHOA_MK = MaHoaMatKhau.GenerateRandomString(5);
        //        NguoiDung.MATKHAU = MaHoaMatKhau.Encode_Data(defaultPassowrd + NguoiDung.MAHOA_MK);
        //        if (!string.IsNullOrEmpty(birthdate))
        //        {
        //            try
        //            {
        //                NguoiDung.NGAYSINH = DateTime.Parse(birthdate);
        //            }
        //            catch (Exception ex)
        //            {
        //                error += "<li>Ngày sinh không đúng định dạng hoặc không tồn tại</li>";
        //            }
        //        }
        //        NguoiDung.NGAYSINH_TEXT = birthdate;
        //        NguoiDung.NGAYSUA = DateTime.Now;
        //        NguoiDung.NGAYTAO = DateTime.Now;
        //        NguoiDung.NGUOISUA = currentUser.TENDANGNHAP;
        //        NguoiDung.NGUOITAO = currentUser.TENDANGNHAP;
        //        NguoiDung.TENDANGNHAP = userName;
        //        NguoiDung.TEN_DONVI = department;
        //        if(DonVi != null)
        //        {
        //            NguoiDung.DM_PHONGBAN_ID = DonVi.ID;
        //        }

        //        NguoiDung.ERROR = error;
        //        if (string.IsNullOrEmpty(error))
        //        {
        //            DM_NGUOIDUNG temp = NguoiDung.ToModel();
        //            temp.IS_ACTIVE = true;
        //            ListNguoiDung.Add(temp);
        //            if(DM_NGUOIDUNGBusiness.repository.All().Where(x => x.TENDANGNHAP == temp.TENDANGNHAP).FirstOrDefault() == null)
        //            {
        //                DM_NGUOIDUNGBusiness.Save(temp);

        //                VaiTro = ListVaiTro.Where(x => x.TEN_VAITRO.ToLower().Equals(vaitro.Trim().ToLower())).FirstOrDefault();
        //                if (VaiTro != null)
        //                {
        //                    NGUOIDUNG_VAITRO nguoidungvaitro = new NGUOIDUNG_VAITRO();
        //                    nguoidungvaitro.NGUOIDUNG_ID = temp.ID;
        //                    nguoidungvaitro.VAITRO_ID = VaiTro.DM_VAITRO_ID;
        //                    NGUOIDUNG_VAITROBusiness.Save(nguoidungvaitro);
        //                }
        //            }

        //        }
        //        ListSuccess.Add(NguoiDung);
        //    } while (!IsEndFile(i, sheet, ListValue));
        //    #endregion
        //    try
        //    {
        //        file1.RemoveFile(physicalPath);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    //result.ListError = ListFail;
        //    return Json(new { Type = "SUCCESS", Message = ListSuccess });
        //}
        //private string GenUserName(string userName, List<DM_NGUOIDUNG> ListNguoiDung)
        //{
        //    //List<string> ListWord = userName.ToListStringLower(' ');
        //    //string userNameTemp = "";
        //    //int size = ListWord.Count;
        //    //for (int i = 0; i < size - 1; i++)
        //    //{
        //    //    userNameTemp += ListWord[i].Substring(0, 1);
        //    //}
        //    //userNameTemp += ListWord[size - 1];
        //    //var size = ListNguoiDung.Where(x => x.TENDANGNHAP.Equals(userName)).Count();
        //    //if (size > 0)
        //    //{
        //    //    userName += size;
        //    //}
        //    return userName;
        //}
        //private bool IsEndFile(int row, IVTWorksheet sheet, List<string> ListValue)
        //{
        //    ListValue.Clear();
        //    ListValue.Add(sheet.GetCellValue(row, 1) == null ? "" : sheet.GetCellValue(row, 1).ToString().Trim());
        //    ListValue.Add(sheet.GetCellValue(row, 2) == null ? "" : sheet.GetCellValue(row, 2).ToString().Trim());
        //    ListValue.Add(sheet.GetCellValue(row, 3) == null ? "" : sheet.GetCellValue(row, 3).ToString().Trim());
        //    ListValue.Add(sheet.GetCellValue(row, 4) == null ? "" : sheet.GetCellValue(row, 4).ToString().Trim());
        //    ListValue.Add(sheet.GetCellValue(row, 5) == null ? "" : sheet.GetCellValue(row, 5).ToString().Trim());
        //    ListValue.Add(sheet.GetCellValue(row, 6) == null ? "" : sheet.GetCellValue(row, 6).ToString().Trim());
        //    ListValue.Add(sheet.GetCellValue(row, 7) == null ? "" : sheet.GetCellValue(row, 7).ToString().Trim());
        //    ListValue.Add(sheet.GetCellValue(row, 8) == null ? "" : sheet.GetCellValue(row, 8).ToString().Trim());
        //    ListValue.Add(sheet.GetCellValue(row, 9) == null ? "" : sheet.GetCellValue(row, 9).ToString().Trim());
        //    return ListValue.Where(x => !string.IsNullOrEmpty(x)).Count() == 0;
        //}
        //#endregion
        #region Import người dùng
        public ActionResult import()
        {
            ImportNguoiDungVM model = new ImportNguoiDungVM();
            model.PathTemplate = Path.Combine(HostUpload, WebConfigurationManager.AppSettings["ImportNguoiDungTemplate"]);
            return View(model);
        }
        [HttpPost]
        public JsonResult CheckImport(HttpPostedFileBase fileImport)
        {
            JsonResultImportBO<DM_NGUOIDUNG_IMPORTBO> result = new JsonResultImportBO<DM_NGUOIDUNG_IMPORTBO>(true);
            var DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
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
                var importHelper = new ImportExcelHelper<DM_NGUOIDUNG_IMPORTBO>();
                importHelper.PathTemplate = saveFileResult.fullPath;
                importHelper.ConfigColumn = new List<ConfigModule>();

                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "TENDANGNHAP",
                    require = true,
                    TypeValue = typeof(string).FullName,
                });
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "VAITRO",
                    require = true,
                    TypeValue = typeof(string).FullName,
                });
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "HOTEN",
                    require = true,
                    TypeValue = typeof(string).FullName,
                });
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "EMAIL",
                    require = true,
                    TypeValue = typeof(string).FullName,
                });
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "SODIENTHOAI",
                    require = false,
                    TypeValue = typeof(string).FullName,
                });
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "DIACHI",
                    require = false,
                    TypeValue = typeof(DateTime).FullName,
                });
                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "NGAYSINH",
                    require = false,
                    TypeValue = typeof(DateTime).FullName,
                });

                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "CHUCVU",
                    require = false,
                    TypeValue = typeof(string).FullName,
                });

                importHelper.ConfigColumn.Add(new ConfigModule()
                {
                    columnName = "PHONGBAN",
                    require = false,
                    TypeValue = typeof(string).FullName,
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
        public JsonResult SaveImportData(List<List<string>> Data)
        {
            var result = new JsonResultBO(true);
            AssignUserInfo();
            var CheckUpdate = false;
            var CheckVaitro = false;
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var deptid = 2948;

            var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            var CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var DM_VAITROBusiness = Get<DM_VAITROBusiness>();
            var NGUOIDUNG_VAITROBusiness = Get<NGUOIDUNG_VAITROBusiness>();
            var listUser = DM_NGUOIDUNGBusiness.repository.All().ToList();
            var LstChucVu = DM_DANHMUC_DATABusiness.DsByMaNhomNull("DMCHUCVU");

            //var LstPhongBan = CCTC_THANHPHANBusiness.GetDropDownListByCode();
            var LstPhongBan =
                CCTC_THANHPHANBusiness.repository.All().Where(x => x.ID > deptid).Select(x => new SelectListItem()
                {
                    Value = x.CODE.ToString(),
                    Text = x.NAME
                }).ToList();
            var LstPhongBanCode =
                CCTC_THANHPHANBusiness.repository.All().Where(x => x.ID > deptid).Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.CODE
                }).ToList();
            var ListVaitro = DM_VAITROBusiness.ListVaiTro();

            var lstObjSave = new List<DM_NGUOIDUNG>();
            var lstVaitro = new List<NGUOIDUNG_VAITRO>();
            try
            {
                var count = 0;
                foreach (var item in Data)
                {
                    count++;
                    var tmpUserName = item[0];
                    if (string.IsNullOrEmpty(tmpUserName))
                    {
                        break;
                    }
                    var newObj = DM_NGUOIDUNGBusiness.repository.All().Where(x => x.TENDANGNHAP == tmpUserName.Trim())
                        .FirstOrDefault();

                    if (newObj == null)
                    {
                        newObj = new DM_NGUOIDUNG();
                    }
                    var username = item[0].Trim().ToString();
                    newObj.TENDANGNHAP = username;

                    //vai trò
                    var StrVaiTro = item[1];

                    var Ngdungvaitro = NGUOIDUNG_VAITROBusiness.repository.All().Where(x => x.NGUOIDUNG_ID == newObj.ID).FirstOrDefault();

                    if (Ngdungvaitro == null)
                    {
                        Ngdungvaitro = new NGUOIDUNG_VAITRO();
                    }
                    if (!string.IsNullOrEmpty(StrVaiTro))
                    {
                        var DataVaiTroObj = ListVaitro.Where(x => x.Text.Trim().ToLower().Equals(StrVaiTro.Trim().ToLower()))
                            .FirstOrDefault();
                        if (DataVaiTroObj != null)
                        {
                            Ngdungvaitro.VAITRO_ID = DataVaiTroObj.Value.ToIntOrZero();
                        }
                        else
                        {
                            var tmp = StrVaiTro;
                        }
                    }
                    //họ tên
                    newObj.HOTEN = item[2];
                    //email
                    newObj.EMAIL = item[3];
                    //số điện thoại
                    newObj.DIENTHOAI = item[4];
                    //địa chỉ
                    newObj.DIACHI = item[5];
                    //ngaysinh
                    //pass
                    newObj.MAHOA_MK = "Sm2Rk";
                    newObj.MATKHAU = "205fc640e6438758363930909f571043";
                    if (!string.IsNullOrEmpty(item[6]))
                    {
                        try
                        {
                            newObj.NGAYSINH = item[6].ToDateTime();
                        }
                        catch (Exception ex)
                        {
                            newObj.NGAYSINH = null;
                        }

                    }
                    //chuc vu
                    var StrChucVu = item[7];
                    if (!string.IsNullOrEmpty(StrChucVu))
                    {
                        var DataChucVuObj = LstChucVu.Where(x => x.Text.ToLower().Equals(StrChucVu.ToLower()))
                            .FirstOrDefault();
                        if (DataChucVuObj != null)
                        {
                            newObj.CHUCVU_ID = DataChucVuObj.Value.ToIntOrZero();
                        }
                        else
                        {
                            DM_DANHMUC_DATA data = new DM_DANHMUC_DATA();
                            data.DM_NHOM_ID = 10;
                            data.TEXT = StrChucVu.ToLower();
                            DM_DANHMUC_DATABusiness.Save(data);
                            newObj.CHUCVU_ID = data.ID;
                        }
                    }

                    var StrPhongBan = item[8];
                    if (!string.IsNullOrEmpty(StrPhongBan))
                    {
                        var DataPhongBanObj = LstPhongBan.Where(x => x.Text.Trim().ToLower().Equals(StrPhongBan.Trim().ToLower()))
                            .FirstOrDefault();
                        if (DataPhongBanObj != null)
                        {
                            if (StrVaiTro == "Ban lãnh đạo")
                            {
                                var finalDeptCODE = "BLD" + DataPhongBanObj.Value;
                                var dept =
                                    LstPhongBanCode.Where(x => x.Text.Trim().ToLower().Equals(finalDeptCODE.Trim().ToLower()))
                                        .FirstOrDefault();
                                if (dept != null)
                                {
                                    newObj.DM_PHONGBAN_ID = dept.Value.ToIntOrZero();
                                }
                            }
                            else
                            {
                                var finalDeptCode = "VP" + DataPhongBanObj.Value;
                                var dept =
                                    LstPhongBanCode.Where(x => x.Text.Trim().ToLower().Equals(finalDeptCode.Trim().ToLower()))
                                        .FirstOrDefault();
                                if (dept != null)
                                {
                                    newObj.DM_PHONGBAN_ID = dept.Value.ToIntOrZero();
                                }
                            }

                        }
                        else
                        {
                            //break;
                            continue;
                        }
                    }

                    newObj.IS_ACTIVE = true;

                    if (newObj.ID > 0)
                    {
                        DM_NGUOIDUNGBusiness.Save(newObj);
                        CheckUpdate = true;
                        NGUOIDUNG_VAITROBusiness.Save(Ngdungvaitro);
                        CheckVaitro = true;
                    }
                    else
                    {
                        //lstObjSave.Add(newObj);
                        DM_NGUOIDUNGBusiness.Save(newObj);
                        Ngdungvaitro.NGUOIDUNG_ID = newObj.ID;
                        NGUOIDUNG_VAITROBusiness.Save(Ngdungvaitro);
                        //lstVaitro.Add(Ngdungvaitro);
                    }

                }

                //if (lstObjSave.Count > 0)
                //{
                //    result = DM_NGUOIDUNGBusiness.saveImport(lstObjSave);
                //    NGUOIDUNG_VAITROBusiness.saveImport(lstVaitro);
                //    for (int i = 0; i < lstObjSave.Count; i++)
                //    {
                //        var idvaitro = lstVaitro[i].ID;
                //        var mymodel = NGUOIDUNG_VAITROBusiness.repository.All().Where(x => x.ID == idvaitro).FirstOrDefault();
                //        mymodel.NGUOIDUNG_ID = lstObjSave[i].ID;
                //        NGUOIDUNG_VAITROBusiness.Save(mymodel);
                //    }

                //}
                //else
                //{
                //    if (CheckUpdate == false)
                //    {
                //        result.Status = false;
                //        result.Message = "Không có dữ liệu để lưu";
                //    }
                //    else
                //    {
                //        result.Status = true;
                //        result.Message = "Cập nhật dữ liệu thành công";
                //    }
                //}

            }
            catch
            {
                result.Status = false;
                result.Message = "Lỗi dữ liệu, không thể import";
            }

            return Json(result);
        }
        [HttpPost]
        //public JsonResult SaveImportData(List<List<string>> Data)
        //{
        //    var result = new JsonResultBO(true);
        //    AssignUserInfo();
        //    var CheckUpdate = false;
        //    var CheckVaitro = false;
        //    DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
        //    var deptid = 2168;

        //    var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
        //    var CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
        //    var DM_VAITROBusiness = Get<DM_VAITROBusiness>();
        //    var NGUOIDUNG_VAITROBusiness = Get<NGUOIDUNG_VAITROBusiness>();
        //    var listUser = DM_NGUOIDUNGBusiness.repository.All().ToList();
        //    var LstChucVu = DM_DANHMUC_DATABusiness.DsByMaNhomNull("DMCHUCVU");

        //    var LstPhongBan = CCTC_THANHPHANBusiness.GetDropDownListByCode();
        //    //var LstPhongBan =
        //    //    CCTC_THANHPHANBusiness.repository.All().Where(x => x.ID > deptid).Select(x => new SelectListItem()
        //    //    {
        //    //        Value = x.CODE.ToString(),
        //    //        Text = x.NAME
        //    //    }).ToList();
        //    //var LstPhongBanCode =
        //    //    CCTC_THANHPHANBusiness.repository.All().Where(x => x.ID > deptid).Select(x => new SelectListItem()
        //    //    {
        //    //        Value = x.ID.ToString(),
        //    //        Text = x.CODE
        //    //    }).ToList();
        //    var ListVaitro = DM_VAITROBusiness.ListVaiTro();

        //    var lstObjSave = new List<DM_NGUOIDUNG>();
        //    var lstVaitro = new List<NGUOIDUNG_VAITRO>();
        //    try
        //    {
        //        var count = 0;
        //        foreach (var item in Data)
        //        {
        //            count++;
        //            var tmpUserName = item[0];
        //            if (string.IsNullOrEmpty(tmpUserName))
        //            {
        //                break;
        //            }
        //            var newObj = DM_NGUOIDUNGBusiness.repository.All().Where(x => x.TENDANGNHAP == tmpUserName.Trim())
        //                .FirstOrDefault();

        //            if (newObj == null)
        //            {
        //                newObj = new DM_NGUOIDUNG();
        //            }
        //            var username = item[0].Trim().ToString();
        //            newObj.TENDANGNHAP = username;

        //            //vai trò
        //            var StrVaiTro = item[1];

        //            var Ngdungvaitro = NGUOIDUNG_VAITROBusiness.repository.All().Where(x => x.NGUOIDUNG_ID == newObj.ID).FirstOrDefault();

        //            if (Ngdungvaitro == null)
        //            {
        //                Ngdungvaitro = new NGUOIDUNG_VAITRO();
        //            }
        //            if (!string.IsNullOrEmpty(StrVaiTro))
        //            {
        //                var DataVaiTroObj = ListVaitro.Where(x => x.Text.Trim().ToLower().Equals(StrVaiTro.Trim().ToLower()))
        //                    .FirstOrDefault();
        //                if (DataVaiTroObj != null)
        //                {
        //                    Ngdungvaitro.VAITRO_ID = DataVaiTroObj.Value.ToIntOrZero();
        //                }
        //                else
        //                {
        //                    var tmp = StrVaiTro;
        //                }
        //            }
        //            //họ tên
        //            newObj.HOTEN = item[2];
        //            //email
        //            newObj.EMAIL = item[3];
        //            //số điện thoại
        //            newObj.DIENTHOAI = item[4];
        //            //địa chỉ
        //            newObj.DIACHI = item[5];
        //            //ngaysinh
        //            //pass
        //            newObj.MAHOA_MK = "Sm2Rk";
        //            newObj.MATKHAU = "205fc640e6438758363930909f571043";
        //            if (!string.IsNullOrEmpty(item[6]))
        //            {
        //                try
        //                {
        //                    newObj.NGAYSINH = item[6].ToDateTime();
        //                }
        //                catch (Exception ex)
        //                {
        //                    newObj.NGAYSINH = null;
        //                }

        //            }
        //            //chuc vu
        //            var StrChucVu = item[7];
        //            if (!string.IsNullOrEmpty(StrChucVu))
        //            {
        //                var DataChucVuObj = LstChucVu.Where(x => x.Text.ToLower().Equals(StrChucVu.ToLower()))
        //                    .FirstOrDefault();
        //                if (DataChucVuObj != null)
        //                {
        //                    newObj.CHUCVU_ID = DataChucVuObj.Value.ToIntOrZero();
        //                }
        //                else
        //                {
        //                    DM_DANHMUC_DATA data = new DM_DANHMUC_DATA();
        //                    data.DM_NHOM_ID = 10;
        //                    data.TEXT = StrChucVu.ToLower();
        //                    DM_DANHMUC_DATABusiness.Save(data);
        //                    newObj.CHUCVU_ID = data.ID;
        //                }
        //            }

        //            var StrPhongBan = item[8];
        //            if (!string.IsNullOrEmpty(StrPhongBan))
        //            {
        //                var DataPhongBanObj = LstPhongBan.Where(x => x.Text.Trim().ToLower().Equals(StrPhongBan.Trim().ToLower()))
        //                    .FirstOrDefault();
        //                if (DataPhongBanObj != null)
        //                {
        //                    //if (StrVaiTro == "Ban lãnh đạo")
        //                    //{
        //                    //    var finalDeptCODE = "BLD" + DataPhongBanObj.Value;
        //                    //    var dept =
        //                    //        LstPhongBanCode.Where(x => x.Text.Trim().ToLower().Equals(finalDeptCODE.Trim().ToLower()))
        //                    //            .FirstOrDefault();
        //                    //    if (dept != null)
        //                    //    {
        //                    //        newObj.DM_PHONGBAN_ID = dept.Value.ToIntOrZero();
        //                    //    }
        //                    //}
        //                    //else
        //                    //{
        //                    //    var finalDeptCode = "VP" + DataPhongBanObj.Value;
        //                    //    var dept =
        //                    //        LstPhongBanCode.Where(x => x.Text.Trim().ToLower().Equals(finalDeptCode.Trim().ToLower()))
        //                    //            .FirstOrDefault();
        //                    //    if (dept != null)
        //                    //    {
        //                    //        newObj.DM_PHONGBAN_ID = dept.Value.ToIntOrZero();
        //                    //    }
        //                    //}
        //                    newObj.DM_PHONGBAN_ID = DataPhongBanObj.Value.ToIntOrZero();

        //                }
        //                else
        //                {
        //                    //break;
        //                    continue;
        //                }
        //            }

        //            newObj.IS_ACTIVE = true;

        //            if (newObj.ID > 0)
        //            {
        //                DM_NGUOIDUNGBusiness.Save(newObj);
        //                NGUOIDUNG_VAITROBusiness.Save(Ngdungvaitro);
        //            }
        //            else
        //            {
        //                //lstObjSave.Add(newObj);
        //                DM_NGUOIDUNGBusiness.Save(newObj);
        //                Ngdungvaitro.NGUOIDUNG_ID = newObj.ID;
        //                NGUOIDUNG_VAITROBusiness.Save(Ngdungvaitro);
        //                //lstVaitro.Add(Ngdungvaitro);
        //            }

        //        }

           
        //    }
        //    catch
        //    {
        //        result.Status = false;
        //        result.Message = "Lỗi dữ liệu, không thể import";
        //    }

        //    return Json(result);
        //}
        public JsonResult GetExportError(List<List<string>> lstData)
        {
            ExportExcelHelper<DM_NGUOIDUNG> exPro = new CommonHelper.ExportExcelHelper<DM_NGUOIDUNG>();
            exPro.PathStore = Path.Combine(UPLOADFOLDER, "ErrorExport");
            exPro.PathTemplate = Path.Combine(UPLOADFOLDER, WebConfigurationManager.AppSettings["ImportNguoiDungTemplate"]);
            exPro.StartRow = 5;
            exPro.StartCol = 2;
            exPro.FileName = "ErrorExportNguoiDung";
            var result = exPro.ExportText(lstData);
            if (result.Status)
            {
                result.PathStore = Path.Combine(HostUpload, "ErrorExport", result.FileName);
            }
            return Json(result);
        }

        private bool IsEndFile(int row, IVTWorksheet sheet, List<string> ListValue)
        {
            ListValue.Clear();
            ListValue.Add(sheet.GetCellValue(row, 1) == null ? "" : sheet.GetCellValue(row, 1).ToString().Trim());
            ListValue.Add(sheet.GetCellValue(row, 2) == null ? "" : sheet.GetCellValue(row, 2).ToString().Trim());
            ListValue.Add(sheet.GetCellValue(row, 3) == null ? "" : sheet.GetCellValue(row, 3).ToString().Trim());
            ListValue.Add(sheet.GetCellValue(row, 4) == null ? "" : sheet.GetCellValue(row, 4).ToString().Trim());
            ListValue.Add(sheet.GetCellValue(row, 5) == null ? "" : sheet.GetCellValue(row, 5).ToString().Trim());
            ListValue.Add(sheet.GetCellValue(row, 6) == null ? "" : sheet.GetCellValue(row, 6).ToString().Trim());
            ListValue.Add(sheet.GetCellValue(row, 7) == null ? "" : sheet.GetCellValue(row, 7).ToString().Trim());
            ListValue.Add(sheet.GetCellValue(row, 8) == null ? "" : sheet.GetCellValue(row, 8).ToString().Trim());
            ListValue.Add(sheet.GetCellValue(row, 9) == null ? "" : sheet.GetCellValue(row, 9).ToString().Trim());
            return ListValue.Where(x => !string.IsNullOrEmpty(x)).Count() == 0;
        }
        #endregion

    }
}