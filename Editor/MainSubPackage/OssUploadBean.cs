using System;
using Aliyun.OSS;
using JQCore.tLog;
using JQCore.tString;
using UnityEngine;

namespace JQEditor.MainSubPackage
{
    public class OssUploadBean
    {
        private string _ossPath;
        private string _localPath;
        private long _fileSize;
        public long TransferredBytes;
        public long TotalBytes;
        private Action<long> _onProgressCallback;
        private int _repeactCount = 0;//重试次数
        private AliyunOSSMgr _aliyunOssMgr;

        public string OssPath
        {
            get { return _ossPath; }
        }

        public string CdnPath
        {
            get{return _ossPath.Replace(" ", "%20");}
        }
        
        public long FileSize
        {
            get { return _fileSize; }
        }

        public OssUploadBean(long fileSize, string ossPath, string localPath)
        {
            _fileSize = fileSize;
            // _ossPath = $"CDN/{PathUtil.platformName}/v{BuildAppInfo.version}/{_fileName}";
            _ossPath = ossPath;
            _localPath = localPath;
        }

        public void Upload(AliyunOSSMgr aliyunOssMgr, Action<long> onProgressCallback)
        {
            _repeactCount = 0;
            _aliyunOssMgr = aliyunOssMgr;
            _onProgressCallback = onProgressCallback;
            _aliyunOssMgr.Upload(_ossPath, _localPath, streamProgressCallback, onError);
        }

        private void onError()
        {
            _repeactCount++;
            if(_repeactCount >= 5)
            {
                JQLog.LogError($"上传失败:{_localPath} => {_ossPath}");
                return;
            }
            JQLog.Log($"上传失败，重试:{_repeactCount}  {_localPath}");
            _aliyunOssMgr.Upload(_ossPath, _localPath, streamProgressCallback, onError);
        }

        private void streamProgressCallback(object sender, StreamTransferProgressArgs args)
        {
            if (args.TotalBytes != _fileSize)
            {
                JQLog.LogError($"文件大小不对:{_localPath}");
            }

            TransferredBytes = args.TransferredBytes;
            TotalBytes = args.TotalBytes;
            // Debug.Log($"上传进度:{TransferredBytes}/{TotalBytes}  {_fileName}");
            _onProgressCallback?.Invoke(args.IncrementTransferred);

            if (TransferredBytes >= TotalBytes)
            {
                Debug.Log($"上传完毕: {_localPath} => {_ossPath}  大小：{StringUtil.GetByteString(_fileSize)}");
            }
            
            // HyDebug.Log($"ProgressCallback - TotalBytes:{args.TotalBytes}, TransferredBytes:{args.TransferredBytes}, IncrementTransferred:{args.IncrementTransferred}");
        }
    }
}