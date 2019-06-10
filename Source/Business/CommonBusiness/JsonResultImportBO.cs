using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonBusiness
{
    public class JsonResultImportBO<T> where T : class
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public List<T> ListData { get; set; }
        public List<List<string>> ListFalse { get; set; }
        public JsonResultImportBO(bool state)
        {
            Status = state;
        }
    }
}
