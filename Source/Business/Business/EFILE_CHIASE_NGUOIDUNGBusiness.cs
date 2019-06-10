using Business.BaseBusiness;
using Business.CommonBusiness;
using Business.CommonModel.TAILIEUTHUOCTINH;
using Model.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using PagedList;
using Business.CommonModel.LOAITAILIEUTHUOCTINH;
using Business.CommonModel.TAILIEUDINHKEM;
using System;

namespace Business.Business
{
    public class EFILE_CHIASE_NGUOIDUNGBusiness : BaseBusiness<EFILE_CHIASE_NGUOIDUNG>
    {
        public EFILE_CHIASE_NGUOIDUNGBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
    }
}
