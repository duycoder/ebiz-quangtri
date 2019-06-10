using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CommonHelper.Upload
{
    public class UploadResult
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string fullPath { get; set; }
        public string path { get; set; }
    }
    public class UploadProvider
    {

        /// <summary>
        /// Lưu file
        /// </summary>
        /// <param name="file">FileBase cần lưu</param>
        /// <param name="name">TeenFile Muốn lưu</param>
        /// <param name="folder">Tên folder chứa file trong thư mục upload</param>
        /// <param name="PathSave">Đường dẫn thư mục upload</param>
        /// <returns></returns>
        public static UploadResult SaveFile(HttpPostedFileBase file, string name, string extentionList, int? maxSize, string folder, string PathSave)
        {
            var result = new UploadResult();
            var fileName = "";


            #region 1.Kiểm tra có ghi đè tên file không
            if (string.IsNullOrEmpty(name))
            {
                fileName = file.FileName;
            }
            else
            {
                fileName = name;
            }
            #endregion



            var dt = DateTime.Now;
            var arrName = fileName.Split('.');
            var extention = '.' + arrName[arrName.Length - 1];
            var Name_File = string.Join(".", arrName, 0, arrName.Length - 1);

            #region Check extention có hợp lệ không

            if (!string.IsNullOrEmpty(extentionList))
            {
                var listExtention = extentionList.Split(',');
                if (!listExtention.Contains(extention))
                {
                    result.status = false;
                    result.message = "Định dạng file không được chấp nhận";
                    return result;
                }
            }
            #endregion

            #region CheckSize
            if (maxSize.HasValue && file.ContentLength > maxSize)
            {
                result.status = false;
                result.message = "File vượt quá kích cỡ cho phép";
                return result;
            }
            #endregion

            #region 2. Kiểm tra có lưu thư mục riêng không. Nếu chưa có thì tạo

            var pathFolder = "";
            if (string.IsNullOrEmpty(folder))
            {
                folder = "Unknow";

            }

            pathFolder = Path.Combine(PathSave, folder);



            //Kiểm tra folder đã tồn tại chưa. Nếu chưa tồn tại rồi thì tạo mới
            if (!Directory.Exists(pathFolder))
            {
                try
                {
                    Directory.CreateDirectory(pathFolder);
                }
                catch
                {

                    result.status = false;
                    result.message = "Không tạo được folder";
                    return result;
                }

            }

            #endregion

            #region 3.Kiểm tra File đã tồn tại chưa? Nếu tồn tại sửa tên

            var pathFile = Path.Combine(pathFolder, fileName); //Đường đẫn vật lý của file;

            if (File.Exists(pathFile))
            {

                Name_File += string.Format("{0:ddMMyyyy-hhmmss}", dt);
                fileName = Name_File + extention;

                pathFile = Path.Combine(pathFolder, fileName);
            }

            #endregion

            #region 4. Lưu file
            try
            {
                file.SaveAs(pathFile);
                var URLFILE = Path.Combine(folder, fileName);
                result.status = true;
                result.path = URLFILE;
                result.fullPath = pathFile;
            }
            catch
            {
                result.status = false;
                result.message = "Không lưu được tài liệu";
                result.path = "";
                result.fullPath = "";
                return result;
            }

            #endregion

            return result;
        }
    }
}
