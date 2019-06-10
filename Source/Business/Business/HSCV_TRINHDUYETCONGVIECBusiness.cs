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
    public class HSCV_TRINHDUYETCONGVIECBusiness : BaseBusiness<HSCV_TRINHDUYETCONGVIEC>
    {
        public HSCV_TRINHDUYETCONGVIECBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
    }
}
