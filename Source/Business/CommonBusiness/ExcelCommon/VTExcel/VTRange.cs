using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NativeExcel;
using System.Drawing;

namespace VTUtils.Excel.Export
{
    public class VTRange : IVTRange
    {
        #region Properties
        public IRange Range { get; set; }

        public IRange this[int row, int col]
        {
            get
            {
                return Range[row, col];
            }
        }

        public IVTWorksheet Sheet { get; set; }

        /// <summary>
        /// Property hiden of entire column
        /// </summary>
        public bool IsColumnHidden
        {
            get
            {
                return Range.EntireColumn.Hidden;
            }
            set
            {
                Range.EntireColumn.Hidden = value;
            }
        }
        
        /// <summary>
        /// Property hiden of entire row 
        /// </summary>
        public bool IsRowHidden
        {
            get
            {
                return Range.EntireRow.Hidden;
            }
            set
            {
                Range.EntireRow.Hidden = value;
            }
        }

        public object Value
        {
            get
            {
                return Range.Value;
            }
            set
            {
                Range.Value = value;
            }
        }

        public int TotalColumn
        {
            get
            {
                return Range.Columns.Count;
            }
        }

        public int TotalRow
        {
            get
            {
                return Range.Rows.Count;
            }
        }

        /// <summary>
        /// Lock cell - effect when the sheet is placed password
        /// Hungnd 28/03/2013
        /// </summary>
        public bool IsLock
        {
            get
            {
                return Range.Locked;
            }
            set
            {
                Range.Locked = value;
            }
        }

        public VTRange(IRange range, IVTWorksheet sheet)
        {
            Range = range;
            Sheet = sheet;
        }

        private object GetPropertyValue(object obj, string property)
        {
            if (property.ToUpper().StartsWith("GET("))
            {
                string key = property.Substring(4, property.Length - 5);
                Dictionary<string, object> dic = (Dictionary<string, object>)obj;
                if (dic.ContainsKey(key))
                {
                    return dic[key];
                }
                else
                {
                    return "";
                }
            }
            return obj.GetType().GetProperty(property).GetValue(obj, null);
        }

        #endregion

        #region Method
        /// <summary>
        /// Delete entire column of range
        /// </summary>
        public void DeleteEntireColumn()
        {
            Range.EntireColumn.Delete();
        }

        public void DeleteEntireRow()
        {
            Range.EntireRow.Delete();
        }

        public void FillVariableValue(Dictionary<string, object> Data, IVTWorksheet OtherRange = null)
        {

            foreach (IRangeRow row in Range.Rows)
            {

                foreach (IRange cell in row.Columns)
                {

                    string value = cell.Value == null ? "" : cell.Value.ToString();
                    // value = ${abc[].xyz.123}
                    MatchCollection matches = Regex.Matches(value, @"\$\{([^\}]+)\}");

                    foreach (Match match in matches)
                    {
                        // Finally, we get the Group value and display it.
                        string key = match.Groups[1].Value;

                        string[] arrKey = key.Split('.');
                        string key0 = arrKey[0];
                        if (key0.EndsWith("[]"))
                        {
                            object val0 = Data.ContainsKey(key0.Replace("[]", "")) ? Data[key0.Replace("[]", "")] : new List<object>();
                            object val = null;
                            List<object> list = (List<object>)val0;
                            IRange cell1 = cell;
                            string value1 = value;
                            if (list.Count > 0)
                            {
                                foreach (object o in list)
                                {
                                    val = o;
                                    for (int i = 1; i < arrKey.Count(); i++)
                                    {
                                        string s = arrKey[i];
                                        val = GetPropertyValue(val, s);
                                    }

                                    if (value == "${" + key + "}")
                                    {
                                        cell1.Formula = val;
                                        if (OtherRange != null) OtherRange.SetCellValue(cell1.Address().Replace("$", ""), val);
                                    }
                                    else
                                    {
                                        value1 = value1.Replace("${" + key + "}", val == null ? "" : val.ToString());
                                        cell1.Formula = value1;
                                        if (OtherRange != null) OtherRange.SetCellValue(cell1.Address().Replace("$", ""), value1);
                                    }
                                    cell1 = Sheet.Worksheet.Range[cell1.Row + 1, cell1.Column];
                                }
                            }
                            else
                            {
                                cell1.Formula = "";
                            }
                        }
                        else
                        {
                            object val0 = Data.ContainsKey(arrKey[0]) ? Data[arrKey[0]] : null;
                            object val = null;
                            foreach (string s in arrKey)
                            {
                                if (val == null)
                                {
                                    val = val0;
                                }
                                else
                                {
                                    val = GetPropertyValue(val, s);
                                }
                            }
                            if (value == "${" + key + "}")
                            {
                                cell.Formula = val;
                                if (OtherRange != null) OtherRange.SetCellValue(cell.Address().Replace("$", ""), val);
                            }
                            else
                            {
                                value = value.Replace("${" + key + "}", val == null ? "" : val.ToString());
                                cell.Formula = value;
                                if (OtherRange != null) OtherRange.SetCellValue(cell.Address().Replace("$", ""), value);
                            }
                        }
                    }
                }
            }

        }

