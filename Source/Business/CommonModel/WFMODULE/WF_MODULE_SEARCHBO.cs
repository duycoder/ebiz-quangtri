using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;

namespace Business.CommonModel.WFMODULE
{
public class WF_MODULE_SEARCHBO:SearchBaseBO
{
public int? QR_ID { get; set; }
public int? QR_WF_STREAM_ID { get; set; }
public DateTime QR_create_at { get; set; }
public DateTime QR_edit_at { get; set; }
public long QR_create_by { get; set; }
public long QR_edit_by { get; set; }
public string QR_MODULE_CODE { get; set; }
public string QR_MODULE_TITLE { get; set; }
}
}

