using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.DMNguoiDung;
using Business.CommonModel.QLNGUOINHANVANBAN;
using CommonHelper;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.QL_NGUOINHAN_VANBANArea.Models;
using Web.Custom;
using Web.FwCore;

namespace Web.Areas.QL_NGUOINHAN_VANBANArea.Controllers
{
    public class QL_NGUOINHAN_VANBANController : BaseController
    {
        // GET: QL_NGUOINHAN_VANBANArea/QL_NGUOINHAN_VANBAN
        private DM_NGUOIDUNGBusiness UserBusiness;
        private QL_NGUOINHAN_VANBANBusiness RecipientBusiness;
        SYS_THONGBAOBusiness SYS_THONGBAOBusiness;

        // GET: DTPhanCongDaoTao/DTPhanCongDaoTao

        /// <summary>
        /// @author: duynn
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            AssignUserInfo();
            RecipientBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();
            QLNguoiNhanVanBanIndexViewModel viewModel = new QLNguoiNhanVanBanIndexViewModel();
            QL_NGUOINHAN_VANBAN_SEARCH_BO searchModel = new QL_NGUOINHAN_VANBAN_SEARCH_BO();
            searchModel.QueryDeptId = currentUser.DeptParentID;
            SessionManager.SetValue("QLNguoiNhanVanBanSearch", searchModel);
            viewModel.GroupRecipients = RecipientBusiness.GetDataByPage(searchModel);
            return View(viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// </summary>
        /// <returns></returns>
        public PartialViewResult EditRecipients(int id = 0)
        {
            AssignUserInfo();
            RecipientBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();
            UserBusiness = Get<DM_NGUOIDUNGBusiness>();

            QLNguoiNhanVanBanEditViewModel viewModel = new QLNguoiNhanVanBanEditViewModel();
            QL_NGUOINHAN_VANBAN entity = new QL_NGUOINHAN_VANBAN();
            List<long> selected = new List<long>();
            if (id > 0)
            {
                entity = RecipientBusiness.Find(id);
                if (string.IsNullOrEmpty(entity.NGUOINHAN_IDS) == false)
                {
                    selected = entity.NGUOINHAN_IDS.ToListLong(',');
                }
            }
            List<NguoiDungPhongBanBO> users = UserBusiness.GetUsersOfDepartments();
            foreach (var item in users)
            {
                SelectListGroup group = new SelectListGroup() { Name = item.PhongBan.NAME };
                item.LstNguoiDung = item.LstNguoiDung.OrderBy(x => x.HOTEN).ToList();
                foreach (var subItem in item.LstNguoiDung)
                {
                    viewModel.GroupUsers.Add(new SelectListItem()
                    {
                        Text = subItem.HOTEN,
                        Value = subItem.ID.ToString(),
                        Selected = selected.Contains(subItem.ID),
                        Group = group
                    });
                }
            }
            viewModel.Entity = entity;
            return PartialView("_EditRecipient", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveRecipients(FormCollection collection)
        {
            AssignUserInfo();
            JsonResultBO result = new JsonResultBO(true);

            RecipientBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();
            long id = collection["ID"].ToLongOrZero();
            string name = collection["TEN_NHOM"];
            string users = collection["NGUOINHAN_IDS"];
            int contentId = collection["DT_NOIDUNG_DAOTAO_ID"].ToIntOrZero();

            QL_NGUOINHAN_VANBAN recipients = new QL_NGUOINHAN_VANBAN();
            recipients.TEN_NHOM = name.Trim();
            recipients.NGUOINHAN_IDS = users;
            recipients.DM_PHONGBAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            if (id > 0)
            {
                bool existed = RecipientBusiness.context.QL_NGUOINHAN_VANBAN
                    .Where(x => x.TEN_NHOM == name && x.ID != id
                    && x.DM_PHONGBAN_ID == currentUser.DM_PHONGBAN_ID
                    && x.IS_DELETE != true).Count() > 0;
                if (existed)
                {
                    result.Status = false;
                    result.Message = "Nhóm đã tồn tại";
                }
                else
                {
                    QL_NGUOINHAN_VANBAN dbRecipients = RecipientBusiness.Find(id);
                    dbRecipients.TEN_NHOM = recipients.TEN_NHOM;
                    dbRecipients.NGUOINHAN_IDS = recipients.NGUOINHAN_IDS;
                    //dbRecipients.DM_PHONGBAN_ID = currentUser.DeptParentID.GetValueOrDefault();
                    RecipientBusiness.Save(dbRecipients);
                    result.Message = "Tạo nhóm thành công";
                }
            }
            else
            {
                bool existed = RecipientBusiness.context.QL_NGUOINHAN_VANBAN
                    .Where(x => x.TEN_NHOM == name
                    && x.DM_PHONGBAN_ID == currentUser.DeptParentID
                    && x.IS_DELETE != true).Count() > 0;
                if (existed)
                {
                    result.Status = false;
                    result.Message = "Nhóm đã tồn tại";
                }
                else
                {
                    RecipientBusiness.Save(recipients);
                    result.Message = "Cập nhật nhóm thành công";
                }
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// </summary>
        /// <returns></returns>
        public JsonResult SearchData(FormCollection collection)
        {
            AssignUserInfo();
            RecipientBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();
            PageListResultBO<QL_NGUOINHAN_VANBAN_BO> result = new PageListResultBO<QL_NGUOINHAN_VANBAN_BO>();
            QL_NGUOINHAN_VANBAN_SEARCH_BO searchModel = (QL_NGUOINHAN_VANBAN_SEARCH_BO)SessionManager.GetValue("QLNguoiNhanVanBanSearch");
            if (searchModel == null)
            {
                searchModel = new QL_NGUOINHAN_VANBAN_SEARCH_BO();
            }
            searchModel.QueryDeptId = currentUser.DeptParentID.GetValueOrDefault();
            searchModel.QueryName = collection["TEN_NHOM"];
            result = RecipientBusiness.GetDataByPage(searchModel);
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDataPerPage(int pageSize, int pageIndex, string sortQuery)
        {
            AssignUserInfo();
            RecipientBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();
            PageListResultBO<QL_NGUOINHAN_VANBAN_BO> result = new PageListResultBO<QL_NGUOINHAN_VANBAN_BO>();
            QL_NGUOINHAN_VANBAN_SEARCH_BO searchModel = (QL_NGUOINHAN_VANBAN_SEARCH_BO)SessionManager.GetValue("QLNguoiNhanVanBanSearch");
            if (searchModel == null)
            {
                searchModel = new QL_NGUOINHAN_VANBAN_SEARCH_BO();
            }
            searchModel.QueryDeptId = currentUser.DeptParentID.GetValueOrDefault();
            searchModel.pageIndex = pageIndex;
            searchModel.pageSize = pageSize;
            searchModel.sortQuery = sortQuery;
            result = RecipientBusiness.GetDataByPage(searchModel);
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// </summary>
        /// <returns></returns>
        public JsonResult Delete(long id)
        {
            JsonResultBO result = new JsonResultBO(true);
            RecipientBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();
            QL_NGUOINHAN_VANBAN entity = RecipientBusiness.Find(id);
            if (entity != null)
            {
                //entity.IS_DELETE = true;
                //RecipientBusiness.Save(entity);
                RecipientBusiness.repository.Delete(id);
                result.Message = "Xóa nhóm người nhận thành công";

            }
            else
            {
                result.Status = false;
                result.Message = "Thông tin nhóm người nhận không tồn tại";
            }
            return Json(result);
        }
    }
}