        public void Merge(bool Across = false)
        {
            Range.VerticalAlignment = XlVAlign.xlVAlignCenter;
            Range.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            Range.Merge(Across);
        }

        public void MergeLeft(bool Across = false)
        {
            Range.VerticalAlignment = XlVAlign.xlVAlignCenter;
            Range.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            Range.Merge(Across);
        }

        public void FillColor(Color color)
        {
            Range.Interior.Color = color;
        }

        public void FillColor(int red, int blue, int green)
        {
            Range.Interior.Color = Color.FromArgb(red, green, blue);
        }

        public void SetBorder(VTBorderStyle borderStyle, VTBorderIndex borderIndex = VTBorderIndex.All)
        {
            if (borderIndex == VTBorderIndex.All)
            {
                Range.Borders.LineStyle = GetBorderStyle(borderStyle);
            }
            else
            {
                XlBordersIndex xlBorderIndex = GetBorderIndex(borderIndex);
                Range.Borders[xlBorderIndex].LineStyle = GetBorderStyle(borderStyle);
            }
        }

        public void SetBorder(VTBorderWeight borderWeight, VTBorderIndex borderIndex = VTBorderIndex.All)
        {
            if (borderIndex == VTBorderIndex.All)
            {
                Range.Borders.Weight = GetBorderWeight(borderWeight);
            }
            else
            {
                XlBordersIndex xlBorderIndex = GetBorderIndex(borderIndex);
                Range.Borders[xlBorderIndex].Weight = GetBorderWeight(borderWeight);
            }
        }

        public void SetBorder(VTBorderStyle borderStyle, VTBorderWeight borderWeight, VTBorderIndex borderIndex = VTBorderIndex.All)
        {
            SetBorder(borderStyle, borderIndex);
            SetBorder(borderWeight, borderIndex);
        }

        public void SetFontStyle(bool bold, Color? color, bool italic, byte? size, bool strikeThrough, bool underline)
        {
            Range.Font.Bold = bold;
            if (color.HasValue) Range.Font.Color = color.Value;
            Range.Font.Italic = italic;
            if (size.HasValue) Range.Font.Size = size.Value;
            Range.Font.Strikethrough = strikeThrough;
            Range.Font.Underline = underline ? XlUnderlineStyle.xlUnderlineStyleSingle : XlUnderlineStyle.xlUnderlineStyleNone;
        }

        public void SetFontColour(Color color)
        {
            Range.Font.Color = color;
        }

        /// <summary>
        /// changes the width of the columns in the used range to achieve the best fit
        /// </summary>
        public void AutoFit()
        {
            Range.Autofit();
        }

