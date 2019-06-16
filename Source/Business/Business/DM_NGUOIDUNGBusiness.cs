using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;
using Business.CommonModel.DMNguoiDung;
using Business.CommonBusiness;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using PagedList;
using Business.CommonModel.DMCHUCNANG;
using System.Collections;
using System.Web.Mvc;
using CommonHelper;

namespace Business.Business
{
    public class DM_NGUOIDUNGBusiness : BaseBusiness<DM_NGUOIDUNG>
    {
        public DM_NGUOIDUNGBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {

        }
        public List<DM_NGUOIDUNG_BO> GetByPhongBan(int id)
        {
            var result = (from nguoiDung in this.context.DM_NGUOIDUNG
                          where nguoiDung.DM_PHONGBAN_ID == id && nguoiDung.IS_ACTIVE == true
                          select new DM_NGUOIDUNG_BO
                          {
                              DIENTHOAI = nguoiDung.DIENTHOAI,
                              ID = nguoiDung.ID,
                              EMAIL = nguoiDung.EMAIL,
                              HOTEN = nguoiDung.HOTEN,
                              TENDANGNHAP = nguoiDung.TENDANGNHAP,
                              LstVaiTro = (from vt in this.context.DM_VAITRO
                                           join vtnd in this.context.NGUOIDUNG_VAITRO on vt.DM_VAITRO_ID equals vtnd.VAITRO_ID
                                           where vtnd.NGUOIDUNG_ID == nguoiDung.ID
                                           select vt.TEN_VAITRO
                                         ).ToList(),
                          });
            return result.OrderBy(x => x.ID).ToList();
        }
        public List<int> GetRecursiveDept(int depid)
        {
            var LstFinal = new List<int>();
            var result = from tbl in this.context.CCTC_THANHPHAN
                         where tbl.PARENT_ID == depid
                         select tbl.ID;
            var LstDept = result.ToList();
            if (LstDept.Count > 0)
            {
                LstFinal.AddRange(LstDept);
                foreach (var item in LstDept)
                {
                    LstFinal.AddRange(GetRecursiveDept(item));
                }
            }
            return LstFinal;
        }
        /// <summary>
        /// Lấy dữ liệu theo đối tượng search và phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageListResultBO<DM_NGUOIDUNG_BO> GetDaTaByPage(DM_NGUOIDUNG_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            //Lấy queryable của lớp
            var query = from tbl in this.context.DM_NGUOIDUNG
                        join tblChucVu in this.context.DM_DANHMUC_DATA on tbl.CHUCVU_ID equals tblChucVu.ID into jchucvu
                        from chucvu in jchucvu.DefaultIfEmpty()
                        join cc in this.context.CCTC_THANHPHAN on tbl.DM_PHONGBAN_ID equals cc.ID
                        into gcc
                        from groupcc in gcc.DefaultIfEmpty()
                        where tbl.IS_ACTIVE == true
                        select new DM_NGUOIDUNG_BO
                        {
                            ID = tbl.ID,
                            ANH_DAIDIEN = tbl.ANH_DAIDIEN,
                            DIACHI = tbl.DIACHI,
                            DIENTHOAI = tbl.DIENTHOAI,
                            DM_PHONGBAN_ID = tbl.DM_PHONGBAN_ID,
                            EMAIL = tbl.EMAIL,
                            HOTEN = tbl.HOTEN,
                            MAHOA_MK = tbl.MAHOA_MK,
                            MATKHAU = tbl.MATKHAU,
                            NGAYSINH = tbl.NGAYSINH,
                            NGAYSUA = tbl.NGAYSUA,
                            NGAYTAO = tbl.NGAYTAO,
                            NGUOISUA = tbl.NGUOISUA,
                            NGUOITAO = tbl.NGUOITAO,
                            TENDANGNHAP = tbl.TENDANGNHAP,
                            TRANGTHAI = tbl.TRANGTHAI,
                            ChucVu = chucvu != null ? chucvu.TEXT : "",
                            TenPhongBan = groupcc.NAME,
                            ChucVu_Id = chucvu != null ? chucvu.ID : 0,
                            PhongBan_Id = groupcc != null ? groupcc.ID : 0,
                        };

