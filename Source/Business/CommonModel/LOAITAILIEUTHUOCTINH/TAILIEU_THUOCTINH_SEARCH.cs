using Business.BaseBusiness;

namespace Business.CommonModel.TAILIEUTHUOCTINH
{
    public class TAILIEU_THUOCTINH_SEARCH : SearchBaseBO
    {
        public string TEN_THUOCTINH { get; set; }
        public int LOAI_TAILIEU { get; set; }
        public bool? TRANGTHAI { get; set; }
    }
}
