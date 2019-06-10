using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTUtils.Excel.Export
{
    public class WorkBookData
    {
        public List<SheetData> ListSheet { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public WorkBookData()
        {
            ListSheet = new List<SheetData>();
            Data = new Dictionary<string, object>();
        }

        public SheetData GetSheet(string name)
        {
            foreach (SheetData sheet in ListSheet)
            {
                if (sheet.Name == name)
                {
                    return sheet;
                }
            }
            return null;
        }
    }
}
