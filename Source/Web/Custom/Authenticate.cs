using Business.BaseBusiness;
using Business.Business;
using Business.CommonBusiness;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Web.FwCore;
using Web.FwCore.Factory;

namespace Web.Custom
{
    /// <summary>
    /// NAMDV - 24/01/2014
    /// </summary>
    public class Authenticate
    {
        private static readonly ILog log = LogManager.GetLogger("Authenticate");

        public Authenticate() { }


        public static bool ValidateLogin(string UserName, string Password)
        {
            //if (DateTime.Now.Year == 2017)
            //{
            //    if (DateTime.Now.Month >= 9 || DateTime.Now.Month < 3)
            //    {
            //        return false;
            //    }
            //}
            //else
            //{
            //    return false;
            //}
            //var MachineName = Environment.MachineName;
            //if (MachineName != "DELL-PC")
            //{
            //    return false;
            //}
            DBEntities context = new DBEntities();
            try
            {
                DmNguoidungBusiness bus = new DmNguoidungBusiness(new UnitOfWork());
                //UserInfoBO UserInfo = bus.GetUserInfo(UserName, CAPDONVI, TINH_ID, HUYEN_ID, xaid, DONVITW_ID);
                UserInfoBO UserInfo = bus.GetUserInfo(UserName);
                if (UserInfo.UserID <= 0)
                {
                    return false;
                }
                string pass = VtEncodeData.Encode_Data(Password + UserInfo.PasswordSalt);
                //string pass = VtCryptography.Encrypt(Password + UserInfo.PasswordSalt, UserInfo.PasswordSalt);

                if (UserInfo.Password == pass)///////////
                {
                    SessionManager.SetValue(SessionManager.USER_INFO, UserInfo);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + "\n" + ex.StackTrace);
                return false;
            }

        }
        public static void LoadUserInfo(string UserName)
        {
            DBEntities context = new DBEntities();
            DmNguoidungBusiness bus = new DmNguoidungBusiness(new UnitOfWork());
            UserInfoBO UserInfo = bus.GetUserInfo(UserName);
            if (UserInfo.LastLoginDate == null)
            {
                DM_NGUOIDUNG user = bus.repository.Find(UserInfo.UserID);
                //user.LAST_LOGIN_DATE = DateTime.Now;
                bus.Save(user);
            }
            SessionManager.SetValue(SessionManager.USER_INFO, UserInfo);
        }
        //public static void LoadUserInfo(string UserName, string Code)
        //{
        //    Entities context = new Entities();
        //    DmNguoidungBusiness bus = new DmNguoidungBusiness(context);
        //    UserInfoBO UserInfo = bus.GetUserInfo(UserName, Code);
        //    //if (UserInfo.LastLoginDate == null) 
        //    //{
        //    //    DM_NGUOIDUNG user = bus.Find(UserInfo.UserID);
        //    //    //user.LAST_LOGIN_DATE = DateTime.Now;
        //    //    bus.Update(user);
        //    //    bus.Save();
        //    //}
        //    SessionManager.SetValue(SessionManager.USER_INFO, UserInfo);
        //}

    }
}