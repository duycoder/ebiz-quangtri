using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonModel.DS_PHONGHOP;

namespace Web.Areas.QL_PHONGHOPArea.Models
{
	public class EditVM
	{
        public QL_PHONGHOP Object { get; set; }
        public List<QL_PHONG_BO> LstPhong { get; set; }
	}
}