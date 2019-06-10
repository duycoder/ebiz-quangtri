using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CommonHelper
{
    public class ExportExcelHelper<T> where T : class
    {
        //vị trí lưu file
        public string PathStore { get; set; }
        //tên file
        public string FileName { get; set; }
        //Đường dẫn template
        public string PathTemplate { get; set; }
        //dòng bắt đầu
        public int StartRow { get; set; }
        //cột bắt đầu
        public int StartCol { get; set; }
        //Cấu hình các trường thông tin theo property của class
        public List<string> ConfigColumn { get; set; }
        public ExportExcelHelper()
        {
            StartRow = 5;
            StartCol = 1;
        }

        private Style GetStyleCell(Workbook workbook)
        {
            Style style = workbook.Styles.Add("new style");

            style.Font.Name = "Times New Roman";
            style.Font.Size = 14;
            style.Borders.Color = Color.Black;
            style.Borders[XlBordersIndex.xlInsideHorizontal].LineStyle = XlLineStyle.xlContinuous;
            style.Borders[XlBordersIndex.xlInsideVertical].LineStyle = XlLineStyle.xlContinuous;
            style.Borders[XlBordersIndex.xlDiagonalDown].LineStyle = XlLineStyle.xlLineStyleNone;
            style.Borders[XlBordersIndex.xlDiagonalUp].LineStyle = XlLineStyle.xlLineStyleNone;
            return style;
        }

        private Style GetStyleText(Workbook workbook)
        {
            Style styleCell = workbook.Styles.Add("new style cell");

            styleCell.Font.Name = "Times New Roman";
            styleCell.Font.Size = 14;
            styleCell.Font.Bold = true;

            return styleCell;
        }
        /// <summary>
        /// Export excel với danh sách đối tượng
        /// </summary>
        /// <param name="listObj"></param>
        /// <returns></returns>
        public ResponseExport Export(List<T> listObj)
        {
            var result = new ResponseExport(true);
            if (string.IsNullOrEmpty(PathStore))
            {
                result.Status = false;
                result.Message = "Vui lòng thiết lập vị trí lưu file kết xuất";
                return result;
            }


            if (listObj == null || !listObj.Any())
            {
                result.Status = false;
                result.Message = "Không có dữ liệu kết xuất";
                return result;
            }
            if (ConfigColumn == null || !ConfigColumn.Any())
            {
                result.Status = false;
                result.Message = "Vui lòng cấu hình trường thông tin cho các cột";
                return result;
            }
            if (string.IsNullOrEmpty(FileName))
            {
                result.Status = false;
                result.Message = "Vui lòng thiết lập tên file";
                return result;
            }
            if (string.IsNullOrEmpty(PathTemplate))
            {
                result.Status = false;
                result.Message = "Vui lòng thiết lập đường dẫn template kết xuất";
                return result;
            }
            else
            {
                if (!File.Exists(PathTemplate))
                {
                    result.Status = false;
                    result.Message = "Template kết xuất không tồn tại";
                    return result;
                }
            }

            if (string.IsNullOrEmpty(PathTemplate))
            {
                result.Status = false;
                result.Message = "Vui lòng thiết lập đường dẫn template kết xuất";
                return result;
            }



            try
            {
                Application app = null;
                Microsoft.Office.Interop.Excel.Workbook workbook = null;
                Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
                app = new Microsoft.Office.Interop.Excel.Application();
                workbook = app.Workbooks.Open(Path.Combine(PathTemplate));
                worksheet = workbook.ActiveSheet;
                int rows = listObj.Count;

                int startRow = StartRow;
                int startColumn = StartCol;
                //Style
                var stylecell = GetStyleCell(workbook);
                var styleText = GetStyleText(workbook);
                //End style

                worksheet.Range[GetExcelColumnName(startColumn) + startRow, GetExcelColumnName(ConfigColumn.Count + startColumn) + (startRow + rows)].Style = stylecell;
                Type typeClass = typeof(T);

                for (int i = 0; i < rows; i++)
                {
                    var dataItem = listObj[i];
                    for (int col = 0; col < ConfigColumn.Count; col++)
                    {
                        var TenTruong = ConfigColumn[col];
                        var propertyCol = typeClass.GetProperty(TenTruong);
                        var dataColum = propertyCol.GetValue(dataItem);
                        worksheet.Cells[startRow + i, startColumn + col] = dataColum;
                    }

                }


                if (!Directory.Exists(PathStore))
                {
                    Directory.CreateDirectory(PathStore);
                }

                var pathFileName = Path.Combine(PathStore, FileName + ".xlsx");
                if (File.Exists(pathFileName))
                {
                    FileName += DateTime.Now.ToString("yyyyMMdd_hhmmss");
                }
                pathFileName = Path.Combine(PathStore, FileName + ".xlsx");

                workbook.SaveAs(pathFileName);
                workbook.Close();
                app.DisplayAlerts = false;
                app.Quit();
                ExcelKiller.TerminateExcelProcess(app);
                result.Status = true;
                result.PathStore = PathStore;
                result.FileName = FileName + ".xlsx";
            }
            catch
            {
                result.Status = false;
                result.Message = "Không kết xuất được file";
            }

            return result;
        }

        /// <summary>
        /// Export excel với danh sách Chuỗi ký tự
        /// </summary>
        /// <param name="listObj"></param>
        /// <returns></returns>
        public ResponseExport ExportText(List<List<string>> listObj)
        {
            var result = new ResponseExport(true);
            if (string.IsNullOrEmpty(PathStore))
            {
                result.Status = false;
                result.Message = "Vui lòng thiết lập vị trí lưu file kết xuất";
                return result;
            }


            if (listObj == null || !listObj.Any())
            {
                result.Status = false;
                result.Message = "Không có dữ liệu kết xuất";
                return result;
            }

            if (string.IsNullOrEmpty(FileName))
            {
                result.Status = false;
                result.Message = "Vui lòng thiết lập tên file";
                return result;
            }
            if (string.IsNullOrEmpty(PathTemplate))
            {
                result.Status = false;
                result.Message = "Vui lòng thiết lập đường dẫn template kết xuất";
                return result;
            }
            else
            {
                if (!File.Exists(PathTemplate))
                {
                    result.Status = false;
                    result.Message = "Template kết xuất không tồn tại";
                    return result;
                }
            }

            if (string.IsNullOrEmpty(PathTemplate))
            {
                result.Status = false;
                result.Message = "Vui lòng thiết lập đường dẫn template kết xuất";
                return result;
            }



            try
            {
                Application app = null;
                Microsoft.Office.Interop.Excel.Workbook workbook = null;
                Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
                app = new Microsoft.Office.Interop.Excel.Application();
                workbook = app.Workbooks.Open(Path.Combine(PathTemplate));
                worksheet = workbook.ActiveSheet;
                int rows = listObj.Count;

                int startRow = StartRow;
                int startColumn = StartCol;
                //Style
                var stylecell = GetStyleCell(workbook);
                var styleText = GetStyleText(workbook);
                //End style

                //worksheet.Range[GetExcelColumnName(startColumn) + startRow, GetExcelColumnName(listObj[0].Count + startColumn) + (startRow + rows)].Style = stylecell;
                Type typeClass = typeof(T);
                var colCount = listObj[0].Count;
                for (int i = 0; i < rows; i++)
                {
                    var dataItem = listObj[i];
                    if (startColumn - 1 > 0)
                    {
                        worksheet.Cells[startRow + i, startColumn - 1] = i + 1;

                    }

                    for (int col = 0; col < colCount; col++)
                    {

                        worksheet.Cells[startRow + i, startColumn + col] = listObj[i][col];
                    }

                }


                if (!Directory.Exists(PathStore))
                {
                    Directory.CreateDirectory(PathStore);
                }

                var pathFileName = Path.Combine(PathStore, FileName + ".xlsx");
                if (File.Exists(pathFileName))
                {
                    FileName += DateTime.Now.ToString("yyyyMMdd_hhmmss");
                }
                pathFileName = Path.Combine(PathStore, FileName + ".xlsx");

                workbook.SaveAs(pathFileName);
                workbook.Close();
                app.DisplayAlerts = false;
                app.Quit();
                ExcelKiller.TerminateExcelProcess(app);
                result.Status = true;
                result.PathStore = PathStore;
                result.FileName = FileName + ".xlsx";
            }
            catch
            {
                result.Status = false;
                result.Message = "Không kết xuất được file";
            }

            return result;
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
    }

    public class ExcelKiller
    {
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);
        public static void TerminateExcelProcess(Application excelApp)
        {
            int id;
            GetWindowThreadProcessId(excelApp.Hwnd, out id);
            var process = Process.GetProcessById(id);
            if (process != null)
            {
                process.Kill();
            }
        }
    }

    public class ResponseExport
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public string PathStore { get; set; }
        public ResponseExport(bool status)
        {
            Status = status;
        }
    }
}
