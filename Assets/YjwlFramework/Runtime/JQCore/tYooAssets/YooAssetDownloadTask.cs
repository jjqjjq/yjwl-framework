using System;
using JQCore.tEvent;
using JQCore.tLog;
using JQCore.tString;
using YooAsset;

namespace JQCore.tYooAssets
{
    public class YooAssetDownloadTask : JQEventDispatcher
    {
        private const string FINISH = "YooAssetDownloadFinish";
        private const string PROCESS = "YooAssetDownloadProcess";
        private string _assetTag;
        private ResourceDownloaderOperation _downloaderOperation;

        private bool _isDownloading;
        private bool _isTag;

        private string _shortAssetPath;

        public YooAssetDownloadTask(string value, bool isTag)
        {
            var downloadingMaxNum = 10;
            var failedTryAgain = 3;
            _isTag = isTag;
            if (isTag)
            {
                _assetTag = value;
                _shortAssetPath = null;
                if (string.IsNullOrEmpty(_assetTag))
                {
                    JQLog.LogError("isTag为true时，assetTag不能为空");
                    return;
                }

                _downloaderOperation = YooAssetMgr.Package.CreateResourceDownloader(_assetTag, downloadingMaxNum, failedTryAgain);
            }
            else
            {
                _assetTag = null;
                _shortAssetPath = value;
                if (string.IsNullOrEmpty(_shortAssetPath))
                {
                    JQLog.LogError("isTag为false时，shortAssetPath不能为空");
                    return;
                }

                var fullRelativePath = YooAssetMgr.LongAssetPath(_shortAssetPath);
                _downloaderOperation = YooAssetMgr.Package.CreateBundleDownloader(fullRelativePath, downloadingMaxNum, failedTryAgain);
            }

            TotalDownloadBytes = _downloaderOperation.TotalDownloadBytes;
            JQLog.Log($"创建下载任务{_assetTag} {_shortAssetPath} {_downloaderOperation}");
        }

        public long CurrentDownloadBytes { get; private set; }

        public long TotalDownloadBytes { get; private set; }

        public void Dispose()
        {
            JQLog.Log($"销毁下载任务{_assetTag} {_shortAssetPath}");
            _shortAssetPath = null;
            _assetTag = null;
            if (_downloaderOperation != null)
            {
                _downloaderOperation.CancelDownload();
                _downloaderOperation = null;
            }

            EventDispose();
        }

        public bool isSame(string value, bool isTag)
        {
            _isTag = isTag;
            if (isTag)
            {
                if (_assetTag == null) return false;

                return _assetTag == value;
            }

            if (_shortAssetPath == null) return false;

            return _shortAssetPath == value;
        }

        public void AddEvent(Action<bool> onDownloadOver, Action<int, int, long, long> onProgress = null)
        {
            AddEventListener(FINISH, onDownloadOver);
            if (onProgress != null) AddEventListener(PROCESS, onProgress);
        }


        public void StartDownload()
        {
            //没有需要下载的资源
            if (_downloaderOperation.TotalDownloadCount == 0)
            {
                TriggerEvent(FINISH, true);
                return;
            }

            if (_isDownloading) return;

            //需要下载的文件总数和总大小
            var totalDownloadCount = _downloaderOperation.TotalDownloadCount;
            var totalDownloadBytes = _downloaderOperation.TotalDownloadBytes;
            JQLog.Log($"totalDownloadCount:{totalDownloadCount}, totalDownloadBytes:{StringUtil.GetByteString(totalDownloadBytes)}");

            //注册回调方法
            _downloaderOperation.OnDownloadErrorCallback = OnDownloadErrorFunction;
            _downloaderOperation.OnStartDownloadFileCallback = OnStartDownloadFileFunction;
            _downloaderOperation.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
            _downloaderOperation.OnDownloadOverCallback = OnDownloadOverFunction;

            //开启下载
            _downloaderOperation.BeginDownload();
            _isDownloading = true;
        }

        private void OnDownloadErrorFunction(string fileName, string error)
        {
            JQLog.LogError($"下载失败：{fileName}，{error}");
            _isDownloading = false;
        }

        private void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            JQLog.Log($"下载进度：{_assetTag} {_shortAssetPath} {currentDownloadCount}/{totalDownloadCount}，{StringUtil.GetByteString(currentDownloadBytes)}/{StringUtil.GetByteString(totalDownloadBytes)}");
            CurrentDownloadBytes = currentDownloadBytes;
            TotalDownloadBytes = totalDownloadBytes;
            TriggerEvent(PROCESS, _downloaderOperation.TotalDownloadCount, _downloaderOperation.CurrentDownloadCount, _downloaderOperation.TotalDownloadBytes, _downloaderOperation.CurrentDownloadBytes);
        }

        private void OnDownloadOverFunction(bool isSucceed)
        {
            JQLog.Log($"下载完成：{_assetTag} {_shortAssetPath}");
            _isDownloading = false;
            TriggerEvent(FINISH, isSucceed);
            YooAssetDownloadMgr.Instance.RemoveTask(this);
        }

        private void OnStartDownloadFileFunction(string fileName, long sizeBytes)
        {
            JQLog.Log($"开始下载：{fileName}，{sizeBytes}");
        }

        public float GetProgress()
        {
            return CurrentDownloadBytes / (float)TotalDownloadBytes;
        }
    }
}