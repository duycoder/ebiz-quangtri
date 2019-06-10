using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSWord = Microsoft.Office.Interop.Word;
using MSWordApp = Microsoft.Office.Interop.Word.Application;
using MSDocument = Microsoft.Office.Interop.Word.Document;
using MSFindWrap = Microsoft.Office.Interop.Word.WdFindWrap;
using MSReplace = Microsoft.Office.Interop.Word.WdReplace;
using System.IO;
using Novacode;
using System.Text.RegularExpressions;
using System.Reflection;

namespace CommonHelper.Word
{
    public class ExportWordHelper<T> where T : class
    {
        public string templateFilePath { set; get; } //đường dẫn file mẫu
        public string tempFolderPath { set; get; } //thư mục chứa file tạm
        public string outputFolderPath { set; get; } //thư mục chứa file kết quả
        public string fileName { set; get; } //tên file kết quả

        public MSWordApp app { set; get; }
        public MSDocument doc { set; get; }

        /// <summary>
        /// @description: convert và kết xuất pdf
        /// @author: duynn
        /// @since: 23/03/2018
        /// </summary>
        /// <param name="outputFolderPath">đường dẫn thư mục lưu trữ file được kết xuất</param>
        /// <param name="fileWordPath">đường dẫn file word mẫu</param>
        /// <returns></returns>
        public ExportPDFResult ConvertAndExportToPDF(string outputFolderPath, string fileWordPath)
        {
            ExportPDFResult result = new ExportPDFResult();
            if (string.IsNullOrEmpty(outputFolderPath) || Directory.Exists(outputFolderPath) == false)
            {
                result.exportResultMessage = "Thư mục lưu trữ không tồn tại";
                return result;
            }

            if (string.IsNullOrEmpty(fileWordPath) || File.Exists(fileWordPath) == false)
            {
                result.exportResultMessage = "File biểu mẫu không tồn tại";
                return result;
            }
            MSWordApp app = new MSWordApp();
            File.SetAttributes(fileWordPath, FileAttributes.Normal);

            MSDocument document = app.Documents.Open(fileWordPath);
            app.Visible = false;
            document.Activate();

            string fileWordName = Path.GetFileName(fileWordPath);
            string filePdfName = fileWordName.Replace(".doc", string.Empty)
                    .Replace(".dot", string.Empty)
                    .Replace(".docx", string.Empty)
                    .Replace(".docm ", string.Empty)
                    .Replace(".dotx ", string.Empty)
                    .Replace(".dotm", string.Empty)
                    .Replace("docb ", string.Empty) + ".pdf";
            string outputFilePath = Path.Combine(outputFolderPath, filePdfName);
            if (File.Exists(outputFilePath))
            {
                filePdfName = filePdfName.Replace(".pdf", string.Empty) + DateTime.Now.ToString("-ddMMyyyy_hhmmss") + ".pdf";
                outputFilePath = Path.Combine(outputFolderPath, filePdfName);
            }

            document.ExportAsFixedFormat(outputFilePath, MSWord.WdExportFormat.wdExportFormatPDF);
            document.Close();
            app.Quit();
            result.exportSuccess = true;
            result.exportResultFileName = filePdfName;
            return result;
        }

