using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.CommonBusiness;
using System.Web.Mvc;
using Business.BaseBusiness;
using Business.CommonModel.LOGSMS;
using PagedList;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace Business.Business
{
    public class LogSMSBusiness : BaseBusiness<LOGSMS>
    {
        public LogSMSBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public PageListResultBO<LOGSMS_BO> GetDaTaByPage(LOGSMS_SEARCHBO searchModel, int pageSize = 20, int pageIndex = 1)
        {
            var query = from tbllog in this.context.LOGSMS
                        join deptSend in this.context.CCTC_THANHPHAN
                        on tbllog.DONVI_GUI equals deptSend.ID
                        into gLogSend
                        from logSend in gLogSend.DefaultIfEmpty()

                        join deptReceive in this.context.CCTC_THANHPHAN
                        on tbllog.DONVI_GUI equals deptReceive.ID
                        into gLogReceive
                        from logReceive in gLogReceive.DefaultIfEmpty()
                        select new LOGSMS_BO
                        {
                            CREATED_AT = tbllog.CREATED_AT,
                            CREATED_BY = tbllog.CREATED_BY,
                            HOTENNGUOIGUI = tbllog.HOTENNGUOIGUI,
                            HOTENNGUOINHAN = tbllog.HOTENNGUOINHAN,
                            ID = tbllog.ID,
                            ITEMID = tbllog.ITEMID,
                            ITEMTYPE = tbllog.ITEMTYPE,
                            KETQUA = tbllog.KETQUA,
                            NOIDUNG = tbllog.NOIDUNG,
                            SODIENTHOAINHAN = tbllog.SODIENTHOAINHAN,
                            SOKYTU = tbllog.SOKYTU,
                            TEN_DONVI_GUI = logSend.NAME,
                            TEN_DONVI_NHAN = logReceive.NAME
                        };
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.NguoiGui))
                {
                    query = query.Where(a => a.HOTENNGUOIGUI.Contains(searchModel.NguoiGui));
                }
                if (!string.IsNullOrEmpty(searchModel.NguoiNhan))
                {
                    query = query.Where(a => a.HOTENNGUOINHAN.Contains(searchModel.NguoiNhan));
                }
                if (!string.IsNullOrEmpty(searchModel.SoDienThoai))
                {
                    query = query.Where(a => a.SODIENTHOAINHAN.Contains(searchModel.SoDienThoai));
                }
                if (searchModel.TuNgay.HasValue)
                {
                    query = query.Where(x => searchModel.TuNgay <= x.CREATED_AT);
                }
                if (searchModel.DenNgay.HasValue)
                {
                    query = query.Where(x => searchModel.DenNgay >= x.CREATED_AT);
                }

                if (!string.IsNullOrEmpty(searchModel.DonViGui))
                {
                    query = query.Where(a => a.TEN_DONVI_GUI.Contains(searchModel.DonViGui));
                }
                if (!string.IsNullOrEmpty(searchModel.DonViNhan))
                {
                    query = query.Where(a => a.TEN_DONVI_NHAN.Contains(searchModel.DonViNhan));
                }

                //Lọc tìm kiếm
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(a => a.ID);
                }
            }
            else
            {
                query = query.OrderByDescending(a => a.ID);
            }

            //Gán nội dung trả về

            var resultmodel = new PageListResultBO<LOGSMS_BO>();
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
    }
}
