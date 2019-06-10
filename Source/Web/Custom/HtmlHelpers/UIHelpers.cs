using Business.Business;
using Business.CommonBusiness;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Web.Common;

namespace Web.Custom.HtmlHelpers
{
    public static class UIHelpers
    {
        public static MvcHtmlString InputFile(this HtmlHelper html, string name, int? currentFileID = 0, string currentFileName = "", bool multiple = false, string onChange = "")
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("<div style='width: 188px;overflow:hidden;float: left; border: 1px solid #c0c0c0; padding: 7px;height:20px;' class='selected-file' style='{3}' id='selected-file-{1}'>{2}</div>");
            string str = currentFileID != null && currentFileID != 0 && currentFileName != "" ? GlobalCommon.GetDownloadLink(currentFileID, currentFileName).ToString() : "";
            string show = currentFileID != null && currentFileID != 0 && currentFileName != "" ? "" : "display:none";
            result.AppendLine("<div id='button-{0}'><input type='button' value='Chọn file...' onclick='$(\"#input-file-{1}\").click()'/></div>");
            result.AppendLine("<input {4} type='file' class='vt-input-file' id='input-file-{1}' name='{0}' style='display:none'/>");
            if (!multiple)
            {
                result.AppendLine("<script type=\"text/javascript\">$(document).ready(function () {{$('#input-file-{1}').change(function (){{$('#selected-file-{1}').html('<span>'+$(this).val()+'</span>');$('#selected-file-{1}').show(); " + onChange + "}})}});</script>");
            }
            else
            {
                result.AppendLine("<script type=\"text/javascript\">$(document).ready(function () {{$('#input-file-{1}').change(function (){{ 	filesStr = ''; 	for (i = 0; i < this.files.length; i++) {{         filesStr += '<span>'+this.files[i].name+'<br />'+'</span>';     }} 	$('#selected-file-{1}').html('<span>'+filesStr+'</span'); 	$('#selected-file-{1}').show(); " + onChange + " }})}});</script>");
            }
            return MvcHtmlString.Create(string.Format(result.ToString(), name, Guid.NewGuid(), str, show, multiple ? "multiple" : ""));
        }

        public static MvcHtmlString GridRowCount(this HtmlHelper html, WebGrid grid)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("<div class='grid-row-count'>");
            result.AppendLine(string.Format("{0} bản ghi", grid.Rows.Count));
            result.AppendLine("</div>");
            return MvcHtmlString.Create(result.ToString());
        }
    }
}