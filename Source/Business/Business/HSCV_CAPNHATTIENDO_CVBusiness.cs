using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVCONGVIEC;
using Model.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Business.Business
{
    public class HSCV_CAPNHATTIENDO_CVBusiness : BaseBusiness<HSCV_CAPNHATTIENDO_CV>
    {
        public HSCV_CAPNHATTIENDO_CVBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public void Save(HSCV_CAPNHATTIENDO_CV item)
        {
            try
            {
                if (item.ID == 0)
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
                throw new Exception(ex.Message);
            }
        }

    }
}
