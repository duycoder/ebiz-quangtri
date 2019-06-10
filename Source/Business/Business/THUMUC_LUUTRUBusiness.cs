using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.THUMUCLUUTRU;
using Model.Entities;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Business.Business
{
    public class THUMUC_LUUTRUBusiness : BaseBusiness<THUMUC_LUUTRU>
    {
        public THUMUC_LUUTRUBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public bool Save(List<THUMUC_LUUTRU> ListThuMuc)
        {
            try
            {
                foreach (var item in ListThuMuc)
                {
                    if (item.ID == 0)
                    {
                        this.repository.Insert(item);
                    }
                    else
                    {
                        this.repository.Update(item);
                    }
                }
                this.repository.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }
        public List<THUMUC_LUUTRU_BO> GetDataByPhong(long pID, int NamLuTru)
        {
            var result = (from thumuc in context.THUMUC_LUUTRU
                          join donvi in context.CCTC_THANHPHAN
                          on thumuc.DONVI_ID equals donvi.ID into group1
                          from g1 in group1.DefaultIfEmpty()
                          join C1 in context.DM_NGUOIDUNG
                          on thumuc.USER_ID equals C1.ID
                          into group2
                          from g2 in group2.DefaultIfEmpty()
                          join A2 in context.THUMUC_LUUTRU
                          on thumuc.PARENT_ID equals A2.ID
                          into group3
                          from g3 in group3.DefaultIfEmpty()
                          where (thumuc.USER_ID == pID) && thumuc.IS_DELETE.HasValue && !thumuc.IS_DELETE.Value
                          && ((thumuc.PARENT_ID.HasValue && thumuc.PARENT_ID == 0) || !thumuc.PARENT_ID.HasValue)
                          //&& thumuc.NAM == NamLuTru
                          orderby thumuc.TENTHUMUC
                          select new THUMUC_LUUTRU_BO
                          {
                              ID = thumuc.ID,
                              PARENT_ID = thumuc.PARENT_ID,
                              TEN_NGUOITAO = g2.HOTEN,
                              TENTHUMUC = thumuc.TENTHUMUC,
                              USER_ID = thumuc.USER_ID,
                              THUMUCCHA = g3.TENTHUMUC,
                              NGAYTAO = thumuc.NGAYTAO,
                              IS_DELETE = thumuc.IS_DELETE.Value,
                              NAM = thumuc.NAM,
                              DONVI_ID = thumuc.DONVI_ID,
                              IS_THUMUC = true
                          });
            return result.ToList();
        }
        public List<THUMUC_LUUTRU> GetChildren(long? ID)
        {
            var result = (from thumuc in this.context.THUMUC_LUUTRU
                          where thumuc.PARENT_ID == ID && thumuc.IS_DELETE == false
                          select thumuc);
            return result.ToList();
        }
        public PageListResultBO<THUMUC_LUUTRU_BO> GetDataByPage(THUMUC_LUUTRU_SEARCHBO searchModel, int page, int limit)
        {
            var result = from thumuc in this.context.THUMUC_LUUTRU
                         join donvi in context.CCTC_THANHPHAN
                          on thumuc.DONVI_ID equals donvi.ID into group1
                         from g1 in group1.DefaultIfEmpty()
                         join C1 in context.DM_NGUOIDUNG
                         on thumuc.USER_ID equals C1.ID
                         into group2
                         from g2 in group2.DefaultIfEmpty()
                         join A2 in context.THUMUC_LUUTRU
                         on thumuc.PARENT_ID equals A2.ID
                         into group3
                         from g3 in group3.DefaultIfEmpty()
                         where
                         //(searchModel.FOLDER_ID.HasValue ? (thumuc.PARENT_ID.HasValue && thumuc.PARENT_ID.Value == searchModel.FOLDER_ID) :
                         //((thumuc.PARENT_ID.HasValue && thumuc.PARENT_ID.Value == 0) || (!thumuc.PARENT_ID.HasValue)))
                         ((thumuc.IS_DELETE.HasValue && !thumuc.IS_DELETE.Value) || (!thumuc.IS_DELETE.HasValue))
                         select new THUMUC_LUUTRU_BO
                         {
                             ID = thumuc.ID,
                             PARENT_ID = thumuc.PARENT_ID,
                             TEN_NGUOITAO = g2.HOTEN,
                             TENTHUMUC = thumuc.TENTHUMUC,
                             USER_ID = thumuc.USER_ID,
                             THUMUCCHA = g3.TENTHUMUC,
                             NGAYTAO = thumuc.NGAYTAO,
                             IS_DELETE = thumuc.IS_DELETE.Value,
                             NAM = thumuc.NAM,
                             DONVI_ID = thumuc.DONVI_ID,
                             IS_THUMUC = true,
                             TEN_DONVI = g1.NAME,
                             CLASS = thumuc.CLASS,
                             PRIOVITY = thumuc.PRIOVITY,
                             ACCESS_MODIFIER = thumuc.ACCESS_MODIFIER,
                             IS_FIXED = thumuc.IS_FIXED,
                             NGAYXOA = thumuc.NGAYXOA,
                             PERMISSION = thumuc.PERMISSION,
                             SLTHUMUC = 0,
                             THUMUC_AO = thumuc.THUMUC_AO
                         };
            if (searchModel != null)
            {
                if (searchModel.USER_ID > 0)
                {
                    result = result.Where(x => x.USER_ID == searchModel.USER_ID);
                }
                if (searchModel.FOLDER_ID.HasValue && searchModel.FOLDER_ID.Value > 0)
                {
                    result = result.Where(x => (x.PARENT_ID.HasValue && x.PARENT_ID.Value == searchModel.FOLDER_ID));
                }
                else
                {
                    result = result.Where(x => (x.PARENT_ID.HasValue && x.PARENT_ID.Value == 0) || (!x.PARENT_ID.HasValue));
                    result = result.OrderBy(x => x.PRIOVITY);
                }
                result = result.OrderBy(x => x.TENTHUMUC);
                if (!string.IsNullOrEmpty(searchModel.TEN_THUMUC))
                {
                    result = result.Where(x => x.TENTHUMUC.ToLower().Contains(searchModel.TEN_THUMUC.ToLower()));
                }
                if (searchModel.FOLDER_PERMISSION.HasValue)
                {
                    result = result.Where(x => x.PERMISSION.HasValue && x.PERMISSION.Value == searchModel.FOLDER_PERMISSION);
                }
                if (searchModel.ACCESS_MODIFIER.HasValue)
                {
                    result = result.Where(x => x.ACCESS_MODIFIER.HasValue && x.ACCESS_MODIFIER.Value == searchModel.ACCESS_MODIFIER);
                }
            }
            else
            {
                result = result.Where(x => (x.PARENT_ID.HasValue && x.PARENT_ID.Value == 0) || (!x.PARENT_ID.HasValue));
                result = result.OrderBy(x => x.PRIOVITY);
            }
            var resultmodel = new PageListResultBO<THUMUC_LUUTRU_BO>();
            if (limit == -1)
            {
                var dataPageList = result.ToList();
                resultmodel.Count = dataPageList.Count;
                resultmodel.TotalPage = 1;
                resultmodel.ListItem = dataPageList;
            }
            else
            {
                var dataPageList = result.ToPagedList(page, limit);
                resultmodel.Count = dataPageList.TotalItemCount;
                resultmodel.TotalPage = dataPageList.PageCount;
                resultmodel.ListItem = dataPageList.ToList();
            }
            return resultmodel;
        }
        public bool CanInsert(string folderName, long parentId, long id, long userId)
        {
            var result = from thumuc in context.THUMUC_LUUTRU
                         where !string.IsNullOrEmpty(thumuc.TENTHUMUC)
                         && folderName.ToLower().Equals(thumuc.TENTHUMUC.ToLower())
                         && (id > 0 ? thumuc.ID != id : true)
                         && ((thumuc.IS_DELETE.HasValue && !thumuc.IS_DELETE.Value) || (!thumuc.IS_DELETE.HasValue))
                         && (parentId > 0 ? (thumuc.PARENT_ID.HasValue && thumuc.PARENT_ID.Value == parentId) : (!thumuc.PARENT_ID.HasValue || (thumuc.PARENT_ID.HasValue && thumuc.PARENT_ID.Value == 0)))
                         select thumuc;
            if (userId > 0)
            {
                result = result.Where(x => userId == x.USER_ID);
            }
            return result.ToList().Count == 0;
        }
        public List<THUMUC_LUUTRU> GetAllParent(long id)
        {
            var listItem = new List<THUMUC_LUUTRU>();
            var queue = new Queue();
            queue.Enqueue(id);
            while (queue.Count > 0)
            {
                var parent = (long?)queue.Dequeue();
                var item = this.context.THUMUC_LUUTRU.Where(x => x.ID == parent).ToList();
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
        public List<THUMUC_LUUTRU> GetAllChildren(long id)
        {
            var listItem = new List<THUMUC_LUUTRU>();
            var queue = new Queue();
            queue.Enqueue(id);
            while (queue.Count > 0)
            {
                var parent = (long)queue.Dequeue();
                var item = this.context.THUMUC_LUUTRU.Where(x => x.PARENT_ID == parent).ToList();
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
        public List<THUMUC_LUUTRU_BO> GetThuMucByParent(int pID, long uID, List<int> Ids)
        {
            var result = (from ThuMuc in this.context.THUMUC_LUUTRU
                          join donvi in this.context.CCTC_THANHPHAN
                          on ThuMuc.DONVI_ID equals donvi.ID into group1
                          from g1 in group1.DefaultIfEmpty()
                          join C1 in this.context.DM_NGUOIDUNG
                          on ThuMuc.USER_ID equals C1.ID
                          into group2
                          from g2 in group2.DefaultIfEmpty()
                              //join A2 in this.context.THUMUC_LUUTRU
                              //on ThuMuc.PARENT_ID equals A2.ID
                              //into group3
                              //from g3 in group3.DefaultIfEmpty()
                          where ThuMuc.PARENT_ID.HasValue == true &&
                          ThuMuc.PARENT_ID == pID && ThuMuc.IS_DELETE == false
                          orderby ThuMuc.TENTHUMUC
                          select new THUMUC_LUUTRU_BO
                          {
                              ID = ThuMuc.ID,
                              PARENT_ID = ThuMuc.PARENT_ID,
                              TEN_NGUOITAO = g2.HOTEN,
                              TENTHUMUC = ThuMuc.TENTHUMUC,
                              USER_ID = ThuMuc.USER_ID,
                              //THUMUCCHA = g3.TENTHUMUC,
                              NGAYTAO = ThuMuc.NGAYTAO,
                              IS_DELETE = ThuMuc.IS_DELETE.Value,
                              TEN_DONVI = g1.NAME,
                              IS_THUMUC = true,
                              DONVI_ID = ThuMuc.DONVI_ID,
                              ACCESS_MODIFIER = ThuMuc.ACCESS_MODIFIER,
                              PERMISSION = ThuMuc.PERMISSION,
                              IS_FIXED = ThuMuc.IS_FIXED,
                              NAM = ThuMuc.NAM,
                              THUMUC_AO = ThuMuc.THUMUC_AO,
                              CLASS = ThuMuc.CLASS,
                              NGAYXOA = ThuMuc.NGAYXOA,
                              PRIOVITY = ThuMuc.PRIOVITY,
                              SLTHUMUC = 0,
                              THUMUCCHA = ""
                          });
            if (uID > 0)
            {
                result = result.Where(x => x.USER_ID == uID);
            }
            if (Ids.Any())
            {
                result = result.Where(x => x.DONVI_ID.HasValue && Ids.Contains(x.DONVI_ID.Value));
            }
            return result.ToList();
        }
        public THUMUC_LUUTRU GetDataByNam(string name, long id)
        {
            var result = from thumuc in this.context.THUMUC_LUUTRU
                         where !string.IsNullOrEmpty(thumuc.THUMUC_AO) && thumuc.THUMUC_AO.Equals(name)
                         && thumuc.IS_FIXED.HasValue && thumuc.IS_FIXED.Value
                         && id == thumuc.FIXED_FOLDER_ID
                         select thumuc;
            return result.FirstOrDefault();
        }
        public PageListResultBO<THUMUC_LUUTRU_BO> GetTrashByPage(THUMUC_LUUTRU_SEARCHBO searchModel, int page, int limit)
        {
            var result = from thumuc in this.context.THUMUC_LUUTRU
                         join donvi in context.CCTC_THANHPHAN
                          on thumuc.DONVI_ID equals donvi.ID into group1
                         from g1 in group1.DefaultIfEmpty()
                         join C1 in context.DM_NGUOIDUNG
                         on thumuc.USER_ID equals C1.ID
                         into group2
                         from g2 in group2.DefaultIfEmpty()
                         join A2 in context.THUMUC_LUUTRU
                         on thumuc.PARENT_ID equals A2.ID
                         into group3
                         from g3 in group3.DefaultIfEmpty()
                         where
                         ((thumuc.IS_DELETE.HasValue && thumuc.IS_DELETE.Value))
                         && thumuc.USER_ID.HasValue && thumuc.USER_ID.Value == searchModel.USER_ID
                         select new THUMUC_LUUTRU_BO
                         {
                             ID = thumuc.ID,
                             PARENT_ID = thumuc.PARENT_ID,
                             TEN_NGUOITAO = g2.HOTEN,
                             TENTHUMUC = thumuc.TENTHUMUC,
                             USER_ID = thumuc.USER_ID,
                             THUMUCCHA = g3.TENTHUMUC,
                             NGAYTAO = thumuc.NGAYTAO,
                             IS_DELETE = thumuc.IS_DELETE.Value,
                             NAM = thumuc.NAM,
                             DONVI_ID = thumuc.DONVI_ID,
                             IS_THUMUC = true,
                             TEN_DONVI = g1.NAME
                         };
            if (searchModel != null)
            {
                if (searchModel.FOLDER_ID.HasValue && searchModel.FOLDER_ID.Value > 0)
                {
                    result = result.Where(x => (x.PARENT_ID.HasValue && x.PARENT_ID.Value == searchModel.FOLDER_ID));
                }
                else
                {
                    result = result.Where(x => (x.PARENT_ID.HasValue && x.PARENT_ID.Value == 0) || (!x.PARENT_ID.HasValue));
                }
                result = result.OrderBy(x => x.TENTHUMUC);
                if (!string.IsNullOrEmpty(searchModel.TEN_THUMUC))
                {
                    result = result.Where(x => x.TENTHUMUC.ToLower().Contains(searchModel.TEN_THUMUC.ToLower()));
                }
                if (searchModel.FOLDER_PERMISSION.HasValue)
                {
                    result = result.Where(x => x.PERMISSION.HasValue && x.PERMISSION.Value == searchModel.FOLDER_PERMISSION);
                }
                if (searchModel.ACCESS_MODIFIER.HasValue)
                {
                    result = result.Where(x => x.ACCESS_MODIFIER.HasValue && x.ACCESS_MODIFIER.Value == searchModel.ACCESS_MODIFIER);
                }
            }
            else
            {
                result = result.Where(x => (x.PARENT_ID.HasValue && x.PARENT_ID.Value == 0) || (!x.PARENT_ID.HasValue));
                result = result.OrderBy(x => x.TENTHUMUC);
            }
            var resultmodel = new PageListResultBO<THUMUC_LUUTRU_BO>();
            if (limit == -1)
            {
                var dataPageList = result.ToList();
                resultmodel.Count = dataPageList.Count;
                resultmodel.TotalPage = 1;
                resultmodel.ListItem = dataPageList;
            }
            else
            {
                var dataPageList = result.ToPagedList(page, limit);
                resultmodel.Count = dataPageList.TotalItemCount;
                resultmodel.TotalPage = dataPageList.PageCount;
                resultmodel.ListItem = dataPageList.ToList();
            }
            return resultmodel;
        }
        public List<long> GetIdsByUserId(long id, long userId)
        {
            var listItem = new List<long>();
            var queue = new Queue();
            queue.Enqueue(id);
            while (queue.Count > 0)
            {
                var parent = (long)queue.Dequeue();
                var item = this.context.THUMUC_LUUTRU.Where(x => x.PARENT_ID == parent && x.USER_ID.HasValue && x.USER_ID.Value == userId
                && x.HAS_FILES.HasValue && x.HAS_FILES.Value && ((x.IS_DELETE.HasValue && !x.IS_DELETE.Value) || (!x.IS_DELETE.HasValue))).ToList();
                if (item.Count > 0)
                {
                    listItem.AddRange(item.Select(x => x.ID));
                    foreach (var cc in item)
                    {
                        queue.Enqueue(cc.ID);
                    }
                }
            }
            return listItem;
        }


        public PageListResultBO<THUMUC_LUUTRU_BO> GetFileByPage(THUMUC_LUUTRU_SEARCHBO searchModel, int page, int limit, string TENDONVI, THUMUC_LUUTRU ThuMuc)
        {
            var result = from tailieu in this.context.TAILIEUDINHKEM
                             //join donvi in context.CCTC_THANHPHAN
                             // on tailieu.DONVI_ID equals donvi.ID into group1
                             //from g1 in group1.DefaultIfEmpty()
                         join C1 in context.DM_NGUOIDUNG
                         on tailieu.USER_ID equals C1.ID
                         into group2
                         from g2 in group2.DefaultIfEmpty()
                         where
                         ((tailieu.IS_DELETE.HasValue && !tailieu.IS_DELETE.Value) || (!tailieu.IS_DELETE.HasValue))
                         select new THUMUC_LUUTRU_BO
                         {
                             TENTHUMUC = tailieu.TENTAILIEU,
                             SLTHUMUC = 0,
                             ID = (long)tailieu.TAILIEU_ID,
                             NGAYTAO = tailieu.NGAYTAO,
                             IS_THUMUC = false,
                             THUMUCCHA = tailieu.DINHDANG_FILE,
                             TEN_NGUOITAO = g2.HOTEN,
                             TEN_DONVI = TENDONVI,
                             DONVI_ID = tailieu.IS_PHEDUYET,
                             USER_ID = tailieu.USER_ID,
                             ACCESS_MODIFIER = ThuMuc.ACCESS_MODIFIER,
                             PERMISSION = ThuMuc.PERMISSION,
                             PARENT_ID = tailieu.FOLDER_ID
                         };
            if (searchModel != null)
            {
                if (searchModel.USER_ID > 0)
                {
                    result = result.Where(x => x.USER_ID == searchModel.USER_ID);
                }
                if (searchModel.FOLDER_ID.HasValue && searchModel.FOLDER_ID.Value > 0)
                {
                    result = result.Where(x => (x.PARENT_ID.HasValue && x.PARENT_ID.Value == searchModel.FOLDER_ID));
                }
                else
                {
                    result = result.Where(x => (x.PARENT_ID.HasValue && x.PARENT_ID.Value == 0) || (!x.PARENT_ID.HasValue));
                }
                result = result.OrderBy(x => x.TENTHUMUC);
                if (!string.IsNullOrEmpty(searchModel.TEN_THUMUC))
                {
                    result = result.Where(x => x.TENTHUMUC.ToLower().Contains(searchModel.TEN_THUMUC.ToLower()));
                }
                if (searchModel.FOLDER_PERMISSION.HasValue)
                {
                    result = result.Where(x => x.PERMISSION.HasValue && x.PERMISSION.Value == searchModel.FOLDER_PERMISSION);
                }
                if (searchModel.ACCESS_MODIFIER.HasValue)
                {
                    result = result.Where(x => x.ACCESS_MODIFIER.HasValue && x.ACCESS_MODIFIER.Value == searchModel.ACCESS_MODIFIER);
                }
            }
            else
            {
                result = result.Where(x => (x.PARENT_ID.HasValue && x.PARENT_ID.Value == 0) || (!x.PARENT_ID.HasValue));
                result = result.OrderBy(x => x.PRIOVITY);
            }
            var resultmodel = new PageListResultBO<THUMUC_LUUTRU_BO>();
            if (limit == -1)
            {
                var dataPageList = result.ToList();
                resultmodel.Count = dataPageList.Count;
                resultmodel.TotalPage = 1;
                resultmodel.ListItem = dataPageList;
            }
            else
            {
                var dataPageList = result.ToPagedList(page, limit);
                resultmodel.Count = dataPageList.TotalItemCount;
                resultmodel.TotalPage = dataPageList.PageCount;
                resultmodel.ListItem = dataPageList.ToList();
            }
            return resultmodel;
        }
    }
}

