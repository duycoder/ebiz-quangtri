using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NativeExcel;

namespace VTUtils.Excel.Export
{
    public class VTWorksheet : IVTWorksheet
    {
        #region Declaration
        private VTXPageSize vTXPageSize = VTXPageSize.VTxlPaperA4;
        private VTXPageOrientation vTXPageOrientation = VTXPageOrientation.VTxlPortrait;
        private const string DEFAULT_PASSWORD = "@Bm17112011"; // Default pass when set passWord

        public NativeExcel.IWorksheet Worksheet { get; set; }

        public VTWorksheet(NativeExcel.IWorksheet worksheet)
        {
            Worksheet = worksheet;
        }

        public IVTRange GetRange(int firstRow, int firstCoumn, int lastRow, int lastColumn)
        {
            return new VTRange(Worksheet.Range[firstRow, firstCoumn, lastRow, lastColumn], this);
        }

        public IVTRange GetRange(string cell1, string cell2)
        {
            return new VTRange(Worksheet.Range[cell1, cell2], this);
        }

        public IVTRange GetRow(int row)
        {
            return new VTRange(Worksheet.Range[row, 1, row, 300], this);
        }

        public IVTRange GetUsedRange()
        {
            IVTRange range = new VTRange(Worksheet.UsedRange, this);
            return range;
        }

        /// <summary>
        /// Return number of rows used
        /// </summary>
        /// <returns></returns>
        public int GetCountUsedRows()
        {
            return Worksheet.UsedRange.Rows.Count;
        }

        /// <summary>
        /// Return number of columns used
        /// </summary>
        /// <returns></returns>
        public int GetCountUsedColumns()
        {
            return Worksheet.UsedRange.Columns.Count;
        }

        /// <summary>
        /// Property Hiden of sheet
        /// <author>Hungnd 20/12/2013</author>
        /// </summary>
        public string Name
        {
            get
            {
                return Worksheet.Name;
            }
            set
            {
                Worksheet.Name = this.StripVNSign(value);
            }
        }

        /// <summary>
        /// Property Hiden of sheet
        /// <author>Hungnd 17/03/2014</author>
        /// </summary>
        public VTSheetVisibility Hiden
        {
            get
            {
                if (Worksheet.Visible == XlSheetVisibility.xlSheetHidden)
                {
                    return VTSheetVisibility.VTXSheetHidden;
                }
                else if (Worksheet.Visible == XlSheetVisibility.xlSheetHidden)
                {
                    return VTSheetVisibility.VTXSheetVeryHidden;
                }
                else
                {
                    return VTSheetVisibility.VTXSheetVisible;
                }
            }
            set
            {
                if (value == VTSheetVisibility.VTXSheetHidden)
                {
                    Worksheet.Visible = XlSheetVisibility.xlSheetHidden;
                }
                else if (value == VTSheetVisibility.VTXSheetVeryHidden)
                {
                    Worksheet.Visible = XlSheetVisibility.xlSheetVeryHidden;
                }
                else
                {
                    Worksheet.Visible = XlSheetVisibility.xlSheetVisible;
                }
            }
        }

        #endregion

        #region Format


        public void CopyPaste(IVTRange range, int firstRow, int firstColumn, bool copyStyleOnly = false)
        {
            NativeExcel.IRange iRange = range.Range;
            NativeExcel.IRange oRange = Worksheet.Range[firstRow, firstColumn, firstRow + iRange.Rows.Count - 1,
                firstColumn + iRange.Columns.Count - 1];

            XlPasteType type = copyStyleOnly ? XlPasteType.xlPasteFormats : XlPasteType.xlPasteAll;
            iRange.Copy(oRange, type);
        }

        public void CutPaste(IVTRange range, int firstRow, int firstColumn)
        {
            NativeExcel.IRange iRange = range.Range;
            NativeExcel.IRange oRange = Worksheet.Range[firstRow, firstColumn, firstRow + iRange.Rows.Count - 1,
                firstColumn + iRange.Columns.Count - 1];

            XlPasteType type = XlPasteType.xlPasteAll;
            iRange.Copy(oRange, type);
            oRange.Delete();
        }

        public void CopyPasteSameRowHeigh(IVTRange range, int firstRow, bool copyStyleOnly = false)
        {
            NativeExcel.IRange iRange = range.Range;
            int firstColumn = iRange.Column;
            CopyPasteSameRowHeigh(range, firstRow, firstColumn, copyStyleOnly);
        }

