using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTUtils.Excel
{
    public class VTFont
    {
        
        public short Boldweight { get; set; }
        //
        public short Charset { get; set; }
        //
        public short Color { get; set; }
        //
        public short FontHeight { get; set; }
        //
        public short FontHeightInPoints { get; set; }
        //
        public string FontName { get; set; }
        //
        //short Index { get;  }
        //
        public bool IsItalic { get; set; }
        //
        public bool IsStrikeout { get; set; }
        //
        public short TypeOffset { get; set; }
        //
        public byte Underline { get; set; }
    }
}
