using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonBusiness
{
    public class JsonResultBO
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public List<string> GroupTokens { set; get; }
        public JsonResultBO(bool st)
        {
            Status = st;
        }
        public void MessageFail(string mss)
        {
            Status = false;
            Message = mss;
        }
    }
}
