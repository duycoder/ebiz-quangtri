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



namespace Business.Business
{
    public class CHAT_GROUPBusiness : BaseBusiness<CHAT_GROUP>
    {
        public CHAT_GROUPBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
       
    }
}

