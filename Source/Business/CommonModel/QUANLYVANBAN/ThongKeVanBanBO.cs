using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QUANLYVANBAN
{
    public class ThongKeVanBanBO
    {
        public string VanBan { get; set; }
        public string HoSo { get; set; }
        public string Nam { get; set; }
        public string Thang { get; set; }
        public string YearText { get; set; }
        public string DaMuon { get; set; }
        public string DaTra { get; set; }
        public List<int> ListMuon { get; set; }
        public List<int> ListTra { get; set; }
    }
}
