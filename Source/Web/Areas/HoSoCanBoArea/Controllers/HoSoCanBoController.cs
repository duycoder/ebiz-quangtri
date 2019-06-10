using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Business;
using Web.Areas.HoSoCanBoArea.Models;
using Web.Custom;
using Model.Entities;
using Web.FwCore;
using Business.CommonModel.HOSOCANBO;

namespace Web.Areas.HoSoCanBoArea.Controllers
{
    public class HoSoCanBoController : BaseController
    {
        // GET: HoSoCanBoArea/HoSoCanBo
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private HOSOCANBOBusiness HOSOCANBOBusiness;
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        private HOSO_CANBO_DAOTAOBusiness HOSO_CANBO_DAOTAOBusiness;
        private HOSOCANBOCONGTACBusiness HOSO_CANBO_CONGTACBusiness;
        private HOSO_CANBO_QUANHE_BANTHANBusiness HOSO_CANBO_QUANHE_BANTHANBusiness;
        private HOSO_CANBO_QUANHE_KETHONBusiness HOSO_CANBO_QUANHE_KETHONBusiness;
        private QUATRINH_LUONG_CANBOBusiness QUATRINH_LUONG_CANBOBusiness;
        public ActionResult Index()
        {
            HOSOCANBOBusiness = Get<HOSOCANBOBusiness>();
            var searchmodel = new HoSoCanBoSearch();
            SessionManager.SetValue("hosocanboSearchModel", null);
            DanhSachHoSoVM model = new DanhSachHoSoVM();
            model.ListResult = HOSOCANBOBusiness.GetDataByPage(null);
            return View(model);
        }

        public ActionResult Create()
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            HoSoCanBoVM model = new HoSoCanBoVM();
            model.HoSo = new HOSO_CANBO_THONGTINCHUNG();
            model.LstGioiTinh = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_GIOITINH", 0);
            model.LstDanToc = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_DANTOC", 0);
            model.LstTonGiao = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_TONGIAO", 0);
            model.LstNgach = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_NGACH", 0);
            model.LstTrinhDoGiaoDuc = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_TRINHDOGIAODUC", 0);
            model.LstTrinhDoChuyenMon = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_TRINHDOCHUYENMON", 0);
            model.LstLyLuanChinhTri = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_LYLUANCHINHTRI", 0);
            model.LstQuanLyNhaNuoc = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_QUANLYNHANUOC", 0);
            model.LstNgoaiNgu = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_NGOAINGU", 0);
            model.LstTinHoc = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_TINHOC", 0);
            model.LstTinhTrangSucKhoe = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_TINHTRANGSUCKHOE", 0);
            model.LstNhomMau = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_NHOMMAU", 0);
            model.LstGiaDinhChinhSach = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_GIADINHCHINHSACH", 0);
            model.LstChucVu = DM_DANHMUC_DATABusiness.DsByMaNhom("DMCHUCVU", 0);
            model.LstDonViHienTai = CCTC_THANHPHANBusiness.GetDropDownList();
            return View(model);
        }

        public ActionResult EditHoSo(long id)
        {
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            HOSOCANBOBusiness = Get<HOSOCANBOBusiness>();
            HoSoCanBoVM model = new HoSoCanBoVM();
            model.HoSo = HOSOCANBOBusiness.Find(id);
            model.LstGioiTinh = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_GIOITINH", 0, model.HoSo.GIOITINH.HasValue ? model.HoSo.GIOITINH.Value : 0);
            model.LstDanToc = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_DANTOC", 0, model.HoSo.DANTOC.HasValue ? model.HoSo.DANTOC.Value : 0);
            model.LstTonGiao = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_TONGIAO", 0, model.HoSo.TONGIAO.HasValue ? model.HoSo.TONGIAO.Value : 0);
            model.LstNgach = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_NGACH", 0, model.HoSo.NGACHCONGCHUCVIENCHUC.HasValue ? model.HoSo.NGACHCONGCHUCVIENCHUC.Value : 0);
            model.LstTrinhDoGiaoDuc = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_TRINHDOGIAODUC", 0, model.HoSo.TRINHDOGIAODUC.HasValue ? model.HoSo.TRINHDOGIAODUC.Value : 0);
            model.LstTrinhDoChuyenMon = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_TRINHDOCHUYENMON", 0, model.HoSo.TRINHDOCHUYENMONCAONHAT.HasValue ? model.HoSo.TRINHDOCHUYENMONCAONHAT.Value : 0);
            model.LstLyLuanChinhTri = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_LYLUANCHINHTRI", 0, model.HoSo.LYLUANCHINHTRI.HasValue ? model.HoSo.LYLUANCHINHTRI.Value : 0);
            model.LstQuanLyNhaNuoc = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_QUANLYNHANUOC", 0, model.HoSo.QUANLYNHANUOC.HasValue ? model.HoSo.QUANLYNHANUOC.Value : 0);
            model.LstNgoaiNgu = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_NGOAINGU", 0, model.HoSo.NGOAINGU.HasValue ? model.HoSo.NGOAINGU.Value : 0);
            model.LstTinHoc = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_TINHOC", 0, model.HoSo.TINHOC.HasValue ? model.HoSo.TINHOC.Value : 0);
            model.LstTinhTrangSucKhoe = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_TINHTRANGSUCKHOE", 0, model.HoSo.SUCKHOE.HasValue ? model.HoSo.SUCKHOE.Value : 0);
            model.LstNhomMau = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_NHOMMAU", 0, model.HoSo.NHOMMAU.HasValue ? model.HoSo.NHOMMAU.Value : 0);
            model.LstGiaDinhChinhSach = DM_DANHMUC_DATABusiness.DsByMaNhom("NHANSU_GIADINHCHINHSACH", 0, model.HoSo.GIADINHCHINHSACH.HasValue ? model.HoSo.GIADINHCHINHSACH.Value : 0);
            model.LstChucVu = DM_DANHMUC_DATABusiness.DsByMaNhom("DMCHUCVU", 0, model.HoSo.CHUCVUHIENTAI.HasValue ? model.HoSo.CHUCVUHIENTAI.Value : 0);
            model.LstDonViHienTai = CCTC_THANHPHANBusiness.GetDropDownList(model.HoSo.DONVI_ID.HasValue ? model.HoSo.DONVI_ID.Value : 0);
            return View("Create", model);
        }

