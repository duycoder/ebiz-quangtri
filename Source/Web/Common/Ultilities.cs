using Business.CommonBusiness;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;

namespace Web.Common
{
    public static class Ultilities
    {
       
        /// <summary>
        /// Sinh mã đối tượng tiêm theo các tham số truyền vào
        /// written by: namdv
        /// </summary>
        /// <param name="MaTinh">Mã tỉnh/thành phố</param>
        /// <param name="MaHuyen">Mã quận/huyện</param>
        /// <param name="MaXa">Mã xã/phường</param>
        /// <param name="SoThuTu">Số thứ tự của đối tượng (Mã tăng tự động)</param>
        /// <returns></returns>
        public static string SINHMABENHNHAN(string MaTinh, string MaHuyen, string MaXa, string SoThuTu)
        {
            string format = "{0}{1}{2}{3}";
            if (!string.IsNullOrEmpty(MaXa))
            {
                return string.Format(format, string.Empty, string.Empty, MaXa, SoThuTu);
            }
            if (!string.IsNullOrEmpty(MaHuyen))
            {
                return string.Format(format, string.Empty, MaHuyen, string.Empty, SoThuTu);
            }
            if (!string.IsNullOrEmpty(MaTinh))
            {
                return string.Format(format, MaTinh, string.Empty, string.Empty, SoThuTu);
            }
            return SoThuTu;
        }

