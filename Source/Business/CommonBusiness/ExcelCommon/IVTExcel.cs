using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using VTUtils.Excel.Style;

namespace VTUtils.Excel
{
    public interface IVTExcel
    {
        #region import
        /// <summary>
        /// Read excel file
        /// </summary>
        /// <param name="xlsPathName">the absolute path of the file</param>
        void LoadTemplate(string xlsPathName);

        /// <summary>
        /// Read excel from stream
        /// </summary>
        /// <param name="xlsStream">stream</param>
        void LoadTemplate(Stream xlsStream);
      
        /// <summary>
        /// Read data type of the cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index (one-base)</param>
        /// <returns>The data type of the cell</returns>
        short GetCellType(string sheetName, int row, int col);

        /// <summary>
        /// Read data type of the cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's name (A,B,...)</param>
        /// <returns>the data type of the cell</returns>
        short GetCellType(string sheetName, int row, string colName);

        /// <summary>
        /// Read value of the cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index (one-base)</param>
        /// <returns>Value of the cell</returns>
        object GetCellValue(string sheetName, int row, int col);

        /// <summary>
        /// Read value of the cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's name (A,B,..)</param>
        /// <returns>Value of the cell</returns>
        object GetCellValue(string sheetName, int row, string colName);

        /// <summary>
        /// Read formula of the cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index (one-base)</param>
        /// <returns>String present formula of the cell</returns>
        string GetCellFormula(string sheetName, int row, int col);

        /// <summary>
        /// Read formula of the cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's name (A,B,..)</param>
        /// <returns>String present formula of the cell</returns>
        string GetCellFormula(string sheetName, int row, string colName);

        /// <summary>
        /// Read table of the excel file
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <returns>DataTable object</returns>
        DataTable ImportToDataTable(string sheetName);

        /// <summary>
        /// Read table of the excel file
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowHeader">the header row's index (one-base)</param>
        /// <returns>DataTable object</returns>
        DataTable ImportToDataTable(string sheetName, int rowHeader);

        //List<T> DataTable2Object<T>(DataTable dt) where T : class;        

        #endregion

        #region export

        #region DocumentInfo
        /// <summary>
        /// Set information of document
        /// </summary>
        /// <param name="company">the company's name</param>
        /// <param name="subject">the subject</param>
        void SetDocumentInfo(string company, string subject);
        #endregion

        #region Sheet

        /// <summary>
        /// Create worksheet
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        void CreateSheet(string sheetName);

        /// <summary>
        /// Delete worksheet
        /// </summary>
        /// <param name="sheetIndex">the sheet's index</param>
        void RemoveSheetAt(int sheetIndex);

        /// <summary>
        /// Set the sheet's name
        /// </summary>
        /// <param name="sheetIndex">the sheet's index </param>
        /// <param name="sheetName">the sheet's name</param>
        void SetSheetNameAt(int sheetIndex, string sheetName);

        /// <summary>
        /// Set position of sheet
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="pos">the position of sheet</param>
        void SetSheetOrder(string sheetName, int pos);

        /// <summary>
        /// Hide or show sheet 
        /// </summary>
        /// <param name="sheetIndex">the sheet's index</param>
        /// <param name="hidden">hidden (true) or show (false)</param>
        void HideSheet(int sheetIndex, bool hidden);

        /// <summary>
        /// Set active sheet
        /// </summary>
        /// <param name="sheetIndex">the sheet's index</param>
        void SetActiveSheet(int sheetIndex);

        /// <summary>
        /// Set selected sheet
        /// </summary>
        /// <param name="sheetIndex">the sheet's index</param>
        void SetSelectedSheet(int sheetIndex);

        #endregion

        #region Row, Column

        /// <summary>
        /// Create row
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rownum">the row's index (one-base)</param>
        void CreateRow(string sheetName, int rownum);

        /// <summary>
        /// Set column width
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="columnIndex">the column's index (one-base)</param>
        /// <param name="width">width's value</param>
        void SetColumnWidth(string sheetName, int columnIndex, int width);

        /// <summary>
        /// Set column width
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="columnIndex">the column's name (A,B,..)</param>
        /// <param name="width">width's value</param>
        void SetColumnWidth(string sheetName, string colName, int width);

        /// <summary>
        /// Hide row
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="hide">hide (true) or show (false)</param>
        void HideRow(string sheetName, int row, bool hide);

        /// <summary>
        /// Hide column
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="col">the column's index (one-base)</param>
        /// <param name="hide">hide (true) or show (false)</param>
        void HideColumn(string sheetName, int col, bool hide);

