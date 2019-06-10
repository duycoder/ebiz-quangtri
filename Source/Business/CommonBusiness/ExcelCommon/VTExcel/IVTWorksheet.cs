using NativeExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTUtils.Excel.Export
{
    public interface IVTWorksheet
    {
        #region Declaration
        NativeExcel.IWorksheet Worksheet { get; set; }

        /// <summary>
        /// Lấy tập các Cell từ CellX tới CellY
        /// </summary>
        /// <param name="firstRow">Vị trí hàng của CellX - tính từ 1</param>
        /// <param name="firstCoumn">Vị trí cột của CellX - tính từ 1</param>
        /// <param name="lastRow">Vị trí hàng của CellY - tính từ 1</param>
        /// <param name="lastColumn">Vị trí cột của CellY - tính từ 1</param>
        /// <returns>Tập các Cell nằm từ CellX tới CellY</returns>
        IVTRange GetRange(int firstRow, int firstCoumn, int lastRow, int lastColumn);

        /// <summary>
        /// Lấy tập các Cell từ Cell1 tới Cell2
        /// </summary>
        /// <param name="cell1">Cell1 - ghi như trong file Excel (A1, E3, G4...)</param>
        /// <param name="cell2">Cell2 - ghi như trong file Excel (A1, E3, G4...)</param>
        /// <returns></returns>
        IVTRange GetRange(string cell1, string cell2);

        /// <summary>
        /// Get all range used in sheet
        /// </summary>
        /// <returns></returns>
        IVTRange GetUsedRange();

        /// <summary>
        /// Get Row by index of row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        IVTRange GetRow(int row);

        /// <summary>
        /// Get index of used row in sheet
        /// </summary>
        /// <returns></returns>
        int GetCountUsedRows();

        /// <summary>
        /// Get index of used column in sheet
        /// </summary>
        /// <returns></returns>
        int GetCountUsedColumns();

        /// <summary>
        /// Property Hiden of sheet
        /// <author>Hungnd 20/12/2013</author>
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Property Hiden of sheet
        /// <author>Hungnd 17/03/2014</author>
        /// </summary>
        VTSheetVisibility Hiden { get; set; }
        #endregion

        #region Format
        /// <summary>
        /// Copy một khoảng dữ liệu (tập các Cell) sang một khoảng khác trong sheet (Độ cao của các Row sẽ không được Copy)
        /// </summary>
        /// <param name="range">Khoảng dữ liệu (tập các Cell)</param>
        /// <param name="firstRow">Vị trí hàng đầu tiên của khoảng sẽ paste dữ liệu</param>
        /// <param name="firstColumn">Vị trí cột đầu tiên của khoảng sẽ paste dữ liệu</param>
        void CopyPaste(IVTRange range, int firstRow, int firstColumn, bool copyStyleOnly = false);

        /// <summary>
        /// Cut một khoảng dữ liệu (tập các Cell) sang một khoảng khác trong sheet (Độ cao của các Row sẽ không được Copy)
        /// </summary>
        /// <param name="range">Khoảng dữ liệu (tập các Cell)</param>
        /// <param name="firstRow">Vị trí hàng đầu tiên của khoảng sẽ paste dữ liệu</param>
        /// <param name="firstColumn">Vị trí cột đầu tiên của khoảng sẽ paste dữ liệu</param>
        void CutPaste(IVTRange range, int firstRow, int firstColumn);

        /// <summary>
        /// Copy một khoảng dữ liệu (tập các Cell) sang một khoảng khác trong sheet (Độ cao của các Row cũng được Copy)
        /// </summary>
        /// <param name="range">Khoảng dữ liệu (tập các Cell)</param>
        /// <param name="firstRow">Vị trí hàng đầu tiên của khoảng sẽ paste dữ liệu</param>
        /// <param name="firstColumn">Vị trí cột đầu tiên của khoảng sẽ paste dữ liệu</param>
        void CopyPasteSameRowHeigh(IVTRange range, int firstRow, int firstColumn, bool copyStyleOnly = false);

        /// <summary>
        /// Copy một khoảng dữ liệu (tập các Cell) sang một khoảng khác trong sheet (Độ cao của các Row cũng được Copy).
        /// Vị trí cột của khoảng sẽ paste dữ liệu sẽ lấy giống vị trí cột của khoảng copy
        /// </summary>
        /// <param name="range">Khoảng dữ liệu (tập các Cell)</param>
        /// <param name="firstRow">Vị trí hàng đầu tiên của dữ liệu được Copy</param>
        void CopyPasteSameRowHeigh(IVTRange range, int firstRow, bool copyStyleOnly = false);

        /// <summary>
        /// Copy một khoảng dữ liệu (tập các Cell) sang một khoảng khác trong sheet (Độ rộng của các Column cũng được Copy)
        /// </summary>
        /// <param name="range">Khoảng dữ liệu (tập các Cell)</param>
        /// <param name="firstRow">Vị trí hàng đầu tiên của khoảng sẽ paste dữ liệu</param>
        /// <param name="firstColumn">Vị trí cột đầu tiên của khoảng sẽ paste dữ liệu</param>
        void CopyPasteSameColumnWidth(IVTRange range, int firstRow, int firstColumn, bool copyStyleOnly = false);

        /// <summary>
        /// Copy một khoảng dữ liệu (tập các Cell) sang một khoảng khác trong sheet (kích thước cũng được Copy)
        /// </summary>
        /// <param name="range">Khoảng dữ liệu (tập các Cell)</param>
        /// <param name="firstRow">Vị trí hàng đầu tiên của khoảng sẽ paste dữ liệu</param>
        /// <param name="firstColumn">Vị trí cột đầu tiên của khoảng sẽ paste dữ liệu</param>
        void CopyPasteSameSize(IVTRange range, int firstRow, int firstColumn);

        /// <summary>
        /// Copy một khoảng dữ liệu (tập các Cell) sang một khoảng khác trong sheet (kích thước cũng được Copy)
        /// </summary>
        /// <param name="range">Khoảng dữ liệu (tập các Cell)</param>
        /// <param name="cell">Vị trí cell đầu tiên</param>
        void CopyPasteSameSize(IVTRange range, string cell);

        /// <summary>
        /// Ghi giá trị vào một Cell trong Sheet
        /// </summary>
        /// <param name="cell">Cell cần ghi giá trị - ghi như trong file Excel (A1, E3, G4...)</param>
        /// <param name="value">Giá trị cần ghi</param>
        void SetCellValue(string cell, object value);

        /// <summary>
        /// Ghi giá trị vào một Cell trong Sheet
        /// </summary>
        /// <param name="row">Vị trí hàng của cell - tính từ 1</param>
        /// <param name="column">Vị trí cột của cell - tính từ 1</param>
        /// <param name="value">giá trị cần ghi</param>
        void SetCellValue(int row, int column, object value);

        /// <summary>
        /// Ghi giá trị vào một Cell trong Sheet
        /// </summary>
        /// <param name="vector">Vector tương ứng vị trí Cell</param>
        /// <param name="value">giá trị cần ghi</param>
        void SetCellValue(VTVector vector, object value);

        /// <summary>
        /// Tạo dấu ngắt trang cho Sheet
        /// </summary>
        /// <param name="row">Dòng cần ngắt</param>
        void SetBreakPage(int row);

        /// <summary>
        /// Copy một Row rồi insert vào vị trí khác
        /// </summary>
        /// <param name="copiedRow"> row cần copy</param>
        /// <param name="aboveRow"> row mà dữ liệu sẽ được insert</param>
        void CopyAndInsertARow(int copiedRow, int insertRow, bool copyStyleOnly = false);

        /// <summary>
        /// Insert một dòng rồi copy dữ liệu vào
        /// </summary>
        /// <param name="copiedRow"> dữ liệu cần copy</param>
        /// <param name="aboveRow"> row mà dữ liệu sẽ được insert</param>
        void CopyAndInsertARow(IVTRange copiedRow, int insertRow, bool copyStyleOnly = false);

        /// <summary>
        /// Lấy độ cao của một row
        /// </summary>
        /// <param name="row">Vị trí của row</param>
        /// <returns></returns>
        double GetRowHeight(int row);

        /// <summary>
        /// Thiết lập lại độ cao của row
        /// </summary>
        /// <param name="row">Vị trí của row</param>
        /// <param name="height">độ cao</param>
        void SetRowHeight(int row, double height);

        /// <summary>
        /// Copy dữ liệu từ một sheet khác
        /// </summary>
        /// <param name="worksheet">Dữ liệu được copy</param>
        void CopySheet(IVTWorksheet worksheet);

        /// <summary>
        /// Copy vùng dữ liệu (từ A1 - lastCell) từ một sheet khác 
        /// </summary>
        /// <param name="worksheet">Sheet được copy</param>
        /// <param name="lastCell"></param>
        void CopySheet(IVTWorksheet worksheet, string lastCell);

        /// <summary>
        /// Xoá sheet khỏi workbook
        /// </summary>
        void Delete();

        /// <summary>
        /// Fill dữ liệu một list theo chiều ngang
        /// Nếu số lượng dữ liệu lớn hơn độ dài cho phép thì merge các cell trong khu vực cho phép đổ dữ liệu rồi 
        /// ghép các dữ liệu cần fill thành chuỗi string cách nhau bởi dấu cách, fill vào cell sau khi merge
        /// </summary>
        /// <param name="listData">Dữ liệu cần fill</param>
        /// <param name="firstPosition">Vị trí đầu tiên để fill</param>
        /// <param name="length">chiều dài cho phép</param>
        void FillDataHorizon<T>(List<T> listData, VTVector firstPosition, int length);

        void FillDataHorizon<T>(List<T> listData, int x, int y, int length = 0);

        /// <summary>
        /// Set khu vực dữ liệu
        /// </summary>
        /// <param name="value">vùng in dữ liệu - kiểu $A$1:$W$100</param>
        void SetPrintArea(string value);

        void FillVariableValue(Dictionary<string, object> Data, IVTWorksheet OtherRange = null);

        void MergeRow(int row, int firstCol, int lastCol);

        void MergeRow(int row, char firstCol, char lastCol);

        double GetColumnWidth(char column);

        double GetColumnWidth(int column);

        void SetColumnWidth(char column, double width);

        void SetColumnWidth(int column, double width);

        void SetFormulaValue(int row, int column, object value);

        void SetFormulaValue(string cell, object value);

        void DeleteRow(int RowNum);

        void DeleteAllHideRow();

        void ClearFormulaKeepValue();
        
        void DeleteAllHideColumn();

        void DeleteColumn(int ColumnIndex);

        void MergeColumn(int col, int firstRow, int lastRow);

        void MergeColumn(char col, int firstRow, int lastRow);

        object GetCellValue(int row, int column);

        object GetCellValue(string cell);

        /// <summary>
        /// Lock range in worksheet and set color
        /// </summary>
        /// <param name="workSheet">WorkSheet name</param>
        /// <param name="row1">start row</param>
        /// <param name="col1">start column</param>
        /// <param name="row2">end row</param>
        /// <param name="col2">end column</param>
        void Lock(IVTWorksheet workSheet, int row1, int col1, int row2, int col2);

        void LockCellExemptType(IVTWorksheet workSheet, int row1, int col1, int row2, int col2);

        string StandartClassName(string className);

        /// <summary>
        /// Protect with manual pass
        /// Hungnd8 28/03/2013
        /// </summary>
        /// <param name="passWord"></param>
        void ProtectSheet(string PassWord);

        /// <summary>
        /// Protect with auto pass
        /// Hungnd 28/03/2013
        /// </summary>
        void ProtectSheet();

        /// <summary>
        /// Lock all cell in sheet
        /// Hungnd8 28/03/2013
        /// </summary>
        bool LockSheet { get; set; }

        /// <summary>
        /// hide column by index
        /// Hungnd8 10/05/2013
        /// </summary>
        /// <param name="ColumnIndex"></param>
        void HideColumn(int ColumnIndex);

        void PasteRange(IVTRange sourceRange, IVTRange targerRange);

        void InsertRow(int insertRow);
        void SetBorder(VTBorderStyle boderStyle, VTBorderIndex borderIndex, int firstRow, int firstCoumn, int lastRow, int lastColumn);
        #endregion

        #region Page Setup
        /// <summary>
        /// Set Print Area Ex: A1:C20
        /// <author>hungnd 30/10.2013</author>
        /// </summary>
        string PrintArea { get; set; }

        /// <summary>
        /// Fit All column to 1 page
        /// <author>hungnd 30/10.2013</author>
        /// </summary>
        bool FitToPage { get; set; }

        /// <summary>
        /// Fit Sheet On One Page (Shrink the printout so that it fit on one page)
        /// <author>hungnd 25/11/2013</author>
        /// </summary>
        bool FitSheetOnOnePage { get; set; }

        /// <summary>
        /// Fit All Columns On One Page (Shrink the printout so that it fit on one page wide)
        /// <author>hungnd 25/11/2013</author>
        /// </summary>
        bool FitAllColumnsOnOnePage { get; set; }

        /// <summary>
        /// Fit All Rows On One Page (Shrink the printout so that it fit on one page high)
        /// <author>hungnd 25/11/2013</author>
        /// </summary>
        bool FitAllRowsOnOnePage { get; set; }

        /// <summary>
        /// Zoom page, No Scalling (Print sheets at their actual size)
        /// <author>hungnd 25/11/2013</author>
        /// </summary>
        int ZoomPage { get; set; }

        /// <summary>
        /// Set margin (Inches)
        /// Top, Left, Right, Bottom
        /// Ex: PageMagin = 2, 2, 3 , 3
        /// </summary>
        string PageMagin { get; set; }

        /// <summary>
        /// Set Margin top (Inches)
        /// </summary>
        double PageMaginTop { get; set; }

        /// <summary>
        /// Set Margin left (Inches)
        /// </summary>
        double PageMaginLeft { get; set; }

        /// <summary>
        /// Set Margin right (Inches)
        /// </summary>
        double PageMaginRight { get; set; }

        /// <summary>
        /// Set Margin bottom (Inches)
        /// </summary>
        double PageMaginBottom { get; set; }

        /// <summary>
        /// Set PageSize for Worksheet. value is VTUtils.Excel.Export.VTXPageSize
        /// Ex: None, A4, A5, A3 ....
        /// </summary>
        VTXPageSize PageSize { get; set; }

        /// <summary>
        /// Sets the first sheet to be printed in landscape orientation
        /// </summary>
        VTXPageOrientation Orientation { get; set; }
        #endregion
    }
}
