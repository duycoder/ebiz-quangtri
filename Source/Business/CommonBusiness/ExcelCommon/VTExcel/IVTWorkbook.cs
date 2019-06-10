using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NativeExcel;
using Model.Entities;
using Business.CommonBusiness;



namespace VTUtils.Excel.Export
{
    public interface IVTWorkbook
    {
        IVTWorksheet Add();
        IWorkbook GetWorkbook();
        /// <summary>
        /// Lay danh sach cac sheet trong book
        /// </summary>
        /// <returns></returns>
        List<IVTWorksheet> GetSheets();

        /// <summary>
        /// Lấy Sheet trong Workbook
        /// </summary>
        /// <param name="index">Số thứ tự của sheet, tính từ 1</param>
        /// <returns></returns>
        IVTWorksheet GetSheet(int index);
        IVTWorksheet GetSheet(string name);

        /// <summary>
        /// Tạo mới một sheet đặt ở cuối Excel và copy dữ liệu từ một sheet có sẵn (copy toàn bộ hoặc 1 phần)
        /// </summary>
        /// <param name="worksheet">Sheet cần copy</param>
        /// <param name="lastCell">Cell cuối cùng cần copy (nếu copy một phần), null nếu copy toàn bộ</param>
        /// <returns></returns>
        IVTWorksheet CopySheetToLast(IVTWorksheet worksheet, string lastCell = null);

        /// <summary>
        /// Tạo mới một sheet đặt ở cuối Excel, set backgroup color cho tat ca ca cell cua sheet và copy dữ liệu từ một sheet có sẵn (copy toàn bộ hoặc 1 phần)
        /// </summary>
        /// <param name="worksheet">Sheet cần copy</param>
        /// <param name="lastCell">Cell cuối cùng cần copy (nếu copy một phần), null nếu copy toàn bộ</param>
        /// <param name="Color">Doi tuong Color la mau can set cho cell</param>
        /// <returns></returns>
        IVTWorksheet CopySheetToLastClearColor(IVTWorksheet worksheet, string lastCell = null, object Color = null);

        /// <summary>
        /// Copy 1 sheet truoc 1 sheet khac
        /// </summary>
        /// <param name="worksheet">worksheet can copy</param>
        /// <param name="beforeSheet">copy den truoc beforeSheet</param>
        /// <returns></returns>
        IVTWorksheet CopySheetToBefore(IVTWorksheet worksheet, IVTWorksheet beforeSheet);

        /// <summary>
        /// Copy 1 sheet den mot vi tri cho truoc
        /// </summary>
        /// <param name="worksheet">worksheet can copy</param>
        /// <param name="index">vi tri can copy toi</param>
        /// <returns></returns>
        IVTWorksheet CopySheetToBefore(IVTWorksheet worksheet, int index);

        /// <summary>
        /// Lưu vào một file
        /// </summary>
        /// <param name="pathFile">Đường dẫn file + tên file</param>
        void SaveToFile(string pathFile);

        string SaveToHtml();

        /// <summary>
        /// Tạo mới một sheet đặt ở ngay trước sheet cuối Excel và copy dữ liệu từ một sheet có sẵn
        /// </summary>
        /// <param name="worksheet"></param>
        /// <returns></returns>
        IVTWorksheet CopySheetToBeforeLast(IVTWorksheet worksheet);

        /// <summary>
        /// Chuyển sang định dạng Stream
        /// </summary>
        Stream ToStream();

        /// <summary>
        /// <para>Tao validation chi cho phep theo dinh dang da chon trong cell</para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="fromNumber"></param>
        /// <param name="toNumber"></param>
        /// <param name="fromRow"></param>
        /// <param name="fromColumn"></param>
        /// <param name="toRow"></param>
        /// <param name="toColumn"></param>
        Stream ToStreamNumberValidationData(int sheetIndex, decimal fromNumber, decimal toNumber, int fromRow, int fromColumn, int toRow, int toColumn);

        Stream ToStreamNumberValidationData(int sheetIndex, string[] lstContrains, int fromRow, int fromColumn, int toRow, int toColumn);

        /// <summary>
        /// CopySheetToBefore
        /// </summary>
        /// <param name="worksheetRange">The worksheet range.</param>
        /// <param name="beforeSheet">The before sheet.</param>
        /// <returns></returns>
        /// <author>
        /// dungnt77
        /// </author>
        /// <remarks>
        /// 14/01/2013   9:07 AM
        /// </remarks>
        IVTWorksheet CopySheetToBefore(IVTRange worksheetRange, IVTWorksheet beforeSheet, string sheetname = "");

        /// <summary>
        /// CopySheetToBefore
        /// </summary>
        /// <param name="worksheetRange">The worksheet range.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <author>
        /// dungnt77
        /// </author>
        /// <remarks>
        /// 14/01/2013   9:07 AM
        /// </remarks>
        IVTWorksheet CopySheetToBefore(IVTRange worksheetRange, int index, string sheetname = "");
    }
}
