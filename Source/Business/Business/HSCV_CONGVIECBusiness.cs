using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.DMNguoiDung;
using Business.CommonModel.HSCVCONGVIEC;
using Model.Entities;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;

namespace Business.Business
{
    public class HSCV_CONGVIECBusiness : BaseBusiness<HSCV_CONGVIEC>
    {
        public HSCV_CONGVIECBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public void Save(HSCV_CONGVIEC obj)
        {
            try
            {
                if (obj.ID == 0)
                {
                    if (obj.CONGVIECGOC_ID.HasValue && obj.CONGVIECGOC_ID.Value > 0)
                    {
                        var Parent = this.Find(obj.CONGVIECGOC_ID);
                        if (Parent != null)
                        {
                            obj.CONGVIEC_LIENQUAN_ID = string.IsNullOrEmpty(Parent.CONGVIEC_LIENQUAN_ID) ? Parent.ID.ToString() : (Parent.CONGVIEC_LIENQUAN_ID + "," + Parent.ID);
                        }
                    }
                    this.repository.Insert(obj);
                }
                else
                {
                    this.repository.Update(obj);
                }
                this.repository.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public PageListResultBO<CongViecBO> GetDaTaByPage(HSCV_CONGVIEC_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var query = from congviec in this.context.HSCV_CONGVIEC
                        join dokhan in this.context.DM_DANHMUC_DATA
                        on congviec.DOKHAN equals dokhan.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()

                        join douutien in this.context.DM_DANHMUC_DATA
                        on congviec.DOUU_TIEN equals douutien.ID
                        into group2
                        from g2 in group2.DefaultIfEmpty()

                        join xlchinh in this.context.DM_NGUOIDUNG
                        on congviec.NGUOIXULYCHINH_ID equals xlchinh.ID
                        into group3
                        from g3 in group3.DefaultIfEmpty()

                        join giaoviec in this.context.DM_NGUOIDUNG
                        on congviec.NGUOIGIAOVIEC_ID equals giaoviec.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()
                        join subtask in this.context.HSCV_CONGVIEC
                        on congviec.ID equals subtask.CONGVIECGOC_ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()

                        select new CongViecBO
                        {
                            CONGVIECGOC_ID = congviec.CONGVIECGOC_ID,
                            DATUDANHGIA = congviec.DATUDANHGIA,
                            DayDiff = 0,
                            DOKHAN = congviec.DOKHAN,
                            DOUU_TIEN = congviec.DOUU_TIEN,
                            HAS_FILE = congviec.HAS_FILE,
                            HAS_NHACVIECDENHAN = congviec.HAS_NHACVIECDENHAN,
                            ID = congviec.ID,
                            IS_EMAIL = congviec.IS_EMAIL,
                            IS_HASPLAN = congviec.IS_HASPLAN,
                            IS_MESG = congviec.IS_MESG,
                            IS_MYJOB = congviec.IS_MYJOB,
                            IS_POPUP = congviec.IS_POPUP,
                            IS_SMS = congviec.IS_SMS,
                            IS_SUBTASK = congviec.IS_SUBTASK,
                            ITEMTYPE = congviec.ITEMTYPE,
                            ITEM_ID = congviec.ITEM_ID,
                            NGAY_NHANVIEC = congviec.NGAY_NHANVIEC,
                            NGAYHOANTHANH_THEOMONGMUON = congviec.NGAYHOANTHANH_THEOMONGMUON,
                            NGAYSUA = congviec.NGAYSUA,
                            NGAYTAO = congviec.NGAYTAO,
                            NGUOIGIAOVIECDANHGIA = congviec.NGUOIGIAOVIECDANHGIA,
                            NGUOIGIAOVIECDAPHANHOI = congviec.NGUOIGIAOVIECDAPHANHOI,
                            NGUOIGIAOVIEC_ID = congviec.NGUOIGIAOVIEC_ID,
                            NGUOITAO = congviec.NGUOITAO,
                            NGUOIXULYCHINH_ID = congviec.NGUOIXULYCHINH_ID,
                            NOIDUNGCONGVIEC = congviec.NOIDUNGCONGVIEC,
                            PHANTRAMHOANTHANH = congviec.PHANTRAMHOANTHANH,
                            PHANTRAMHOANTHANH_OLD = congviec.PHANTRAMHOANTHANH_OLD,
                            SONGAYNHACTRUOCHAN = congviec.SONGAYNHACTRUOCHAN,
                            SUBTASK_ID = congviec.SUBTASK_ID,
                            TENCONGVIEC = congviec.TENCONGVIEC,
                            TRANGTHAI_ID = congviec.TRANGTHAI_ID,
                            TEN_DOKHAN = g1.TEXT,
                            TEN_DOUUTIEN = g2.TEXT,
                            ICON_DOKHAN = g1.ICON,
                            ICON_DOUUTIEN = g2.ICON,
                            TEN_NGUOIGIAOVIEC = g4.HOTEN,
                            TEN_NGUOIXULYCHINH = g3.HOTEN,
                            IS_BATDAU = (true == congviec.IS_BATDAU),
                            HasChild = (g5 != null),
                            SONGAYCONLAI = (true == congviec.IS_HASPLAN && congviec.NGAYKETTHUC_KEHOACH.HasValue) ? EntityFunctions.DiffDays(DateTime.Now, congviec.NGAYKETTHUC_KEHOACH) : EntityFunctions.DiffDays(DateTime.Now, congviec.NGAYHOANTHANH_THEOMONGMUON)
                        };
            if (searchModel != null)
            {
                #region Tim kiem
                switch (searchModel.LOAI_CONGVIEC)
                {
                    //Đối với công việc cá nhân
                    case LOAI_CONGVIEC.CANHAN:
                        //query = query.Where(x => searchModel.USER_ID == x.NGUOITAO || searchModel.USER_ID == x.NGUOIGIAOVIEC_ID);
                        query = query.Where(x => searchModel.USER_ID == x.NGUOIGIAOVIEC_ID);
                        break;
                    //Đối với công việc được giao
                    case LOAI_CONGVIEC.DUOCGIAO:
                        //query = query.Where(x => searchModel.USER_ID == x.NGUOIXULYCHINH_ID && true != x.IS_MYJOB);
                        query = query.Where(x => searchModel.USER_ID == x.NGUOIXULYCHINH_ID);
                        break;
                    //Đối với công việc được phối hợp xử lý
                    case LOAI_CONGVIEC.PHOIHOP_XULY:
                        var Ids = (from phoihop in this.context.HSCV_CONGVIEC_NGUOITHAMGIAXULY
                                   where searchModel.USER_ID == phoihop.USER_ID
                                   select phoihop.CONGVIEC_ID).ToList();
                        query = query.Where(x => Ids.Contains(x.ID));
                        break;
                }
                if (searchModel.DOKHAN.HasValue)
                {
                    query = query.Where(x => searchModel.DOKHAN.Value == x.DOKHAN);
                }
                if (searchModel.DO_UUTIEN.HasValue)
                {
                    query = query.Where(x => searchModel.DO_UUTIEN.Value == x.DOUU_TIEN);
                }
                if (searchModel.IS_HASPLAN.HasValue)
                {
                    query = query.Where(x => searchModel.IS_HASPLAN.Value == x.IS_HASPLAN);
                }
                if (searchModel.NGAYBATDAU_FROM.HasValue)
                {
                    query = query.Where(x => searchModel.NGAYBATDAU_FROM.Value <= x.NGAY_NHANVIEC);
                }
                if (searchModel.NGAYBATDAU_TO.HasValue)
                {
                    query = query.Where(x => searchModel.NGAYBATDAU_TO.Value >= x.NGAY_NHANVIEC);
                }
                if (searchModel.NGAYKETTHUC_FROM.HasValue)
                {
                    query = query.Where(x => searchModel.NGAYKETTHUC_FROM.Value <= x.NGAYHOANTHANH_THEOMONGMUON);
                }
                if (searchModel.NGAYKETTHUC_TO.HasValue)
                {
                    query = query.Where(x => searchModel.NGAYKETTHUC_TO.Value >= x.NGAYHOANTHANH_THEOMONGMUON);
                }
                if (searchModel.NOTIF_TYPE.HasValue)
                {
                }
                if (!string.IsNullOrEmpty(searchModel.TENCONGVIEC))
                {
                    query = query.Where(x => x.TENCONGVIEC.ToLower().Contains(searchModel.TENCONGVIEC.ToLower()));
                }
                #endregion
                query = query.GroupBy(x => x.ID).Select(y => y.FirstOrDefault());
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.NGAYTAO);
                }
            }
            else
            {
                query = query.GroupBy(x => x.ID).Select(y => y.FirstOrDefault());
                query = query.OrderByDescending(x => x.NGAYTAO);
            }

            var resultmodel = new PageListResultBO<CongViecBO>();
            var dataPageList = query.ToPagedList(pageIndex, pageSize);
            resultmodel.Count = dataPageList.TotalItemCount;
            resultmodel.TotalPage = dataPageList.PageCount;
            resultmodel.ListItem = dataPageList.ToList();
            return resultmodel;
        }

