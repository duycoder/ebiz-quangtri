using Business.BaseBusiness;

namespace Business.CommonModel.THUMUCLUUTRU
{
    public class THUMUC_LUUTRU_SEARCHBO : SearchBaseBO
    {
        public string TEN_THUMUC { get; set; }
        public string TEN_TAILIEU { get; set; }
        public int? ACCESS_MODIFIER { get; set; }
        public int? FOLDER_PERMISSION { get; set; }
        public long? FOLDER_ID { get; set; }
        public long USER_ID { get; set; }
    }
}
