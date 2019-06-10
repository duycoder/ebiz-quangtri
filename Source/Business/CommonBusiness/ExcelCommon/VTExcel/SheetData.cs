using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTUtils.Excel.Export
{
    public class SheetData
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public SheetData()
        {
            Data = new Dictionary<string, object>();
        }
        public SheetData(string Name)
        {
            this.Name = Name;
            Data = new Dictionary<string, object>();
        }
    }
}
