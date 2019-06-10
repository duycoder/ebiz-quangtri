using Model.Entities;

namespace Business.CommonModel
{
    public class DetailVanBanBO : QUANLY_VANBAN
    {
        public string HOSO_NAME { get; set; }
        public string NGONNGU_NAME { get; set; }
        public string COQUAN_BANHANH_NAME { get; set; }
        public string LINHVUC_NAME { get; set; }
        public string LOAI_VANBAN_NAME { get; set; }
        public string TINHTRANG_VATLY_NAME { get; set; }
        public string DOMAT_NAME { get; set; }
        public string MUCDO_TRUYCAP_NAME { get; set; }
    }
}