        public void CopyPasteSameRowHeigh(IVTRange range, int firstRow, int firstColumn, bool copyStyleOnly = false)
        {
            NativeExcel.IRange iRange = range.Range;
            NativeExcel.IRange oRange = Worksheet.Range[firstRow, firstColumn, firstRow + iRange.Rows.Count - 1,
                firstColumn + iRange.Columns.Count - 1];

            XlPasteType type = copyStyleOnly ? NativeExcel.XlPasteType.xlPasteFormats : NativeExcel.XlPasteType.xlPasteAll;

            iRange.Copy(oRange, type);
            List<double> listHeight = new List<double>();
            for (int i = 1; i <= iRange.Rows.Count; i++)
            {
                listHeight.Add(iRange.Rows[i].RowHeight);
            }
            for (int i = 1; i <= iRange.Rows.Count; i++)
            {
                oRange.Rows[i].RowHeight = listHeight[i - 1];
            }
        }

        public void CopyPasteSameColumnWidth(IVTRange range, int firstRow, int firstColumn, bool copyStyleOnly = false)
        {
            NativeExcel.IRange iRange = range.Range;
            NativeExcel.IRange oRange = Worksheet.Range[firstRow, firstColumn, firstRow + iRange.Rows.Count - 1,
                firstColumn + iRange.Columns.Count - 1];

            XlPasteType type = copyStyleOnly ? NativeExcel.XlPasteType.xlPasteFormats : NativeExcel.XlPasteType.xlPasteAll;

            iRange.Copy(oRange, type);
            List<double> listWidth = new List<double>();
            for (int i = 1; i <= iRange.Columns.Count; i++)
            {
                listWidth.Add(iRange.Columns[i].ColumnWidth);
            }
            for (int i = 1; i <= iRange.Columns.Count; i++)
            {
                oRange.Columns[i].ColumnWidth = listWidth[i - 1];
            }
        }

        public void CopyPasteSameSize(IVTRange range, int firstRow, int firstColumn)
        {
            NativeExcel.IRange iRange = range.Range;
            NativeExcel.IRange oRange = Worksheet.Range[firstRow, firstColumn, firstRow + iRange.Rows.Count - 1,
                firstColumn + iRange.Columns.Count - 1];
            iRange.Copy(oRange, NativeExcel.XlPasteType.xlPasteAll);
            List<double> listHeight = new List<double>();
            for (int i = 1; i <= iRange.Rows.Count; i++)
            {
                listHeight.Add(iRange.Rows[i].RowHeight);
            }
            for (int i = 1; i <= iRange.Rows.Count; i++)
            {
                oRange.Rows[i].RowHeight = listHeight[i - 1];
            }
            List<double> listWidth = new List<double>();
            for (int i = 1; i <= iRange.Columns.Count; i++)
            {
                listWidth.Add(iRange.Columns[i].ColumnWidth);
            }
            for (int i = 1; i <= iRange.Columns.Count; i++)
            {
                oRange.Columns[i].ColumnWidth = listWidth[i - 1];
            }
        }

        public void CopyPasteSameSize(IVTRange range, string cell)
        {
            VTVector vec = new VTVector(cell);
            CopyPasteSameSize(range, vec.X, vec.Y);
        }

        public void SetCellValue(string cell, object value)
        {
            NativeExcel.IRange iCell = Worksheet.Range[cell];
            iCell.Formula = value;
        }

        public void SetCellValue(int row, int column, object value)
        {
            NativeExcel.IRange iCell = Worksheet.Range[row, column];
            iCell.Formula = value;
        }
        public object GetCellValue(int row, int column)
        {
            NativeExcel.IRange iCell = Worksheet.Range[row, column];
            object Value;
            Value = iCell.Value;
            return Value;
        }

        public object GetCellValue(string cell)
        {
            NativeExcel.IRange iCell = Worksheet.Range[cell];
            object Value;
            Value = iCell.Value;
            return Value;
        }

        public void SetCellValue(VTVector vector, object value)
        {
            SetCellValue(vector.X, vector.Y, value);
        }

        public void InsertRow(int insertRow)
        {
            IRange aboveRange = Worksheet.Range["A" + insertRow, "A" + insertRow];
            aboveRange.EntireRow.Insert(XlInsertShiftDirection.xlShiftDown);
        }