        /// <summary>
        ///  change the height of the rows in order to achieve the best fit
        ///  <author>Hungnd 27/11/2013</author>
        /// </summary>
        public void AutoFitRowHeight()
        {
            Range.Rows.Autofit();
        }
        // NAMDV
        public void SetHAlign(VTHAlign align)
        {
            XlHAlign _align = XlHAlign.xlHAlignLeft;
            switch (align)
            {
                case VTHAlign.xlHAlignCenter:
                    _align = XlHAlign.xlHAlignLeft;
                    break;
                case VTHAlign.xlHAlignCenterAcrossSelection:
                    _align = XlHAlign.xlHAlignCenterAcrossSelection;
                    break;
                case VTHAlign.xlHAlignDistributed:
                    _align = XlHAlign.xlHAlignDistributed;
                    break;
                case VTHAlign.xlHAlignFill:
                    _align = XlHAlign.xlHAlignFill;
                    break;
                case VTHAlign.xlHAlignGeneral:
                    _align = XlHAlign.xlHAlignGeneral;
                    break;
                case VTHAlign.xlHAlignJustify:
                    _align = XlHAlign.xlHAlignJustify;
                    break;
                case VTHAlign.xlHAlignLeft:
                    _align = XlHAlign.xlHAlignLeft;
                    break;
                case VTHAlign.xlHAlignRight:
                    _align = XlHAlign.xlHAlignRight;
                    break;
            }

            Range.HorizontalAlignment = _align;
        }

        #endregion

        #region private function
        private XlBordersIndex GetBorderIndex(VTBorderIndex borderIndex)
        {
            XlBordersIndex xlBorderIndex;
            switch (borderIndex)
            {
                case VTBorderIndex.Around: xlBorderIndex = XlBordersIndex.xlAround;
                    break;
                case VTBorderIndex.DiagonalDown: xlBorderIndex = XlBordersIndex.xlDiagonalDown;
                    break;
                case VTBorderIndex.DiagonalUp: xlBorderIndex = XlBordersIndex.xlDiagonalUp;
                    break;
                case VTBorderIndex.EdgeBottom: xlBorderIndex = XlBordersIndex.xlEdgeBottom;
                    break;
                case VTBorderIndex.EdgeLeft: xlBorderIndex = XlBordersIndex.xlEdgeLeft;
                    break;
                case VTBorderIndex.EdgeRight: xlBorderIndex = XlBordersIndex.xlEdgeRight;
                    break;
                case VTBorderIndex.EdgeTop: xlBorderIndex = XlBordersIndex.xlEdgeTop;
                    break;
                case VTBorderIndex.InsideAll: xlBorderIndex = XlBordersIndex.xlInsideAll;
                    break;
                case VTBorderIndex.InsideHorizontal: xlBorderIndex = XlBordersIndex.xlInsideHorizontal;
                    break;
                case VTBorderIndex.InsideVertical: xlBorderIndex = XlBordersIndex.xlInsideVertical;
                    break;
                default: xlBorderIndex = XlBordersIndex.xlInsideAll;
                    break;
            }
            return xlBorderIndex;
        }

        private XlLineStyle GetBorderStyle(VTBorderStyle borderStyle)
        {
            XlLineStyle lineStyle;
            switch (borderStyle)
            {
                case VTBorderStyle.Dashed: lineStyle = XlLineStyle.xlDash;
                    break;
                case VTBorderStyle.Dotted: lineStyle = XlLineStyle.xlDot;
                    break;
                case VTBorderStyle.Double: lineStyle = XlLineStyle.xlDouble;
                    break;
                case VTBorderStyle.Hidden: lineStyle = XlLineStyle.xlLineStyleNone;
                    break;
                case VTBorderStyle.Solid: lineStyle = XlLineStyle.xlContinuous;
                    break;
                default: lineStyle = XlLineStyle.xlContinuous;
                    break;
            }
            return lineStyle;
        }

        private XlBorderWeight GetBorderWeight(VTBorderWeight borderWeight)
        {
            XlBorderWeight bWeight;
            switch (borderWeight)
            {
                case VTBorderWeight.HairLine: bWeight = XlBorderWeight.xlHairline;
                    break;
                case VTBorderWeight.Thin: bWeight = XlBorderWeight.xlThin;
                    break;
                case VTBorderWeight.Medium: bWeight = XlBorderWeight.xlMedium;
                    break;
                case VTBorderWeight.Thick: bWeight = XlBorderWeight.xlThick;
                    break;
                default: bWeight = XlBorderWeight.xlUndefined;
                    break;
            }
            return bWeight;
        }
        #endregion



    }
}
