using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.DUNGLUONGLUUTRU
{
    public class DUNGLUONG_LUUTRU_BO : DUNGLUONG_LUUTRU
    {
        public string TEN_NHANVIEN { get; set; }
        public string TEN_DONVI { get; set; }
        public long? CONLAI { get; set; }
    }
}
