using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.THUMUCLUUTRUArea.Models
{
    public class AccessModifier
    {
        /// <summary>
        /// Toàn bộ người dùng trong hệ thống được phép truy cập
        /// </summary>
        public const int ALL_SYSTEM = 1;
        /// <summary>
        /// Toàn bộ người dùng trong đơn vị
        /// </summary>
        public const int ALL_UNIT = 2;
        /// <summary>
        /// Toàn bộ người dùng trong phòng ban
        /// </summary>
        public const int ALL_DEPARTMENT = 3;
        /// <summary>
        /// Chỉ cá nhân
        /// </summary>
        public const int PRIVATE = 4;
        /// <summary>
        /// Quyền thao tác tệp tin của tập đoàn
        /// </summary>
        public const string ACCESS_SYSTEM = "ACCESS_SYSTEM";
        /// <summary>
        /// Quyền thao tác tệp tin của phòng ban
        /// </summary>
        public const string ACCESS_DEPT = "ACCESS_DEPT";
        /// <summary>
        /// Quyền thao tác tệp tin của đơn vị
        /// </summary>
        public const string ACCESS_UNIT = "ACCESS_UNIT";
    }
    public class FolderPermission
    {
        /// <summary>
        /// Chỉ có thể đọc
        /// </summary>
        public const int CAN_READ = 1;
        /// <summary>
        /// Chỉ có thể ghi
        /// </summary>
        public const int CAN_WRITE = 2;
        /// <summary>
        /// Thực hiện cả đọc lẫn ghi
        /// </summary>
        public const int BOTH = 3;
    }
}