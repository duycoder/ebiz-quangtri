using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public class resultValidate<T> where T : struct
    {
        public bool Status { get; set; }
        public T Value { get; set; }
    }
    public static class ValidValueData
    {
        public static resultValidate<int> CheckValueInt(this string obj)
        {
            resultValidate<int> result = new resultValidate<int>();
            result.Value = new int();
            int intdata;
            if (int.TryParse(obj, out intdata))
            {
                result.Status = true;
                result.Value = intdata;
            }
            else
            {
                result.Status = false;
            }
            return result;
        }
        public static resultValidate<long> CheckValueLong(this string obj)
        {
            resultValidate<long> result = new resultValidate<long>();
            result.Value = new int();
            long intdata;
            if (long.TryParse(obj, out intdata))
            {
                result.Status = true;
                result.Value = intdata;
            }
            else
            {
                result.Status = false;
            }
            return result;
        }
        public static resultValidate<decimal> CheckValueDecimal(this string obj)
        {
            resultValidate<decimal> result = new resultValidate<decimal>();
            result.Value = new decimal();
            decimal intdata;
            if (decimal.TryParse(obj, out intdata))
            {
                result.Status = true;
                result.Value = intdata;
            }
            else
            {
                result.Status = false;
            }
            return result;
        }

        public static resultValidate<float> CheckValueFloat(this string obj)
        {
            resultValidate<float> result = new resultValidate<float>();
            result.Value = new float();
            float intdata;
            if (float.TryParse(obj, out intdata))
            {
                result.Status = true;
                result.Value = intdata;
            }
            else
            {
                result.Status = false;
            }
            return result;
        }

        public static resultValidate<double> CheckValueDouble(this string obj)
        {
            resultValidate<double> result = new resultValidate<double>();
            result.Value = new double();
            double intdata;
            if (double.TryParse(obj, out intdata))
            {
                result.Status = true;
                result.Value = intdata;
            }
            else
            {
                result.Status = false;
            }
            return result;
        }
        public static resultValidate<bool> CheckValueBool(this string obj, string trueData)
        {
            resultValidate<bool> result = new resultValidate<bool>();
            result.Value = new bool();

            if (obj.ToUpper().Equals(trueData.ToUpper()))
            {
                result.Status = true;
                result.Value = true;
            }
            else
            {
                result.Status = true;
                result.Value = false;
            }
            return result;
        }

        public static resultValidate<DateTime> CheckValueDateTime(this string obj)
        {
            resultValidate<DateTime> result = new resultValidate<DateTime>();
            DateTime dateData;

            if (!string.IsNullOrEmpty(obj))
            {
                var date = obj.Split('/');
                if (date != null)
                {
                    if (date.Length == 3 && !string.IsNullOrEmpty(date[0]) && !string.IsNullOrEmpty(date[1]) && !string.IsNullOrEmpty(date[2]))
                    {
                        string day = "", month = "", year = "";
                        try
                        {
                            day = int.Parse(date[0]).ToString("00");
                            month = int.Parse(date[1]).ToString("00");
                            year = int.Parse(date[2].Substring(0, 4)).ToString("0000");
                        }
                        catch
                        {

                            result.Status = false;
                        }


                        result.Status = DateTime.TryParseExact(string.Format("{0}/{1}/{2}", day, month, year), "dd/MM/yyyy", null, DateTimeStyles.None, out dateData);
                        if (result.Status)
                        {
                            result.Value = dateData;
                        }

                    }
                    else
                    {
                        result.Status = false;
                    }
                }
                return result;
            }
            else
            {

                result.Status = false;

            }

            return result;
        }


    }
}
