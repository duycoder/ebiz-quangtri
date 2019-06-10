using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;
using Model.Entities;
using Business.CommonModel.WFLOG;
using Business.CommonModel.WFSTEP;
using Business.CommonModel.DMNguoiDung;

namespace Business.Business
{
    public class WF_LOGBusiness : BaseBusiness<WF_LOG>
    {
        public WF_LOGBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public List<WF_LOG_BO> GetLstLog(long itemId, string itemType)
        {
            var query = (from log in this.context.WF_LOG.Where(x => x.ITEM_ID == itemId && x.ITEM_TYPE.Equals(itemType))

                         join tblUser in this.context.DM_NGUOIDUNG on log.NGUOIXULY_ID equals tblUser.ID into jNgXuLy
                         from ngxuly in jNgXuLy.DefaultIfEmpty()

                         join tblUser2 in this.context.DM_NGUOIDUNG on log.NGUONHAN_ID equals tblUser2.ID into jNgNhan
                         from ngnhan in jNgNhan.DefaultIfEmpty()

                         join tblsteo in this.context.WF_STEP on log.STEP_ID equals tblsteo.ID into jstep
                         from step in jstep.DefaultIfEmpty()

                         join tblUserThamGia in this.context.WF_ITEM_USER_PROCESS.Where(x => x.IS_XULYCHINH == false) on new { item = log.ITEM_ID, type = log.ITEM_TYPE, step = log.STEP_ID } equals new { item = tblUserThamGia.ITEM_ID, type = tblUserThamGia.ITEM_TYPE, step = tblUserThamGia.STEP_ID } into jthamgia

                         select new WF_LOG_BO
                         {
                             ID = log.ID,
                             create_at = log.create_at,
                             ITEM_ID = log.ITEM_ID,
                             ITEM_TYPE = log.ITEM_TYPE,
                             MESSAGE = log.MESSAGE,
                             NGUOI_UYQUYEN = log.NGUOI_UYQUYEN,
                             NGUOIXULY_ID = log.NGUOIXULY_ID,
                             NGUONHAN_ID = log.NGUONHAN_ID,
                             STEP_ID = log.STEP_ID,
                             WF_ID = log.WF_ID,
                             IS_RETURN=log.IS_RETURN,
                             TenNguoiNhan = ngnhan != null ? ngnhan.HOTEN : "",
                             TenNguoiXuLy = ngxuly != null ? ngxuly.HOTEN : "",
                             step = step != null ? new WF_STEP_BO
                             {
                                 ID = step.ID,
                                 WF_ID = step.WF_ID,
                                 NAME = step.NAME,
                                 GHICHU = step.GHICHU,
                                 STATE_BEGIN = step.STATE_BEGIN,
                                 STATE_END = step.STATE_END,
                                 ICON = step.ICON,
                                 IS_RETURN = step.IS_RETURN,
                                 TrangThaiBatDau = this.context.WF_STATE.Where(x => x.ID == step.STATE_BEGIN).Select(x => x.STATE_NAME).FirstOrDefault() ?? "",
                                 TrangThaiKetThuc = this.context.WF_STATE.Where(x => x.ID == step.STATE_END).Select(x => x.STATE_NAME).FirstOrDefault() ?? "",
                             } : null,
                             LstThamGia = (from ngthamgia in jthamgia
                                           join tblUser in this.context.DM_NGUOIDUNG on ngthamgia.USER_ID equals tblUser.ID into jngthamgia
                                           from tg in jngthamgia.DefaultIfEmpty()
                                           select tg.HOTEN).ToList(),
                             LstTaiLieuDinhKem = (from tailieu in this.context.TAILIEUDINHKEM
                                                  where tailieu.ITEM_ID == log.ID && tailieu.LOAI_TAILIEU == 1400 select tailieu).ToList(),
                         }).OrderByDescending(x => x.ID).ToList();
            return query;
        }

        public WF_LOG_BO GetDataByID(long id)
        {
            var query = (from log in this.context.WF_LOG.Where(x => x.ID == id)

                         join tblUser in this.context.DM_NGUOIDUNG on log.NGUOIXULY_ID equals tblUser.ID into jNgXuLy
                         from ngxuly in jNgXuLy.DefaultIfEmpty()

                         join tblUser2 in this.context.DM_NGUOIDUNG on log.NGUONHAN_ID equals tblUser2.ID into jNgNhan
                         from ngnhan in jNgNhan.DefaultIfEmpty()

                         join tblsteo in this.context.WF_STEP on log.STEP_ID equals tblsteo.ID into jstep
                         from step in jstep.DefaultIfEmpty()

                         join tblUserThamGia in this.context.WF_ITEM_USER_PROCESS.Where(x => x.IS_XULYCHINH == false) on new { item = log.ITEM_ID, type = log.ITEM_TYPE, step = log.STEP_ID } equals new { item = tblUserThamGia.ITEM_ID, type = tblUserThamGia.ITEM_TYPE, step = tblUserThamGia.STEP_ID } into jthamgia

                         select new WF_LOG_BO
                         {
                             ID = log.ID,
                             create_at = log.create_at,
                             ITEM_ID = log.ITEM_ID,
                             ITEM_TYPE = log.ITEM_TYPE,
                             MESSAGE = log.MESSAGE,
                             NGUOI_UYQUYEN = log.NGUOI_UYQUYEN,
                             NGUOIXULY_ID = log.NGUOIXULY_ID,
                             NGUONHAN_ID = log.NGUONHAN_ID,
                             STEP_ID = log.STEP_ID,
                             WF_ID = log.WF_ID,
                             TenNguoiNhan = ngnhan != null ? ngnhan.HOTEN : "",
                             TenNguoiXuLy = ngxuly != null ? ngxuly.HOTEN : "",
                             step = step != null ? new WF_STEP_BO
                             {
                                 ID = step.ID,
                                 WF_ID = step.WF_ID,
                                 NAME = step.NAME,
                                 GHICHU = step.GHICHU,
                                 STATE_BEGIN = step.STATE_BEGIN,
                                 STATE_END = step.STATE_END,
                                 ICON = step.ICON,
                                 IS_RETURN = step.IS_RETURN,
                                 TrangThaiBatDau = this.context.WF_STATE.Where(x => x.ID == step.STATE_BEGIN).Select(x => x.STATE_NAME).FirstOrDefault() ?? "",
                                 TrangThaiKetThuc = this.context.WF_STATE.Where(x => x.ID == step.STATE_END).Select(x => x.STATE_NAME).FirstOrDefault() ?? "",
                             } : null,
                             LstThamGia = (from ngthamgia in jthamgia
                                           join tblUser in this.context.DM_NGUOIDUNG on ngthamgia.USER_ID equals tblUser.ID into jngthamgia
                                           from tg in jngthamgia.DefaultIfEmpty()
                                           select tg.HOTEN).ToList()
                         }).OrderByDescending(x => x.ID).FirstOrDefault();
            return query;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy người xử lý cuối cùng
        /// @since: 15/08/2018
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public DM_NGUOIDUNG_BO GetFinalProcessor(long itemId, string itemType)
        {
            var result = (from user in this.context.DM_NGUOIDUNG
                          join log in this.context.WF_LOG.Where(x => x.ITEM_ID == itemId && x.ITEM_TYPE == itemType)
                          on user.ID equals log.NGUOIXULY_ID
                          orderby log.ID descending
                          select new DM_NGUOIDUNG_BO()
                          {
                              ID = user.ID,
                              HOTEN = user.HOTEN
                          }).FirstOrDefault();
            result =  result ?? new DM_NGUOIDUNG_BO();
            return result;
        }
    }
}
