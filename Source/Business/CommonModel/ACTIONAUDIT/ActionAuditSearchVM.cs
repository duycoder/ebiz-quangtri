using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.ACTIONAUDIT
{
    public class ActionAuditSearchVM
    {
        public string sortQuery { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public long? USER_ID { get; set; }
        public string TENDANGNHAP { get; set; }
    }
}
