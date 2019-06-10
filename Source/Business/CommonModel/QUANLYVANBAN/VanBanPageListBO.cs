using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QUANLYVANBAN
{
    public class VanBanPageListBO
    {
        public long HOSO_ID { get; set; }
        public string HOSO_NAME { get; set; }
        public int? DONVI_ID { get; set; }
        public int TotalVanBan { get; set; }
        public ListVanBanBO ListVanBan { get; set; }
        public int? HOSO_NAM { get; set; }
        public int? NAM_CHINH_LY { get; set; }
        public int? KHO_ID { get; set; }
        public int? PHONG_ID { get; set; }
        public string FTS { get; set; }

    }
    public class ListVanBanBO
    {
        public List<VanBanChildBO> ListVanBan { get; set; }
        public long? HoSoID { get; set; }
    }
    public class VanBanChildBO
    {
        public long HOSO_ID { get; set; }
        public long VANBAN_ID { get; set; }
        public string SO_KYHIEU { get; set; }
        public DateTime? NGAYBANHANH { get; set; }
        public int? COQUAN_BANHANH_ID { get; set; }
        public string COQUAN_BANHANH_NAME { get; set; }
        public string TRICHYEU_VANBAN { get; set; }
        public string NGAYBANHANH_FORMAT { get; set; }
        public long? TAILIEU_ID { get; set; }
        public string TAILIEU_NAME { get; set; }
    }
}
 