        /// <summary>
        /// Hide column
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="colName">the column's name (A,B...)</param>
        /// <param name="hide">hide (true) or show (false)</param>
        void HideColumn(string sheetName, string colName, bool hide);

        #endregion

        #region Cell

        #region create,set value       

        /// <summary>
        /// Create cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="columnIndex">the column's index (one-base)</param>
        void CreateCell(string sheetName, int rowIndex, int columnIndex);

        /// <summary>
        /// Create cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="colName">the column's name</param>
        void CreateCell(string sheetName, int rowIndex, string colName);

        /// <summary>
        /// Create cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="columnIndex">the column's index (one-base)</param>
        /// <param name="cellType">the data type of cell</param>
        void CreateCell(string sheetName, int rowIndex, int columnIndex, int cellType);

        /// <summary>
        /// Create cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="colName">the column's name</param>
        /// <param name="type">the cell's type</param>
        void CreateCell(string sheetName, int rowIndex, string colName, int type);

        /// <summary>
        /// Create range of cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowBegin">the first row's index (one-base)</param>
        /// <param name="rowEnd">the last row's index (one-base)</param>
        /// <param name="colBegin">the first column's index (one-base)</param>
        /// <param name="colEnd"> the last column's index (one-base)</param>
        void CreateRange(string sheetName, int rowBegin, int rowEnd, int colBegin, int colEnd);

        /// <summary>
        /// Set cell's type
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="columnIndex">the column's index (one-base)</param>
        /// <param name="cellType">cell's type</param>
        void SetCellType(string sheetName, int rowIndex, int columnIndex, int cellType);

        /// <summary>
        /// Set cell's type
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="colName">the column's name (A,B,..)</param>
        /// <param name="cellType">cell's type</param>
        void SetCellType(string sheetName, int rowIndex, string colName, int cellType);

        /// <summary>
        /// Set cell's formula
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="columnIndex">the column's index (one-base)</param>
        /// <param name="formula">the formula</param>
        void SetCellFormula(string sheetName, int rowIndex, int columnIndex, string formula);

        /// <summary>
        /// Set cell's formula
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="colName">the column's name (A, B,..)</param>
        /// <param name="formula">the formula</param>
        void SetCellFormula(string sheetName, int rowIndex, string colName, string formula);

        /// <summary>
        /// Set cell's value
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="columnIndex">the column's index (one-base)</param>
        /// <param name="value">the boolean value</param>
        void SetCellValue(string sheetName, int rowIndex, int columnIndex, bool value);

        /// <summary>
        /// Set cell's value
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="colName">the column's index (A,B,..)</param>
        /// <param name="value">the boolean value</param>
        void SetCellValue(string sheetName, int rowIndex, string colName, bool value);

        /// <summary>
        /// Set cell's value
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="columnIndex">the column's index (one-base)</param>
        /// <param name="value">the double value</param>
        void SetCellValue(string sheetName, int rowIndex, int columnIndex, double value);

        /// <summary>
        /// Set cell's value
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="colName">the column's name (A,B,...)</param>
        /// <param name="value">the double value</param>
        void SetCellValue(string sheetName, int rowIndex, string colName, double value);

        /// <summary>
        /// Set cell's value
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="columnIndex">the column's index (one-base)</param>
        /// <param name="value">the string value</param>
        void SetCellValue(string sheetName, int rowIndex, int columnIndex, string value);

        /// <summary>
        /// Set cell's value
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="colName">the column's name</param>
        /// <param name="value">the string value</param>
        void SetCellValue(string sheetName, int rowIndex, string colName, string value);

        /// <summary>
        /// Set cell's value
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="columnIndex">the column's index (one-base)</param>
        /// <param name="dateTimeFormat">the date format</param>
        /// <param name="value">the date value</param>
        void SetCellValue(string sheetName, int rowIndex, int columnIndex, string dateTimeFormat, DateTime value);

        /// <summary>
        /// Set cell's value
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowIndex">the row's index (one-base)</param>
        /// <param name="colName">the column's name (one-base)</param>
        /// <param name="dateTimeFormat">the date format</param>
        /// <param name="value">the date value</param>
        void SetCellValue(string sheetName, int rowIndex, string colName, string dateTimeFormat, DateTime value);
        #endregion

