using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.DS_PHONGHOP
{
    public class QLPHONGHOP_SEARCHBO : SearchBaseBO
    {
        public string TenPhong { get; set; }
        public string MaPhong { get; set; }
    }
}
