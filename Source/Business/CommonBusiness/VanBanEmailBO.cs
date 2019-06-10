using Model.Entities;
using System.Collections.Generic;

namespace Business.CommonBusiness
{
    public class VanBanEmailBO : BaseEmailBO
    {
        public List<HSCV_VANBANDI> LstVanBanDi { get; set; }
        public List<long> LstVanBanIds { get; set; }
    }
}
