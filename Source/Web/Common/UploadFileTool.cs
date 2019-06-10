using Business.BaseBusiness;
using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using CommonHelper;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Web.Areas.THUMUCLUUTRUArea.Models;
using Web.FwCore;

namespace Web.Common
{
    public class UploadFileTool
    {
        private UnitOfWork context = new UnitOfWork();
        private THUMUC_LUUTRUBusiness THUMUC_LUUTRUBusiness;
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private List<string> arrFolder = new List<string>();
        public UploadFileTool() { }
        /// <summary>
        /// Hàm upload file Version 2 danh cho tải file thêm chọn loại hồ sơ
        /// </summary>
        /// <param name="file">file đầu vào</param>+6398+-+
        /// <param name="Is_MultiFile">cho phép multiple file hay không</param>
        /// <param name="extension">chuỗi định dạng file(.jpg,.png,.doc,.pdf)</param>
        /// <param name="path_Folder">Đường dẫn đến folder</param>
        /// <param name="MaxSize">Kích cỡ tối đa của file (0 = unlimited) tính theo byte</param>
        /// <param name="THUMUC_ID">Thư mục tồn tại trên hệ thống</param>
        /// <param name="ITEM_ID">ID của ....(VD đơn xin nghỉ,đơn xin vắng mặt,kế hoạch nâng lương....)</param>
        /// <param name="ITEM_TYPE">kiểu của item chính là LOAI_TAILIEU trong TAILIEUDINHKEM</param>
        /// <returns></returns>
        public bool UploadCustomFileV2(IEnumerable<HttpPostedFileBase> file, bool Is_MultiFile, string extension, string path_Folder, int MaxSize, string[] THUMUC_ID, string[] filename, string[] fileloaihoso, long ITEM_ID, int ITEM_TYPE = 1, string moduleName = "Đơn xin nghỉ", UserInfoBO user = null)
        {
            MaxSize *= 1048576;
            UserInfoBO UserInfo;
            if (user == null)
            {
                UserInfo = (UserInfoBO)SessionManager.GetUserInfo();
            }
            else
            {
                UserInfo = user;
            }
            if (THUMUC_LUUTRUBusiness == null)
            {
                THUMUC_LUUTRUBusiness = new THUMUC_LUUTRUBusiness(context);
            }
            bool IsSave = false;
            var extend = extension.ToListStringLower(',');
            FileUltilities fileulti = new FileUltilities();
            int count = 0;
            int Folder_ID = 0;
            //Load tất cả fileupload            
            if (file != null)
            {
                foreach (HttpPostedFileBase f in file)
                {
                    if (f != null)
                    {

                        if (string.IsNullOrEmpty(filename[count]))
                        {
                            filename[count] = Path.GetFileNameWithoutExtension(f.FileName);
                        }
                        FileInfo info = new FileInfo(f.FileName);
                        bool IsMaxSize = false;
                        if (MaxSize == 0)
                        {
                            IsMaxSize = true;
                        }
                        else if (f.ContentLength < MaxSize)
                        {
                            IsMaxSize = true;
                        }
                        else
                        {
                            IsMaxSize = false;
                        }
                        if (IsMaxSize)
                        {
                            if (!string.IsNullOrEmpty(info.Extension) && extend.Contains(info.Extension.ToLower()) && !string.IsNullOrEmpty(path_Folder))
                            {
                                //Kiểm tra định dạng file và maxsize                                    
                                string url = "";
                                if (THUMUC_ID != null)
                                {
                                    int.TryParse(THUMUC_ID[count], out Folder_ID);
                                }
                                if (Folder_ID > 0)
                                {
                                    arrFolder.Clear();
                                    //Lấy tất cả thư mục cha
                                    THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(Folder_ID);
                                    arrFolder = this.GetAllParent(THUMUC.PARENT_ID);
                                    //url = this.GetPath(THUMUC.DONVI_ID, THUMUC.Ph, THUMUC.COSO_ID);
                                    url = url + "\\eFile";
                                    arrFolder.Reverse();
                                    for (int i = 0; i < arrFolder.Count; i++)
                                    {
                                        url += "\\" + arrFolder[i];
                                    }
                                    //Kiểm tra thư mục tồn tại hay chưa
                                    if (!Directory.Exists(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO))
                                    {
                                        ////Chưa có: tạo mới folder với đường dẫn như trên
                                        fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO);
                                    }
                                    //Kiểm tra file trùng lặp
                                    string path = Path.Combine(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO, file.ElementAt(count).FileName);
                                    string file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, file.ElementAt(count).FileName);
                                    string pdfFilePath = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(f.FileName) + ".pdf");
                                    if (System.IO.File.Exists(path))
                                    {
                                        filename[count] = filename[count] + DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                                        path = Path.Combine(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                        file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                        pdfFilePath = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                    }
                                    f.SaveAs(path);
                                    IsSave = true;
                                    //Chinh sua loai tai lieu dung constant
                                    this.CreateTaiLieuDinhKem(filename[count], ITEM_TYPE,
                                        filename[count].Trim() == "" ? f.FileName : filename[count].Trim(), file_path,
                                        f.ContentType, (long)UserInfo.ID, THUMUC.ID, 1, ITEM_ID, f.ContentLength, path_Folder, pdfFilePath);
                                }
                                else
                                {
                                    url = this.GetPath(UserInfo.DM_PHONGBAN_ID, UserInfo.ID);
                                    url = url + "\\" + moduleName;
                                    if (Directory.Exists(path_Folder + "\\" + url + "\\" + ITEM_ID))
                                    {
                                        string path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, f.FileName);
                                        string file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, f.FileName);
                                        string pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + ".pdf");
                                        if (System.IO.File.Exists(path))
                                        {
                                            // filename[count] = filename[count] + DateTime.Now.ToString("dd-MM-yyyy hh");

                                            path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                            file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                            pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                        }
                                        f.SaveAs(path);
                                        IsSave = true;
                                        this.CreateTaiLieuDinhKemV2(filename[count], fileloaihoso[count], ITEM_TYPE,
                                            filename[count].Trim() == "" ? f.FileName : filename[count].Trim(),
                                            file_path, info.Extension, (long)UserInfo.ID, 0, 1, ITEM_ID, f.ContentLength, path_Folder, pdfFilePath);
                                    }
                                    else
                                    {
                                        fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + ITEM_ID);
                                        string path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, f.FileName);
                                        string file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, f.FileName);
                                        string pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + ".pdf");
                                        if (System.IO.File.Exists(path))
                                        {
                                            // filename[count] = filename[count] + DateTime.Now.ToString("dd-MM-yyyy hh");
                                            path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                            file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                            pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                        }
                                        f.SaveAs(path);
                                        IsSave = true;
                                        this.CreateTaiLieuDinhKemV2(filename[count], fileloaihoso[count], ITEM_TYPE, filename[count].Trim() == "" ? f.FileName : filename[count].Trim(),
                                            file_path, f.ContentType, (long)UserInfo.ID, 0, 1, ITEM_ID, f.ContentLength, path_Folder, pdfFilePath);
                                    }
                                }
                                //Kiểm tra cho phép multifile (false => thoát)
                                if (!Is_MultiFile)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    count++;
                }
            }
            return IsSave;
        }
        /// <summary>
        /// Hàm upload file
        /// </summary>
        /// <param name="file">file đầu vào</param>+6398+-+
        /// <param name="Is_MultiFile">cho phép multiple file hay không</param>
        /// <param name="extension">chuỗi định dạng file(.jpg,.png,.doc,.pdf)</param>
        /// <param name="path_Folder">Đường dẫn đến folder</param>
        /// <param name="MaxSize">Kích cỡ tối đa của file (0 = unlimited) tính theo byte</param>
        /// <param name="THUMUC_ID">Thư mục tồn tại trên hệ thống</param>
        /// <param name="ITEM_ID">ID của ....(VD đơn xin nghỉ,đơn xin vắng mặt,kế hoạch nâng lương....)</param>
        /// <param name="ITEM_TYPE">kiểu của item chính là LOAI_TAILIEU trong TAILIEUDINHKEM</param>
        /// <returns></returns>
        public bool UploadCustomFile(IEnumerable<HttpPostedFileBase> file, bool Is_MultiFile, string extension, string path_Folder, int MaxSize, string[] THUMUC_ID, string[] filename, long ITEM_ID, int ITEM_TYPE = 1, string moduleName = "Đơn xin nghỉ", UserInfoBO user = null)
        {
            MaxSize *= 1048576;
            UserInfoBO UserInfo;
            if (user == null)
            {
                UserInfo = (UserInfoBO)SessionManager.GetUserInfo();
            }
            else
            {
                UserInfo = user;
            }
            if (THUMUC_LUUTRUBusiness == null)
            {
                THUMUC_LUUTRUBusiness = new THUMUC_LUUTRUBusiness(context);
            }
            bool IsSave = false;
            var extend = extension.ToListStringLower(',');
            FileUltilities fileulti = new FileUltilities();
            int count = 0;
            int Folder_ID = 0;
            //Load tất cả fileupload            
            if (file != null)
            {
                foreach (HttpPostedFileBase f in file)
                {
                    if (f != null)
                    {
                        if (string.IsNullOrEmpty(filename[count]))
                        {
                            filename[count] = Path.GetFileNameWithoutExtension(f.FileName);
                        }
                        FileInfo info = new FileInfo(f.FileName);
                        bool IsMaxSize = false;
                        if (MaxSize == 0)
                        {
                            IsMaxSize = true;
                        }
                        else if (f.ContentLength < MaxSize)
                        {
                            IsMaxSize = true;
                        }
                        else
                        {
                            IsMaxSize = false;
                        }
                        if (IsMaxSize)
                        {
                            if (!string.IsNullOrEmpty(info.Extension) && extend.Contains(info.Extension.ToLower()) && !string.IsNullOrEmpty(path_Folder))
                            {
                                //Kiểm tra định dạng file và maxsize                                    
                                string url = "";
                                if (THUMUC_ID != null)
                                {
                                    int.TryParse(THUMUC_ID[count], out Folder_ID);
                                }
                                if (Folder_ID > 0)
                                {
                                    arrFolder.Clear();
                                    //Lấy tất cả thư mục cha
                                    THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(Folder_ID);
                                    arrFolder = this.GetAllParent(THUMUC.PARENT_ID);
                                    url = this.GetPath(THUMUC.DONVI_ID, THUMUC.USER_ID);
                                    url = url + "\\eFile";
                                    arrFolder.Reverse();
                                    for (int i = 0; i < arrFolder.Count; i++)
                                    {
                                        url += "\\" + arrFolder[i];
                                    }
                                    //Kiểm tra thư mục tồn tại hay chưa
                                    if (!Directory.Exists(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO))
                                    {
                                        ////Chưa có: tạo mới folder với đường dẫn như trên
                                        fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO);
                                    }
                                    //Kiểm tra file trùng lặp
                                    string path = Path.Combine(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO, file.ElementAt(count).FileName);
                                    string file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, file.ElementAt(count).FileName);
                                    string pdfFilePath = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(f.FileName) + ".pdf");
                                    if (System.IO.File.Exists(path))
                                    {
                                        filename[count] = filename[count] + DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                                        path = Path.Combine(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                        file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                        pdfFilePath = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                    }
                                    f.SaveAs(path);
                                    IsSave = true;
                                    //Chinh sua loai tai lieu dung constant
                                    this.CreateTaiLieuDinhKem(filename[count], ITEM_TYPE,
                                        filename[count].Trim() == "" ? f.FileName : filename[count].Trim(),
                                        file_path, f.ContentType, (long)UserInfo.ID, THUMUC.ID, 1, ITEM_ID,
                                        f.ContentLength, path_Folder, pdfFilePath);
                                }
                                else
                                {
                                    //url = "Tài Liệu Lưu Trữ";
                                    url = this.GetPath(UserInfo.DM_PHONGBAN_ID, UserInfo.ID);
                                    url = url + "\\" + moduleName;
                                    if (Directory.Exists(path_Folder + "\\" + url + "\\" + ITEM_ID))
                                    {
                                        string path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, f.FileName);
                                        string file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, f.FileName);
                                        string pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + ".pdf");
                                        if (System.IO.File.Exists(path))
                                        {
                                            // filename[count] = filename[count] + DateTime.Now.ToString("dd-MM-yyyy hh");
                                            path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                            file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                            pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                        }
                                        f.SaveAs(path);
                                        IsSave = true;
                                        this.CreateTaiLieuDinhKem(filename[count], ITEM_TYPE,
                                            filename[count].Trim() == "" ? f.FileName : filename[count].Trim(), file_path,
                                            info.Extension, (long)UserInfo.ID, 0, 1, ITEM_ID, f.ContentLength, path_Folder, pdfFilePath);
                                    }
                                    else
                                    {
                                        fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + ITEM_ID);
                                        string path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, f.FileName);
                                        string file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, f.FileName);
                                        string pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + ".pdf");
                                        if (System.IO.File.Exists(path))
                                        {
                                            // filename[count] = filename[count] + DateTime.Now.ToString("dd-MM-yyyy hh");
                                            path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                            file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                            pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                        }
                                        f.SaveAs(path);
                                        IsSave = true;
                                        this.CreateTaiLieuDinhKem(filename[count], ITEM_TYPE, filename[count].Trim() == "" ? f.FileName : filename[count].Trim(),
                                            file_path, f.ContentType, (long)UserInfo.ID, 0, 1, ITEM_ID, f.ContentLength,
                                            path_Folder, pdfFilePath);
                                    }
                                }
                                //Kiểm tra cho phép multifile (false => thoát)
                                if (!Is_MultiFile)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    count++;
                }
            }
            return IsSave;
        }
        public bool UploadCustomFileAndOutPath(IEnumerable<HttpPostedFileBase> file, bool Is_MultiFile, string extension, string path_Folder, int MaxSize, string[] THUMUC_ID, string[] filename, long ITEM_ID, out List<string> OutFilePath, out List<string> OutFileName, out List<string> OutFileExt, out List<long> OutFileID, int ITEM_TYPE = 1, string moduleName = "Đơn xin nghỉ")
        {
            MaxSize *= 1048576;
            UserInfoBO UserInfo = (UserInfoBO)SessionManager.GetUserInfo();
            if (THUMUC_LUUTRUBusiness == null)
            {
                THUMUC_LUUTRUBusiness = new THUMUC_LUUTRUBusiness(context);
            }
            bool IsSave = false;
            var extend = extension.ToListStringLower(',');
            OutFilePath = new List<string>();
            OutFileName = new List<string>();
            OutFileExt = new List<string>();
            OutFileID = new List<long>();
            FileUltilities fileulti = new FileUltilities();
            int count = 0;
            int Folder_ID = 0;
            //Load tất cả fileupload            
            if (file != null)
            {
                foreach (HttpPostedFileBase f in file)
                {

                    if (f != null)
                    {
                        if (string.IsNullOrEmpty(filename[count]))
                        {
                            filename[count] = Path.GetFileNameWithoutExtension(f.FileName);
                        }
                        FileInfo info = new FileInfo(f.FileName);

                        //Load lan luot định dạng file cho phép
                        bool IsMaxSize = false;
                        if (MaxSize == 0)
                        {
                            IsMaxSize = true;
                        }
                        else if (f.ContentLength < MaxSize)
                        {
                            IsMaxSize = true;
                        }
                        else
                        {
                            IsMaxSize = false;
                        }
                        if (IsMaxSize)
                        {
                            long taiLieuID = 0;
                            var IsValidFile = !string.IsNullOrEmpty(info.Extension) && extend.Contains(info.Extension.ToLower()) && !string.IsNullOrEmpty(path_Folder);
                            if (IsValidFile || string.IsNullOrEmpty(extension))
                            {
                                //Kiểm tra định dạng file và maxsize                                    
                                string url = "";
                                if (THUMUC_ID != null)
                                {
                                    int.TryParse(THUMUC_ID[count], out Folder_ID);
                                }
                                if (Folder_ID > 0)
                                {
                                    arrFolder.Clear();
                                    //Lấy tất cả thư mục cha
                                    THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(Folder_ID);
                                    arrFolder = this.GetAllParent(THUMUC.PARENT_ID);
                                    url = this.GetPath(THUMUC.DONVI_ID, THUMUC.USER_ID);
                                    arrFolder.Reverse();
                                    for (int i = 0; i < arrFolder.Count; i++)
                                    {
                                        url += "\\" + arrFolder[i];
                                    }
                                    //Kiểm tra thư mục tồn tại hay chưa
                                    if (!Directory.Exists(path_Folder + "\\" + url + "\\" + THUMUC.TENTHUMUC))
                                    {
                                        ////Chưa có: tạo mới folder với đường dẫn như trên
                                        fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + THUMUC.TENTHUMUC);
                                    }
                                    string path = Path.Combine(path_Folder + "\\" + url + "\\" + THUMUC.TENTHUMUC, f.FileName);
                                    string file_path = Path.Combine("\\" + url + "\\" + THUMUC.TENTHUMUC, f.FileName);

                                    f.SaveAs(path);
                                    IsSave = true;
                                    //Chinh sua loai tai lieu dung constant
                                    this.CreateTaiLieuDinhKemOutID(filename[count], ITEM_TYPE, filename[count].Trim() == "" ? f.FileName : filename[count].Trim(), file_path, f.ContentType, (int)UserInfo.ID, THUMUC.ID, 1, ITEM_ID, out taiLieuID);

                                    OutFilePath.Add("/Uploads/" + url.Replace("\\", "/") + "/" + THUMUC.TENTHUMUC + "/" + f.FileName);
                                    OutFileName.Add(f.FileName);
                                    OutFileExt.Add(info.Extension.ToLower());
                                    OutFileID.Add(taiLieuID);
                                }
                                else
                                {
                                    //url = "Tài Liệu Lưu Trữ";
                                    url = this.GetPath(UserInfo.DM_PHONGBAN_ID, UserInfo.ID);
                                    if (Directory.Exists(path_Folder + "\\" + url + "\\" + moduleName + "\\" + ITEM_ID))
                                    {
                                        string path = Path.Combine(path_Folder + "\\" + url + "\\" + moduleName + "\\" + ITEM_ID, f.FileName);
                                        string file_path = Path.Combine("\\" + url + "\\" + moduleName + "\\" + ITEM_ID, f.FileName);
                                        f.SaveAs(path);
                                        IsSave = true;
                                        this.CreateTaiLieuDinhKemOutID(filename[count], ITEM_TYPE, filename[count].Trim() == "" ? f.FileName : filename[count].Trim(), file_path, info.Extension, (int)UserInfo.ID, 0, 1, ITEM_ID, out taiLieuID);
                                        OutFilePath.Add("/Uploads/" + url.Replace("\\", "/") + "/" + moduleName + "/" + ITEM_ID + "/" + f.FileName);
                                        OutFileName.Add(f.FileName);
                                        OutFileExt.Add(info.Extension.ToLower());
                                        OutFileID.Add(taiLieuID);
                                    }
                                    else
                                    {
                                        fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + moduleName + "\\" + ITEM_ID);
                                        string path = Path.Combine(path_Folder + "\\" + url + "\\" + moduleName + "\\" + ITEM_ID, f.FileName);
                                        string file_path = Path.Combine("\\" + url + "\\" + moduleName + "\\" + ITEM_ID, f.FileName);
                                        f.SaveAs(path);
                                        IsSave = true;
                                        this.CreateTaiLieuDinhKemOutID(filename[count], ITEM_TYPE, filename[count].Trim() == "" ? f.FileName : filename[count].Trim(), file_path, f.ContentType, (int)UserInfo.ID, 0, 1, ITEM_ID, out taiLieuID);
                                        OutFilePath.Add("/Uploads/" + url.Replace("\\", "/") + "/" + moduleName + "/" + ITEM_ID + "/" + f.FileName);
                                        OutFileName.Add(f.FileName);
                                        OutFileExt.Add(info.Extension.ToLower());
                                        OutFileID.Add(taiLieuID);
                                    }
                                }
                                //Kiểm tra cho phép multifile (false => thoát)
                                if (!Is_MultiFile)
                                {
                                    return true;
                                }
                                //    }
                                //}
                            }
                        }

                    }
                    count++;

                }
            }
            return IsSave;

        }
        public List<string> GetAllParent(long? ID)
        {
            if (ID > 0)
            {
                if (THUMUC_LUUTRUBusiness == null)
                {
                    THUMUC_LUUTRUBusiness = new THUMUC_LUUTRUBusiness(context);
                }
                List<THUMUC_LUUTRU> thumuc = THUMUC_LUUTRUBusiness.GetAllParent(ID.HasValue ? ID.Value : 0);
                thumuc.Reverse();
                foreach (var item in thumuc)
                {
                    arrFolder.Add(item.TENTHUMUC);
                }
            }
            return arrFolder;
        }
        /// <summary>
        /// Lấy cơ sở,đơn vị,phòng ban
        /// </summary>
        /// <param name="DONVI_ID">Dơn vị ID</param>
        /// <param name="PHONGBAN_ID">Phòng Ban ID</param>
        /// <param name="COSO_ID">Cơ sở ID</param>
        /// <returns></returns>
        public string GetPath(int? DONVI_ID, long? USER_ID)
        {
            string url = "";
            if (THUMUC_LUUTRUBusiness == null)
            {
                THUMUC_LUUTRUBusiness = new THUMUC_LUUTRUBusiness(context);
            }
            if (CCTC_THANHPHANBusiness == null)
            {
                CCTC_THANHPHANBusiness = new CCTC_THANHPHANBusiness(context);
            }
            if (DM_NGUOIDUNGBusiness == null)
            {
                DM_NGUOIDUNGBusiness = new DM_NGUOIDUNGBusiness(context);
            }
            var ListParent = CCTC_THANHPHANBusiness.GetDSParent(DONVI_ID.HasValue ? DONVI_ID.Value : 0);
            ListParent.Reverse();
            foreach (var item in ListParent)
            {
                url += "\\" + item.NAME;
            }
            if (!USER_ID.HasValue || USER_ID == 0)
            {
                UserInfoBO userInfo = (UserInfoBO)SessionManager.GetUserInfo();
                url += "\\" + userInfo.TENDANGNHAP;
            }
            else
            {
                DM_NGUOIDUNG NguoiDung = DM_NGUOIDUNGBusiness.Find(USER_ID);
                url += "\\" + (NguoiDung != null ? NguoiDung.TENDANGNHAP : "");
            }

            return url;
        }
        /// <summary>
        /// Thêm mới tài liệu đính kèm version 2 dành cho thêm file có chọn danh mục loại hồ sơ
        /// </summary>
        /// <param name="TENTAILIEU">Tên tài liệu do người dùng nhập</param>
        /// <param name="LOAI_TAILIEU">Loại tài liệu</param>
        /// <param name="MOTA">Mô tả cho tài liệu</param>
        /// <param name="DUONGDAN_FILE">Đường dẫn đến file</param>
        /// <param name="DINHDANG_FILE">Định dạng file (.jpg,.pdf,.docx,.doc)</param>
        /// <param name="USER_ID">ID của người dùng</param>
        /// <param name="FOLDER_ID">ID của thư mục được chọn lưu</param>
        /// <param name="IS_ACTIVE">???</param>
        /// <returns></returns>
        //this.CreateTaiLieuDinhKem(fileName, itemType, fileName, file_path, item.ContentType, (long)user.UserID, itemId, 1, itemId, item.ContentLength);
        private bool CreateTaiLieuDinhKemV2(string TENTAILIEU, string LOAI_HOSO_ID, int LOAI_TAILIEU, string MOTA,
            string DUONGDAN_FILE, string DINHDANG_FILE, long USER_ID, long FOLDER_ID, int IS_ACTIVE, long ITEM_ID,
            long KICHCO, string UrlPath, string PdfVersion)
        {
            bool IS_Create = false;
            TAILIEUDINHKEM TAILIEU = new TAILIEUDINHKEM();
            if (TAILIEUDINHKEMBusiness == null)
            {
                TAILIEUDINHKEMBusiness = new TAILIEUDINHKEMBusiness(context);
            }
            TAILIEU.TENTAILIEU = TENTAILIEU;
            TAILIEU.LOAI_TAILIEU = LOAI_TAILIEU;
            TAILIEU.MOTA = MOTA;
            TAILIEU.DUONGDAN_FILE = DUONGDAN_FILE;
            TAILIEU.DINHDANG_FILE = DINHDANG_FILE;
            TAILIEU.USER_ID = USER_ID;
            TAILIEU.FOLDER_ID = FOLDER_ID;
            TAILIEU.IS_ACTIVE = IS_ACTIVE;
            TAILIEU.SOLUONG_DOWNLOAD = 0;
            TAILIEU.ITEM_ID = ITEM_ID;
            TAILIEU.NGAYTAO = DateTime.Now;
            TAILIEU.DM_LOAITAILIEU_ID = 0;
            //TAILIEU.IS_MODIFY = 0;
            TAILIEU.IS_DELETE = false;
            TAILIEU.IS_LOCK = false;
            TAILIEU.NGUOI_LOCK = 0;
            TAILIEU.IS_QLPHIENBAN = true;
            TAILIEU.KICHCO = KICHCO / 1024;
            if (IsWord(DINHDANG_FILE))
            {
                FileUltilities file = new FileUltilities();
                TAILIEU.PDF_VERSION = file.ConvertToPdf(UrlPath + DUONGDAN_FILE, PdfVersion);
            }
            else if (IsPpt(DINHDANG_FILE))
            {
                FileUltilities file = new FileUltilities();
                TAILIEU.PDF_VERSION = file.ConvertPpt2Pdf(UrlPath + DUONGDAN_FILE, PdfVersion);
            }
            else if (IsExcel(DINHDANG_FILE))
            {
                FileUltilities file = new FileUltilities();
                TAILIEU.PDF_VERSION = file.ConvertExcelToPdf(UrlPath + DUONGDAN_FILE, PdfVersion);
            }
            if (TAILIEUDINHKEMBusiness.Save(TAILIEU))
            {
                IS_Create = true;
            }
            return IS_Create;
        }
        /// <summary>
        /// Thêm mới tài liệu đính kèm
        /// </summary>
        /// <param name="TENTAILIEU">Tên tài liệu do người dùng nhập</param>
        /// <param name="LOAI_TAILIEU">Loại tài liệu</param>
        /// <param name="MOTA">Mô tả cho tài liệu</param>
        /// <param name="DUONGDAN_FILE">Đường dẫn đến file</param>
        /// <param name="DINHDANG_FILE">Định dạng file (.jpg,.pdf,.docx,.doc)</param>
        /// <param name="USER_ID">ID của người dùng</param>
        /// <param name="FOLDER_ID">ID của thư mục được chọn lưu</param>
        /// <param name="IS_ACTIVE">???</param>
        /// <returns></returns>
        //this.CreateTaiLieuDinhKem(fileName, itemType, fileName, file_path, item.ContentType, (long)user.UserID, itemId, 1, itemId, item.ContentLength);
        private bool CreateTaiLieuDinhKem(string TENTAILIEU, int LOAI_TAILIEU, string MOTA, string DUONGDAN_FILE,
            string DINHDANG_FILE, long USER_ID, long FOLDER_ID, int IS_ACTIVE, long ITEM_ID, long KICHCO, string UrlPath, string PdfVersion)
        {
            bool IS_Create = false;
            TAILIEUDINHKEM TAILIEU = new TAILIEUDINHKEM();
            if (TAILIEUDINHKEMBusiness == null)
            {
                TAILIEUDINHKEMBusiness = new TAILIEUDINHKEMBusiness(context);
            }
            TAILIEU.TENTAILIEU = TENTAILIEU;
            TAILIEU.LOAI_TAILIEU = LOAI_TAILIEU;
            TAILIEU.MOTA = MOTA;
            TAILIEU.DUONGDAN_FILE = DUONGDAN_FILE;
            TAILIEU.DINHDANG_FILE = DINHDANG_FILE;
            TAILIEU.USER_ID = USER_ID;
            TAILIEU.FOLDER_ID = FOLDER_ID;
            TAILIEU.IS_ACTIVE = IS_ACTIVE;
            TAILIEU.SOLUONG_DOWNLOAD = 0;
            TAILIEU.ITEM_ID = ITEM_ID;
            TAILIEU.NGAYTAO = DateTime.Now;
            TAILIEU.DM_LOAITAILIEU_ID = 0;
            //TAILIEU.IS_MODIFY = 0;
            TAILIEU.IS_LOCK = false;
            TAILIEU.NGUOI_LOCK = 0;
            TAILIEU.IS_QLPHIENBAN = true;
            TAILIEU.IS_DELETE = false;
            TAILIEU.KICHCO = KICHCO / 1024;
            if (IsWord(DINHDANG_FILE))
            {
                FileUltilities file = new FileUltilities();
                TAILIEU.PDF_VERSION = file.ConvertToPdf(UrlPath + DUONGDAN_FILE, PdfVersion);
            }
            else if (IsPpt(DINHDANG_FILE))
            {
                FileUltilities file = new FileUltilities();
                TAILIEU.PDF_VERSION = file.ConvertPpt2Pdf(UrlPath + DUONGDAN_FILE, PdfVersion);
            }
            else if (IsExcel(DINHDANG_FILE))
            {
                FileUltilities file = new FileUltilities();
                TAILIEU.PDF_VERSION = file.ConvertExcelToPdf(UrlPath + DUONGDAN_FILE, PdfVersion);
            }
            if (TAILIEUDINHKEMBusiness.Save(TAILIEU))
            {
                IS_Create = true;
            }
            return IS_Create;
        }
        private bool CreateTaiLieuDinhKemOutID(string TENTAILIEU, int LOAI_TAILIEU, string MOTA, string DUONGDAN_FILE, string DINHDANG_FILE, int USER_ID, long FOLDER_ID, int IS_ACTIVE, long ITEM_ID, out long OutID)
        {
            OutID = 0;
            bool IS_Create = false;
            TAILIEUDINHKEM TAILIEU = new TAILIEUDINHKEM();
            if (TAILIEUDINHKEMBusiness == null)
            {
                TAILIEUDINHKEMBusiness = new TAILIEUDINHKEMBusiness(context);
            }
            TAILIEU.TENTAILIEU = TENTAILIEU;
            TAILIEU.LOAI_TAILIEU = LOAI_TAILIEU;
            TAILIEU.MOTA = MOTA;
            TAILIEU.DUONGDAN_FILE = DUONGDAN_FILE;
            TAILIEU.DINHDANG_FILE = DINHDANG_FILE;
            TAILIEU.USER_ID = USER_ID;
            TAILIEU.FOLDER_ID = FOLDER_ID;
            TAILIEU.IS_ACTIVE = IS_ACTIVE;
            TAILIEU.SOLUONG_DOWNLOAD = 0;
            TAILIEU.ITEM_ID = ITEM_ID;
            TAILIEU.NGAYTAO = DateTime.Now;
            TAILIEU.DM_LOAITAILIEU_ID = 0;
            //TAILIEU.IS_MODIFY = 0;
            TAILIEU.IS_LOCK = false;
            TAILIEU.NGUOI_LOCK = 0;
            //TAILIEU.KICHCO = KICHCO / 1024;
            TAILIEU.IS_DELETE = false;
            if (TAILIEUDINHKEMBusiness.Save(TAILIEU))
            {
                IS_Create = true;
            }
            OutID = TAILIEU.TAILIEU_ID;
            return IS_Create;
        }
        private long CreateTaiLieuDinhKemver2(string TENTAILIEU, int LOAI_TAILIEU, string MOTA, string DUONGDAN_FILE,
            string DINHDANG_FILE, long USER_ID, long FOLDER_ID, int IS_ACTIVE, long ITEM_ID, int LOAITAILIE_ID,
            int IS_PHEDUYET, string CODE, string TACGIA, DateTime? NGAYPHATHANH, bool? IS_PRIVATE, long KICHCO,
            string VERSION, int TRANGTHAI, int? PERMISSION, string UrlPath, string PdfVersion)
        {

            TAILIEUDINHKEM TAILIEU = new TAILIEUDINHKEM();
            if (TAILIEUDINHKEMBusiness == null)
            {
                TAILIEUDINHKEMBusiness = new TAILIEUDINHKEMBusiness(context);
            }
            TAILIEU.TENTAILIEU = TENTAILIEU;
            TAILIEU.LOAI_TAILIEU = LOAI_TAILIEU;
            TAILIEU.MOTA = MOTA;
            TAILIEU.DUONGDAN_FILE = DUONGDAN_FILE;
            TAILIEU.DINHDANG_FILE = DINHDANG_FILE;
            TAILIEU.USER_ID = USER_ID;
            TAILIEU.FOLDER_ID = FOLDER_ID;
            TAILIEU.IS_ACTIVE = IS_ACTIVE;
            TAILIEU.SOLUONG_DOWNLOAD = 0;
            TAILIEU.ITEM_ID = ITEM_ID;
            TAILIEU.NGAYTAO = DateTime.Now;
            TAILIEU.DM_LOAITAILIEU_ID = string.IsNullOrEmpty(LOAITAILIE_ID.ToString()) == true ? 0 : LOAITAILIE_ID;
            TAILIEU.IS_LOCK = false;
            TAILIEU.NGUOI_LOCK = 0;
            TAILIEU.IS_QLPHIENBAN = true;
            TAILIEU.IS_PHEDUYET = IS_PHEDUYET;
            TAILIEU.MATAILIEU = CODE;
            TAILIEU.TENTACGIA = TACGIA;
            TAILIEU.NGAYPHATHANH = NGAYPHATHANH;
            TAILIEU.IS_PRIVATE = IS_PRIVATE;
            TAILIEU.KICHCO = KICHCO / 1024;
            TAILIEU.VERSION = VERSION;
            TAILIEU.TRANGTHAI = TRANGTHAI;
            TAILIEU.IS_DELETE = false;
            TAILIEU.PERMISSION = PERMISSION;
            if (IsWord(DINHDANG_FILE))
            {
                FileUltilities file = new FileUltilities();
                TAILIEU.PDF_VERSION = file.ConvertToPdf(UrlPath + DUONGDAN_FILE, PdfVersion);
            }
            else if (IsPpt(DINHDANG_FILE))
            {
                FileUltilities file = new FileUltilities();
                TAILIEU.PDF_VERSION = file.ConvertPpt2Pdf(UrlPath + DUONGDAN_FILE, PdfVersion);
            }
            else if (IsExcel(DINHDANG_FILE))
            {
                FileUltilities file = new FileUltilities();
                TAILIEU.PDF_VERSION = file.ConvertExcelToPdf(UrlPath + DUONGDAN_FILE, PdfVersion);
            }
            if (TAILIEUDINHKEMBusiness.Save(TAILIEU))
            {
                return TAILIEU.TAILIEU_ID;
            }
            return 0;
        }
        public bool UploadSingleFile(int PHEDUYET, UserInfoBO user, IEnumerable<HttpPostedFileBase> file, string path_Folder, long ITEM_ID, string mota, int ITEM_TYPE = 1, string moduleName = "Hình đại diện tài liệu")
        {
            UserInfoBO UserInfo = (UserInfoBO)SessionManager.GetUserInfo();
            if (THUMUC_LUUTRUBusiness == null)
            {
                THUMUC_LUUTRUBusiness = new THUMUC_LUUTRUBusiness(context);
            }
            bool IsSave = false;
            FileUltilities fileulti = new FileUltilities();
            if (file != null)
            {
                for (int count = 0; count < file.Count(); count++)
                {
                    if (file.ElementAt(count) != null)
                    {
                        FileInfo info = new FileInfo(file.ElementAt(count).FileName);
                        string url = "";
                        if (!string.IsNullOrEmpty(path_Folder))
                        {

                            url = this.GetPath(UserInfo.DM_PHONGBAN_ID, UserInfo.ID);
                            //Kiểm tra folder đã tồn tại hay chưa
                            if (Directory.Exists(path_Folder + "\\" + url + "\\" + moduleName + "\\" + ITEM_ID))
                            {
                                string path = Path.Combine(path_Folder + "\\" + url + "\\" + moduleName + "\\" + ITEM_ID, file.ElementAt(count).FileName);
                                string file_path = Path.Combine("\\" + url + "\\" + moduleName + "\\" + ITEM_ID, file.ElementAt(count).FileName);
                                string pdfFilePath = Path.Combine("\\" + url + "\\" + moduleName + "\\" + ITEM_ID, GetFileName(file.ElementAt(count).FileName) + ".pdf");
                                if (System.IO.File.Exists(path))
                                {
                                    path = Path.Combine(path_Folder + "\\" + url + "\\" + moduleName + "\\" + ITEM_ID, GetFileName(file.ElementAt(count).FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(file.ElementAt(count).FileName));
                                    file_path = Path.Combine("\\" + url + "\\" + moduleName + "\\" + ITEM_ID, GetFileName(file.ElementAt(count).FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(file.ElementAt(count).FileName));
                                    pdfFilePath = Path.Combine("\\" + url + "\\" + moduleName + "\\" + ITEM_ID, GetFileName(file.ElementAt(count).FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                }
                                file.ElementAt(count).SaveAs(path);
                                IsSave = true;
                                CreateTaiLieuDinhKem(file.ElementAt(count).FileName, ITEM_TYPE,
                                    file.ElementAt(count).FileName, file_path, file.ElementAt(count).ContentType,
                                    (long)user.ID, ITEM_ID, 1, ITEM_ID, file.ElementAt(count).ContentLength, path_Folder, pdfFilePath);
                            }
                            else
                            {
                                fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + moduleName + "\\" + ITEM_ID);
                                string path = Path.Combine(path_Folder + "\\" + url + "\\" + moduleName + "\\" + ITEM_ID, file.ElementAt(count).FileName);
                                string file_path = Path.Combine("\\" + url + "\\" + moduleName + "\\" + ITEM_ID, file.ElementAt(count).FileName);
                                string pdfFilePath = Path.Combine("\\" + url + "\\" + moduleName + "\\" + ITEM_ID, GetFileName(file.ElementAt(count).FileName) + ".pdf");
                                if (System.IO.File.Exists(path))
                                {
                                    path = Path.Combine(path_Folder + "\\" + url + "\\" + moduleName + "\\" + ITEM_ID, GetFileName(file.ElementAt(count).FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(file.ElementAt(count).FileName));
                                    file_path = Path.Combine("\\" + url + "\\" + moduleName + "\\" + ITEM_ID, GetFileName(file.ElementAt(count).FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(file.ElementAt(count).FileName));
                                    pdfFilePath = Path.Combine("\\" + url + "\\" + moduleName + "\\" + ITEM_ID, GetFileName(file.ElementAt(count).FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                }
                                file.ElementAt(count).SaveAs(path);
                                IsSave = true;
                                CreateTaiLieuDinhKem(file.ElementAt(count).FileName, ITEM_TYPE,
                                    file.ElementAt(count).FileName, file_path, file.ElementAt(count).ContentType,
                                    (long)user.ID, ITEM_ID, 1, ITEM_ID, file.ElementAt(count).ContentLength, path_Folder, pdfFilePath);
                            }
                        }
                    }
                }

            }
            return IsSave;
        }
        public bool UploadCustomSingleFile(HttpPostedFileBase file, string extension, string path_Folder, int MaxSize, string THUMUC_ID, string filename, long ITEM_ID, int ITEM_TYPE = 1, string moduleName = "Đơn xin nghỉ", UserInfoBO user = null)
        {
            MaxSize *= 1048576;
            UserInfoBO UserInfo;
            if (user == null)
            {
                UserInfo = (UserInfoBO)SessionManager.GetUserInfo();
            }
            else
            {
                UserInfo = user;
            }
            if (THUMUC_LUUTRUBusiness == null)
            {
                THUMUC_LUUTRUBusiness = new THUMUC_LUUTRUBusiness(context);
            }
            bool IsSave = false;
            var extend = extension.ToListStringLower(',');

            FileUltilities fileulti = new FileUltilities();
            int Folder_ID = 0;
            //Load tất cả fileupload            
            if (file != null)
            {


                FileInfo info = new FileInfo(file.FileName);

                bool IsMaxSize = false;
                if (MaxSize == 0)
                {
                    IsMaxSize = true;
                }
                else if (file.ContentLength < MaxSize)
                {
                    IsMaxSize = true;
                }
                else
                {
                    IsMaxSize = false;
                }
                if (IsMaxSize)
                {
                    if (!string.IsNullOrEmpty(info.Extension) && extend.Contains(info.Extension.ToLower()) && !string.IsNullOrEmpty(path_Folder))
                    {
                        //Kiểm tra định dạng file và maxsize                                    
                        string url = "";
                        if (THUMUC_ID != null)
                        {
                            int.TryParse(THUMUC_ID, out Folder_ID);
                        }
                        if (Folder_ID > 0)
                        {
                            arrFolder.Clear();
                            //Lấy tất cả thư mục cha
                            THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(Folder_ID);
                            arrFolder = this.GetAllParent(THUMUC.PARENT_ID);
                            url = this.GetPath(THUMUC.DONVI_ID, THUMUC.USER_ID);
                            url = url + "\\eFile";
                            arrFolder.Reverse();
                            for (int i = 0; i < arrFolder.Count; i++)
                            {
                                url += "\\" + arrFolder[i];
                            }
                            //Kiểm tra thư mục tồn tại hay chưa
                            if (!Directory.Exists(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO))
                            {
                                ////Chưa có: tạo mới folder với đường dẫn như trên
                                fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO);
                            }
                            //Kiểm tra file trùng lặp
                            string path = Path.Combine(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO, file.FileName);
                            string file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, file.FileName);
                            string pdfFilePath = "\\" + url + "\\" + THUMUC.THUMUC_AO;
                            if (System.IO.File.Exists(path))
                            {
                                filename = filename + DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                                path = Path.Combine(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO, file.FileName.Split('.')[0] + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + "." + file.FileName.Split('.')[1]);
                                file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, file.FileName.Split('.')[0] + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + "." + file.FileName.Split('.')[1]);
                            }
                            file.SaveAs(path);
                            IsSave = true;
                            //Chinh sua loai tai lieu dung constant
                            this.CreateTaiLieuDinhKem(filename, ITEM_TYPE, filename == null ? file.FileName : filename.Trim(),
                                file_path, file.ContentType, (long)UserInfo.ID, THUMUC.ID, 1, ITEM_ID, file.ContentLength, path_Folder, pdfFilePath);
                        }
                        else
                        {
                            //url = "Tài Liệu Lưu Trữ";
                            url = this.GetPath(UserInfo.DM_PHONGBAN_ID, UserInfo.ID);
                            url = url + "\\" + moduleName;
                            if (Directory.Exists(path_Folder + "\\" + url + "\\" + ITEM_ID))
                            {
                                string path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, file.FileName);
                                string file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, file.FileName);
                                string pdfFilePath = "\\" + url + "\\" + ITEM_ID;
                                if (System.IO.File.Exists(path))
                                {
                                    // filename[count] = filename[count] + DateTime.Now.ToString("dd-MM-yyyy hh");
                                    path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, file.FileName.Split('.')[0] + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + "." + file.FileName.Split('.')[1]);
                                    file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, file.FileName.Split('.')[0] + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + "." + file.FileName.Split('.')[1]);
                                }
                                file.SaveAs(path);
                                IsSave = true;
                                this.CreateTaiLieuDinhKem(filename, ITEM_TYPE, filename == null ? file.FileName : filename.Trim(),
                                    file_path, info.Extension, (long)UserInfo.ID, 0, 1, ITEM_ID, file.ContentLength,
                                    path_Folder, pdfFilePath);
                            }
                            else
                            {
                                fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + ITEM_ID);
                                string path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, file.FileName);
                                string file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, file.FileName);
                                string pdfFilePath = "\\" + url + "\\" + ITEM_ID;
                                if (System.IO.File.Exists(path))
                                {
                                    // filename[count] = filename[count] + DateTime.Now.ToString("dd-MM-yyyy hh");
                                    path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, file.FileName.Split('.')[0] + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + "." + file.FileName.Split('.')[1]);
                                    file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, file.FileName.Split('.')[0] + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + "." + file.FileName.Split('.')[1]);
                                }
                                file.SaveAs(path);
                                IsSave = true;
                                this.CreateTaiLieuDinhKem(filename, ITEM_TYPE, filename == null ? file.FileName : filename.Trim(),
                                    file_path, file.ContentType, (long)UserInfo.ID, 0, 1, ITEM_ID, file.ContentLength,
                                    path_Folder, pdfFilePath);
                            }
                        }
                        //Kiểm tra cho phép multifile (false => thoát)

                    }
                }

            }
            return IsSave;
        }
        public List<long> UploadCustomFileVer2(int IS_PHEDUYET, IEnumerable<HttpPostedFileBase> file, bool Is_MultiFile,
            string extension, string path_Folder, int MaxSize, long THUMUC_ID, string[] filename, long ITEM_ID,
            string[] LOAITAILIEU, string mota, string[] filecode, string TACGIA, DateTime? NGAYPHATHANH,
            bool? IS_PRIVATE, string VERSION, int TRANGTHAI, int? PERMISSION, int ITEM_TYPE = 1, string moduleName = "Đơn xin nghỉ")
        {
            MaxSize *= 1048576;
            List<long> ListTaiLieu = new List<long>();
            UserInfoBO UserInfo = (UserInfoBO)SessionManager.GetUserInfo();
            if (THUMUC_LUUTRUBusiness == null)
            {
                THUMUC_LUUTRUBusiness = new THUMUC_LUUTRUBusiness(context);
            }
            var extend = extension.ToListStringLower(',');
            FileUltilities fileulti = new FileUltilities();
            //Load tất cả fileupload            
            if (file != null)
            {
                if (LOAITAILIEU == null)
                {
                    LOAITAILIEU = new string[file.Count()];
                }
                for (int count = 0; count < file.Count(); count++)
                {
                    if (file.ElementAt(count) != null)
                    {
                        if (string.IsNullOrEmpty(filename[count]))
                        {
                            filename[count] = Path.GetFileNameWithoutExtension(file.ElementAt(count).FileName);
                        }
                        FileInfo info = new FileInfo(file.ElementAt(count).FileName);
                        //Load lan luot định dạng file cho phép
                        if (!string.IsNullOrEmpty(info.Extension) && extend.Contains(info.Extension.ToLower()))
                        {
                            bool IsMaxSize = false;
                            if (MaxSize == 0)
                            {
                                IsMaxSize = true;
                            }
                            else if (file.ElementAt(count).ContentLength < MaxSize)
                            {
                                IsMaxSize = true;
                            }
                            else
                            {
                                IsMaxSize = false;
                            }
                            if (IsMaxSize)
                            {
                                if (!string.IsNullOrEmpty(path_Folder))
                                {
                                    //Kiểm tra định dạng file và maxsize 
                                    string url = "";
                                    if (THUMUC_ID > 0)
                                    {
                                        arrFolder.Clear();
                                        //Lấy tất cả thư mục cha
                                        THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(THUMUC_ID);
                                        arrFolder = this.GetAllParent(THUMUC.PARENT_ID);
                                        url = this.GetPath(THUMUC.DONVI_ID, THUMUC.USER_ID);
                                        url += "\\" + moduleName;
                                        arrFolder.Reverse();
                                        for (int i = 0; i < arrFolder.Count; i++)
                                        {
                                            url += "\\" + arrFolder[i];
                                        }
                                        //Kiểm tra thư mục tồn tại hay chưa
                                        if (!Directory.Exists(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO))
                                        {
                                            ////Chưa có: tạo mới folder với đường dẫn như trên
                                            fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO);
                                        }
                                        //Kiểm tra file trùng lặp
                                        string ss = path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO + "\\" + file.ElementAt(count).FileName;
                                        string path = Path.Combine(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO, file.ElementAt(count).FileName);
                                        string file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, file.ElementAt(count).FileName);
                                        string pdfFilePath = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(file.ElementAt(count).FileName) + ".pdf");
                                        if (System.IO.File.Exists(path))
                                        {
                                            if (!string.IsNullOrEmpty(filename[count]))
                                            {
                                                filename[count] = filename[count] + DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                                            }
                                            path = Path.Combine(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO, file.ElementAt(count).FileName.Split('.')[0] + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + "." + file.ElementAt(count).FileName.Split('.')[1]);
                                            file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(file.ElementAt(count).FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(file.ElementAt(count).FileName));
                                            pdfFilePath = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(file.ElementAt(count).FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                        }
                                        file.ElementAt(count).SaveAs(path);
                                        long ID = this.CreateTaiLieuDinhKemver2(filename[count], ITEM_TYPE, mota == "" ? filename[count] : mota,
                                            file_path, file.ElementAt(count).ContentType, (long)UserInfo.ID, THUMUC.ID, 1,
                                            ITEM_ID, LOAITAILIEU[count].ToIntOrZero(), IS_PHEDUYET, filecode[count], TACGIA, NGAYPHATHANH,
                                            IS_PRIVATE, file.ElementAt(count).ContentLength, VERSION, TRANGTHAI,
                                            PERMISSION, path_Folder, pdfFilePath);
                                        // this.ElasticInsert(file_path, ID, file.ElementAt(count).ContentType, mota == "" ? filename[count] : mota, filename[count]);\
                                        ListTaiLieu.Add(ID);
                                    }
                                    if (!Is_MultiFile)
                                    {
                                        return ListTaiLieu;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ListTaiLieu;
        }
        public bool SaveDragDropFile(List<HttpPostedFileBase> lstFile, bool isMultiple, string extension, string pathFolder, long maxSize, long folderId, int status, int itemType, UserInfoBO user, THUMUC_LUUTRU THUMUC, string moduleName)
        {
            if (user == null)
            {
                user = (UserInfoBO)SessionManager.GetUserInfo();
            }
            var extend = extension.ToListStringLower(',');
            FileUltilities fileulti = new FileUltilities();
            bool isSave = false;
            foreach (var item in lstFile)
            {
                if (item != null)
                {
                    FileInfo fileInfo = new FileInfo(item.FileName);
                    string fileName = item.FileName;
                    if (!string.IsNullOrEmpty(fileInfo.Extension) && extend.Contains(fileInfo.Extension.ToLower()))
                    {
                        bool IsMaxSize = false;
                        if (maxSize == 0 || item.ContentLength < maxSize)
                        {
                            IsMaxSize = true;
                        }
                        else
                        {
                            IsMaxSize = false;
                        }
                        if (IsMaxSize)
                        {
                            if (!string.IsNullOrEmpty(pathFolder))
                            {
                                //Kiểm tra định dạng file và maxsize 
                                string url = "";
                                if (folderId > 0)
                                {
                                    arrFolder.Clear();
                                    //Lấy tất cả thư mục cha
                                    arrFolder = this.GetAllParent(THUMUC.PARENT_ID);
                                    url = this.GetPath(THUMUC.DONVI_ID, THUMUC.USER_ID);
                                    url += "\\" + moduleName;
                                    arrFolder.Reverse();
                                    for (int i = 0; i < arrFolder.Count; i++)
                                    {
                                        url += "\\" + arrFolder[i];
                                    }
                                    //Kiểm tra thư mục tồn tại hay chưa
                                    if (!Directory.Exists(pathFolder + "\\" + url + "\\" + THUMUC.THUMUC_AO))
                                    {
                                        ////Chưa có: tạo mới folder với đường dẫn như trên
                                        fileulti.CreateFolder(pathFolder + "\\" + url + "\\" + THUMUC.THUMUC_AO);
                                    }
                                    //Kiểm tra file trùng lặp
                                    string path = Path.Combine(pathFolder + "\\" + url + "\\" + THUMUC.THUMUC_AO, fileName);
                                    string file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, fileName);
                                    string pdfFilePath = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(item.FileName) + ".pdf");
                                    if (System.IO.File.Exists(path))
                                    {
                                        if (!string.IsNullOrEmpty(fileName))
                                        {
                                            fileName = fileName + DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                                        }
                                        path = Path.Combine(pathFolder + "\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(item.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(item.FileName));
                                        file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(item.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(item.FileName));
                                        pdfFilePath = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(item.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                    }
                                    item.SaveAs(path);
                                    isSave = true;
                                    this.CreateTaiLieuDinhKem(fileName, itemType, fileName, file_path, item.ContentType,
                                        (long)user.ID, folderId, 1, folderId, item.ContentLength, pathFolder, pdfFilePath);
                                }
                                if (!isMultiple)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return isSave;
        }
        /// <summary>
        /// Hàm này chỉ áp dụng đối với trường hợp muốn hiển thị trong thư mục lưu trữ
        /// </summary>
        /// <param name="file"></param>
        /// <param name="Is_MultiFile"></param>
        /// <param name="extension"></param>
        /// <param name="path_Folder"></param>
        /// <param name="MaxSize"></param>
        /// <param name="THUMUC_ID"></param>
        /// <param name="filename"></param>
        /// <param name="ITEM_ID"></param>
        /// <param name="ITEM_TYPE"></param>
        /// <param name="moduleName"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UploadCustomFileVer3(IEnumerable<HttpPostedFileBase> file, bool Is_MultiFile, string extension, string path_Folder, int MaxSize, string[] THUMUC_ID, string[] filename, long ITEM_ID, int ITEM_TYPE = 1, string moduleName = "Đơn xin nghỉ", UserInfoBO user = null)
        {
            if (file == null)
            {
                return false;
            }
            MaxSize *= 1048576;
            UserInfoBO UserInfo;
            if (user == null)
            {
                UserInfo = (UserInfoBO)SessionManager.GetUserInfo();
            }
            else
            {
                UserInfo = user;
            }
            if (THUMUC_LUUTRUBusiness == null)
            {
                THUMUC_LUUTRUBusiness = new THUMUC_LUUTRUBusiness(context);
            }
            #region Thêm thư mục lưu trữ cho văn bản hay công việc đang được thêm tài liệu
            long id = 0;
            if (ITEM_TYPE == LOAITAILIEU.CONGVIEC)
            {
                id = ThuMucLuuTruConstant.DefaultCongViec;
            }
            else if (ITEM_TYPE == LOAITAILIEU.VANBANDEN)
            {
                id = ThuMucLuuTruConstant.DefaultVbDen;
            }
            else
            {
                id = ThuMucLuuTruConstant.DefaultVanban;
            }
            THUMUC_LUUTRU ThuMuc = THUMUC_LUUTRUBusiness.GetDataByNam(ITEM_ID.ToString(), id);
            if (ThuMuc == null)
            {
                ThuMuc = new THUMUC_LUUTRU();
                ThuMuc.ACCESS_MODIFIER = AccessModifier.PRIVATE;
                ThuMuc.DONVI_ID = UserInfo.DM_PHONGBAN_ID;
                ThuMuc.IS_DELETE = false;
                ThuMuc.NAM = DateTime.Now.Year;
                ThuMuc.NGAYTAO = DateTime.Now;
                ThuMuc.NGAYXOA = DateTime.Now;
                if (ITEM_TYPE == LOAITAILIEU.CONGVIEC)
                {
                    ThuMuc.PARENT_ID = ThuMucLuuTruConstant.DefaultCongViec;
                    ThuMuc.FIXED_FOLDER_ID = ThuMucLuuTruConstant.DefaultCongViec;
                }
                else if (ITEM_TYPE == LOAITAILIEU.VANBANDEN)
                {
                    ThuMuc.PARENT_ID = ThuMucLuuTruConstant.DefaultVbDen;
                    ThuMuc.FIXED_FOLDER_ID = ThuMucLuuTruConstant.DefaultVbDen;
                }
                else
                {
                    ThuMuc.PARENT_ID = ThuMucLuuTruConstant.DefaultVanban;
                    ThuMuc.FIXED_FOLDER_ID = ThuMucLuuTruConstant.DefaultVanban;
                }
                ThuMuc.PERMISSION = FolderPermission.CAN_READ;
                ThuMuc.TENTHUMUC = ITEM_ID.ToString();
                ThuMuc.THUMUC_AO = ITEM_ID.ToString();
                ThuMuc.USER_ID = UserInfo.ID;
                ThuMuc.IS_FIXED = true;
                THUMUC_LUUTRUBusiness.Save(ThuMuc);
            }
            else if (ThuMuc.IS_DELETE.HasValue && ThuMuc.IS_DELETE.Value)
            {
                ThuMuc.IS_DELETE = false;
                THUMUC_LUUTRUBusiness.Save(ThuMuc);
            }
            if (THUMUC_ID == null)
            {
                THUMUC_ID = new string[file.Count()];
            }
            if (!THUMUC_ID.Any())
            {
                var size = THUMUC_ID.Length;
                for (int i = 0; i < size; i++)
                {
                    THUMUC_ID[i] = ThuMuc.ID.ToString();
                }
            }
            else
            {
                var size = THUMUC_ID.Length;
                for (int i = 0; i < size; i++)
                {
                    THUMUC_ID[i] = ThuMuc.ID.ToString();
                }
            }
            #endregion
            bool IsSave = false;
            var extend = extension.ToListStringLower(',');
            FileUltilities fileulti = new FileUltilities();
            int count = 0;
            long Folder_ID = 0;
            //Load tất cả fileupload            
            foreach (HttpPostedFileBase f in file)
            {
                if (f != null)
                {
                    if (string.IsNullOrEmpty(filename[count]))
                    {
                        filename[count] = Path.GetFileNameWithoutExtension(f.FileName);
                    }
                    FileInfo info = new FileInfo(f.FileName);
                    bool IsMaxSize = false;
                    if (MaxSize == 0)
                    {
                        IsMaxSize = true;
                    }
                    else if (f.ContentLength < MaxSize)
                    {
                        IsMaxSize = true;
                    }
                    else
                    {
                        IsMaxSize = false;
                    }
                    if (IsMaxSize)
                    {
                        if (!string.IsNullOrEmpty(info.Extension) && extend.Contains(info.Extension.ToLower()) && !string.IsNullOrEmpty(path_Folder))
                        {
                            //Kiểm tra định dạng file và maxsize                                    
                            string url = "";
                            if (THUMUC_ID != null)
                            {
                                long.TryParse(THUMUC_ID[count], out Folder_ID);
                            }
                            if (Folder_ID > 0)
                            {
                                arrFolder.Clear();
                                //Lấy tất cả thư mục cha
                                THUMUC_LUUTRU THUMUC = THUMUC_LUUTRUBusiness.Find(Folder_ID);
                                arrFolder = this.GetAllParent(THUMUC.PARENT_ID);
                                url = this.GetPath(THUMUC.DONVI_ID, THUMUC.USER_ID);
                                url = url + "\\eFile";
                                arrFolder.Reverse();
                                for (int i = 0; i < arrFolder.Count; i++)
                                {
                                    url += "\\" + arrFolder[i];
                                }
                                //Kiểm tra thư mục tồn tại hay chưa
                                if (!Directory.Exists(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO))
                                {
                                    ////Chưa có: tạo mới folder với đường dẫn như trên
                                    fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO);
                                }
                                //Kiểm tra file trùng lặp
                                string path = Path.Combine(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO, file.ElementAt(count).FileName);
                                string file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, file.ElementAt(count).FileName);
                                string pdfFilePath = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(f.FileName) + ".pdf");
                                if (System.IO.File.Exists(path))
                                {
                                    filename[count] = filename[count] + DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                                    path = Path.Combine(path_Folder + "\\" + url + "\\" + THUMUC.THUMUC_AO, file.ElementAt(count).FileName.Split('.')[0] + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + "." + file.ElementAt(count).FileName.Split('.')[1]);
                                    file_path = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, file.ElementAt(count).FileName.Split('.')[0] + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + "." + file.ElementAt(count).FileName.Split('.')[1]);
                                    pdfFilePath = Path.Combine("\\" + url + "\\" + THUMUC.THUMUC_AO, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                }
                                f.SaveAs(path);
                                IsSave = true;
                                //Chinh sua loai tai lieu dung constant
                                this.CreateTaiLieuDinhKem(filename[count], ITEM_TYPE,
                                    filename[count].Trim() == "" ? f.FileName : filename[count].Trim(),
                                    file_path, f.ContentType, (long)UserInfo.ID, THUMUC.ID, 1, ITEM_ID,
                                    f.ContentLength, path_Folder, pdfFilePath);
                            }
                            else
                            {
                                //url = "Tài Liệu Lưu Trữ";
                                url = this.GetPath(UserInfo.DM_PHONGBAN_ID, UserInfo.ID);
                                url = url + "\\" + moduleName;
                                if (Directory.Exists(path_Folder + "\\" + url + "\\" + ITEM_ID))
                                {
                                    string path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, f.FileName);
                                    string file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, f.FileName);
                                    string pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + ".pdf");
                                    if (System.IO.File.Exists(path))
                                    {
                                        // filename[count] = filename[count] + DateTime.Now.ToString("dd-MM-yyyy hh");
                                        path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                        file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                        pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                    }
                                    f.SaveAs(path);
                                    IsSave = true;
                                    this.CreateTaiLieuDinhKem(filename[count], ITEM_TYPE,
                                        filename[count].Trim() == "" ? f.FileName : filename[count].Trim(),
                                        file_path, info.Extension, (long)UserInfo.ID, 0, 1, ITEM_ID,
                                        f.ContentLength, path_Folder, pdfFilePath);
                                }
                                else
                                {
                                    fileulti.CreateFolder(path_Folder + "\\" + url + "\\" + ITEM_ID);
                                    string path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, f.FileName);
                                    string file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, f.FileName);
                                    string pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + ".pdf");
                                    if (System.IO.File.Exists(path))
                                    {
                                        path = Path.Combine(path_Folder + "\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                        file_path = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + GetFileExtension(f.FileName));
                                        pdfFilePath = Path.Combine("\\" + url + "\\" + ITEM_ID, GetFileName(f.FileName) + "-" + DateTime.Now.ToString("dd-MM-yyyy hh") + ".pdf");
                                    }
                                    f.SaveAs(path);
                                    IsSave = true;
                                    this.CreateTaiLieuDinhKem(filename[count], ITEM_TYPE,
                                        filename[count].Trim() == "" ? f.FileName : filename[count].Trim(),
                                        file_path, f.ContentType, (long)UserInfo.ID, 0, 1, ITEM_ID,
                                        f.ContentLength, path_Folder, pdfFilePath);
                                }
                            }
                            //Kiểm tra cho phép multifile (false => thoát)
                            if (!Is_MultiFile)
                            {
                                return true;
                            }
                        }
                    }
                }
                count++;
            }
            return IsSave;
        }

        /// <summary>
        /// @author: duynn
        /// @description: cập nhật file
        /// </summary>
        /// <param name="files"></param>
        /// <param name="extensions"></param>
        /// <param name="rootFolder"></param>
        /// <param name="fileNames"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="fileSize"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public bool UploadFiles(IEnumerable<HttpPostedFileBase> files, List<string> fileExtensions, string rootFolder, string[] fileNames, long itemId, int itemType, int fileSize, UserInfoBO currentUser)
        {
            TAILIEUDINHKEMBusiness = new TAILIEUDINHKEMBusiness(context);
            bool result = true;
            fileSize *= 1048576;
            int fileCount = files != null ? files.Where(x => x != null).Count() : 0;
            string folderPathToSave = string.Empty;

            //đường dẫn thư mục lưu file bắt đầu từ thư mục gốc
            string suffixesFolderPath = string.Empty;
            if (fileCount > 0)
            {
                int dept = currentUser.DM_PHONGBAN_ID.GetValueOrDefault();
                long userId = currentUser.ID;
                //hậu tố thư mục lưu trữ
                suffixesFolderPath = "\\" + dept + "\\" + userId + "\\" + itemType + "\\" + itemId;
                folderPathToSave = Path.Combine(rootFolder + suffixesFolderPath);

                if (!System.IO.Directory.Exists(folderPathToSave))
                {
                    System.IO.Directory.CreateDirectory(folderPathToSave);
                }
            }

            for (int i = 0; i < fileCount; i++)
            {
                HttpPostedFileBase file = files.ElementAt(i);
                FileInfo info = new FileInfo(file.FileName);
                string extension = info.Extension.ToLower(); //=>.png, .exe, .jpg
                if (fileSize > 0 && file.ContentLength > fileSize)
                {
                    continue;
                }
                else if (fileExtensions != null && !fileExtensions.Contains(extension))
                {
                    continue;
                }
                else
                {
                    //tiêu đề file mà bạn đặt
                    string fileTitle = fileNames[i];
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(info.Name);

                    if (string.IsNullOrEmpty(fileTitle))
                    {
                        fileTitle = fileNameWithoutExtension;
                    }

                    string finalFileName = info.Name;
                    string filePathToSave = folderPathToSave + "\\" + info.Name;

                    if (System.IO.File.Exists(filePathToSave))
                    {
                        finalFileName = fileNameWithoutExtension + "-" + string.Format("{0:dd-MM-yyyy HH-mm-ss}", DateTime.Now) + extension;
                        filePathToSave = folderPathToSave + "\\" + finalFileName;
                    }
                    file.SaveAs(filePathToSave);

                    TAILIEUDINHKEM att = new TAILIEUDINHKEM();
                    att.TENTACGIA = currentUser.HOTEN;
                    att.TENTAILIEU = fileTitle;
                    att.USER_ID = currentUser.ID;
                    att.DUONGDAN_FILE = Path.Combine(suffixesFolderPath, finalFileName);
                    att.IS_ACTIVE = 1;
                    att.IS_DELETE = false;
                    att.IS_LOCK = false;
                    att.ITEM_ID = itemId;
                    att.LOAI_TAILIEU = itemType;
                    att.DINHDANG_FILE = file.ContentType;
                    att.KICHCO = file.ContentLength;
                    att.NGAYTAO = DateTime.Now;
                    TAILIEUDINHKEMBusiness.Save(att);
                }
            }
            return result;
        }


        private bool IsWord(string extension)
        {
            if (extension.Equals(".docx") || extension.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                || extension.Equals("application/vnd.ms-word.document.12") || extension.Equals("application/msword"))
            {
                return true;
            }
            return false;
        }
        private bool IsPpt(string extension)
        {
            if (extension.Equals(".pptx") || extension.Equals("application/vnd.openxmlformats-officedocument.presentationml.presentation")
                || extension.Equals(".ppt") || extension.Equals("application/vnd.ms-powerpoint"))
            {
                return true;
            }
            return false;
        }
        private bool IsExcel(string extension)
        {
            if (extension.Equals(".xlsx") || extension.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                || extension.Equals("application/vnd.ms-excel") || extension.Equals(".xls"))
            {
                return true;
            }
            return false;
        }
        private string GetFileName(string file)
        {
            return Path.GetFileNameWithoutExtension(file);
        }
        private string GetFileExtension(string file)
        {
            return Path.GetExtension(file);
        }

        public List<string> GetWordExtension()
        {
            List<string> ListExtension = new List<string>();
            ListExtension.Add(".docx");
            ListExtension.Add("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            ListExtension.Add("application/vnd.ms-word.document.12");
            ListExtension.Add("application/msword");
            return ListExtension;
        }
        public List<string> GetPdfExtension()
        {
            List<string> ListExtension = new List<string>();
            ListExtension.Add(".pdf");
            ListExtension.Add("application/pdf");
            ListExtension.Add("x-pdf");
            return ListExtension;
        }
    }
}
