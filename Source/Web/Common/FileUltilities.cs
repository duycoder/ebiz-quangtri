using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using Model.Entities;
//using Spire.Presentation;
//using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using CommonHelper;
using iTextSharpSign;
using wSigner;

namespace Web.Common
{
    public class FileUltilities
    {
        private string path = WebConfigurationManager.AppSettings["FileUpload"];
        private static string CHUKYSO = WebConfigurationManager.AppSettings["CHUKYSO"];
        private static string PASSCHUKYSO = WebConfigurationManager.AppSettings["PASSCHUKYSO"];
        private static string ENABLECHUKYSO = WebConfigurationManager.AppSettings["ENABLECHUKYSO"];
        private static string FolderPath = WebConfigurationManager.AppSettings["FileUpload"];
        public bool CreateFolder(string name)
        {
            bool isCreate = false;

            //string UrlFile = path + name;
            try
            {
                if (!Directory.Exists(name))
                {
                    Directory.CreateDirectory(name);
                    isCreate = true;
                }
            }
            catch
            {
                isCreate = false;
            }
            return isCreate;
        }
        //Xóa thư mục
        public bool RemoveFolder(string name)
        {
            bool isRemove = false;
            //string UrlFile = path + name;
            if (Directory.Exists(name))
            {
                Directory.Delete(name, true);
                isRemove = true;
            }
            return isRemove;
        }
        public void RemoveFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        //Đổi tên thư mục
        public bool RenameFolder(string oldFolder, string newFolder)
        {
            bool isRename = false;
            string str = path + "\\" + oldFolder;
            if (oldFolder != newFolder)
            {
                if (Directory.Exists(str))
                {
                    Directory.Move(path + "\\" + oldFolder, path + "\\" + newFolder);
                    isRename = true;
                }
            }
            else
            {
                isRename = true;
            }
            return isRename;
        }
        /// <summary>
        /// Di chuyển file
        /// </summary>
        /// <param name="oldFolder">thư mục hiện tại</param>
        /// <param name="newFolder">Thư mục mới</param>
        /// <returns></returns>
        public bool MoveFile(string oldFolder, string newFolder)
        {
            try
            {
                if (File.Exists(newFolder))
                {
                    File.Delete(newFolder);
                }
                System.IO.File.Move(oldFolder, newFolder);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool MoveFolder(string fromFolder, string toFolder)
        {
            MoveDir(fromFolder, toFolder);
            return true;
        }
        private void MoveDir(string sourceFolder, string destFolder)
        {
            try
            {
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);

                // Get Files & Copy
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);

                    // ADD Unique File Name Check to Below!!!!
                    string dest = Path.Combine(destFolder, name);
                    File.Move(file, dest);
                }

                // Get dirs recursively and copy files
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    MoveDir(folder, dest);
                }
            }
            catch
            {

            }
        }
        public bool MoveDirectory(string source, string target)
        {
            try
            {
                var sourcePath = source.TrimEnd('\\', ' ');
                var targetPath = target.TrimEnd('\\', ' ');
                var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                                     .GroupBy(s => Path.GetDirectoryName(s));
                foreach (var folder in files)
                {
                    var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                    Directory.CreateDirectory(targetFolder);
                    foreach (var file in folder)
                    {
                        var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                        if (File.Exists(targetFile)) File.Delete(targetFile);
                        File.Move(file, targetFile);
                    }
                }
                Directory.Delete(source, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string ConvertToPdf(string source, string des)
        {
            try
            {
                //Spire.Doc.Document document = new Spire.Doc.Document();
                //document.LoadFromFile(source);
                ////document.LoadFromFile(source, Spire.Doc.FileFormat.Docx);
                //string outputFileName = Path.ChangeExtension(source, "pdf");
                //document.SaveToFile(outputFileName, Spire.Doc.FileFormat.PDF);
                //return des;
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string ConvertPpt2Pdf(string source, string des)
        {
            try
            {
                //Presentation presentation = new Presentation();
                //presentation.LoadFromFile(source);
                //string outputFileName = Path.ChangeExtension(source, "pdf");
                //presentation.SaveToFile(outputFileName, Spire.Presentation.FileFormat.PDF);
                //return des;
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string ConvertExcelToPdf(string source, string des)
        {
            try
            {
                //Workbook workbook = new Workbook();
                //workbook.LoadFromFile(source);
                //string outputFileName = Path.ChangeExtension(source, "pdf");
                //workbook.SaveToFile(outputFileName, Spire.Xls.FileFormat.PDF);
                //return des;
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static void CreateWatermark(string watermarkText, string path, string Signature, Application app)
        {
            try
            {
                var doc = app.Documents.Open(path);
                var shape = doc.Bookmarks["PicHere"].Range.InlineShapes.AddPicture(Signature, false, true);
                shape.Width = 345;
                shape.Height = 150;
                doc.Close();
                object Missing = System.Reflection.Missing.Value;
                object oMissing = System.Reflection.Missing.Value;
                object saveChanges = null;
                Microsoft.Office.Interop.Word.Shape nShape = null;
                string wmText = watermarkText;
                foreach (Microsoft.Office.Interop.Word.Section section in doc.Sections)
                {
                    nShape =
                    section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Shapes.AddTextEffect(MsoPresetTextEffect.msoTextEffect1,
                    wmText, "Calibri", (float)80, MsoTriState.msoTrue,
                    MsoTriState.msoFalse, 0, 350, ref oMissing);
                    nShape.Fill.Visible =
                    MsoTriState.msoTrue;
                    nShape.Line.Visible =
                    MsoTriState.msoFalse;
                    nShape.Fill.Solid();
                    nShape.Fill.ForeColor.RGB = (Int32)WdColor.wdColorGray80;
                    nShape.Left = (float)WdShapePosition.wdShapeLeft;
                    nShape.Top = 250;
                    nShape.Rotation = -45;
                }
                object newFile = path;
                object a = 11;
                doc.SaveAs(ref newFile, ref a, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing);
                doc.Close(ref saveChanges, ref oMissing, ref oMissing);
                app.Quit(ref saveChanges, ref Missing, ref Missing);
            }
            catch
            {

            }
        }
        public static void CreateListWatermark(string watermarkText, List<TAILIEUDINHKEM> ListTaiLieu, string Signature, string personalSign)
        {
            Application app = new Application();
            try
            {
                foreach (var item in ListTaiLieu)
                {
                    var doc = app.Documents.Open(FolderPath + item.DUONGDAN_FILE);
                    try
                    {
                        if (doc.Bookmarks != null && !string.IsNullOrEmpty(personalSign))
                        {
                            if (doc.Bookmarks["chuky"] != null)
                            {
                                var shape = doc.Bookmarks["chuky"].Range.InlineShapes
                                    .AddPicture(personalSign, false, true);
                            }

                        }

                        //var shape = doc.Bookmarks["PicHere"].Range.InlineShapes.AddPicture(Signature, false, true);
                        //shape.Width = 345;
                        //shape.Height = 150;
                        //doc.Close();
                        object Missing = System.Reflection.Missing.Value;
                        object oMissing = System.Reflection.Missing.Value;
                        object saveChanges = null;
                        Microsoft.Office.Interop.Word.Shape nShape = null;
                        string wmText = watermarkText;
                        foreach (Microsoft.Office.Interop.Word.Section section in doc.Sections)
                        {
                            nShape =
                                section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Shapes.AddTextEffect(
                                    MsoPresetTextEffect.msoTextEffect1,
                                    wmText, "Calibri", (float)65, MsoTriState.msoTrue,
                                    MsoTriState.msoFalse, 0, 350, ref oMissing);
                            nShape.Fill.Visible =
                                MsoTriState.msoTrue;
                            nShape.Line.Visible =
                                MsoTriState.msoFalse;
                            nShape.Fill.Solid();
                            nShape.Fill.ForeColor.RGB = (Int32)WdColor.wdColorGray35;
                            nShape.Left = (float)WdShapePosition.wdShapeLeft;
                            nShape.Top = 250;
                            nShape.Rotation = -45;
                        }

                        object newFile = FolderPath + item.DUONGDAN_FILE;
                        object a = 11;
                        doc.SaveAs(ref newFile, ref a, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing,
                            ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing,
                            ref Missing, ref Missing);
                        doc.Close(ref saveChanges, ref oMissing, ref oMissing);
                        app.Quit(ref saveChanges, ref Missing, ref Missing);
                        doc.Close();                        
                    }
                    catch (Exception ex)
                    {
                        doc.Close();
                    }

                    #region ap dung chu ky so
                    if (ENABLECHUKYSO.ToIntOrZero() == 1)
                    {
                        FileUltilities file = new FileUltilities();
                        string fileinput = FolderPath + item.DUONGDAN_FILE;
                        string fileoutput = Path.GetDirectoryName(FolderPath + item.DUONGDAN_FILE) + Path.GetFileNameWithoutExtension(FolderPath + item.DUONGDAN_FILE) + "_1.docx";
                        if (System.IO.File.Exists(fileoutput))
                        {
                            System.IO.File.Delete(fileoutput);
                        }
                        var signer = DocumentSigner.For(fileinput);
                        var cert = CertUtil.GetFromFile(CHUKYSO, PASSCHUKYSO);
                        var serial = cert.SerialNumber;
                        signer.Sign(fileinput, fileoutput, cert);
                        file.MoveFile(fileoutput, fileinput);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void CreateWaterMarkPdf(List<TAILIEUDINHKEM> ListTaiLieu, string waterMark)
        {
            FileUltilities file = new FileUltilities();
            BaseFont bfFont = BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
            iTextSharp.text.pdf.PdfGState gstate = null;
            gstate = new iTextSharp.text.pdf.PdfGState();
            gstate.FillOpacity = 0.3f;
            gstate.StrokeOpacity = 0.3f;
            foreach (var item in ListTaiLieu)
            {
                try
                {
                    #region gan water mark
                    string DestinationFile = Path.GetDirectoryName(FolderPath + item.DUONGDAN_FILE) + Path.GetFileNameWithoutExtension(FolderPath + item.DUONGDAN_FILE) + "_1.pdf";
                    PdfReader reader = new PdfReader(FolderPath + item.DUONGDAN_FILE);
                    PdfStamper stamper = new PdfStamper(reader,
                        new FileStream(DestinationFile, FileMode.Create, FileAccess.Write));
                    for (int iCount = 0; iCount < reader.NumberOfPages; iCount++)
                    {
                        iTextSharp.text.Rectangle PageSize = reader.GetCropBox(iCount + 1);
                        PdfContentByte PDFData = stamper.GetOverContent(iCount + 1);
                        BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                        int altitude = Math.Max(bfFont.GetAscent(waterMark), bfFont.GetDescent(waterMark));
                        int width = bfFont.GetWidth(waterMark);
                        PDFData.SaveState();
                        PDFData.SetGState(gstate);
                        PDFData.BeginText();
                        PDFData.SetColorFill(CMYKColor.LIGHT_GRAY);
                        PDFData.SetFontAndSize(bfFont, (float)80);
                        PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, waterMark, (PageSize.Right + PageSize.Left) / 2, (PageSize.Top + PageSize.Bottom) / 2, 45);
                        PDFData.EndText();
                    }
                    stamper.Close();
                    file.MoveFile(DestinationFile, FolderPath + item.DUONGDAN_FILE);
                    #endregion

                    #region ap dung chu ky so
                    if (ENABLECHUKYSO.ToIntOrZero() == 1)
                    {
                        string TmpDestinationFile = Path.GetDirectoryName(FolderPath + item.DUONGDAN_FILE) + "\\" + Path.GetFileNameWithoutExtension(FolderPath + item.DUONGDAN_FILE) + "_111.pdf";
                        Cert myCert = null;
                        myCert = new Cert(CHUKYSO, PASSCHUKYSO);
                        MetaData metaDT = new MetaData();
                        metaDT.Author = "Duynt";
                        metaDT.Title = "Demo title";
                        metaDT.Subject = "Demo subject";
                        metaDT.Creator = "Duynt create";
                        metaDT.Producer = "HiNet product";

                        PDFSigner pdf = new PDFSigner(FolderPath + item.DUONGDAN_FILE, TmpDestinationFile, myCert, metaDT);
                        pdf.Sign("Reason text", "Contact text", "Location text", true);
                        file.MoveFile(TmpDestinationFile, FolderPath + item.DUONGDAN_FILE);
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }
    }
}