        public void CopyAndInsertARow(int copiedRow, int insertRow, bool copyStyleOnly = false)
        {
            if (copiedRow >= insertRow)
            {
                copiedRow++;
            }
            IRange aboveRange = Worksheet.Range["A" + insertRow, "A" + insertRow];
            aboveRange.EntireRow.Insert(XlInsertShiftDirection.xlShiftDown);
            IVTRange copiedRange = GetRange(copiedRow, 1, copiedRow, Worksheet.UsedRange.Columns.Count + Worksheet.UsedRange.Column);
            CopyPasteSameSize(copiedRange, insertRow, 1);
        }

        public void CopyAndInsertARow(IVTRange copiedRow, int insertRow, bool copyStyleOnly = false)
        {
            IRange aboveRange = Worksheet.Range["A" + insertRow, "A" + insertRow];
            aboveRange.EntireRow.Insert(XlInsertShiftDirection.xlShiftDown);
            CopyPasteSameSize(copiedRow, insertRow, 1);
        }

        public double GetRowHeight(int row)
        {
            return Worksheet.Range[row, 1].RowHeight;
        }

        public void SetRowHeight(int row, double height)
        {
            Worksheet.Range[row, 1].Rows[1].RowHeight = height;
        }

        public void CopySheet(IVTWorksheet worksheet)
        {
            NativeExcel.IWorksheet templateSheet = worksheet.Worksheet;
            NativeExcel.IRange usedRang = templateSheet.UsedRange;
            IVTRange vtRange = worksheet.GetRange(1, 1, usedRang.Rows.Count + usedRang.Row - 1, usedRang.Columns.Count + 1);
            CopyPasteSameSize(vtRange, 1, 1);
            CopyPageSetup(templateSheet);
        }

        public void CopySheet(IVTWorksheet worksheet, string lastCell)
        {
            NativeExcel.IWorksheet templateSheet = worksheet.Worksheet;
            NativeExcel.IRange usedRang = templateSheet.UsedRange;
            IVTRange vtRange = worksheet.GetRange("A1", lastCell);
            CopyPasteSameSize(vtRange, 1, 1);
            CopyPageSetup(templateSheet);
        }

        private void CopyPageSetup(NativeExcel.IWorksheet templateSheet)
        {
            Worksheet.PageSetup.Orientation = templateSheet.PageSetup.Orientation;
            Worksheet.PageSetup.HeaderMargin = templateSheet.PageSetup.HeaderMargin;
            Worksheet.PageSetup.FooterMargin = templateSheet.PageSetup.FooterMargin;
            Worksheet.PageSetup.BottomMargin = templateSheet.PageSetup.BottomMargin;
            Worksheet.PageSetup.TopMargin = templateSheet.PageSetup.TopMargin;
            Worksheet.PageSetup.LeftMargin = templateSheet.PageSetup.LeftMargin;
            Worksheet.PageSetup.RightMargin = templateSheet.PageSetup.RightMargin;
            Worksheet.PageSetup.PaperSize = templateSheet.PageSetup.PaperSize;
            Worksheet.PageSetup.Zoom = templateSheet.PageSetup.Zoom;
            Worksheet.PageSetup.PrintTitleColumns = templateSheet.PageSetup.PrintTitleColumns;
            Worksheet.PageSetup.PrintTitleRows = templateSheet.PageSetup.PrintTitleRows;
            Worksheet.FreezePanes = templateSheet.FreezePanes;
            Worksheet.ScrollRow = templateSheet.ScrollRow;
            Worksheet.ScrollColumn = templateSheet.ScrollColumn;
            Worksheet.SplitRow = templateSheet.SplitRow;
            Worksheet.SplitColumn = templateSheet.SplitColumn;
            Worksheet.Split = templateSheet.Split;
            Worksheet.PageSetup.PrintArea = templateSheet.PageSetup.PrintArea;
        }

        public void Delete()
        {
            Worksheet.Delete();
        }

        public void FillDataHorizon<T>(List<T> listData, VTVector firstPosition, int length = 0)
        {
            if (listData != null)
            {
                if (length <= 0 || listData.Count <= length)
                {
                    for (int i = 0; i < listData.Count; i++)
                    {
                        SetCellValue(firstPosition + new VTVector(0, i), listData[i]);
                    }
                }
                else
                {
                    IRange range = Worksheet.Range[firstPosition.X, firstPosition.Y, firstPosition.X, firstPosition.Y + length - 1];
                    range.Merge();
                    range.ShrinkToFit = true;
                    StringBuilder data = new StringBuilder("");
                    for (int i = 0; i < listData.Count; i++)
                    {
                        data.Append(listData[i]);
                        if (i != listData.Count)
                        {
                            data.Append(" ");
                        }
                    }
                    SetCellValue(firstPosition, data);
                }
            }
        }

