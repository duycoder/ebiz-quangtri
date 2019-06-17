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
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;

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
            SessionManager.SetValue("QLNguoiNhanVanBanSearch", searchModel);
            viewModel.GroupRecipients = RecipientBusiness.GetDataByPage(searchModel, currentUser);
            viewModel.IsSystemAdmin = currentUser.ListVaiTro.Any(x => x.MA_VAITRO == "QLHT");
            viewModel.DepartmentId = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
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
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();

            QLNguoiNhanVanBanEditViewModel viewModel = new QLNguoiNhanVanBanEditViewModel();
            QL_NGUOINHAN_VANBAN entity = RecipientBusiness.Find(id) ?? new QL_NGUOINHAN_VANBAN();
            List<long> choosenUserIds = new List<long>();

            /**
             * dah sách ngươi dùng
             */
            if (!string.IsNullOrEmpty(entity.NGUOINHAN_IDS))
            {
                choosenUserIds = entity.NGUOINHAN_IDS.ToListLong(',');
            }
            /**
             * lấy danh sách người dùng phòng ban
             */
            List<NguoiDungPhongBanBO> groupDepts = UserBusiness.GetUsersOfDepartments();
            
            //if (currentUser.ListVaiTro.Any(x=>x.MA_VAITRO == "QLHT"))
            //{
            //    groupDeptUsers = UserBusiness.GetUsersOfDepartments();
            //}
            //else
            //{
            //    groupDeptUsers = UserBusiness.GetUsersOfDepartments(currentUser.DM_PHONGBAN_ID.GetValueOrDefault());
            //}

            foreach (var dept in groupDepts)
            {
                SelectListGroup group = new SelectListGroup() { Name = dept.PhongBan.NAME };
                viewModel.GroupUsers.AddRange(this.GetGroupUsers(group, choosenUserIds, dept.LstNguoiDung));
            }
            viewModel.Department = CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID) ?? new CCTC_THANHPHAN();
            viewModel.IsSystemAdmin = currentUser.ListVaiTro.Any(x => x.MA_VAITRO == "QLHT");
            viewModel.Entity = entity;
            return PartialView("_EditRecipient", viewModel);
        }

        /// <summary>
        /// @author:duynn
        /// @description: lấy danh sách người dùng
        /// </summary>
        /// <param name="group"></param>
        /// <param name="choosenUserIds"></param>
        /// <param name="groupUsers"></param>
        /// <returns></returns>
        private IEnumerable<SelectListItem> GetGroupUsers(SelectListGroup group,
            List<long> choosenUserIds, 
            List<DM_NGUOIDUNG_BO> groupUsers)
        {
            foreach (var subItem in groupUsers)
            {
                var item = new SelectListItem()
                {
                    Text = subItem.HOTEN,
                    Value = subItem.ID.ToString(),
                    Selected = choosenUserIds.Contains(subItem.ID),
                    Group = group
                };
                yield return item;
            }
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
            RecipientBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();

            JsonResultBO result = new JsonResultBO(true);
            long id = collection["ID"].ToLongOrZero();
            
            QL_NGUOINHAN_VANBAN recipients = RecipientBusiness.Find(id) ?? new QL_NGUOINHAN_VANBAN();
            recipients.TEN_NHOM = collection["TEN_NHOM"];
            bool existed = RecipientBusiness.CheckExistedName(recipients.TEN_NHOM, recipients.ID);
            if (existed)
            {
                result.Status = false;
                result.Message = "Nhóm đã tồn tại trên hệ thống";
            }

            recipients.NGUOINHAN_IDS = collection["NGUOINHAN_IDS"];
            recipients.DM_PHONGBAN_ID = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
            recipients.IS_DEFAULT = collection["IS_DEFAULT"].ToIntOrZero() > 0;
            RecipientBusiness.Save(recipients);

            result.Message = "Cập nhật nhóm người nhận thành công";
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
            //searchModel.QueryDeptId = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
            searchModel.QueryName = collection["TEN_NHOM"];
            result = RecipientBusiness.GetDataByPage(searchModel, currentUser);
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
            //searchModel.QueryDeptId = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
            searchModel.pageIndex = pageIndex;
            searchModel.pageSize = pageSize;
            searchModel.sortQuery = sortQuery;
            result = RecipientBusiness.GetDataByPage(searchModel, currentUser);
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

        /// <summary>
        /// @author:duynn
        /// @description:hiển thị danh sách người dùng của nhóm vai trò
        /// @since: 11/06/2019
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult ListUsers(int groupId)
        {
            UserBusiness = Get<DM_NGUOIDUNGBusiness>();
            var users = UserBusiness.GetUsersByRecipient(groupId, string.Empty);
            return PartialView("_ListUsers", users);
        }
    }
}