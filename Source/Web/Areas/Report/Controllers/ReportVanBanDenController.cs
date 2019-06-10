using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using CommonHelper;
using CommonHelper.DateExtend;
using CommonHelper.Excel;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.Report.Models;
using Web.Custom;
using Web.FwCore;

namespace Web.Areas.Report.Controllers
{
    public class ReportVanBanDenController : BaseController
    {
        //đường dẫn thư mục chứa file đẩy lên server
        private static string UPLOAD_PATH = WebConfigurationManager.AppSettings["FileUpload"];
        //đường dẫn mẫu báo cáo văn bản đến theo loại
        private static string TEMPLATE_REPORT_LOAI_PATH = WebConfigurationManager.AppSettings["BieuMauBaoCaoVanBanDenTheoLoai"];
        private static string TEMPLATE_REPORT_DONVIGUI_PATH = TEMPLATE_REPORT_LOAI_PATH;
        private static string TEMPLATE_REPORT_PATH = TEMPLATE_REPORT_LOAI_PATH;
        //đường dẫn mẫu báo cáo văn bản đến theo lĩnh vực
        private static string TEMPLATE_REPORT_LINHVUC_PATH = WebConfigurationManager.AppSettings["BieuMauBaoCaoVanBanDenTheoLinhVuc"];

