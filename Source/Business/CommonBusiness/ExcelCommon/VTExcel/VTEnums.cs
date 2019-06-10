using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTUtils.Excel.Export
{
    public enum VTBorderStyle
    {
        Dashed, Dotted, Double, Hidden, Solid
    }

    public enum VTBorderWeight
    {
        HairLine, Thin, Medium, Thick
    }

    public enum VTBorderIndex
    {
        All,
        Around,
        DiagonalDown,
        DiagonalUp,
        EdgeBottom,
        EdgeLeft,
        EdgeRight,
        EdgeTop,
        InsideAll,
        InsideHorizontal,
        InsideVertical  
    }

    /// <summary>
    /// Enum Page size
    /// <author>hungnd 08/11.2013</author>
    /// </summary>
    public enum VTXPageSize
    {
        //Not specified
        VTxlNone = 0,
        //Letter (8-1/2 in. x 11 in.)
        VTxlPaperLetter = 1,
        //A3 (297 mm x 420 mm)
        VTxlPaperA3 = 2,
        //A4 (210 mm x 297 mm)
        VTxlPaperA4 = 3,
        //A5 (148 mm x 210 mm)
        VTxlPaperA5 = 4,
        //Executive (7-1/2 in. x 10-1/2 in.)
        VTxlPaperExecutive,
    }

    /// <summary>
    /// Enum Page mode
    /// <author>hungnd 08/11.2013</author>
    /// </summary>
    public enum VTXPageOrientation
    {
        //Page Landscape mode.
        VTxlLandscape = 0,
        //Page Portrait mode.
        VTxlPortrait = 1,
    }
    public enum VTHAlign
    {
        xlHAlignGeneral = 0,
        xlHAlignLeft = 1,
        xlHAlignCenter = 2,
        xlHAlignRight = 3,
        xlHAlignFill = 4,
        xlHAlignJustify = 5,
        xlHAlignCenterAcrossSelection = 6,
        xlHAlignDistributed = 7,
    }
}