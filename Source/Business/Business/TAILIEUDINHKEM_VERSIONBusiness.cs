using Business.BaseBusiness;
using Business.CommonModel.TAILIEUDINHKEMVERSION;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Business
{
    public class TAILIEUDINHKEM_VERSIONBusiness : BaseBusiness<TAILIEUDINHKEM_VERSION>
    {
        public TAILIEUDINHKEM_VERSIONBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<TAILIEUDINHKEM_VERSION_BO> FindDataByTaiLieu(long TAILIEU)
        {
            var result = from version in this.context.TAILIEUDINHKEM_VERSION
                         join nguoidung in this.context.DM_NGUOIDUNG
                         on version.NGUOITAI equals nguoidung.ID
                         into group1
                         from g1 in group1.DefaultIfEmpty()
                         orderby version.NGAYTAI descending
                         where version.TAILIEU_ID == TAILIEU
                         select new TAILIEUDINHKEM_VERSION_BO
                         {
                             ID = version.ID,
                             DINHDANG_FILE = version.DINHDANG_FILE,
                             DUONGDAN_FILE = version.DUONGDAN_FILE,
                             NGAYTAI = version.NGAYTAI,
                             NGUOITAI = version.NGUOITAI,
                             TAILIEU_ID = version.TAILIEU_ID,
                             TENNGUOITAI = g1.HOTEN,
                             TEN_TAILIEU = version.TEN_TAILIEU,
                             MOTA = version.MOTA,
                             VERSION = version.VERSION,
                         };
            return result.ToList();
        }
        public List<TAILIEUDINHKEM_VERSION> GetDataByTaiLieuID(long TAILIEU_ID)
        {
            var result = from version in this.context.TAILIEUDINHKEM_VERSION
                         where version.TAILIEU_ID == TAILIEU_ID
                         select version;
            return result.ToList();
        }
    }
}
