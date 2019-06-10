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
using Business.CommonModel.CCTCTHANHPHAN;
using System.Collections;
using System.Web.Mvc;
using System.Web;

namespace Business.Business
{
    public class CCTC_THANHPHANBusiness : BaseBusiness<CCTC_THANHPHAN>
    {
        public CCTC_THANHPHANBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        private List<CCTCItemTreeBO> ListAll = new List<CCTCItemTreeBO>();
        public JsonResultBO saveImport(List<CCTC_THANHPHAN> lstObj)
        {
            var result = new JsonResultBO(true);
            using (var transaction = repository.Context.Database.BeginTransaction())
            {
                try
                {

                    repository.Context.CCTC_THANHPHAN.AddRange(lstObj);
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
        public bool ExistCode(string code, int id = 0)
        {
            var obj = this.context.CCTC_THANHPHAN.Where(x => x.CODE == code).FirstOrDefault();
            if (obj == null)
            {
                return false;
            }
            else
            {
                if (id > 0 && obj.ID == id)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool ExistChild(int id)
        {
            var lst = this.context.CCTC_THANHPHAN.Where(x => x.PARENT_ID == id).Count();
            if (lst > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Save(CCTC_THANHPHAN thanhphan)
        {
            try
            {
                if (thanhphan.ID == 0)
                {
                    thanhphan.IS_DELETE = false;
                    this.repository.Insert(thanhphan);
                }
                else
                    this.repository.Update(thanhphan);

                this.repository.Save();
            }
            catch (Exception ex)
            {
                //LogHelper.Error(string.Format("UserService.Save: {0}", ex.Message));
                throw new Exception(ex.Message);
            }
        }

        public List<CCTC_THANHPHAN> GetAllByLeVelUp(int level = 0)
        {
            var lstData = this.context.CCTC_THANHPHAN.Where(x => x.IS_DELETE != true);
            if (level > 0)
            {
                lstData = lstData.Where(x => x.ITEM_LEVEL <= level).OrderBy(x => x.ITEM_LEVEL);
                return lstData.ToList();
            }
            else
            {
                return lstData.OrderBy(x => x.ITEM_LEVEL).ToList();
            }
        }

        public List<CCTC_THANHPHAN> GetDSChild(int id)
        {
            var listItem = new List<CCTC_THANHPHAN>();
            var queue = new Queue();
            queue.Enqueue(id);
            while (queue.Count > 0)
            {
                var parent = (int)queue.Dequeue();
                var item = this.context.CCTC_THANHPHAN.AsNoTracking().Where(x => x.PARENT_ID == parent).ToList();
                if (item.Count > 0)
                {
                    listItem.AddRange(item);
                    foreach (var cc in item)
                    {
                        queue.Enqueue(cc.ID);
                    }
                }
            }

            return listItem;
        }
        public void GetChildLabel(ref CCTCItemTreeBO Node, int deptId)
        {
            var id = Node.ID;
            var lstChild = new List<CCTCItemTreeBO>();
            if (deptId == 1)
            {
                lstChild = ListAll.Where(x => x.PARENT_ID == id).ToList();
            }
            else
            {
                lstChild = ListAll.Where(x => (x.PARENT_ID == id && (x.TYPE == 10 || x.TYPE == 11))).ToList();
            }
            
            if (lstChild.Count > 0)
            {
                if (Node.Child == null)
                {
                    Node.Child = new List<CCTCItemTreeBO>();
                }
                Node.Child.AddRange(lstChild);
                var count = lstChild.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = Node.Child[i];
                    if (item.ID == deptId)
                    {
                        var subChild = ListAll.Where(x => x.PARENT_ID == deptId && x.TYPE != 10);
                        if (subChild.Any())
                        {
                            item.Child = new List<CCTCItemTreeBO>();
                            item.Child.AddRange(subChild);
                        }
                    }
                    
                    GetChildLabel(ref item, deptId == item.ID ? item.ID : 0);
                    Node.Child[i] = item;
                }
            }
        }

        public void GetChildLabel(ref CCTCItemTreeBO Node, int deptId, List<int> idsExclude)
        {
            var id = Node.ID;
            var lstChild = new List<CCTCItemTreeBO>();
            if (deptId == 1)
            {
                lstChild = ListAll.Where(x => x.PARENT_ID == id && idsExclude.Contains(x.ID) == false).ToList();
            }
            else
            {
                lstChild = ListAll.Where(x => (x.PARENT_ID == id && x.TYPE == 10 && idsExclude.Contains(x.ID) == false)).ToList();
            }

            if (lstChild.Count > 0)
            {
                if (Node.Child == null)
                {
                    Node.Child = new List<CCTCItemTreeBO>();
                }
                Node.Child.AddRange(lstChild);
                for (int i = 0; i < lstChild.Count; i++)
                {
                    var item = Node.Child[i];
                    if (item.ID == deptId)
                    {
                        var subChild = ListAll.Where(x => x.PARENT_ID == deptId && x.TYPE != 10 && idsExclude.Contains(x.ID) == false).ToList();
                        if (subChild.Count > 0)
                        {
                            item.Child = new List<CCTCItemTreeBO>();
                            item.Child.AddRange(subChild);
                        }
                    }

                    //if (item.TYPE == 10)
                    //{
                    GetChildLabel(ref item, deptId == item.ID ? item.ID : 0, idsExclude);
                    Node.Child[i] = item;
                    //}
                }
            }
        }
        public void GetChild(ref CCTCItemTreeBO Node)
        {
            var id = Node.ID;
            var lstChild = ListAll.Where(x => x.PARENT_ID == id).OrderBy(x => x.THUTU)
                .ThenByDescending(x=>x.TYPE).ThenByDescending(x=>x.NAME).ToList();
            if (lstChild.Count > 0)
            {
                Node.Child = new List<CCTCItemTreeBO>();
                Node.Child.AddRange(lstChild);
                for (int i = 0; i < lstChild.Count; i++)
                {
                    var item = Node.Child[i];
                    GetChild(ref item);
                    Node.Child[i] = item;
                }
            }
        }

        public List<CCTCItemTreeBO> GetAll(List<int> idsExclude)
        {
            ListAll.Clear();
            ListAll = (from item in this.context.CCTC_THANHPHAN
                       where item.IS_DELETE != true
                       select new CCTCItemTreeBO
                       {
                           ID = item.ID,
                           IS_DELETE = item.IS_DELETE.HasValue ? item.IS_DELETE : false,
                           ITEM_LEVEL = item.ITEM_LEVEL,
                           NAME = item.NAME,
                           NGAYSUA = item.NGAYSUA,
                           NGAYTAO = item.NGAYTAO,
                           NGUOISUA = item.NGUOISUA,
                           NGUOITAO = item.NGUOITAO,
                           PARENT_ID = item.PARENT_ID,
                           TYPE = item.TYPE,
                           CODE = item.CODE,
                           THUTU = item.THUTU,
                           IS_ALLOW_SELECT = !idsExclude.Contains(item.ID)
                       }).ToList();
            return ListAll;
        }

        public List<CCTCItemTreeBO> GetAll(int excludeid = 0)
        {
            ListAll.Clear();
            ListAll = (from item in this.context.CCTC_THANHPHAN.AsNoTracking()
                       where item.IS_DELETE != true
                       select new CCTCItemTreeBO
                       {
                           ID = item.ID,
                           IS_DELETE = item.IS_DELETE.HasValue ? item.IS_DELETE : false,
                           ITEM_LEVEL = item.ITEM_LEVEL,
                           NAME = item.NAME,
                           NGAYSUA = item.NGAYSUA,
                           NGAYTAO = item.NGAYTAO,
                           NGUOISUA = item.NGUOISUA,
                           NGUOITAO = item.NGUOITAO,
                           PARENT_ID = item.PARENT_ID,
                           TYPE = item.TYPE,
                           CODE = item.CODE,
                           THUTU = item.THUTU,
                           IS_ALLOW_SELECT = true
                       }).ToList();
            if (excludeid != 0)
            {
                ListAll = ListAll.Where(x => x.ID != excludeid).ToList();
            }
            return ListAll;
        }

        public CCTCItemTreeBO GetTreeLabel(UserInfoBO user)
        {
            GetAll();
            CCTCItemTreeBO ListRoot = new CCTCItemTreeBO();
            ListRoot = ListAll.Where(x => x.PARENT_ID == null).FirstOrDefault();
            if (ListRoot != null)
            {
                var item = ListRoot;
                GetChildLabel(ref ListRoot, 1);
                ListRoot = item;
            }
            return ListRoot;
        }

        public CCTCItemTreeBO GetTreeLabel(UserInfoBO user, List<int> idsExclude)
        {
            GetAll(idsExclude);
            CCTCItemTreeBO ListRoot = new CCTCItemTreeBO();
            ListRoot = ListAll.Where(x => x.PARENT_ID == null).FirstOrDefault();
            if (ListRoot != null)
            {
                var item = ListRoot;
                GetChildLabel(ref ListRoot, 1);
                ListRoot = item;
            }
            return ListRoot;
        }

        public CCTCItemTreeBO GetTree(int dvID = 0)
        {
            GetAll();
            CCTCItemTreeBO ListRoot = new CCTCItemTreeBO();
            if (dvID > 0)
            {
                ListRoot = ListAll.Where(x => x.ID == dvID).FirstOrDefault();
                if (ListRoot != null)
                {
                    var item = ListRoot;
                    GetChild(ref ListRoot);
                    ListRoot = item;
                }
            }
            else
            {
                ListRoot = ListAll.Where(x => x.PARENT_ID == null).FirstOrDefault();
                if (ListRoot != null)
                {
                    var item = ListRoot;
                    GetChild(ref ListRoot);
                    ListRoot = item;
                }
            }

            return ListRoot;
        }
        public List<SelectListItem> GetDropDownListByListSelect(List<int> lstitem)
        {
            List<SelectListItem> listResult = this.repository.All().Where(x => x.IS_DELETE != true && lstitem.Contains(x.ID))
                .OrderBy(x => x.ITEM_LEVEL).ThenBy(x => x.ID)
                .Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.NAME,
                    //Selected = (x.ID == selected)
                }).ToList();
            return listResult;
        }
        public List<SelectListItem> GetDropDownList(int selected = 0)
        {
            List<SelectListItem> listResult = this.repository.All().Where(x => x.IS_DELETE != true)
                .OrderBy(x => x.ITEM_LEVEL).ThenBy(x => x.ID)
                .Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.NAME,
                    Selected = (x.ID == selected)
                }).ToList();
            return listResult;
        }
        public List<CCTC_THANHPHAN> GetDSParent(int id)
        {
            var listItem = new List<CCTC_THANHPHAN>();
            var queue = new Queue();
            queue.Enqueue(id);
            while (queue.Count > 0)
            {
                var parent = (int?)queue.Dequeue();
                var item = this.context.CCTC_THANHPHAN.Where(x => x.ID == parent).ToList();
                if (item.Count > 0)
                {
                    listItem.AddRange(item);
                    foreach (var cc in item)
                    {
                        queue.Enqueue(cc.PARENT_ID);
                    }
                }
            }
            return listItem;
        }
        public CCTC_THANHPHAN GetFirstParent()
        {
            var result = from donvi in this.context.CCTC_THANHPHAN.Where(x => !x.PARENT_ID.HasValue || (x.PARENT_ID.Value == 0))
                         select donvi;
            return result.FirstOrDefault();
        }
        public List<CCTC_THANHPHAN> GetDataByIds(List<int> ids)
        {
            var result = from donvi in this.context.CCTC_THANHPHAN
                         where ids.Contains(donvi.ID)
                         select donvi;
            return result.ToList();
        }
        public List<CCTC_THANHPHAN> GetData()
        {
            var result = from donvi in this.context.CCTC_THANHPHAN.AsNoTracking()
                         where false == donvi.IS_DELETE || donvi.IS_DELETE == null
                         select donvi;
            return result.ToList();
        }
        public List<SelectListItem> GetDataByParent(int parentId, int selected)
        {
            List<SelectListItem> listResult = this.repository.All().Where(x => x.IS_DELETE != true && parentId == x.PARENT_ID)
                .OrderBy(x => x.ITEM_LEVEL).ThenBy(x => x.ID)
                .Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.NAME,
                    Selected = (x.ID == selected)
                }).ToList();
            return listResult;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy toàn bộ danh sách
        /// @since: 14/08/2018
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public List<SelectListItem> GetDropDownData(List<int> listSelected)
        {
            List<SelectListItem> result = this.repository.All().Where(x => x.IS_DELETE != true)
                .OrderBy(x => x.ITEM_LEVEL).ThenBy(x => x.ID)
                .Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.NAME,
                    Selected = listSelected.Contains(x.ID)
                }).ToList();
            return result;
        }


