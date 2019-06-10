using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.CongViecArea.Models
{
    public class CongViecCountModel
    {
        public int countAll { set; get; }
        public int countDangXuLy { set; get; }
        public int countCaNhan { set; get; }
        public int countXuLyChinh { set; get; }
        public int countThamGiaXuLy { set; get; }
        public int countDaGiao { set; get; }
        public int countTheoDoi { set; get; }

        public CongViecCountModel()
        {

        }

        public CongViecCountModel(int all, int viecDangXuLy, int viecCaNhan, int viecXuLyChinh, int viecThamGiaXuLy, int viecDaGiao, int viecTheoDoi)
        {
            countAll = all;
            countDangXuLy = viecDangXuLy;
            countCaNhan = viecCaNhan;
            countXuLyChinh = viecXuLyChinh;
            countThamGiaXuLy = viecThamGiaXuLy;
            countDaGiao = viecDaGiao;
            countTheoDoi = viecDaGiao;
        }
    }
}