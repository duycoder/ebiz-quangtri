using Business.BaseBusiness;
using Business.CommonModel.TAILIEUDINHKEM;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Business.Business
{
    public class FCM_NOTIFICATIONBusiness : BaseBusiness<FCM_NOTIFICATION>
    {
        public FCM_NOTIFICATIONBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }

        public void Save(FCM_NOTIFICATION item)
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
