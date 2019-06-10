using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.QLCHUYEN;
using CommonHelper;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.QL_CHUYENArea.Models;
using Web.Custom;
using Web.FwCore;
using Business.CommonModel.CONSTANT;
using CommonHelper.Word;
using System.Web.Configuration;
using System.IO;
using CommonHelper.DateExtend;
using Web.Filter;
using CommonHelper.Excel;
using Microsoft.Office.Interop.Excel;
using System.Drawing;

namespace Web.Areas.QL_CHUYENArea.Controllers
{
    public class QL_CHUYENController : BaseController
    {
        // GET: QL_CHUYENArea/QL_CHUYEN
        //đường dẫn thư mục chứa file đẩy lên server
        private string UPLOAD_PATH = WebConfigurationManager.AppSettings["FileUpload"];
        //đường dẫn mẫu báo cáo vận chuyển
        private string TEMPLATE_REPORT_PATH = WebConfigurationManager.AppSettings["BieuMauBaoCaoVanChuyen"];
        //đường dẫn thư mục lưu tạm file upload
        private string TEMPORARY_UPLOAD_FOLDER_PATH = WebConfigurationManager.AppSettings["TempUploadFolderPath"];
        //đường dẫn thư mục lưu file kết quả dạng word
        private string OUTPUT_WORD_FOLDER_PATH = WebConfigurationManager.AppSettings["TempUploadWordFolderPath"];

        //đường dẫn thư mục lưu file kết quả dạng excel
        private string OUTPUT_EXCEL_FOLDER_PATH = WebConfigurationManager.AppSettings["TempUploadExcelFolderPath"];

        private QL_DANGKYXE_LAIXEBusiness qlChuyenBusiness;
        private QL_DANGKY_XEBusiness qlDangKyXeBusiness;
        private QL_LAIXEBusiness qlLaiXeBusiness;
        private QL_XEBusiness qlXeBusiness;
        private DM_NGUOIDUNGBusiness dmNguoiDungBusiness;

        [ActionAudit]
        public ActionResult Index()
        {
            AssignUserInfo();
            qlChuyenBusiness = Get<QL_DANGKYXE_LAIXEBusiness>();
            qlLaiXeBusiness = Get<QL_LAIXEBusiness>();
            qlXeBusiness = Get<QL_XEBusiness>();

            ChuyenIndexViewModel viewModel = new ChuyenIndexViewModel();
            ChuyenSearchBO searchModel = new ChuyenSearchBO();
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            viewModel.listChuyens = qlChuyenBusiness.GetDataByPage(searchModel);
            viewModel.groupCars = qlXeBusiness.GetDropDownAvailableCars();
            viewModel.groupDrivers = qlLaiXeBusiness.GetDropDownAvailableDrivers();
            viewModel.currentUserId = currentUser.ID;
            SessionManager.SetValue("SearchChuyen", searchModel);
            return View(viewModel);
        }

        [ActionAudit]
        public JsonResult GetData(int pageIndex, string sortQuery, int pageSize)
        {
            AssignUserInfo();
            qlChuyenBusiness = Get<QL_DANGKYXE_LAIXEBusiness>();
            ChuyenSearchBO searchModel = (ChuyenSearchBO)SessionManager.GetValue("SearchChuyen");
            if (searchModel == null)
            {
                searchModel = new ChuyenSearchBO();
            }
            searchModel.sortQuery = sortQuery;
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            PageListResultBO<ChuyenBO> data = qlChuyenBusiness.GetDataByPage(searchModel);
            return Json(data);
        }

