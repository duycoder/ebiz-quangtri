using Business.CommonModel.HSCVVANBANDENDONVINHAN;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.HSCVVANBANDEN
{
    public class HSCV_VANBANDEN_BO : HSCV_VANBANDEN
    {
        public string TENSOVANBAN { get; set; }
        public string TEN_NGUOITAO { get; set; }
        public string TEN_HINHTHUC { get; set; }
        public string ICON_HINHTHUC { get; set; }
        public string TEN_LINHVUC { get; set; }
        public string ICON_LINHVUC { get; set; }
        public string TEN_DOKHAN { get; set; }
        public string ICON_DOKHAN { get; set; }
        public string TEN_DOMAT { get; set; }
        public string ICON_DOMAT { get; set; }
        public string VANBANDI_TRICHYEU { get; set; }
        public string VANBANDI_SOHIEU { get; set; }
        public string COLOR { get; set; }
        public string TEN_DONVI { get; set; }
        public List<CCTC_THANHPHAN> ListDonVi { get; set; }
        public List<HSCV_VANBANDEN_DONVINHAN_BO> ListDonViNhan { get; set; }
        public long? PROCESS_USER_ID { set; get; }
        public bool CAN_CREATE_CALENDAR { set; get; } //có thể tạo lịch công tác
        public DateTime? Update_At { get; set; }
    }
}