        public void FillDataHorizon<T>(List<T> listData, int x, int y, int length = 0)
        {
            FillDataHorizon(listData, new VTVector(x, y), length);
        }

        public void SetPrintArea(string value)
        {
            Worksheet.PageSetup.PrintArea = value;
        }

        public void FillVariableValue(Dictionary<string, object> Data, IVTWorksheet OtherRange = null)
        {
            IVTRange range = new VTRange(Worksheet.UsedRange, this);
            range.FillVariableValue(Data, OtherRange);
        }

        public void DeleteRow(int RowNum)
        {
            Worksheet.Range["A" + RowNum].EntireRow.Delete();
        }

        public void ClearFormulaKeepValue()
        {
            IRange range = this.Worksheet.UsedRange;

            for (int r = 1; r <= range.Rows.Count; r++)
            {
                for (int c = 1; c <= range.Columns.Count; c++)
                {
                    range[r, c].Formula = range[r, c].Value;
                }
            }
        }

        public void DeleteAllHideRow()
        {
            IRange range = this.Worksheet.UsedRange;
            List<int> ListDelete = new List<int>();
            for (int r = 1; r <= range.Rows.Count; r++)
            {
                if (range[r, 1].EntireRow.Hidden)
                {
                    ListDelete.Add(range.Row + r - 1);
                }
            }

            for (int i = ListDelete.Count - 1; i >= 0; i--)
            {
                this.DeleteRow(ListDelete[i]);
            }
        }

        public void DeleteAllHideColumn()
        {
            IRange range = this.Worksheet.UsedRange;
            List<int> ListDelete = new List<int>();
            for (int c = 1; c <= range.Columns.Count; c++)
            {
                if (range[1, c].EntireColumn.Hidden)
                {
                    ListDelete.Add(range.Column + c - 1);
                }
            }

            for (int i = ListDelete.Count - 1; i >= 0; i--)
            {
                this.DeleteColumn(ListDelete[i]);
            }
        }

        public void DeleteColumn(int ColumnIndex)
        {
            //Worksheet.Cells[1, ColumnIndex].Columns.Delete();
            Worksheet.Cells.Columns[ColumnIndex].Delete();
        }

        public void MergeRow(int row, int firstCol, int lastCol)
        {
            Worksheet.Range[row, firstCol, row, lastCol].Merge();
        }

        public void MergeRow(int row, char firstCol, char lastCol)
        {
            Worksheet.Range[row, VTVector.dic[firstCol], row, VTVector.dic[lastCol]].Merge();
        }

        public void MergeColumn(int col, int firstRow, int lastRow)
        {
            Worksheet.Range[firstRow, col, lastRow, col].Merge();
        }

        public void MergeColumn(char col, int firstRow, int lastRow)
        {
            Worksheet.Range[firstRow, VTVector.dic[col], lastRow, VTVector.dic[col]].Merge();
        }

        public double GetColumnWidth(char column)
        {
            return GetColumnWidth(VTVector.dic[column]);
        }

        public double GetColumnWidth(int column)
        {
            return Worksheet.Range[1, column].ColumnWidth;
        }

        public void SetColumnWidth(char column, double width)
        {
            SetColumnWidth(VTVector.dic[column], width);
        }

        public void SetColumnWidth(int column, double width)
        {
            Worksheet.Range[1, column].Columns[1].ColumnWidth = width;
        }

        public void SetFormulaValue(int row, int column, object value)
        {
            Worksheet.Cells[row, column].Formula = value;
        }

        public void SetFormulaValue(string cell, object value)
        {
            Worksheet.Cells[cell].Formula = value;
        }