        public PageListResultBO<CongViecBO> GetListProcessedJob(HSCV_CONGVIEC_SEARCH searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var query = from congviec in this.context.HSCV_CONGVIEC
                        join dokhan in this.context.DM_DANHMUC_DATA
                        on congviec.DOKHAN equals dokhan.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()

                        join douutien in this.context.DM_DANHMUC_DATA
                        on congviec.DOUU_TIEN equals douutien.ID
                        into group2
                        from g2 in group2.DefaultIfEmpty()

                        join xlchinh in this.context.DM_NGUOIDUNG
                        on congviec.NGUOIXULYCHINH_ID equals xlchinh.ID
                        into group3
                        from g3 in group3.DefaultIfEmpty()

                        join giaoviec in this.context.DM_NGUOIDUNG
                        on congviec.NGUOIGIAOVIEC_ID equals giaoviec.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()

                        select new CongViecBO
                        {
                            CONGVIECGOC_ID = congviec.CONGVIECGOC_ID,
                            DATUDANHGIA = congviec.DATUDANHGIA,
                            DayDiff = 0,
                            DOKHAN = congviec.DOKHAN,
                            DOUU_TIEN = congviec.DOUU_TIEN,
                            HAS_FILE = congviec.HAS_FILE,
                            HAS_NHACVIECDENHAN = congviec.HAS_NHACVIECDENHAN,
                            ID = congviec.ID,
                            IS_EMAIL = congviec.IS_EMAIL,
                            IS_HASPLAN = congviec.IS_HASPLAN,
                            IS_MESG = congviec.IS_MESG,
                            IS_MYJOB = congviec.IS_MYJOB,
                            IS_POPUP = congviec.IS_POPUP,
                            IS_SMS = congviec.IS_SMS,
                            IS_SUBTASK = congviec.IS_SUBTASK,
                            ITEMTYPE = congviec.ITEMTYPE,
                            ITEM_ID = congviec.ITEM_ID,
                            NGAY_NHANVIEC = congviec.NGAY_NHANVIEC,
                            NGAYHOANTHANH_THEOMONGMUON = congviec.NGAYHOANTHANH_THEOMONGMUON,
                            NGAYSUA = congviec.NGAYSUA,
                            NGAYTAO = congviec.NGAYTAO,
                            NGUOIGIAOVIECDANHGIA = congviec.NGUOIGIAOVIECDANHGIA,
                            NGUOIGIAOVIECDAPHANHOI = congviec.NGUOIGIAOVIECDAPHANHOI,
                            NGUOIGIAOVIEC_ID = congviec.NGUOIGIAOVIEC_ID,
                            NGUOITAO = congviec.NGUOITAO,
                            NGUOIXULYCHINH_ID = congviec.NGUOIXULYCHINH_ID,
                            NOIDUNGCONGVIEC = congviec.NOIDUNGCONGVIEC,
                            PHANTRAMHOANTHANH = congviec.PHANTRAMHOANTHANH,
                            PHANTRAMHOANTHANH_OLD = congviec.PHANTRAMHOANTHANH_OLD,
                            SONGAYNHACTRUOCHAN = congviec.SONGAYNHACTRUOCHAN,
                            SUBTASK_ID = congviec.SUBTASK_ID,
                            TENCONGVIEC = congviec.TENCONGVIEC,
                            TRANGTHAI_ID = congviec.TRANGTHAI_ID,
                            TEN_DOKHAN = g1.TEXT,
                            TEN_DOUUTIEN = g2.TEXT,
                            ICON_DOKHAN = g1.ICON,
                            ICON_DOUUTIEN = g2.ICON,
                            TEN_NGUOIGIAOVIEC = g4.HOTEN,
                            TEN_NGUOIXULYCHINH = g3.HOTEN
                        };
            if (searchModel != null)
            {
                query = query.Where(x => searchModel.USER_ID == x.NGUOIGIAOVIEC_ID && x.TRANGTHAI_ID == 1);
                if (searchModel.DOKHAN.HasValue)
                {
                    query = query.Where(x => searchModel.DOKHAN.Value == x.DOKHAN);
                }
                if (searchModel.DO_UUTIEN.HasValue)
                {
                    query = query.Where(x => searchModel.DO_UUTIEN.Value == x.DOUU_TIEN);
                }
                if (searchModel.IS_HASPLAN.HasValue)
                {
                    query = query.Where(x => searchModel.IS_HASPLAN.Value == x.IS_HASPLAN);
                }
                if (searchModel.NGAYBATDAU_FROM.HasValue)
                {
                    query = query.Where(x => searchModel.NGAYBATDAU_FROM.Value >= x.NGAY_NHANVIEC);
                }
                if (searchModel.NGAYBATDAU_TO.HasValue)
                {
                    query = query.Where(x => searchModel.NGAYBATDAU_TO.Value <= x.NGAY_NHANVIEC);
                }
                if (searchModel.NGAYKETTHUC_FROM.HasValue)
                {
                    query = query.Where(x => searchModel.NGAYKETTHUC_FROM.Value >= x.NGAYHOANTHANH_THEOMONGMUON);
                }
                if (searchModel.NGAYKETTHUC_TO.HasValue)
                {
                    query = query.Where(x => searchModel.NGAYKETTHUC_TO.Value <= x.NGAYHOANTHANH_THEOMONGMUON);
                }
                if (searchModel.NOTIF_TYPE.HasValue)
                {
                }
                if (!string.IsNullOrEmpty(searchModel.TENCONGVIEC))
                {
                    query = query.Where(x => x.TENCONGVIEC.ToLower().Contains(searchModel.TENCONGVIEC.ToLower()));
                }
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderBy(x => x.TRANGTHAI_ID);
                }
            }
            else
            {
                query = query.OrderBy(x => x.TRANGTHAI_ID);
            }
            var resultmodel = new PageListResultBO<CongViecBO>();
            var dataPageList = query.ToPagedList(pageIndex, pageSize);
            resultmodel.Count = dataPageList.TotalItemCount;
            resultmodel.TotalPage = dataPageList.PageCount;
            resultmodel.ListItem = dataPageList.ToList();
            return resultmodel;
        }
        /// <summary>
        /// Danh sách công việc sắp đến hạn
        /// </summary>
        /// <returns></returns>
        public List<CongViecEmailBO> GetJobComingSoon()
        {
            var now = DateTime.Now;
            var curentDate = now.Date;
            var result = from job in context.HSCV_CONGVIEC
                         join user in context.DM_NGUOIDUNG
                         on (job.NGUOIXULYCHINH_ID != null ? job.NGUOIXULYCHINH_ID : job.NGUOIGIAOVIEC_ID) equals user.ID
                         where 100 != job.PHANTRAMHOANTHANH
                           && (
                           ((true == job.IS_HASPLAN && job.NGAYKETTHUC_KEHOACH.HasValue) ? (curentDate <= job.NGAYKETTHUC_KEHOACH &&
                           DbFunctions.AddDays(now, job.SONGAYNHACTRUOCHAN.HasValue ? job.SONGAYNHACTRUOCHAN.Value : 2) >= job.NGAYKETTHUC_KEHOACH)
                           :
                           (curentDate <= job.NGAYHOANTHANH_THEOMONGMUON && DbFunctions.AddDays(now, job.SONGAYNHACTRUOCHAN.HasValue ? job.SONGAYNHACTRUOCHAN.Value : 2) >= job.NGAYHOANTHANH_THEOMONGMUON)
                           )
                           )
                         group new { job, user } by user.ID into gNdung
                         select new CongViecEmailBO
                         {
                             title = "Danh các công việc sắp hết hạn xử lý",
                             subtitle = "Vui lòng xử lý công việc đúng hạn",
                             email = gNdung.FirstOrDefault().user.EMAIL,
                             userName = gNdung.FirstOrDefault().user.HOTEN,
                             ListCongViec = gNdung.Select(x => x.job).ToList()
                         };
            return result.ToList();
        }
        public List<HSCV_CONGVIEC> GetDataByUserId(long userId, DateTime? FROM, DateTime? TO)
        {
            var result = from job in this.context.HSCV_CONGVIEC.AsNoTracking()
                         where userId == job.NGUOIXULYCHINH_ID
                         //&& job.XEPLOAICONGVIEC.HasValue
                         && FROM.HasValue ? FROM <= job.NGAY_NHANVIEC : true
                         && TO.HasValue ? TO >= job.NGAY_NHANVIEC : true
                         select job;
            return result.ToList();
        }
        public PageListResultBO<CongViecBO> GetDataByUserId(long userId, int pageSize, int pageIndex)
        {
            var query = from job in this.context.HSCV_CONGVIEC
                        join domat in this.context.DM_DANHMUC_DATA
                        on job.DOKHAN equals domat.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()

                        join douutien in this.context.DM_DANHMUC_DATA
                            on job.DOUU_TIEN equals douutien.ID
                            into group2
                        from g2 in group2.DefaultIfEmpty()

                        join xlchinh in this.context.DM_NGUOIDUNG
                        on job.NGUOIXULYCHINH_ID equals xlchinh.ID
                        into group3
                        from g3 in group3.DefaultIfEmpty()

                        join giaoviec in this.context.DM_NGUOIDUNG
                        on job.NGUOIGIAOVIEC_ID equals giaoviec.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()
                        where userId == job.NGUOIXULYCHINH_ID
                        && 100 != job.PHANTRAMHOANTHANH
                        orderby job.NGAYTAO descending
                        select new CongViecBO
                        {
                            CACBUOC_THUCHIEN = job.CACBUOC_THUCHIEN,
                            DAGIAOVIEC = job.DAGIAOVIEC,
                            CONGVIECGOC_ID = job.CONGVIECGOC_ID,
                            DATUDANHGIA = job.DATUDANHGIA,
                            DayDiff = 0,
                            DOKHAN = job.DOKHAN,
                            DOUU_TIEN = job.DOUU_TIEN,
                            HAS_FILE = job.HAS_FILE,
                            HAS_NHACVIECDENHAN = job.HAS_NHACVIECDENHAN,
                            ICON_DOKHAN = g1.ICON,
                            ICON_DOUUTIEN = "",
                            XEPLOAICONGVIEC = job.XEPLOAICONGVIEC,
                            ID = job.ID,
                            IS_ASSIGNED = job.IS_ASSIGNED,
                            IS_BATDAU = job.IS_BATDAU,
                            IS_EMAIL = job.IS_EMAIL,
                            IS_EXTEND_TASK = job.IS_EXTEND_TASK,
                            IS_HASPLAN = job.IS_HASPLAN,
                            IS_MESG = job.IS_MESG,
                            IS_MYJOB = job.IS_MYJOB,
                            IS_POPUP = job.IS_POPUP,
                            IS_SMS = job.IS_SMS,
                            IS_SUBTASK = job.IS_SUBTASK,
                            ITEMTYPE = job.ITEMTYPE,
                            ITEM_ID = job.ITEM_ID,
                            MUCTIEU_CONGVIEC = job.MUCTIEU_CONGVIEC,
                            NGAYBATDAU_KEHOACH = job.NGAYBATDAU_KEHOACH,
                            NGAYBATDAU_THUCTE = job.NGAYBATDAU_THUCTE,
                            NGAYDUYET = job.NGAYDUYET,
                            NGAYHOANTHANH_THEOMONGMUON = job.NGAYHOANTHANH_THEOMONGMUON,
                            NGAYKETTHUC_KEHOACH = job.NGAYKETTHUC_KEHOACH,
                            NGAYKETTHUC_THUCTE = job.NGAYKETTHUC_THUCTE,
                            NGAYSUA = job.NGAYSUA,
                            NGAYTAO = job.NGAYTAO,
                            NGAY_NHANVIEC = job.NGAY_NHANVIEC,
                            NGUOIGIAOVIECDANHGIA = job.NGUOIGIAOVIECDANHGIA,
                            NGUOIGIAOVIECDAPHANHOI = job.NGUOIGIAOVIECDAPHANHOI,
                            NGUOIGIAOVIEC_ID = job.NGUOIGIAOVIEC_ID,
                            NGUOIGIAOVIEC_PHANHOI = job.NGUOIGIAOVIEC_PHANHOI,
                            NGUOITAO = job.NGUOITAO,
                            NGUOIXULYCHINH_ID = job.NGUOIXULYCHINH_ID,
                            NOIDUNGCONGVIEC = job.NOIDUNGCONGVIEC,
                            PHANTRAMHOANTHANH = job.PHANTRAMHOANTHANH,
                            PHANTRAMHOANTHANH_OLD = job.PHANTRAMHOANTHANH_OLD,
                            SONGAYNHACTRUOCHAN = job.SONGAYNHACTRUOCHAN,
                            SUBTASK_ID = job.SUBTASK_ID,
                            TENCONGVIEC = job.TENCONGVIEC,
                            TEN_DOKHAN = g1.TEXT,
                            TEN_DOUUTIEN = g2.TEXT,
                            TEN_NGUOIGIAOVIEC = g4.HOTEN,
                            TEN_NGUOIXULYCHINH = g3.HOTEN,
                            TRANGTHAI_ID = job.TRANGTHAI_ID,
                            COLOR = g1.COLOR,
                        };
            var resultmodel = new PageListResultBO<CongViecBO>();
            var dataPageList = query.ToPagedList(pageIndex, pageSize);
            resultmodel.Count = dataPageList.TotalItemCount;
            resultmodel.TotalPage = dataPageList.PageCount;
            resultmodel.ListItem = dataPageList.ToList();
            return resultmodel;
        }