        public List<SelectListItem> GetHierarchicalData(List<int> listSelected)
        {
            List<CCTC_THANHPHAN> ouput = new List<CCTC_THANHPHAN>();
            List<CCTC_THANHPHAN> allItems = this.context.CCTC_THANHPHAN.Where(x => x.IS_DELETE != true).ToList();
            IEnumerable<CCTC_THANHPHAN> parents = allItems.Where(x => x.PARENT_ID == 0 || x.PARENT_ID == null);
            foreach (var item in parents)
            {
                SetHierachicalName(item, allItems, ouput);
            }

            List<SelectListItem> result = ouput.Select(x => new SelectListItem()
            {
                Value = x.ID.ToString(),
                Text = x.NAME,
                Selected = listSelected.Contains(x.ID)
            }).ToList();
            return result;
        }

        public void SetHierachicalName(CCTC_THANHPHAN item, IEnumerable<CCTC_THANHPHAN> list, List<CCTC_THANHPHAN> output, int times = 0)
        {
            int i = 0;
            string padding = string.Empty;
            while (i < times)
            {
                padding += "|-";
                i++;
            }
            item.NAME = padding + item.NAME;

            output.Add(item);

            var children = list.Where(x => x.PARENT_ID == item.ID);
            if (children.Count() > 0)
            {
                foreach (var child in children)
                {
                    times++;
                    SetHierachicalName(child, list, output, times);
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy các phòng ban của tỉnh ủy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<SelectListItem> GetChildrenOfDepartments(int deptId, int parentDeptId, UserInfoBO user)
        {
            //kiểm tra người dùng có thể xem báo cáo toàn hệ thống
            DM_THAOTAC viewSystemReport = user.ListThaoTac.Where(o => o.MA_THAOTAC.ToUpper() == "XEM_BAOCAO_HETHONG").FirstOrDefault();
            List<SelectListItem> result = new List<SelectListItem>();
            List<CCTC_THANHPHAN> output = new List<CCTC_THANHPHAN>();
            List<CCTC_THANHPHAN> allDeparments = this.context.CCTC_THANHPHAN.Where(x => x.IS_DELETE != true).ToList();
            CCTC_THANHPHAN department = this.context.CCTC_THANHPHAN.Find(deptId);

            //cùng cha cùng type
            var sameParentsAndTypes = allDeparments.Where(x => x.PARENT_ID == parentDeptId && x.CATEGORY == department.CATEGORY)
                .Select(x => new SelectListItem()
                {
                    Text = x.NAME,
                    Value = x.ID.ToString(),
                }).ToList();
            result.AddRange(sameParentsAndTypes);

            //lấy các phòng ban có "TYPE = 10" khi người dùng có quyền xem báo cáo của hệ thống
            if(viewSystemReport != null)
            {
                List<SelectListItem> depts = this.context.CCTC_THANHPHAN.Where(x => x.TYPE == 10)
                    .Select(x => new SelectListItem()
                    {
                        Text = x.NAME,
                        Value = x.ID.ToString(),
                    }).ToList();
                result.AddRange(depts);
            }

            var sameParentsWithLabel = allDeparments.Where(x => x.PARENT_ID == parentDeptId && x.TYPE == 10).ToList();
            foreach (var item in sameParentsWithLabel)
            {
                SetHierarchicalNames(item, allDeparments, output);
            }

            result.AddRange(output.Select(x => new SelectListItem()
            {
                Text = x.NAME,
                Value = x.ID.ToString(),
            }));
            result = result.GroupBy(x => x.Value).Select(x => x.FirstOrDefault()).ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy tên phân cấp của cơ quan tổ chức
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void SetHierarchicalNames(CCTC_THANHPHAN item, IEnumerable<CCTC_THANHPHAN> list, List<CCTC_THANHPHAN> output, int times = 0)
        {
            int i = 0;
            string padding = string.Empty;
            while (i < times)
            {
                padding += "|--";
                i++;
            }
            item.NAME = padding + item.NAME;
            output.Add(item);
            var children = list.Where(x => x.TYPE == 10 && x.PARENT_ID == item.ID).ToList();
            if (children.Count() > 0)
            {
                times++;
                foreach (var child in children)
                {
                    SetHierarchicalNames(child, list, output, times);
                }
            }
            else
            {
                return;
            }
        }
        public List<SelectListItem> GetDropDownListByCode(int selected = 0)
        {
            List<SelectListItem> listResult = this.repository.All().Where(x => x.IS_DELETE != true && x.TYPE != 10 && x.TYPE != 11)
            //List<SelectListItem> listResult = this.repository.All().Where(x => x.IS_DELETE != true)
                .OrderBy(x => x.ITEM_LEVEL).ThenBy(x => x.ID)
                .Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.CODE,
                    Selected = (x.ID == selected)
                }).ToList();
            return listResult;
        }
    }


}

