using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Business.CommonModel.HSCVSUBTASK
{
    public class HSCV_SUBTASK_BO : HSCV_SUBTASK
    {
        public string TEN_MUCDOUUTIEN { set; get; }
        public string TEN_DOKHAN { set; get; }
        public string TEXT_IS_HASPLAN { set; get; }
        public string HANHOANTHANH_TEXT { set; get; }
    }
}
