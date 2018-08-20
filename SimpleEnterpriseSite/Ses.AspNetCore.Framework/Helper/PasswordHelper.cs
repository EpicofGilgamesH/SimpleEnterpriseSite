
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Framework.Helper
{
    public static class PasswordHelper
    {
        public static string GetMD5(string str)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                // 默认为utf-8编码
                var inputBytes = Encoding.UTF8.GetBytes(str);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var hashByte in hashBytes)
                {
                    sb.Append(hashByte.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 通过MD5加密过的密文加密
        /// </summary>
        /// <param name="md5Password"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static string EncryptMD5Password(string md5Password, string secretKey)
        {
            secretKey = GetMD5(secretKey).Substring(0, 16);
            string encryptedPassword = EncryptHelper.AESEncrypt(md5Password.ToLower(), secretKey).ToLower();
            string ret = GetMD5(encryptedPassword).ToLower();
            return ret;
        }

        /// <summary>
        /// 明文密码加密
        /// </summary>
        /// <param name="password"></param>
        /// <param name="secretKye"></param>
        /// <returns></returns>
        public static string EncryptPassword(string password, string secretKye)
        {
            string md5Password = GetMD5(password);
            return EncryptMD5Password(md5Password, secretKye);
        }

        /// <summary>
        /// 密码长度验证
        /// </summary>
        /// <param name="password"></param>
        public static void ValidatePassword(string password)
        {
            if (password == null || password.Length < 6 || password.Length > 15)
                throw new Exception("密码必须是6-15位");
        }

        /// <summary>
        /// 生成SecretKey
        /// </summary>
        /// <returns></returns>
        public static string NewSecretKey()
        {
            return GetMD5(NewNo()).Substring(8, 16).ToLower();
        }

        public static string NewNo()
        {
            Random random = new Random();
            string strRandom = random.Next(1000, 10000).ToString();
            return DateTime.Now.ToString("yyyyMMddHHmmss") + strRandom;
        }
    }
}
