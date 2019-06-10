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
using Business.CommonModel.WFMODULE;



namespace Business.Business
{
    public class WF_REVIEWBusiness : BaseBusiness<WF_REVIEW>
    {
        public WF_REVIEWBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        
    }
}

