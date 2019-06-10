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
    public class CHAT_GROUP_USERBusiness : BaseBusiness<CHAT_GROUP_USER>
    {
        public CHAT_GROUP_USERBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public string GetListUserName(long groupID)
        {
            var result = (from user in this.context.DM_NGUOIDUNG
                join grp_user in this.context.CHAT_GROUP_USER
                    on user.ID equals grp_user.USER_ID
                where grp_user.GROUP_ID == groupID
                select user.HOTEN).ToList();
            if (result != null && result.Count > 0)
            {
                var str_result = "";
                var i = 1;
                var total = result.Count;
                foreach (var item in result)
                {
                    if (i < total)
                    {
                        str_result += item + ", ";
                    }
                    else
                    {
                        str_result += item;
                    }
                    i++;
                }
                return str_result;
            }
            else
            {
                return string.Empty;
            }
        }

    }
}