        public void Lock(IVTWorksheet workSheet, int row1, int col1, int row2, int col2)
        {
            NativeExcel.IWorksheet templateSheet = workSheet.Worksheet;
            NativeExcel.IRange usedRang = templateSheet.Range[row1, col1, row2, col2];
            usedRang.Locked = true;
            usedRang.Font.Color = System.Drawing.Color.Black;
            usedRang.Interior.Color = System.Drawing.Color.Yellow;
            //usedRang.Font.Strikethrough = true;
        }
        /// <summary>
        ///Lock cell cua hoc sinh mien giam 
        /// </summary>
        public void LockCellExemptType(IVTWorksheet workSheet, int row1, int col1, int row2, int col2)
        {
            NativeExcel.IWorksheet templateSheet = workSheet.Worksheet;
            NativeExcel.IRange usedRang = templateSheet.Range[row1, col1, row2, col2];
            usedRang.Locked = true;
            usedRang.EntireRow.Locked = true;
        }
        /// <summary>
        /// chuyển thành tiếng việt không dấu
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string StripVNSign(string text)
        {
            //Ky tu dac biet

            /*for (int i = 32; i < 48; i++)
            {
                text = text.Replace(((char)i).ToString(), "-");
            }*/
            //text = text.Replace(".", "-");
            //text = text.Replace(" ", "-");
            //text = text.Replace(",", "-");
            //text = text.Replace(";", "-");
            //text = text.Replace(":", "-");
            //'Dấu Ngang
            text = text.Replace("A", "A");
            text = text.Replace("a", "a");
            text = text.Replace("Ă", "A");
            text = text.Replace("ă", "a");
            text = text.Replace("Â", "A");
            text = text.Replace("â", "a");
            text = text.Replace("E", "E");
            text = text.Replace("e", "e");
            text = text.Replace("Ê", "E");
            text = text.Replace("ê", "e");
            text = text.Replace("I", "I");
            text = text.Replace("i", "i");
            text = text.Replace("O", "O");
            text = text.Replace("o", "o");
            text = text.Replace("Ô", "O");
            text = text.Replace("ô", "o");
            text = text.Replace("Ơ", "O");
            text = text.Replace("ơ", "o");
            text = text.Replace("U", "U");
            text = text.Replace("u", "u");
            text = text.Replace("Ư", "U");
            text = text.Replace("ư", "u");
            text = text.Replace("Y", "Y");
            text = text.Replace("y", "y");

            //    'Dấu Huyền
            text = text.Replace("À", "A");
            text = text.Replace("à", "a");
            text = text.Replace("Ằ", "A");
            text = text.Replace("ằ", "a");
            text = text.Replace("Ầ", "A");
            text = text.Replace("ầ", "a");
            text = text.Replace("È", "E");
            text = text.Replace("è", "e");
            text = text.Replace("Ề", "E");
            text = text.Replace("ề", "e");
            text = text.Replace("Ì", "I");
            text = text.Replace("ì", "i");
            text = text.Replace("Ò", "O");
            text = text.Replace("ò", "o");
            text = text.Replace("Ồ", "O");
            text = text.Replace("ồ", "o");
            text = text.Replace("Ờ", "O");
            text = text.Replace("ờ", "o");
            text = text.Replace("Ù", "U");
            text = text.Replace("ù", "u");
            text = text.Replace("Ừ", "U");
            text = text.Replace("ừ", "u");
            text = text.Replace("Ỳ", "Y");
            text = text.Replace("ỳ", "y");

            //'Dấu Sắc
            text = text.Replace("Á", "A");
            text = text.Replace("á", "a");
            text = text.Replace("Ắ", "A");
            text = text.Replace("ắ", "a");
            text = text.Replace("Ấ", "A");
            text = text.Replace("ấ", "a");
            text = text.Replace("É", "E");
            text = text.Replace("é", "e");
            text = text.Replace("Ế", "E");
            text = text.Replace("ế", "e");
            text = text.Replace("Í", "I");
            text = text.Replace("í", "i");
            text = text.Replace("Ó", "O");
            text = text.Replace("ó", "o");
            text = text.Replace("Ố", "O");
            text = text.Replace("ố", "o");
            text = text.Replace("Ớ", "O");
            text = text.Replace("ớ", "o");
            text = text.Replace("Ú", "U");
            text = text.Replace("ú", "u");
            text = text.Replace("Ứ", "U");
            text = text.Replace("ứ", "u");
            text = text.Replace("Ý", "Y");
            text = text.Replace("ý", "y");

            //'Dấu Hỏi
            text = text.Replace("Ả", "A");
            text = text.Replace("ả", "a");
            text = text.Replace("Ẳ", "A");
            text = text.Replace("ẳ", "a");
            text = text.Replace("Ẩ", "A");
            text = text.Replace("ẩ", "a");
            text = text.Replace("Ẻ", "E");
            text = text.Replace("ẻ", "e");
            text = text.Replace("Ể", "E");
            text = text.Replace("ể", "e");
            text = text.Replace("Ỉ", "I");
            text = text.Replace("ỉ", "i");
            text = text.Replace("Ỏ", "O");
            text = text.Replace("ỏ", "o");
            text = text.Replace("Ổ", "O");
            text = text.Replace("ổ", "o");
            text = text.Replace("Ở", "O");
            text = text.Replace("ở", "o");
            text = text.Replace("Ủ", "U");
            text = text.Replace("ủ", "u");
            text = text.Replace("Ử", "U");
            text = text.Replace("ử", "u");
            text = text.Replace("Ỷ", "Y");
            text = text.Replace("ỷ", "y");

            //'Dấu Ngã   
            text = text.Replace("Ã", "A");
            text = text.Replace("ã", "a");
            text = text.Replace("Ẵ", "A");
            text = text.Replace("ẵ", "a");
            text = text.Replace("Ẫ", "A");
            text = text.Replace("ẫ", "a");
            text = text.Replace("Ẽ", "E");
            text = text.Replace("ẽ", "e");
            text = text.Replace("Ễ", "E");
            text = text.Replace("ễ", "e");
            text = text.Replace("Ĩ", "I");
            text = text.Replace("ĩ", "i");
            text = text.Replace("Õ", "O");
            text = text.Replace("õ", "o");
            text = text.Replace("Ỗ", "O");
            text = text.Replace("ỗ", "o");
            text = text.Replace("Ỡ", "O");
            text = text.Replace("ỡ", "o");
            text = text.Replace("Ũ", "U");
            text = text.Replace("ũ", "u");
            text = text.Replace("Ữ", "U");
            text = text.Replace("ữ", "u");
            text = text.Replace("Ỹ", "Y");
            text = text.Replace("ỹ", "y");

            //'Dẫu Nặng
            text = text.Replace("Ạ", "A");
            text = text.Replace("ạ", "a");
            text = text.Replace("Ặ", "A");
            text = text.Replace("ặ", "a");
            text = text.Replace("Ậ", "A");
            text = text.Replace("ậ", "a");
            text = text.Replace("Ẹ", "E");
            text = text.Replace("ẹ", "e");
            text = text.Replace("Ệ", "E");
            text = text.Replace("ệ", "e");
            text = text.Replace("Ị", "I");
            text = text.Replace("ị", "i");
            text = text.Replace("Ọ", "O");
            text = text.Replace("ọ", "o");
            text = text.Replace("Ộ", "O");
            text = text.Replace("ộ", "o");
            text = text.Replace("Ợ", "O");
            text = text.Replace("ợ", "o");
            text = text.Replace("Ụ", "U");
            text = text.Replace("ụ", "u");
            text = text.Replace("Ự", "U");
            text = text.Replace("ự", "u");
            text = text.Replace("Ỵ", "Y");
            text = text.Replace("ỵ", "y");
            text = text.Replace("Đ", "D");
            text = text.Replace("đ", "d");
            text = StandartClassName(text);
            return text;
        }
        //HoanTV5
        /// <summary>
        /// chuyển những ký tự đặc biệt thành dấu "_" khi đẩy vào sheetName
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string StandartClassName(string str)
        {
            if (str.Contains("/"))
            {
                str = str.Replace("/", "_");
            }
            if (str.Contains("["))
            {
                str = str.Replace("[", "_");
            }
            if (str.Contains("]"))
            {
                str = str.Replace("]", "_");
            }
            if (str.Contains(":"))
            {
                str = str.Replace(":", "_");
            }
            if (str.Contains("*"))
            {
                str = str.Replace("*", "_");
            }

            return str;
        }

