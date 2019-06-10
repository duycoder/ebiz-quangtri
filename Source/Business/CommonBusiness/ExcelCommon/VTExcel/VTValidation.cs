using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.SS.Util;

namespace VTUtils.Excel.Export
{
    public class VTValidation : IVTValidation
    {
        public FileStream sfile { get; set; } // Stream of file Exported
        public string FilePath { get; set; } // File Exported
        /// <summary>
        /// property
        /// </summary>
        public VTValidation(string sFilePath)
        {
            FilePath = sFilePath;
            sfile = new System.IO.FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite);

        }

        /// <summary>
        /// Create data validation
        /// Hungnd 04/04/2013
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="fromRow"></param>
        /// <param name="fromCol"></param>
        /// <param name="toRow"></param>
        /// <param name="toCol"></param>
        /// <param name="validationType">The value in VTValidationType</param>
        /// <param name="fromValue"></param>
        /// <param name="toValue"></param>
        /// <param name="errorTitle">Default "Error value"</param>
        /// <param name="errorMessage">Default "Error input value"</param>
        /// <param name="errorStyle">Default ErrorStyle.STOP</param>
        /// <param name="lstContrains">Default null</param>
        public void CreateCellValidation(int sheetIndex,
            int fromRow, int fromColumn, int toRow, int toColumn,
            int validationType, string fromValue, string toValue, string[] lstContrains = null,
            string errorTitle = "Error value", string errorMessage = "Error input value",
            int errorStyle = ErrorStyle.STOP
            )
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(sfile);
            if (hssfworkbook != null)
            {
                CellRangeAddressList rangeList = new CellRangeAddressList(fromRow - 1, toRow - 1, fromColumn - 1, toColumn - 1); // Vì NPOI sử dụng row và column bắt đầu từ 0
                DVConstraint dvconstraint = null;
                if (validationType.Equals(VTValidationType.LIST))
                {
                    dvconstraint = DVConstraint.CreateExplicitListConstraint(lstContrains);
                }
                else
                {
                    dvconstraint = DVConstraint.CreateNumericConstraint(validationType, OperatorType.BETWEEN, fromValue, toValue);
                }
                HSSFDataValidation dataValidation = new HSSFDataValidation(rangeList, dvconstraint);
                if (!validationType.Equals(VTValidationType.LIST))
                {
                    dataValidation.CreateErrorBox(errorTitle, errorMessage);
                    dataValidation.ErrorStyle = errorStyle;
                }
                HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(sheetIndex);
                if (sheet != null)
                {
                    sheet.AddValidationData(dataValidation);
                    sfile = new FileStream(FilePath, FileMode.Create);
                    hssfworkbook.Write(sfile);
                }
            }
        }
        /// <summary>
        /// Complete Create validation in file
        /// hungnd8 09/04/2013
        /// </summary>
        public void CompleteValidation()
        {
            sfile.Close();
        }
    }

    /// <summary>
    /// param of validation type
    /// hungnd8 04/05/2013
    /// </summary>
    public sealed class VTValidationType
    {
        public const int ANY = 0; // Any value
        public const int INTEGER = 1; // Validation with interger
        public const int DECIMAL = 2; // Validation with Decimal
        public const int LIST = 3; // Validation with list many value (dropdownlist)
        public const int DATE = 4; // Validation with datetime
        public const int TIME = 5; // Validation with time
        public const int TEXT_LENGTH = 6; // Validation with length of value
        public const int FORMULA = 7; //
    }

    /// <summary>
    /// Error style of datavalidation
    /// hungnd8 04/05/2013
    /// </summary>
    public sealed class ErrorStyle
    {
        public const int STOP = 0;
        public const int WARNING = 1;
        public const int INFO = 2;
    }
}
