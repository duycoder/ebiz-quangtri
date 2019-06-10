using Model.Entities;
using System.Collections.Generic;

namespace Business.CommonBusiness
{
    public class CongViecEmailBO : BaseEmailBO
    {
        public List<HSCV_CONGVIEC> ListCongViec { get; set; }
        public HSCV_CONGVIEC CongViec { get; set; }
    }
}