        /// <summary>
        /// Protect with manual pass
        /// Hungnd8 28/03/2013
        /// </summary>
        /// <param name="passWord"></param>
        public void ProtectSheet(string passWord)
        {
            Worksheet.Protect(passWord);
        }

        /// <summary>
        /// Protect with auto pass
        /// Hungnd8 28/03/2013
        /// </summary>
        public void ProtectSheet()
        {
            Worksheet.Protect(DEFAULT_PASSWORD);
        }

        /// <summary>
        /// Lock all cell in sheet
        /// Hungnd8 28/03/2013
        /// </summary>
        public bool LockSheet
        {
            get
            {
                return Worksheet.Cells.Locked;
            }
            set
            {
                Worksheet.Cells.Locked = value;
            }
        }

        /// <summary>
        /// Hide column by index
        /// 10/05/2013 Hungnd8
        /// </summary>
        /// <param name="ColumnIndex"></param>
        public void HideColumn(int ColumnIndex)
        {
            Worksheet.Cells.Columns[ColumnIndex].Hidden = true;
        }

        public void PasteRange(IVTRange sourceRange, IVTRange targerRange)
        {
            NativeExcel.IRange iRange = sourceRange.Range;
            iRange.Copy(targerRange.Range, XlPasteType.xlPasteAll);
        }
        public void SetBorder(VTBorderStyle boderStyle, VTBorderIndex borderIndex, int firstRow, int firstCoumn, int lastRow, int lastColumn)
        {
            IVTRange range = new VTRange(Worksheet.Range[firstRow, firstCoumn, lastRow, lastColumn], this);
            range.SetBorder(boderStyle, borderIndex);
        }
        #endregion

