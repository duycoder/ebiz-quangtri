using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NPOI.SS.Util;

namespace VTUtils.Excel.Export
{
    public class VTExport : IVTExport
    {
        /// <summary>
        /// Create workbook ny file path
        /// </summary>
        /// <param name="templateFilePath"></param>
        /// <returns></returns>
        public static IVTWorkbook OpenWorkbook(string templateFilePath)
        {
            return new VTWorkbook(NativeExcel.Factory.OpenWorkbook(templateFilePath));
        }

        /// <summary>
        /// Create workbook ny file stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IVTWorkbook OpenWorkbook(Stream stream)
        {
            return new VTWorkbook(NativeExcel.Factory.OpenWorkbook(stream));
        }

        /// <summary>
        /// Create validation of expoted file
        /// </summary>
        /// <param name="ExportedFile"></param>
        /// <returns></returns>
        public static IVTValidation CreateValidation(string ExportedFile)
        {
            return new VTValidation(ExportedFile);
        }
    }
}
