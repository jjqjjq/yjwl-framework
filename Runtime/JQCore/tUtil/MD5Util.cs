using System;
using System.IO;
using System.Security.Cryptography;
using JQCore.tLog;

namespace JQCore.tUtil
{

    public class Md5Util
    {
        public static string FormatMd5(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "").ToLower();
        }

        public static string BuildFileMd5(string filename)
        {
            string result = null;
            try
            {
                using (FileStream fileStream = File.OpenRead(filename))
                {
                    MD5 mD = MD5.Create();
                    byte[] data = mD.ComputeHash(fileStream);
                    result = FormatMd5(data);
                }
            }
            catch (Exception ex)
            {
                JQLog.LogError(ex);
            }
            return result;
        }

        public static byte[] CreateMd5(byte[] data)
        {
            byte[] result;
            using (MD5 mD = MD5.Create())
            {
                result = mD.ComputeHash(data);
            }
            return result;
        }

        public static string CreateMd5(string str)
        {
            string result;
            using (MD5 mD = MD5.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
                bytes = mD.ComputeHash(bytes);
                result = FormatMd5(bytes);
            }
            return result;
        }
    }
}
