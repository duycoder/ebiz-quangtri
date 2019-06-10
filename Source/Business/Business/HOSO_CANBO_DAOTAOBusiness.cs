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
using Business.CommonModel.DMCHUCNANG;
using Business.CommonModel.HOSOCANBO;


namespace Business.Business
{
    public class HOSO_CANBO_DAOTAOBusiness : BaseBusiness<HOSO_CANBO_DAOTAO>
    {
        public HOSO_CANBO_DAOTAOBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
    }
}