        public List<CongViecBO> GetDsCongViec(List<int> DeptId, List<long> ListNguoiDung, DateTime NGAYGIAOVIEC_FROM, DateTime NGAYGIAOVIEC_TO)
        {
            var item = (from cv in this.context.HSCV_CONGVIEC
                        join nxl in this.context.DM_NGUOIDUNG
                        on cv.NGUOIXULYCHINH_ID.Value equals nxl.ID
                             into group4
                        from g4 in group4.DefaultIfEmpty()

                        where
                        //cv.IS_SUBTASK == null && 
                        (DeptId.Any() ? (cv.NGUOIXULYCHINH_ID.HasValue && ListNguoiDung.Contains(cv.NGUOIXULYCHINH_ID.Value)) : true)
                        select new CongViecBO
                        {
                            ID = cv.ID,
                            SUBTASK_ID = cv.SUBTASK_ID,
                            TENCONGVIEC = cv.TENCONGVIEC,
                            PHANTRAMHOANTHANH = cv.PHANTRAMHOANTHANH,
                            NGAY_NHANVIEC = cv.NGAY_NHANVIEC,
                            NGAYHOANTHANH_THEOMONGMUON = cv.NGAYHOANTHANH_THEOMONGMUON,
                            NGAYBATDAU_KEHOACH = cv.NGAYBATDAU_KEHOACH,
                            NGAYKETTHUC_KEHOACH = cv.NGAYKETTHUC_KEHOACH,
                            NGAYBATDAU_THUCTE = cv.NGAYBATDAU_THUCTE,
                            NGAYKETTHUC_THUCTE = cv.NGAYKETTHUC_THUCTE,
                            TEN_NGUOIXULYCHINH = g4.HOTEN,
                            DeptId = g4.DM_PHONGBAN_ID.Value,
                            IS_SUBTASK = cv.IS_SUBTASK,
                            NOIDUNGCONGVIEC = cv.NOIDUNGCONGVIEC,
                            CACBUOC_THUCHIEN = cv.CACBUOC_THUCHIEN,
                            CONGVIECGOC_ID = cv.CONGVIECGOC_ID,
                            DAGIAOVIEC = cv.DAGIAOVIEC,
                            DATUDANHGIA = cv.DATUDANHGIA,
                            IS_ASSIGNED = cv.IS_ASSIGNED,
                            IS_BATDAU = cv.IS_BATDAU,
                            IS_HASPLAN = cv.IS_HASPLAN,
                            IS_EXTEND_TASK = cv.IS_EXTEND_TASK,
                            MUCTIEU_CONGVIEC = cv.MUCTIEU_CONGVIEC,
                            NGAYDUYET = cv.NGAYDUYET,
                            XEPLOAICONGVIEC = cv.XEPLOAICONGVIEC
                        });
            item = item.Where(x => (NGAYGIAOVIEC_FROM <= x.NGAY_NHANVIEC && x.NGAY_NHANVIEC <= NGAYGIAOVIEC_TO) || (NGAYGIAOVIEC_FROM <= x.NGAYBATDAU_KEHOACH && x.NGAYBATDAU_KEHOACH <= NGAYGIAOVIEC_TO));
            var listItem = item.OrderByDescending(x => x.ID).ToList();
            //var queue = new Queue();
            //foreach (var task in listItem)
            //{
            //    queue.Enqueue(task.ID);
            //}
            //while (queue.Count > 0)
            //{
            //    var cv_id = (long)queue.Dequeue();
            //    var subitem = (from cv in this.context.HSCV_CONGVIEC
            //                   join nxl in this.context.DM_NGUOIDUNG
            //                   on cv.NGUOIXULYCHINH_ID.Value equals nxl.ID
            //                   into group4
            //                   from g4 in group4.DefaultIfEmpty()
            //                   select new CongViecBO
            //                   {
            //                       ID = cv.ID,
            //                       SUBTASK_ID = cv.SUBTASK_ID,
            //                       TENCONGVIEC = cv.TENCONGVIEC,
            //                       PHANTRAMHOANTHANH = cv.PHANTRAMHOANTHANH,
            //                       NGAY_NHANVIEC = cv.NGAY_NHANVIEC,
            //                       NGAYHOANTHANH_THEOMONGMUON = cv.NGAYHOANTHANH_THEOMONGMUON,
            //                       NGAYBATDAU_KEHOACH = cv.NGAYBATDAU_KEHOACH,
            //                       NGAYKETTHUC_KEHOACH = cv.NGAYKETTHUC_KEHOACH,
            //                       NGAYBATDAU_THUCTE = cv.NGAYBATDAU_THUCTE,
            //                       NGAYKETTHUC_THUCTE = cv.NGAYKETTHUC_THUCTE,
            //                       TEN_NGUOIXULYCHINH = g4.HOTEN,
            //                       DeptId = g4.DM_PHONGBAN_ID.Value,
            //                       IS_SUBTASK = cv.IS_SUBTASK,
            //                       NOIDUNGCONGVIEC = cv.NOIDUNGCONGVIEC,
            //                       CONGVIECGOC_ID = cv.CONGVIECGOC_ID,
            //                   }).Where(x => x.CONGVIECGOC_ID == cv_id).ToList();
            //    if (subitem.Count > 0)
            //    {
            //        listItem.AddRange(subitem);
            //        foreach (var cc in subitem)
            //        {
            //            queue.Enqueue(cc.ID);
            //        }
            //    }
            //}
            listItem = listItem.Distinct().ToList();
            return listItem;

        }
        public List<CongViecBO> GetDSChild(long id)
        {
            var listItem = new List<CongViecBO>();
            var queue = new Queue();
            queue.Enqueue(id);
            while (queue.Count > 0)
            {
                var cv_id = (long)queue.Dequeue();
                //var subtask = this.context.HSCV_SUBTASK.Where(x => x.CONGVIEC_ID == cv_id).Select(x => x.ID).ToList();
                //if (subtask.Count > 0)
                //{
                var item = (from cv in this.context.HSCV_CONGVIEC
                                //join subcv in this.context.HSCV_SUBTASK on cv.SUBTASK_ID.Value equals subcv.ID
                            join nxl in this.context.DM_NGUOIDUNG
                                on cv.NGUOIXULYCHINH_ID.Value equals nxl.ID
                                into group4
                            from g4 in group4.DefaultIfEmpty()
                            select new CongViecBO
                            {
                                ID = cv.ID,
                                SUBTASK_ID = cv.SUBTASK_ID,
                                TENCONGVIEC = cv.TENCONGVIEC,
                                PHANTRAMHOANTHANH = cv.PHANTRAMHOANTHANH,
                                NGAYHOANTHANH_THEOMONGMUON = cv.NGAYHOANTHANH_THEOMONGMUON,
                                NGAYKETTHUC_THUCTE = cv.NGAYKETTHUC_THUCTE,
                                TEN_NGUOIXULYCHINH = g4.HOTEN,
                                ParentId = cv.CONGVIECGOC_ID.Value
                            }).Where(x => x.ParentId == cv_id).ToList();
                if (item.Count > 0)
                {
                    listItem.AddRange(item);
                    foreach (var cc in item)
                    {
                        queue.Enqueue(cc.ID);
                    }
                }
                //}
            }

            return listItem;
        }

