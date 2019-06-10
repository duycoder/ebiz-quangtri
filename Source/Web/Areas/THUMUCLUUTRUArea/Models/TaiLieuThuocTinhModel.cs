using Business.CommonBusiness;
using Business.CommonModel.LOAITAILIEUTHUOCTINH;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.THUMUCLUUTRUArea.Models
{
    public class TaiLieuThuocTinhModel
    {
        public LOAITAILIEU_THUOCTINH ThuocTinh { get; set; }
        public List<DM_DANHMUC_DATA> ListLoaiTaiLieu { get; set; }
        public List<LOAITAILIEU_THUOCTINH> ListThuocTinh { get; set; }
        public PageListResultBO<LOAITAILIEU_THUOCTINH_BO> ListResult { get; set; }
    }
}