using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using log4net;
using NativeExcel;
using NPOI.XSSF.UserModel;
using System.Drawing;
using Model.Entities;
using Business.CommonBusiness;

namespace VTUtils.Excel.Export
{
    public class VTWorkbook : IVTWorkbook
    {
        private static ILog logger = LogManager.GetLogger(typeof(VTWorkbook).Name);
        public NativeExcel.IWorkbook Workbook { get; set; }
        public NPOI.HSSF.UserModel.HSSFWorkbook HWorkbook { get; set; }

        public VTWorkbook(NativeExcel.IWorkbook workbook)
        {
            Workbook = workbook;
        }
        public IVTWorksheet GetSheet(int index)
        {
            return new VTWorksheet(Workbook.Worksheets[index]);
        }

        public IVTWorksheet Add()
        {
            return new VTWorksheet(Workbook.Worksheets.Add());
        }

        public IVTWorksheet GetSheet(string name)
        {
            return new VTWorksheet(Workbook.Worksheets[name]);
        }
        public NativeExcel.IWorkbook GetWorkbook()
        {
            return Workbook;
        }

        public List<IVTWorksheet> GetSheets()
        {
            List<IVTWorksheet> list = new List<IVTWorksheet>();
            foreach (NativeExcel.IWorksheet sheet in Workbook.Worksheets)
                list.Add(new VTWorksheet(sheet));
            return list;
        }

        public IVTWorksheet CopySheetToLastClearColor(IVTWorksheet worksheet, string lastCell = null, object Color = null)
        {
            NativeExcel.IWorksheet templateSheet = worksheet.Worksheet;
            NativeExcel.IWorksheet sheet = Workbook.Worksheets.AddAfter(Workbook.Worksheets.Count);

            if (Color != null)
            {
                sheet.Cells.Interior.Color = (Color)Color;
            }

            IVTWorksheet vtSheet = new VTWorksheet(sheet);

            if (lastCell == null)
            {
                vtSheet.CopySheet(worksheet);
            }
            else
            {
                vtSheet.CopySheet(worksheet, lastCell);
            }
            return vtSheet;
        }

        public IVTWorksheet CopySheetToLast(IVTWorksheet worksheet, string lastCell = null)
        {
            NativeExcel.IWorksheet templateSheet = worksheet.Worksheet;
            NativeExcel.IWorksheet sheet = Workbook.Worksheets.AddAfter(Workbook.Worksheets.Count);

            IVTWorksheet vtSheet = new VTWorksheet(sheet);
            if (lastCell == null)
            {
                vtSheet.CopySheet(worksheet);
            }
            else
            {
                vtSheet.CopySheet(worksheet, lastCell);
            }
            return vtSheet;
        }

        public IVTWorksheet CopySheetToBeforeLast(IVTWorksheet worksheet)
        {
            NativeExcel.IWorksheet templateSheet = worksheet.Worksheet;
            NativeExcel.IWorksheet sheet = Workbook.Worksheets.AddBefore(Workbook.Worksheets.Count);
            IVTWorksheet vtSheet = new VTWorksheet(sheet);
            vtSheet.CopySheet(worksheet);
            return vtSheet;
        }

        public IVTWorksheet CopySheetToBefore(IVTWorksheet worksheet, IVTWorksheet beforeSheet)
        {
            NativeExcel.IWorksheet sheet = Workbook.Worksheets.AddBefore(beforeSheet.Worksheet);
            IVTWorksheet vtSheet = new VTWorksheet(sheet);
            vtSheet.CopySheet(worksheet);
            return vtSheet;
        }

        public IVTWorksheet CopySheetToBefore(IVTWorksheet worksheet, int index)
        {
            NativeExcel.IWorksheet sheet = Workbook.Worksheets.AddBefore(index);
            IVTWorksheet vtSheet = new VTWorksheet(sheet);
            vtSheet.CopySheet(worksheet);
            return vtSheet;
        }

        public IVTWorksheet CopySheetToBefore(IVTRange worksheetRange, IVTWorksheet beforeSheet, string sheetname = "")
        {
            NativeExcel.IWorksheet sheet = Workbook.Worksheets.AddBefore(beforeSheet.Worksheet);
            IVTWorksheet vtSheet = new VTWorksheet(sheet);
            vtSheet.CopyPasteSameSize(worksheetRange, 1, 1);
            vtSheet.Name = sheetname;
            return vtSheet;
        }

