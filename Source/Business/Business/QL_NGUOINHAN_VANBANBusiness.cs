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
using Business.CommonModel.DMDANHMUCDATA;
using System.Web.Mvc;
using Business.CommonModel.ACTIONAUDIT;
using Business.CommonModel.QLNGUOINHANVANBAN;
using CommonHelper;

namespace Business.Business
{
    public class QL_NGUOINHAN_VANBANBusiness : BaseBusiness<QL_NGUOINHAN_VANBAN>
    {
        public QL_NGUOINHAN_VANBANBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {

        }

        /// <summary>
        /// @author:duynn
        /// @since: 11/06/2019
        /// @description: kiểm tra trùng tên
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckExistedName(string name, int id)
        {
            return this.repository.All()
                .Any(x => x.TEN_NHOM == name && x.ID != id);
            //if(user.ListVaiTro.Any(x=>x.MA_VAITRO == "QLHT"))
            //{
            //    return this.repository.All()
            //        .Where(x=> x.DM_PHONGBAN_ID == user.DM_PHONGBAN_ID || x.IS_DEFAULT == true)
            //        .Where(x => x.TEN_NHOM == name && x.ID != id).Any();
            //}
            //else
            //{
            //    return this.repository.All()
            //        .Where(x => x.DM_PHONGBAN_ID == user.DM_PHONGBAN_ID)
            //        .Where(x => x.TEN_NHOM == name && x.ID != id).Any();
            //}
        }

        /// <summary>
        /// @author: duynn
        /// @description: danh sách người nhận văn bản
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageListResultBO<QL_NGUOINHAN_VANBAN_BO> GetDataByPage(
            QL_NGUOINHAN_VANBAN_SEARCH_BO searchModel, UserInfoBO user, int pageIndex = 1, int pageSize = 20)
        {
            var queryUsers = this.context.DM_NGUOIDUNG;
            var queryResult = (from recipient in this.context.QL_NGUOINHAN_VANBAN
                               join dept in this.context.CCTC_THANHPHAN
                               on recipient.DM_PHONGBAN_ID equals dept.ID
                               into groupDeptRecipients
                               from gDeptRecipients in groupDeptRecipients.DefaultIfEmpty()
                               where recipient.IS_DELETE != true
                               && (recipient.IS_DEFAULT == true || recipient.DM_PHONGBAN_ID == user.DM_PHONGBAN_ID)
                               orderby recipient.TEN_NHOM
                               select new QL_NGUOINHAN_VANBAN_BO()
                               {
                                   ID = recipient.ID,
                                   TEN_NHOM = recipient.TEN_NHOM,
                                   NGUOINHAN_IDS = recipient.NGUOINHAN_IDS,
                                   IS_DEFAULT = recipient.IS_DEFAULT,
                                   DM_PHONGBAN_ID = recipient.DM_PHONGBAN_ID,
                                   TEN_PHONGBAN = gDeptRecipients.NAME,
                               });
            if (searchModel != null)
            {
                if (string.IsNullOrEmpty(searchModel.QueryName) == false)
                {
                    searchModel.QueryName = searchModel.QueryName.Trim().ToLower();
                    queryResult = queryResult.Where(x => x.TEN_NHOM != null && x.TEN_NHOM.Trim().ToLower().Contains(searchModel.QueryName));
                }

                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    queryResult = queryResult.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    queryResult = queryResult.OrderByDescending(x => x.ID);
                }
            }
            else
            {
                queryResult = queryResult.OrderByDescending(x => x.ID);
            }

            var result = new PageListResultBO<QL_NGUOINHAN_VANBAN_BO>();
            if (pageSize == -1)
            {
                var listData = queryResult.ToList();
                result.Count = listData.Count;
                result.TotalPage = 1;
                result.ListItem = listData;
            }
            else
            {
                var pagedListData = queryResult.ToPagedList(pageIndex, pageSize);
                result.Count = pagedListData.TotalItemCount;
                result.TotalPage = pagedListData.PageCount;
                result.ListItem = pagedListData.ToList();
            }
            foreach (var item in result.ListItem)
            {
                if (!string.IsNullOrEmpty(item.NGUOINHAN_IDS))
                {
                    List<long> userIds = item.NGUOINHAN_IDS.ToListLong(',');
                    IQueryable<DM_NGUOIDUNG> users = queryUsers.Where(x => userIds.Contains(x.ID));
                    item.Members = string.Join("<br/>", users.Select(x => x.HOTEN).ToArray());
                }
            }
            return result;
        }

        //danh sách quản lý

        /// <summary>
        /// @lấy danh sách người nhận văn bản của cơ quan
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public List<QL_NGUOINHAN_VANBAN_BO> GetRecipientGroups(int deptId)
        {
            List<QL_NGUOINHAN_VANBAN_BO> result = new List<QL_NGUOINHAN_VANBAN_BO>();
            IQueryable<QL_NGUOINHAN_VANBAN> recipientGroups = this.context.QL_NGUOINHAN_VANBAN
                .Where(x => x.DM_PHONGBAN_ID == deptId && x.IS_DELETE != true);

            foreach (var item in recipientGroups)
            {
                QL_NGUOINHAN_VANBAN_BO itemResult = new QL_NGUOINHAN_VANBAN_BO();
                itemResult.ID = item.ID;
                itemResult.TEN_NHOM = item.TEN_NHOM;
                if (string.IsNullOrEmpty(item.NGUOINHAN_IDS) == false)
                {
                    List<long> userIds = item.NGUOINHAN_IDS.ToListLong(',');
                    itemResult.Users = this.context.DM_NGUOIDUNG.Where(x => userIds.Contains(x.ID)).ToList();
                }
                result.Add(itemResult);
            }
            return result;
        }

        public List<QL_NGUOINHAN_VANBAN_BO> GetRecipientGroups(int deptId, List<long> idsExclude)
        {
            List<QL_NGUOINHAN_VANBAN_BO> result = new List<QL_NGUOINHAN_VANBAN_BO>();
            IQueryable<QL_NGUOINHAN_VANBAN> recipientGroups = this.context.QL_NGUOINHAN_VANBAN
                .Where(x => x.DM_PHONGBAN_ID == deptId && x.IS_DELETE != true);

            foreach (var item in recipientGroups)
            {
                QL_NGUOINHAN_VANBAN_BO itemResult = new QL_NGUOINHAN_VANBAN_BO();
                itemResult.ID = item.ID;
                itemResult.TEN_NHOM = item.TEN_NHOM;
                if (string.IsNullOrEmpty(item.NGUOINHAN_IDS) == false)
                {
                    List<long> userIds = item.NGUOINHAN_IDS.ToListLong(',');
                    itemResult.Users = this.context.DM_NGUOIDUNG.Where(x => userIds.Contains(x.ID) && idsExclude.Contains(x.ID) == false).ToList();
                }
                result.Add(itemResult);
            }
            return result;
        }
    }
}

