using Business.BaseBusiness;
using System;
using System.Collections.Generic;

namespace Business.CommonModel.HSCVVANBANDEN
{
    public class HSCV_VANBANDEN_SEARCH : SearchBaseBO
    {
        public string SOHIEU { get; set; }
        public string TRICHYEU { get; set; }
        public int? LOAIVANBAN_ID { get; set; }
        public int? LINHVUCVANBAN_ID { get; set; }
        public int? DOKHAN_ID { get; set; }
        public int? DOMAT_ID { get; set; }
        public string NGUOIKY { get; set; }
        public long USER_ID { get; set; }
        public int? DONVI_ID { get; set; }
        public int? SOVANBANDEN_ID { set; get; }
        /// <summary>
        /// Danh sách đơn vị có thể xem van bản
        /// </summary>
        public List<long> ListIds { get; set; }
        public List<int> ListDonVi { get; set; }

        public bool isMobileFilter { set; get; }
        public string mobileQuery { set; get; }
        public DateTime? NGAYBANHANH_TU { get; set; }
        public DateTime? NGAYBANHANH_DEN { get; set; }
        public DateTime? NGAYVANBAN_TU { get; set; }
        public DateTime? NGAYVANBAN_DEN { get; set; }
        public DateTime? NGAYHIEULUC_TU { get; set; }
        public DateTime? NGAYHIEULUC_DEN { get; set; }
        public DateTime? NGAYHETHIEULUC_TU { get; set; }
        public DateTime? NGAYHETHIEULUC_DEN { get; set; }
        public string ITEM_TYPE { set; get; }
        public bool isInternal { set; get; } //xác định văn bản nội bộ
        
    }
}