        public ActionResult ChiTietHoSo(long id)
        {
            HOSOCANBOBusiness = Get<HOSOCANBOBusiness>();
            HoSoCanBoChiTietVM model = new HoSoCanBoChiTietVM();
            var result = HOSOCANBOBusiness.GetPersonalData(id);
            if (result != null)
            {
                model.ThongTinChung = result;
                return View(model);
            }

            return RedirectToAction("Index");
        }
        public ActionResult SaveHoSoCanBo(HOSO_CANBO_THONGTINCHUNG submitObj)
        {
            HOSOCANBOBusiness = Get<HOSOCANBOBusiness>();
            HOSOCANBOBusiness.Save(submitObj);
            return Redirect("/HoSoCanBoArea/HoSoCanBo/ChiTietHoSo/" + submitObj.ID);
        }

        public ActionResult SaveThongTinDaoTao(long HOSOID, List<string> TENTRUONG, List<string> CHUYENNGANHDAOTAO, List<string> THOIGIANDAOTAO, List<string> HINHTHUCDAOTAO, List<string> VANBANGCHUNGCHI, long DAOTAO_ID = 0)
        {
            HOSOCANBOBusiness = Get<HOSOCANBOBusiness>();
            HOSO_CANBO_DAOTAOBusiness = Get<HOSO_CANBO_DAOTAOBusiness>();
            var HoSoObj = HOSOCANBOBusiness.Find(HOSOID);
            if (HoSoObj != null)
            {
                var Total = TENTRUONG.Count;
                for (var i = 0; i < Total; i++)
                {
                    var obj = new HOSO_CANBO_DAOTAO();
                    if (DAOTAO_ID != 0)
                    {
                        obj = HOSO_CANBO_DAOTAOBusiness.Find(DAOTAO_ID);
                    }
                    
                    obj.HOSO_ID = HOSOID;
                    obj.TENTRUONG = TENTRUONG[i];
                    obj.CHUYENNGANHDAOTAO = CHUYENNGANHDAOTAO[i];
                    obj.THOIGIANDAOTAO = THOIGIANDAOTAO[i];
                    obj.HINHTHUCDAOTAO = HINHTHUCDAOTAO[i];
                    obj.VANBANGCHUNGCHI = VANBANGCHUNGCHI[i];
                    HOSO_CANBO_DAOTAOBusiness.Save(obj);
                }

                return Redirect("/HoSoCanBoArea/HoSoCanBo/ChiTietHoSo/"+ HoSoObj.ID);
            }
            else
            {
                return RedirectToAction("index");
            }
        }


