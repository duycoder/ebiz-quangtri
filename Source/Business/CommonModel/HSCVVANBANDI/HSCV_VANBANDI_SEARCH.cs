using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.HSCVVANBANDI
{
    public class HSCV_VANBANDI_SEARCH : SearchBaseBO
    {
        public int? SOVANBAN_ID { get; set; }
        public string SOHIEU { get; set; }
        public string TRICHYEU { get; set; }
        public int? LOAIVANBAN_ID { get; set; }
        public int? LINHVUCVANBAN_ID { get; set; }
        public int? DOKHAN_ID { get; set; }
        public int? DOUUTIEN_ID { get; set; }
        public long USER_ID { get; set; }
        public int? DONVI_ID { get; set; }
        public string ITEM_TYPE { get; set; }
        public int? LOAI_VANBAN { get; set; }
        public bool? IS_APPROVE { get; set; }
        public DateTime? NGAYBANHANH_TU { get; set; }
        public DateTime? NGAYBANHANH_DEN { get; set; }
        public DateTime? NGAYVANBAN_TU { get; set; }
        public DateTime? NGAYVANBAN_DEN { get; set; }
        public DateTime? NGAYHIEULUC_TU { get; set; }
        public DateTime? NGAYHIEULUC_DEN { get; set; }
        public DateTime? NGAYHETHIEULUC_TU { get; set; }
        public DateTime? NGAYHETHIEULUC_DEN { get; set; }
        public DateTime? NGAYTAO_TU { get; set; }
        public DateTime? NGAYTAO_DEN { get; set; }

        public bool isMobileFilter { set; get; }
        public string mobileQuery { set; get; }
    }
}