        /// <summary>
        /// @description: lấy danh sách người liên quan đến công việc
        /// </summary>
        /// <param name="user"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public List<long> GetEmployeesInvolveToTask(UserInfoBO user, HSCV_CONGVIEC task)
        {
            //HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            //HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness = Get<HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness>();
            List<long> result = new List<long>();

            if (user.ID != task.NGUOIGIAOVIEC_ID)
            {
                result.Add(task.NGUOIGIAOVIEC_ID ?? 0);
            }

            if (user.ID != task.NGUOIXULYCHINH_ID)
            {
                result.Add(task.NGUOIXULYCHINH_ID ?? 0);
            }

            //danh sách người tham gia xử ly
            List<long> joinResult = this.context.HSCV_CONGVIEC_NGUOITHAMGIAXULY.Where(x => x.CONGVIEC_ID == task.ID && x.USER_ID != user.ID)
                .Select(x => x.USER_ID.Value).ToList();
            result.AddRange(joinResult);
            return result;
        }

        public List<CongViecBO> GetDsCongViecCon(long id, int LoaiCongViec, long UserId)
        {
            var query = from congviec in this.context.HSCV_CONGVIEC
                        join dokhan in this.context.DM_DANHMUC_DATA
                        on congviec.DOKHAN equals dokhan.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()

                        join douutien in this.context.DM_DANHMUC_DATA
                        on congviec.DOUU_TIEN equals douutien.ID
                        into group2
                        from g2 in group2.DefaultIfEmpty()

                        join xlchinh in this.context.DM_NGUOIDUNG
                        on congviec.NGUOIXULYCHINH_ID equals xlchinh.ID
                        into group3
                        from g3 in group3.DefaultIfEmpty()

                        join giaoviec in this.context.DM_NGUOIDUNG
                        on congviec.NGUOIGIAOVIEC_ID equals giaoviec.ID
                        into group4
                        from g4 in group4.DefaultIfEmpty()
                        join subtask in this.context.HSCV_CONGVIEC
                        on congviec.ID equals subtask.CONGVIECGOC_ID
                        into group5
                        from g5 in group5.DefaultIfEmpty()

                        where id == congviec.CONGVIECGOC_ID
                        select new CongViecBO
                        {
                            CONGVIECGOC_ID = congviec.CONGVIECGOC_ID,
                            DATUDANHGIA = congviec.DATUDANHGIA,
                            DayDiff = 0,
                            DOKHAN = congviec.DOKHAN,
                            DOUU_TIEN = congviec.DOUU_TIEN,
                            HAS_FILE = congviec.HAS_FILE,
                            HAS_NHACVIECDENHAN = congviec.HAS_NHACVIECDENHAN,
                            ID = congviec.ID,
                            IS_EMAIL = congviec.IS_EMAIL,
                            IS_HASPLAN = congviec.IS_HASPLAN,
                            IS_MESG = congviec.IS_MESG,
                            IS_MYJOB = congviec.IS_MYJOB,
                            IS_POPUP = congviec.IS_POPUP,
                            IS_SMS = congviec.IS_SMS,
                            IS_SUBTASK = congviec.IS_SUBTASK,
                            ITEMTYPE = congviec.ITEMTYPE,
                            ITEM_ID = congviec.ITEM_ID,
                            NGAY_NHANVIEC = congviec.NGAY_NHANVIEC,
                            NGAYHOANTHANH_THEOMONGMUON = congviec.NGAYHOANTHANH_THEOMONGMUON,
                            NGAYSUA = congviec.NGAYSUA,
                            NGAYTAO = congviec.NGAYTAO,
                            NGUOIGIAOVIECDANHGIA = congviec.NGUOIGIAOVIECDANHGIA,
                            NGUOIGIAOVIECDAPHANHOI = congviec.NGUOIGIAOVIECDAPHANHOI,
                            NGUOIGIAOVIEC_ID = congviec.NGUOIGIAOVIEC_ID,
                            NGUOITAO = congviec.NGUOITAO,
                            NGUOIXULYCHINH_ID = congviec.NGUOIXULYCHINH_ID,
                            NOIDUNGCONGVIEC = congviec.NOIDUNGCONGVIEC,
                            PHANTRAMHOANTHANH = congviec.PHANTRAMHOANTHANH,
                            PHANTRAMHOANTHANH_OLD = congviec.PHANTRAMHOANTHANH_OLD,
                            SONGAYNHACTRUOCHAN = congviec.SONGAYNHACTRUOCHAN,
                            SUBTASK_ID = congviec.SUBTASK_ID,
                            TENCONGVIEC = congviec.TENCONGVIEC,
                            TRANGTHAI_ID = congviec.TRANGTHAI_ID,
                            TEN_DOKHAN = g1.TEXT,
                            TEN_DOUUTIEN = g2.TEXT,
                            ICON_DOKHAN = g1.ICON,
                            ICON_DOUUTIEN = g2.ICON,
                            TEN_NGUOIGIAOVIEC = g4.HOTEN,
                            TEN_NGUOIXULYCHINH = g3.HOTEN,
                            IS_BATDAU = (true == congviec.IS_BATDAU),
                            HasChild = (g5 != null)
                        };
            //switch (LoaiCongViec)
            //{
            //    //Đối với công việc cá nhân
            //    case LOAI_CONGVIEC.CANHAN:
            //        query = query.Where(x => UserId == x.NGUOITAO || UserId == x.NGUOIGIAOVIEC_ID);
            //        break;
            //    //Đối với công việc được giao
            //    case LOAI_CONGVIEC.DUOCGIAO:
            //        //query = query.Where(x => searchModel.USER_ID == x.NGUOIXULYCHINH_ID && true != x.IS_MYJOB);
            //        query = query.Where(x => UserId == x.NGUOIXULYCHINH_ID);
            //        break;
            //    //Đối với công việc được phối hợp xử lý
            //    case LOAI_CONGVIEC.PHOIHOP_XULY:
            //        var Ids = (from phoihop in this.context.HSCV_CONGVIEC_NGUOITHAMGIAXULY
            //                   where UserId == phoihop.USER_ID
            //                   select phoihop.CONGVIEC_ID).ToList();
            //        query = query.Where(x => Ids.Contains(x.ID));
            //        break;
            //}
            query = query.GroupBy(x => x.ID).Select(y => y.FirstOrDefault());
            query = query.OrderBy(x => x.TENCONGVIEC);
            return query.ToList();
        }