        /// <summary>
        /// @description: hàm kết xuất excel cho sản phẩm
        /// @author: duynn
        /// @since: 23/03/2018
        /// </summary>
        /// <param name="objectToExport"></param>
        /// <returns></returns>
        public ExportWordResult Export(T objectToExport)
        {
            ExportWordResult exportResult = new ExportWordResult();

            //kiểm tra file biểu mẫu tồn tại
            if (string.IsNullOrEmpty(templateFilePath) || File.Exists(templateFilePath) == false)
            {
                exportResult.exportResultMessage = "File biểu mẫu không tồn tại!";
                return exportResult;
            }

            //kiểm tra thư mục xuất tồn tại
            if (string.IsNullOrEmpty(outputFolderPath) || Directory.Exists(outputFolderPath) == false)
            {
                exportResult.exportResultMessage = "Thư mục kết xuất không tồn tại!";
                return exportResult;
            }

            //kiểm tra thư mục tạm tồn tại
            if (string.IsNullOrEmpty(tempFolderPath) || Directory.Exists(tempFolderPath) == false)
            {
                exportResult.exportResultMessage = "Thư mục lưu trữ không tồn tại!";
                return exportResult;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                exportResult.exportResultMessage = "Tên file không được để trống";
                return exportResult;
            }

            //lấy tên file biểu mẫu
            string templateFileName = Path.GetFileName(templateFilePath);

            //tạo tên cho file tạm (phòng khi bị trùng)
            string tempFileName = SetNewFileName(templateFileName);
            //tạo đường dẫn cho file tạm
            string tempFilePath = Path.Combine(tempFolderPath, tempFileName);

            FileInfo fileTemplate = new FileInfo(templateFilePath);
            fileTemplate.CopyTo(tempFilePath);

            app = new MSWordApp();
            doc = app.Documents.Open(templateFilePath);
            try
            {
                app.Visible = false;
                doc.Activate();

                DocX docX = DocX.Load(tempFilePath);
                List<string> wordsToReplace = docX.FindUniqueByPattern(@"\[\[\w*\]\]", RegexOptions.IgnoreCase);

                if (wordsToReplace != null && wordsToReplace.Count() > 0)
                {
                    Type objectType = typeof(T);

                    foreach (string wordToReplace in wordsToReplace)
                    {
                        object replaceValue = string.Empty;
                        string propertyName = wordToReplace.Replace("[[", string.Empty).Replace("]]", string.Empty).Trim();
                        if (string.IsNullOrEmpty(propertyName) == false)
                        {
                            PropertyInfo property = objectType.GetProperty(propertyName);
                            if (property != null)
                            {
                                replaceValue = property.GetValue(objectToExport);
                            }
                        }
                        Replace(app, wordToReplace, replaceValue);
                    }
                }

                //save file
                string outputFilePath = Path.Combine(outputFolderPath, fileName);
                if (File.Exists(outputFilePath))
                {
                    fileName = SetNewFileName(fileName);
                    outputFilePath = Path.Combine(outputFolderPath, fileName);
                }
                app.ActiveDocument.SaveAs2(outputFilePath);

                exportResult.exportSuccess = true;
                exportResult.exportResultUrl = outputFilePath;
                exportResult.exportResultFileName = fileName;
                return exportResult;
            }
            catch (Exception ex)
            {
                exportResult.exportResultMessage = ex.Message;
                return exportResult;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close();
                }
                app.Quit();
            }
        }

        private void Replace(MSWordApp application, object replacePosition, object replaceValue)
        {
            object matchCase = true;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object nmatchAllWordForms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = false;
            object visible = true;
            object replace = 2;
            object wrap = MSFindWrap.wdFindContinue;
            object replaceAll = MSReplace.wdReplaceAll;
            application.Selection.Find.Execute(ref replacePosition, ref matchCase, ref matchWholeWord, ref matchWildCards, ref matchSoundsLike,
            ref nmatchAllWordForms, ref forward, ref wrap, ref format, ref replaceValue, ref replaceAll, ref matchKashida,
            ref matchDiacritics, ref matchAlefHamza, ref matchControl);
        }

        private string SetNewFileName(string oldFileName)
        {
            string result = string.Empty;
            result = oldFileName.Replace(".doc", string.Empty)
                    .Replace(".dot", string.Empty)
                    .Replace(".docx", string.Empty)
                    .Replace(".docm", string.Empty)
                    .Replace(".dotx", string.Empty)
                    .Replace(".dotm", string.Empty)
                    .Replace("docb", string.Empty) + DateTime.Now.ToString("-ddMMyyyy_hhmmss") + ".docx";
            return result;
        }

    }

    public class ExportWordResult
    {
        public bool exportSuccess { set; get; }
        public string exportResultMessage { set; get; }
        public string exportResultUrl { set; get; }
        public string exportResultFileName { set; get; }
        public ExportWordResult()
        {
            exportResultMessage = string.Empty;
            exportResultUrl = string.Empty;
            exportResultFileName = string.Empty;
        }
    }

    public class ExportPDFResult : ExportWordResult
    {

    }
}
