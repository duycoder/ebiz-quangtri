using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop;
using ExcelInterop = Microsoft.Office.Interop.Excel;
using System.Reflection;
using CommonHelper;
namespace CommonHelper.Excel
{
    public class ConstantData
    {
        public const string TYPE_STRING = "System.String";
        public const string TYPE_INT = "System.Int32";
        public const string TYPE_INT_NULL = "System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
        public const string TYPE_DATETIME = "System.DateTime";
        public const string TYPE_DATETIME_NULL = "System.Nullable`1[[System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
        public const string TYPE_LONG = "System.Int64";
        public const string TYPE_LONG_NULL = "System.Nullable`1[[System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
        public const string TYPE_DECIMAL = "System.Decimal";
        public const string TYPE_DECIMAL_NULL = "System.Nullable`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
        public const string TYPE_FLOAT = "System.Single";
        public const string TYPE_DOUBLE = "System.Double";
        public const string TYPE_BOOL = "System.Boolean";

    }
    public class ConfigModule
    {
        //Tên thuộc tính trong lớp import
        public string columnName { get; set; }
        //Thuộc tính có bắt buộc hay không 
        public bool require { get; set; }
        //Có ràng buộc nhiều giá trị hay không 
        public bool isMulti { get; set; }
        //Danh sách các giá trị
        public List<object> ListValue { get; set; }

        public PropertyInfo Property { get; set; }
        public string TypeValue { get; set; }
        public object Min { get; set; }
        public object Max { get; set; }
        public string TrueValue { get; set; }
    }

    public class ReponseImport<T> where T : class
    {
        public bool Status { get; set; }
        public string Message { get; set; }

        public List<T> ListTrue { get; set; }
        public List<List<string>> lstFalse { get; set; }
    }

    public class ImportExcelHelper<T> where T : class
    {

