using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;
using Business.CommonBusiness;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using PagedList;
using Business.CommonModel.DMCHUCNANG;
using Business.CommonModel.HOSOCANBO;


namespace Business.Business
{
    public class HOSOCANBOBusiness : BaseBusiness<HOSO_CANBO_THONGTINCHUNG>
    {
        public HOSOCANBOBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public HOSOCANBOCUSTOMBO GetPersonalData(long id)
        {
            var query = from hoso in this.context.HOSO_CANBO_THONGTINCHUNG
                        where hoso.ID == id
                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.GIOITINH equals datadanhmuc.ID
                            into group1
                        from gioitinh in group1.DefaultIfEmpty()

                        join ndung in this.context.DM_NGUOIDUNG
                            on hoso.USER_ID equals ndung.ID
                            into group2
                        from nguoidung in group2.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.NGACHCONGCHUCVIENCHUC equals datadanhmuc.ID
                            into group3
                        from ngachcc in group3.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.CHUCVUHIENTAI equals datadanhmuc.ID
                            into group4
                        from chucvu in group4.DefaultIfEmpty()

                        join cocau in this.context.CCTC_THANHPHAN
                            on hoso.DONVI_ID equals cocau.ID
                            into group5
                        from cocautochuc in group5.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.DANTOC equals datadanhmuc.ID
                            into group6
                        from dantoc in group6.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.TONGIAO equals datadanhmuc.ID
                            into group7
                        from tongiao in group7.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.TRINHDOGIAODUC equals datadanhmuc.ID
                            into group8
                        from giaoduc in group8.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.TRINHDOCHUYENMONCAONHAT equals datadanhmuc.ID
                            into group9
                        from chuyenmon in group9.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.LYLUANCHINHTRI equals datadanhmuc.ID
                            into group10
                        from lyluan in group10.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.QUANLYNHANUOC equals datadanhmuc.ID
                            into group11
                        from quanly in group11.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.NGOAINGU equals datadanhmuc.ID
                            into group12
                        from ngoaingu in group12.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.TINHOC equals datadanhmuc.ID
                            into group13
                        from tinhoc in group13.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.QUANHAMCAONHAT equals datadanhmuc.ID
                            into group14
                        from quanham in group14.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.SUCKHOE equals datadanhmuc.ID
                            into group15
                        from suckhoe in group15.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.NHOMMAU equals datadanhmuc.ID
                            into group16
                        from nhommau in group16.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.GIADINHCHINHSACH equals datadanhmuc.ID
                            into group17
                        from chinhsach in group17.DefaultIfEmpty()
                        select new HOSOCANBOCUSTOMBO
                        {
                            ID = hoso.ID,
                            HOTEN = hoso.HOTEN,
                            TENGOIKHAC = hoso.TENGOIKHAC,
                            NGAYSINH = hoso.NGAYSINH,
                            GIOITINH = hoso.GIOITINH,
                            NOISINH_XA = hoso.NOISINH_XA,
                            NOISINH_HUYEN = hoso.NOISINH_HUYEN,
                            NOISINH_TINH = hoso.NOISINH_TINH,
                            QUEQUAN_XA = hoso.QUEQUAN_XA,
                            QUEQUAN_HUYEN = hoso.QUEQUAN_HUYEN,
                            QUEQUAN_TINH = hoso.QUEQUAN_TINH,
                            DANTOC = hoso.DANTOC,
                            TONGIAO = hoso.TONGIAO,
                            NOIDANGKYHOKHAUTHUONGTRU = hoso.NOIDANGKYHOKHAUTHUONGTRU,
                            NOIOHIENNAY = hoso.NOIOHIENNAY,
                            NGHENGHIEPKHIDUOCTUYENDUNG = hoso.NGHENGHIEPKHIDUOCTUYENDUNG,
                            NGAYTRUNGTUYEN = hoso.NGAYTRUNGTUYEN,
                            COQUANTUYENDUNG = hoso.COQUANTUYENDUNG,
                            CHUCVUHIENTAI = hoso.CHUCVUHIENTAI,
                            CONGVIECCHINHDUOCGIAO = hoso.CONGVIECCHINHDUOCGIAO,
                            NGACHCONGCHUCVIENCHUC = hoso.NGACHCONGCHUCVIENCHUC,
                            MANGACH = hoso.MANGACH,
                            BACLUONG = hoso.BACLUONG,
                            HESO = hoso.HESO,
                            NGAYHUONG = hoso.NGAYHUONG,
                            PHUCAPCHUCVU = hoso.PHUCAPCHUCVU,
                            PHUCAPKHAC = hoso.PHUCAPKHAC,
                            TRINHDOGIAODUC = hoso.TRINHDOGIAODUC,
                            TRINHDOCHUYENMONCAONHAT = hoso.TRINHDOCHUYENMONCAONHAT,
                            LYLUANCHINHTRI = hoso.LYLUANCHINHTRI,
                            QUANLYNHANUOC = hoso.QUANLYNHANUOC,
                            NGOAINGU = hoso.NGOAINGU,
                            TINHOC = hoso.TINHOC,
                            NGAYVAODANG = hoso.NGAYVAODANG,
                            NGAYCHINHTHUC = hoso.NGAYCHINHTHUC,
                            NGAYTHAMGIATCCTXH = hoso.NGAYTHAMGIATCCTXH,
                            NGAYNHAPNGU = hoso.NGAYNHAPNGU,
                            NGAYXUATNGU = hoso.NGAYXUATNGU,
                            QUANHAMCAONHAT = hoso.QUANHAMCAONHAT,
                            DANHHIEUDUOCPHONG = hoso.DANHHIEUDUOCPHONG,
                            SOTRUONGCONGTAC = hoso.SOTRUONGCONGTAC,
                            KHENTHUONG = hoso.KHENTHUONG,
                            KYLUAT = hoso.KYLUAT,
                            CHIEUCAO = hoso.CHIEUCAO,
                            CANNANG = hoso.CANNANG,
                            THUONGBINHHANG = hoso.THUONGBINHHANG,
                            CMTND = hoso.CMTND,
                            NGAYCAP = hoso.NGAYCAP,
                            SOSOBHXH = hoso.SOSOBHXH,
                            DACDIEMLICHSU_BIBAT = hoso.DACDIEMLICHSU_BIBAT,
                            DACDIEMLICHSU_THAMGIA = hoso.DACDIEMLICHSU_THAMGIA,
                            DACDIEMLICHSU_THANNHAN = hoso.DACDIEMLICHSU_THANNHAN,
                            NHANXET = hoso.NHANXET,

                            STRGIOITINH = gioitinh.TEXT,
                            TENDANGNHAP = nguoidung.TENDANGNHAP,
                            STRNGACH = ngachcc.TEXT,
                            STRCHUCVU = chucvu.TEXT,
                            STRDONVI = cocautochuc.NAME,
                            STRDANTOC = dantoc.TEXT,
                            STRTONGIAO = tongiao.TEXT,
                            STRTRINHDOGIAODUC = giaoduc.TEXT,
                            STRTRINHDOCHUYENMONCAONHAT = chuyenmon.TEXT,
                            STRLYLUANCHINHTRI = lyluan.TEXT,
                            STRQUANLYNHANUOC = quanly.TEXT,
                            STRNGOAINGU = ngoaingu.TEXT,
                            STRTINHOC = tinhoc.TEXT,
                            STRQUANHAMCAONHAT = quanham.TEXT,
                            STRSUCKHOE = suckhoe.TEXT,
                            STRNHOMMAU = nhommau.TEXT,
                            STRGIADINHCHINHSACH = chinhsach.TEXT,

                            HOSO_CANBO_CONGTAC =  hoso.HOSO_CANBO_CONGTAC,
                            HOSO_CANBO_DAOTAO =  hoso.HOSO_CANBO_DAOTAO,
                            HOSO_CANBO_QUANHEBANTHAN =  hoso.HOSO_CANBO_QUANHEBANTHAN,
                            HOSO_CANBO_QUANHEKETHON =  hoso.HOSO_CANBO_QUANHEKETHON,
                            HOSO_CANBO_QUATRINH_LUONG=hoso.HOSO_CANBO_QUATRINH_LUONG
                        };
            return query.FirstOrDefault();
        }
        public PageListResultBO<HOSOCANBOCUSTOMBO> GetDataByPage(HoSoCanBoSearch searchModel, int pageIndex = 1,int pageSize = 20 )
        {
            var query = from hoso in this.context.HOSO_CANBO_THONGTINCHUNG
                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.GIOITINH equals datadanhmuc.ID
                            into group1
                        from gioitinh in group1.DefaultIfEmpty()

                        join ndung in this.context.DM_NGUOIDUNG
                            on hoso.USER_ID equals ndung.ID
                            into group2
                        from nguoidung in group2.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.NGACHCONGCHUCVIENCHUC equals datadanhmuc.ID
                            into group3
                        from ngachcc in group3.DefaultIfEmpty()

                        join datadanhmuc in this.context.DM_DANHMUC_DATA
                            on hoso.CHUCVUHIENTAI equals datadanhmuc.ID
                            into group4
                        from chucvu in group4.DefaultIfEmpty()

                        join cocau in this.context.CCTC_THANHPHAN
                            on hoso.DONVI_ID equals cocau.ID
                            into group5
                        from cocautochuc in group5.DefaultIfEmpty()
                        select new HOSOCANBOCUSTOMBO
                        {
                            ID = hoso.ID,
                            HOTEN = hoso.HOTEN,
                            TENGOIKHAC = hoso.TENGOIKHAC,
                            NGAYSINH = hoso.NGAYSINH,
                            GIOITINH = hoso.GIOITINH,
                            NOISINH_XA = hoso.NOISINH_XA,
                            NOISINH_HUYEN = hoso.NOISINH_HUYEN,
                            NOISINH_TINH = hoso.NOISINH_TINH,
                            QUEQUAN_XA = hoso.QUEQUAN_XA,
                            QUEQUAN_HUYEN = hoso.QUEQUAN_HUYEN,
                            QUEQUAN_TINH = hoso.QUEQUAN_TINH,
                            DANTOC = hoso.DANTOC,
                            TONGIAO = hoso.TONGIAO,
                            NOIDANGKYHOKHAUTHUONGTRU = hoso.NOIDANGKYHOKHAUTHUONGTRU,
                            NOIOHIENNAY = hoso.NOIOHIENNAY,
                            NGHENGHIEPKHIDUOCTUYENDUNG = hoso.NGHENGHIEPKHIDUOCTUYENDUNG,
                            NGAYTRUNGTUYEN = hoso.NGAYTRUNGTUYEN,
                            COQUANTUYENDUNG = hoso.COQUANTUYENDUNG,
                            CHUCVUHIENTAI = hoso.CHUCVUHIENTAI,
                            NGACHCONGCHUCVIENCHUC = hoso.NGACHCONGCHUCVIENCHUC,
                            MANGACH = hoso.MANGACH,
                            BACLUONG = hoso.BACLUONG,
                            HESO = hoso.HESO,
                            TRINHDOGIAODUC = hoso.TRINHDOGIAODUC,
                            TRINHDOCHUYENMONCAONHAT = hoso.TRINHDOCHUYENMONCAONHAT,
                            STRGIOITINH = gioitinh.TEXT,
                            TENDANGNHAP = nguoidung.TENDANGNHAP,
                            STRNGACH = ngachcc.TEXT,
                            STRCHUCVU = chucvu.TEXT,
                            STRDONVI = cocautochuc.NAME
                        };
            if (searchModel!=null)
            {
                if (!string.IsNullOrEmpty(searchModel.HOTEN))
                {
                    query = query.Where(x => x.HOTEN.Contains(searchModel.HOTEN));
                }
                if (!string.IsNullOrEmpty(searchModel.MANGACH_BAC))
                {
                    query = query.Where(x => x.MANGACH.Contains(searchModel.MANGACH_BAC));
                }
                if (!string.IsNullOrEmpty(searchModel.STRCHUCVU))
                {
                    query = query.Where(x => x.STRCHUCVU.Contains(searchModel.STRCHUCVU));
                }
                if (!string.IsNullOrEmpty(searchModel.TENDANGNHAP))
                {
                    query = query.Where(x => x.TENDANGNHAP.Contains(searchModel.TENDANGNHAP));
                }
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x=>x.ID);
                }
            }
            else
            {
                query = query.OrderByDescending(x=>x.ID);
            }
          
            var resultmodel = new PageListResultBO<HOSOCANBOCUSTOMBO>();
            if (pageSize==-1)
            {
                var dataPageList = query.ToList();
                resultmodel.Count = dataPageList.Count;
                resultmodel.TotalPage = 1;
                resultmodel.ListItem = dataPageList;
            }
            else
            {
                var dataPageList = query.ToPagedList(pageIndex, pageSize);
                resultmodel.Count = dataPageList.TotalItemCount;
                resultmodel.TotalPage = dataPageList.PageCount;
                resultmodel.ListItem = dataPageList.ToList();
            }
            return resultmodel;
        }
    }
}