        #region Page Setup
        /// <summary>
        /// Set Print Area Ex: A1:C20
        /// <author>hungnd 30/10.2013</author>
        /// </summary>
        public string PrintArea
        {
            get
            {
                return Worksheet.PageSetup.PrintArea;
            }
            set
            {
                Worksheet.PageSetup.PrintArea = value;
            }
        }

        /// <summary>
        /// Fit All column to 1 page
        /// <author>hungnd 30/10.2013</author>
        /// </summary>
        public bool FitToPage
        {
            get
            {
                return Worksheet.PageSetup.FitToPagesWide == 1;
            }
            set
            {
                if (value == true)
                {
                    Worksheet.ResetAllPageBreaks();
                    Worksheet.PageSetup.FitToPagesWide = 1;
                }
            }
        }

        /// <summary>
        /// Fit Sheet On One Page (Shrink the printout so that it fit on one page)
        /// <author>hungnd 25/11/2013</author>
        /// </summary>
        public bool FitSheetOnOnePage
        {
            get
            {
                if (Worksheet.PageSetup.FitToPagesTall == 1 && Worksheet.PageSetup.FitToPagesWide == 1)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                {
                    Worksheet.PageSetup.FitToPagesTall = 1;
                    Worksheet.PageSetup.FitToPagesWide = 1;
                }
            }
        }

        /// <summary>
        /// Fit All Columns On One Page (Shrink the printout so that it fit on one page wide)
        /// <author>hungnd 25/11/2013</author>
        /// </summary>
        public bool FitAllColumnsOnOnePage
        {
            get
            {
                if (Worksheet.PageSetup.FitToPagesTall == 0 && Worksheet.PageSetup.FitToPagesWide == 1)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                {
                    Worksheet.PageSetup.FitToPagesTall = 0;
                    Worksheet.PageSetup.FitToPagesWide = 1;
                }
            }
        }

        /// <summary>
        /// Fit All Rows On One Page (Shrink the printout so that it fit on one page high)
        /// <author>hungnd 25/11/2013</author>
        /// </summary>
        public bool FitAllRowsOnOnePage
        {
            get
            {
                if (Worksheet.PageSetup.FitToPagesTall == 1 && Worksheet.PageSetup.FitToPagesWide == 0)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                {
                    Worksheet.PageSetup.FitToPagesTall = 1;
                    Worksheet.PageSetup.FitToPagesWide = 0;
                }
            }
        }

        /// <summary>
        /// Zoom page, No Scalling (Print sheets at their actual size)
        /// <author>hungnd 25/11/2013</author>
        /// </summary>
        public int ZoomPage
        {
            get
            {
                return Worksheet.PageSetup.Zoom;
            }
            set
            {
                Worksheet.PageSetup.Zoom = value;
            }
        }