        public List<HSCV_CONGVIEC> GetData(List<long> Ids, long id)
        {
            var result = from congviec in this.context.HSCV_CONGVIEC
                         where Ids.Contains(congviec.ID) && (id == congviec.NGUOIXULYCHINH_ID || id == congviec.NGUOIGIAOVIEC_ID)
                         select congviec;
            return result.ToList();
        }
        public List<CongViecBO> GetDanhGiaCongViec(List<int> DeptId, List<long> ListNguoiDung, DateTime NGAYGIAOVIEC_FROM, DateTime NGAYGIAOVIEC_TO)
        {
            var item = (from cv in this.context.HSCV_CONGVIEC
                        join nxl in this.context.DM_NGUOIDUNG
                        on cv.NGUOIXULYCHINH_ID.Value equals nxl.ID
                             into group4
                        from g4 in group4.DefaultIfEmpty()

                        join phieu in this.context.PHIEUDANHGIACONGVIEC
                        on cv.ID equals phieu.CONGVIEC_ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()

                        join datadm in this.context.DM_DANHMUC_DATA
                        on cv.DOKHAN equals datadm.ID
                        into groupdata
                        from gdta in groupdata.DefaultIfEmpty()
                            //where 100 == cv.PHANTRAMHOANTHANH &&

                        where (ListNguoiDung.Any() ? (cv.NGUOIXULYCHINH_ID.HasValue && ListNguoiDung.Contains(cv.NGUOIXULYCHINH_ID.Value)) : true)
                        select new CongViecBO
                        {
                            ID = cv.ID,
                            TENCONGVIEC = cv.TENCONGVIEC,
                            PHANTRAMHOANTHANH = cv.PHANTRAMHOANTHANH,
                            NGAY_NHANVIEC = cv.NGAY_NHANVIEC,
                            NGAYHOANTHANH_THEOMONGMUON = cv.NGAYHOANTHANH_THEOMONGMUON,
                            NGAYBATDAU_KEHOACH = cv.NGAYBATDAU_KEHOACH,
                            NGAYKETTHUC_KEHOACH = cv.NGAYKETTHUC_KEHOACH,
                            NGAYBATDAU_THUCTE = cv.NGAYBATDAU_THUCTE,
                            NGAYKETTHUC_THUCTE = cv.NGAYKETTHUC_THUCTE,
                            TEN_NGUOIXULYCHINH = g4.HOTEN,
                            DATUDANHGIA = cv.DATUDANHGIA,
                            IS_ASSIGNED = cv.IS_ASSIGNED,
                            IS_BATDAU = cv.IS_BATDAU,
                            XEPLOAICONGVIEC = cv.XEPLOAICONGVIEC,
                            TONGDIEM = g1.TONGDIEM.HasValue ? g1.TONGDIEM.Value : 0,
                            XEPLOAI = g1.XEPLOAI,
                            TEN_DOKHAN = gdta.TEXT,
                            TRONGSOCONGVIEC = gdta.DATA.Value
                        });
            item = item.Where(x => (NGAYGIAOVIEC_FROM <= x.NGAY_NHANVIEC && x.NGAY_NHANVIEC <= NGAYGIAOVIEC_TO) || (NGAYGIAOVIEC_FROM <= x.NGAYBATDAU_KEHOACH && x.NGAYBATDAU_KEHOACH <= NGAYGIAOVIEC_TO));
            return item.ToList();

        }
        public CongViecBO FindById(long id)
        {
            var result = from cv in this.context.HSCV_CONGVIEC
                         join nguoidung in this.context.DM_NGUOIDUNG
                         on cv.NGUOIXULYCHINH_ID equals nguoidung.ID
                         into group1
                         from g1 in group1.DefaultIfEmpty()
                         where cv.ID == id
                         select new CongViecBO
                         {
                             ID = cv.ID,
                             TENCONGVIEC = cv.TENCONGVIEC,
                             PHANTRAMHOANTHANH = cv.PHANTRAMHOANTHANH,
                             NGAY_NHANVIEC = cv.NGAY_NHANVIEC,
                             NGAYHOANTHANH_THEOMONGMUON = cv.NGAYHOANTHANH_THEOMONGMUON,
                             NGAYBATDAU_KEHOACH = cv.NGAYBATDAU_KEHOACH,
                             NGAYKETTHUC_KEHOACH = cv.NGAYKETTHUC_KEHOACH,
                             NGAYBATDAU_THUCTE = cv.NGAYBATDAU_THUCTE,
                             NGAYKETTHUC_THUCTE = cv.NGAYKETTHUC_THUCTE,
                             TEN_NGUOIXULYCHINH = g1.HOTEN,
                             DATUDANHGIA = cv.DATUDANHGIA,
                             IS_ASSIGNED = cv.IS_ASSIGNED,
                             IS_BATDAU = cv.IS_BATDAU,
                             XEPLOAICONGVIEC = cv.XEPLOAICONGVIEC,
                         };
            return result.FirstOrDefault();
        }

