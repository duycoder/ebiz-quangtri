using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;
using Business.CommonModel.SYSTHONGBAO;

namespace Business.Business
{
    public class SYS_THONGBAOBusiness : BaseBusiness<THONGBAO>
    {
        public SYS_THONGBAOBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public List<SYS_THONGBAO_BO> GetThongBaoNew(long userId)
        {
            var query = (from tb in this.context.THONGBAO.Where(x => x.NGUOI_NHAN == userId && x.IS_READ!=true)
                         join tblNgGui in this.context.DM_NGUOIDUNG on tb.NGUOI_GUI equals tblNgGui.ID into jnggui
                         from nguoigui in jnggui.DefaultIfEmpty()
                         join tblNgNhan in this.context.DM_NGUOIDUNG on tb.NGUOI_GUI equals tblNgNhan.ID into jngnhan
                         from nguoinhanh in jngnhan
                         select new SYS_THONGBAO_BO()
                         {
                             ID = tb.ID,
                             LINK = tb.LINK,
                             MESSAGE = tb.MESSAGE,
                             NGUOI_GUI = tb.NGUOI_GUI,
                             NGUOI_NHAN = tb.NGUOI_NHAN,
                             create_at = tb.create_at,
                             create_by = tb.create_by,
                             InfoNguoiGui = nguoigui,
                             InfoNguoiNhan = nguoinhanh
                         }
                        )
                .OrderByDescending(x => x.create_at)
                .ToList();
            return query;
        }
    }
}
