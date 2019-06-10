using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace com.gmail.nishantsinhaindia.DocumentConverter
{

    public enum ContentType
    {
        DOC,
        DOCX,
        XLS,
        XLSX,
        VSD,
        VDX,
        PPT,
        PPTX,
        XDW,
        PDF,
        XPS,
        JPEG,
        JPG,
        BMP,
        PNG,
        TIF,
        TIFF,
        GIF,
        SVG,
        TXT,
        RTF,
        XML,
        CSV,
        UNKNOWN = -1
    }
    public class Util
    {
        public static void releaseObject(object obj)
        {
            Console.WriteLine("MethodName: releaseObject of Class: Util started");
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }

            catch (Exception exReleaseObject)
            {
                obj = null;
                //   Console.WriteLine(CMSResourceFile.REALESE_FAILED+ exReleaseObject);

            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Console.WriteLine("MethodName: releaseObject of Class: Util ended");

        }
        public static string getTargetFileName(string contentPath, string contentName, ContentType contentType)
        {
            string getTargetFileNameStatus = string.Empty;

            try
            {
                string tempFileName = Path.GetFileNameWithoutExtension(contentName) + ".";
                tempFileName += contentType.ToString().ToLower();
                getTargetFileNameStatus =  Path.Combine(contentPath, tempFileName);
            }
            catch (Exception exGetTargetFileName)
            {
            }

            return getTargetFileNameStatus;
        }
        public static short getFileExtension(string sourcePath, out ContentType fileExtn)
        {
            short getFileExtnStatus = 0;
            fileExtn = ContentType.UNKNOWN;
            try
            {

                string tempFileExtn = Path.GetExtension(sourcePath);

                tempFileExtn = tempFileExtn.Replace(".", string.Empty);
                fileExtn = (ContentType)Enum.Parse(typeof(ContentType), tempFileExtn, true);
            }
            catch (Exception exGetFileExtension)
            {
            } return getFileExtnStatus;
        }

    }

    public enum ConversionType
    {
        Doc2Pdf,
        Pdf2Img,
        Img2Img
    }
}
