using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

namespace VTUtils.Excel
{
    public class VTComment
    {
        public HSSFComment Comment { get; set; }
        public VTComment() 
        {
            //Comment = new HSSFComment(new HSSFClientShape(), new HSSFClientAnchor());
          
           
        }
        public VTComment(HSSFComment comment)
        {
            Comment = comment;
        }

        public VTComment(ref HSSFWorkbook workbook, string sheetName)
        {
            IDrawing patr = (HSSFPatriarch)workbook.GetSheet(sheetName).CreateDrawingPatriarch();
            Comment = (HSSFComment)patr.CreateCellComment(new HSSFClientAnchor());
        }

        public void SetAuthor(string author)
        {
            Comment.Author = author;
        }

        public void SetColumn(int col)
        {
            Comment.Column = col;
        }

        public void SetRow(int row)
        {
            Comment.Row = row;
        }

        public void SetText(string text)
        {
            Comment.String = new HSSFRichTextString(text) ;
        }

        public void SetVisible(bool visible)
        {
            Comment.Visible = visible;
        }

        public void SetHorizontalAlignment(short horizontalAlignment)
        {
            Comment.HorizontalAlignment = horizontalAlignment;
        }

        public void SetVerticalAlignment(short verticalAlignment)
        {
            Comment.VerticalAlignment = verticalAlignment;
        }

        public void SetMarginBottom(int marginBottom)
        {
            Comment.MarginBottom = marginBottom;
        }

        public void SetMarginLeft(int marginLeft)
        {
            Comment.MarginLeft = marginLeft;
        }

        public void SetMarginRight(int marginRight)
        {
            Comment.MarginRight = marginRight;
        }

        public void SetMarginTop(int marginTop)
        {
            Comment.MarginTop = marginTop;            
        }
        
        //shape
        public void SetFillColor(int red, int green, int blue)
        {
            Comment.SetFillColor(red, green, blue);
        }

        public void SetLineStyleColor(int lineStyleColor)
        {
            Comment.SetLineStyleColor(lineStyleColor);
        }

        public void SetLineStyleColor(int red, int green, int blue)
        {
            Comment.SetLineStyleColor(red, green, blue);
        }

        public void SetLineWidth(int lineWidth)
        {
            Comment.LineWidth = lineWidth;
        }

        public void SetFillColor(int fillColor)
        {
            Comment.FillColor = fillColor;
        }

        public void SetIsNoFill(bool isNoFill)
        {
            Comment.IsNoFill = isNoFill;
        }

        public void SetLineStyle(int lineStyle)
        {
            //None = -1,
            //Solid = 0,
            //DashSys = 1,
            //DotSys = 2,
            //DashDotSys = 3,
            //DashDotDotSys = 4,
            //DotGel = 5,
            //DashGel = 6,
            //LongDashGel = 7,
            //DashDotGel = 8,
            //LongDashDotGel = 9,
            //LongDashDotDotGel = 10,

            switch(lineStyle)
            {               
                case 0:
                    Comment.LineStyle = LineStyle.Solid;
                    break;
                case 2:
                    Comment.LineStyle = LineStyle.DotSys;
                    break;
                case 3:
                    Comment.LineStyle = LineStyle.DashDotSys;
                    break;
                case 4:
                    Comment.LineStyle = LineStyle.DashDotDotSys;
                    break;
                case 5:
                    Comment.LineStyle = LineStyle.DotGel;
                    break;
                case 6:
                    Comment.LineStyle = LineStyle.DashGel;
                    break;
                case 7:
                    Comment.LineStyle = LineStyle.LongDashGel;
                    break;
                case 8:
                    Comment.LineStyle = LineStyle.DashDotGel;
                    break;
                case 9:
                    Comment.LineStyle = LineStyle.LongDashDotGel;
                    break;
                case 10:
                    Comment.LineStyle = LineStyle.LongDashDotDotGel;
                    break;
                default:
                    Comment.LineStyle = LineStyle.None;
                    break;

            }
            
        }

        //
        // Summary:
        //     Sets the top-left and bottom-right coordinates
        //     of the anchor.
        //
        // Parameters:
        //   dx1:
        //     the x coordinate within the first cell.
        //
        //   dy1:
        //     the y coordinate within the first cell.
        //
        //   dx2:
        //     the x coordinate within the second cell.
        //
        //   dy2:
        //     the y coordinate within the second cell.
        //
        //   col1:
        //     the column (0 based) of the first cell.
        //
        //   row1:
        //     the row (0 based) of the first cell.
        //
        //   col2:
        //     the column (0 based) of the second cell.
        //
        //   row2:
        //     the row (0 based) of the second cell.
        public void SetAnchor(int dx1, int dy1, int dx2, int dy2, int col1, int row1, int col2, int row2)
        {
            Comment.Anchor = new HSSFClientAnchor(dx1, dy1, dx2, dy2, col1, row1, col2, row2);
        }
      
    }
}
