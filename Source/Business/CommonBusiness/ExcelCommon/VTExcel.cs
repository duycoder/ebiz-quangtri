using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NPOI.SS.UserModel;
using System.Reflection;
using NPOI.HSSF.UserModel;
using System.Data;
using NPOI.SS.Util;
using NPOI.HPSF;
using VTUtils.Excel.Style;
using NPOI.XSSF.UserModel;

namespace VTUtils.Excel
{
    public class VTExcel:IVTExcel
    {
        private IWorkbook VTWorkbook;        
        private int CurrentRow;
        private int CurrentColumn;
        private int CurrentSheet;
        private string TypeXLS;
        private Dictionary<string, ICellStyle> StyleTable ;
        private IClientAnchor AnchorDefault;
        

        #region contructor

        public VTExcel()
        {
            VTWorkbook = new HSSFWorkbook();            
            CurrentRow = 0;
            CurrentColumn = 0;
            StyleTable = new System.Collections.Generic.Dictionary<string, ICellStyle>();
            AnchorDefault = new HSSFClientAnchor(0, 0, 0, 0, 1, 1, 3, 3);
            TypeXLS = "xls";
            //set default value...
        }

        public VTExcel(string excelType)
        {

            if (excelType.Trim() == "xls")
            {
                VTWorkbook = new HSSFWorkbook();
                AnchorDefault = new HSSFClientAnchor(0, 0, 0, 0, 1, 1, 3, 3);
                TypeXLS = "xls";
            }
            else
            {
                VTWorkbook = new XSSFWorkbook();
                AnchorDefault = new XSSFClientAnchor(0, 0, 0, 0, 1, 1, 3, 3);
                TypeXLS = "xlsx";
            }
            CurrentRow = 0;
            CurrentColumn = 0;
            StyleTable = new System.Collections.Generic.Dictionary<string, ICellStyle>();
            
            //set default value...
        }


        #endregion

        #region import

        public void LoadTemplate(string xlsPathName)
        {
            if (File.Exists(xlsPathName))
            {
                string extension = Path.GetExtension(xlsPathName);
                if (!string.IsNullOrEmpty(extension))
                {
                    using (FileStream file = new FileStream(xlsPathName, FileMode.Open, FileAccess.Read))
                    {
                        if (extension == ".xls")
                        {
                            VTWorkbook = new HSSFWorkbook(file);
                        }
                        else
                        {
                            if (extension == ".xlsx")
                            {
                                VTWorkbook = new XSSFWorkbook(file);
                                AnchorDefault = new XSSFClientAnchor(0, 0, 0, 0, 1, 1, 3, 3);
                                TypeXLS = "xlsx";
                            }
                        }
                        file.Close();
                        CurrentColumn = 0;
                        CurrentRow = 0;
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("File not found!");
            }
        }
                
        public void LoadTemplate(Stream xlsStream)
        {
            VTWorkbook = new HSSFWorkbook(xlsStream);
        }

        private void CalculAddress(int row, int col)
        {
            CurrentColumn = col - 1;
            CurrentRow = row - 1;
        }

        private void CalculColumn(int col)
        {
            CurrentColumn = col - 1;
        }

        private void CalculRow(int row)
        {
            CurrentRow = row - 1;
        }

        public bool hasValue(string sheetName, int row, int col)
        {
            CalculAddress(row, col);
            if (VTWorkbook.GetSheet(sheetName) == null)
                return false;
            if (CurrentRow < 0 || CurrentColumn < 0)
                return false;
            if (VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow) == null)
                return false;
            if (VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn) == null)
                return false;
            return true;
        }

        private bool IsCellExist(string sheetName, int row, int col)
        {
            
            if (VTWorkbook.GetSheet(sheetName) == null)
            {
                throw new Exception("The workSheet doesn't exist !");
            }

            if (row < 0 || col < 0)
            {
                throw new Exception("The index of row/col is not valid");
            }

            if (VTWorkbook.GetSheet(sheetName).GetRow(row) == null)
            {
                throw new Exception("The row does not exist");
            }

            if (VTWorkbook.GetSheet(sheetName).GetRow(row).GetCell(col) == null)
            {
                throw new Exception("The cell does not exist");
            }

            return true;
        }

        private int ColNameToNumber(string colName)
        {
            if (string.IsNullOrEmpty(colName)) throw new ArgumentNullException("columnName");

            char[] characters = colName.ToUpperInvariant().ToCharArray();

            int num = 0;

            for (int i = 0; i < characters.Length; i++)
            {
                num *= 26;
                num += (characters[i] - 'A' + 1);
            }

            return num;

        }
       
        public short GetCellType(string sheetName, int row, int col)
        {
            CalculAddress(row, col);
            IsCellExist(sheetName, CurrentRow, CurrentColumn);
            
            //Unknown = -1,
            //NUMERIC = 0,
            //STRING = 1,
            //FORMULA = 2,
            //BLANK = 3,
            //BOOLEAN = 4,
            //ERROR = 5,
            switch (VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellType)
            {
                case CellType.NUMERIC:
                    return 0;
                case CellType.STRING:
                    return 1;
                case CellType.FORMULA:
                    return 2;
                case CellType.BLANK:
                    return 3;
                case CellType.BOOLEAN:
                    return 4;
                case CellType.ERROR:
                    return 5;
                default:
                    return -1;

            }
            
        }

        public short GetCellType(string sheetName, int row, string colName)
        {
            int col = ColNameToNumber(colName);
            return GetCellType(sheetName, row, col);
        }
        
        public object GetCellValue(string sheetName, int row, int col)
        {
            CalculAddress(row, col);
            IsCellExist(sheetName, CurrentRow, CurrentColumn);

