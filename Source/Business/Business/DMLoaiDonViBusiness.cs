using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.CommonBusiness;
using System.Web.Mvc;
using Business.BaseBusiness;
namespace Business.Business
{
    public class DMLoaiDonViBusiness : BaseBusiness<DM_LOAI_DONVI>
    {
        public DMLoaiDonViBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public List<SelectListItem> GetDropDownList(int selected = 0)
        {
            var listResult = this.repository.All().OrderBy(x => x.LOAI)
                .Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.LOAI,
                    Selected = (x.ID == selected)
                }).ToList();
            return listResult;
        }
        public void Save(DM_LOAI_DONVI donvi)
        {
            try
            {
                if (donvi.ID == 0)
                {
                    this.repository.Insert(donvi);
                }
                else
                {
                    this.repository.Update(donvi);
                }
                this.repository.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public bool CheckExist(string loai, int id = 0)
        {
            var loaidonvi = this.context.DM_LOAI_DONVI.Where(x => x.LOAI.ToLower().Equals(loai.ToLower())).ToList();
            if (id > 0)
            {
                loaidonvi = loaidonvi.Where(x => x.ID != id).ToList();

            }

            if (loaidonvi.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool checkDelete(int id)
        {
            var CCTC = this.context.CCTC_THANHPHAN.Where(x => x.TYPE == id).Count();
            if (CCTC > 0)
            {
                return false;
            }
            else
            {
                var existChild = this.context.DM_LOAI_DONVI.Where(x => x.PARENT_ID == id).Count();
                if (existChild > 0)
                {
                    return false;
                }
                return true;
            }
        }
        public List<DMLoaiDonViBO> GetAll()
        {
            var listResult = (from loai in this.context.DM_LOAI_DONVI
                              join loaiCha in this.context.DM_LOAI_DONVI on loai.PARENT_ID equals loaiCha.ID
                              into jloaicha
                              from tencha in jloaicha.DefaultIfEmpty()
                              select new DMLoaiDonViBO()
                              {
                                  ID = loai.ID,
                                  LOAI = loai.LOAI,
                                  PARENT_ID = loai.PARENT_ID,
                                  PARENT_NAME = tencha.LOAI,
                                  NGAYSUA = loai.NGAYSUA,
                                  NGAYTAO = loai.NGAYTAO,
                                  NGUOISUA = loai.NGUOISUA,
                                  NGUOITAO = loai.NGUOISUA,
                              }).ToList();
            return listResult;
        }

        public DMLoaiDonViBO GetByID(int id)
        {
            var listResult = (from loai in this.context.DM_LOAI_DONVI
                              where loai.ID == id
                              join loaiCha in this.context.DM_LOAI_DONVI on loai.PARENT_ID equals loaiCha.ID
                              into jloaicha
                              from tencha in jloaicha.DefaultIfEmpty()
                              select new DMLoaiDonViBO()
                              {
                                  ID = loai.ID,
                                  LOAI = loai.LOAI,
                                  PARENT_ID = loai.PARENT_ID,
                                  PARENT_NAME = tencha.LOAI,
                                  NGAYSUA = loai.NGAYSUA,
                                  NGAYTAO = loai.NGAYTAO,
                                  NGUOISUA = loai.NGUOISUA,
                                  NGUOITAO = loai.NGUOISUA,
                              }).FirstOrDefault();
            return listResult;
        }


        public List<SelectListItem> DSLoaiDonVi(int? selectedID = null, int ChildID = 0)
        {

            if (ChildID > 0)
            {
                var ListResult = this.context.DM_LOAI_DONVI.Where(x => x.ID != ChildID).Select(x => new SelectListItem()
                {
                    Text = x.LOAI,
                    Value = x.ID.ToString(),
                    Selected = selectedID != null && x.ID == selectedID
                }).ToList();
                return ListResult;
            }
            else
            {
                var ListResult = this.context.DM_LOAI_DONVI.Select(x => new SelectListItem()
                {
                    Text = x.LOAI,
                    Value = x.ID.ToString(),
                    Selected = selectedID != null && x.ID == selectedID
                }).ToList();
                return ListResult;
            }

        }
    }
}