        public static Random random = new Random((int)DateTime.Now.Ticks);
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static string RandomStrings(DateTime date, int coso_id)
        {
            string format = "{0}{1}{2}{3}{4}{5}";
            return string.Format(format, date.Year.ToString().Substring(2, 2), date.Month.ToString("00"), date.Day.ToString("00"), date.Hour.ToString("00"), date.Minute.ToString("00"), coso_id.ToString("0000"));
        }
        public static DateTime? ToDataTime(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                var date = obj.Split('/');
                if (date != null)
                {
                    if (!string.IsNullOrEmpty(date[0]) && !string.IsNullOrEmpty(date[1]) && !string.IsNullOrEmpty(date[2]))
                    {
                        var day = int.Parse(date[0]).ToString("00");
                        var month = int.Parse(date[1]).ToString("00");
                        var year = int.Parse(date[2]).ToString("0000");
                        return DateTime.ParseExact(string.Format("{0}/{1}/{2}", day, month, year), "dd/MM/yyyy", null);
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        //
        public static DateTime? ToDate(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                var date = obj.Split('-');
                if (date != null)
                {
                    if (!string.IsNullOrEmpty(date[0]) && !string.IsNullOrEmpty(date[1]))
                    {
                        var datetime = date[0];
                        var ngay = datetime.Split('/');
                        var day = int.Parse(ngay[0]).ToString("00");
                        var month = int.Parse(ngay[1]).ToString("00");
                        var year = int.Parse(ngay[2]).ToString("0000");


                        return DateTime.ParseExact(string.Format("{0}/{1}/{2}", day, month, year), "dd/MM/yyyy", null);
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        public static string ToTime(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                var date = obj.Split('-');
                if (date != null)
                {
                    if (!string.IsNullOrEmpty(date[0]) && !string.IsNullOrEmpty(date[1]))
                    {
                        var datetime = date[1];
                        var ngay = datetime.Split(':');
                        var hour = int.Parse(ngay[0]).ToString("00");
                        var minute = int.Parse(ngay[1]).ToString("00");



                        return string.Format("{0}:{1}", hour, minute);
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }








        public static DateTime ToDateTimeNotNull(this string obj)
        {
            var date = obj.Split('/');
            var day = int.Parse(date[0]).ToString("00");
            var month = int.Parse(date[1]).ToString("00");
            var year = int.Parse(date[2]).ToString("0000");
            return DateTime.ParseExact(string.Format("{0}/{1}/{2}", day, month, year), "dd/MM/yyyy", null);
        }
        public static DateTime? ToDateTime(this string obj)
        {
            //try
            //{
            if (!string.IsNullOrEmpty(obj))
            {
                var date = obj.Split('/');
                if (date != null)
                {
                    if (!string.IsNullOrEmpty(date[0]) && !string.IsNullOrEmpty(date[1]) && !string.IsNullOrEmpty(date[2]))
                    {
                        var day = int.Parse(date[0]).ToString("00");
                        var month = int.Parse(date[1]).ToString("00");
                        var year = int.Parse(date[2]).ToString("0000");
                        return DateTime.ParseExact(string.Format("{0}/{1}/{2}", day, month, year), "dd/MM/yyyy", null);
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
            //}catch(Exception ex){
            //    return null;
            // }
        }

        public static bool ToBoolByOnOff(this string obj)
        {
            if (!string.IsNullOrEmpty(obj) && obj.ToLower().Equals("On".ToLower()))
            {
                return true;
            }
                return false;
            
        }
        public static DateTime? ToDateTimeFromMonth(this string obj)
        {
            //try
            //{
            if (!string.IsNullOrEmpty(obj))
            {
                var date = obj.Split('/');
                if (date != null)
                {
                    if (!string.IsNullOrEmpty(date[0]) && !string.IsNullOrEmpty(date[1]))
                    {
                        
                        var month = int.Parse(date[0]).ToString("00");
                        var year = int.Parse(date[1]).ToString("0000");
                        return DateTime.ParseExact(string.Format("{0}/{1}/{2}", "01", month, year), "dd/MM/yyyy", null);
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
           
        }
       

        public static DateTime? ToDateTimeFromYear(this string obj)
        {
            //try
            //{
            if (!string.IsNullOrEmpty(obj))
            {
                return DateTime.ParseExact(string.Format("{0}/{1}/{2}", "01", "01", obj), "dd/MM/yyyy", null);

            }
            else
            {
                return null;
            }
            //}catch(Exception ex){
            //    return null;
            // }
        }
        public static DateTime? ToEndDay(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                var date = obj.Split('/');
                if (date != null)
                {
                    if (!string.IsNullOrEmpty(date[0]) && !string.IsNullOrEmpty(date[1]) && !string.IsNullOrEmpty(date[2]))
                    {
                        var day = int.Parse(date[0]).ToString("00");
                        var month = int.Parse(date[1]).ToString("00");
                        var year = int.Parse(date[2]).ToString("0000");
                        return DateTime.ParseExact(string.Format("{0}/{1}/{2} 23:59:59", day, month, year), "dd/MM/yyyy HH:mm:ss", null);
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        public static DateTime? ToStartDay(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                var date = obj.Split('/');
                if (date != null)
                {
                    if (!string.IsNullOrEmpty(date[0]) && !string.IsNullOrEmpty(date[1]) && !string.IsNullOrEmpty(date[2]))
                    {
                        var day = int.Parse(date[0]).ToString("00");
                        var month = int.Parse(date[1]).ToString("00");
                        var year = int.Parse(date[2]).ToString("0000");
                        return DateTime.ParseExact(string.Format("{0}/{1}/{2} 00:00:00", day, month, year), "dd/MM/yyyy HH:mm:ss", null);
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        public static DateTime? ToEndYear(this DateTime? obj)
        {
            if (obj.HasValue)
            {
                var day = DateTime.DaysInMonth( obj.Value.Year,12).ToString("00");

                return DateTime.ParseExact(string.Format("{0}/{1}/{2} 23:59:59", day, "12", obj.Value.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);

            }
            else
            {
                return null;
            }
        }

        public static DateTime? ToEndMonth(this DateTime? obj)
        {
            if (obj.HasValue)
            {
                var day = DateTime.DaysInMonth(obj.Value.Year, obj.Value.Month).ToString("00");

                return DateTime.ParseExact(string.Format("{0}/{1}/{2} 23:59:59", day, obj.Value.Month.ToString("00"), obj.Value.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);

            }
            else
            {
                return null;
            }
        }
        public static DateTime ToEndMonth(this DateTime obj)
        {
            
                var day = DateTime.DaysInMonth(obj.Year, obj.Month).ToString("00");

                return DateTime.ParseExact(string.Format("{0}/{1}/{2} 23:59:59", day, obj.Month.ToString("00"), obj.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);

           
        }
        public static DateTime? ToEndDay(this DateTime? obj)
        {
            if (obj.HasValue)
            {


                return DateTime.ParseExact(string.Format("{0}/{1}/{2} 23:59:59", obj.Value.Day.ToString("00"), obj.Value.Month.ToString("00"), obj.Value.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);

            }
            else
            {
                return null;
            }
        }
        public static DateTime ToEndDay(this DateTime obj)
        {
            

                return DateTime.ParseExact(string.Format("{0}/{1}/{2} 23:59:59", obj.Day.ToString("00"), obj.Month.ToString("00"), obj.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);

        }
        public static DateTime? ToStartDay(this DateTime? obj)
        {
            if (obj.HasValue)
            {


                return DateTime.ParseExact(string.Format("{0}/{1}/{2} 00:00:00", obj.Value.Day.ToString("00"), obj.Value.Month.ToString("00"), obj.Value.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);

            }
            else
            {
                return null;
            }
        }
        public static DateTime ToStartDay(this DateTime obj)
        {
            
                return DateTime.ParseExact(string.Format("{0}/{1}/{2} 00:00:00", obj.Day.ToString("00"), obj.Month.ToString("00"), obj.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);

        }
        public static DateTime? ToStartMonth(this DateTime? obj)
        {
            if (obj.HasValue)
            {
                

                return DateTime.ParseExact(string.Format("{0}/{1}/{2} 00:00:00", "01", obj.Value.Month.ToString("00"), obj.Value.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);

            }
            else
            {
                return null;
            }
        }
        public static DateTime ToStartMonth(this DateTime obj)
        {
            

                return DateTime.ParseExact(string.Format("{0}/{1}/{2} 00:00:00", "01", obj.Month.ToString("00"), obj.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);

        }
        public static DateTime ToEndYear(this DateTime obj)
        {

            var day = DateTime.DaysInMonth( obj.Year,12).ToString("00");

            return DateTime.ParseExact(string.Format("{0}/{1}/{2} 23:59:59", day, "12", obj.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);


        }
        public static DateTime? ToStartYear(this DateTime? obj)
        {
            if (obj.HasValue)
            {

                return DateTime.ParseExact(string.Format("{0}/{1}/{2} 00:00:00", "01", "01", obj.Value.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);

            }
            else
            {
                return null;
            }
        }
        public static DateTime ToStartYear(this DateTime obj)
        {

            return DateTime.ParseExact(string.Format("{0}/{1}/{2} 00:00:00", "01", "01", obj.Year.ToString("0000")), "dd/MM/yyyy HH:mm:ss", null);


        }

        public static short? ToShortOrNULL(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return short.Parse(obj);
            }
            else
            {
                return null;
            }
        }

        public static int? ToIntOrNULL(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return int.Parse(obj);
            }
            else
            {
                return null;
            }
        }
        public static long? ToLongOrNULL(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return long.Parse(obj);
            }
            else
            {
                return null;
            }
        }
        public static short ToShortOrZero(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return short.Parse(obj);
            }
            else
            {
                return 0;
            }
        }

        public static int ToIntOrZero(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return int.Parse(obj);
            }
            else
            {
                return 0;
            }
        }
        public static bool ToBoolOrFalse(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return bool.Parse(obj);
            }
            else
            {
                return false;
            }
        }
        public static long ToLongOrZero(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return long.Parse(obj);
            }
            else
            {
                return 0;
            }
        }

        public static float ToFloatOrZero(this string obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(obj))
                {
                    return float.Parse(obj);
                }
                else
                {
                    return 0;
                }
            }
            catch
            {

                return 0;
            }

        }
        public static decimal ToDecimalOrZero(this string obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(obj))
                {
                    return decimal.Parse(obj);
                }
                else
                {
                    return 0;
                }
            }
            catch
            {

                return 0;
            }
        }
        public static List<long> ToListLong(this string obj, char split_key)
        {
            List<long> listLong = new List<long>();
            if (!string.IsNullOrEmpty(obj))
            {
                var list = obj.Split(split_key);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            listLong.Add(long.Parse(item));
                        }
                    }
                }
            }
            return listLong;
        }
        public static List<int> ToListInt(this string obj, char split_key)
        {
            List<int> listInt = new List<int>();
            if (!string.IsNullOrEmpty(obj))
            {
                var list = obj.Split(split_key);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            listInt.Add(int.Parse(item));
                        }
                    }
                }
            }
            return listInt;
        }
        public static List<string> ToListStringLower(this string obj, char split_key)
        {
            List<string> listInt = new List<string>();
            if (!string.IsNullOrEmpty(obj))
            {
                var list = obj.Split(split_key);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            listInt.Add(item);
                        }
                    }
                }
            }
            return listInt;
        }


        public static List<short> ToListShort(this string obj, char split_key)
        {
            List<short> listInt = new List<short>();
            if (!string.IsNullOrEmpty(obj))
            {
                var list = obj.Split(split_key);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            listInt.Add(short.Parse(item));
                        }
                    }
                }
            }
            return listInt;
        }

        public static string CollapseSpaces(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return Regex.Replace(value, @"\s+", " ");
            }
            return string.Empty;
        }

        public static string HtmlEncoder(this string value)
        {
            StringBuilder sb = new StringBuilder(HttpUtility.HtmlEncode(value));
            // Selectively allow  and <i>
            sb.Replace("&lt;b&gt;", "<b>");
            sb.Replace("&lt;/b&gt;", "");
            sb.Replace("&lt;i&gt;", "<i>");
            sb.Replace("&lt;/i&gt;", "");
            return sb.ToString();
        }

        public static string HtmlDecoder(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return HttpUtility.HtmlDecode(value);
                String myEncodedString;
                // Encode the string.
                myEncodedString = HttpUtility.HtmlEncode(value);

                StringWriter myWriter = new StringWriter();
                // Decode the encoded string.
                HttpUtility.HtmlDecode(myEncodedString, myWriter);
                return myWriter.ToString();
                //return HttpUtility.HtmlDecode(value);
            }
            return value;
        }
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear, System.Globalization.CultureInfo ci)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if (firstWeek <= 1 || firstWeek > 50)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }
        public static DateTime GetStartOfWeek(int year, int month, int weekofmonth)
        {
            //lấy ngày bắt đầu của tuần trong tháng
            var day = weekofmonth * 7 - 6;
            var StartDate = new DateTime(year, month, day);
            var weekOfYear = GetIso8601WeekOfYear(StartDate);
            return FirstDateOfWeek(year, weekOfYear, CultureInfo.CurrentCulture);
        }

        public static DateTime GetEndOfWeek(DateTime startOfWeek)
        {
            return startOfWeek.AddDays(6);
        }

       
        private static double DaysInYear(this int iYear)
        {
            var startDate = new DateTime(iYear, 1, 1);
            var enddate = new DateTime(iYear, 12, 31);
            var totalDate = enddate - startDate;
            return totalDate.TotalDays;
        }

        /// <summary>
        /// Trả ra số tuần trong 1 năm
        /// </summary>
        /// <param name="iYear">Năm cần tính số tuần</param>
        /// <returns></returns>
        public static List<int> ListWeekOfYear(int iYear)
        {
            //lấy tổng số ngày trong năm
            double countDays = iYear.DaysInYear();
            List<int> arrWeeks = new List<int>();
            var j = 0;
            for (int i = 1; i <= countDays; i = i + 7)
            {
                j++;
                arrWeeks.Add(j);
            }
            //trả ra danh sách tuần theo năm
            return arrWeeks;
        }


        /// <summary>
        /// Trả ra ngày bắt đầu và ngày kết thúc của tuần theo năm
        /// </summary>
        /// <param name="iWeek">Tuần</param>
        /// <param name="iYear">Năm</param>
        /// <returns></returns>
       

        static readonly string[] Columns = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ", "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ" };
        public static string ToIndexToColumn(this int index)
        {
            if (index <= 0)
                throw new IndexOutOfRangeException("index must be a positive number");

            return Columns[index - 1];
        }

        public static string Truncate(string input = "", int length = 0)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (input.Length <= length)
                {
                    return input;
                }
                else
                {
                    return input.Substring(0, length) + "...";
                }
            }
            return string.Empty;
        }

        public static string GetSummary(this string input, int length = 0)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (input.Length <= length)
                {
                    return input;
                }
                else
                {
                    return input.Substring(0, length) + "...";
                }
            }
            return string.Empty;
        }

      
        public static string ConvertToVN(this string chucodau)
        {
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            int index = -1;
            char[] arrChar = FindText.ToCharArray();
            while ((index = chucodau.IndexOfAny(arrChar)) != -1)
            {
                int index2 = FindText.IndexOf(chucodau[index]);
                chucodau = chucodau.Replace(chucodau[index], ReplText[index2]);
            }
            return chucodau;
        }

     
        private static readonly string[] VietnameseSigns = new string[]

        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"

        };
        public static string RemoveSign4VietnameseString(string str)
        {

            //Tiến hành thay thế , lọc bỏ dấu cho chuỗi

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {

                for (int j = 0; j < VietnameseSigns[i].Length; j++)

                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

            }

            return str;

        }
        public static string ConvertMoney(string number)
        {
            var sb = "";
            var split = number.Trim().Split('.').ToList();
            for (var i = 0; i < split.Count; i++)
            {
                if (i > 0)
                    sb += " lẻ ";
                sb += ChuyenSo(split[i]);
            }
            sb += " đồng";

            var result = Regex.Replace(sb.Substring(0, 1).ToUpper() + sb.Substring(1).ToLower(), @"\s+", " ");
            return "" + result + ".";
        }