        /// <summary>
        /// Set margin (Inches)
        /// Top, Left, Right, Bottom
        /// Ex: PageMagin = 2, 2, 3 , 3
        /// <author>hungnd 08/11.2013</author>
        /// </summary>
        public string PageMagin
        {
            get
            {
                string Margin = Worksheet.PageSetup.TopMargin.ToString();
                Margin = ", " + Worksheet.PageSetup.LeftMargin.ToString();
                Margin = ", " + Worksheet.PageSetup.RightMargin.ToString();
                Margin = ", " + Worksheet.PageSetup.BottomMargin.ToString();
                return Margin;
            }
            set
            {
                string Margin = value;
                List<string> lsMargin = Margin.Split(',').ToList();
                if (lsMargin.Count > 0)
                {
                    Worksheet.PageSetup.TopMargin = Convert.ToDouble(lsMargin[0]);
                    if (lsMargin.Count > 1)
                        Worksheet.PageSetup.LeftMargin = Convert.ToDouble(lsMargin[1]);
                    if (lsMargin.Count > 2)
                        Worksheet.PageSetup.RightMargin = Convert.ToDouble(lsMargin[2]);
                    else
                        Worksheet.PageSetup.BottomMargin = Convert.ToDouble(lsMargin[3]);
                }
            }
        }

        /// <summary>
        /// Set Margin top (Inches)
        /// <author>hungnd 08/11.2013</author>
        /// </summary>
        public double PageMaginTop
        {
            get
            {
                return Worksheet.PageSetup.TopMargin;
            }
            set
            {
                Worksheet.PageSetup.TopMargin = value;
            }
        }

        /// <summary>
        /// Set Margin left (Inches)
        /// <author>hungnd 08/11.2013</author>
        /// </summary>
        public double PageMaginLeft
        {
            get
            {
                return Worksheet.PageSetup.LeftMargin;
            }
            set
            {
                Worksheet.PageSetup.LeftMargin = value;
            }
        }

        /// <summary>
        /// Set Margin top (Inches)
        /// <author>hungnd 08/11.2013</author>
        /// </summary>
        public double PageMaginRight
        {
            get
            {
                return Worksheet.PageSetup.RightMargin;
            }
            set
            {
                Worksheet.PageSetup.RightMargin = value;
            }
        }

        /// <summary>
        /// Set Margin top (Inches)
        /// <author>hungnd 08/11.2013</author>
        /// </summary>
        public double PageMaginBottom
        {
            get
            {
                return Worksheet.PageSetup.BottomMargin;
            }
            set
            {
                Worksheet.PageSetup.BottomMargin = value;
            }
        }

        /// <summary>
        /// Set PageSize for Worksheet. value is VTUtils.Excel.Export.VTXPageSize
        /// Ex: None, A4, A5, A3 ....
        /// <author>hungnd 08/11.2013</author>
        /// </summary>
        public VTXPageSize PageSize
        {
            get
            {
                return vTXPageSize;
            }
            set
            {
                if (value == VTXPageSize.VTxlNone)
                {
                    Worksheet.PageSetup.PaperSize = XlPaperSize.xlNone;
                }
                if (value == VTXPageSize.VTxlPaperA3)
                {
                    Worksheet.PageSetup.PaperSize = XlPaperSize.xlPaperA3;
                }
                if (value == VTXPageSize.VTxlPaperA4)
                {
                    Worksheet.PageSetup.PaperSize = XlPaperSize.xlPaperA4;
                }
                if (value == VTXPageSize.VTxlPaperA5)
                {
                    Worksheet.PageSetup.PaperSize = XlPaperSize.xlPaperA5;
                }
                if (value == VTXPageSize.VTxlPaperExecutive)
                {
                    Worksheet.PageSetup.PaperSize = XlPaperSize.xlPaperExecutive;
                }
                if (value == VTXPageSize.VTxlPaperLetter)
                {
                    Worksheet.PageSetup.PaperSize = XlPaperSize.xlPaperLetter;
                }
            }
        }

        /// <summary>
        /// Sets the first sheet to be printed in landscape orientation
        /// <author>hungnd 08/11.2013</author>
        /// </summary>
        public VTXPageOrientation Orientation
        {
            get
            {
                return vTXPageOrientation;
            }
            set
            {
                //Page Landscape mode.
                if (value == VTXPageOrientation.VTxlLandscape)
                {
                    Worksheet.PageSetup.Orientation = XlPageOrientation.xlLandscape;
                }
                //Page Portrait mode.
                if (value == VTXPageOrientation.VTxlPortrait)
                {
                    Worksheet.PageSetup.Orientation = XlPageOrientation.xlPortrait;
                }
            }
        }

        /// <summary>
        /// Set PageBreak
        /// </summary>
        /// <param name="row"></param>
        public void SetBreakPage(int breakRangeRow)
        {
            Worksheet.HPageBreaks.Add(breakRangeRow);
        }
        #endregion
    }
}
