using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.WFSTEP
{
    public class GoModel
    {
        public string nodeKeyProperty { get; set; }
        public List<NodeItem> nodeDataArray { get; set; }
        public List<StepItem> linkDataArray { get; set; }
    }
    public class NodeItem
    {
        public int id { get; set; }
        public string loc { get; set; }
        public string text { get; set; }
    }

    public class StepItem
    {
        public int from { get; set; }
        public int to { get; set; }
        public string text { get; set; }
    }
}