        #region Comment
        // Parameters:
        //   dx1:
        //     the x coordinate within the first cell.
        //
        //   dy1:
        //     the y coordinate within the first cell.
        //
        //   dx2:
        //     the x coordinate within the second cell.
        //
        //   dy2:
        //     the y coordinate within the second cell.
        //
        //   col1:
        //     the column (0 based) of the first cell.
        //
        //   row1:
        //     the row (0 based) of the first cell.
        //
        //   col2:
        //     the column (0 based) of the second cell.
        //
        //   row2:
        //     the row (0 based) of the second cell.
        /// <summary>
        /// Set comment
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="anchorDx1">anchor x1</param>
        /// <param name="anchorDy1">anchor y1</param>
        /// <param name="anchorDx2">anchor x2</param>
        /// <param name="anchorDy2">anchor y2</param>
        /// <param name="anchorCol1"></param>
        /// <param name="anchorRow1"></param>
        /// <param name="anchorCol2"></param>
        /// <param name="anchorRow2"></param>
        /// <param name="content"></param>
        /// <param name="author"></param>
        /// <param name="visible"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        void SetCellComment(string sheetName, int anchorDx1, int anchorDy1,
                                   int anchorDx2, int anchorDy2, int anchorCol1, int anchorRow1,
                                   int anchorCol2, int anchorRow2, string content, string author,
                                   bool visible, int row, int col);

        /// <summary>
        /// Set comment
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="anchorDx1"></param>
        /// <param name="anchorDy1"></param>
        /// <param name="anchorDx2"></param>
        /// <param name="anchorDy2"></param>
        /// <param name="anchorCol1"></param>
        /// <param name="anchorRow1"></param>
        /// <param name="anchorCol2"></param>
        /// <param name="anchorRow2"></param>
        /// <param name="content"></param>
        /// <param name="author"></param>
        /// <param name="visible"></param>
        /// <param name="row"></param>
        /// <param name="colName"></param>
        void SetCellComment(string sheetName, int anchorDx1, int anchorDy1,
                                   int anchorDx2, int anchorDy2, int anchorCol1, int anchorRow1,
                                   int anchorCol2, int anchorRow2, string content, string author,
                                   bool visible, int row, string colName);
        /// <summary>
        /// Set comment
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="content"></param>
        /// <param name="author"></param>
        /// <param name="visible"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        void SetCellComment(string sheetName, string content, string author, bool visible, int row, int col);

        /// <summary>
        /// Set comment
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="content"></param>
        /// <param name="author"></param>
        /// <param name="visible"></param>
        /// <param name="row"></param>
        /// <param name="colName"></param>
        void SetCellComment(string sheetName, string content, string author, bool visible, int row, string colName);

        /// <summary>
        /// Set comment content
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="content"></param>
        void SetCommentContent(string sheetName, int row, int col, string content);

        /// <summary>
        /// Hide or show comment
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="visible"></param>
        void SetCommentVisible(string sheetName, int row, int col, bool visible);

        /// <summary>
        /// Set comment font
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="vtFont"></param>
        void SetCommentFont(string sheetName, int row, int col, VTFont vtFont);

        #endregion

        #region Style

        #region style border
        /// <summary>
        /// Set border of cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index</param>
        /// <param name="col">the column's index</param>
        /// <param name="borderBottom">the bottom border</param>
        /// <param name="bottomBorderColor">the bottom border color</param>
        /// <param name="borderTop">the top border</param>
        /// <param name="topBorderColor">the top border color</param>
        /// <param name="borderLeft">the left border</param>
        /// <param name="leftBorderColor">the left border color</param>
        /// <param name="borderRight">the right border</param>
        /// <param name="rightBorderColor">the right border color</param>
        void SetStyleBorder(string sheetName, int row, int col,
                                    VTBorderStyle borderBottom, short bottomBorderColor,
                                    VTBorderStyle borderTop, short topBorderColor,
                                    VTBorderStyle borderLeft, short leftBorderColor,
                                    VTBorderStyle borderRight, short rightBorderColor);
        /// <summary>
        /// Set border of cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index (one-base)</param>
        /// <param name="borderBottom">borderBottom</param>
        /// <param name="borderTop">borderTop</param>
        /// <param name="borderLeft">borderLeft</param>
        /// <param name="borderRight">borderRight</param>
        void SetStyleBorder(string sheetName, int row, int col,
                                    VTBorderStyle borderBottom, VTBorderStyle borderTop,
                                    VTBorderStyle borderLeft, VTBorderStyle borderRight);

        /// <summary>
        /// Set border of cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="colName">the column's name (A,B,..)</param>
        /// <param name="borderBottom">borderBottom</param>
        /// <param name="borderTop">borderTop</param>
        /// <param name="borderLeft">borderLeft</param>
        /// <param name="borderRight">borderRight</param>
        void SetStyleBorder(string sheetName, int row, string colName,
                                    VTBorderStyle borderBottom, VTBorderStyle borderTop,
                                    VTBorderStyle borderLeft, VTBorderStyle borderRight);
        #endregion

