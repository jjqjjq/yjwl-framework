using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace JQCore.tString
{
    public static class StringUtil
    {
        //输入一个byte数，放回一个带单位的string
        public static string GetByteString(long byteCount)
        {
            if (byteCount < 1024)
                return $"{byteCount}B";
            if (byteCount < 1024 * 1024)
                return $"{byteCount / 1024f:F2}KB";
            if (byteCount < 1024 * 1024 * 1024)
                return $"{byteCount / 1024f / 1024f:F2}MB";
            return $"{byteCount / 1024f / 1024f / 1024f:F2}GB";
        }

        public static IEnumerable<byte> ToBytes(this string str)
        {
            var byteArray = Encoding.Default.GetBytes(str);
            return byteArray;
        }

        public static byte[] ToByteArray(this string str)
        {
            var byteArray = Encoding.Default.GetBytes(str);
            return byteArray;
        }
        
        
        /// <summary>
        /// "4322" -》 int[]{4,3,2,2}
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int[] NumberStringToIntArr(string str)
        {
            return str.Select(c => c - '0').ToArray();
        }

        public static byte[] ToUtf8(this string str)
        {
            var byteArray = Encoding.UTF8.GetBytes(str);
            return byteArray;
        }

        public static byte[] HexToBytes(this string hexString)
        {
            if (hexString.Length % 2 != 0)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "The binary key cannot have an odd number of digits: {0}", hexString));

            var hexAsBytes = new byte[hexString.Length / 2];
            for (var index = 0; index < hexAsBytes.Length; index++)
            {
                var byteValue = "";
                byteValue += hexString[index * 2];
                byteValue += hexString[index * 2 + 1];
                hexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return hexAsBytes;
        }

        public static string Fmt(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static string ListToString<T>(this List<T> list)
        {
            var sb = new StringBuilder();
            foreach (var t in list)
            {
                sb.Append(t);
                sb.Append(",");
            }

            return sb.ToString();
        }

        public static string ArrayToString<T>(this T[] args)
        {
            if (args == null) return "";

            var argStr = " [";
            for (var arrIndex = 0; arrIndex < args.Length; arrIndex++)
            {
                argStr += args[arrIndex];
                if (arrIndex != args.Length - 1) argStr += ", ";
            }

            argStr += "]";
            return argStr;
        }

        public static string ArrayToString<T>(this T[] args, int index, int count)
        {
            if (args == null) return "";

            var argStr = " [";
            for (var arrIndex = index; arrIndex < count + index; arrIndex++)
            {
                argStr += args[arrIndex];
                if (arrIndex != args.Length - 1) argStr += ", ";
            }

            argStr += "]";
            return argStr;
        }

        public static int[] StringToIntArray(string str, char splitChar = ';')
        {
            var strArr = str.Split(splitChar);
            var intArr = new int[strArr.Length];
            for (var i = 0; i < strArr.Length; i++)
            {
                intArr[i] = int.Parse(strArr[i]);
            }

            return intArr;
        }

        public static float[] StringToFloatArray(string str, char splitChar = ';')
        {
            var strArr = str.Split(splitChar);
            var intArr = new float[strArr.Length];
            for (var i = 0; i < strArr.Length; i++)
            {
                intArr[i] = float.Parse(strArr[i]);
            }

            return intArr;
        }
        
        /// <summary>
        /// 提取结尾数字 Map_001 取出1
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ExtractEndNumber(string str)
        {
            var match = Regex.Match(str, @"(\d+)$");
            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            else
            {
                // 没有找到数字，可以返回-1或抛出异常
                return -1;
            }
        }

        #region 加解密文本

        //写一个字符串加密函数
        //默认密钥向量
        //随机另外一个Keys，其中的数字是随便的
        private static byte[] KEYS = { 0xF5, 0x2C, 0x9A, 0x4B, 0x33, 0xBE, 0x1D, 0x88 };
        private static string ENCRYPTKEY = "h6kd8vbh"; //加密密钥,要求为8位

        /**/ /**/ /**/ /**/ /**/ /**/ /**/
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(ENCRYPTKEY);
                byte[] rgbIV = KEYS;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey,
                    rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /**/ /**/ /**/ /**/ /**/ /**/ /**/
        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(ENCRYPTKEY);
                byte[] rgbIV = KEYS;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey,
                    rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }

        #endregion
    }
}