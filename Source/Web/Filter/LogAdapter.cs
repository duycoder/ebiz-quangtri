using System;
using System.Collections.Generic;
using System.Configuration;
using Business.BaseBusiness;
using Model.Entities;
using Business.Business;

namespace Web.Filter
{
    public static class LogAdapter
    {
        public static List<ACTION_AUDIT> ListToInsert = new List<ACTION_AUDIT>();

        private static int GetMaxBulk()
        {
            return Int32.Parse(ConfigurationManager.AppSettings["MaxBulkLog"]);
        }

        public static void Insert(ACTION_AUDIT ActionAudit)
        {
            ActionAuditBusiness aab = new ActionAuditBusiness(new UnitOfWork());
            aab.Save(ActionAudit);
            /*
            ListToInsert.Add(ActionAudit);
            if (ListToInsert.Count >= GetMaxBulk())
            {
                Entities context = new Entities();
                ActionAuditBusiness aab = new ActionAuditBusiness(context);
                aab.BulkInsert(ListToInsert);
                ListToInsert.Clear();
            }
            */
        }
    }
}