        //Đường dẫn template
        public string PathTemplate { get; set; }
        //dòng bắt đầu
        public int StartRow { get; set; }
        //cột bắt đầu
        public int StartCol { get; set; }
        //Cấu hình các trường thông tin theo property của class
        public List<ConfigModule> ConfigColumn { get; set; }
        public ImportExcelHelper()
        {
            StartRow = 4;
            StartCol = 2;
        }
        public ReponseImport<T> Import()
        {
            string ListExtensionAllow = ".xls,.xlsx";
            var result = new ReponseImport<T>();
            result.Status = true;
            result.ListTrue = new List<T>();
            result.lstFalse = new List<List<string>>();
            if (string.IsNullOrEmpty(PathTemplate))
            {
                result.Status = false;
                result.Message = "Không tìm thấy đường dẫn đọc dữ liệu";
                return result;
            }

            if (!(ConfigColumn != null && ConfigColumn.Any()))
            {
                result.Status = false;
                result.Message = "Vui lòng cấu hình thông tin cột import";
                return result;
            }

            if (!File.Exists(PathTemplate))
            {
                result.Status = false;
                result.Message = "Không tìm thấy File.";
                return result;
            }
            else
            {
                var filename = Path.GetFileName(PathTemplate);
                var arrName = filename.Split('.');
                var extention = '.' + arrName[arrName.Length - 1];

                #region Check extention có hợp lệ không

                var listExtention = ListExtensionAllow.Split(',');
                if (!listExtention.Contains(extention))
                {
                    result.Status = false;
                    result.Message = "Định dạng file không được chấp nhận. <br/> Chỉ import file .xls và .xlsx";
                    return result;
                }
                #endregion
            }

            ExcelInterop.Application xlApp = new ExcelInterop.Application();
            ExcelInterop.Workbook xlWorkbook = xlApp.Workbooks.Open(PathTemplate);
            ExcelInterop._Worksheet xlWorksheet = (ExcelInterop._Worksheet)xlWorkbook.Sheets[1];
            ExcelInterop.Range xlRange = xlWorksheet.UsedRange;
            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            foreach (var item in ConfigColumn)
            {
                item.Property = typeof(T).GetProperty(item.columnName);
                //item.TypeValue = item.Property.PropertyType;
            }

            for (int i = StartRow; i <= rowCount; i++)
            {
                var isTrueValue = true;
                T objTrue = Activator.CreateInstance<T>();
                //Khởi tạo listSai
                var lstFalseobj = new List<string>();
                lstFalseobj.Add(i.ToString());
                int checkEndFile = 0;
                var MessErr = "";
                for (int c = 2; c < ConfigColumn.Count + 2; c++)
                {
                    //ĐỐi tượng config cho cột
                    var objConfig = ConfigColumn[c - 2];
                    //Khởi tạo đối tượng đúng
                    var excelComlumnData = (xlRange.Cells[i, c] as ExcelInterop.Range).Value;
                    string vl = excelComlumnData != null ? excelComlumnData.ToString() : "";
                    if (vl.ToUpper().Equals("NULL"))
                    {
                        vl = "";
                    }
                    //if (objConfig.require && string.IsNullOrEmpty(vl))
                    //{
                    //    isTrueValue = false;
                    //}
                    //Gán gia trị vào listFalse
                    lstFalseobj.Add(vl);


                    if (!string.IsNullOrEmpty(vl))
                    {

                        switch (objConfig.TypeValue)
                        {
                            case ConstantData.TYPE_STRING:
                                objConfig.Property.SetValue(objTrue, vl);
                                break;
                            case ConstantData.TYPE_INT:
                                var rsCheckValue = vl.CheckValueInt();
                                if (rsCheckValue.Status)
                                {
                                    objConfig.Property.SetValue(objTrue, rsCheckValue.Value);
                                    if (objConfig.isMulti && !objConfig.ListValue.Contains((object)rsCheckValue.Value))
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị nằm ngoài miền cho phép </br>";
                                    }
                                    if (objConfig.Min != null && (int)objConfig.Min > rsCheckValue.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần lớn hơn " + objConfig.Min.ToString() + " </br>";
                                    }
                                    if (objConfig.Max != null && (int)objConfig.Max < rsCheckValue.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần nhỏ hơn " + objConfig.Max.ToString() + " </br>";
                                    }
                                }
                                else
                                {

                                    isTrueValue = false;
                                    MessErr += "+ Cột số " + c + " cần nhập số </br>";

                                };
                                break;
                            case ConstantData.TYPE_LONG:
                                var rsCheckValuelong = vl.CheckValueLong();
                                if (rsCheckValuelong.Status)
                                {
                                    objConfig.Property.SetValue(objTrue, rsCheckValuelong.Value);
                                    if (objConfig.isMulti && !objConfig.ListValue.Contains((object)rsCheckValuelong.Value))
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị nằm ngoài miền cho phép </br>";
                                    }
                                    if (objConfig.Min != null && Convert.ToInt64(objConfig.Min) > rsCheckValuelong.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần lớn hơn " + objConfig.Min.ToString() + " </br>";
                                    }
                                    if (objConfig.Max != null && Convert.ToInt64(objConfig.Max) < rsCheckValuelong.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần nhỏ hơn " + objConfig.Max.ToString() + " </br>";
                                    }
                                }
                                else
                                {
                                    isTrueValue = false;
                                    MessErr += "+ Cột số " + c + " cần nhập số </br>";

                                };
                                break;
                            case ConstantData.TYPE_DECIMAL:
                                var rsCheckValueDC = vl.CheckValueDecimal();
                                if (rsCheckValueDC.Status)
                                {
                                    objConfig.Property.SetValue(objTrue, rsCheckValueDC.Value);
                                    if (objConfig.isMulti && !objConfig.ListValue.Contains((object)rsCheckValueDC.Value))
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị nằm ngoài miền cho phép </br>";
                                    }
                                    if (objConfig.Min != null && Convert.ToDecimal(objConfig.Min) > rsCheckValueDC.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần lớn hơn " + objConfig.Min.ToString() + " </br>";
                                    }
                                    if (objConfig.Max != null && Convert.ToDecimal(objConfig.Max) < rsCheckValueDC.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần nhỏ hơn " + objConfig.Max.ToString() + " </br>";
                                    }
                                }
                                else
                                {

                                    isTrueValue = false;
                                    MessErr += "+ Cột số " + c + " cần nhập số </br>";

                                };
                                break;
                            case ConstantData.TYPE_DATETIME:
                                var rsCheckValueDT = vl.CheckValueDateTime();
                                if (rsCheckValueDT.Status)
                                {
                                    objConfig.Property.SetValue(objTrue, rsCheckValueDT.Value);
                                    if (objConfig.isMulti && !objConfig.ListValue.Contains((object)rsCheckValueDT.Value))
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị nằm ngoài miền cho phép </br>";
                                    }
                                    if (objConfig.Min != null && Convert.ToDateTime(objConfig.Min) > rsCheckValueDT.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần lớn hơn " + objConfig.Min.ToString() + " </br>";
                                    }
                                    if (objConfig.Max != null && Convert.ToDateTime(objConfig.Max) < rsCheckValueDT.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần nhỏ hơn " + objConfig.Max.ToString() + " </br>";
                                    }
                                }
                                else
                                {

                                    isTrueValue = false;
                                    MessErr += "+ Cột số " + c + " cần nhập đúng định dạng dd/MM/yyyy </br>";


                                };
                                break;
                            case ConstantData.TYPE_FLOAT:
                                var rsCheckValueFL = vl.CheckValueFloat();
                                if (rsCheckValueFL.Status)
                                {
                                    objConfig.Property.SetValue(objTrue, rsCheckValueFL.Value);
                                    if (objConfig.isMulti && !objConfig.ListValue.Contains((object)rsCheckValueFL.Value))
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị nằm ngoài miền cho phép </br>";
                                    }
                                    if (objConfig.Min != null && Convert.ToSingle(objConfig.Min) > rsCheckValueFL.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần lớn hơn " + objConfig.Min.ToString() + " </br>";
                                    }
                                    if (objConfig.Max != null && Convert.ToSingle(objConfig.Max) < rsCheckValueFL.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần nhỏ hơn " + objConfig.Max.ToString() + " </br>";
                                    }
                                }
                                else
                                {

                                    isTrueValue = false;
                                    MessErr += "+ Cột số " + c + " cần nhập đúng định dạng số thập phân </br>";


                                };
                                break;
                            case ConstantData.TYPE_DOUBLE:
                                var rsCheckValueDB = vl.CheckValueDouble();
                                if (rsCheckValueDB.Status)
                                {
                                    objConfig.Property.SetValue(objTrue, rsCheckValueDB.Value);
                                    if (objConfig.isMulti && !objConfig.ListValue.Contains((object)rsCheckValueDB.Value))
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị nằm ngoài miền cho phép </br>";
                                    }
                                    if (objConfig.Min != null && Convert.ToDouble(objConfig.Min) > rsCheckValueDB.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần lớn hơn " + objConfig.Min.ToString() + " </br>";
                                    }
                                    if (objConfig.Max != null && Convert.ToDouble(objConfig.Max) < rsCheckValueDB.Value)
                                    {
                                        isTrueValue = false;
                                        MessErr += "+ Cột số " + c + " giá trị cần nhỏ hơn " + objConfig.Max.ToString() + " </br>";
                                    }
                                }
                                else
                                {

                                    isTrueValue = false;
                                    MessErr += "+ Cột số " + c + " cần nhập đúng định dạng số thập phân </br>";


                                };
                                break;
                            case ConstantData.TYPE_BOOL:
                                var rsCheckValueBool = vl.CheckValueBool(objConfig.TrueValue);
                                if (rsCheckValueBool.Status)
                                {
                                    objConfig.Property.SetValue(objTrue, rsCheckValueBool.Value);
                                }
                                break;
                        }

                    }
                    else
                    {
                        checkEndFile++;
                        if (objConfig.require)
                        {
                            isTrueValue = false;
                        }

                    }


                }
                if (checkEndFile == ConfigColumn.Count)
                {
                    break;
                }
                if (isTrueValue)
                {
                    result.ListTrue.Add(objTrue);
                }
                else
                {
                    lstFalseobj.Add(MessErr);
                    result.lstFalse.Add(lstFalseobj);
                }
            }
            xlWorkbook.Close();
            xlApp.DisplayAlerts = false;
            xlApp.Quit();
            ExcelKiller.TerminateExcelProcess(xlApp);
            return result;

        }


    }

}
