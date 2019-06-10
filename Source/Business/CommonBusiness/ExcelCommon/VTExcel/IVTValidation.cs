using NPOI.HSSF.UserModel;
using System;
namespace VTUtils.Excel.Export
{
    public interface IVTValidation
    {
        void CreateCellValidation(int sheetIndex, int fromRow, int fromColumn,
            int toRow, int toColumn, int validationType,
            string fromValue, string toValue, string[] lstContrains = null, 
            string errorTitle = "Error value", string errorMessage = "Error input value", int errorStyle = ErrorStyle.STOP);
        
        void CompleteValidation();
        
        string FilePath { get; set; }

        System.IO.FileStream sfile { get; set; }
       
    }
}