        #region style aligment
        /// <summary>
        /// Set alignment
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index (one-base)</param>
        /// <param name="alignment">horizontal alignment</param>
        /// <param name="verticalAlignment">vertical alignment</param>
        void SetStyleAlignment(string sheetName, int row, int col, VTHorizontalAlignment alignment, VTVerticalAlignment verticalAlignment);

        /// <summary>
        /// Set alignment
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="colName">the column's name (A,B,...)</param>
        /// <param name="alignment">horizontal alignment</param>
        /// <param name="verticalAlignment">vertical alignment</param>
        void SetStyleAlignment(string sheetName, int row, string colName, VTHorizontalAlignment alignment, VTVerticalAlignment verticalAlignment);
        
        #endregion
        
        #region color
        /// <summary>
        /// Set background color
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index (one-base)</param>
        /// <param name="backGroundColor">color</param>
        void SetStyleBGColor(string sheetName, int row, int col, System.Drawing.Color backGroundColor);

        /// <summary>
        /// Set background color
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="colName">the column's name (A,B,...)</param>
        /// <param name="backGroundColor">color</param>
        void SetStyleBGColor(string sheetName, int row, string colName, System.Drawing.Color backGroundColor);
        
        #endregion

        #region Font
        /// <summary>
        /// Set text font
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index (one-base)</param>
        /// <param name="fColor">the color of text</param>
        /// <param name="isItalic">italic or not</param>
        /// <param name="bold">bold or not</param>
        /// <param name="underline">underline or not</param>
        /// <param name="fontHeightInPoints">font height</param>
        /// <param name="fontName">font's name</param>
        void SetStyleFont(string sheetName, int row, int col,
                                 System.Drawing.Color fColor, bool isItalic, bool bold,
                                 VTFontUnderlineType underline, short fontHeightInPoints,
                                string fontName);

        /// <summary>
        /// Set text font
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index (one-base)</param>
        /// <param name="fColor">the color of text</param>
        /// <param name="isItalic">italic or not</param>
        /// <param name="bold">bold or not</param>
        /// <param name="underline">underline or not</param>
        /// <param name="fontHeightInPoints">font height</param>
        /// <param name="fontName">font's name</param>
        void SetStyleFont(string sheetName, int row, string colName,
                                 System.Drawing.Color fColor, bool isItalic, bool bold,
                                 VTFontUnderlineType underline, short fontHeightInPoints,
                                string fontName);
        
        #endregion

        #region wraptext and rotation
        /// <summary>
        /// Set Wraptext
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index (one-base)</param>
        /// <param name="wraptext">wraptext (true) or not wraptex (false)</param>
        void SetStyleWraptext(string sheetName, int row, int col, bool wraptext);

        /// <summary>
        /// Set Wraptext
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="colName">the column's name (A,B,...)</param>
        /// <param name="wraptext">wraptext (true) or not wraptex (false)</param>
        void SetStyleWraptext(string sheetName, int row, string colName, bool wraptext);
        
        //a valid rotate value is from -90 - 90 
        /// <summary>
        /// Set rotation
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index (one-base)</param>
        /// <param name="rotation">rotation between -90 and 90</param>
        void SetStyleRotation(string sheetName, int row, int col, short rotation);

        /// <summary>
        /// Set rotation
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="colName">the column's name (A,B,...)</param>
        /// <param name="rotation">rotation between -90 and 90</param>
        void SetStyleRotation(string sheetName, int row, string colName, short rotation);

        /// <summary>
        /// Clone style
        /// </summary>
        /// <param name="sheetOriginal"></param>
        /// <param name="rowOriginal"></param>
        /// <param name="colOriginal"></param>
        /// <param name="sheetDestination"></param>
        /// <param name="rowDestination"></param>
        /// <param name="colDestination"></param>
        void CloneStyle(string sheetOriginal, int rowOriginal, int colOriginal, string sheetDestination, int rowDestination, int colDestination);

        #endregion       

        #endregion

        #region merge and lock cell

        /// <summary>
        /// Merge cell
        /// </summary>
        /// <param name="sheetName">sheet's name</param>
        /// <param name="firstRow">first row's index (one-base)</param>
        /// <param name="firstCol">first column's index</param>
        /// <param name="lastRow">last row's index</param>
        /// <param name="lastCol">last column's index</param>
        void MergeCell(string sheetName, int firstRow, int firstCol, int lastRow, int lastCol);