        /// <summary>
        /// @author: duynn
        /// @since: 11/08/2018
        /// @description: kiểm tra xem có thể giao việc hoặc cho tham gia xử lý hay không?
        /// </summary>
        /// <param name="input">người giao việc</param>
        /// <param name="destination">người được giao việc hoặc người được phân tham gia xử lý</param>
        /// <returns></returns>
        public bool CheckCanAssignOrJoinTask(long input, long destination)
        {
            List<string> roleCodes = new List<string> { "THUKY", "CHUYENVIEN", "TRUONGPHONG", "GIAMDOCDONVI", "BANTONGGIAMDOC" };

            List<DM_NGUOIDUNG_BO> inputUserRoles = (from user_role in this.context.NGUOIDUNG_VAITRO.Where(x => x.NGUOIDUNG_ID == input)
                                                    join role in this.context.DM_VAITRO
                                                    on user_role.VAITRO_ID equals role.DM_VAITRO_ID
                                                    where user_role.VAITRO_ID.HasValue
                                                    select new DM_NGUOIDUNG_BO()
                                                    {
                                                        MA_VAITRO = role.MA_VAITRO
                                                    }).ToList();

            List<DM_NGUOIDUNG_BO> destUserRoles = (from user_role in this.context.NGUOIDUNG_VAITRO.Where(x => x.NGUOIDUNG_ID == destination)
                                                   join role in this.context.DM_VAITRO
                                                   on user_role.VAITRO_ID equals role.DM_VAITRO_ID
                                                   where user_role.VAITRO_ID.HasValue
                                                   select new DM_NGUOIDUNG_BO()
                                                   {
                                                       MA_VAITRO = role.MA_VAITRO
                                                   }).ToList();

            int maxIndexOfInputUser = -1;
            int maxIndexOfDestUser = -1;

            inputUserRoles.ForEach(x =>
            {
                int index = roleCodes.IndexOf(x.MA_VAITRO);

                if(index > maxIndexOfInputUser)
                {
                    maxIndexOfInputUser = index;
                }

            });

            destUserRoles.ForEach(x =>
            {
                int index = roleCodes.IndexOf(x.MA_VAITRO);

                if (index > maxIndexOfDestUser)
                {
                    maxIndexOfDestUser = index;
                }
            });

            if(maxIndexOfInputUser >= maxIndexOfDestUser)
            {
                return true;
            }
            return false;
        }
    }
}
