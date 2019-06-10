using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public class MaHoaMatKhau
    {
        public static string GenerateRandomString(int length)
        {
            //It will generate string with combination of small,capital letters and numbers
            char[] charArr = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
            string randomString = string.Empty;
            Random objRandom = new Random();
            for (int i = 0; i < length; i++)
            {
                //Don't Allow Repetation of Characters
                int x = objRandom.Next(1, charArr.Length);
                if (!randomString.Contains(charArr.GetValue(x).ToString()))
                    randomString += charArr.GetValue(x);
                else
                    i--;
            }
            return randomString;
        }
        #region MD5_Encode
        public static string MD5_Encode(string str_endcode)
        {
            MD5 md5Hash = new MD5CryptoServiceProvider();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(str_endcode));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        #endregion

        #region SHA1_Encode
        public static string SHA1_Encode(string str_endcode)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(str_endcode));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sBuilder.Append(hash[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
        #endregion

        #region
        public static string Encode_Data(string str_endcode)
        {
            return MD5_Encode(SHA1_Encode(str_endcode));
        }
        #endregion
    }
}