        /// <summary>
        /// Merge cell
        /// </summary>
        /// <param name="sheetName">sheetName's</param>
        /// <param name="firstRow">first row's index (one-base)</param>
        /// <param name="firstColName">first column's name (A,B,..)</param>
        /// <param name="lastRow">last row's index</param>
        /// <param name="lastColName">last column's name</param>
        void MergeCell(string sheetName, int firstRow, string firstColName, int lastRow, string lastColName);

        /// <summary>
        /// Unlock one cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base) </param>
        /// <param name="col">the column's index</param>
        void UnLockCell(string sheetName, int row, int col);

        /// <summary>
        /// Unlock one cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base) </param>
        /// <param name="col">the column's name</param>
        void UnLockCell(string sheetName, int row, string colName);

        /// <summary>
        /// Set protected one sheet
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="password">the protected password </param>
        void LockSheet(string sheetName, string password);

        #endregion

        #region FreezePane

        /// <summary>
        /// FreezePane
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowSplit">the row's index </param>
        /// <param name="colSplit">the column's index</param>
        void FreezePane(string sheetName, int rowSplit, int colSplit);

        /// <summary>
        /// FreezePane
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowSplit">the row's index </param>
        /// <param name="colSplitName">the column's name (A,B,...)</param>
        void FreezePane(string sheetName, int rowSplit, string colSplitName);

        /// <summary>
        /// FreezePane
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="rowSplit">the split row's index</param>
        /// <param name="colSplit">the split column's index</param>
        /// <param name="leftmostColumn">left column</param>
        /// <param name="topRow">top row</param>
        void FreezePane(string sheetName, int rowSplit, int colSplit, int leftmostColumn, int topRow);

        #endregion

        #region Set DropDownList to Cell

        /// <summary>
        /// Validate value of one cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index</param>
        /// <param name="listValues">list values validate</param>
        void SetDropDownListCell(string sheetName, int row, int col, List<string> listValues);

        /// <summary>
        /// Validate value of one cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="colName">the column's name</param>
        /// <param name="listValues">list values validate</param>
        void SetDropDownListCell(string sheetName, int row, string colName, List<string> listValues);

        /// <summary>
        /// Validate value of one cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index</param>
        /// <param name="listValues">list values validate</param>
        void SetDropDownListCell(string sheetName, int row, int col, string[] listValues);

        /// <summary>
        /// Validate value of one cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="col">the column's index</param>
        /// <param name="formula">formula presente the values (example "Sheet2!$E1:$E3")</param>        
        void SetDropDownListCell(string sheetName, int row, int col, string formula);

        /// <summary>
        /// Validate value of one cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="row">the row's index (one-base)</param>
        /// <param name="colName">the column's name</param>
        /// <param name="formula">formula presente the values (example "Sheet2!$E1:$E3")</param>
        void SetDropDownListCell(string sheetName, int row, string colName, string formula);

        /// <summary>
        /// Validate value of the range of the cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="firstRow">the first row's index</param>
        /// <param name="firstCol">the first column's index</param>
        /// <param name="lastRow">the last row's index</param>
        /// <param name="lastCol">the last column's index</param>
        /// <param name="listValues">list of value</param>
        void SetDropDownListRangeCell(string sheetName, int firstRow, int firstCol, int lastRow, int lastCol, List<string> listValues);

        /// <summary>
        /// Validate value of the cell
        /// </summary>
        /// <param name="sheetName">the sheet's name</param>
        /// <param name="firstRow">the first row's index</param>
        /// <param name="firstCol">the first col's index</param>
        /// <param name="lastRow">the last row's index</param>
        /// <param name="lastCol">the last row's index</param>
        /// <param name="listValues"></param>
        void SetDropDownListRangeCell(string sheetName, int firstRow, int firstCol, int lastRow, int lastCol, string[] listValues);

        /// <summary>
        /// Validate the value of range of the cell
        /// </summary>
        /// <param name="sheetName">sheet's name</param>
        /// <param name="firstRow">the first row's index (one-base)</param>
        /// <param name="firstCol">the first column's index</param>
        /// <param name="lastRow">the last row's index</param>
        /// <param name="lastCol">the last columne's index </param>
        /// <param name="formula">the formula present the range of value</param>
        void SetDropDownListRangeCell(string sheetName, int firstRow, int firstCol, int lastRow, int lastCol, string formula);

        #endregion

        #endregion

        #region Save
        /// <summary>
        /// Save to file
        /// </summary>
        /// <param name="pathName">the path of the file</param>
        void Save(string pathName);

        /// <summary>
        /// Save to stream
        /// </summary>
        /// <param name="streamExcel">stream's name</param>
        void Save(ref Stream streamExcel);      

        #endregion
        
        #endregion

    }
}
