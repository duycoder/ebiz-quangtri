using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CommonHelper.DateExtend
{
    public static class Utility
    {
        static GregorianCalendar gc = new GregorianCalendar();

        /// <summary>
        /// @author: duynn
        /// @description: lấy số tt của tuần
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetWeekNumber(this DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            int weekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNumber;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy tên của tuần
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetNameOfWeek(this DateTime time)
        {
            string weekName = "Tuần " + time.GetWeekNumber();
            return weekName;
        }


        /// <summary>
        /// @author: duynn
        /// @description: lấy ngày bắt đầu của tuần
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetStartDayOfWeek(this DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day == DayOfWeek.Sunday)
            {
                time = time.AddDays(-6);
            }
            else if (day != DayOfWeek.Monday)
            {
                int diff = (7 + (time.DayOfWeek - DayOfWeek.Monday)) % 7;
                time = time.AddDays(-1 * diff).Date;
            }
            return time;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy ngày kết thúc của tuần
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>

        public static DateTime GetEndDayOfWeek(this DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day == DayOfWeek.Monday)
            {
                time = time.AddDays(6);
            }
            else if (day != DayOfWeek.Sunday)
            {
                int diff = (7 + (time.DayOfWeek - DayOfWeek.Sunday)) % 7;
                time = time.AddDays(-1 * diff).AddDays(7).Date;
            }
            return time;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy tên ngày
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetNameOfDay(this DateTime time)
        {
            string title = string.Empty;
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            switch (day)
            {
                case DayOfWeek.Sunday:
                    title = "Chủ nhật";
                    break;
                case DayOfWeek.Monday:
                    title = "Thứ 2";
                    break;
                case DayOfWeek.Tuesday:
                    title = "Thứ 3";
                    break;
                case DayOfWeek.Wednesday:
                    title = "Thứ 4";
                    break;
                case DayOfWeek.Thursday:
                    title = "Thứ 5";
                    break;
                case DayOfWeek.Friday:
                    title = "Thứ 6";
                    break;
                default:
                    title = "Thứ 7";
                    break;
            }
            return title;
        }

        public static string ToVietnameseDateFormat(this DateTime time)
        {
            return string.Format("{0:dd/MM/yyyy}", time);
        }


        /// <summary>
        /// @author: duynn
        /// @description: kiểm tra ngày đã ở tuần cũ
        /// @since: 15/08/2018
        /// </summary>
        /// <param name="workfDate"></param>
        /// <returns></returns>
        public static bool IsOldWeek(this DateTime workfDate)
        {
            int weekOfWorkDate = workfDate.GetWeekNumber();
            int weekOfCurrentDate = DateTime.Now.GetWeekNumber();

            double diffDay = DateTime.Now.Subtract(workfDate).TotalDays;

            bool isObsolete = (diffDay > 0 && (weekOfCurrentDate > weekOfWorkDate));
            return isObsolete;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy số tt của tuần trong tháng từ ngày hiện tại
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách giờ
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetHours(int selectedHour = 0, int startHour = 0)
        {
            List<SelectListItem> hours = new List<SelectListItem>();
            Enumerable.Range(startHour, 24 - startHour).ToList().ForEach(x =>
                {
                    hours.Add(new SelectListItem()
                    {
                        Value = x.ToString(),
                        Text = x.ToString("D2"),
                        Selected = (x == selectedHour)
                    });
                });
            return hours;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách phút
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetMinutes(int selectedMinute = 0, int divisor = 0)
        {
            List<SelectListItem> minutes = new List<SelectListItem>();
            if (divisor > 0)
            {
                Enumerable.Range(0, 60).ToList().ForEach(x =>
                {
                    if (x % divisor == 0)
                    {
                        minutes.Add(new SelectListItem()
                        {
                            Value = x.ToString(),
                            Text = x.ToString("D2"),
                            Selected = (x == selectedMinute)
                        });
                    }
                });
            }
            else
            {
                Enumerable.Range(0, 60).ToList().ForEach(x =>
                {
                    minutes.Add(new SelectListItem()
                    {
                        Value = x.ToString(),
                        Text = x.ToString("D2"),
                        Selected = (x == selectedMinute)
                    });
                });
            }
            return minutes;
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách tháng
        /// </summary>
        /// <param name="selectedMonth"></param>
        /// <returns></returns>
        public static List<SelectListItem> GetMonths(int selectedMonth = 1)
        {
            List<SelectListItem> months = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = "Tháng " + i,
                    Selected = (i == selectedMonth)
                });
            }
            return months;
        }


        /// <summary>
        /// @author: duynn
        /// @description: lấy danh sách tháng
        /// </summary>
        /// <param name="selectedMonth"></param>
        /// <returns></returns>
        public static List<SelectListItem> GetYears(int selectedYear = 0)
        {
            List<SelectListItem> months = new List<SelectListItem>();
            int yearStart = DateTime.Now.Year - 10;
            int yearEnd = DateTime.Now.Year;
            for (int i = yearEnd; i >= yearStart; i--)
            {
                months.Add(new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString(),
                    Selected = (i == selectedYear)
                });
            }
            return months;
        }

        /// <summary>
        /// @author: duynn
        /// @description: tính toán số tuần trong tháng
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int GetWeeksOfMonth(int month, int year)
        {
            int mondaysCount = 0;
            int tuesdaysCount = 0;
            int wednesdaysCount = 0;
            int thursdaysCount = 0;
            int fridaysCount = 0;
            int saturdaysCount = 0;
            int sundaysCount = 0;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            List<int> groupDaysCountOfWeek = new List<int>() {
                mondaysCount,
                tuesdaysCount,
                wednesdaysCount,
                thursdaysCount,
                fridaysCount,
                saturdaysCount,
                sundaysCount
            };

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            for (int i = 0; i < daysInMonth; i++)
            {
                var dayOfWeek = firstDayOfMonth.AddDays(i).DayOfWeek;
                if (dayOfWeek == DayOfWeek.Monday)
                {
                    mondaysCount++;
                    groupDaysCountOfWeek[0] = mondaysCount;
                }
                else if (dayOfWeek == DayOfWeek.Tuesday)
                {
                    tuesdaysCount++;
                    groupDaysCountOfWeek[1] = tuesdaysCount;
                }
                else if (dayOfWeek == DayOfWeek.Wednesday)
                {
                    wednesdaysCount++;
                    groupDaysCountOfWeek[2] = wednesdaysCount;
                }
                else if (dayOfWeek == DayOfWeek.Thursday)
                {
                    thursdaysCount++;
                    groupDaysCountOfWeek[3] = thursdaysCount;
                }
                else if (dayOfWeek == DayOfWeek.Friday)
                {
                    fridaysCount++;
                    groupDaysCountOfWeek[4] = fridaysCount;
                }
                else if (dayOfWeek == DayOfWeek.Saturday)
                {
                    saturdaysCount++;
                    groupDaysCountOfWeek[5] = saturdaysCount;
                }
                else if (dayOfWeek == DayOfWeek.Sunday)
                {
                    sundaysCount++;
                    groupDaysCountOfWeek[6] = sundaysCount;
                }
            }
            int result = groupDaysCountOfWeek.Max();
            return result;
        }
    }
}
