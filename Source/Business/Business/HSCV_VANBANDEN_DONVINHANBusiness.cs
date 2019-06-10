using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVCONGVIEC;
using Business.CommonModel.HSCVVANBANDENDONVINHAN;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Business.Business
{
    public class HSCV_VANBANDEN_DONVINHANBusiness : BaseBusiness<HSCV_VANBANDEN_DONVINHAN>
    {
        public HSCV_VANBANDEN_DONVINHANBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public void Save(HSCV_VANBANDEN_DONVINHAN Task)
        {
            try
            {
                if (Task.ID == 0)
                {
                    this.repository.Insert(Task);
                }
                else
                {
                    this.repository.Update(Task);
                }
                this.repository.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<HSCV_VANBANDEN_DONVINHAN> GetData(long id)
        {
            var result = from vanban in this.context.HSCV_VANBANDEN_DONVINHAN
                         where id == vanban.VANBANDEN_ID
                         select vanban;
            return result.ToList();
        }
        public List<HSCV_VANBANDEN_DONVINHAN_BO> GetDataBO(long id)
        {
            var result = from vanban in this.context.HSCV_VANBANDEN_DONVINHAN
                         join unit in this.context.CCTC_THANHPHAN
                         on vanban.DONVI_ID equals unit.ID
                         where id == vanban.VANBANDEN_ID
                         select new HSCV_VANBANDEN_DONVINHAN_BO
                         {
                             DONVI_ID = vanban.DONVI_ID,
                             ID = vanban.ID,
                             TEN_DONVI = unit.NAME,
                             VANBANDEN_ID = vanban.VANBANDEN_ID,
                             CODE = unit.CODE
                         };
            return result.ToList();
        }
    }
}