        public IVTWorksheet CopySheetToBefore(IVTRange worksheetRange, int index, string sheetname = "")
        {
            NativeExcel.IWorksheet sheet = Workbook.Worksheets.AddBefore(index);
            IVTWorksheet vtSheet = new VTWorksheet(sheet);
            vtSheet.CopyPasteSameSize(worksheetRange, 1, 1);
            vtSheet.Name = sheetname;
            return vtSheet;
        }

        public void SaveToFile(string pathFile)
        {
            Stream fout = ToStream();
            FileInfo fileInf = new FileInfo(pathFile);
            fileInf.Directory.Create();
            if (Directory.Exists(Path.GetDirectoryName(pathFile)))
                using (Stream destination = File.Create(pathFile))
                {
                    for (int a = fout.ReadByte(); a != -1; a = fout.ReadByte())
                    {
                        destination.WriteByte((byte)a);
                    }
                }
        }

        public string SaveToHtml()
        {
            List<string> lstFirstCellText = new List<string>();
            string CellFirst = "";

            for (int i = 1; i <= Workbook.Worksheets.Count; i++)
            {
                CellFirst = Workbook.Worksheets[i].Cells["A1"].Value == null ? "" : Workbook.Worksheets[i].Cells["A1"].Value.ToString();
                lstFirstCellText.Add(CellFirst);
            }

            Stream htmlStream = new MemoryStream();
            this.GetWorkbook().SaveAs(htmlStream, XlFileFormat.xlHtml);

            StreamReader reader = new StreamReader(htmlStream);
            htmlStream.Position = 0;
            reader.DiscardBufferedData();
            string ExcelHtml = reader.ReadToEnd();

            for (int i = 0; i < lstFirstCellText.Count; i++)
            {
                string str = lstFirstCellText[i];
                ExcelHtml = ReplaceFirst(ExcelHtml, "This worksheet was created by demo version of NativeExcel library", str);
            }

            return ExcelHtml;
        }

