using com.gmail.nishantsinhaindia.DocumentConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class Excel2Pdf
    {
        public short Convert(string originalXlsPath, string pdfPath)
        {
            short convertExcel2PdfResult = -1;
            // Create COM Objects
            //originalXlsPath = "E:\\03.HiNET_Project\\2018\\Tỉnh Ủy Quảng Trị\\vanban_quangtri\\Source\\Web\\Uploads\\Đảng Bộ Tỉnh Quảng Trị\\Các Huyện, Thị, Thành Ủy\\Thành ủy Đông Hà\\Lãnh đạo thành ủy Đông Hà\\thanhuydongha\\eFile\\Văn bản phát hành\\205\\DANH SÁCH HỌC VIÊN QUỐC TẾ (4) (1).xlsx";
            Microsoft.Office.Interop.Excel.Application excelApplication = null;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook = null;
            object unknownType = System.Reflection.Missing.Value;
            try
            {
                //open excel application
                excelApplication = new Microsoft.Office.Interop.Excel.Application
                {
                    ScreenUpdating = false,
                    DisplayAlerts = false
                };

                //open excel sheet
                if (excelApplication != null)
                    excelWorkbook = excelApplication.Workbooks.Open(originalXlsPath, unknownType, unknownType,
                                                                    unknownType, unknownType, unknownType,
                                                                    unknownType, unknownType, unknownType,
                                                                    unknownType, unknownType, unknownType,
                                                                    unknownType, unknownType, unknownType);
                if (excelWorkbook != null)
                {


                    // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                    excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF,
                                                      pdfPath,
                                                      unknownType, unknownType, unknownType, unknownType, unknownType,
                                                      unknownType, unknownType);
                    excelWorkbook.Close();
                    convertExcel2PdfResult = 0;

                }
                else
                {
                    Console.WriteLine("Error occured for conversion of office excel to PDF ");
                    convertExcel2PdfResult = 504;
                }

            }
            catch (Exception exExcel2Pdf)
            {
                Console.WriteLine("Error occured for conversion of office excel to PDF, Exception: ", exExcel2Pdf);
                convertExcel2PdfResult = 504;
            }
            finally
            {
                // Close the workbook, quit the Excel, and clean up regardless of the results...

                if (excelWorkbook != null)
                    excelWorkbook.Close(unknownType, unknownType, unknownType);
                if (excelApplication != null) excelApplication.Quit();

                Util.releaseObject(excelWorkbook);
                Util.releaseObject(excelApplication);
            }
            return convertExcel2PdfResult;
        }
    }
}