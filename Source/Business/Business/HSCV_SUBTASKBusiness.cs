using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVCONGVIEC;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Business.Business
{
    public class HSCV_SUBTASKBusiness : BaseBusiness<HSCV_SUBTASK>
    {
        public HSCV_SUBTASKBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public void Save(HSCV_SUBTASK Task)
        {
            try
            {
                if (Task.ID == 0)
                {
                    this.repository.Insert(Task);
                }
                else
                {
                    this.repository.Update(Task);
                }
                this.repository.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SubTaskBO> getSubTask(long CONGVIEC_ID, int TRANGTHAI_ID)
        {
            var result = (from subtask in this.context.HSCV_SUBTASK
                          join cv in this.context.HSCV_CONGVIEC
                          on subtask.ID equals cv.SUBTASK_ID
                          into g1
                          from group1 in g1.DefaultIfEmpty()
                          join nguoidung in this.context.DM_NGUOIDUNG
                          on group1.NGUOIXULYCHINH_ID equals nguoidung.ID
                          into g2
                          from group2 in g2.DefaultIfEmpty()
                          select new SubTaskBO
                          {
                              CONGVIEC_ID = subtask.CONGVIEC_ID,
                              DAGIAOVIEC = subtask.DAGIAOVIEC,
                              HANHOANTHANH = subtask.HANHOANTHANH,
                              ID = subtask.ID,
                              MUCDOUUTIEN = subtask.MUCDOUUTIEN,
                              NGAYHOANTHANH = subtask.NGAYHOANTHANH,
                              NGAYTAO = subtask.NGAYTAO,
                              NOIDUNG = subtask.NOIDUNG,
                              TRANGTHAI_ID = subtask.TRANGTHAI_ID,
                              NGUOITHUCHIEN = group2.HOTEN,
                              PHANTRAMHOANTHANH = group1.PHANTRAMHOANTHANH,
                          }
                              ).Where(x => x.CONGVIEC_ID == CONGVIEC_ID);
            if (TRANGTHAI_ID >= 0)
            {
                result = result.Where(x => x.TRANGTHAI_ID == TRANGTHAI_ID);
            }
            return result.OrderBy(x => x.NGAYHOANTHANH).ToList();

        }
        public List<SubTaskBO> getSubTaskForJob(long CONGVIEC_ID, int TRANGTHAI_ID)
        {
            var result = (from subtask in this.context.HSCV_SUBTASK
                          join cv in this.context.HSCV_CONGVIEC
                              on subtask.ID equals cv.SUBTASK_ID
                              into g1
                          from group1 in g1.DefaultIfEmpty()
                          join nguoidung in this.context.DM_NGUOIDUNG
                              on group1.NGUOIXULYCHINH_ID equals nguoidung.ID
                              into g2
                          from group2 in g2.DefaultIfEmpty()
                          join dmuc in this.context.DM_DANHMUC_DATA
                              on subtask.DOKHAN equals dmuc.ID
                          into g3
                          from group3 in g3.DefaultIfEmpty()
                          join dmuc2 in this.context.DM_DANHMUC_DATA
                              on subtask.MUCDOUUTIEN equals dmuc2.ID
                              into g4
                          from group4 in g4.DefaultIfEmpty()
                          select new SubTaskBO
                          {
                              CONGVIEC_ID = subtask.CONGVIEC_ID,
                              DAGIAOVIEC = subtask.DAGIAOVIEC,
                              HANHOANTHANH = subtask.HANHOANTHANH,
                              ID = subtask.ID,
                              MUCDOUUTIEN = subtask.MUCDOUUTIEN,
                              NGAYHOANTHANH = subtask.NGAYHOANTHANH,
                              NGAYTAO = subtask.NGAYTAO,
                              NOIDUNG = subtask.NOIDUNG,
                              TRANGTHAI_ID = subtask.TRANGTHAI_ID,
                              NGUOITHUCHIEN = group2.HOTEN,
                              PHANTRAMHOANTHANH = group1.PHANTRAMHOANTHANH,
                              DOKHAN_TEXT = group3.TEXT,
                              DOUUTIEN_TEXT = group4.TEXT
                          }
                ).Where(x => x.CONGVIEC_ID == CONGVIEC_ID);
            if (TRANGTHAI_ID >= 0)
            {
                result = result.Where(x => x.TRANGTHAI_ID == TRANGTHAI_ID);
            }
            return result.OrderBy(x => x.NGAYHOANTHANH).ToList();

        }

        /// <summary>
        /// @description: lấy danh sách công việc
        /// @author: duynn
        /// @since: 01/06/2018
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<SubTaskBO> GetSubTaskByMainTaskId(long taskId, string query, int pageIndex = 1, int pageSize = 20)
        {
            var subTaskQuery = (from subtask in this.context.HSCV_SUBTASK
                                join cv in this.context.HSCV_CONGVIEC
                                    on subtask.ID equals cv.SUBTASK_ID
                                    into g1
                                from group1 in g1.DefaultIfEmpty()
                                join nguoidung in this.context.DM_NGUOIDUNG
                                    on group1.NGUOIXULYCHINH_ID equals nguoidung.ID
                                    into g2
                                from group2 in g2.DefaultIfEmpty()
                                join dmuc in this.context.DM_DANHMUC_DATA
                                    on subtask.DOKHAN equals dmuc.ID
                                into g3
                                from group3 in g3.DefaultIfEmpty()
                                join dmuc2 in this.context.DM_DANHMUC_DATA
                                    on subtask.MUCDOUUTIEN equals dmuc2.ID
                                    into g4
                                from group4 in g4.DefaultIfEmpty()
                                select new SubTaskBO
                                {
                                    CONGVIEC_ID = subtask.CONGVIEC_ID,
                                    TIEUDE_CONGVIEC = "Công việc #" + subtask.ID,
                                    DAGIAOVIEC = subtask.DAGIAOVIEC,
                                    HANHOANTHANH = subtask.HANHOANTHANH,
                                    ID = subtask.ID,
                                    MUCDOUUTIEN = subtask.MUCDOUUTIEN,
                                    NGAYHOANTHANH = subtask.NGAYHOANTHANH,
                                    NGAYTAO = subtask.NGAYTAO,
                                    NOIDUNG = subtask.NOIDUNG,
                                    TRANGTHAI_ID = subtask.TRANGTHAI_ID,
                                    NGUOITHUCHIEN = group2.HOTEN,
                                    PHANTRAMHOANTHANH = group1.PHANTRAMHOANTHANH,
                                    DOKHAN_TEXT = group3.TEXT,
                                    DOUUTIEN_TEXT = group4.TEXT
                                }
               ).Where(x => x.CONGVIEC_ID == taskId);

            if (!string.IsNullOrEmpty(query))
            {
                query = query.Trim().ToLower();
                subTaskQuery = subTaskQuery.Where(x => string.IsNullOrEmpty(x.TIEUDE_CONGVIEC) == false
                && x.TIEUDE_CONGVIEC.Trim().ToLower().Contains(query));
            }
            var result = subTaskQuery.OrderByDescending(x => x.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }
    }
}
