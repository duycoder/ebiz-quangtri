using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;
using System.Web.Mvc;

namespace Business.Business
{
    public class WF_FUNCTIONBusiness : BaseBusiness<WF_FUNCTION>
    {
        public WF_FUNCTIONBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<SelectListItem> GetDsFunction(int selectedItem = 0)
        {
            var query = this.context.WF_FUNCTION.Select(x => new SelectListItem
            {
                Text = x.FUNTION_TITLE,
                Value = x.ID.ToString(),
                Selected = selectedItem > 0 && x.ID == selectedItem
            }).ToList();

            return query;
        }
    }
}