        [ActionAudit]
        [HttpPost]
        public JsonResult SearchData(FormCollection fc)
        {
            AssignUserInfo();
            qlChuyenBusiness = Get<QL_DANGKYXE_LAIXEBusiness>();
            var searchModel = (ChuyenSearchBO)SessionManager.GetValue("SearchChuyen");
            searchModel.TEN_CHUYEN = fc["TEN_CHUYEN"].Trim();
            searchModel.queryTimeStart = fc["queryTimeStart"].ToDateTime();
            searchModel.queryTimeEnd = fc["queryTimeEnd"].ToDateTime();
            searchModel.XE_ID = fc["XE_ID"].ToIntOrNULL();
            searchModel.LAIXE_ID = fc["LAIXE_ID"].ToIntOrNULL();
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            SessionManager.SetValue("SearchChuyen", searchModel);
            var result = qlChuyenBusiness.GetDataByPage(searchModel);
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @description: bắt đầu chuyến
        /// @since: 28/08/2018
        /// </summary>
        /// <param name="tripId"></param>
        /// <returns></returns>
        [ActionAudit]
        [HttpPost]
        public JsonResult StartTrip(long tripId)
        {
            JsonResultBO result = new JsonResultBO(false);
            qlChuyenBusiness = Get<QL_DANGKYXE_LAIXEBusiness>();
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            QL_DANGKYXE_LAIXE entity = qlChuyenBusiness.Find(tripId);

            if (entity != null)
            {
                entity.TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.DANGCHAY_ID;
                //cập nhật yêu cầu xe đang được thực thi
                QL_DANGKY_XE registration = qlDangKyXeBusiness.Find(entity.QL_DANGKY_XE_ID);
                if (registration != null)
                {
                    registration.TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DANG_THUCHIEN_ID;
                    qlDangKyXeBusiness.Save(registration);
                }
                qlChuyenBusiness.Save(entity);
                result.Message = "Đã bắt đầu chuyến";
                result.Status = true;
            }
            else
            {
                result.Message = "Chuyến không tồn tại";
            }
            return Json(result);
        }

        [ActionAudit]
        [HttpGet]
        public PartialViewResult ReturnTrip(long tripId)
        {
            qlChuyenBusiness = Get<QL_DANGKYXE_LAIXEBusiness>();
            ChuyenBO trip = qlChuyenBusiness.GetDetail(tripId);
            return PartialView("_ReturnTrip", trip);
        }

        /// <summary>
        /// @author: duynn
        /// @description: trả chuyến
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [ActionAudit]
        [HttpPost]
        public JsonResult ReturnTrip(FormCollection fc)
        {
            JsonResultBO result = new JsonResultBO(false);
            qlChuyenBusiness = Get<QL_DANGKYXE_LAIXEBusiness>();
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            long id = fc["ID"].ToLongOrZero();
            QL_DANGKYXE_LAIXE entity = qlChuyenBusiness.Find(id);
            if (entity != null)
            {
                entity.TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.DA_HOANTHANH_ID;
                entity.GHICHU = fc["GHICHU"].Trim();
                entity.NGAYSUA = DateTime.Now;
                //cập nhật quãng đường di chuyển
                if (!string.IsNullOrEmpty(fc["QUANGDUONG_DICHUYEN"]))
                {
                    entity.QUANGDUONG_DICHUYEN = fc["QUANGDUONG_DICHUYEN"].Replace(",", string.Empty).Replace(".00", string.Empty).ToIntOrZero();
                }
                //cập nhật chi phí
                if (!string.IsNullOrEmpty(fc["TONG_CHIPHI"]))
                {
                    entity.TONG_CHIPHI = fc["TONG_CHIPHI"].Replace(",", string.Empty).Replace(".00", string.Empty).ToIntOrZero();
                }
                qlChuyenBusiness.Save(entity);

                //cập nhật hoàn thành yêu cầu đăng ký xe
                QL_DANGKY_XE registration = qlDangKyXeBusiness.Find(entity.QL_DANGKY_XE_ID);
                if(registration != null)
                {
                    registration.TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_HOANTHANH_ID;
                    qlDangKyXeBusiness.Save(registration);
                }
                result.Message = "Đã trả chuyến thành công";
                result.Status = true;
            }
            else
            {
                result.Message = "Chuyến không tồn tại";
            }
            return Json(result);
        }


        /// <summary>
        /// @author: duynn
        /// @description: chi tiết chuyến
        /// </summary>
        /// <param name="tripId"></param>
        /// <returns></returns>
        [ActionAudit]
        [HttpGet]
        public PartialViewResult DetailTrip(long tripId)
        {
            qlChuyenBusiness = Get<QL_DANGKYXE_LAIXEBusiness>();
            ChuyenDetailViewModel viewModel = new ChuyenDetailViewModel();
            viewModel.tripEntity = qlChuyenBusiness.GetDetail(tripId);
            return PartialView("_DetailTrip", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: báo cáo sử dụng xe
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult ReportTrip()
        {
            AssignUserInfo();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            ReportTripViewModel viewModel = new ReportTripViewModel();
            viewModel.groupOfUsers = dmNguoiDungBusiness.GetListUserByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(), 0);
            return View(viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: tạo báo cáo chuyến
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [ActionAudit]
        [HttpPost]
        public ActionResult ReportTrip(FormCollection collection)
        {
            AssignUserInfo();
            qlChuyenBusiness = Get<QL_DANGKYXE_LAIXEBusiness>();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();

            ReportTripViewModel reportResult = new ReportTripViewModel();
            reportResult.reportEntity = new List<ChuyenReportBO>();

            int type = collection["queryType"].ToIntOrZero();
            string reportTitle = string.Empty;
            string timeRangeReportTitle = string.Empty;

            ChuyenSearchBO searchModel = new ChuyenSearchBO();
            List<long> userIds = new List<long>();
            if (collection["CANBO_ID"] != null)
            {
                userIds = collection["CANBO_ID"].ToListLong(',');
                searchModel.CANBO_IDs = userIds;
            }
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            if (type == LOAI_BAOCAO_THOIGIAN_CONSTANT.NGAY)
            {
                searchModel.queryTimeStart = collection["queryTimeStart"].ToDateTime();
                searchModel.queryTimeEnd = collection["queryTimeEnd"].ToDateTime();

                string dateStartStr = searchModel.queryTimeStart.Value.ToVietnameseDateFormat();
                string dateEndStr = searchModel.queryTimeEnd.Value.ToVietnameseDateFormat();

                reportResult.title = string.Format("Từ ngày {0} đến ngày {1}", dateStartStr, dateEndStr);
                reportTitle = string.Format("BÁO CÁO CÔNG TÁC TỪ NGÀY {0} ĐẾN NGÀY {1}", dateStartStr, dateEndStr);
                timeRangeReportTitle = string.Format("từ ngày {0} đến ngày {1}", dateStartStr, dateEndStr);
            }
            else if (type == LOAI_BAOCAO_THOIGIAN_CONSTANT.THANG)
            {
                int year = collection["queryYear"].ToIntOrZero();
                int month = collection["queryMonth"].ToIntOrZero();

                searchModel.queryTimeStart = new DateTime(year, month, 1);
                searchModel.queryTimeEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                reportResult.title = string.Format("Trong tháng {0} năm {1}", month, year);

                reportTitle = string.Format("BÁO CÁO CÔNG TÁC THÁNG {0}/{1}", month, year);
                timeRangeReportTitle = string.Format("trong tháng {0}/{1}", month, year);
            }
            else if (type == LOAI_BAOCAO_THOIGIAN_CONSTANT.NAM)
            {
                int year = collection["queryYear"].ToIntOrZero();
                searchModel.queryTimeStart = new DateTime(year, 1, 1);
                searchModel.queryTimeEnd = new DateTime(year, 12, 31);

                reportResult.title = string.Format("Trong năm {0}", year);
                reportTitle = string.Format("BÁO CÁO CÔNG TÁC NĂM {0}", year);
                timeRangeReportTitle = string.Format("trong năm {0}", year);
            }

            if(userIds != null && userIds.Count > 0)
            {
                string[] arrNames = new string[userIds.Count()];
                foreach(var userId in userIds)
                {
                    DM_NGUOIDUNG user = dmNguoiDungBusiness.Find(userId);
                    arrNames[userIds.IndexOf(userId)] = user.HOTEN;
                }
                timeRangeReportTitle = string.Format("{0} của cán bộ {1}", timeRangeReportTitle, string.Join(",", arrNames));
            }

            List<ChuyenReportBO> tripReportResult = qlChuyenBusiness.GetTripReportResult(searchModel);
            reportResult.title = reportTitle;
            reportResult.timeLine = timeRangeReportTitle.ToUpper();
            reportResult.reportEntity = tripReportResult;

            SessionManager.SetValue("ReportTripSession", reportResult);
            return PartialView("_ReportTripResult", reportResult);
        }

        /// <summary>
        /// @author: duynn
        /// @description: kết xuất báo cáo thống kê công tác vận chuyển
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [ActionAudit]
        [HttpPost]
        public JsonResult ExportReportTrip()
        {
            JsonResultBO result = new JsonResultBO(true);
            ReportTripViewModel reportResult = (ReportTripViewModel)SessionManager.GetValue("ReportTripSession");

            ExportExcelSupplier<SelectListItem> helper = new ExportExcelSupplier<SelectListItem>();
            //thư mục chứa file kết quả
            helper.outputFolderPath = OUTPUT_EXCEL_FOLDER_PATH;
            //đường dẫn file biểu mẫu
            helper.templateFilePath = Path.Combine(UPLOAD_PATH, TEMPLATE_REPORT_PATH);
            //dòng bắt đầu
            helper.startRow = 6;
            //tên file muốn kết xuất
            helper.fileName = "Báo cáo công tác vận chuyển.xlsx";

            int rowHeight = 50;
            var borderCellColor = Color.FromArgb(17, 138, 203);
            bool isWorkBookOpen = helper.OpenWorkBook();
            if (isWorkBookOpen)
            {
                var entities = reportResult.reportEntity;
                int currentRowIndex = helper.startRow;

                Range titleRange = helper.workSheet.Range["A4", "G4"];

                titleRange.Value = string.Format("({0})", reportResult.timeLine);
                titleRange.Font.Bold = true;
                titleRange.Font.Color = borderCellColor;
                foreach (var entity in entities)
                {
                    var trips = entity.groupOfTrips;

                    Range userRange = helper.workSheet.Range["A" + currentRowIndex, "H" + currentRowIndex];

                    userRange.Value = null;
                    userRange.Merge(Type.Missing);
                    userRange.Value = entity.TEN_CANBO;

                    userRange.Borders.LineStyle = XlLineStyle.xlContinuous;
                    userRange.Borders.Color = borderCellColor;
                    userRange.Font.Bold = true;
                    userRange.Interior.Color = Color.FromArgb(255, 234, 167);
                    userRange.RowHeight = rowHeight;
                    userRange.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                    userRange.VerticalAlignment = XlHAlign.xlHAlignCenter;

                    currentRowIndex++;

                    int tripIndex = 1;
                    foreach (var trip in trips)
                    {
                        helper.workSheet.Range["A" + currentRowIndex, "A" + currentRowIndex].Value = tripIndex.ToString();
                        helper.workSheet.Range["B" + currentRowIndex, "B" + currentRowIndex].Value = trip.TEN_CHUYEN;
                        if(trip.NGAY_XUATPHAT != null)
                        {
                            helper.workSheet.Range["C" + currentRowIndex, "C" + currentRowIndex].Value = string.Format("{0} lúc {1}h{2}", string.Format("{0:dd/MM/yyyy}", trip.NGAY_XUATPHAT.Value), trip.GIO_XUATPHAT.GetValueOrDefault().ToString("D2"), trip.PHUT_XUATPHAT.GetValueOrDefault().ToString("D2"));
                        }
                        helper.workSheet.Range["D" + currentRowIndex, "D" + currentRowIndex].Value = trip.DIEM_XUATPHAT;
                        helper.workSheet.Range["E" + currentRowIndex, "E" + currentRowIndex].Value = trip.DIEM_KETTHUC;
                        if (trip.NGAYSUA != null)
                        {
                            helper.workSheet.Range["F" + currentRowIndex, "F" + currentRowIndex].Value = string.Format("{0} lúc {1}h{2}", string.Format("{0:dd/MM/yyyy}", trip.NGAYSUA.Value), trip.NGAYSUA.Value.Hour.ToString("D2"), trip.NGAYSUA.Value.Minute.ToString("D2"));
                        }
                        helper.workSheet.Range["G" + currentRowIndex, "G" + currentRowIndex].Value = trip.QUANGDUONG_DICHUYEN.GetValueOrDefault().ToString("#,#0");
                        helper.workSheet.Range["H" + currentRowIndex, "H" + currentRowIndex].Value = trip.TONG_CHIPHI.GetValueOrDefault().ToString("#,#0");
                        var dataRange = helper.workSheet.Range["A" + currentRowIndex, "H" + currentRowIndex];
                        dataRange.Borders.LineStyle = XlLineStyle.xlContinuous;
                        dataRange.Borders.Color = borderCellColor;
                        dataRange.RowHeight = rowHeight;
                        dataRange.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                        dataRange.VerticalAlignment = XlHAlign.xlHAlignCenter;
                        dataRange.WrapText = true;

                        tripIndex++;
                        currentRowIndex++;
                    }
                }

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