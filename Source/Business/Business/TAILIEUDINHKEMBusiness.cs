using Business.BaseBusiness;
using Business.CommonModel.TAILIEUDINHKEM;
using Business.CommonModel.THUMUCLUUTRU;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Business
{
    public class TAILIEUDINHKEMBusiness : BaseBusiness<TAILIEUDINHKEM>
    {
        public TAILIEUDINHKEMBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public bool Save(TAILIEUDINHKEM TAILIEU)
        {
            try
            {
                if (TAILIEU.TAILIEU_ID == 0)
                {
                    this.repository.Insert(TAILIEU);
                }
                else
                {
                    this.repository.Update(TAILIEU);
                }
                this.repository.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                // return false;
            }
        }
        public bool Save(List<TAILIEUDINHKEM> ListTaiLieu)
        {
            try
            {
                foreach (var item in ListTaiLieu)
                {
                    if (item.TAILIEU_ID == 0)
                    {
                        this.repository.Insert(item);
                    }
                    else
                    {
                        this.repository.Update(item);
                    }
                }
                repository.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                // return false;
            }
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy thông tin tài liệu đính kèm
        /// </summary>
        /// <param name="ITEM_ID"></param>
        /// <param name="LOAITAI_LIEU"></param>
        /// <returns></returns>
        public List<TAILIEUDINHKEM_BO> GetData(long ITEM_ID, int LOAITAI_LIEU)
        {
            var result = (from tailieu in this.context.TAILIEUDINHKEM
                          where tailieu.ITEM_ID == ITEM_ID && tailieu.LOAI_TAILIEU == LOAITAI_LIEU
                          join user in this.context.DM_NGUOIDUNG

                          on tailieu.USER_ID equals user.ID
                          into group1
                          from g1 in group1.DefaultIfEmpty()
                          orderby tailieu.TENTAILIEU
                          select new TAILIEUDINHKEM_BO()
                          {
                              TAILIEU_ID = tailieu.TAILIEU_ID,
                              TAILIEU_GOC_ID = tailieu.TAILIEU_GOC_ID,
                              TENTAILIEU = tailieu.TENTAILIEU,
                              NGAYTAO = tailieu.NGAYTAO,
                              TENTACGIA = g1.HOTEN
                          }).ToList();
            return result;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy các tài liệu mới được thêm
        /// </summary>
        /// <param name="ITEM_ID"></param>
        /// <param name="LOAITAI_LIEU"></param>
        /// <returns></returns>
        public List<TAILIEUDINHKEM> GetInsertedData(long ITEM_ID, int LOAITAI_LIEU)
        {
            var result = from tailieu in this.context.TAILIEUDINHKEM
                         where tailieu.ITEM_ID == ITEM_ID && tailieu.LOAI_TAILIEU == LOAITAI_LIEU
                         && tailieu.IS_DELETE != true && tailieu.TAILIEU_GOC_ID == null
                         orderby tailieu.TENTAILIEU
                         select tailieu;
            return result.ToList();
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy tài liệu mới nhất
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public List<TAILIEUDINHKEM> GetNewestData(long itemId, int itemType)
        {
            var result = (from tailieu in this.context.TAILIEUDINHKEM
                          where tailieu.ITEM_ID == itemId && tailieu.LOAI_TAILIEU == itemType
                          && tailieu.IS_ACTIVE == 1 && tailieu.IS_DELETE != true
                          orderby tailieu.TAILIEU_ID descending
                          group tailieu by tailieu.TAILIEU_GOC_ID
                          into group1
                          select group1.FirstOrDefault()).ToList();
            return result;
        }

        public List<TAILIEUDINHKEM> GetDataByItemID(long ITEM_ID, int LOAITAI_LIEU)
        {
            var result = from tailieu in this.context.TAILIEUDINHKEM
                         where tailieu.ITEM_ID == ITEM_ID && tailieu.LOAI_TAILIEU == LOAITAI_LIEU
                         orderby tailieu.TENTAILIEU
                         select tailieu;
            return result.ToList();
        }

        public List<TAILIEUDINHKEM> GetDataByItemID(List<long> Ids, int LOAITAI_LIEU)
        {
            var result = from tailieu in this.context.TAILIEUDINHKEM
                         where tailieu.ITEM_ID.HasValue && Ids.Contains(tailieu.ITEM_ID.Value)
                         && tailieu.LOAI_TAILIEU == LOAITAI_LIEU
                         orderby tailieu.TENTAILIEU
                         select tailieu;
            return result.ToList();
        }
        public List<THUMUC_LUUTRU_BO> getListFileByFolder(long ITEM_ID, int LOAITAI_LIEU, long userId,
            string TENDONVI, THUMUC_LUUTRU ThuMuc)
        {

            var result = from tailieu in this.context.TAILIEUDINHKEM
                         join nguoidung in this.context.DM_NGUOIDUNG
                         on tailieu.USER_ID equals nguoidung.ID
                         into group1
                         from g1 in group1.DefaultIfEmpty()
                         where tailieu.ITEM_ID == ITEM_ID && tailieu.LOAI_TAILIEU == LOAITAI_LIEU
                         && (!tailieu.IS_DELETE.HasValue || !tailieu.IS_DELETE.Value)
                         orderby tailieu.TENTAILIEU
                         select new THUMUC_LUUTRU_BO
                         {
                             TENTHUMUC = tailieu.TENTAILIEU,
                             SLTHUMUC = 0,
                             ID = (long)tailieu.TAILIEU_ID,
                             NGAYTAO = tailieu.NGAYTAO,
                             IS_THUMUC = false,
                             THUMUCCHA = tailieu.DINHDANG_FILE,
                             TEN_NGUOITAO = g1.HOTEN,
                             TEN_DONVI = TENDONVI,
                             DONVI_ID = tailieu.IS_PHEDUYET,
                             USER_ID = tailieu.USER_ID,
                             ACCESS_MODIFIER = ThuMuc.ACCESS_MODIFIER,
                             PERMISSION = ThuMuc.PERMISSION
                         };
            if (userId > 0)
            {
                result = result.Where(x => userId == x.USER_ID);
            }
            return result.ToList();
        }
        public long CountStorageSize(List<long> Ids)
        {
            var result = (from tailieu in this.context.TAILIEUDINHKEM
                          where tailieu.FOLDER_ID.HasValue && tailieu.IS_DELETE.HasValue && !tailieu.IS_DELETE.Value
                          && Ids.Contains(tailieu.FOLDER_ID.Value)
                          && tailieu.KICHCO.HasValue
                          select tailieu.KICHCO).ToList();
            return result.Sum(x => x.Value);
        }
        public List<TAILIEUDINHKEM> GetDataForTaskByListItemId(List<long> LstItemId, int LOAITAI_LIEU)
        {
            var result = from tailieu in this.context.TAILIEUDINHKEM
                where LstItemId.Contains(tailieu.ITEM_ID.Value) && tailieu.LOAI_TAILIEU == LOAITAI_LIEU
                orderby tailieu.TENTAILIEU
                select tailieu;
            return result.ToList();
        }

        public void Delete(object id)
        {
            this.repository.Delete(id);
            this.repository.Save();
        }

        /// <summary>
        /// @author: duynn
        /// @description: cập nhật nhiều tài liệu
        /// </summary>
        /// <param name="attachments"></param>
        public void UpdateRange(List<TAILIEUDINHKEM> attachments)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    foreach(var att in attachments)
                    {
                        if(att.TAILIEU_ID == 0)
                        {
                            this.repository.Insert(att);
                        }
                        else
                        {
                            this.repository.Update(att);
                        }
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }
    }
}
