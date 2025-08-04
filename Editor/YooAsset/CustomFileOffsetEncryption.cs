using System;
using System.IO;
using JQCore;
using JQCore.tString;
using JQCore.tUtil;

namespace YooAsset.Editor
{
    public class CustomFileOffsetEncryption : IEncryptionServices
    {
        private static Random random = new Random();

        public EncryptResult Encrypt(EncryptFileInfo fileInfo)
        {
            int offset = PathUtil.EncryptHeadLen;
            string crc = HashUtility.FileCRC32(fileInfo.FilePath);
            byte[] crcByte = crc.ToByteArray();
            int copyLen = Math.Min(crcByte.Length, offset);
            
            byte[] fileData = File.ReadAllBytes(fileInfo.FilePath);
            var encryptedData = new byte[fileData.Length + offset];
            Buffer.BlockCopy(crcByte, 0, encryptedData, 0, copyLen);
            Buffer.BlockCopy(fileData, 0, encryptedData, offset, fileData.Length);

            EncryptResult result = new EncryptResult();
            result.LoadMethod = EBundleLoadMethod.LoadFromFileOffset;
            result.EncryptedData = encryptedData;
            return result;
        }

        private static byte[] getRandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            random.NextBytes(bytes);
            return bytes;
        }
    }
}