        public JsonResult XoaThongTinDaoTao(long id)
        {
            HOSO_CANBO_DAOTAOBusiness = Get<HOSO_CANBO_DAOTAOBusiness>();
            var DaoTaoObj = HOSO_CANBO_DAOTAOBusiness.Find(id);
            if (DaoTaoObj != null)
            {
                HOSO_CANBO_DAOTAOBusiness.repository.Delete(id);
                HOSO_CANBO_DAOTAOBusiness.Save();
                return Json(new { Type = "SUCCESS", Message = "Xóa thông tin thành công" });
            }
            else
            {
                return Json(new { Type = "ERROR", Message = "Xóa thông tin không thành công" });
            }
        }
        // quản lý hồ sơ cán bộ công tác
        public ActionResult SaveThongTinCanBoCongTac(long HOSOID, List<string> THOIGIANCONGTAC, List<string> CHUCDANHDONVI, long DAOTAO_ID = 0)
        {
            HOSOCANBOBusiness = Get<HOSOCANBOBusiness>();
            HOSO_CANBO_CONGTACBusiness = Get<HOSOCANBOCONGTACBusiness>();
            var HoSoObj = HOSOCANBOBusiness.Find(HOSOID);
            if (HoSoObj != null)
            {
                var Total = THOIGIANCONGTAC.Count;
                for (var i = 0; i < Total; i++)
                {
                    var obj = new HOSO_CANBO_CONGTAC();
                    if (DAOTAO_ID != 0)
                    {
                        obj = HOSO_CANBO_CONGTACBusiness.Find(DAOTAO_ID);
                    }

                    obj.HOSO_ID = HOSOID;
                    obj.THOIGIANCONGTAC = THOIGIANCONGTAC[i];
                    obj.CHUCDANHDONVI = CHUCDANHDONVI[i];
                    HOSO_CANBO_CONGTACBusiness.Save(obj);
                }

                return Redirect("/HoSoCanBoArea/HoSoCanBo/ChiTietHoSo/" + HoSoObj.ID);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        public JsonResult XoaCanBoCongTac(long id)
        {
            HOSO_CANBO_CONGTACBusiness = Get<HOSOCANBOCONGTACBusiness>();
            var DaoTaoObj = HOSO_CANBO_CONGTACBusiness.Find(id);
            if (DaoTaoObj != null)
            {
                HOSO_CANBO_CONGTACBusiness.repository.Delete(id);
                HOSO_CANBO_CONGTACBusiness.Save();
                return Json(new { Type = "SUCCESS", Message = "Xóa thông tin thành công" });
            }
            else
            {
                return Json(new { Type = "ERROR", Message = "Xóa thông tin không thành công" });
            }
        }
        // quản lý cán bộ quan hệ bản thân 
        public ActionResult SaveThongTinQuanHeBanThan(long HOSOID, List<string> MOIQUANHE, List<string> HOTEN, List<int> NAMSINH,List<string> QUEQUAN, long DAOTAO_ID = 0)
        {
            HOSOCANBOBusiness = Get<HOSOCANBOBusiness>();
            HOSO_CANBO_QUANHE_BANTHANBusiness = Get<HOSO_CANBO_QUANHE_BANTHANBusiness>();
            var HoSoObj = HOSOCANBOBusiness.Find(HOSOID);
            if (HoSoObj != null)
            {
                var Total = MOIQUANHE.Count;
                for (var i = 0; i < Total; i++)
                {
                    var obj = new HOSO_CANBO_QUANHEBANTHAN();
                    if (DAOTAO_ID != 0)
                    {
                        obj = HOSO_CANBO_QUANHE_BANTHANBusiness.Find(DAOTAO_ID);
                    }

                    obj.HOSO_ID = HOSOID;
                    obj.MOIQUANHE = MOIQUANHE[i];
                    obj.HOTEN = HOTEN[i];
                    obj.NAMSINH = NAMSINH[i];
                    obj.QUEQUAN = QUEQUAN[i];
                    HOSO_CANBO_QUANHE_BANTHANBusiness.Save(obj);
                }

                return Redirect("/HoSoCanBoArea/HoSoCanBo/ChiTietHoSo/" + HoSoObj.ID);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        public JsonResult XoaQuanHeBanThan(long id)
        {
            HOSO_CANBO_QUANHE_BANTHANBusiness = Get<HOSO_CANBO_QUANHE_BANTHANBusiness>();
            var DaoTaoObj = HOSO_CANBO_QUANHE_BANTHANBusiness.Find(id);
            if (DaoTaoObj != null)
            {
                HOSO_CANBO_QUANHE_BANTHANBusiness.repository.Delete(id);
                HOSO_CANBO_QUANHE_BANTHANBusiness.Save();
                return Json(new { Type = "SUCCESS", Message = "Xóa thông tin thành công" });
            }
            else
            {
                return Json(new { Type = "ERROR", Message = "Xóa thông tin không thành công" });
            }
        }
        // quản lý cán bộ quan hệ kết hôn
        public ActionResult SaveThongTinQuanHeKetHon(long HOSOID, List<string> MOIQUANHE, List<string> HOTEN, List<int> NAMSINH, List<string> QUEQUAN, long DAOTAO_ID = 0)
        {
            HOSOCANBOBusiness = Get<HOSOCANBOBusiness>();
            HOSO_CANBO_QUANHE_KETHONBusiness = Get<HOSO_CANBO_QUANHE_KETHONBusiness>();
            var HoSoObj = HOSOCANBOBusiness.Find(HOSOID);
            if (HoSoObj != null)
            {
                var Total = MOIQUANHE.Count;
                for (var i = 0; i < Total; i++)
                {
                    var obj = new HOSO_CANBO_QUANHEKETHON();
                    if (DAOTAO_ID != 0)
                    {
                        obj = HOSO_CANBO_QUANHE_KETHONBusiness.Find(DAOTAO_ID);
                    }

                    obj.HOSO_ID = HOSOID;
                    obj.MOIQUANHE = MOIQUANHE[i];
                    obj.HOTEN = HOTEN[i];
                    obj.NAMSINH = NAMSINH[i];
                    obj.QUEQUAN = QUEQUAN[i];
                    HOSO_CANBO_QUANHE_KETHONBusiness.Save(obj);
                }

                return Redirect("/HoSoCanBoArea/HoSoCanBo/ChiTietHoSo/" + HoSoObj.ID);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        public JsonResult XoaQuanHeKetHon(long id)
        {
            HOSO_CANBO_QUANHE_KETHONBusiness = Get<HOSO_CANBO_QUANHE_KETHONBusiness>();
            var DaoTaoObj = HOSO_CANBO_QUANHE_KETHONBusiness.Find(id);
            if (DaoTaoObj != null)
            {
                HOSO_CANBO_QUANHE_KETHONBusiness.repository.Delete(id);
                HOSO_CANBO_QUANHE_KETHONBusiness.Save();
                return Json(new { Type = "SUCCESS", Message = "Xóa thông tin thành công" });
            }
            else
            {
                return Json(new { Type = "ERROR", Message = "Xóa thông tin không thành công" });
            }
        }
        // quá trình luong cán bộ
        public ActionResult SaveQuaTrinhLuongCanBo(long HOSOID, List<string> THANG_NAM, List<string> MANGACH_BAC, List<string> HESOLUONG, long DAOTAO_ID = 0)
        {
            HOSOCANBOBusiness = Get<HOSOCANBOBusiness>();
            QUATRINH_LUONG_CANBOBusiness = Get<QUATRINH_LUONG_CANBOBusiness>();
            var HoSoObj = HOSOCANBOBusiness.Find(HOSOID);
            if (HoSoObj != null)
            {
                var Total = THANG_NAM.Count;
                for (var i = 0; i < Total; i++)
                {
                    var obj = new HOSO_CANBO_QUATRINH_LUONG();
                    if (DAOTAO_ID != 0)
                    {
                        obj = QUATRINH_LUONG_CANBOBusiness.Find(DAOTAO_ID);
                    }
                    obj.HOSO_ID = HOSOID;
                    obj.THANG_NAM = THANG_NAM[i];
                    obj.MANGACH_BAC = MANGACH_BAC[i];
                    obj.HESOLUONG = HESOLUONG[i];
                    QUATRINH_LUONG_CANBOBusiness.Save(obj);
                }
                return Redirect("/HoSoCanBoArea/HoSoCanBo/ChiTietHoSo/" + HoSoObj.ID);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        public JsonResult XoaQuaTrinhLuongCanBo(long id)
        {
            QUATRINH_LUONG_CANBOBusiness = Get<QUATRINH_LUONG_CANBOBusiness>();
            var DaoTaoObj = QUATRINH_LUONG_CANBOBusiness.Find(id);
            if (DaoTaoObj != null)
            {
                QUATRINH_LUONG_CANBOBusiness.repository.Delete(id);
                QUATRINH_LUONG_CANBOBusiness.Save();
                return Json(new { Type = "SUCCESS", Message = "Xóa thông tin thành công" });
            }
            else
            {
                return Json(new { Type = "ERROR", Message = "Xóa thông tin không thành công" });
            }
        }
        //search
        [HttpPost]
        public JsonResult searchData(FormCollection form)
        {
            HOSOCANBOBusiness = Get<HOSOCANBOBusiness>();
            var searchModel = SessionManager.GetValue("hosocanboSearchModel") as HoSoCanBoSearch;
            if (searchModel==null)
            {
                searchModel = new HoSoCanBoSearch();
                searchModel.pageSize = 20;
            }
            searchModel.HOTEN = form["HOTEN"];
            searchModel.MANGACH_BAC = form["MANGACH_BAC"];
            searchModel.STRCHUCVU = form["STRCHUCVU"];
            searchModel.TENDANGNHAP = form["TENDANGNHAP"];
            SessionManager.SetValue("hosocanboSearchModel", searchModel);
            var data = HOSOCANBOBusiness.GetDataByPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }



    }
}