using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Business.CommonHelper
{
    public class Encrypt
    {
        public static string MD5(string s)
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(s);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes).ToLower().Replace("-", "");
        }

        public static string EncryptMark(decimal input, string key)
        {
            key = key + "skfhwlfssdl28397#23794";
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            string Password = key;
            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(input.ToString());
            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
            //Creates a symmetric encryptor object.
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream();
            //Defines a stream that links data streams to cryptographic transformations
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(PlainText, 0, PlainText.Length);
            //Writes the final state and clears the buffer
            cryptoStream.FlushFinalBlock();
            byte[] CipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string EncryptedData = Convert.ToBase64String(CipherBytes);
            return EncryptedData;
        }

        public static decimal DecryptMark(string input, string key)
        {
            key = key + "skfhwlfssdl28397#23794";
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            string Password = key;
            byte[] EncryptedData = Convert.FromBase64String(input);
            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
            //Making of the key for decryption
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
            //Creates a symmetric Rijndael decryptor object.
            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream(EncryptedData);
            //Defines the cryptographics stream for decryption.THe stream contains decrpted data
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
            byte[] PlainText = new byte[EncryptedData.Length];
            int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
            memoryStream.Close();
            cryptoStream.Close();


            string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
            return decimal.Parse(DecryptedData);
        }
    }
}
