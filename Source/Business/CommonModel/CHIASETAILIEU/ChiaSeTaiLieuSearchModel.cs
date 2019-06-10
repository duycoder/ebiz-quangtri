using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.CHIASETAILIEU
{
    public class ChiaSeTaiLieuSearchModel:SearchBaseBO
    {
        public int? STATUS { get; set; }
        public long? USER_YEU_CAU { get; set; }
        public long? USER_CHIA_SE { get; set; }
        public long? USER_PHE_DUYET { get; set; }
        public string KEYWORD { get; set; }
    }
}
