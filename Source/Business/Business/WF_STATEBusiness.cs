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
using Business.CommonModel.WFSTATE;
using System.Web.Mvc;
using CommonHelper;

namespace Business.Business
{
    public class WF_STATEBusiness : BaseBusiness<WF_STATE>
    {
        public WF_STATEBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        /// <summary>
        /// hủy toàn bộ trạng thái kết thúc
        /// </summary>
        /// <param name="id"></param>
        public void ClearKetThuc(int id)
        {
            var lstEnd = this.context.WF_STATE.Where(x => x.WF_ID == id && x.IS_KETTHUC == true).ToList();
            foreach (var item in lstEnd)
            {
                item.IS_KETTHUC = false;
                Save();
            }
        }
        public void ClearBatDau(int id)
        {
            var lstEnd = this.context.WF_STATE.Where(x => x.WF_ID == id && x.IS_START == true).ToList();
            foreach (var item in lstEnd)
            {
                item.IS_START = false;
                item.CHUCVU_ID = null;
                item.VAITRO_ID = null;
                Save();
            }
        }

        public List<SelectListItem> GetDsTrangThai(int idStream, int selectedItem = 0)
        {
            var query = this.context.WF_STATE.Where(x => x.WF_ID == idStream).Select(x => new SelectListItem()
            {
                Text = x.STATE_NAME,
                Value = x.ID.ToString(),
                Selected = selectedItem > 0 && x.ID == selectedItem
            }).ToList();
            return query;
        }
        public PageListResultBO<WF_STATE_BO> GetDaTaByPage(int idStream, WF_STATE_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from tbl in this.context.WF_STATE
                        where tbl.WF_ID == idStream
                        join tblFunction in this.context.WF_STATE_FUNCTION on tbl.ID equals tblFunction.WF_STATE_ID into jstatefunction
                        from statefunction in jstatefunction.DefaultIfEmpty()

                        join tblChucVu in this.context.DM_DANHMUC_DATA on tbl.CHUCVU_ID equals tblChucVu.ID into jChucVu
                        from chucVu in jChucVu.DefaultIfEmpty()

                        join tblVaiTro in this.context.DM_VAITRO on tbl.VAITRO_ID equals tblVaiTro.DM_VAITRO_ID into jVaiTro
                        from vaiTro in jVaiTro.DefaultIfEmpty()

                        select new WF_STATE_BO
                        {
                            ID = tbl.ID,
                            WF_ID = tbl.WF_ID,
                            STATE_NAME = tbl.STATE_NAME,
                            GHICHU = tbl.GHICHU,
                            IS_KETTHUC = tbl.IS_KETTHUC,
                            create_at = tbl.create_at,
                            create_by = tbl.create_by,
                            edit_at = tbl.edit_at,
                            edit_by = tbl.edit_by,
                            FunctionID = statefunction.ACTION,
                            FunctionName = statefunction != null ? this.context.WF_FUNCTION.Where(x => x.ID == statefunction.ACTION).Select(x => x.FUNTION_TITLE).FirstOrDefault() : "",
                            IS_START = tbl.IS_START,
                            ChucVu = chucVu != null ? chucVu.TEXT : "",
                            VaiTro = vaiTro != null ? vaiTro.TEN_VAITRO : ""
                        };
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.QR_STATE_NAME))
                {
                    query = query.Where(x => x.STATE_NAME.Contains(searchModel.QR_STATE_NAME));

                }

                if (!string.IsNullOrEmpty(searchModel.QR_GHICHU))
                {
                    query = query.Where(x => x.GHICHU.Contains(searchModel.QR_GHICHU));

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
            var resultmodel = new PageListResultBO<WF_STATE_BO>();
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
        public WF_STATE_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.WF_STATE
                        where tbl.ID == ID
                        select new WF_STATE_BO
                        {
                            ID = tbl.ID,
                            WF_ID = tbl.WF_ID,
                            STATE_NAME = tbl.STATE_NAME,
                            GHICHU = tbl.GHICHU,
                            IS_KETTHUC = tbl.IS_KETTHUC,
                            create_at = tbl.create_at,
                            create_by = tbl.create_by,
                            edit_at = tbl.edit_at,
                            edit_by = tbl.edit_by,
                        };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy trạng thái cuối cùng của đối tượng trong luồng xử lý
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public WF_STATE GetFinalStateOfItem(string itemType, UserInfoBO user)
        {
            WF_STATE result = new WF_STATE();
            WF_MODULE wfModule = this.context.WF_MODULE.Where(x => x.MODULE_CODE == itemType).FirstOrDefault();
            if (wfModule != null && string.IsNullOrEmpty(wfModule.WF_STREAM_ID) == false)
            {
                //Lấy thông tin luồng xử lý
                var department = this.context.CCTC_THANHPHAN.Find(user.DM_PHONGBAN_ID) ?? new CCTC_THANHPHAN();
                var wfStreamIds = wfModule.WF_STREAM_ID.ToListInt(',');

                var wfStream = this.context.WF_STREAM
                    .Where(x => x.LEVEL_ID == department.CATEGORY && wfStreamIds.Contains(x.ID))
                    .FirstOrDefault() ?? new WF_STREAM();

                result = this.context.WF_STATE
                        .Where(x => x.WF_ID == wfStream.ID && x.IS_KETTHUC == true)
                        .FirstOrDefault();
            }
            return result;
        }
    }
}

