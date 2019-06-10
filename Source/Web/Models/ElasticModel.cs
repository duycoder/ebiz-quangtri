using Model.Entities;
using System.Collections.Generic;
using CommonHelper;
using System.Web;
using System.Web.Configuration;

/// <summary>
/// Author: David Luan
/// </summary>
namespace Web.Models
{
    public class ElasticModel
    {
        /// <summary>
        /// Số hiệu hoặc tên công việc
        /// </summary>
        public string SoHieu { get; set; }
        public string SoHieuNoSign { get; set; }
        public string NoiDung { get; set; }
        public string NoiDungNoSign { get; set; }
        public string TrichYeu { get; set; }
        public string TrichYeuNoSign { get; set; }
        /// <summary>
        /// Người ký hoặc người nhận việc
        /// </summary>
        public string NguoiKy { get; set; }
        public string NguoiKyNoSign { get; set; }
        public string Url { get; set; }
        public long Id { get; set; }
        public List<long> ListUser { get; set; }
        public int Type { get; set; }
        public static ElasticModel ConvertJob(HSCV_CONGVIEC CongViec, List<long> ListUser, string NGUOIGIAOVIEC)
        {
            ElasticModel elasticModel = new ElasticModel();
            elasticModel.Id = CongViec.ID;
            elasticModel.ListUser = ListUser;
            elasticModel.NguoiKy = NGUOIGIAOVIEC;
            elasticModel.NguoiKyNoSign = NGUOIGIAOVIEC.ConvertToVN();
            elasticModel.NoiDung = HttpUtility.HtmlDecode(CongViec.NOIDUNGCONGVIEC).Trim().RemoveHtml();
            elasticModel.NoiDungNoSign = HttpUtility.HtmlDecode(CongViec.NOIDUNGCONGVIEC.ConvertToVN()).Trim().RemoveHtml();
            elasticModel.SoHieu = CongViec.TENCONGVIEC;
            elasticModel.SoHieuNoSign = CongViec.TENCONGVIEC.ConvertToVN();
            elasticModel.TrichYeu = CongViec.TENCONGVIEC;
            elasticModel.TrichYeuNoSign = CongViec.TENCONGVIEC.ConvertToVN();
            elasticModel.Type = 1;
            elasticModel.Url = "/QuanLyCongViec/QuanLyCongViec/Detail/" + CongViec.ID;
            return elasticModel;
        }

        public static ElasticModel ConvertVanBan(HSCV_VANBANDI VanBan, List<long> ListUser, string NGUOIKY)
        {
            ElasticModel elasticModel = new ElasticModel();
            string EnableElasticServer = WebConfigurationManager.AppSettings["EnableElasticServer"];
            if(EnableElasticServer == "0")
            {
                return elasticModel;
            }
            
            elasticModel.Id = VanBan.ID;
            elasticModel.ListUser = ListUser;
            elasticModel.NguoiKy = NGUOIKY;
            elasticModel.NguoiKyNoSign = NGUOIKY.ConvertToVN();
            if (!string.IsNullOrEmpty(VanBan.NOIDUNG))
            {
                elasticModel.NoiDung = HttpUtility.HtmlDecode(VanBan.NOIDUNG).Trim().RemoveHtml();
                elasticModel.NoiDungNoSign = HttpUtility.HtmlDecode(VanBan.NOIDUNG.ConvertToVN()).Trim().RemoveHtml();
            }else
            {
                elasticModel.NoiDung = string.Empty;
                elasticModel.NoiDungNoSign = string.Empty;
            }
            
            elasticModel.SoHieu = VanBan.SOHIEU;
            elasticModel.SoHieuNoSign = VanBan.SOHIEU.ConvertToVN();
            elasticModel.TrichYeu = VanBan.TRICHYEU;
            elasticModel.TrichYeuNoSign = VanBan.TRICHYEU.ConvertToVN();
            elasticModel.Type = 2;
            elasticModel.Url = "/HSVanBanDiArea/HSVanBanDi/DetailVanBan/" + VanBan.ID;
            return elasticModel;
        }
        public static ElasticModel ConvertVanBanDen(HSCV_VANBANDEN VanBan, List<long> ListUser)
        {
            ElasticModel elasticModel = new ElasticModel();
            elasticModel.Id = VanBan.ID;
            elasticModel.ListUser = ListUser;
            elasticModel.NguoiKy = VanBan.CHUCVU + " " +VanBan.NGUOIKY;
            elasticModel.NguoiKyNoSign = elasticModel.NguoiKy.ConvertToVN();
            if (!string.IsNullOrEmpty(VanBan.NOIDUNG))
            {
                elasticModel.NoiDung = HttpUtility.HtmlDecode(VanBan.NOIDUNG).Trim().RemoveHtml();
                elasticModel.NoiDungNoSign = HttpUtility.HtmlDecode(VanBan.NOIDUNG.ConvertToVN()).Trim().RemoveHtml();
            }
            else
            {
                elasticModel.NoiDung = string.Empty;
                elasticModel.NoiDungNoSign = string.Empty;
            }
            
            elasticModel.SoHieu = VanBan.SOHIEU;
            elasticModel.SoHieuNoSign = VanBan.SOHIEU.ConvertToVN();
            elasticModel.TrichYeu = VanBan.TRICHYEU;
            elasticModel.TrichYeuNoSign = VanBan.TRICHYEU.ConvertToVN();
            elasticModel.Type = 1;
            elasticModel.Url = "/HSCV_VANBANDENArea/HSCV_VANBANDEN/DetailVanBanDen?id=" + VanBan.ID;
            return elasticModel;
        }
    }
}