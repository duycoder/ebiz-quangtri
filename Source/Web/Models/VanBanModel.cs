using Business.CommonBusiness;
using Business.CommonModel.HSCVVANBANDEN;
using Business.CommonModel.HSCVVANBANDI;
using Business.CommonModel.TAILIEUDINHKEM;
using Model.Entities;
using System.Collections.Generic;

namespace Web.Models
{
    public class VanBanModel
    {
        public PageListResultBO<HSCV_VANBANDI_BO> ListVbDi { get; set; }
        public PageListResultBO<HSCV_VANBANDEN_BO> ListVbDen { get; set; }
        public PageListResultBO<HSCV_VANBANDEN_BO> ListVbDenNoiBo { set; get; }
        public List<HSCV_VANBANDEN_BO> Update_At { get; set; }

        public bool isVanBanDiFullWidth { set; get; }
        public bool isVanBanDenFullWidth { set; get; }
        public bool isHideAllVanBan { set; get; }

        public List<TAILIEUDINHKEM_BO> GroupAttachments { set; get; }
        public string ItemType { set; get; }
        public long ItemId { set; get; }
        public long RootItemType { set; get; }
        public HSCV_VANBANDI EntityVanBanDi { set; get; }
        public HSCV_VANBANDEN EntityVanBanDen { set; get; }
        public TAILIEUDINHKEM Attachment { set; get; }

    }
    public class Statistical
    {
        public PageListResultBO<HSCV_VANBANDI_BO> ListResultVanBan { get; set; }
        public PageListResultBO<HSCV_VANBANDEN_BO> ListResultVanBanDen { get; set; }
        public PageListResultBO<CongViecBO> ListResultCongViec { get; set; }
    }
}