            //searchmodel là đối tượng search của bảng được lưu trong session
            if (searchModel != null)
            {
                if (searchModel.deptId.HasValue)
                {
                    //query ra danh sách tất cả các đơn vị con của đơn vị này
                    var LstFinal = new List<int>();
                    var result = from tbl in this.context.CCTC_THANHPHAN
                                 where tbl.PARENT_ID == searchModel.deptId || tbl.ID == searchModel.deptId
                                 select tbl.ID;
                    var LstDept = result.ToList();
                    LstFinal.AddRange(LstDept);
                    foreach (var item in LstDept)
                    {
                        LstFinal.AddRange(GetRecursiveDept(item));
                    }
                    query = query.Where(x => LstFinal.Contains(x.DM_PHONGBAN_ID.Value));
                }
                //code
                if (searchModel.sea_CHUCVU_ID.HasValue)
                {
                    query = query.Where(x => x.ChucVu_Id == searchModel.sea_CHUCVU_ID);
                }
                if (searchModel.sea_PHONGBAN_ID.HasValue)
                {
                    query = query.Where(x => x.PhongBan_Id == searchModel.sea_PHONGBAN_ID);
                }
                if (!string.IsNullOrEmpty(searchModel.sea_HoTen))
                {
                    query = query.Where(x => x.HOTEN.Contains(searchModel.sea_HoTen));
                }
                if (!string.IsNullOrEmpty(searchModel.sea_Email))
                {
                    query = query.Where(x => x.EMAIL.Contains(searchModel.sea_Email));
                }
                if (!string.IsNullOrEmpty(searchModel.sea_DienThoai))
                {
                    query = query.Where(x => x.DIENTHOAI.Contains(searchModel.sea_DienThoai));
                }
                if (!string.IsNullOrEmpty(searchModel.sea_TenDangNhap))
                {
                    query = query.Where(x => x.TENDANGNHAP.Contains(searchModel.sea_TenDangNhap));
                }
                //lọc tìm kiếm tại đây
                //sử dụng để sort datatable
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.ID);
                }
            }
            else
            {
                //Phần thiết lập sort mặc định (Nếu lớp k có trường ID thì thay bằng trường thông tin khác)
                query = query.OrderByDescending(x => x.ID);
            }
            // phần gán nội dung trả về
            var resultmodel = new PageListResultBO<DM_NGUOIDUNG_BO>();
            //nếu pageSize =-1 tức là sẽ lấy ra hết tất cả
            if (pageSize == -1)
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

            string temp = "";
            List<long> Ids = resultmodel.ListItem.Select(x => x.ID).ToList();
            var NguoiDungVaiTro = (from role in this.context.NGUOIDUNG_VAITRO.AsNoTracking()
                                   where role.NGUOIDUNG_ID.HasValue && role.VAITRO_ID.HasValue
                                   && Ids.Contains(role.NGUOIDUNG_ID.Value)
                                   select role).ToList();
            List<DM_VAITRO> ListVaiTro = this.context.DM_VAITRO.AsNoTracking().ToList();
            foreach (var item in resultmodel.ListItem)
            {
                temp = "";
                var ListTemp = NguoiDungVaiTro.Where(x => item.ID == x.NGUOIDUNG_ID).ToList();
                if (ListTemp.Any())
                {
                    var ListVaiTroTemp = ListVaiTro.Where(x => ListTemp.Select(y => y.VAITRO_ID).Contains(x.DM_VAITRO_ID)).ToList();
                    if (ListVaiTroTemp.Any())
                    {
                        int size = ListVaiTroTemp.Count;
                        for (int i = 0; i < size; i++)
                        {
                            if (i == (size - 1))
                            {
                                temp += ListVaiTroTemp[i].TEN_VAITRO + " ";
                            }
                            else
                            {
                                temp += ListVaiTroTemp[i].TEN_VAITRO + ", ";
                            }
                        }
                        item.TEN_VAITRO = temp;
                    }
                }
            }
            return resultmodel;
        }
        /// <summary>
        /// Lấy danh sách user không thuộc phòng ban nào
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageListResultBO<DM_NGUOIDUNG_BO> GetUserTuDoByPage(DM_NGUOIDUNG_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            //Lấy queryable của lớp
            var query = from tbl in this.context.DM_NGUOIDUNG
                        where tbl.DM_PHONGBAN_ID == null && tbl.IS_ACTIVE == true
                        select new DM_NGUOIDUNG_BO
                        {
                            ID = tbl.ID,
                            ANH_DAIDIEN = tbl.ANH_DAIDIEN,
                            DIACHI = tbl.DIACHI,
                            DIENTHOAI = tbl.DIENTHOAI,
                            DM_PHONGBAN_ID = tbl.DM_PHONGBAN_ID,
                            EMAIL = tbl.EMAIL,
                            HOTEN = tbl.HOTEN,
                            MAHOA_MK = tbl.MAHOA_MK,
                            MATKHAU = tbl.MATKHAU,
                            NGAYSINH = tbl.NGAYSINH,
                            NGAYSUA = tbl.NGAYSUA,
                            NGAYTAO = tbl.NGAYTAO,
                            NGUOISUA = tbl.NGUOISUA,
                            NGUOITAO = tbl.NGUOITAO,
                            TENDANGNHAP = tbl.TENDANGNHAP,
                            TRANGTHAI = tbl.TRANGTHAI,
                        };

            //searchmodel là đối tượng search của bảng được lưu trong session
            if (searchModel != null)
            {
                //code
                if (!string.IsNullOrEmpty(searchModel.sea_HoTen))
                {
                    query = query.Where(x => x.HOTEN.Contains(searchModel.sea_HoTen));
                }
                if (!string.IsNullOrEmpty(searchModel.sea_Email))
                {
                    query = query.Where(x => x.EMAIL.Contains(searchModel.sea_Email));
                }
                if (!string.IsNullOrEmpty(searchModel.sea_DienThoai))
                {
                    query = query.Where(x => x.DIENTHOAI.Contains(searchModel.sea_DienThoai));
                }
                if (!string.IsNullOrEmpty(searchModel.sea_TenDangNhap))
                {
                    query = query.Where(x => x.TENDANGNHAP.Contains(searchModel.sea_TenDangNhap));
                }
                //lọc tìm kiếm tại đây
                //sử dụng để sort datatable
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.ID);
                }


            }
            else
            {
                //Phần thiết lập sort mặc định (Nếu lớp k có trường ID thì thay bằng trường thông tin khác)
                query = query.OrderByDescending(x => x.ID);
            }

            // phần gán nội dung trả về
            var resultmodel = new PageListResultBO<DM_NGUOIDUNG_BO>();
            //nếu pageSize =-1 tức là sẽ lấy ra hết tất cả
            if (pageSize == -1)
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

        #region Kiểm tra tên người dùng đã tồn tại chưa?>
        /// <summary>
        /// Kiểm tra tên tài khoản đã được sử dụng chưa
        /// </summary>
        /// <param name="taikhoan"></param>
        /// <returns>Status = true tức là tài khoản đã tồn tại</returns>
        public JsonResultBO CheckExsitTaiKhoan(string taikhoan)
        {
            var result = new JsonResultBO(true);
            var query = this.context.DM_NGUOIDUNG.Where(x => x.TENDANGNHAP.Equals(taikhoan)).Any();

            result.Status = query;
            return result;
        }
        #endregion

        #region Kiểm tra Email đã sử dụng chưa ?
        /// <summary>
        /// Kiểm tra tên tài khoản đã được sử dụng chưa
        /// </summary>
        /// <param name="taikhoan"></param>
        /// <returns>Status = true tức là tài khoản đã tồn tại</returns>
        public JsonResultBO CheckExsitEmail(string email, long iduser = 0)
        {
            var result = new JsonResultBO(true);

            if (iduser == 0)
            {
                var query = this.context.DM_NGUOIDUNG.Where(x => x.EMAIL.Equals(email)).Any();
                result.Status = query;
            }
            else
            {
                var query = this.context.DM_NGUOIDUNG.Where(x => x.EMAIL.Equals(email) && x.ID != iduser).Any();
                result.Status = query;
            }
            return result;
        }
        #endregion
        // lấy nội dung theo id
        public DM_NGUOIDUNG_BO GetDaTaByID(long ID)
        {
            var query = from tbl in this.context.DM_NGUOIDUNG
                        where tbl.ID == ID && tbl.IS_ACTIVE == true
                        join tblChucVu in this.context.DM_DANHMUC_DATA on tbl.CHUCVU_ID equals tblChucVu.ID into jchucvu
                        from chucvu in jchucvu.DefaultIfEmpty()
                        select new DM_NGUOIDUNG_BO
                        {
                            ID = tbl.ID,
                            ANH_DAIDIEN = tbl.ANH_DAIDIEN,
                            DIACHI = tbl.DIACHI,
                            DIENTHOAI = tbl.DIENTHOAI,
                            DM_PHONGBAN_ID = tbl.DM_PHONGBAN_ID,
                            EMAIL = tbl.EMAIL,
                            HOTEN = tbl.HOTEN,
                            MAHOA_MK = tbl.MAHOA_MK,
                            MATKHAU = tbl.MATKHAU,
                            NGAYSINH = tbl.NGAYSINH,
                            NGAYSUA = tbl.NGAYSUA,
                            NGAYTAO = tbl.NGAYTAO,
                            NGUOISUA = tbl.NGUOISUA,
                            NGUOITAO = tbl.NGUOITAO,
                            TENDANGNHAP = tbl.TENDANGNHAP,
                            TRANGTHAI = tbl.TRANGTHAI,
                            ChucVu = chucvu != null ? chucvu.TEXT : "Không có chức vụ"
                        };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }
        public UserInfoBO GetNewUserInfo(long ID)
        {
            var query = from tbl in this.context.DM_NGUOIDUNG
                        where tbl.ID == ID && tbl.IS_ACTIVE == true
                        join tblChucVu in this.context.DM_DANHMUC_DATA on tbl.CHUCVU_ID equals tblChucVu.ID into jchucvu
                        from chucvu in jchucvu.DefaultIfEmpty()
                        select new UserInfoBO
                        {
                            ID = tbl.ID,
                            ANH_DAIDIEN = tbl.ANH_DAIDIEN,
                            DIACHI = tbl.DIACHI,
                            DIENTHOAI = tbl.DIENTHOAI,
                            DM_PHONGBAN_ID = tbl.DM_PHONGBAN_ID,
                            EMAIL = tbl.EMAIL,
                            HOTEN = tbl.HOTEN,
                            MAHOA_MK = tbl.MAHOA_MK,
                            MATKHAU = tbl.MATKHAU,
                            NGAYSINH = tbl.NGAYSINH,
                            NGAYSUA = tbl.NGAYSUA,
                            NGAYTAO = tbl.NGAYTAO,
                            NGUOISUA = tbl.NGUOISUA,
                            NGUOITAO = tbl.NGUOITAO,
                            TENDANGNHAP = tbl.TENDANGNHAP,
                            TRANGTHAI = tbl.TRANGTHAI,
                            ChucVu = chucvu != null ? chucvu.TEXT : "Không có chức vụ",
                            CHUCVU_ID = chucvu != null ? chucvu.ID : 0,
                            signpath = tbl.signpath
                        };

            var resultmodel = query.FirstOrDefault();
            if (resultmodel != null)
            {
                resultmodel.ListChucNangMenu = GetListThaoTacByNguoiDung(resultmodel.ID);
                resultmodel.ListThaoTac = new List<DM_THAOTAC>();
                foreach (var item in resultmodel.ListChucNangMenu.Where(x => x.ListThaoTac != null && x.ListThaoTac.Any()).ToList())
                {
                    resultmodel.ListThaoTac.AddRange(item.ListThaoTac);
                }
                resultmodel.ListVaiTro = GetListVaiTro(resultmodel.ID);
                var DeptObj = this.context.CCTC_THANHPHAN.Find(resultmodel.DM_PHONGBAN_ID);
                if (DeptObj != null)
                {
                    resultmodel.DeptParentID = DeptObj.PARENT_ID;
                }
            }


            return resultmodel;
        }
        public UserInfoBO GetUserInfo(string userName)
        {
            var query = from tbl in this.context.DM_NGUOIDUNG
                        where tbl.TENDANGNHAP.Equals(userName) && tbl.IS_ACTIVE == true
                        join tblChucVu in this.context.DM_DANHMUC_DATA on tbl.CHUCVU_ID equals tblChucVu.ID into jchucvu
                        from chucvu in jchucvu.DefaultIfEmpty()
                        select new UserInfoBO
                        {
                            ID = tbl.ID,
                            ANH_DAIDIEN = tbl.ANH_DAIDIEN,
                            DIACHI = tbl.DIACHI,
                            DIENTHOAI = tbl.DIENTHOAI,
                            DM_PHONGBAN_ID = tbl.DM_PHONGBAN_ID,
                            EMAIL = tbl.EMAIL,
                            HOTEN = tbl.HOTEN,
                            MAHOA_MK = tbl.MAHOA_MK,
                            MATKHAU = tbl.MATKHAU,
                            NGAYSINH = tbl.NGAYSINH,
                            NGAYSUA = tbl.NGAYSUA,
                            NGAYTAO = tbl.NGAYTAO,
                            NGUOISUA = tbl.NGUOISUA,
                            NGUOITAO = tbl.NGUOITAO,
                            TENDANGNHAP = tbl.TENDANGNHAP,
                            TRANGTHAI = tbl.TRANGTHAI,
                            ChucVu = chucvu != null ? chucvu.TEXT : "Không có chức vụ",
                            CHUCVU_ID = chucvu != null ? chucvu.ID : 0,
                            signpath = tbl.signpath
                        };

            var resultmodel = query.FirstOrDefault();
            if (resultmodel != null)
            {
                resultmodel.ListChucNangMenu = GetListThaoTacByNguoiDung(resultmodel.ID);
                resultmodel.ListThaoTac = new List<DM_THAOTAC>();
                foreach (var item in resultmodel.ListChucNangMenu.Where(x => x.ListThaoTac != null && x.ListThaoTac.Any()).ToList())
                {
                    resultmodel.ListThaoTac.AddRange(item.ListThaoTac);
                }
                resultmodel.ListVaiTro = GetListVaiTro(resultmodel.ID);
                var DeptObj = this.context.CCTC_THANHPHAN.Find(resultmodel.DM_PHONGBAN_ID);
                var LstDepIds = new List<int>();
                if (DeptObj != null)
                {
                    resultmodel.DeptParentID = DeptObj.PARENT_ID;
                    resultmodel.DeptType = DeptObj.TYPE;
                    var DepParOject = this.context.CCTC_THANHPHAN.Find(resultmodel.DeptParentID);
                    if (DepParOject != null)
                    {
                        resultmodel.CanSendSMS = DepParOject.CAN_SEND_SMS.GetValueOrDefault();
                        resultmodel.TenPhongBan = DeptObj.NAME + " / " + DepParOject.NAME;
                        var LstDepId = this.context.CCTC_THANHPHAN.Where(x => x.PARENT_ID == DeptObj.PARENT_ID).Select(x => x.ID).ToList();
                        if (LstDepId.Count > 0)
                        {
                            LstDepIds.AddRange(LstDepId);
                        }
                    }
                    else
                    {
                        resultmodel.TenPhongBan = DeptObj.NAME;
                    }

                }
                #region lay danh sach nguoi dung de chat
                var listUserBO = this.repository.All().Where(o => o.ID != resultmodel.ID && LstDepIds.Contains(o.DM_PHONGBAN_ID.Value))
                    .Select(o => new { o.HOTEN, o.TENDANGNHAP, o.ID }).OrderBy(o => o.TENDANGNHAP).ToList();
                if (listUserBO != null && listUserBO.Count > 0)
                {
                    var idx = 0;
                    string ListUserName = "[";
                    List<UserBO> lst_UserBO = new List<UserBO>();
                    foreach (var item in listUserBO)
                    {
                        UserBO bo = new UserBO();
                        bo.idx = idx;
                        bo.label = item.HOTEN;
                        lst_UserBO.Add(bo);
                        if (idx == listUserBO.Count - 1)
                        {
                            ListUserName += "\"" + item.TENDANGNHAP + "\"]";
                        }
                        else
                        {
                            ListUserName += "\"" + item.TENDANGNHAP + "\", ";
                        }
                        idx++;
                    }
                    resultmodel.ListUserBO = lst_UserBO;
                    resultmodel.ListUserName = ListUserName;
                }
                #endregion
            }


            return resultmodel;
        }
        public List<DM_CHUCNANG_BO> GetListThaoTacByNguoiDung(long id)
        {
            var queryVaiTro = (from vaitrothaotac in this.context.VAITRO_THAOTAC
                               join tbl_vaitrongdung in this.context.NGUOIDUNG_VAITRO.Where(x => x.NGUOIDUNG_ID == id) on vaitrothaotac.VAITRO_ID equals tbl_vaitrongdung.VAITRO_ID
                               join thaotac in this.context.DM_THAOTAC on vaitrothaotac.DM_THAOTAC_ID equals thaotac.DM_THAOTAC_ID
                               group thaotac by thaotac.DM_CHUCNANG_ID into gchucnang
                               join tbl_chucnang in this.context.DM_CHUCNANG on gchucnang.Key equals tbl_chucnang.DM_CHUCNANG_ID into jchucnang
                               from chucnang in jchucnang.DefaultIfEmpty()
                               orderby chucnang.TT_HIENTHI
                               select new DM_CHUCNANG_BO
                               {
                                   DM_CHUCNANG_ID = gchucnang.Key.Value,
                                   TEN_CHUCNANG = chucnang.TEN_CHUCNANG,
                                   TT_HIENTHI = chucnang.TT_HIENTHI,
                                   CSSCLASS = chucnang.CSSCLASS,
                                   ICONPATH = chucnang.ICONPATH,
                                   IS_HIENTHI = chucnang.IS_HIENTHI,
                                   MA_CHUCNANG = chucnang.MA_CHUCNANG,
                                   TRANGTHAI = chucnang.TRANGTHAI,
                                   URL = chucnang.URL,
                                   ListThaoTac = gchucnang.Distinct().OrderBy(x => x.TT_HIENTHI).ToList()
                               }).ToList();
            // Lấy thao tác riêng của người dùng
            var queryThaotacRiengADD = (from nguoidungthaotac in this.context.DM_NGUOIDUNG_THAOTAC.Where(x => x.DM_NGUOIDUNG_ID == id && x.TRANGTHAI == true)
                                        join thaotac in this.context.DM_THAOTAC on nguoidungthaotac.DM_THAOTAC equals thaotac.DM_THAOTAC_ID
                                        join tbl_chucnang in this.context.DM_CHUCNANG on thaotac.DM_CHUCNANG_ID equals tbl_chucnang.DM_CHUCNANG_ID into jchucnang
                                        from chucnang in jchucnang.DefaultIfEmpty()
                                        group new { thaotac, chucnang } by chucnang into gchucnang
                                        where gchucnang.Key != null
                                        select new DM_CHUCNANG_BO
                                        {
                                            DM_CHUCNANG_ID = gchucnang.Key.DM_CHUCNANG_ID,
                                            TEN_CHUCNANG = gchucnang.Key.TEN_CHUCNANG,
                                            TT_HIENTHI = gchucnang.Key.TT_HIENTHI,
                                            CSSCLASS = gchucnang.Key.CSSCLASS,
                                            ICONPATH = gchucnang.Key.ICONPATH,
                                            IS_HIENTHI = gchucnang.Key.IS_HIENTHI,
                                            MA_CHUCNANG = gchucnang.Key.MA_CHUCNANG,
                                            TRANGTHAI = gchucnang.Key.TRANGTHAI,
                                            URL = gchucnang.Key.URL,
                                            ListThaoTac = gchucnang.Select(x => x.thaotac).Distinct().OrderBy(x => x.TT_HIENTHI).ToList()
                                        }).ToList();
            var queryThaotacRiengRemove = (from nguoidungthaotac in this.context.DM_NGUOIDUNG_THAOTAC.Where(x => x.DM_NGUOIDUNG_ID == id && x.TRANGTHAI != true)
                                           join thaotac in this.context.DM_THAOTAC on nguoidungthaotac.DM_THAOTAC equals thaotac.DM_THAOTAC_ID
                                           group thaotac by thaotac.DM_CHUCNANG_ID into gchucnang
                                           join tbl_chucnang in this.context.DM_CHUCNANG on gchucnang.Key equals tbl_chucnang.DM_CHUCNANG_ID into jchucnang
                                           from chucnang in jchucnang.DefaultIfEmpty()
                                           select new DM_CHUCNANG_BO
                                           {
                                               DM_CHUCNANG_ID = gchucnang.Key.Value,
                                               TEN_CHUCNANG = chucnang.TEN_CHUCNANG,
                                               TT_HIENTHI = chucnang.TT_HIENTHI,
                                               CSSCLASS = chucnang.CSSCLASS,
                                               ICONPATH = chucnang.ICONPATH,
                                               IS_HIENTHI = chucnang.IS_HIENTHI,
                                               MA_CHUCNANG = chucnang.MA_CHUCNANG,
                                               TRANGTHAI = chucnang.TRANGTHAI,
                                               URL = chucnang.URL,
                                               ListThaoTac = gchucnang.Distinct().OrderBy(x => x.TT_HIENTHI).ToList()
                                           }).ToList();
            foreach (var item in queryThaotacRiengADD)
            {

                var objtt = queryVaiTro.Where(x => x.DM_CHUCNANG_ID == item.DM_CHUCNANG_ID).FirstOrDefault();
                //Chức năng thao tác riêng không tồn tại trong vai trò
                if (objtt == null)
                {
                    queryVaiTro.Add(item);
                }
                else
                {
                    objtt.ListThaoTac.AddRange(item.ListThaoTac);
                    objtt.ListThaoTac = objtt.ListThaoTac.Distinct().OrderBy(x => x.TT_HIENTHI).ToList();
                }
            }

            foreach (var item in queryThaotacRiengRemove)
            {

                var objtt = queryVaiTro.Where(x => x.DM_CHUCNANG_ID == item.DM_CHUCNANG_ID).FirstOrDefault();
                //Chức năng thao tác riêng không tồn tại trong vai trò
                if (objtt != null)
                {
                    foreach (var thaotac in objtt.ListThaoTac.ToList())
                    {
                        if (item.ListThaoTac.Contains(thaotac))
                        {
                            objtt.ListThaoTac.Remove(thaotac);
                        }
                    }
                }
            }
            queryVaiTro = queryVaiTro.OrderBy(x => x.TT_HIENTHI).ToList();
            return queryVaiTro;
        }
        public List<NguoiDungPhongBanBO> GetNguoiDungFlow(WF_STEP_USER_PROCESS userprocess, long currentid)
        {
            var modelresult = new List<NguoiDungPhongBanBO>();
            var lstPhongBan = new List<int>();
            var userCurrent = this.repository.Find(currentid);
            var listPhongBanALL = this.context.CCTC_THANHPHAN.ToList();
            if (userCurrent == null || userCurrent.DM_PHONGBAN_ID == null)
            {
                return modelresult;
            }
            #region Lấy danh sách phòng ban con
            var queue = new Queue();
            queue.Enqueue(userCurrent.DM_PHONGBAN_ID);
            while (queue.Count > 0)
            {
                var parent = (int)queue.Dequeue();
                var item = listPhongBanALL.Where(x => x.PARENT_ID == parent).ToList();
                if (item.Count > 0)
                {
                    lstPhongBan.AddRange(item.Select(x => x.ID).ToList());
                    foreach (var cc in item)
                    {
                        queue.Enqueue(cc.ID);
                    }
                }
            }
            #endregion
            //chỉ gửi trong phòng
            if (userprocess.IS_CUNGPHONG == true)
            {
                lstPhongBan.Add(userCurrent.DM_PHONGBAN_ID.Value);
            }
            var currentDeparment = listPhongBanALL.Find(x => x.ID == userCurrent.DM_PHONGBAN_ID);
            var Parrent = listPhongBanALL.Find(x => x.ID == currentDeparment.PARENT_ID);
            //Gửi cho phòng ban cấp trên
            if (userprocess.IS_CUNGNHANH == true)
            {

                do
                {
                    if (Parrent != null)
                    {
                        lstPhongBan.Add(Parrent.ID);
                        Parrent = listPhongBanALL.Find(x => x.ID == Parrent.PARENT_ID && x.CATEGORY == Parrent.CATEGORY);
                    }


                } while (Parrent != null);
            }
            //Gửi phòng ban độc lập
            if (userprocess.PHONGBAN_ID != null)
            {
                lstPhongBan.Add(userprocess.PHONGBAN_ID.Value);
            }
            if (userprocess.IS_CUNGPHONG != true && userprocess.PHONGBAN_ID == null && userprocess.IS_CUNGNHANH != true)
            {
                lstPhongBan = this.context.CCTC_THANHPHAN.Where(x => x.CATEGORY == currentDeparment.CATEGORY && x.PARENT_ID == currentDeparment.PARENT_ID).Select(x => x.ID).ToList();
            }

            var ngdungQuery = from ngdung in this.context.DM_NGUOIDUNG
                              where ngdung.DM_PHONGBAN_ID != null && lstPhongBan.Contains(ngdung.DM_PHONGBAN_ID.Value) && ngdung.IS_ACTIVE == true
                              select ngdung;

            if (userprocess.CHUCVU_ID != null)
            {
                ngdungQuery = ngdungQuery.Where(x => x.CHUCVU_ID == userprocess.CHUCVU_ID);
            }
            if (userprocess.VAITRO_ID != null)
            {
                ngdungQuery = from ngdung in ngdungQuery
                              join tblvaitro in this.context.NGUOIDUNG_VAITRO.Where(x => x.VAITRO_ID == userprocess.VAITRO_ID) on ngdung.ID equals tblvaitro.NGUOIDUNG_ID
                              select ngdung;
            }
            var query = from ngdung in ngdungQuery

                        join tblpb in this.context.CCTC_THANHPHAN on ngdung.DM_PHONGBAN_ID equals tblpb.ID into jphong
                        from phongban in jphong.DefaultIfEmpty()

                        group new { ngdung, phongban } by phongban into gPhong
                        select new NguoiDungPhongBanBO
                        {
                            PhongBan = gPhong.Key,
                            LstNguoiDung = (from nd in gPhong.Select(x => x.ngdung)
                                            join tblChucVu in this.context.DM_DANHMUC_DATA on nd.CHUCVU_ID equals tblChucVu.ID into jchucvu
                                            from chucvu in jchucvu.DefaultIfEmpty()
                                            select new DM_NGUOIDUNG_BO
                                            {
                                                ID = nd.ID,
                                                ANH_DAIDIEN = nd.ANH_DAIDIEN,
                                                DIACHI = nd.DIACHI,
                                                DIENTHOAI = nd.DIENTHOAI,
                                                DM_PHONGBAN_ID = nd.DM_PHONGBAN_ID,
                                                EMAIL = nd.EMAIL,
                                                HOTEN = nd.HOTEN,
                                                NGAYSINH = nd.NGAYSINH,
                                                TENDANGNHAP = nd.TENDANGNHAP,
                                                ChucVu = chucvu != null ? chucvu.TEXT : "Không có chức vụ"
                                            }).ToList()
                        };

            return query.OrderBy(x => x.PhongBan.ITEM_LEVEL).ToList();

        }

        /// <summary>
        /// @description: tìm kiếm người dùng trong luồng xử lý
        /// @author: duynn
        /// @since: 25/05/2018
        /// </summary>
        /// <param name="userprocess"></param>
        /// <param name="currentid"></param>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<NguoiDungPhongBanBO> SearchUserInFlow(WF_STEP_USER_PROCESS userprocess, long currentid, string query, int pageIndex = 1, int pageSize = 5)
        {
            var modelresult = new List<NguoiDungPhongBanBO>();
            var lstPhongBan = new List<int>();
            var userCurrent = this.repository.Find(currentid);
            var listPhongBanALL = this.context.CCTC_THANHPHAN.ToList();
            if (userCurrent == null || userCurrent.DM_PHONGBAN_ID == null)
            {
                return modelresult;
            }
            #region Lấy danh sách phòng ban con


            var queue = new Queue();
            queue.Enqueue(userCurrent.DM_PHONGBAN_ID);
            while (queue.Count > 0)
            {
                var parent = (int)queue.Dequeue();
                var item = listPhongBanALL.Where(x => x.PARENT_ID == parent).ToList();
                if (item.Count > 0)
                {
                    lstPhongBan.AddRange(item.Select(x => x.ID).ToList());
                    foreach (var cc in item)
                    {
                        queue.Enqueue(cc.ID);
                    }
                }
            }
            #endregion
            //chỉ gửi trong phòng
            if (userprocess.IS_CUNGPHONG == true)
            {
                lstPhongBan.Add(userCurrent.DM_PHONGBAN_ID.Value);
            }
            //Gửi cho phòng ban cấp trên
            if (userprocess.IS_CUNGNHANH == true)
            {
                var currentDeparment = listPhongBanALL.Find(x => x.ID == userCurrent.DM_PHONGBAN_ID);
                var Parrent = listPhongBanALL.Find(x => x.ID == currentDeparment.PARENT_ID);
                do
                {
                    if (Parrent != null)
                    {
                        lstPhongBan.Add(Parrent.ID);
                        Parrent = listPhongBanALL.Find(x => x.ID == Parrent.PARENT_ID);
                    }


                } while (Parrent != null);
            }
            //Gửi phòng ban độc lập
            if (userprocess.PHONGBAN_ID != null)
            {
                lstPhongBan.Add(userprocess.PHONGBAN_ID.Value);
            }
            if (userprocess.IS_CUNGPHONG != true && userprocess.PHONGBAN_ID == null && userprocess.IS_CUNGNHANH != true)
            {
                lstPhongBan = this.context.CCTC_THANHPHAN.Select(x => x.ID).ToList();
            }

            var ngdungQuery = from ngdung in this.context.DM_NGUOIDUNG
                              where ngdung.DM_PHONGBAN_ID != null && lstPhongBan.Contains(ngdung.DM_PHONGBAN_ID.Value) && ngdung.IS_ACTIVE == true
                              select ngdung;
            if (userprocess.CHUCVU_ID != null)
            {
                ngdungQuery = ngdungQuery.Where(x => x.CHUCVU_ID == userprocess.CHUCVU_ID);
            }
            if (userprocess.VAITRO_ID != null)
            {
                ngdungQuery = from ngdung in ngdungQuery
                              join tblvaitro in this.context.NGUOIDUNG_VAITRO.Where(x => x.VAITRO_ID == userprocess.VAITRO_ID) on ngdung.ID equals tblvaitro.NGUOIDUNG_ID
                              select ngdung;
            }

            if (!string.IsNullOrEmpty(query))
            {
                ngdungQuery = ngdungQuery.Where(x => x.HOTEN != null && x.HOTEN.Trim().ToLower().Contains(query.Trim().ToLower()));
            }
            var result = (from ngdung in ngdungQuery

                          join tblpb in this.context.CCTC_THANHPHAN on ngdung.DM_PHONGBAN_ID equals tblpb.ID into jphong
                          from phongban in jphong.DefaultIfEmpty()

                          group new { ngdung, phongban } by phongban into gPhong
                          select new NguoiDungPhongBanBO
                          {
                              PhongBan = gPhong.Key,
                              LstNguoiDung = (from nd in gPhong.Select(x => x.ngdung)
                                              join tblChucVu in this.context.DM_DANHMUC_DATA on nd.CHUCVU_ID equals tblChucVu.ID into jchucvu
                                              from chucvu in jchucvu.DefaultIfEmpty()
                                              select new DM_NGUOIDUNG_BO
                                              {
                                                  ID = nd.ID,
                                                  ANH_DAIDIEN = nd.ANH_DAIDIEN,
                                                  DIACHI = nd.DIACHI,
                                                  DIENTHOAI = nd.DIENTHOAI,
                                                  DM_PHONGBAN_ID = nd.DM_PHONGBAN_ID,
                                                  EMAIL = nd.EMAIL,
                                                  HOTEN = nd.HOTEN,
                                                  NGAYSINH = nd.NGAYSINH,
                                                  TENDANGNHAP = nd.TENDANGNHAP,
                                                  ChucVu = chucvu != null ? chucvu.TEXT : "Không có chức vụ"
                                              }).ToList()
                          }).OrderBy(x => x.PhongBan.ITEM_LEVEL).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;

        }

        /// <summary>
        /// @description: tìm kiếm người dùng review
        /// @author: duynn
        /// @since: 25/05/2018
        /// </summary>
        /// <param name="requestUserId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<NguoiDungPhongBanBO> SearchUserReview(long requestUserId, string query, int pageIndex = 1, int pageSize = 5)
        {
            List<int> deptIds = this.context.CCTC_THANHPHAN.Select(x => x.ID).ToList();
            var ngdungQuery = from ngdung in this.context.DM_NGUOIDUNG
                              where ngdung.DM_PHONGBAN_ID != null && deptIds.Contains(ngdung.DM_PHONGBAN_ID.Value) && ngdung.ID != requestUserId
                              && ngdung.IS_ACTIVE == true
                              select ngdung;
            if (!string.IsNullOrEmpty(query))
            {
                ngdungQuery = ngdungQuery.Where(x => x.HOTEN != null && x.HOTEN.Trim().ToLower().Contains(query.Trim().ToLower()));
            }

            var result = (from ngdung in ngdungQuery

                          join tblpb in this.context.CCTC_THANHPHAN on ngdung.DM_PHONGBAN_ID equals tblpb.ID into jphong
                          from phongban in jphong.DefaultIfEmpty()

                          group new { ngdung, phongban } by phongban into gPhong
                          select new NguoiDungPhongBanBO
                          {
                              PhongBan = gPhong.Key,
                              LstNguoiDung = (from nd in gPhong.Select(x => x.ngdung)
                                              join tblChucVu in this.context.DM_DANHMUC_DATA on nd.CHUCVU_ID equals tblChucVu.ID into jchucvu
                                              from chucvu in jchucvu.DefaultIfEmpty()
                                              select new DM_NGUOIDUNG_BO
                                              {
                                                  ID = nd.ID,
                                                  ANH_DAIDIEN = nd.ANH_DAIDIEN,
                                                  DIACHI = nd.DIACHI,
                                                  DIENTHOAI = nd.DIENTHOAI,
                                                  DM_PHONGBAN_ID = nd.DM_PHONGBAN_ID,
                                                  EMAIL = nd.EMAIL,
                                                  HOTEN = nd.HOTEN,
                                                  NGAYSINH = nd.NGAYSINH,
                                                  TENDANGNHAP = nd.TENDANGNHAP,
                                                  ChucVu = chucvu != null ? chucvu.TEXT : "Không có chức vụ"
                                              }).ToList()
                          }).OrderBy(x => x.PhongBan.ITEM_LEVEL).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }

        /// <summary>
        /// @description: lấy danh sách người dùng và phòng ban từ danh sách mã người dùng (dùng khi giao việc)
        /// @author: duynn
        /// @since: 31/05/2018
        /// </summary>
        /// <param name="groupUsersIds"></param>
        /// <para
        /// <returns></returns>
        public List<NguoiDungPhongBanBO> GetUsersDepartmentByGroupUserIdsAndDeptIds(List<long?> groupUsersIds, List<int?> groupDeptIds, string query, int pageIndex, int pageSize = 5)
        {
            var queryUsers = from ngdung in this.context.DM_NGUOIDUNG
                             where ngdung.DM_PHONGBAN_ID != null && groupDeptIds.Contains(ngdung.DM_PHONGBAN_ID.Value)
                             && groupUsersIds.Contains(ngdung.ID)
                             select ngdung;

            if (!string.IsNullOrEmpty(query))
            {
                queryUsers = queryUsers.Where(x => x.HOTEN != null && x.HOTEN.Trim().ToLower().Contains(query.Trim().ToLower()));
            }

            var result = (from ngdung in queryUsers

                          join tblpb in this.context.CCTC_THANHPHAN on ngdung.DM_PHONGBAN_ID equals tblpb.ID into jphong
                          from phongban in jphong.DefaultIfEmpty()

                          group new { ngdung, phongban } by phongban into gPhong
                          select new NguoiDungPhongBanBO
                          {
                              PhongBan = gPhong.Key,
                              LstNguoiDung = (from nd in gPhong.Select(x => x.ngdung)
                                              join tblChucVu in this.context.DM_DANHMUC_DATA on nd.CHUCVU_ID equals tblChucVu.ID into jchucvu
                                              from chucvu in jchucvu.DefaultIfEmpty()
                                              select new DM_NGUOIDUNG_BO
                                              {
                                                  ID = nd.ID,
                                                  ANH_DAIDIEN = nd.ANH_DAIDIEN,
                                                  DIACHI = nd.DIACHI,
                                                  DIENTHOAI = nd.DIENTHOAI,
                                                  DM_PHONGBAN_ID = nd.DM_PHONGBAN_ID,
                                                  EMAIL = nd.EMAIL,
                                                  HOTEN = nd.HOTEN,
                                                  NGAYSINH = nd.NGAYSINH,
                                                  TENDANGNHAP = nd.TENDANGNHAP,
                                                  ChucVu = chucvu != null ? chucvu.TEXT : "Không có chức vụ"
                                              }).ToList()
                          }).OrderBy(x => x.PhongBan.ITEM_LEVEL).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }


        public List<NguoiDungPhongBanBO> GetNguoiDungReview(long currentUserId)
        {
            var modelresult = new List<NguoiDungPhongBanBO>();
            var lstPhongBan = new List<int>();

            lstPhongBan = this.context.CCTC_THANHPHAN.Select(x => x.ID).ToList();
            var ngdungQuery = from ngdung in this.context.DM_NGUOIDUNG
                              where ngdung.DM_PHONGBAN_ID != null && lstPhongBan.Contains(ngdung.DM_PHONGBAN_ID.Value) && ngdung.ID != currentUserId && ngdung.IS_ACTIVE == true
                              select ngdung;

            var query = from ngdung in ngdungQuery

                        join tblpb in this.context.CCTC_THANHPHAN on ngdung.DM_PHONGBAN_ID equals tblpb.ID into jphong
                        from phongban in jphong.DefaultIfEmpty()

                        group new { ngdung, phongban } by phongban into gPhong
                        select new NguoiDungPhongBanBO
                        {
                            PhongBan = gPhong.Key,
                            LstNguoiDung = (from nd in gPhong.Select(x => x.ngdung)
                                            join tblChucVu in this.context.DM_DANHMUC_DATA on nd.CHUCVU_ID equals tblChucVu.ID into jchucvu
                                            from chucvu in jchucvu.DefaultIfEmpty()
                                            select new DM_NGUOIDUNG_BO
                                            {
                                                ID = nd.ID,
                                                ANH_DAIDIEN = nd.ANH_DAIDIEN,
                                                DIACHI = nd.DIACHI,
                                                DIENTHOAI = nd.DIENTHOAI,
                                                DM_PHONGBAN_ID = nd.DM_PHONGBAN_ID,
                                                EMAIL = nd.EMAIL,
                                                HOTEN = nd.HOTEN,
                                                NGAYSINH = nd.NGAYSINH,
                                                TENDANGNHAP = nd.TENDANGNHAP,
                                                ChucVu = chucvu != null ? chucvu.TEXT : "Không có chức vụ"
                                            }).ToList()
                        };

            return query.OrderBy(x => x.PhongBan.ITEM_LEVEL).ToList();

        }
        public List<SelectListItem> GetUserByRoleAndDeptId(int ROLE_ID, int DEPT_ID, long SelectedId = 0)
        {
            List<long?> LstUserId = this.context.NGUOIDUNG_VAITRO.Where(x => x.VAITRO_ID == ROLE_ID).Select(x => x.NGUOIDUNG_ID).ToList();

            var result = (from user in this.context.DM_NGUOIDUNG
                          join dept in this.context.CCTC_THANHPHAN on user.DM_PHONGBAN_ID equals dept.ID
                          where dept.ID == DEPT_ID && LstUserId.Contains(user.ID) && user.IS_ACTIVE == true
                          select new SelectListItem
                          {
                              Text = user.HOTEN,
                              Value = user.ID.ToString(),
                              Selected = user.ID == SelectedId
                          });
            return result.ToList();
        }
        public List<SelectListItem> GetUserByRoleAndParentDept(int ROLE_ID, int PARRENT_DEPT_ID, long SelectedId = 0)
        {
            List<long?> LstUserId = this.context.NGUOIDUNG_VAITRO.Where(x => x.VAITRO_ID == ROLE_ID).Select(x => x.NGUOIDUNG_ID).ToList();

            var result = (from user in this.context.DM_NGUOIDUNG
                              //join dept in this.context.CCTC_THANHPHAN on user.DM_PHONGBAN_ID equals dept.ID
                              //where dept.PARENT_ID == PARRENT_DEPT_ID && LstUserId.Contains(user.ID) && user.IS_ACTIVE == true
                          join dept in this.context.CCTC_THANHPHAN on user.DM_PHONGBAN_ID equals dept.ID
                          where dept.PARENT_ID == PARRENT_DEPT_ID && LstUserId.Contains(user.ID) && user.IS_ACTIVE == true
                          select new SelectListItem
                          {
                              Text = user.HOTEN,
                              Value = user.ID.ToString(),
                              Selected = user.ID == SelectedId
                          });
            return result.ToList();
        }
        public List<SelectListItem> GetUserByRoleAndListDept(int ROLE_ID, List<int> LstDept)
        {
            List<long?> LstUserId = this.context.NGUOIDUNG_VAITRO.Where(x => x.VAITRO_ID == ROLE_ID).Select(x => x.NGUOIDUNG_ID).ToList();
            var result = (from user in this.context.DM_NGUOIDUNG
                          where LstUserId.Contains(user.ID) && user.IS_ACTIVE == true && LstDept.Contains(user.DM_PHONGBAN_ID.Value)
                          select new SelectListItem
                          {
                              Text = user.TENDANGNHAP,
                              Value = user.ID.ToString()
                          });
            return result.ToList();
        }
        public List<SelectListItem> GetAllUsers()
        {
            var result = (from user in this.context.DM_NGUOIDUNG
                          select new SelectListItem
                          {
                              Text = user.HOTEN,
                              Value = user.ID.ToString()
                          });
            return result.ToList();
        }
        public List<SelectListItem> GetUserByRoleBanLanhDao(int ROLE_ID)
        {
            List<long?> LstUserId = this.context.NGUOIDUNG_VAITRO.Where(x => x.VAITRO_ID == ROLE_ID).Select(x => x.NGUOIDUNG_ID).ToList();
            var result = (from user in this.context.DM_NGUOIDUNG
                          join dept in this.context.CCTC_THANHPHAN on user.DM_PHONGBAN_ID equals dept.ID
                          where LstUserId.Contains(user.ID) && user.IS_ACTIVE == true
                          select new SelectListItem
                          {
                              Text = user.HOTEN,
                              Value = user.ID.ToString()
                          });
            return result.ToList();
        }
        public List<DM_NGUOIDUNG_BO> GetByPhongBan(List<int> Ids, long userId, List<int> RoleId)
        {
            var result = (from nguoiDung in this.context.DM_NGUOIDUNG
                          join donvi in this.context.CCTC_THANHPHAN
                          on nguoiDung.DM_PHONGBAN_ID equals donvi.ID
                          into group1
                          from g1 in group1.DefaultIfEmpty()
                          join luutru in this.context.DUNGLUONG_LUUTRU
                          on nguoiDung.ID equals luutru.USER_ID
                          into group2
                          from g2 in group2.DefaultIfEmpty()
                          where nguoiDung.DM_PHONGBAN_ID.HasValue && Ids.Contains(nguoiDung.DM_PHONGBAN_ID.Value) && nguoiDung.IS_ACTIVE == true

                          select new DM_NGUOIDUNG_BO
                          {
                              DIENTHOAI = nguoiDung.DIENTHOAI,
                              ID = nguoiDung.ID,
                              EMAIL = nguoiDung.EMAIL,
                              TEN_DONVI = g1.NAME,
                              HOTEN = nguoiDung.HOTEN,
                              TENDANGNHAP = nguoiDung.TENDANGNHAP,
                              DUNGLUONG = g2.DUNGLUONG,
                              TRANGTHAI_2 = g2.TRANGTHAI,
                              TYPE = g2.TYPE,
                              DM_PHONGBAN_ID = nguoiDung.DM_PHONGBAN_ID
                          });
            if (userId > 0)
            {
                result = result.Where(x => x.ID != userId);
            }
            if (RoleId.Any())
            {
                List<long?> LstUserId = this.context.NGUOIDUNG_VAITRO.Where(x => x.VAITRO_ID.HasValue
                    && RoleId.Contains(x.VAITRO_ID.Value)).Select(x => x.NGUOIDUNG_ID).ToList();
                result = result.Where(x => LstUserId.Contains(x.ID));
            }
            return result.OrderBy(x => x.ID).ToList();
        }
        private List<DM_VAITRO> GetListVaiTro(long id)
        {
            var result = (from vaitro in this.context.NGUOIDUNG_VAITRO
                          where vaitro.NGUOIDUNG_ID.HasValue && vaitro.NGUOIDUNG_ID.Value == id
                          && vaitro.VAITRO_ID.HasValue
                          select vaitro.VAITRO_ID.Value).ToList();
            var query = from vaitro in this.context.DM_VAITRO
                        where result.Contains(vaitro.DM_VAITRO_ID)
                        select vaitro;
            return query.ToList();
        }
        public UserInfoBO GetUserInfo(long id)
        {
            var query = from tbl in this.context.DM_NGUOIDUNG
                        where tbl.ID == id && tbl.IS_ACTIVE == true
                        join tblChucVu in this.context.DM_DANHMUC_DATA on tbl.CHUCVU_ID equals tblChucVu.ID into jchucvu
                        from chucvu in jchucvu.DefaultIfEmpty()
                        select new UserInfoBO
                        {
                            ID = tbl.ID,
                            ANH_DAIDIEN = tbl.ANH_DAIDIEN,
                            DIACHI = tbl.DIACHI,
                            DIENTHOAI = tbl.DIENTHOAI,
                            DM_PHONGBAN_ID = tbl.DM_PHONGBAN_ID,
                            EMAIL = tbl.EMAIL,
                            HOTEN = tbl.HOTEN,
                            MAHOA_MK = tbl.MAHOA_MK,
                            MATKHAU = tbl.MATKHAU,
                            NGAYSINH = tbl.NGAYSINH,
                            NGAYSUA = tbl.NGAYSUA,
                            NGAYTAO = tbl.NGAYTAO,
                            NGUOISUA = tbl.NGUOISUA,
                            NGUOITAO = tbl.NGUOITAO,
                            TENDANGNHAP = tbl.TENDANGNHAP,
                            TRANGTHAI = tbl.TRANGTHAI,
                            ChucVu = chucvu != null ? chucvu.TEXT : "Không có chức vụ",
                            CHUCVU_ID = chucvu != null ? chucvu.ID : 0
                        };

            var resultmodel = query.FirstOrDefault();
            var LstDepIds = new List<int>();
            if (resultmodel != null)
            {
                resultmodel.ListChucNangMenu = GetListThaoTacByNguoiDung(resultmodel.ID);
                resultmodel.ListThaoTac = new List<DM_THAOTAC>();
                foreach (var item in resultmodel.ListChucNangMenu.Where(x => x.ListThaoTac != null && x.ListThaoTac.Any()).ToList())
                {
                    resultmodel.ListThaoTac.AddRange(item.ListThaoTac);
                }
                resultmodel.ListVaiTro = GetListVaiTro(resultmodel.ID);
                var DeptObj = this.context.CCTC_THANHPHAN.Find(resultmodel.DM_PHONGBAN_ID);
                if (DeptObj != null)
                {
                    resultmodel.DeptParentID = DeptObj.PARENT_ID;
                    var LstDepId = this.context.CCTC_THANHPHAN.Where(x => x.PARENT_ID == DeptObj.PARENT_ID).Select(x => x.ID).ToList();
                    if (LstDepId.Count > 0)
                    {
                        LstDepIds.AddRange(LstDepId);
                    }
                }

            }
            #region lay danh sach nguoi dung de chat
            var listUserBO = this.repository.All().Where(o => o.ID != id && LstDepIds.Contains(o.DM_PHONGBAN_ID.Value))
                .Select(o => new { o.HOTEN, o.TENDANGNHAP, o.ID }).OrderBy(o => o.TENDANGNHAP).ToList();
            if (listUserBO != null && listUserBO.Count > 0)
            {
                var idx = 0;
                string ListUserName = "[";
                List<UserBO> lst_UserBO = new List<UserBO>();
                foreach (var item in listUserBO)
                {
                    UserBO bo = new UserBO();
                    bo.idx = idx;
                    bo.label = item.HOTEN;
                    lst_UserBO.Add(bo);
                    if (idx == listUserBO.Count - 1)
                    {
                        ListUserName += "\"" + item.TENDANGNHAP + "\"]";
                    }
                    else
                    {
                        ListUserName += "\"" + item.TENDANGNHAP + "\", ";
                    }
                    idx++;
                }
                resultmodel.ListUserBO = lst_UserBO;
                resultmodel.ListUserName = ListUserName;
            }
            #endregion

            return resultmodel;
        }
        public List<DM_NGUOIDUNG> GetData()
        {
            var result = from nguoidung in this.context.DM_NGUOIDUNG.AsNoTracking()
                         where nguoidung.IS_ACTIVE == true
                         select nguoidung;
            return result.ToList();
        }
        public List<DM_NGUOIDUNG> GetData(List<long?> ids)
        {
            var result = from nguoidung in this.context.DM_NGUOIDUNG.AsNoTracking()
                         where ids.Contains(nguoidung.ID) && nguoidung.IS_ACTIVE == true
                         select nguoidung;
            return result.ToList();
        }
        public List<SelectListItem> GetDsNguoiDung(int UnitId)
        {
            List<SelectListItem> listResult = this.repository.All().Where(x => UnitId == x.DM_PHONGBAN_ID)
                .OrderBy(x => x.HOTEN).ThenBy(x => x.ID)
                .Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.HOTEN,
                }).ToList();
            return listResult;
        }
        public List<DM_NGUOIDUNG> GetDataByUnitId(List<int> Ids)
        {
            var result = from nguoidung in this.context.DM_NGUOIDUNG
                         where nguoidung.DM_PHONGBAN_ID.HasValue && Ids.Contains(nguoidung.DM_PHONGBAN_ID.Value)
                         select nguoidung;
            return result.ToList();
        }

        /// <summary>
        /// @description: lấy thông tin người dùng bằng email
        /// @author: duynn
        /// @since: 14/06/2018
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public UserInfoBO GetUserByEmail(string email)
        {
            var query = from tbl in this.context.DM_NGUOIDUNG
                        where tbl.EMAIL.Equals(email) && tbl.IS_ACTIVE == true
                        join tblChucVu in this.context.DM_DANHMUC_DATA on tbl.CHUCVU_ID equals tblChucVu.ID into jchucvu
                        from chucvu in jchucvu.DefaultIfEmpty()
                        select new UserInfoBO
                        {
                            ID = tbl.ID,
                            ANH_DAIDIEN = tbl.ANH_DAIDIEN,
                            DIACHI = tbl.DIACHI,
                            DIENTHOAI = tbl.DIENTHOAI,
                            DM_PHONGBAN_ID = tbl.DM_PHONGBAN_ID,
                            EMAIL = tbl.EMAIL,
                            HOTEN = tbl.HOTEN,
                            MAHOA_MK = tbl.MAHOA_MK,
                            MATKHAU = tbl.MATKHAU,
                            NGAYSINH = tbl.NGAYSINH,
                            NGAYSUA = tbl.NGAYSUA,
                            NGAYTAO = tbl.NGAYTAO,
                            NGUOISUA = tbl.NGUOISUA,
                            NGUOITAO = tbl.NGUOITAO,
                            TENDANGNHAP = tbl.TENDANGNHAP,
                            TRANGTHAI = tbl.TRANGTHAI,
                            ChucVu = chucvu != null ? chucvu.TEXT : "Không có chức vụ",
                            CHUCVU_ID = chucvu != null ? chucvu.ID : 0
                        };

            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách người dùng kèm vai trò
        /// @since: 14/08/2018
        /// </summary>
        /// <param name="listSelected"></param>
        /// <returns></returns>
        public List<SelectListItem> GetDropDownData(List<long> listSelected)
        {
            List<SelectListItem> result = this.context.DM_NGUOIDUNG.Where(x => x.IS_ACTIVE == true)
                .Join(this.context.DM_DANHMUC_DATA,
                    user => user.CHUCVU_ID,
                    category => category.ID, (user, category) =>
                 new SelectListItem()
                 {
                     Value = user.ID.ToString(),
                     Text = user.HOTEN + " - " + category.TEXT,
                     Selected = listSelected.Contains(user.ID)
                 }).ToList();
            return result;
        }
        public List<SelectListItem> GetDanhSachTacGia(int deptparentId, long selected = 0)
        {
            IQueryable<int> listSameParentDeptIds = this.context.CCTC_THANHPHAN
                .Where(x => x.PARENT_ID == deptparentId).Select(x => x.ID);
            List<SelectListItem> result = (from user in this.context.DM_NGUOIDUNG.Where(x => x.IS_ACTIVE != false)
                                           .Where(x => x.DM_PHONGBAN_ID != null && listSameParentDeptIds.Contains(x.DM_PHONGBAN_ID.Value))
                                           select new SelectListItem()
                                           {

                                               Value = user.ID.ToString(),
                                               Text = user.HOTEN + " ( " + user.TENDANGNHAP + ")",
                                               Selected = user.ID == selected
                                           }).ToList();
            return result;
        }
        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách người dùng có cùng cấp đơn vị
        /// </summary>
        /// <param name="deptParentId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetDropDownByDeptParentId(int deptParentId, long selected)
        {
            List<string> leaderRoles = new List<string>() { "BANTONGGIAMDOC", "GIAMDOCDONVI" };

            IQueryable<int> listSameParentDeptIds = this.context.CCTC_THANHPHAN
                .Where(x => x.PARENT_ID == deptParentId).Select(x => x.ID);
            List<SelectListItem> result = (from user in this.context.DM_NGUOIDUNG.Where(x => x.IS_ACTIVE != false)
                                           .Where(x => x.DM_PHONGBAN_ID != null && listSameParentDeptIds.Contains(x.DM_PHONGBAN_ID.Value))
                                           join category in this.context.DM_DANHMUC_DATA
                                           on user.CHUCVU_ID equals category.ID

                                           join userRole in this.context.NGUOIDUNG_VAITRO
                                           on user.ID equals userRole.NGUOIDUNG_ID

                                           join role in this.context.DM_VAITRO.Where(x => leaderRoles.Contains(x.MA_VAITRO))
                                           on userRole.VAITRO_ID equals role.DM_VAITRO_ID
                                           select new SelectListItem()
                                           {

                                               Value = user.ID.ToString(),
                                               Text = user.HOTEN + " - " + category.TEXT,
                                               Selected = user.ID == selected
                                           }).ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: kiểm tra là lãnh đạo hay không
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckIsLeader(long userId)
        {
            List<string> leaderRoles = new List<string>() { "BANTONGGIAMDOC", "GIAMDOCDONVI" };

            IQueryable<NGUOIDUNG_VAITRO> roles = (from userRole in this.context.NGUOIDUNG_VAITRO.Where(x => x.NGUOIDUNG_ID == userId)
                                                  join role in this.context.DM_VAITRO.Where(x => leaderRoles.Contains(x.MA_VAITRO))
                                                  on userRole.VAITRO_ID equals role.DM_VAITRO_ID
                                                  select userRole);
            bool isLeader = roles.Count() > 0;
            return isLeader;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách người dùng có cùng cấp đơn vị và có thao tác
        /// </summary>
        /// <param name="functionCode">mã thao tác</param>
        /// <param name="deptId">mã phòng ban cùng cấp</param>
        /// <returns></returns>

        public List<long> GetListUsersByFunctionCodeAndDeptId(string functionCode, int deptId)
        {
            List<long> result = new List<long>();
            //lấy những người dùng cùng cấp đơn vị
            List<long> sameDeptUserIds = (from user in this.context.DM_NGUOIDUNG.Where(x => x.IS_ACTIVE != false)
                                          join dept in this.context.CCTC_THANHPHAN.Where(x => x.IS_DELETE != true)
                                          on user.DM_PHONGBAN_ID equals dept.ID
                                          where dept.PARENT_ID == deptId
                                          select user.ID).ToList();
            //lấy những người dùng có vai trò được phân thao tác
            List<long> rolesHaveFunction = (from roleFunction in this.context.VAITRO_THAOTAC
                                            join userRole in this.context.NGUOIDUNG_VAITRO
                                            on roleFunction.VAITRO_ID equals userRole.VAITRO_ID
                                            join func in this.context.DM_THAOTAC
                                            on roleFunction.DM_THAOTAC_ID equals func.DM_THAOTAC_ID
                                            join user in this.context.DM_NGUOIDUNG
                                            on userRole.NGUOIDUNG_ID equals user.ID
                                            where func.MA_THAOTAC == functionCode
                                            select user.ID).ToList();
            //lấy những người dùng được phân thao tác
            List<long> usersHaveFunction = (from funcUser in this.context.DM_NGUOIDUNG_THAOTAC.Where(x => x.TRANGTHAI == true)
                                            join func in this.context.DM_THAOTAC
                                            on funcUser.DM_THAOTAC equals func.DM_THAOTAC_ID

                                            join user in this.context.DM_NGUOIDUNG.Where(x => x.IS_ACTIVE != false)
                                            on funcUser.DM_NGUOIDUNG_ID equals user.ID
                                            where func.MA_THAOTAC == functionCode
                                            select user.ID).ToList();

            result.AddRange(rolesHaveFunction);
            result.AddRange(usersHaveFunction);

            result = result.Where(x => sameDeptUserIds.Contains(x)).Distinct().ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách người dùng cùng cấp phòng ban
        /// </summary>
        /// <param name="deptParentId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetListUserByDeptParentId(long deptParentId, long selected = 0)
        {
            List<SelectListItem> result = (from user in this.context.DM_NGUOIDUNG.Where(x => x.IS_ACTIVE != false)

                                           join role in this.context.DM_DANHMUC_DATA
                                           on user.CHUCVU_ID equals role.ID

                                           join department in this.context.CCTC_THANHPHAN.Where(x => x.IS_DELETE != true)
                                           on user.DM_PHONGBAN_ID equals department.ID
                                           where department.PARENT_ID == deptParentId
                                           select new SelectListItem()
                                           {
                                               Value = user.ID.ToString(),
                                               Text = user.HOTEN + " - " + role.TEXT,
                                               Selected = (selected == user.ID)
                                           }).ToList();
            return result;
        }
        public List<SelectListItem> GetDanhSachByListIds(List<long> LstIds)
        {
            if (LstIds != null)
            {
                return this.repository.All().Where(x => x.IS_ACTIVE == true && LstIds.Contains(x.ID)).Select(x =>
                new SelectListItem
                {
                    Text = x.HOTEN,
                    Value = x.ID.ToString()
                }).ToList();
            }
            return new List<SelectListItem>();
        }
        public List<SelectListItem> GetDropDow(List<long> listSelected = null)
        {
            if (listSelected != null && listSelected.Any())
            {
                return this.repository.All().Where(x => x.IS_ACTIVE == true).Select(x => new SelectListItem { Text = x.HOTEN, Value = x.ID.ToString(), Selected = (listSelected.Contains(x.ID)) }).ToList();
            }
            return this.repository.All().Where(x => x.IS_ACTIVE == true).Select(x => new SelectListItem { Text = x.HOTEN, Value = x.ID.ToString() }).ToList();
        }

        public List<SelectListItem> GetByRole(string keyRole, long? removeId = 0)
        {
            var listUserId = (from user in this.context.DM_NGUOIDUNG
                              join userRole in this.context.NGUOIDUNG_VAITRO
                              on user.ID equals userRole.NGUOIDUNG_ID
                              join role in this.context.DM_VAITRO
                              on userRole.VAITRO_ID equals role.DM_VAITRO_ID
                              where role.MA_VAITRO.Equals(keyRole) && user.ID != removeId
                              select new
                              {
                                  user.ID
                              }).Select(x => x.ID).Distinct().ToList();
            if (listUserId == null)
            {
                return new List<SelectListItem>();
            }
            var result = this.repository.All().Where(x => listUserId.Contains(x.ID)).Select(x => new SelectListItem { Text = x.HOTEN, Value = x.ID.ToString() }).ToList();
            return result;
        }

        public string GetName(long? id = 0)
        {
            if (id > 0)
            {
                var find = this.Find(id);
                if (find != null)
                {
                    return find.HOTEN;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách người dùng theo danh sách mã cho trước
        /// </summary>
        /// <param name="selectedIds"></param>
        /// <returns></returns>
        public List<DM_NGUOIDUNG_BO> GetUsersByGroupIds(List<long> selectedIds = null)
        {
            List<DM_NGUOIDUNG_BO> result = new List<DM_NGUOIDUNG_BO>();
            if (selectedIds != null && selectedIds.Any())
            {
                result = (from user in this.context.DM_NGUOIDUNG.Where(x => selectedIds.Contains(x.ID))
                          join dept in this.context.CCTC_THANHPHAN
                          on user.DM_PHONGBAN_ID equals dept.ID
                          into group1
                          from g1 in group1.DefaultIfEmpty()
                          select new DM_NGUOIDUNG_BO()
                          {
                              ID = user.ID,
                              HOTEN = user.HOTEN,
                              DM_PHONGBAN_ID = user.DM_PHONGBAN_ID,
                              TenPhongBan = g1.NAME,
                              DeptType = g1 != null ? g1.TYPE : 0,
                              DeptParentId = g1.PARENT_ID ?? 0
                          }).ToList();
            }
            else
            {
                result = (from user in this.context.DM_NGUOIDUNG
                          join dept in this.context.CCTC_THANHPHAN
                          on user.DM_PHONGBAN_ID equals dept.ID
                          into group1
                          from g1 in group1.DefaultIfEmpty()
                          select new DM_NGUOIDUNG_BO()
                          {
                              ID = user.ID,
                              HOTEN = user.HOTEN,
                              DM_PHONGBAN_ID = user.DM_PHONGBAN_ID,
                              TenPhongBan = g1.NAME,
                              DeptType = g1 != null ? g1.TYPE : 0,
                              DeptParentId = g1.PARENT_ID ?? 0
                          }).ToList();
            }
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách người dùng có thể nhận trực tiếp văn bản
        /// </summary>
        /// <returns></returns>
        public List<DM_NGUOIDUNG_BO> GetUsersReceiveDirectly()
        {
            List<DM_NGUOIDUNG_BO> result = new List<DM_NGUOIDUNG_BO>();
            result = (from user in this.context.DM_NGUOIDUNG
                      join userRole in this.context.NGUOIDUNG_VAITRO
                      on user.ID equals userRole.NGUOIDUNG_ID

                      join role in this.context.DM_VAITRO
                      on userRole.VAITRO_ID equals role.DM_VAITRO_ID

                      join dept in this.context.CCTC_THANHPHAN
                      on user.DM_PHONGBAN_ID equals dept.ID
                      where role.IS_RECEIVE_DOC_DIRECTLY == true
                      select new DM_NGUOIDUNG_BO()
                      {
                          ID = user.ID,
                          HOTEN = user.HOTEN,
                          DM_PHONGBAN_ID = user.DM_PHONGBAN_ID,
                          DM_VAITRO_ID = role.DM_VAITRO_ID,
                          TEN_VAITRO = role.TEN_VAITRO,
                          TenPhongBan = dept.NAME,
                          DeptType = dept.TYPE,
                          DeptParentId = dept.PARENT_ID ?? 0,
                      }).ToList();
            return result;
        }

        /// <summary>
        /// @author:duynn
        /// @description: lấy danh sách người dùng nhận văn bản nội bộ
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<DM_NGUOIDUNG_BO> GetUsersReceiveInternal(UserInfoBO user)
        {
            List<int> deptIds = new List<int>();
            List<CCTC_THANHPHAN> allDeparments = this.context.CCTC_THANHPHAN.Where(x => x.IS_DELETE != true).ToList();
            CCTC_THANHPHAN currentDepartment = this.context.CCTC_THANHPHAN.Find(user.DM_PHONGBAN_ID.GetValueOrDefault());

            //lấy danh sách các đơn vị cùng cấp và cùng loại
            var sameParentsAndCategory = allDeparments.Where(x => x.PARENT_ID == user.DeptParentID.GetValueOrDefault() && x.CATEGORY == currentDepartment.CATEGORY)
                .Select(x => x.ID).ToList();
            deptIds.AddRange(sameParentsAndCategory);

            //lấy danh sách các đơn vị con và cháu của đơn vị hiện tại
            SetHierarchicalIds(currentDepartment, allDeparments, deptIds);

            List<DM_NGUOIDUNG_BO> result = new List<DM_NGUOIDUNG_BO>();
            result = (from us in this.context.DM_NGUOIDUNG
                      join dept in this.context.CCTC_THANHPHAN.Where(x => x.IS_DELETE != true)
                      on us.DM_PHONGBAN_ID equals dept.ID
                      where us.ID != user.ID
                      select new DM_NGUOIDUNG_BO()
                      {
                          ID = us.ID,
                          HOTEN = us.HOTEN,
                          DM_PHONGBAN_ID = dept.ID,
                          TenPhongBan = dept.NAME
                      }).Where(x => deptIds.Contains(x.DM_PHONGBAN_ID.Value)).ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// </summary>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <param name="output"></param>
        /// <param name="times"></param>
        public void SetHierarchicalIds(CCTC_THANHPHAN item, IEnumerable<CCTC_THANHPHAN> list, List<int> output, int times = 0)
        {
            output.Add(item.ID);
            var children = list.Where(x => x.PARENT_ID == item.ID).ToList();
            if (children.Count() > 0)
            {
                foreach (var child in children)
                {
                    SetHierarchicalIds(child, list, output, times);
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách người dùng của phòng ban
        /// </summary>
        /// <returns></returns>
        public List<NguoiDungPhongBanBO> GetUsersOfDepartments(int deptId = 0)
        {
            var queryResult = (from user in this.context.DM_NGUOIDUNG
                               join department in this.context.CCTC_THANHPHAN
                               .Where(x => (deptId > 0 ? (x.ID == deptId) : true))
                               on user.DM_PHONGBAN_ID equals department.ID
                               into jUserDept
                               from jDept in jUserDept
                               group new { user, jDept } by jDept into gDept
                               orderby gDept.Key.NAME
                               select new NguoiDungPhongBanBO
                               {
                                   PhongBan = gDept.Key,
                                   LstNguoiDung = (from nd in gDept.Select(x => x.user)
                                                   join tblChucVu in this.context.DM_DANHMUC_DATA on nd.CHUCVU_ID
                                                   equals tblChucVu.ID into jchucvu
                                                   from chucvu in jchucvu.DefaultIfEmpty()
                                                   orderby nd.HOTEN
                                                   select new DM_NGUOIDUNG_BO
                                                   {
                                                       ID = nd.ID,
                                                       ANH_DAIDIEN = nd.ANH_DAIDIEN,
                                                       DIACHI = nd.DIACHI,
                                                       DIENTHOAI = nd.DIENTHOAI,
                                                       DM_PHONGBAN_ID = nd.DM_PHONGBAN_ID,
                                                       EMAIL = nd.EMAIL,
                                                       HOTEN = nd.HOTEN,
                                                       NGAYSINH = nd.NGAYSINH,
                                                       TENDANGNHAP = nd.TENDANGNHAP,
                                                       ChucVu = chucvu != null ? chucvu.TEXT : "Không có chức vụ"
                                                   }).ToList()
                               }).ToList();

            List<NguoiDungPhongBanBO> result = queryResult.ToList();
            return result;
        }
        public JsonResultBO saveImport(List<DM_NGUOIDUNG> lstObj)
        {
            var result = new JsonResultBO(true);
            using (var transaction = repository.Context.Database.BeginTransaction())
            {
                try
                {
                    repository.Context.DM_NGUOIDUNG.AddRange(lstObj);
                    repository.Context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    result.Status = false;
                    result.Message = "Không import được dữ liệu";
                    transaction.Rollback();
                }
            }
            return result;
        }

        /// <summary>
        /// @author:duynn
        /// @description: tìm kiếm người dùng theo nhóm người nhận văn bản
        /// @since: 11/06/2019
        /// </summary>
        /// <param name="recipientGroupId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<DM_NGUOIDUNG_BO> GetUsersByRecipient(int recipientGroupId, string name)
        {
            List<DM_NGUOIDUNG_BO> result = new List<DM_NGUOIDUNG_BO>();
            QL_NGUOINHAN_VANBAN recipientGroup = this.context.QL_NGUOINHAN_VANBAN.Find(recipientGroupId) ?? new QL_NGUOINHAN_VANBAN();
            if (recipientGroup.NGUOINHAN_IDS != null)
            {
                List<long> groupUserIds = recipientGroup.NGUOINHAN_IDS.ToListLong(',');
                result = (from user in this.context.DM_NGUOIDUNG
                          join dept in this.context.CCTC_THANHPHAN
                          on user.DM_PHONGBAN_ID equals dept.ID
                          into groupDeptUser
                          from gDeptUser in groupDeptUser.DefaultIfEmpty()

                          join position in this.context.DM_DANHMUC_DATA
                          on user.CHUCVU_ID equals position.ID
                          into groupPositionUser
                          from gPositionUser in groupPositionUser.DefaultIfEmpty()

                          select new DM_NGUOIDUNG_BO()
                          {
                              ID = user.ID,
                              HOTEN = user.HOTEN,
                              DM_PHONGBAN_ID = gDeptUser.ID,
                              TenPhongBan = gDeptUser.NAME,
                              ChucVu = gPositionUser.TEXT,
                          })
                          .Where(x => groupUserIds.Contains(x.ID))
                          .ToList();
                if (!string.IsNullOrEmpty(name))
                {
                    name = name.Trim().ToLower();
                    result = result.Where(x => x.HOTEN != null && x.HOTEN.Trim().ToLower().Contains(name)).ToList();
                }
            }
            return result;
        }

        /// <summary>
        /// @author:duynn
        /// @description: người dùng có vai trò cao nhất trong phòng ban
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public UserInfoBO GetUserHighestPriority(int deptId)
        {
            var result = (from user in this.context.DM_NGUOIDUNG
                          join dept in this.context.CCTC_THANHPHAN.Where(x => x.ID == deptId)
                          on user.DM_PHONGBAN_ID equals dept.ID
                          join userRole in this.context.NGUOIDUNG_VAITRO
                          on user.ID equals userRole.NGUOIDUNG_ID
                          into groupUserRole
                          from gUserRole in groupUserRole.DefaultIfEmpty()
                          join role in this.context.DM_VAITRO
                          on gUserRole.VAITRO_ID equals role.DM_VAITRO_ID
                          into groupRole
                          from gRole in groupRole.DefaultIfEmpty()

                          orderby gRole.TRONGSO descending, gRole.TEN_VAITRO descending,
                          user.HOTEN descending
                          select new UserInfoBO()
                          {
                              ID = user.ID,
                              HOTEN = user.HOTEN,
                              EMAIL = user.EMAIL,
                              DIENTHOAI = user.DIENTHOAI
                          }).FirstOrDefault() ?? new UserInfoBO();
            return result;
        }

        /// <summary>
        /// @author:duynn
        /// @description: danh sách người dùng trong phòng ban
        /// @since: 14/06/2019
        /// </summary>
        /// <returns></returns>
        public PageListResultBO<DM_NGUOIDUNG_BO> GetUsersInDept(DM_NGUOIDUNG_SEARCHBO searchModel, int deptId, int pageIndex = 1, int pageSize = 5)
        {
            var query = (from user in this.context.DM_NGUOIDUNG
                         .Where(x => x.DM_PHONGBAN_ID == deptId)
                         join userRole in this.context.NGUOIDUNG_VAITRO
                         on user.ID equals userRole.NGUOIDUNG_ID
                         join role in this.context.DM_VAITRO
                         on userRole.VAITRO_ID equals role.DM_VAITRO_ID
                         into groupUserRole
                         join position in this.context.DM_DANHMUC_DATA
                         on user.CHUCVU_ID equals position.ID
                         into groupUserPosition
                         from gUserPosition in groupUserPosition.DefaultIfEmpty()
                         select new DM_NGUOIDUNG_BO()
                         {
                             ID = user.ID,
                             HOTEN = user.HOTEN,
                             EMAIL = user.EMAIL,
                             DIENTHOAI = user.DIENTHOAI,
                             ChucVu = gUserPosition.TEXT,
                             CHUCVU_ID = user.CHUCVU_ID,
                             LstVaiTro = groupUserRole
                             .Select(x => x.TEN_VAITRO).ToList(),
                         });
            if (searchModel != null)
            {
                if (searchModel.sea_CHUCVU_ID.HasValue)
                {
                    query = query.Where(x => x.ChucVu_Id == searchModel.sea_CHUCVU_ID);
                }

                if (!string.IsNullOrEmpty(searchModel.sea_HoTen))
                {
                    searchModel.sea_HoTen = searchModel.sea_HoTen.Trim().ToLower();
                    query = query.Where(x => x.HOTEN != null && x.HOTEN.Trim().ToLower().Contains(searchModel.sea_HoTen));
                }
                if (!string.IsNullOrEmpty(searchModel.sea_Email))
                {
                    searchModel.sea_Email = searchModel.sea_Email.Trim().ToLower();
                    query = query.Where(x => x.EMAIL != null && x.EMAIL.Trim().ToLower().Contains(searchModel.sea_Email));
                }

                if (!string.IsNullOrEmpty(searchModel.sea_DienThoai))
                {
                    searchModel.sea_DienThoai = searchModel.sea_DienThoai.Trim().ToLower();
                    query = query.Where(x => x.DIENTHOAI != null && x.DIENTHOAI.Contains(searchModel.sea_DienThoai));
                }

                if (!string.IsNullOrEmpty(searchModel.sea_TenDangNhap))
                {
                    searchModel.sea_TenDangNhap = searchModel.sea_TenDangNhap.Trim().ToLower();
                    query = query.Where(x => x.TENDANGNHAP != null && x.TENDANGNHAP.Contains(searchModel.sea_TenDangNhap));
                }

                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.ID);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.ID);
            }
            var result = new PageListResultBO<DM_NGUOIDUNG_BO>();
            if (pageSize == -1)
            {
                var pagedList = query.ToList();
                result.Count = pagedList.Count;
                result.TotalPage = 1;
                result.ListItem = pagedList;
            }
            else
            {
                var pagedList = query.ToPagedList(pageIndex, pageSize);
                result.Count = pagedList.TotalItemCount;
                result.TotalPage = pagedList.PageCount;
                result.ListItem = pagedList.ToList();
            }
            return result;
        }
    }
}
