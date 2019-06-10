using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.QL_LAIXEArea.Models
{
    public class LaiXeBenhVienEditViewModel
    {
        public QL_LAIXE laiXeEntity { set; get; }
        public LaiXeBenhVienEditViewModel()
        {
            laiXeEntity = new QL_LAIXE();
        }

        public LaiXeBenhVienEditViewModel(QL_LAIXE laiXeEntity)
        {
            this.laiXeEntity = laiXeEntity;
        }
    }
}