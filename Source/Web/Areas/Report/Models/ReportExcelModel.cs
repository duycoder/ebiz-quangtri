using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.Report.Models
{
    public class ReportExcelModel
    {
        /// <summary>
        /// STT
        /// </summary>
        public string Stt { get; set; }
        /// <summary>
        /// Hạng mục công việc
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// Nội dung chi tiết
        /// </summary>
        public string JobContent { get; set; }
        /// <summary>
        /// Các bước thực hiện
        /// </summary>
        public string JobStep { get; set; }
        /// <summary>
        /// Mục tiêu công việc
        /// </summary>
        public string Purpose { get; set; }
        /// <summary>
        /// Người xử lý chính
        /// </summary>
        public string MainUser { get; set; }
        /// <summary>
        /// Người hỗ trợ
        /// </summary>
        public string JoinUser { get; set; }
        public string Support { get; set; }
        /// <summary>
        /// Ngày nhận việc
        /// </summary>
        public string ReceiveDate { get; set; }
        /// <summary>
        /// Ngày hoàn thành
        /// </summary>
        public string DateLine { get; set; }

        public string FirstReview { get; set; }
        public string FirstFeedback { get; set; }
        public string FirstWaitingResponse { get; set; }
        public string FirstResult { get; set; }
        public string SecondReview { get; set; }
        public string SecondFeedback { get; set; }
        public string SecondWaitingResponse { get; set; }
        public string SecondResult { get; set; }

        public string ThirdReview { get; set; }
        public string ThirdFeedback { get; set; }
        public string ThirdWaitingResponse { get; set; }
        public string ThirdResult { get; set; }
        /// <summary>
        /// Ngày duyệt kế hoạch
        /// </summary>
        public string ApprovalDate { get; set; }
        /// <summary>
        /// Ngày bắt đầu KH
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// Ngày hoàn thành KH
        /// </summary>
        public string CompleteDate { get; set; }
        /// <summary>
        /// Thời gian triển khai theo KH
        /// </summary>
        public string DeployDay { get; set; }
        /// <summary>
        /// Ngày bắt đầu triển khai thực tế
        /// </summary>
        public string DeployDate { get; set; }
        #region Thời gian lập kế hoạch
        public string FirstBrowser { get; set; }
        public string FirstFeedbackBrowser { get; set; }
        public string FirstWaitingBrowser { get; set; }
        public string FirstResultBrowser { get; set; }

        public string SecondBrowser { get; set; }
        public string SecondFeedbackBrowser { get; set; }
        public string SecondWaitingBrowser { get; set; }
        public string SecondResultBrowser { get; set; }

        public string ThirdBrowser { get; set; }
        public string ThirdFeedbackBrowser { get; set; }
        public string ThirdWaitingBrowser { get; set; }
        public string ThirdResultBrowser { get; set; }
        #endregion
        /// <summary>
        /// Ngày hoàn thành và được phê duyệt
        /// </summary>
        public string ApprovedDate { get; set; }
        /// <summary>
        /// Số ngày triển khai theo thực tế
        /// </summary>
        public string ImplementedReality { get; set; }
        /// <summary>
        /// Số ngày dành cho công việc
        /// </summary>
        public string TotalDays { get; set; }
        /// <summary>
        /// SỐ ngày chờ cấp trên duyệt
        /// </summary>
        public string TotalWaitingApproval { get; set; }
        /// <summary>
        /// Số ngày triển khai thực tế
        /// </summary>
        public string TotalDeployed { get; set; }
        /// <summary>
        /// Số lần trình duyệt
        /// </summary>
        public string TotalBrowsers { get; set; }
        /// <summary>
        /// Số lần điều chỉnh
        /// </summary>
        public string TotalEdit { get; set; }
        /// <summary>
        /// Chênh lệch
        /// </summary>
        public string TotalDifferences { get; set; }
        /// <summary>
        /// Điểm
        /// </summary>
        public string Score { get; set; }
        /// <summary>
        /// Xếp loại
        /// </summary>
        public string Rank { get; set; }
    }
}