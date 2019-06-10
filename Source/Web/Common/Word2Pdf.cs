using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;

namespace Web.Common
{
    public class Word2Pdf
    {
        Microsoft.Office.Interop.Word.Application _Word = new Microsoft.Office.Interop.Word.Application();
        object _MissingValue = System.Reflection.Missing.Value;

        public void Convert(string wordFileName,string pdfFileName)
        {
            _Word.Visible = false;
            _Word.ScreenUpdating = false;
            // Cast as Object for word Open method
            object filename = (object)wordFileName;
            // Use the dummy value as a placeholder for optional arguments
            Document doc = _Word.Documents.Open(ref filename, ref _MissingValue,
             ref _MissingValue, ref _MissingValue, ref _MissingValue, ref _MissingValue, ref _MissingValue,
             ref _MissingValue, ref _MissingValue, ref _MissingValue, ref _MissingValue, ref _MissingValue,
             ref _MissingValue, ref _MissingValue, ref _MissingValue, ref _MissingValue);
            doc.Activate();
            //object outputFileName = pdfFileName = Path.ChangeExtension(wordFileName, "pdf");
            object outputFileName = (object)pdfFileName;
            object fileFormat = WdSaveFormat.wdFormatPDF;
            // Save document into PDF Format
            doc.SaveAs(ref outputFileName,
             ref fileFormat, ref _MissingValue, ref _MissingValue,
             ref _MissingValue, ref _MissingValue, ref _MissingValue, ref _MissingValue,
             ref _MissingValue, ref _MissingValue, ref _MissingValue, ref _MissingValue,
             ref _MissingValue, ref _MissingValue, ref _MissingValue, ref _MissingValue);
            // Close the Word document, but leave the Word application open.
            // doc has to be cast to type _Document so that it will find the
            // correct Close method.    
            object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
            ((_Document)doc).Close(ref saveChanges, ref _MissingValue, ref _MissingValue);
            doc = null;

            // word has to be cast to type _Application so that it will find
            // the correct Quit method.
            ((_Application)_Word).Quit(ref _MissingValue, ref _MissingValue, ref _MissingValue);
            _Word = null;
            //return outputFileName.ToString();
        }
    }
}