        private string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        
        public Stream ToStream()
        {
            String pathFile = AppDomain.CurrentDomain.BaseDirectory
                + "Uploads" + Path.DirectorySeparatorChar
                + "Temp" + Path.DirectorySeparatorChar
                + Guid.NewGuid().ToString().Replace("-", "").ToUpper() + ".xls";
            //Ex: C://root//website//Uploads//Temp//0928AA991A9919A888.xls
            try
            {
                List<string> lstFirstCellText = new List<string>();
                string CellFirst = "";

                for (int i = 1; i <= Workbook.Worksheets.Count; i++)
                {
                    CellFirst = Workbook.Worksheets[i].Cells["A1"].Value == null ? "" : Workbook.Worksheets[i].Cells["A1"].Value.ToString();
                    lstFirstCellText.Add(CellFirst);
                }
                Workbook.SaveAs(pathFile);
                //Remove Demo text
                RemoveNativeDemoText_NPOI(pathFile, lstFirstCellText);
                MemoryStream memStream;
                using (FileStream fileStream = File.OpenRead(pathFile))
                {
                    memStream = new MemoryStream();
                    memStream.SetLength(fileStream.Length);
                    fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                }
                FileInfo fi = new FileInfo(pathFile);
                fi.Delete();
                return memStream;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Stream ToStreamNumberValidationData(int sheetIndex, decimal fromNumber, decimal toNumber, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            String pathFile = AppDomain.CurrentDomain.BaseDirectory + Guid.NewGuid().ToString() + ".xls";
            try
            {
                List<string> lstFirstCellText = new List<string>();
                string CellFirst = "";

                for (int i = 1; i < Workbook.Worksheets.Count; i++)
                {
                    CellFirst = Workbook.Worksheets[i].Cells["A1"].Value == null ? "" : Workbook.Worksheets[i].Cells["A1"].Value.ToString();
                    lstFirstCellText.Add(CellFirst);
                }
                Workbook.SaveAs(pathFile);
                CreateDataValidation(pathFile, sheetIndex, fromNumber, toNumber, fromRow, fromColumn, toRow, toColumn);
                RemoveNativeDemoText_NPOI(pathFile, lstFirstCellText);
                MemoryStream memStream;
                using (FileStream fileStream = File.OpenRead(pathFile))
                {
                    memStream = new MemoryStream();
                    memStream.SetLength(fileStream.Length);
                    fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                }
                FileInfo fi = new FileInfo(pathFile);
                fi.Delete();
                return memStream;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (System.IO.File.Exists(pathFile))
                {
                    FileInfo fi = new FileInfo(pathFile);
                    fi.Delete();
                }
            }
        }

        public void CreateDataValidation(string filePath, int sheetIndex, decimal fromNumber, decimal toNumber, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
            if (hssfworkbook != null)
            {
                CellRangeAddressList rangeList = new CellRangeAddressList(fromRow - 1, toRow - 1, fromColumn - 1, toColumn - 1); // Vì NPOI sử dụng row và column bắt đầu từ 0
                DVConstraint dvconstraint = DVConstraint.CreateNumericConstraint(2, OperatorType.BETWEEN, fromNumber.ToString(), toNumber.ToString());
                HSSFDataValidation dataValidation = new HSSFDataValidation(rangeList, dvconstraint);
                HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(sheetIndex - 1);
                if (sheet != null)
                {
                    sheet.AddValidationData(dataValidation);
                    file = new FileStream(filePath, FileMode.Create);
                    hssfworkbook.Write(file);
                    file.Close();
                }
            }
        }

        public Stream ToStreamNumberValidationData(int sheetIndex, string[] lstContrains, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            List<string> lstFirstCellText = new List<string>();
            string CellFirst = "";
            String pathFile = AppDomain.CurrentDomain.BaseDirectory + Guid.NewGuid().ToString() + ".xls";
            for (int i = 1; i < Workbook.Worksheets.Count; i++)
            {
                CellFirst = Workbook.Worksheets[i].Cells["A1"].Value == null ? "" : Workbook.Worksheets[i].Cells["A1"].Value.ToString();
                lstFirstCellText.Add(CellFirst);
            }
            Workbook.SaveAs(pathFile);
            CreateDataValidation(pathFile, sheetIndex, lstContrains, fromRow, fromColumn, toRow, toColumn);
            RemoveNativeDemoText_NPOI(pathFile, lstFirstCellText);
            MemoryStream memStream;
            using (FileStream fileStream = File.OpenRead(pathFile))
            {
                memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }
            FileInfo fi = new FileInfo(pathFile);
            fi.Delete();
            return memStream;
        }

        public void CreateDataValidation(string filePath, int sheetIndex, string[] lstContrains, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
            if (hssfworkbook != null)
            {
                CellRangeAddressList rangeList = new CellRangeAddressList(fromRow - 1, toRow - 1, fromColumn - 1, toColumn - 1); // Vì NPOI sử dụng row và column bắt đầu từ 0
                DVConstraint dvconstraint = DVConstraint.CreateExplicitListConstraint(lstContrains);
                HSSFDataValidation dataValidation = new HSSFDataValidation(rangeList, dvconstraint);
                HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(sheetIndex - 1);
                if (sheet != null)
                {
                    sheet.AddValidationData(dataValidation);
                    file = new FileStream(filePath, FileMode.Create);
                    hssfworkbook.Write(file);
                    file.Close();
                }
            }
        }
        /// <summary>
        /// Remove demoText using NPOI API
        /// Chuẩn hóa Formula để hổ trợ các function mà Native không hổ trợ.
        /// Hungnd 22/02/2012 khi exception return
        /// </summary>
        /// <param name="fileName">đường dẫn đến file Excel cần thực hiện</param>
        private static void RemoveNativeDemoText_NPOI(string filePath, List<string> StringFirstCell)
        {
            FileStream file = null;
            try
            {
                file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(file, true);
                for (int i = 0; i < hssfworkbook.NumberOfSheets; i++)
                {
                    HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(i);
                    if (StringFirstCell[i] == "This worksheet was created by demo version of NativeExcel library")
                    {
                        sheet.GetRow(0).GetCell(0).SetCellValue("");
                    }
                    else
                    {
                        sheet.GetRow(0).GetCell(0).SetCellValue(StringFirstCell[i]);
                    }
                    //int lastRowNum = sheet.LastRowNum;
                    //for (int j = 0; j < lastRowNum; j++)
                    //{
                    //    IRow row = sheet.GetRow(j);
                    //    if (row == null) continue;
                    //    for (int k = 0; k <= row.LastCellNum; k++)
                    //    {
                    //        ICell cell = row.GetCell(k);
                    //        if (cell != null)
                    //        {
                    //            if (cell.CellType == CellType.STRING)
                    //            {
                    //                if (cell.StringCellValue.StartsWith("="))
                    //                {
                    //                    cell.CellFormula = cell.StringCellValue.Substring(1);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //IRow blankRow = sheet.GetRow(0);
                    //ICell blankCell = blankRow.GetCell(0);
                    //blankCell.SetCellValue(StringFirstCell[i]);
                }
                file = new FileStream(filePath, FileMode.Create);
                hssfworkbook.Write(file);
                file.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                if (file != null) file.Close();
                return;
            }
        }

        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}
