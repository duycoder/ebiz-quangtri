using Business.BaseBusiness;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Business
{
    public class QLTokenBusiness : BaseBusiness<QL_TOKEN>
    {
        public QLTokenBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public void Save(QL_TOKEN item)
        {
            try
            {
                if (item.ID == 0)
                {
                    this.repository.Insert(item);
                }
                else
                {
                    this.repository.Update(item);
                }
                this.repository.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateUserToken(long userId, string token, bool isActive)
        {
            var result = (from tk in this.context.QL_TOKEN
                          where userId == tk.DM_NGUOIDUNG_ID && tk.TOKEN.ToLower().Equals(token)
                          select tk).FirstOrDefault();
            if (result != null)
            {
                result.IS_ACTIVE = isActive;
                this.Save(result);
            }
            return false;
        }

        /// <summary>
        /// @description: kích hoạt token của người dùng
        /// @author: duynn
        /// @since: 24/04/2018
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ActiveUserToken(long userId, string token)
        {
            bool result = true;
            try
            {
                QL_TOKEN existedUserToken = this.context.QL_TOKEN.Where(x => x.DM_NGUOIDUNG_ID == userId && x.TOKEN == token).FirstOrDefault();

                //lấy thông tin những người dùng khác có cùng token
                List<QL_TOKEN> existedOtherUserToken = this.context.QL_TOKEN.Where(x => x.DM_NGUOIDUNG_ID != userId && x.TOKEN == token).ToList();
                if (existedUserToken == null)
                {
                    QL_TOKEN freshToken = new QL_TOKEN();
                    freshToken.DM_NGUOIDUNG_ID = userId;
                    freshToken.TOKEN = token;
                    freshToken.IS_ACTIVE = true;
                    freshToken.NGAYTAO = DateTime.Now;
                    this.Save(freshToken);
                }
                else
                {
                    existedUserToken.IS_ACTIVE = true;
                    this.Save(existedUserToken);
                }

                if (existedOtherUserToken.Any())
                {
                    foreach (var item in existedOtherUserToken)
                    {
                        item.IS_ACTIVE = false;
                        this.repository.Update(item);
                    }
                    this.repository.Save();
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// @description: vô hiệu hóa token của người dùng trên thiết bị hiện tại khi đăng xuất
        /// @author: duynn
        /// @since: 24/04/2018
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DeActiveUserToken(long userId, string token)
        {
            bool result = true;
            try
            {
                QL_TOKEN existedUserToken = this.context.QL_TOKEN.Where(x => x.DM_NGUOIDUNG_ID == userId && x.TOKEN == token).FirstOrDefault();
                if (existedUserToken != null)
                {
                    existedUserToken.IS_ACTIVE = false;
                    this.Save(existedUserToken);
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// @description: lấy các token đang kích hoạt của người dùng
        /// @author: duynn
        /// @since: 24/04/2018
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<string> GetActiveTokensByUserId(long userId)
        {
            List<string> result = this.context.QL_TOKEN
                .Where(x => x.DM_NGUOIDUNG_ID == userId && x.IS_ACTIVE == true && string.IsNullOrEmpty(x.TOKEN) == false)
                .Select(x => x.TOKEN).ToList();
            return result;
        }

        /// <summary>
        /// @description: lấy các token đang kích hoạt của người dùng
        /// @author: duynn
        /// @since: 21/05/2018
        /// </summary>
        /// <param name="groupUserdIds"></param>
        /// <returns></returns>
        public List<string> GetActiveTokenByGroupUserIds(List<long> groupUserdIds)
        {
            List<string> result = new List<string>();
            if (groupUserdIds != null && groupUserdIds.Count > 0)
            {
                foreach(var item in groupUserdIds)
                {
                    result.AddRange(this.GetActiveTokensByUserId(item));
                }
            }
            return result;
        }
    }
}