        private static string ChuyenSo(string number)
        {
            string[] dv = { "", "mươi", "trăm", "nghìn, ", "triệu, ", "tỉ, " };
            string[] cs = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

            var length = number.Length;
            number += "ss";
            var doc = new StringBuilder();
            var rd = 0;

            var i = 0;
            while (i < length)
            {
                //So chu so o hang dang duyet
                var n = (length - i + 2) % 3 + 1;

                //Kiem tra so 0
                var found = 0;
                int j;
                for (j = 0; j < n; j++)
                {
                    if (number[i + j] == '0') continue;
                    found = 1;
                    break;
                }

                //Duyet n chu so
                int k;
                if (found == 1)
                {
                    rd = 1;
                    for (j = 0; j < n; j++)
                    {
                        var ddv = 1;
                        switch (number[i + j])
                        {
                            case '0':
                                if (n - j == 3)
                                    doc.Append(cs[0]);
                                if (n - j == 2)
                                {
                                    if (number[i + j + 1] != '0')
                                        doc.Append("lẻ");
                                    ddv = 0;
                                }
                                break;
                            case '1':
                                switch (n - j)
                                {
                                    case 3:
                                        doc.Append(cs[1]);
                                        break;
                                    case 2:
                                        doc.Append("mười");
                                        ddv = 0;
                                        break;
                                    case 1: k = (i + j == 0) ? 0 : i + j - 1;
                                        doc.Append((number[k] != '1' && number[k] != '0') ? "mốt" : cs[1]);
                                        break;
                                }
                                break;
                            case '5':
                                doc.Append((i + j == length - 1) ? "lăm" : cs[5]);
                                break;
                            default:
                                doc.Append(cs[number[i + j] - 48]);
                                break;
                        }

                        doc.Append(" ");

                        //Doc don vi nho
                        if (ddv == 1)
                            doc.Append(dv[n - j - 1] + " ");
                    }
                }


                //Doc don vi lon
                if (length - i - n > 0)
                {
                    if ((length - i - n) % 9 == 0)
                    {
                        if (rd == 1)
                            for (k = 0; k < (length - i - n) / 9; k++)
                                doc.Append("tỉ, ");
                        rd = 0;
                    }
                    else
                        if (found != 0) doc.Append(dv[((length - i - n + 1) % 9) / 3 + 2] + " ");
                }

                i += n;
            }

            return (length == 1) && (number[0] == '0' || number[0] == '5') ? cs[number[0] - 48] : doc.ToString();
        }

        public static string ToSafeFileName(this string s)
        {
            return s
                .Replace("\\", "")
                .Replace("/", "")
                .Replace("\"", "")
                .Replace("*", "")
                .Replace(":", "")
                .Replace("?", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "");
        }
    }
}