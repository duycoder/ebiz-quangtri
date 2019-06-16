using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.DMVAITRO;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;


namespace Business.Business
{
    public class DM_VAITROBusiness : BaseBusiness<DM_VAITRO>
    {
        public DM_VAITROBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public void Save(DM_VAITRO item)
        {
            try
            {

                if (item.DM_VAITRO_ID == 0)
                {
                    this.repository.Insert(item);
                }
                else
                {
                    this.repository.Update(item);
                }


                this.repository.Save();
            }
            catch (Exception ex)
            {
                //LogHelper.Error(string.Format("UserService.Save: {0}", ex.Message));
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// @author:duynn
        /// @description: lấy vai trò
        /// </summary>
        /// <param name="code">mã vai trò</param>
        /// <returns></returns>
        public DM_VAITRO GetRoleByCode(string code)
        {
            return this.repository.AllNoTracking.FirstOrDefault(x => x.MA_VAITRO == code);
        }

        public JsonResultBO checkExistCode(string code, long id = 0)
        {
            var result = new JsonResultBO(true);
            if (id > 0)
            {
                var exist = repository.All().Where(x => x.MA_VAITRO.ToUpper().Equals(code.ToUpper()) && x.DM_VAITRO_ID != id).Any();
                result.Status = exist;
            }
            else
            {
                var exist = repository.All().Where(x => x.MA_VAITRO.ToUpper().Equals(code.ToUpper())).Any();
                result.Status = exist;
            }
            return result;
        }

        public List<SelectListItem> DsVaiTro(List<int> lstVaiTro)
        {
            var query = this.context.DM_VAITRO.Select(x => new SelectListItem
            {
                Text = x.TEN_VAITRO,
                Value = x.DM_VAITRO_ID.ToString(),

            }).OrderBy(x => x.Text).ToList();
            foreach (var item in query)
            {
                item.Selected = lstVaiTro != null && lstVaiTro.Contains(int.Parse(item.Value));
            }
            return query;
        }
        public PageListResultBO<DM_VAITRO_BO> GetDaTaByPage(DM_VAITRO_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from tbl in this.context.DM_VAITRO
                        select new DM_VAITRO_BO
                        {
                            DM_VAITRO_ID = tbl.DM_VAITRO_ID,
                            MA_VAITRO = tbl.MA_VAITRO,
                            TEN_VAITRO = tbl.TEN_VAITRO,
                            TRANGTHAI = tbl.TRANGTHAI,
                            NGAYTAO = tbl.NGAYTAO,
                            NGAYSUA = tbl.NGAYSUA,
                            NGUOITAO = tbl.NGUOITAO,
                            NGUOISUA = tbl.NGUOISUA,
                            IS_DELETE = tbl.IS_DELETE,
                            TRONGSO = tbl.TRONGSO,
                            IS_RECEIVE_DOC_DIRECTLY = tbl.IS_RECEIVE_DOC_DIRECTLY
                        };
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.QR_MA))
                {
                    query = query.Where(x => x.MA_VAITRO.Contains(searchModel.QR_MA));
                }

                if (!string.IsNullOrEmpty(searchModel.QR_VAITRO))
                {
                    query = query.Where(x => x.TEN_VAITRO.Contains(searchModel.QR_VAITRO));
                }

                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.DM_VAITRO_ID);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.DM_VAITRO_ID);
            }
            var resultmodel = new PageListResultBO<DM_VAITRO_BO>();
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
        public DM_VAITRO_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.DM_VAITRO
                        where tbl.DM_VAITRO_ID == ID
                        select new DM_VAITRO_BO
                        {
                            DM_VAITRO_ID = tbl.DM_VAITRO_ID,
                            MA_VAITRO = tbl.MA_VAITRO,
                            TEN_VAITRO = tbl.TEN_VAITRO,
                            TRANGTHAI = tbl.TRANGTHAI,
                            NGAYTAO = tbl.NGAYTAO,
                            NGAYSUA = tbl.NGAYSUA,
                            NGUOITAO = tbl.NGUOITAO,
                            NGUOISUA = tbl.NGUOISUA,
                            IS_DELETE = tbl.IS_DELETE,
                        };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }
        public List<DM_VAITRO_BO> GetListUserInRole(int? RoleID = 0)
        {
            var query = from ndvt in this.context.NGUOIDUNG_VAITRO
                        join nd in this.context.DM_NGUOIDUNG
                        on ndvt.NGUOIDUNG_ID equals nd.ID
                        into group1
                        from g1 in group1.DefaultIfEmpty()

                        join dept in this.context.CCTC_THANHPHAN
                        on g1.DM_PHONGBAN_ID equals dept.ID
                        select new DM_VAITRO_BO
                        {
                            USERID = g1.ID,
                            HOVATEN = g1.HOTEN + "(" +g1.TENDANGNHAP+ ")",
                            DM_VAITRO_ID = (ndvt.VAITRO_ID != null && ndvt.VAITRO_ID.HasValue) ? ndvt.VAITRO_ID.Value : 0,
                            NGUOIDUNG_VAITRO_ID = ndvt.ID,
                            DepartmentName = dept.NAME
                        };
            if (RoleID > 0)
            {
                query = query.Where(o => RoleID == o.DM_VAITRO_ID);
            }
            return query.ToList();
        }
        public List<SelectListItem> ListUserNotInRole(int RoleID)
        {
            List<int> UserInRole = this.context.NGUOIDUNG_VAITRO.Where(o => o.VAITRO_ID == RoleID).Select(o => (int)o.NGUOIDUNG_ID).ToList();
            List<SelectListItem> ListUserNotInRole = this.context.DM_NGUOIDUNG.Where(o => !UserInRole.Contains((int)o.ID)).Select(s => new SelectListItem
            {
                Text = s.HOTEN,
                Value = s.ID.ToString(),
            }).ToList();
            return ListUserNotInRole;
        }


        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách vai trò có thể nhận gửi đích danh văn bản
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetRolesReceiveDirectly()
        {
            List<SelectListItem> result = (from role in this.context.DM_VAITRO
                                           .Where(x => x.IS_DELETE != true && x.IS_RECEIVE_DOC_DIRECTLY == true)
                                           select new SelectListItem()
                                           {
                                               Value = role.DM_VAITRO_ID.ToString(),
                                               Text = role.TEN_VAITRO
                                           }).ToList();
            return result;
        }
        public List<SelectListItem> ListVaiTro(int iSelected = 0)
        {
            var query = this.context.DM_VAITRO.Select(s => new SelectListItem
            {
                Text = s.TEN_VAITRO,
                Value = s.DM_VAITRO_ID.ToString(),
            }).ToList();
            return query;
        }
    }
}

