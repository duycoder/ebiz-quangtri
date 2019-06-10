using Business.Business;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Business.CommonBusiness;

namespace Web.Common
{
    public class EmailProvider
    {
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private static string ACCOUNT_MAIL = ConfigurationManager.AppSettings["ACCOUNT_MAIL"];
        private static string PASS_MAIL = ConfigurationManager.AppSettings["PASS_MAIL"];
        public static void sendEmail(string body, string subject, List<string> address)
        {
            SmtpClient server = new SmtpClient();
            try
            {
                //string from = ACCOUNT_MAIL;
                string from = ACCOUNT_MAIL;
                string pass = PASS_MAIL;

                //server.Host = "smtp.gmail.com";
                server.Host = "mail.tinhuyquangtri.vn";
                //server.Host = "mail.doji.vn";
                //server.Port = 587;                
                server.Port = 25;                
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                foreach (var item in address)
                {
                    mail.To.Add(item);
                }
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                server.Credentials = new NetworkCredential(from, pass);
                //server.EnableSsl = true;
                server.Send(mail);
            }
            catch(Exception ex)
            {
            }
        }
        public static void SendMailTemplate(UserInfoBO currentUser, string body, string subject, List<string> address)
        {
            var emailtemplate_path = Path.Combine(ConfigurationManager.AppSettings["FileUpload"], ConfigurationManager.AppSettings["temple_CanhCao_VB"]);
            if(!string.IsNullOrEmpty(emailtemplate_path) && File.Exists(emailtemplate_path))
            {
                var strEmail = File.ReadAllText(emailtemplate_path);
                strEmail = strEmail.Replace("[[Email]]", currentUser.EMAIL);
                strEmail = strEmail.Replace("[[HoTen]]", currentUser.HOTEN);
                strEmail = strEmail.Replace("[[Title]]", subject);
                strEmail = strEmail.Replace("[[Subtitle]]", subject);
                var lstViec = "";
                lstViec += "<tr class='item-lst'><td class='img-lst'><img src='http://sv1.upsieutoc.com/2017/11/22/icon-document.png'></td>";
                lstViec += body;
                lstViec += "</tr>";
                strEmail = strEmail.Replace("[[contentlst]]", lstViec);
                sendEmail(strEmail, subject, address);
            }
        }
    }
}