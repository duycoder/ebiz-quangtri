using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Business.CommonModel.DMVAITRO
{
    public class DM_VAITRO_BO : DM_VAITRO
    {
        public string HOVATEN { get; set; }
        public long USERID { get; set; }
        public int NGUOIDUNG_VAITRO_ID { get; set; }
        public string DepartmentName { set; get; }
    }
}
