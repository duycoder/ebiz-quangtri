using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.LICH_CONGTAC
{
    public class LICHCONGTAC_SEARCH
    {
        public int? queryDeptId { set; get; }
        public int calendarType { set; get; }
        public DateTime? calendarDay { set; get; }
        public DateTime? startDate { set; get; }
        public DateTime? endDate { set; get; }
        public long? leaderId { set; get; }
    }
}