        //đường dẫn thư mục lưu tạm file upload
        private static string TEMPORARY_UPLOAD_FOLDER_PATH = WebConfigurationManager.AppSettings["TempUploadFolderPath"];
        //đường dẫn thư mục lưu file kết quả dạng excel
        private static string OUTPUT_EXECL_FOLDER_PATH = WebConfigurationManager.AppSettings["TempUploadExcelFolderPath"];
        private HSCV_VANBANDENBusiness hscvVanBanDenBusiness;
        private DM_DANHMUC_DATABusiness dmDanhMucDataBusiness;
        private CCTC_THANHPHANBusiness cctcThanhPhanBusiness;
        // GET: Report/ReportVanBanDen
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy biểu mẫu tiêu chí báo cáo thống kê
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult GetReportCriteriaForm(int reportType)
        {
            AssignUserInfo();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();
            cctcThanhPhanBusiness = Get<CCTC_THANHPHANBusiness>();
            ReportVanBanDenFilterViewModel viewModel = new ReportVanBanDenFilterViewModel(reportType);
            switch (reportType)
            {
                case LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LOAI_VANBANDEN:
                    viewModel.groupOfItemCategoryFilter = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.LOAI_VANBAN, 0);
                    break;
                case LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LINHVUC_VANBANDEN:
                    viewModel.groupOfItemCategoryFilter = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.LINHVUCVANBAN, 0);
                    break;
                default:
                    viewModel.groupOfItemCategoryFilter = dmDanhMucDataBusiness.DsByMaNhom(DMLOAI_CONSTANT.DONVIGUI_VANBAN, 0);
                    break;
            }
            var demo = currentUser.DeptParentID.GetValueOrDefault();
            viewModel.groupOfDeparmtents = cctcThanhPhanBusiness.GetChildrenOfDepartments(currentUser.DM_PHONGBAN_ID.GetValueOrDefault(), currentUser.DeptParentID.GetValueOrDefault(), currentUser);
            return PartialView("_ReportVanBanDenFilter", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy kết quả báo cáo thống kê
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult GetReportResult(FormCollection form)
        {
            AssignUserInfo();
            string categoryCode = string.Empty;
            string reportTitle = string.Empty;
            string departmentExtensionTitle = string.Empty;
            int reportType = form["reportType"].ToIntOrZero();
            int timeFilterType = form["timeFilterType"].ToIntOrZero();
            int queryItemCategoryType = form["queryItemCategoryType"].ToIntOrZero();
            int queryDepartment = form["queryDepartment"].ToIntOrZero();
            DateTime? queryDateStart;
            DateTime? queryDateEnd;
            hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();
            cctcThanhPhanBusiness = Get<CCTC_THANHPHANBusiness>();
            CCTC_THANHPHAN department = cctcThanhPhanBusiness.Find(queryDepartment);
            departmentExtensionTitle = department != null ? string.Format("CỦA ĐƠN VỊ {0}", department.NAME.ToUpper()) : string.Empty;

            DM_DANHMUC_DATA category = dmDanhMucDataBusiness.Find(queryItemCategoryType) ?? new DM_DANHMUC_DATA();
            category.TEXT = string.IsNullOrEmpty(category.TEXT) ? string.Empty : category.TEXT.ToUpper();

            ReportVanBanDenResultViewModel viewModel = new ReportVanBanDenResultViewModel();
            viewModel.reportType = reportType;

            if (reportType == LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LOAI_VANBANDEN)
            {
                categoryCode = DMLOAI_CONSTANT.LOAI_VANBAN;
            }
            else if (reportType == LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LINHVUC_VANBANDEN)
            {
                categoryCode = DMLOAI_CONSTANT.LINHVUCVANBAN;
            }
            else
            {
                categoryCode = DMLOAI_CONSTANT.DONVIGUI_VANBAN;
            }

            if (timeFilterType == LOAI_BAOCAO_THOIGIAN_CONSTANT.NGAY)
            {
                queryDateStart = form["queryDateStart"].ToDateTime();
                queryDateEnd = form["queryDateEnd"].ToDateTime();
                if (reportType == LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LOAI_VANBANDEN)
                {
                    reportTitle = string.Format("THỐNG KÊ VĂN BẢN ĐẾN THEO LOẠI {0} TỪ NGÀY {1} ĐẾN NGÀY {2} {3}", category.TEXT, queryDateStart.Value.ToVietnameseDateFormat(), queryDateEnd.Value.ToVietnameseDateFormat(), departmentExtensionTitle);
                }
                else if (reportType == LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LINHVUC_VANBANDEN)
                {
                    reportTitle = string.Format("THỐNG KÊ VĂN BẢN ĐẾN THEO LĨNH VỰC {0} TỪ NGÀY {1} ĐẾN NGÀY {2} {3}", category.TEXT, queryDateStart.Value.ToVietnameseDateFormat(), queryDateEnd.Value.ToVietnameseDateFormat(), departmentExtensionTitle);
                }
                else
                {
                    reportTitle = string.Format("THỐNG KÊ VĂN BẢN ĐẾN THEO ĐƠN VỊ {0} GỬI TỪ NGÀY {1} ĐẾN NGÀY {2} {3}", category.TEXT, queryDateStart.Value.ToVietnameseDateFormat(), queryDateEnd.Value.ToVietnameseDateFormat(), departmentExtensionTitle);
                }
            }
            else if (timeFilterType == LOAI_BAOCAO_THOIGIAN_CONSTANT.THANG)
            {
                int month = form["queryMonth"].ToIntOrZero();
                int year = form["queryYear"].ToIntOrZero();
                queryDateStart = new DateTime(year, month, 1);
                queryDateEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                if (reportType == LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LOAI_VANBANDEN)
                {
                    reportTitle = string.Format("BÁO CÁO VĂN BẢN ĐẾN THEO LOẠI {0} TRONG THÁNG {1} NĂM {2} {3}", category.TEXT, month, year, departmentExtensionTitle);
                }
                else if (reportType == LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LINHVUC_VANBANDEN)
                {
                    reportTitle = string.Format("BÁO CÁO VĂN BẢN ĐẾN THEO LĨNH VỰC {0} TRONG THÁNG {1} NĂM {2} {3}", category.TEXT, month, year, departmentExtensionTitle);
                }
                else
                {
                    reportTitle = string.Format("BÁO CÁO VĂN BẢN ĐẾN THEO ĐƠN VỊ {0} GỬI TRONG THÁNG {1} NĂM {2} {3}", category.TEXT, month, year, departmentExtensionTitle);
                }
            }
            else
            {
                int year = form["queryYearOnly"].ToIntOrZero();
                queryDateStart = new DateTime(year, 1, 1);
                queryDateEnd = new DateTime(year, 12, 31);
                if (reportType == LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LOAI_VANBANDEN)
                {
                    reportTitle = string.Format("BÁO CÁO VĂN BẢN ĐẾN THEO LOẠI {0} TRONG NĂM {1} {2}", category.TEXT, year, departmentExtensionTitle);
                }
                else if (reportType == LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LINHVUC_VANBANDEN)
                {
                    reportTitle = string.Format("BÁO CÁO VĂN BẢN ĐẾN THEO LĨNH VỰC {0} TRONG NĂM {1} {2}", category.TEXT, year, departmentExtensionTitle);
                }
                else
                {
                    reportTitle = string.Format("BÁO CÁO VĂN BẢN ĐẾN THEO ĐƠN VỊ {0} GỬI TRONG NĂM {1} {2}", category.TEXT, year, departmentExtensionTitle);
                }
            }
            viewModel.title = reportTitle;
            viewModel.groupOfReportResultItems = hscvVanBanDenBusiness.GetStatisticResultByCategory(queryDepartment, MODULE_CONSTANT.VANBANDEN, categoryCode, queryDateStart, queryDateEnd, queryItemCategoryType);
            SessionManager.SetValue("ReportVanBanDen" + reportType, viewModel);
            return PartialView("_ReportVanBanDenResult", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: kết xuất excel biểu mẫu báo cáo thống kê
        /// </summary>
        /// <param name="reportType"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelReportResult(int reportType)
        {
            JsonResultBO result = new JsonResultBO(true);
            ReportVanBanDenResultViewModel data = (ReportVanBanDenResultViewModel)SessionManager.GetValue("ReportVanBanDen" + reportType);
            string fileName = string.Empty;
            if (reportType == LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LINHVUC_VANBANDEN)
            {
                fileName = "Thống kê văn bản đến theo lĩnh vực.xlsx";
            }
            else if (reportType == LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LOAI_VANBANDEN)
            {
                fileName = "Thống kê văn bản đến theo loại.xlsx";
            }
            else
            {
                fileName = "Thống kê văn bản đến theo đơn vị gửi.xlsx";
            }

            ExportExcelSupplier<SelectListItem> helper = new ExportExcelSupplier<SelectListItem>();
            //thư mục chứa file kết quả
            helper.outputFolderPath = OUTPUT_EXECL_FOLDER_PATH;
            //đường dẫn file biểu mẫu
            helper.templateFilePath = Path.Combine(UPLOAD_PATH, TEMPLATE_REPORT_PATH);
            //tên file muốn kết xuất
            helper.fileName = fileName;
            //cột bắt đầu
            helper.startCell = 1;
            //dòng bắt đầu
            helper.startRow = 5;

            helper.propertyColumns = new List<string>() { "STT", "Text", "Value" };
            bool isWorkBookOpen = helper.OpenWorkBook();
            if (isWorkBookOpen)
            {
                int dataSize = data.groupOfReportResultItems.Count;
                //điền dữ liệu vào bảng
                helper.FillTableData(data.groupOfReportResultItems);

                //điền thông tin đặc thù vào bảng
                helper.workSheet.Range["A3", "C3"].Value = data.title;
                helper.workSheet.Range["A3", "C3"].Font.Bold = true;

                helper.workSheet.Range["A5", "A" + (5 + dataSize)].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                helper.workSheet.Range["A5", "A" + (5 + dataSize)].Cells.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                helper.workSheet.Range["A" + (5 + dataSize), "A" + (5 + dataSize)].Value = null;
                helper.workSheet.Range["A" + (5 + dataSize), "B" + (5 + dataSize)].Merge(Type.Missing);
                helper.workSheet.Range["A" + (5 + dataSize), "B" + (5 + dataSize)].Value = "Tổng";
                helper.workSheet.Range["A" + (5 + dataSize), "B" + (5 + dataSize)].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                helper.workSheet.Range["A" + (5 + dataSize), "B" + (5 + dataSize)].Cells.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                helper.workSheet.Range["C" + (5 + dataSize), "C" + (5 + dataSize)].Value = data.groupOfReportResultItems.Select(x => int.Parse(x.Value)).Sum();

                var borderRange = helper.workSheet.Range["A1", "C" + (5 + dataSize)];
                helper.SetBorderRange(borderRange);
                var centerRange = helper.workSheet.Range["C5", "C" + (5 + dataSize)];
                helper.SetTextCenterRange(centerRange);

                ExportExcelResult exportResult = helper.SaveAndCloseWorkBook();
                if (exportResult.exportSuccess)
                {
                    result.Message = "/Uploads/Output/Excel/" + exportResult.exportResultFileName;
                }
                else
                {
                    result.Status = false;
                    result.Message = exportResult.exportResultMessage;
                }
            }
            return Json(result);
        }
    }
}