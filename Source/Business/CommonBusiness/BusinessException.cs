using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonBusiness
{
    public class BusinessException : Exception
    {
        public object Result;
        public BusinessException(string Message, object Result = null)
            : base(Message)
        {
            this.Result = Result;
        }
    }
}
