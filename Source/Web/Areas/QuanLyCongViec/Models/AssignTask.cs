using System.Collections.Generic;
using Model.Entities;

namespace Web.Areas.QuanLyCongViec.Models
{
    public class AssignTask
    {
        public bool HasRoleAssignTask { get; set; }
        public bool HasRoleAssignChuyenVien { get; set; }
        public bool HasRoleAssignDepartment { get; set; }
        public List<DM_NGUOIDUNG> LstUser { get; set; }
        public List<CCTC_THANHPHAN> LstDept { get; set; }
        public List<CCTC_THANHPHAN> LstAddDept { get; set; }
        public bool AllowAssignDiffDept { get; set; }
        public bool IsCapPhongBan { get; set; }
        public long TASKID { get; set; }
        public long SUBTASKID { get; set; }
        public bool IsAddMoreMember { get; set; }        
    }
}