            string bankValue = "";
            switch (VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellType)
            {
                case CellType.STRING:
                    return VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).StringCellValue;
                case CellType.NUMERIC:
                    return VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).NumericCellValue;
                case CellType.BOOLEAN:
                    return VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).BooleanCellValue;
                case CellType.FORMULA:
                    //force to numeric
                    return VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).NumericCellValue;
                default:
                    return bankValue;
            }
        }

        public object GetCellValue(string sheetName, int row, string colName)
        {
            int col = ColNameToNumber(colName);
            return GetCellValue(sheetName, row, col);

        }

        public string GetCellFormula(string sheetName, int row, int col)
        {
            CalculAddress(row, col);
            IsCellExist(sheetName, CurrentRow, CurrentColumn);

            if (GetCellType(sheetName, row, col) != 2)
                throw new Exception("not formula");

            return VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellFormula;

        }

        public string GetCellFormula(string sheetName, int row, string colName)
        {
            int col = ColNameToNumber(colName);
            return GetCellFormula(sheetName, row, col);
        }    
        
        public DataTable ImportToDataTable(string sheetName)
        {
            DataTable dt = new DataTable();
            if (VTWorkbook.GetSheet(sheetName) != null)
            {
                ISheet sheet = VTWorkbook.GetSheet(sheetName);
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

                //header
                for (int j = 0; j < 5; j++)
                {
                    dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
                }

                while (rows.MoveNext())
                {
                    IRow row = (HSSFRow)rows.Current;
                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < row.LastCellNum; i++)
                    {
                        ICell cell = row.GetCell(i);

                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.ToString();
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }       

        public DataTable ImportToDataTable(string sheetName, int rowHeader)
        {
            DataTable dt = new DataTable();
            if (VTWorkbook.GetSheet(sheetName) != null)
            {
                HSSFSheet sheet = (HSSFSheet)VTWorkbook.GetSheet(sheetName);
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

                HSSFRow headerRow = (HSSFRow)sheet.GetRow(rowHeader);
                int cellCount = headerRow.LastCellNum;

                for (int j = 0; j < cellCount; j++)
                {
                    HSSFCell cell = (HSSFCell)headerRow.GetCell(j);
                    dt.Columns.Add(cell.ToString());
                }

                //for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                for (int i = (rowHeader + 1); i <= sheet.LastRowNum; i++)
                {
                    HSSFRow row = (HSSFRow)sheet.GetRow(i);
                    DataRow dataRow = dt.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            dataRow[j] = row.GetCell(j).ToString();
                        }
                    }

                    dt.Rows.Add(dataRow);
                }
            }
             
            return dt;              
            
        }

        //public List<T> DataTable2Object<T>(DataTable dt) where T : class
        //{
        //    List<T> ls = new List<T>();
        //    for (int j = 0; j < dt.Rows.Count; j++)
        //    {
        //        T obj;
        //        obj = Activator.CreateInstance<T>();

        //        PropertyInfo[] propertyInfo = obj.GetType().GetProperties();
        //        for (int i = 1; i < dt.Columns.Count; i++)
        //        {
        //            //each colum search property

        //            foreach (var item in propertyInfo)
        //            {
        //                if (dt.Columns[i].ColumnName.ToString() == item.Name.ToString())
        //                {
        //                    item.SetValue(obj, dt.Rows[j][dt.Columns[i].ColumnName.ToString()], null);
        //                }
        //            }
        //        }
        //        ls.Add(obj);
        //    }

        //    return ls;

        //}

        #endregion

        #region export

        #region DocumentInfo
        public void SetDocumentInfo(string company, string subject)
        {
            ////create a entry of DocumentSummaryInformation
            //DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //dsi.Company = company;
            //VTWorkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            //SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //si.Subject = subject;
            //VTWorkbook.SummaryInformation = si;
         
        }
        #endregion

        #region Sheet
        private void CalculSheet(int sheetIndex)
        {
            CurrentSheet = sheetIndex - 1;
        }

        public void CreateSheet(string sheetName)
        {
            if (VTWorkbook == null)
            {
                VTWorkbook = new HSSFWorkbook();
            }
            if (VTWorkbook.GetSheet(sheetName) == null)
            {
                VTWorkbook.CreateSheet(sheetName);
            }
        }
        
        public void RemoveSheetAt(int sheetIndex)
        {
            //convert to zero-base
            CalculSheet(sheetIndex);
            VTWorkbook.RemoveSheetAt(CurrentSheet);
        }

        public void SetSheetNameAt(int sheetIndex, string sheetName)
        {
            //convert to zero-base
            CalculSheet(sheetIndex);
            VTWorkbook.SetSheetName(CurrentSheet, sheetName);
        }

        public void SetSheetOrder(string sheetName, int pos)
        {
            if (TypeXLS == "xls")
            {
                VTWorkbook.SetSheetOrder(sheetName, pos);
            }
        }

        public void HideSheet(int sheetIndex, bool hidden)
        {
            CalculSheet(sheetIndex);
            //VTWorkbook.SetSheetHidden(CurrentSheet, hidden);

            if(hidden==true)
            VTWorkbook.SetSheetHidden(CurrentSheet, 1);
            else
                VTWorkbook.SetSheetHidden(CurrentSheet, 0);
        }

        public void SetActiveSheet(int sheetIndex)
        {
            CalculSheet(sheetIndex);
            VTWorkbook.SetActiveSheet(CurrentSheet);
        }

        public void SetSelectedSheet(int sheetIndex)
        {
            CalculSheet(sheetIndex);
            VTWorkbook.SetSelectedTab(CurrentSheet);
        }

        #endregion

        #region Row, Column

        public void CreateRow(string sheetName, int rownum)
        {
            CalculRow(rownum);
            if (VTWorkbook.GetSheet(sheetName) == null)
            {
                CreateSheet(sheetName);
            }
            if (VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow) == null)
                VTWorkbook.GetSheet(sheetName).CreateRow(CurrentRow);
        }

        public void SetColumnWidth(string sheetName, int columnIndex, int width)
        {
            CalculColumn(columnIndex);
            VTWorkbook.GetSheet(sheetName).SetColumnWidth(CurrentColumn, width);
        }

        public void SetColumnWidth(string sheetName, string colName, int width)
        {
            int col = ColNameToNumber(colName);
            SetColumnWidth(sheetName, col, width);
        }

        public void HideRow(string sheetName, int row, bool hide)
        {
            CalculRow(row);
            CreateRow(sheetName, row);
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).ZeroHeight = hide;

        }

        public void HideColumn(string sheetName, int col, bool hide)
        {
            CalculColumn(col);
            CreateSheet(sheetName);
            VTWorkbook.GetSheet(sheetName).SetColumnHidden(CurrentColumn, hide);
        }

        public void HideColumn(string sheetName, string colName, bool hide)
        {
            int col = ColNameToNumber(colName);
            HideColumn(sheetName, col,hide);
        }

        #endregion

        #region Cell    

        #region create,set value
        private CellType GetCellType(int cellType)
        {
            if (cellType > 5 || cellType < 0)
            {
                throw new Exception("There have no idea what type that Is!");
            }

            switch (cellType)
            {
                case 0:
                    return CellType.NUMERIC;                    
                case 1:
                    return CellType.STRING;                    
                case 2:
                    return CellType.FORMULA;                    
                case 3:
                    return CellType.BLANK;                    
                case 4:
                    return CellType.BOOLEAN;                    
                case 5:
                    return CellType.ERROR;
                    
            }
            return CellType.STRING;
        }

        public void CreateCell(string sheetName, int rowIndex, int columnIndex)
        {
            CalculAddress(rowIndex,columnIndex);
            if (VTWorkbook.GetSheet(sheetName) == null)
            {
                CreateSheet(sheetName);
            }

            if (VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow) == null)
            {
                CreateRow(sheetName, rowIndex);
            }

            if(VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn)==null)
                VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).CreateCell(CurrentColumn);
        }

        public void CreateCell(string sheetName, int rowIndex, string colName)
        {
            int col = ColNameToNumber(colName);
            CreateCell(sheetName, rowIndex, col);
        }

        public void CreateCell(string sheetName, int rowIndex, int columnIndex, int cellType)
        {
            CalculAddress(rowIndex, columnIndex);
            if (VTWorkbook.GetSheet(sheetName) == null)
            {
                CreateSheet(sheetName);
            }

            if (VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow) == null)
            {
                CreateRow(sheetName, rowIndex);
            }

            if (VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn) == null)
            {
                if (cellType > 5 || cellType < 0)
                {
                    throw new Exception("There have no idea what type that Is!");
                }

                switch (cellType)
                {
                    case 0:
                        VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).CreateCell(CurrentColumn, CellType.NUMERIC);                        
                        break;
                    case 1:
                        VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).CreateCell(CurrentColumn, CellType.STRING);                        
                        break;
                    case 2:
                        VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).CreateCell(CurrentColumn, CellType.FORMULA);
                        break;
                    case 3:
                        VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).CreateCell(CurrentColumn, CellType.BLANK);
                        break;
                    case 4:
                        VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).CreateCell(CurrentColumn, CellType.BOOLEAN);
                        break;
                    case 5:
                        VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).CreateCell(CurrentColumn, CellType.ERROR);
                        break;
                }
                
            }
        }

        public void CreateCell(string sheetName, int rowIndex, string colName, int type)
        {
            int col = ColNameToNumber(colName);
            CreateCell(sheetName, rowIndex, col, type);
        }

        public void CreateRange(string sheetName, int rowBegin, int rowEnd, int colBegin, int colEnd)
        {
            for (int i = rowBegin; i <= rowEnd; i++)
            {
                for (int j = colBegin; j <= colEnd; j++)
                {
                    CreateCell(sheetName, i, j);
                }
            }
        }

        public void SetCellType(string sheetName, int rowIndex, int columnIndex, int cellType)
        {
            CalculAddress(rowIndex, columnIndex);
            CreateCell(sheetName, rowIndex, columnIndex);
            /*  Unknown = -1,
                NUMERIC = 0,
                STRING = 1,
                FORMULA = 2,
                BLANK = 3,
                BOOLEAN = 4,
                ERROR = 5,
             */
            if (cellType > 5 || cellType < 0)
            {
                throw new Exception("There have no idea what type that Is!");
            }

            switch (cellType)
            {
                case 0:
                    VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellType(CellType.NUMERIC);
                    break;
                case 1:
                    VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellType(CellType.STRING);
                    break;
                case 2:
                    VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellType(CellType.FORMULA);
                    break;
                case 3:
                    VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellType(CellType.BLANK);
                    break;
                case 4:
                    VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellType(CellType.BOOLEAN);
                    break;
                case 5:
                    VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellType(CellType.ERROR);
                    break;
            }
        }

        public void SetCellType(string sheetName, int rowIndex, string colName, int cellType)
        {
            int col = ColNameToNumber(colName);
            SetCellType(sheetName, rowIndex, col, cellType);
        }
        
        public void SetCellFormula(string sheetName, int rowIndex, int columnIndex, string formula)
        {
            CalculAddress(rowIndex, columnIndex);
            CreateCell(sheetName, rowIndex, columnIndex);
            //VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).Cells[CurrentColumn].SetCellType(CellType.FORMULA);
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).Cells[CurrentColumn].SetCellFormula(formula);
        }

        public void SetCellFormula(string sheetName, int rowIndex, string colName, string formula)
        {
            int col = ColNameToNumber(colName);
            SetCellFormula(sheetName, rowIndex, col, formula);
        }

        public void SetCellValue(string sheetName, int rowIndex, int columnIndex, bool value)
        {
            CreateCell(sheetName, rowIndex, columnIndex);            
            CalculAddress(rowIndex, columnIndex);
            //VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellType(CellType.BOOLEAN);
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellValue(value);
        }

        public void SetCellValue(string sheetName, int rowIndex, string colName, bool value)
        {
            int col = ColNameToNumber(colName);
            SetCellValue(sheetName, rowIndex, col, value);
        }

        public void SetCellValue(string sheetName, int rowIndex, int columnIndex, double value)
        {
            CreateCell(sheetName, rowIndex, columnIndex);                        
            CalculAddress(rowIndex, columnIndex);
            //VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellType(CellType.NUMERIC);
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellValue(value);
        }

        public void SetCellValue(string sheetName, int rowIndex, string colName, double value)
        {
            int col = ColNameToNumber(colName);
            SetCellValue(sheetName, rowIndex, col, value);
        }

        public void SetCellValue(string sheetName, int rowIndex, int columnIndex, string value)
        {
            //create cell if cell doesn't exist
            CreateCell(sheetName, rowIndex, columnIndex);            
            CalculAddress(rowIndex, columnIndex);
            //VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellType(CellType.STRING);
            if (TypeXLS == "xls")
                VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellValue(new HSSFRichTextString(value));
            else
            {
                if(TypeXLS=="xlsx")
                    VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellValue(new XSSFRichTextString(value));
            }
        }

        public void SetCellValue(string sheetName, int rowIndex, string colName, string value)
        {
            int col = ColNameToNumber(colName);
            SetCellValue(sheetName, rowIndex, col, value);

        }
        
        public void SetCellValue(string sheetName, int rowIndex, int columnIndex,string dateTimeFormat, DateTime value)
        {
            //example dateTimeFormat = "dd/mm/yy"
            CreateCell(sheetName, rowIndex, columnIndex);
            CalculAddress(rowIndex, columnIndex);
            IDataFormat format = VTWorkbook.CreateDataFormat();            
            
            string styleName = string.Concat(sheetName, "_", CurrentRow.ToString(), "_", CurrentColumn.ToString());
            ICellStyle style;
            if (!StyleTable.ContainsKey(styleName))
            {
                style = VTWorkbook.CreateCellStyle();
                //add to StyleTable
                StyleTable.Add(styleName, style);
            }
            else
            {
                style = (ICellStyle)StyleTable[styleName];
            }

            style.DataFormat = format.GetFormat(dateTimeFormat);

            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellStyle = style;           
            
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).SetCellValue(value);
        }

        public void SetCellValue(string sheetName, int rowIndex, string colName, string dateTimeFormat, DateTime value)
        {
            int col = ColNameToNumber(colName);
            SetCellValue(sheetName, rowIndex, col, dateTimeFormat, value);
        }
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
        public void SetCellComment(string sheetName, int anchorDx1, int anchorDy1,
                                   int anchorDx2, int anchorDy2, int anchorCol1, int anchorRow1, 
                                   int anchorCol2, int anchorRow2, string content, string author,
                                   bool visible, int row, int col)
        {
            if (VTWorkbook.GetSheet(sheetName) != null)
            {
                IDrawing patr = VTWorkbook.GetSheet(sheetName).CreateDrawingPatriarch();
                IComment comment;
                // set text in the comment
                if (TypeXLS == "xls")
                {
                    //anchor defines size and position of the comment in worksheet
                    comment = patr.CreateCellComment(new HSSFClientAnchor(anchorDx1, anchorDy1, anchorDx2, anchorDy2, anchorCol1, anchorRow1, anchorCol2, anchorRow2));

                    comment.String = (new HSSFRichTextString(content));
                    //set comment author.                
                    comment.Author = (author);

                    //set visible
                    comment.Visible = visible;

                    //assigne comment to cell
                    CreateCell(sheetName, row, col);
                    CalculAddress(row, col);
                    VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellComment = (comment);
                }
                else
                {
                    if (TypeXLS == "xlsx")
                    {                      

                        //anchor defines size and position of the comment in worksheet
                        comment = patr.CreateCellComment(new XSSFClientAnchor(anchorDx1, anchorDy1, anchorDx2, anchorDy2, anchorCol1, anchorRow1, anchorCol2, anchorRow2));

                        comment.String = (new XSSFRichTextString(content));
                        //set comment author.                
                        comment.Author = (author);

                        //set visible
                        comment.Visible = visible;

                        //assigne comment to cell
                        CreateCell(sheetName, row, col);
                        CalculAddress(row, col);
                        VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellComment = (comment);
                    }
                }
              

            }
        }

        public void SetCellComment(string sheetName, int anchorDx1, int anchorDy1,
                                   int anchorDx2, int anchorDy2, int anchorCol1, int anchorRow1,
                                   int anchorCol2, int anchorRow2, string content, string author,
                                   bool visible, int row, string colName)
        {
            int col = ColNameToNumber(colName);
            SetCellComment(sheetName, anchorDx1, anchorDy1, anchorDx2, anchorDy2, anchorCol1, anchorRow1, anchorCol2, anchorRow2, content, author, visible, row, col);
        }

        public void SetCellComment(string sheetName, string content, string author, bool visible, int row, int col)
        {
            
            SetCellComment(sheetName, AnchorDefault.Dx1, AnchorDefault.Dy1, AnchorDefault.Dx2, AnchorDefault.Dy2,
                AnchorDefault.Col1,AnchorDefault.Row1,AnchorDefault.Col2, AnchorDefault.Row2, content, author, visible, row, col);
        }

        public void SetCellComment(string sheetName, string content, string author, bool visible, int row, string colName)
        {
            int col = ColNameToNumber(colName);
            SetCellComment(sheetName, content, author, visible, row, col);
        }

        private void CreateComment(string sheetName, int row, int col)
        { 
            CalculAddress(row, col);
            if (VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellComment == null)
            {
                
                IDrawing patr = VTWorkbook.GetSheet(sheetName).CreateDrawingPatriarch();
                IComment comment;
                // set text in the comment
                if (TypeXLS == "xls")
                {
                    //anchor defines size and position of the comment in worksheet
                    comment = patr.CreateCellComment(AnchorDefault);

                   
                    CreateCell(sheetName, row, col);
                    CalculAddress(row, col);
                    VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellComment = (comment);
                }
                else
                {
                    if (TypeXLS == "xlsx")
                    {                      

                        //anchor defines size and position of the comment in worksheet
                        comment = patr.CreateCellComment(AnchorDefault);
                        
                        //assigne comment to cell
                        CreateCell(sheetName, row, col);
                        CalculAddress(row, col);
                        VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellComment = (comment);
                    }
                }            
            }
        }
        public void SetCommentContent(string sheetName, int row, int col, string content)
        {            
            CreateCell(sheetName, row, col);
            CalculAddress(row, col);
            CreateComment(sheetName, row, col);
            if (TypeXLS == "xls")
                VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellComment.String = (new HSSFRichTextString(content));
            else 
            {
                if(TypeXLS=="xlsx")
                    VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellComment.String = (new XSSFRichTextString(content));
            }
        }
        
        public void SetCommentAuthor(string sheetName,int row, int col, string author)
        {
            CreateCell(sheetName, row, col);
            CalculAddress(row, col);
            CreateComment(sheetName, row, col);
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellComment.Author = author;
        }        

        public void SetCommentVisible(string sheetName, int row, int col, bool visible)
        {
            CreateCell(sheetName, row, col);
            CalculAddress(row, col);
            CreateComment(sheetName, row, col);
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellComment.Visible = visible;
        }

        public void SetCommentFont(string sheetName, int row, int col, VTFont vtFont)
        {
            IFont font = VTWorkbook.CreateFont();
            font.Boldweight = vtFont.Boldweight;
            font.Charset = vtFont.Charset;
            font.Color = vtFont.Color;
            font.FontHeight = vtFont.FontHeight;
            font.FontHeightInPoints = vtFont.FontHeightInPoints;
            font.FontName = vtFont.FontName;
            font.IsItalic = vtFont.IsItalic;
            font.TypeOffset = vtFont.TypeOffset;
            font.Underline = vtFont.Underline;

            CreateCell(sheetName, row, col);
            CalculAddress(row, col);
            CreateComment(sheetName, row, col);
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellComment.String.ApplyFont(font);
        }
        #endregion

        #region Style

        #region style border
        private BorderStyle GetBorderStyle(VTBorderStyle bstyle)
        {
            switch(bstyle)
            {
                case VTBorderStyle.THIN:
                    return BorderStyle.THIN;
                case VTBorderStyle.MEDIUM:
                    return BorderStyle.MEDIUM;
                case VTBorderStyle.DASHED:
                    return BorderStyle.DASHED;
                case VTBorderStyle.HAIR:
                    return BorderStyle.HAIR;
                case VTBorderStyle.THICK:
                    return BorderStyle.THICK;
                case VTBorderStyle.DOUBLE:
                    return BorderStyle.DOUBLE;
                case VTBorderStyle.DOTTED:
                    return BorderStyle.DOTTED;
                case VTBorderStyle.MEDIUM_DASHED:
                    return BorderStyle.MEDIUM_DASHED;
                case VTBorderStyle.DASH_DOT:
                    return BorderStyle.DASH_DOT;
                case VTBorderStyle.MEDIUM_DASH_DOT:
                    return BorderStyle.DASH_DOT;
                case VTBorderStyle.DASH_DOT_DOT:
                    return BorderStyle.DASH_DOT_DOT;
                case VTBorderStyle.MEDIUM_DASH_DOT_DOT:
                    return BorderStyle.MEDIUM_DASH_DOT_DOT;
                case VTBorderStyle.SLANTED_DASH_DOT:
                    return BorderStyle.SLANTED_DASH_DOT;
                default:
                    return BorderStyle.NONE;


            }
                
        }
        public void SetStyleBorder( string sheetName, int row, int col,
                                    VTBorderStyle borderBottom, short bottomBorderColor, 
                                    VTBorderStyle borderTop, short topBorderColor,
                                    VTBorderStyle borderLeft, short leftBorderColor,
                                    VTBorderStyle borderRight, short rightBorderColor)
        {
            CalculAddress(row,col);
            
            CreateCell(sheetName, row, col);
            string styleName = string.Concat(sheetName, "_", CurrentRow.ToString(), "_", CurrentColumn.ToString());
            ICellStyle style;
            if (!StyleTable.ContainsKey(styleName))
            {
                style = VTWorkbook.CreateCellStyle();                
                //add to StyleTable
                StyleTable.Add(styleName, style);               
            }
            else 
            {
               style = (ICellStyle)StyleTable[styleName];

            }

            //assigne new value of border
            style.BorderBottom = GetBorderStyle(borderBottom);
            style.BorderTop = GetBorderStyle(borderTop);
            style.BorderLeft = GetBorderStyle(borderLeft);
            style.BorderRight = GetBorderStyle(borderRight);
            //assigne new value of color
            style.BottomBorderColor = bottomBorderColor;
            style.TopBorderColor = topBorderColor;
            style.LeftBorderColor = leftBorderColor;
            style.RightBorderColor = rightBorderColor;

            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellStyle = style;

        }

        public void SetStyleBorder( string sheetName, int row, int col,
                                    VTBorderStyle borderBottom, VTBorderStyle borderTop, 
                                    VTBorderStyle borderLeft, VTBorderStyle borderRight)
        {
            short colorDefault = NPOI.HSSF.Util.HSSFColor.BLACK.index;
            SetStyleBorder(sheetName,row,col,borderBottom,colorDefault,borderTop, colorDefault, borderLeft, colorDefault, borderRight, colorDefault);
        }
           
        public void SetStyleBorder( string sheetName, int row, string colName,
                                    VTBorderStyle borderBottom, VTBorderStyle borderTop, 
                                    VTBorderStyle borderLeft, VTBorderStyle borderRight)
        {
            int col = ColNameToNumber(colName);
            SetStyleBorder(sheetName,row,col,borderBottom,borderTop, borderLeft, borderRight);

        }
        #endregion 

        #region style aligment
        private HorizontalAlignment GetHAlignment(VTHorizontalAlignment horAlignment)
        {
            switch (horAlignment)
            { 
                case VTHorizontalAlignment.CENTER:
                    return HorizontalAlignment.CENTER;
                case VTHorizontalAlignment.CENTER_SELECTION:
                    return HorizontalAlignment.CENTER_SELECTION;
                case VTHorizontalAlignment.DISTRIBUTED:
                    return HorizontalAlignment.DISTRIBUTED;
                case VTHorizontalAlignment.FILL:
                    return HorizontalAlignment.FILL;
                case VTHorizontalAlignment.JUSTIFY:
                    return HorizontalAlignment.JUSTIFY;
                case VTHorizontalAlignment.LEFT:
                    return HorizontalAlignment.LEFT;
                case VTHorizontalAlignment.RIGHT:
                    return HorizontalAlignment.RIGHT;
                default:
                    return HorizontalAlignment.GENERAL;

            }
        }
        private VerticalAlignment GetVAlignment(VTVerticalAlignment verAlignment)
        {
            switch (verAlignment)
            { 
                case VTVerticalAlignment.BOTTOM:
                    return VerticalAlignment.BOTTOM;
                case VTVerticalAlignment.DISTRIBUTED:
                    return VerticalAlignment.DISTRIBUTED;
                case VTVerticalAlignment.JUSTIFY:
                    return VerticalAlignment.JUSTIFY;
                case VTVerticalAlignment.TOP:
                    return VerticalAlignment.TOP;
                default:
                    return VerticalAlignment.CENTER;
            }

        }
        public void SetStyleAlignment(string sheetName, int row, int col, VTHorizontalAlignment alignment, VTVerticalAlignment verticalAlignment)
        {            
            CalculAddress(row,col);
            CreateCell(sheetName, row, col);
            string styleName = string.Concat(sheetName, "_", CurrentRow.ToString(), "_", CurrentColumn.ToString());
            ICellStyle style;
            if (!StyleTable.ContainsKey(styleName))
            {
                style = VTWorkbook.CreateCellStyle();
                //add to StyleTable
                StyleTable.Add(styleName, style);
            }
            else
            {
                style = (ICellStyle)StyleTable[styleName];
            }

            style.Alignment = GetHAlignment(alignment);
            style.VerticalAlignment = GetVAlignment(verticalAlignment);
            //assigne style to cell
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellStyle = style;
        }

        public void SetStyleAlignment(string sheetName, int row, string colName, VTHorizontalAlignment alignment, VTVerticalAlignment verticalAlignment)
        {
            int col = ColNameToNumber(colName);
            SetStyleAlignment(sheetName,row,col,alignment,verticalAlignment);
        }
        #endregion 

        #region color
        public void SetStyleBGColor(string sheetName, int row, int col, System.Drawing.Color backGroundColor)
        {
            CalculAddress(row,col);
            CreateCell(sheetName, row, col);
            string styleName = string.Concat(sheetName, "_", CurrentRow.ToString(), "_", CurrentColumn.ToString());
            ICellStyle style;
            if (!StyleTable.ContainsKey(styleName))
            {
                style = VTWorkbook.CreateCellStyle();
                //add to StyleTable
                StyleTable.Add(styleName, style);
            }
            else
            {
                style = (ICellStyle)StyleTable[styleName];
            }

            if (backGroundColor != System.Drawing.Color.Empty)
            {
                style.FillBackgroundColor = GetXLColourBG(backGroundColor);
                style.FillPattern = FillPatternType.SOLID_FOREGROUND;
            }
            //assigne style to cell
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellStyle = style;
           
        }

        public void SetStyleBGColor(string sheetName, int row, string colName, System.Drawing.Color backGroundColor)
        {
            int col = ColNameToNumber(colName);
            SetStyleBGColor(sheetName, row, col,backGroundColor);
        }

        //reference to Excel XLS BIFF File Reader/Writer - Shell for NPOI 1.2.4
        private short GetXLColourBG(System.Drawing.Color SystemColour)
	    {
		    //Lookup RGB from .NET system colour in Excel pallete - or create a new entry (get nearest if palette full). Return the XL palette index.
		    if (SystemColour == System.Drawing.Color.Empty)
			    return NPOI.HSSF.Util.HSSFColor.COLOR_NORMAL;
		    //COLOR_NORMAL=unspecified. 'alt= AUTOMATIC.index 
            //HSSFPalette XlPalette = VTWorkbook.GetCustomPalette();
            //NPOI.HSSF.Util.HSSFColor XlColour = XlPalette.FindColor(SystemColour.R, SystemColour.G, SystemColour.B);
            //if ((XlColour == null)) {
            //    //Available colour palette entries: 65 to 32766 (0-64=standard palette; 64=auto, 32767=unspecified)
            //    if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 255) {
            //        if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 67) {
            //            //Move start pointer up. First Entry seems to fail, so add dummy.
            //            NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE = 67;
            //            XlColour = XlPalette.AddColor(100, 100, 100);
            //            XlColour = XlPalette.AddColor(100, 100, 100);
            //        }
            //        NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE += 1;
            //        XlColour = XlPalette.AddColor(SystemColour.R, SystemColour.G, SystemColour.B);
            //    } else {
            //        XlColour = XlPalette.FindSimilarColor(SystemColour.R, SystemColour.G, SystemColour.B);
            //    }
            //    return XlColour.GetIndex();
            //} else {
            //    return XlColour.GetIndex();
            //}
            return 0;
	    }

        #endregion 
 
        #region Font
        private FontUnderlineType GetFontUnderlineType(VTFontUnderlineType fontUnderline)
        {
            switch (fontUnderline)
            { 
                case VTFontUnderlineType.DOUBLE:
                    return FontUnderlineType.DOUBLE;
                case VTFontUnderlineType.DOUBLE_ACCOUNTING:
                    return FontUnderlineType.DOUBLE_ACCOUNTING;
                case VTFontUnderlineType.SINGLE:
                    return FontUnderlineType.SINGLE;
                case VTFontUnderlineType.SINGLE_ACCOUNTING:
                    return FontUnderlineType.SINGLE_ACCOUNTING;
                default:
                    return FontUnderlineType.NONE;

            }
        }
        public void SetStyleFont(string sheetName, int row, int col,
                                 System.Drawing.Color fColor, bool isItalic, bool bold,
                                 VTFontUnderlineType underline, short fontHeightInPoints,
                                string fontName)
        {
            IFont font = VTWorkbook.CreateFont();
            if(fColor != System.Drawing.Color.Empty)
            {
                font.Color = GetXLColourFG(fColor);
            }

            //chữ nghiêng
            font.IsItalic = isItalic;
            //chữ có gạch chân
            font.Underline = (byte)(GetFontUnderlineType(underline));
            //chữ đậm
            if (bold)
                    font.Boldweight = (short)FontBoldWeight.BOLD;
            //cỡ chữ
            font.FontHeightInPoints = fontHeightInPoints;
            //font name
            if(fontName !=null)
                font.FontName = fontName;
           
            //apply font to style
            CalculAddress(row, col);
            CreateCell(sheetName, row, col);
            string styleName = string.Concat(sheetName, "_", CurrentRow.ToString(), "_", CurrentColumn.ToString());
            ICellStyle style;
            if (!StyleTable.ContainsKey(styleName))
            {
                style = VTWorkbook.CreateCellStyle();
                //add to StyleTable
                StyleTable.Add(styleName, style);
            }
            else
            {
                style = (ICellStyle)StyleTable[styleName];
            }

            style.SetFont(font);
            //assigne style to cell
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellStyle = style;
            
        }

        public void SetStyleFont(string sheetName, int row, string colName,
                                 System.Drawing.Color fColor, bool isItalic, bool bold,
                                 VTFontUnderlineType underline, short fontHeightInPoints,
                                string fontName)
        {
            int col = ColNameToNumber(colName);
            SetStyleFont(sheetName, row, col,fColor, isItalic, bold, underline, fontHeightInPoints, fontName);
        }

        private short GetXLColourFG(System.Drawing.Color SystemColour)
	    {
            ////Lookup RGB from .NET system colour in Excel pallete - or nearest match. Font colour in 1.2.3 has a bug and does not support custom entries
            //if (SystemColour == System.Drawing.Color.Empty)
            //    return NPOI.HSSF.Util.HSSFColor.COLOR_NORMAL;
            ////COLOR_NORMAL=unspecified. 'alt= AUTOMATIC.index 
            //HSSFPalette XlPalette = VTWorkbook.GetCustomPalette();
            //NPOI.HSSF.Util.HSSFColor XlColour = XlPalette.FindColor(SystemColour.R, SystemColour.G, SystemColour.B);
            //if ((XlColour == null)) {
            //    XlColour = XlPalette.FindSimilarColor(SystemColour.R, SystemColour.G, SystemColour.B);
            //    return XlColour.GetIndex();
            //} else {
            //    return XlColour.GetIndex();
            //}
            return 0;
	    }
        #endregion
        
        #region wraptext and rotation

        public void SetStyleWraptext(string sheetName, int row, int col, bool wraptext)
        {
            CalculAddress(row, col);
            CreateCell(sheetName, row, col);
            string styleName = string.Concat(sheetName, "_", CurrentRow.ToString(), "_", CurrentColumn.ToString());
            ICellStyle style;
            if (!StyleTable.ContainsKey(styleName))
            {
                style = VTWorkbook.CreateCellStyle();
                //add to StyleTable
                StyleTable.Add(styleName, style);
            }
            else
            {
                style = (ICellStyle)StyleTable[styleName];
            }

            //assigne alignment
            style.WrapText = wraptext;
            //assigne style to cell
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellStyle = style;
        }
        public void SetStyleWraptext(string sheetName, int row, string colName, bool wraptext)
        {
            int col = ColNameToNumber(colName);
            SetStyleWraptext(sheetName,row,col,wraptext);
        }
         //a valid rotate value is from -90 - 90
        public void SetStyleRotation(string sheetName, int row, int col, short rotation)
        {
            if (TypeXLS == "xls")
            {
                CalculAddress(row, col);
                CreateCell(sheetName, row, col);
                string styleName = string.Concat(sheetName, "_", CurrentRow.ToString(), "_", CurrentColumn.ToString());
                ICellStyle style;
                if (!StyleTable.ContainsKey(styleName))
                {
                    style = VTWorkbook.CreateCellStyle();
                    //add to StyleTable
                    StyleTable.Add(styleName, style);
                }
                else
                {
                    style = (ICellStyle)StyleTable[styleName];
                }

                //assigne alignment
                style.Rotation = (short)rotation;
                //assigne style to cell
                VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellStyle = style;
            }
        }

        public void SetStyleRotation(string sheetName, int row, string colName, short rotation)
        {
            if (TypeXLS == "xls")
            {
                int col = ColNameToNumber(colName);
                SetStyleRotation(sheetName, row, col, rotation);
            }
        }

        public void CloneStyle(string sheetOriginal, int rowOriginal, int colOriginal, string sheetDestination, int rowDestination,int colDestination)
        {
            if(VTWorkbook.GetSheet(sheetOriginal)==null)
                throw new Exception("the worksheet original does not exist");
            if(VTWorkbook.GetSheet(sheetDestination) == null)
                throw new Exception("the worksheet destiantion does not exist");

            CalculAddress(rowOriginal, colOriginal);
            CreateCell(sheetOriginal, rowOriginal, colOriginal);
            string styleNameOriginal = string.Concat(sheetOriginal, "_", CurrentRow.ToString(), "_", CurrentColumn.ToString());

            CreateCell(sheetDestination, rowDestination, colDestination);
            string styleNameDestination = string.Concat(sheetDestination, "_", (rowDestination - 1).ToString(), "_",(colDestination-1).ToString());
            
            ICellStyle styleOri;
            ICellStyle styleDes;
            if (StyleTable.ContainsKey(styleNameOriginal))
            {
                styleOri = (ICellStyle)StyleTable[styleNameOriginal];
                //add to StyleTable

                if(StyleTable.ContainsKey(styleNameDestination))
                {
                    styleDes = (ICellStyle)StyleTable[styleNameOriginal];             
                                     
                }
                else
                {
                    styleDes = VTWorkbook.CreateCellStyle();
                    StyleTable.Add(styleNameDestination,styleDes);                    
                }
                 styleDes.Alignment = styleOri.Alignment;
                styleDes.BorderBottom = styleOri.BorderBottom;
                styleDes.BorderLeft = styleOri.BorderLeft;
                styleDes.BorderRight = styleOri.BorderRight;
                styleDes.BorderTop = styleOri.BorderTop;
                styleDes.BottomBorderColor = styleOri.BottomBorderColor;
                styleDes.DataFormat = styleOri.DataFormat;
                styleDes.FillBackgroundColor = styleOri.FillBackgroundColor;
                styleDes.FillForegroundColor = styleOri.FillForegroundColor;
                styleDes.FillPattern = styleOri.FillPattern;                    
                styleDes.Indention = styleOri.Indention;
                styleDes.IsHidden = styleOri.IsHidden;
                styleDes.IsLocked = styleOri.IsLocked;
                styleDes.LeftBorderColor = styleOri.LeftBorderColor;
                styleDes.RightBorderColor = styleOri.RightBorderColor;
                styleDes.Rotation = styleOri.Rotation;
                styleDes.TopBorderColor = styleOri.TopBorderColor;
                styleDes.VerticalAlignment = styleOri.VerticalAlignment;
                styleDes.WrapText = styleOri.WrapText;
                VTWorkbook.GetSheet(sheetDestination).GetRow(rowDestination - 1).GetCell(colDestination - 1).CellStyle = styleDes;
                
            }           
            
        }
        #endregion


        #endregion

        #region merge and lock cell
        public void MergeCell(string sheetName, int firstRow, int firstCol, int lastRow, int lastCol)
        {
            //normaliser to zero base
            int row1 = firstRow - 1;
            int col1 = firstCol - 1;
            int row2 = lastRow - 1;
            int col2 = lastCol - 1;

            CellRangeAddress region = new CellRangeAddress(row1,row2,col1,col2);
            VTWorkbook.GetSheet(sheetName).AddMergedRegion(region);
        }

        public void MergeCell(string sheetName, int firstRow, string firstColName, int lastRow, string lastColName)
        {
            int firstCol = ColNameToNumber(firstColName);
            int lastCol = ColNameToNumber(lastColName);
            MergeCell(sheetName, firstRow, firstCol, lastRow, lastCol);
        }

        public void UnLockCell(string sheetName, int row, int col)
        {
            CalculAddress(row, col);
            CreateCell(sheetName,row,col);

            ICellStyle unlockStyle = VTWorkbook.CreateCellStyle();    
            unlockStyle.IsLocked = false;
            VTWorkbook.GetSheet(sheetName).GetRow(CurrentRow).GetCell(CurrentColumn).CellStyle = unlockStyle;            
        }

        public void UnLockCell(string sheetName, int row, string colName)
        {
            int col = ColNameToNumber(colName);
            UnLockCell(sheetName, row, col);
        }

        public void LockSheet(string sheetName, string password)
        {
            if(VTWorkbook.GetSheet(sheetName)!= null)
                VTWorkbook.GetSheet(sheetName).ProtectSheet(password);
        }
        #endregion

        #region FreezePane
        public void FreezePane(string sheetName, int rowSplit, int colSplit)
        {
            CalculAddress(rowSplit, colSplit);
            CreateCell(sheetName, rowSplit, colSplit);
            VTWorkbook.GetSheet(sheetName).CreateFreezePane(CurrentColumn, CurrentRow);
        }

        public void FreezePane(string sheetName, int rowSplit, string colSplitName)
        {
            int col = ColNameToNumber(colSplitName);
            FreezePane(sheetName, rowSplit, col);
        }

        public void FreezePane(string sheetName, int rowSplit, int colSplit, int leftmostColumn, int topRow)
        {
            CalculAddress(rowSplit, colSplit);
            CreateCell(sheetName, rowSplit, colSplit);
            VTWorkbook.GetSheet(sheetName).CreateFreezePane(CurrentColumn, CurrentRow, leftmostColumn-1, topRow - 1);
        }
        #endregion

        #region Set DropDownList to Cell
        public void SetDropDownListCell(string sheetName, int row, int col, List<string> listValues)
        {            
            string[] lsVals = new string[listValues.Count()]; 

            for(int i = 0;i<listValues.Count();i++)
                lsVals[i] = listValues[i];

            SetDropDownListCell(sheetName, row, col, lsVals);
           
        }

        public void SetDropDownListCell(string sheetName, int row, string colName, List<string> listValues)
        {
            int col = ColNameToNumber(colName);
            SetDropDownListCell(sheetName, row, col, listValues);
        }

        public void SetDropDownListCell(string sheetName, int row, int col, string[] listValues)
        {
            CalculAddress(row, col);
            CellRangeAddressList lsRange = new CellRangeAddressList();
            lsRange.AddCellRangeAddress(new CellRangeAddress(CurrentRow, CurrentRow, CurrentColumn, CurrentColumn)); 
            
            if (TypeXLS == "xls")
            {
                DVConstraint constraintValues = DVConstraint.CreateExplicitListConstraint(listValues);
                HSSFDataValidation dataValidation = new
                        HSSFDataValidation(lsRange, constraintValues);

                VTWorkbook.GetSheet(sheetName).AddValidationData(dataValidation);
            }
            else 
            {
                if (TypeXLS == "xlsx")
                {
                    //XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper((XSSFSheet)VTWorkbook.GetSheet(sheetName));
                    //XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint) dvHelper.CreateExplicitListConstraint(listValues);              
                    //XSSFDataValidation validation =(XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, lsRange);
                    //validation.SuppressDropDownArrow = true;
                    //VTWorkbook.GetSheet(sheetName).AddValidationData(validation);
                    
                }
            }

        }
                
        //set DropDownListCell from data of excel file, (ex formula = "Sheet2!$E1:$E3")
        public void SetDropDownListCell(string sheetName, int row, int col, string formula)
        {
            CalculAddress(row, col);
            CellRangeAddressList lsRange = new CellRangeAddressList();
            lsRange.AddCellRangeAddress(new CellRangeAddress(CurrentRow, CurrentRow, CurrentColumn, CurrentColumn));
            if (TypeXLS == "xls")
            {
                DVConstraint constraintValues = DVConstraint.CreateFormulaListConstraint(formula);

                HSSFDataValidation dataValidation = new
                        HSSFDataValidation(lsRange, constraintValues);

                VTWorkbook.GetSheet(sheetName).AddValidationData(dataValidation);
            }
            else
            {
                if (TypeXLS == "xlsx")
                {
                    //XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper((XSSFSheet)VTWorkbook.GetSheet(sheetName));
                    //XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint) dvHelper.CreateFormulaListConstraint(formula);              
                    //XSSFDataValidation validation =(XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, lsRange);
                    //validation.SuppressDropDownArrow = true;
                    //VTWorkbook.GetSheet(sheetName).AddValidationData(validation);
                }
            }
        }

        public void SetDropDownListCell(string sheetName, int row, string colName, string formula)
        {
            int col = ColNameToNumber(colName);
            SetDropDownListCell(sheetName, row, col, formula);
        }

        public void SetDropDownListRangeCell(string sheetName, int firstRow, int firstCol, int lastRow, int lastCol, List<string> listValues)
        {
            string[] lsVals = new string[listValues.Count()];

            for (int i = 0; i < listValues.Count(); i++)
                lsVals[i] = listValues[i];

            SetDropDownListRangeCell(sheetName, firstRow, firstCol, lastRow, lastCol, lsVals);
            
        }

        public void SetDropDownListRangeCell(string sheetName, int firstRow, int firstCol, int lastRow, int lastCol, string[] listValues)
        {
            CalculAddress(firstRow, firstCol);
            int nbRow = lastRow - firstRow;
            int nbCol = lastCol - firstCol;

            CellRangeAddressList lsRange = new CellRangeAddressList();
            lsRange.AddCellRangeAddress(new CellRangeAddress(CurrentRow, CurrentRow + nbRow, CurrentColumn, CurrentColumn + nbCol));

            if (TypeXLS == "xls")
            {
                DVConstraint constraintValues = DVConstraint.CreateExplicitListConstraint(listValues);
                HSSFDataValidation dataValidation = new
                        HSSFDataValidation(lsRange, constraintValues);

                VTWorkbook.GetSheet(sheetName).AddValidationData(dataValidation);
            }
            else
            {
                if (TypeXLS == "xlsx")
                {
                    //XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper((XSSFSheet)VTWorkbook.GetSheet(sheetName));
                    //XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)dvHelper.CreateExplicitListConstraint(listValues);
                    //XSSFDataValidation validation = (XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, lsRange);
                    //VTWorkbook.GetSheet(sheetName).AddValidationData(validation);
                }
            }

        }

        public void SetDropDownListRangeCell(string sheetName, int firstRow, int firstCol, int lastRow, int lastCol, string formula)
        {
            
            CalculAddress(firstRow, firstCol);
            int nbRow = lastRow - firstRow;
            int nbCol = lastCol - firstCol;

            CellRangeAddressList lsRange = new CellRangeAddressList();
            lsRange.AddCellRangeAddress(new CellRangeAddress(CurrentRow, CurrentRow + nbRow, CurrentColumn, CurrentColumn + nbCol));

            if (TypeXLS == "xls")
            {
                DVConstraint constraintValues = DVConstraint.CreateFormulaListConstraint(formula);

                HSSFDataValidation dataValidation = new
                        HSSFDataValidation(lsRange, constraintValues);

                VTWorkbook.GetSheet(sheetName).AddValidationData(dataValidation);
            }
            else
            {
                if (TypeXLS == "xlsx")
                {
                    //XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper((XSSFSheet)VTWorkbook.GetSheet(sheetName));
                    //XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint) dvHelper.CreateFormulaListConstraint(formula);              
                    //XSSFDataValidation validation =(XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, lsRange);
                    //validation.SuppressDropDownArrow = true;
                    //VTWorkbook.GetSheet(sheetName).AddValidationData(validation);
                }
            }
        }
        #endregion

        #endregion

        #region Save
        public void Save(string pathName)
        {
            try
            {
                if (TypeXLS == "xls")
                {
                    if (File.Exists(pathName))
                        File.Delete(pathName);
                    using (FileStream fs = new FileStream(pathName, FileMode.Create))
                    {
                        VTWorkbook.Write(fs);
                        fs.Flush();
                        fs.Close();
                    }
                }
                else
                {
                    if (TypeXLS == "xlsx")
                    {
                        FileStream sw = File.Create(pathName);
                        VTWorkbook.Write(sw);
                        sw.Close();
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
                
        }

        public void Save(ref Stream streamExcel)
        {
            try
            {
                VTWorkbook.Write(streamExcel);
            }
            catch (Exception)
            {
                throw;
            }

        }

        //public MemoryStream WriteToStream()
        //{            
        //    MemoryStream excelStream = new MemoryStream();
        //    VTWorkbook.Write(excelStream);
        //    return excelStream;
        //}

        #endregion
        #endregion
    }
}
