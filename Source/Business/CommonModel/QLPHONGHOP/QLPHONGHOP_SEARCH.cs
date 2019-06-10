using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QLPHONGHOP
{
    public class QLPHONGHOP_SEARCH
    {
        public DateTime? calendarDay { set; get; }
        public int calendarType { set; get; }
        public int? queryDeptParentID { set; get; }
        public long? queryUserId { set; get; }
        public DateTime? queryStartDate { set; get; }
        public DateTime? queryEndDate { set; get; }
    }
}
