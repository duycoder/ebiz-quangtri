using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NativeExcel;

namespace VTUtils.Excel.Export
{
    public interface IVTRange
    {
        NativeExcel.IRange Range { get; set; }
        NativeExcel.IRange this[int row, int col] { get; }

        #region Properties

        /// <summary>
        /// Property hiden of entire column
        /// <author>Hungnd 17/03/2014</author>
        /// </summary>
        bool IsColumnHidden { get; set; }

        /// <summary>
        /// Property hiden of entire row 
        /// <author>Hungnd 17/03/2014</author>
        /// </summary>
        bool IsRowHidden { get; set; }

        object Value { get; set; }

        int TotalColumn { get; }

        int TotalRow { get; }

        /// <summary>
        /// Lock cell - effect when the sheet is placed password
        /// Hungnd 28/03/2013
        /// </summary>
        bool IsLock { get; set; }

        #endregion

        #region  Function

        /// <summary>
        /// Fill Giá trị các biến được khai báo trong file template tướng ứng với giá trị trong Data
        /// </summary>
        /// <param name="Data"></param>
        void FillVariableValue(Dictionary<string, object> Data, IVTWorksheet OtherRange = null);

        /// <summary>
        /// Merge cell
        /// </summary>
        /// <param name="Across">Merge mỗi hàng hay không. Nếu TRUE thì merge từng row</param>
        void Merge(bool Across = false);
        /// <summary>
        /// Merge cell
        /// </summary>
        /// <param name="Across">Merge mỗi hàng hay không. Nếu TRUE thì merge từng row</param>
        void MergeLeft(bool Across = false);

        void FillColor(Color color);

        void FillColor(int red, int blue, int green);

        void SetBorder(VTBorderStyle borderStyle, VTBorderIndex borderIndex = VTBorderIndex.InsideAll);

        void SetBorder(VTBorderWeight borderWeight, VTBorderIndex borderIndex = VTBorderIndex.InsideAll);

        void SetBorder(VTBorderStyle borderStyle, VTBorderWeight borderWeight, VTBorderIndex borderIndex = VTBorderIndex.InsideAll);

        void SetFontStyle(bool bold, Color? color, bool italic, byte? size, bool strikeThrough, bool underline);

        void SetFontColour(Color color);

        void SetHAlign(VTHAlign align);

        /// <summary>
        /// Function delete entire column
        /// <author>Hungnd 17/03/2014</author>
        /// </summary>
        void DeleteEntireColumn();

        /// <summary>
        /// Function delete entire row
        /// <author>Hungnd 17/03/2014</author>
        /// </summary>
        void DeleteEntireRow();

        /// <summary>
        /// changes the width of the columns in the used range to achieve the best fit
        /// </summary>
        void AutoFit();

        /// <summary>
        ///  change the height of the rows in order to achieve the best fit
        ///  <author>Hungnd 27/11/2013</author>
        /// </summary>
        void AutoFitRowHeight();
        #endregion

    }
}
