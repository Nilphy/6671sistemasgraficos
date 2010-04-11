using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Common
{
    public static class EncryptionUtil
    {
        public static string GetMD5(string password)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            byte[] dataMd5 = md5.ComputeHash(Encoding.Default.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dataMd5.Length; i++)
                sb.AppendFormat("{0:x2}", dataMd5[i]);
            return sb.ToString();
        }
    }
}
