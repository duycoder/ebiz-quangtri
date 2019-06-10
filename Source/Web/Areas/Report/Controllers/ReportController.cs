using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Custom;
using Model.Entities;
using Business.Business;
using Web.Areas.BaoCaoThongKe.Models;
using CommonHelper;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
//using Web.Areas.QuanLyCongViec.Models;
using Web.Areas.Report.Models;
using Web.FwCore;
using System.IO;
using System.Web.Hosting;

namespace Web.Areas.Report.Controllers
{
    public class ReportController : BaseController
    {
        // GET: Report/Report
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;

        private HSCV_CONGVIECBusiness HSCV_CONGVIECBusiness;
        private HSCV_TRINHDUYETCONGVIECBusiness HSCV_TRINHDUYETCONGVIECBusiness;
        private PHIEUDANHGIACONGVIECBusiness PHIEUDANHGIACONGVIECBusiness;
        private HSCV_CONGVIEC_LAPKEHOACHBusiness HSCV_CONGVIEC_LAPKEHOACHBusiness;
        private HSCV_CONGVIEC_XINLUIHANBusiness HSCV_CONGVIEC_XINLUIHANBusiness;
        private HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private List<ReportExcelModel> ListReportModel;
        private int currentJobStt { get; set; }
        /// <summary>
        /// Function theo dõi công việc dành cho ban lãnh đạo
        /// tách biệt các cấp ra cho dễ viết
        /// </summary>
        /// <returns></returns>
        public ActionResult TheoDoiCongViec()
        {
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            AssignUserInfo();
            // Chỉ lấy các khối đơn vị


            ReportViewModel model = new ReportViewModel();
            if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.DONVI))
            {
                List<SelectListItem> LstCoCauToChuc = CCTC_THANHPHANBusiness.repository.All().Where(x => x.PARENT_ID == currentUser.DeptParentID && x.ID != currentUser.DM_PHONGBAN_ID).Select(
                x => new SelectListItem()
                {
                    Text = x.NAME,
                    Value = x.ID.ToString(),
                }).ToList();
                model.LstCoCauToChuc = LstCoCauToChuc;
                model.LstDonVi = new List<SelectListItem>();
                model.HasRoleAssignUnit = true;
            }
            else if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.PHONGBAN))
            {
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID);
                if (DonVi != null)
                {
                    SelectListItem selectListItem = new SelectListItem();
                    selectListItem.Text = DonVi.NAME;
                    selectListItem.Value = DonVi.ID.ToString();
                    selectListItem.Selected = true;
                    List<SelectListItem> selectListItems = new List<SelectListItem>();
                    selectListItems.Add(selectListItem);
                    model.LstCoCauToChuc = selectListItems;
                    model.LstDonVi = CCTC_THANHPHANBusiness.GetDataByParent(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0, 0);
                }
                else
                {
                    model.LstCoCauToChuc = new List<SelectListItem>();
                    model.LstDonVi = new List<SelectListItem>();
                }
                model.HasRoleAssignDepartment = true;
            }
            else if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.CANHAN))
            {
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID);
                if (DonVi != null)
                {
                    DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                    SelectListItem selectListItem = new SelectListItem();
                    selectListItem.Text = DonVi.NAME;
                    selectListItem.Value = DonVi.ID.ToString();
                    selectListItem.Selected = true;
                    List<SelectListItem> selectListItems = new List<SelectListItem>();
                    selectListItems.Add(selectListItem);
                    model.LstDonVi = selectListItems;
                    model.LstNhanVien = DM_NGUOIDUNGBusiness.GetDsNguoiDung(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                }
                else
                {
                    model.LstDonVi = new List<SelectListItem>();
                }
                model.HasRoleAssignChuyenVien = true;
            }
            return View(model);
        }

        public PartialViewResult ChangeKhoiDonVi(int? KHOIDONVI_ID)
        {
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            List<SelectListItem> LstCoCauToChuc = CCTC_THANHPHANBusiness.repository.All().Where(x => x.PARENT_ID == KHOIDONVI_ID).Select(
                x => new SelectListItem()
                {
                    Text = x.NAME,
                    Value = x.ID.ToString(),
                }).ToList();
            return PartialView("_ListDonVi", LstCoCauToChuc);
        }

        public PartialViewResult ResultTheoDoiCongViec(string FROM_QUERY_DATE, string TO_QUERY_DATE, string DONVI_ID, string NHANVIEN_ID)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_CONGVIEC_LAPKEHOACHBusiness = Get<HSCV_CONGVIEC_LAPKEHOACHBusiness>();
            HSCV_TRINHDUYETCONGVIECBusiness = Get<HSCV_TRINHDUYETCONGVIECBusiness>();
            HSCV_CONGVIEC_XINLUIHANBusiness = Get<HSCV_CONGVIEC_XINLUIHANBusiness>();
            PHIEUDANHGIACONGVIECBusiness = Get<PHIEUDANHGIACONGVIECBusiness>();
            HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness = Get<HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            DateTime date1 = FROM_QUERY_DATE.ToDateTime().Value;
            DateTime date2 = TO_QUERY_DATE.ToDateTime().Value;
            var LstDeptId = DONVI_ID.ToListInt(',');
            List<DM_NGUOIDUNG> ListNguoiDung = DM_NGUOIDUNGBusiness.GetDataByUnitId(LstDeptId);
            
            var LstCongViec = HSCV_CONGVIECBusiness.GetDsCongViec(LstDeptId, ListNguoiDung.Select(x => x.ID).ToList(), date1, date2);
            // Công việc gốc
            var LstCongViecRoot = LstCongViec.Where(x => x.IS_SUBTASK == null).ToList();
                        
            List<long> ListJobId = LstCongViec.Select(x => x.ID).ToList();
            // Add thêm các công việc con được giao xuống nhưng có công việc cha ko nằm trong danh sách công việc hiện tại của phòng ban, danh sách người dùng
            foreach(var job in LstCongViec)
            {
                if (!ListJobId.Contains(job.ParentId) && !LstCongViecRoot.Contains(job))
                {
                    LstCongViecRoot.Add(job);
                }
            }
            var ListThamGia = HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness.GetData(ListJobId);
            var ListPlan = HSCV_CONGVIEC_LAPKEHOACHBusiness.GetData(ListJobId);
            ListNguoiDung = DM_NGUOIDUNGBusiness.GetData(ListThamGia.Select(x => x.USER_ID).ToList());
            var html = "";
            var idx = 1;
            int sizePlan = 0;
            int size = 0;
            string thamgia = "";
            ListReportModel = new List<ReportExcelModel>();
            foreach (var item in LstCongViecRoot)
            {
                currentJobStt = idx;
                InitRow(LstCongViec, ListThamGia, ListPlan, ListNguoiDung, ref html, ref idx,
                    out sizePlan, out size, ref thamgia, item, "", true);
            }
            SessionManager.SetValue("ListReportModel", ListReportModel);
            return PartialView("ResultTheoDoiCongViec", html);
        }
        private void InitRow(List<CongViecBO> LstCongViec, List<HSCV_CONGVIEC_NGUOITHAMGIAXULY> ListThamGia,
            List<HSCV_CONGVIEC_LAPKEHOACH> ListPlan, List<DM_NGUOIDUNG> ListNguoiDung, ref string html,
            ref int rootIdx, out int sizePlan, out int size, ref string thamgia,
            CongViecBO item, string idx, bool isRoot)
        {
            int chenhlech = 0;
            var LstTrinhDuyet = HSCV_TRINHDUYETCONGVIECBusiness.repository.All().Where(x => x.CONGVIEC_ID == item.ID).ToArray();
            var LstLuiHan = HSCV_CONGVIEC_XINLUIHANBusiness.repository.All().Where(x => x.CONGVIEC_ID == item.ID).Count();
            var DanhGia = PHIEUDANHGIACONGVIECBusiness.repository.All().Where(x => x.CONGVIEC_ID == item.ID).FirstOrDefault();
            var ListNguoiDungTemp = ListNguoiDung.Where(x => ListThamGia.Select(y => y.CONGVIEC_ID).Contains(item.ID)).ToList();
            var ListPlanTemp = ListPlan.Where(x => item.ID == x.CONGVIEC_ID).ToList();
            ReportExcelModel reportExcel = new ReportExcelModel();
            string level = "level-";
            if (string.IsNullOrEmpty(idx))
            {
                level = "level-1";
            }
            else
            {
                level = "level-" + (currentJobStt + idx.ToString()).ToListInt('.').Count;
            }
            html += "<tr " + (isRoot ? "class='job-parent " + level + "'" : "class='" + level + "'") + ">";
            //STT
            reportExcel.Stt = (string.IsNullOrEmpty(idx) ? rootIdx.ToString() : (currentJobStt + idx.ToString()));
            html += "<td class='first-child'><strong>" + (string.IsNullOrEmpty(idx) ? rootIdx.ToString() : (currentJobStt + idx.ToString())) + "</strong></td>";
            //Hạng mục công việc
            reportExcel.JobName = item.TENCONGVIEC;
            html += "<td><div title='" + item.TENCONGVIEC + "'>" + (!string.IsNullOrEmpty(item.TENCONGVIEC) && item.TENCONGVIEC.Trim().Length > 150 ? (item.TENCONGVIEC.Trim().Substring(0, 150) + "...") : item.TENCONGVIEC) + "</div></td>";
            //Nội dung chi tiết
            string noidung = HttpUtility.HtmlDecode(item.NOIDUNGCONGVIEC.RemoveHtml());
            reportExcel.JobContent = noidung;
            html += "<td><div title='" + noidung + "' >" + (!string.IsNullOrEmpty(noidung) && noidung.Trim().Length > 150 ? (noidung.Substring(0, 150) + "...") : noidung) + "</div></td>";
            //Các bước thực hiện
            string step = HttpUtility.HtmlDecode(item.CACBUOC_THUCHIEN.RemoveHtml());
            reportExcel.JobStep = step;
            html += "<td><div title='" + step + "' >" + (!string.IsNullOrEmpty(step) && step.Trim().Length > 150 ? (step.Substring(0, 150) + "...") : step) + "</div></td>";
            //Mục tiêu công việc
            string muctieu = HttpUtility.HtmlDecode(item.MUCTIEU_CONGVIEC.RemoveHtml());
            reportExcel.Purpose = muctieu;
            html += "<td><div title='" + muctieu + "'>" + (!string.IsNullOrEmpty(muctieu) && muctieu.Trim().Length > 150 ? (muctieu.Substring(0, 150) + "...") : muctieu) + "</div></td>";
            //Người xử lý chính
            reportExcel.MainUser = item.TEN_NGUOIXULYCHINH;
            html += "<td>" + item.TEN_NGUOIXULYCHINH + "</td>";
            size = ListNguoiDungTemp.Count;
            thamgia = "";
            for (int i = 0; i < size; i++)
            {
                if (i == size - 1)
                {
                    thamgia += ListNguoiDungTemp[i].HOTEN;
                }
                else
                {
                    thamgia += ListNguoiDungTemp[i].HOTEN + ", ";
                }
            }
            //Người hỗ trợ
            reportExcel.JoinUser = thamgia;
            html += "<td>" + thamgia + "</td>";
            //Ngày nhận việc
            reportExcel.ReceiveDate = (item.NGAY_NHANVIEC.HasValue ? item.NGAY_NHANVIEC.Value.ToString("dd/MM/yyyy") : "");
            html += "<td>" + (item.NGAY_NHANVIEC.HasValue ? item.NGAY_NHANVIEC.Value.ToString("dd/MM/yyyy") : "") + "</td>";
            //Ngày hoàn thành
            reportExcel.DateLine = (item.NGAYHOANTHANH_THEOMONGMUON.HasValue ? item.NGAYHOANTHANH_THEOMONGMUON.Value.ToString("dd/MM/yyyy") : "");
            html += "<td>" + (item.NGAYHOANTHANH_THEOMONGMUON.HasValue ? item.NGAYHOANTHANH_THEOMONGMUON.Value.ToString("dd/MM/yyyy") : "") + "</td>";
            var TongSoNgayChoDuyet = 0;
            sizePlan = ListPlanTemp.Count;
            #region Kế hoạch công việc
            for (var i = 0; i < 3; i++)
            {
                if (i < sizePlan)
                {
                    string Browser = (ListPlanTemp[i].CREATED_AT.HasValue ? ListPlanTemp[i].CREATED_AT.Value.ToString("dd/MM/yyyy") : "");
                    string FeedBack = ListPlanTemp[i].NGAYCAPTRENPHANHOI != null ? ListPlanTemp[i].NGAYCAPTRENPHANHOI.Value.ToString("dd/MM/yyyy") : "";
                    string WaitingResponse = ListPlanTemp[i].SONGAYCHOPHANHOI.HasValue ? ListPlanTemp[i].SONGAYCHOPHANHOI.Value.ToString() : "";
                    string Result = ListPlanTemp[i].KETQUATRINHDUYET;
                    switch (i)
                    {
                        case 0:
                            reportExcel.FirstReview = Browser;
                            reportExcel.FirstFeedback = FeedBack;
                            reportExcel.FirstWaitingResponse = WaitingResponse;
                            reportExcel.SecondResult = Result;
                            break;
                        case 1:
                            reportExcel.SecondReview = Browser;
                            reportExcel.SecondFeedback = FeedBack;
                            reportExcel.SecondWaitingResponse = WaitingResponse;
                            reportExcel.FirstResult = Result;
                            break;
                        case 2:
                            reportExcel.ThirdReview = Browser;
                            reportExcel.ThirdFeedback = FeedBack;
                            reportExcel.ThirdWaitingResponse = WaitingResponse;
                            reportExcel.ThirdResult = Result;
                            break;
                    }
                    html += "<td>" + Browser + "</td>";
                    html += "<td>" + FeedBack + "</td>";
                    html += "<td>" + WaitingResponse + "</td>";
                    html += "<td>" + Result + "</td>";
                    TongSoNgayChoDuyet += ListPlanTemp[i].SONGAYCHOPHANHOI.HasValue ? ListPlanTemp[i].SONGAYCHOPHANHOI.Value : 0;
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            reportExcel.FirstReview = "";
                            reportExcel.FirstFeedback = "";
                            reportExcel.FirstWaitingResponse = "";
                            reportExcel.FirstResult = "";
                            break;
                        case 1:
                            reportExcel.SecondReview = "";
                            reportExcel.SecondFeedback = "";
                            reportExcel.SecondWaitingResponse = "";
                            reportExcel.SecondResult = "";
                            break;
                        case 2:
                            reportExcel.ThirdReview = "";
                            reportExcel.ThirdFeedback = "";
                            reportExcel.ThirdWaitingResponse = "";
                            reportExcel.ThirdResult = "";
                            break;
                    }
                    html += "<td></td>";
                    html += "<td></td>";
                    html += "<td></td>";
                    html += "<td></td>";
                }
            }
            #endregion
            //Ngày duyệt kế hoạch
            string ApprovalDate = (item.NGAYDUYET.HasValue ? item.NGAYDUYET.Value.ToString("dd/MM/yyyy") : "");
            reportExcel.ApprovalDate = ApprovalDate;
            html += "<td>" + ApprovalDate + "</td>";
            //Ngày bắt đầu KH
            string StartDate = (item.NGAYBATDAU_KEHOACH.HasValue ? item.NGAYBATDAU_KEHOACH.Value.ToString("dd/MM/yyyy") : "");
            reportExcel.StartDate = StartDate;
            html += "<td>" + StartDate + "</td>";
            //Ngày hoàn thành KH
            string CompleteDate = (item.NGAYKETTHUC_KEHOACH.HasValue ? item.NGAYKETTHUC_KEHOACH.Value.ToString("dd/MM/yyyy") : "");
            reportExcel.CompleteDate = CompleteDate;
            html += "<td>" + CompleteDate + "</td>";
            //Thời gian triển khai theo KH
            if (item.NGAYBATDAU_KEHOACH.HasValue && item.NGAYKETTHUC_KEHOACH.HasValue)
            {
                chenhlech = (int)(item.NGAYKETTHUC_KEHOACH.Value - item.NGAYBATDAU_KEHOACH.Value).TotalDays;
                html += "<td>" + chenhlech + "</td>";
            }
            else
            {
                html += "<td>0</td>";
            }
            reportExcel.DeployDay = chenhlech.ToString();
            //Ngày bắt đầu
            string DeployDate = (item.NGAYBATDAU_THUCTE.HasValue ? item.NGAYBATDAU_THUCTE.Value.ToString("dd/MM/yyyy") : "");
            reportExcel.DeployDate = DeployDate;
            html += "<td>" + DeployDate + "</td>";
            size = LstTrinhDuyet.Count();
            #region Thời gian triển khai thực tế
            for (var i = 0; i < 3; i++)
            {
                if (i < size)
                {
                    string Browser = (LstTrinhDuyet[i].CREATED_AT.HasValue ? LstTrinhDuyet[i].CREATED_AT.Value.ToString("dd/MM/yyyy") : "");
                    string FeedbackBrowser = LstTrinhDuyet[i].NGAYPHANHOI != null ? LstTrinhDuyet[i].NGAYPHANHOI.Value.ToString("dd/MM/yyyy") : "";
                    string WaitingBrowser = LstTrinhDuyet[i].SONGAYCHOPHANHOI.HasValue ? LstTrinhDuyet[i].SONGAYCHOPHANHOI.Value.ToString() : "";
                    string ResultBrowser = LstTrinhDuyet[i].KETQUATRINHDUYET;
                    switch (i)
                    {
                        case 0:
                            reportExcel.FirstBrowser = Browser;
                            reportExcel.FirstFeedbackBrowser = FeedbackBrowser;
                            reportExcel.FirstWaitingBrowser = WaitingBrowser;
                            reportExcel.FirstResultBrowser = ResultBrowser;
                            break;
                        case 1:
                            reportExcel.SecondReview = Browser;
                            reportExcel.SecondFeedback = FeedbackBrowser;
                            reportExcel.SecondWaitingResponse = WaitingBrowser;
                            reportExcel.FirstResult = ResultBrowser;
                            break;
                        case 2:
                            reportExcel.ThirdReview = Browser;
                            reportExcel.ThirdFeedback = FeedbackBrowser;
                            reportExcel.ThirdWaitingResponse = WaitingBrowser;
                            reportExcel.ThirdResult = ResultBrowser;
                            break;
                    }
                    TongSoNgayChoDuyet = TongSoNgayChoDuyet + LstTrinhDuyet[i].SONGAYCHOPHANHOI.Value;
                    html += "<td>" + (LstTrinhDuyet[i].CREATED_AT.HasValue ? LstTrinhDuyet[i].CREATED_AT.Value.ToString("dd/MM/yyyy") : "") + "</td>";
                    html += "<td>" + LstTrinhDuyet[i].NGAYPHANHOI + "</td>";
                    html += "<td>" + LstTrinhDuyet[i].SONGAYCHOPHANHOI + "</td>";
                    html += "<td>" + LstTrinhDuyet[i].KETQUATRINHDUYET + "</td>";
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            reportExcel.FirstBrowser = "";
                            reportExcel.FirstFeedbackBrowser = "";
                            reportExcel.FirstWaitingBrowser = "";
                            reportExcel.FirstResultBrowser = "";
                            break;
                        case 1:
                            reportExcel.SecondReview = "";
                            reportExcel.SecondFeedback = "";
                            reportExcel.SecondWaitingResponse = "";
                            reportExcel.FirstResult = "";
                            break;
                        case 2:
                            reportExcel.ThirdReview = "";
                            reportExcel.ThirdFeedback = "";
                            reportExcel.ThirdWaitingResponse = "";
                            reportExcel.ThirdResult = "";
                            break;
                    }
                    html += "<td></td>";
                    html += "<td></td>";
                    html += "<td></td>";
                    html += "<td></td>";
                }
            }
            #endregion
            // Ngày hoàn thành và được phê duyệt
            string ApprovedDate = (item.NGAYKETTHUC_THUCTE.HasValue ? item.NGAYKETTHUC_THUCTE.Value.ToString("dd/MM/yyyy") : "");
            html += "<td>" + ApprovedDate + "</td>";
            #region Số ngày triển khai theo thực tế
            var SoNgayTrienKhai = 0;
            if (item.NGAYKETTHUC_THUCTE.HasValue)
            {

                SoNgayTrienKhai = (int)(item.NGAYKETTHUC_THUCTE.Value - item.NGAYBATDAU_THUCTE.Value).TotalDays;
                SoNgayTrienKhai = SoNgayTrienKhai > 0 ? SoNgayTrienKhai : 0;
                html += "<td>" + SoNgayTrienKhai.ToString() + "</td>";
            }
            else
            {
                html += "<td>0</td>";
            }
            reportExcel.ImplementedReality = SoNgayTrienKhai.ToString();
            #endregion
            //Số ngày dành cho công việc
            int TotalDays = 0;
            if (sizePlan > 0 && ListPlanTemp[0].NGAYTRINHKEHOACH.HasValue && item.NGAYKETTHUC_THUCTE.HasValue)
            {
                TotalDays = (int)(item.NGAYKETTHUC_THUCTE.Value - ListPlanTemp[0].NGAYTRINHKEHOACH.Value).TotalDays;

                html += "<td>" + TotalDays.ToString() + "</td>";
            }
            else
            {
                html += "<td>0</td>";
            }
            reportExcel.TotalDays = TotalDays.ToString();
            //SỐ ngày chờ cấp trên duyệt
            reportExcel.TotalWaitingApproval = TongSoNgayChoDuyet.ToString();
            html += "<td>" + TongSoNgayChoDuyet.ToString() + "</td>";
            //Số ngày triển khai thực tế
            int chenhlechthucte = 0;
            if (item.NGAYBATDAU_THUCTE.HasValue && item.NGAYKETTHUC_THUCTE.HasValue)
            {
                chenhlechthucte = Math.Abs((int)(item.NGAYKETTHUC_THUCTE.Value - item.NGAYBATDAU_THUCTE.Value).TotalDays);
                html += "<td>" + chenhlechthucte + "</td>";
            }
            else
            {
                html += "<td>0</td>";
            }
            reportExcel.TotalDeployed = chenhlechthucte.ToString();
            //Số lần trình duyệt
            reportExcel.TotalBrowsers = (size + sizePlan).ToString();
            html += "<td>" + (size + sizePlan) + "</td>";
            //Số lần điều chỉnh
            reportExcel.TotalEdit = LstLuiHan.ToString();
            html += "<td>" + LstLuiHan.ToString() + "</td>";
            //Chênh lệch
            reportExcel.TotalDifferences = Math.Abs((chenhlech - chenhlechthucte)).ToString();
            html += "<td>" + Math.Abs((chenhlech - chenhlechthucte)) + "</td>";
            if (DanhGia != null)
            {
                reportExcel.Score = DanhGia.TONGDIEM.HasValue ? DanhGia.TONGDIEM.Value.ToString() : "0";
                reportExcel.Rank = DanhGia.XEPLOAI;
                html += "<td>" + DanhGia.TONGDIEM + "</td>";
                html += "<td>" + DanhGia.XEPLOAI + "</td>";
            }
            else
            {
                reportExcel.Score = "0";
                reportExcel.Rank = "";
                html += "<td></td>";
                html += "<td></td>";
            }
            html += "</tr>";
            ListReportModel.Add(reportExcel);
            html += RecusiveCongViec(item.ID, LstCongViec, idx.ToString(), ListNguoiDung, ListThamGia, ListPlan);
            rootIdx = rootIdx + 1;
        }

        public string RecusiveCongViec(long congviecid, List<CongViecBO> LstCongViec, string rootIdx,
            List<DM_NGUOIDUNG> ListNguoiDung, List<HSCV_CONGVIEC_NGUOITHAMGIAXULY> ListThamGia,
            List<HSCV_CONGVIEC_LAPKEHOACH> ListPlan)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_TRINHDUYETCONGVIECBusiness = Get<HSCV_TRINHDUYETCONGVIECBusiness>();
            HSCV_CONGVIEC_XINLUIHANBusiness = Get<HSCV_CONGVIEC_XINLUIHANBusiness>();
            PHIEUDANHGIACONGVIECBusiness = Get<PHIEUDANHGIACONGVIECBusiness>();

            var LstCongViecRoot = LstCongViec.Where(x => x.CONGVIECGOC_ID == congviecid).ToList();
            var html = "";
            string thamgia = "";
            var idx = 1;
            int size = 0;
            int sizePlan = 0;
            if (LstCongViecRoot.Count > 0)
            {
                foreach (var item in LstCongViecRoot)
                {
                    InitRow(LstCongViec, ListThamGia, ListPlan, ListNguoiDung, ref html, ref idx,
                        out sizePlan, out size, ref thamgia, item, rootIdx + "." + idx.ToString(), false);
                }
            }
            return html;
        }
        public JsonResult ExportCongViec()
        {
            List<ReportExcelModel> ListReportModel = SessionManager.GetValue("ListReportModel") as List<ReportExcelModel>;
            if (ListReportModel == null)
            {
                ResponseExport result1 = new ResponseExport(false);
                result1.Message = "Bạn cần xem trước báo cáo trước khi kết xuất file";
                return Json(result1);

            }
            ExportExcelHelper<ReportExcelModel> exPro = new ExportExcelHelper<ReportExcelModel>();
            exPro.PathStore = Path.Combine(HostingEnvironment.MapPath("/Uploads"), "ErrorExport");
            exPro.PathTemplate = Path.Combine(HostingEnvironment.MapPath("/Uploads"), @"TemplateExcel\KeHoachCongViec.xlsx");
            exPro.StartRow = 15;
            exPro.StartCol = 1;
            exPro.FileName = "Theo doi ke hoach - Phieu giao viec";
            List<List<string>> lstData = new List<List<string>>();
            foreach (var item in ListReportModel)
            {
                List<string> temp = new List<string>();
                temp.Add(item.Stt);
                temp.Add(item.JobName);
                temp.Add(item.JobContent);
                temp.Add(item.JobStep);
                temp.Add(item.Purpose);
                temp.Add(item.MainUser);
                temp.Add(item.JoinUser);
                temp.Add(item.ReceiveDate);
                temp.Add(item.DateLine);
                temp.Add(item.FirstReview);
                temp.Add(item.FirstFeedback);
                temp.Add(item.FirstWaitingResponse);
                temp.Add(item.FirstResult);
                temp.Add(item.SecondReview);
                temp.Add(item.SecondFeedback);
                temp.Add(item.SecondWaitingResponse);
                temp.Add(item.SecondResult);
                temp.Add(item.ThirdReview);
                temp.Add(item.ThirdFeedback);
                temp.Add(item.ThirdWaitingResponse);
                temp.Add(item.ThirdResult);
                temp.Add(item.ApprovalDate);
                temp.Add(item.StartDate);
                temp.Add(item.CompleteDate);
                temp.Add(item.DeployDay);
                temp.Add(item.DeployDate);
                temp.Add(item.FirstBrowser);
                temp.Add(item.FirstFeedbackBrowser);
                temp.Add(item.FirstWaitingBrowser);
                temp.Add(item.FirstResultBrowser);
                temp.Add(item.SecondBrowser);
                temp.Add(item.SecondFeedbackBrowser);
                temp.Add(item.SecondWaitingBrowser);
                temp.Add(item.SecondResultBrowser);
                temp.Add(item.ThirdBrowser);
                temp.Add(item.ThirdFeedbackBrowser);
                temp.Add(item.ThirdWaitingBrowser);
                temp.Add(item.ThirdResultBrowser);
                temp.Add(item.ApprovedDate);
                temp.Add(item.ImplementedReality);
                temp.Add(item.TotalDays);
                temp.Add(item.TotalWaitingApproval);
                temp.Add(item.TotalDeployed);
                temp.Add(item.TotalBrowsers);
                temp.Add(item.TotalEdit);
                temp.Add(item.TotalDifferences);
                temp.Add(item.Score);
                temp.Add(item.Rank);
                lstData.Add(temp);
            }
            var result = exPro.ExportText(lstData);
            if (result.Status)
            {
                result.PathStore = Path.Combine(@"/Uploads/ErrorExport", result.FileName);
            }
            return Json(result);
        }
        public ActionResult DanhGiaChatLuong()
        {
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            AssignUserInfo();
            // Chỉ lấy các khối đơn vị
            ReportViewModel model = new ReportViewModel();
            if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.DONVI))
            {
                List<SelectListItem> LstCoCauToChuc = CCTC_THANHPHANBusiness.repository.All().Where(x => x.PARENT_ID == currentUser.DeptParentID && x.ID != currentUser.DM_PHONGBAN_ID).Select(
                x => new SelectListItem()
                {
                    Text = x.NAME,
                    Value = x.ID.ToString(),
                }).ToList();
                model.LstCoCauToChuc = LstCoCauToChuc;
                model.LstDonVi = new List<SelectListItem>();
                model.HasRoleAssignUnit = true;
                model.LstNhanVien = new List<SelectListItem>();
            }
            else if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.PHONGBAN))
            {
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID);
                if (DonVi != null)
                {
                    SelectListItem selectListItem = new SelectListItem();
                    selectListItem.Text = DonVi.NAME;
                    selectListItem.Value = DonVi.ID.ToString();
                    selectListItem.Selected = true;
                    List<SelectListItem> selectListItems = new List<SelectListItem>();
                    selectListItems.Add(selectListItem);
                    model.LstCoCauToChuc = selectListItems;
                    model.LstDonVi = CCTC_THANHPHANBusiness.GetDataByParent(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0, 0);
                }
                else
                {
                    model.LstCoCauToChuc = new List<SelectListItem>();
                    model.LstDonVi = new List<SelectListItem>();
                }
                model.HasRoleAssignDepartment = true;
                model.LstNhanVien = new List<SelectListItem>();
                if(model.LstDonVi.Count == 0 && currentUser.DM_PHONGBAN_ID.HasValue)
                {
                    List<int> LstTmpDonVi = new List<int>();
                    LstTmpDonVi.Add(currentUser.DM_PHONGBAN_ID.Value);
                    model.LstNhanVien = DM_NGUOIDUNGBusiness.GetByPhongBan(LstTmpDonVi, 0, new List<int>())
                .Select(x => new SelectListItem
                {
                    Text = x.HOTEN,
                    Value = x.ID.ToString()
                }).ToList();
                }
            }
            else if (IsInActivities(currentUser.ListThaoTac, PermissionVanbanModel.CANHAN))
            {
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(currentUser.DM_PHONGBAN_ID);
                if (DonVi != null)
                {
                    SelectListItem selectListItem = new SelectListItem();
                    selectListItem.Text = DonVi.NAME;
                    selectListItem.Value = DonVi.ID.ToString();
                    selectListItem.Selected = true;
                    List<SelectListItem> selectListItems = new List<SelectListItem>();
                    selectListItems.Add(selectListItem);
                    model.LstDonVi = selectListItems;
                    model.LstNhanVien = DM_NGUOIDUNGBusiness.GetDsNguoiDung(currentUser.DM_PHONGBAN_ID.HasValue ? currentUser.DM_PHONGBAN_ID.Value : 0);
                }
                else
                {
                    model.LstDonVi = new List<SelectListItem>();
                }
                model.HasRoleAssignChuyenVien = true;
            }
            return View(model);
        }
        public PartialViewResult ResultDanhGiaCongViec(string FROM_QUERY_DATE, string TO_QUERY_DATE, string DONVI_ID, string NHANVIEN_ID)
        {
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            AssignUserInfo();
            DateTime date1 = FROM_QUERY_DATE.ToDateTime().Value;
            DateTime date2 = TO_QUERY_DATE.ToDateTime().Value;
            var LstDeptId = new List<int>();
            if (!string.IsNullOrEmpty(DONVI_ID))
            {
                LstDeptId = DONVI_ID.ToListInt(',');
            }

            if (!string.IsNullOrEmpty(NHANVIEN_ID) && NHANVIEN_ID != "null")
            {
                var LstCongViec = HSCV_CONGVIECBusiness.GetDanhGiaCongViec(LstDeptId, NHANVIEN_ID.ToListLong(','), date1, date2);
                return PartialView("_SearchResult", LstCongViec);
            }
            else
            {
                if (currentUser.DM_PHONGBAN_ID.HasValue)
                {
                    LstDeptId.Add(currentUser.DM_PHONGBAN_ID.Value);
                }
                
                List<DM_NGUOIDUNG> ListNguoiDung = DM_NGUOIDUNGBusiness.GetDataByUnitId(LstDeptId);
                var LstCongViec = HSCV_CONGVIECBusiness.GetDanhGiaCongViec(LstDeptId, ListNguoiDung.Select(x => x.ID).ToList(), date1, date2);
                return PartialView("_SearchResult", LstCongViec);
            }

        }
        public PartialViewResult DetailDanhGia(long id)
        {
            DanhGiaCongViecModel model = new DanhGiaCongViecModel();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            CongViecBO CongViec = HSCV_CONGVIECBusiness.FindById(id);
            if (CongViec != null)
            {
                model.CongViec = CongViec;
                PHIEUDANHGIACONGVIECBusiness = Get<PHIEUDANHGIACONGVIECBusiness>();
                model.PhieuDanhGia = PHIEUDANHGIACONGVIECBusiness.GetData(id);
            }
            else
            {
                model.CongViec = new CongViecBO();
                model.PhieuDanhGia = new PHIEUDANHGIACONGVIEC();
            }
            return PartialView("_Detail", model);
        }
        public ActionResult ExportData(long id)
        {
            DanhGiaCongViecModel model = new DanhGiaCongViecModel();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            CongViecBO CongViec = HSCV_CONGVIECBusiness.FindById(id);
            if (CongViec != null)
            {
                model.CongViec = CongViec;
                PHIEUDANHGIACONGVIECBusiness = Get<PHIEUDANHGIACONGVIECBusiness>();
                model.PhieuDanhGia = PHIEUDANHGIACONGVIECBusiness.GetData(id);
            }
            else
            {
                model.CongViec = new CongViecBO();
                model.PhieuDanhGia = new PHIEUDANHGIACONGVIEC();
            }
            return View("Export", model);
        }
        public PartialViewResult ChangeDonVi(string DONVI_ID)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            List<SelectListItem> ListNguoiDung = DM_NGUOIDUNGBusiness.GetByPhongBan(DONVI_ID.ToListInt(','), 0, new List<int>())
                .Select(x => new SelectListItem
                {
                    Text = x.HOTEN,
                    Value = x.ID.ToString()
                }).ToList();
            return PartialView("_ListNhanVien", ListNguoiDung);
        }

    }
}