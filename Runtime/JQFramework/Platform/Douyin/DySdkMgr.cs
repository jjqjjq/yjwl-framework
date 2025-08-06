using System;
using System.Threading.Tasks;
using ET.ETFramework.Model.Client.Game;
using JQCore;
using JQCore.tCfg;
using JQCore.tLog;
#if SDK_DOUYIN
using TTSDK;
#endif

namespace JQFramework.Platform
{
#if SDK_DOUYIN
    public class DySdkMgr : ISdkMgr
    {
        private DyAdCtrl _dyAdCtrl;
        private TTFileSystemManager _dyFileSystemMgr;

        public DySdkMgr(Action initFinishAction)
        {
            JQLog.Log("TT.InitSDK");
            TT.InitSDK((code, env) =>
            {
                TT.SetKeepScreenOn(true);
                JQLog.Log("Unity message init sdk callback");
                JQLog.Log("Unity message code: " + code);
                JQLog.Log("Unity message HostEnum: " + env.m_HostEnum);
                _dyFileSystemMgr = TT.GetFileSystemManager();
                InitDir();
                initFinishAction?.Invoke();
            });
        }

        private void InitDir()
        {
            Mkdir("ttfile://user/StreamingAssets");
            Mkdir("ttfile://user/StreamingAssets/yoo");
            Mkdir("ttfile://user/StreamingAssets/yoo/asset");
        }

        public bool IsStreamingAssetsExist()
        {
            return _dyFileSystemMgr.AccessSync("ttfile://user/StreamingAssets");
        }

        private void Mkdir(string dirPath)
        {
            if (_dyFileSystemMgr.AccessSync(dirPath)) return;
            JQLog.Log("Need Mkdir:"+dirPath);
            string error = _dyFileSystemMgr.MkdirSync(dirPath);
            if (!string.IsNullOrEmpty(error))
            {
                JQLog.LogError($"FileSystem Mkdir:{error}");
            }
        }

        public SdkPlatform GetPlatform()
        {
            return SdkPlatform.douyin;
        }

        public bool NeedShowSubscribeTips(string tmplId)
        {
            return false;
        }

        public void RequestSubscribeMessage(string[] tmplIds)
        {
        }

        public void StartAntiAddiction(string accountName)
        {
            throw new NotImplementedException();
        }

        public bool IsSubscribed(string tmplId)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public void AddTouchEnd(string type)
        {
            throw new NotImplementedException();
        }

        public void RemoveTouchEnd(string type)
        {
            throw new NotImplementedException();
        }

        public void AddPermissions()
        {
            throw new NotImplementedException();
        }

        public bool IsNeedAddPermissions()
        {
            throw new NotImplementedException();
        }

        public void InitAfterPermission()
        {
            _dyAdCtrl = new DyAdCtrl();
            Sys.adCtrl = _dyAdCtrl;
        }

        public Task<bool> isLogined()
        {
            throw new NotImplementedException();
        }

        public string GetAccessToken()
        {
            throw new NotImplementedException();
        }

        public (float, float, float, float) GetSafeAreaInfo()
        {
            var info = TT.GetSystemInfo();
            double safeAreaLeft = info.safeArea.left;
            double safeAreaRight = info.screenWidth - info.safeArea.right;
            double safeAreaTop = info.safeArea.top;
            double safeAreaBottom = info.screenHeight - info.safeArea.bottom;
            JQLog.Log(
                $"x:{info.safeArea.top}, y:{info.safeArea.left}, width:{info.safeArea.width}, height:{info.safeArea.height} screen:{info.screenWidth} {info.screenHeight}");

            float safeLeftRate = (float)(safeAreaLeft / info.screenWidth);
            float safeRightRate = (float)(safeAreaRight / info.screenWidth);
            float safeTopRate = (float)(safeAreaTop / info.screenHeight);
            float safeBottomRate = (float)(safeAreaBottom / info.screenHeight);
            return (safeLeftRate, safeRightRate, safeTopRate, safeBottomRate);
        }

        public void SetParam(string str, object obj)
        {
            throw new NotImplementedException();
        }

        public void ReportEvent<T>(string eventId, T data)
        {
            JQLog.LogWarning($"ReportEvent:{eventId} {data}");
            //开通了再打开
            // IReportEvent iReportEvent = data as IReportEvent;
            // if (iReportEvent != null)
            // {
            //     TT.ReportAnalytics(eventId, iReportEvent.ToDictionary());
            // }
        }

        public void GC()
        {
            MemoryUnloader.instance.addUnloadTask(null);
        }

        public void ClearData(string key)
        {
            TT.ClearAllSavings();
        }

        public bool IsDataExist(string key)
        {
            return true;
        }

        public void VibrateShort(int type)
        {
            if (!AppConfig.IsRelease) return;
            long[] vibrateLong = null;

            switch (type)
            {
                case 0:
                    vibrateLong = new long[] { 30 };
                    break;
                case 1:
                    vibrateLong = new long[] { 100 };
                    break;
                case 2:
                    vibrateLong = new long[] { 400 };
                    break;
            }

            TT.Vibrate(vibrateLong);
        }

        public void LoadDataAsync(string key, Action<string> callback)
        {
            string value = TT.LoadSaving<string>(key);
            callback(value);
        }

        public void SaveDataAsync(string key, string value)
        {
            TT.Save(key, value);
        }

        public void SaveCloudDataAsync(string key)
        {
            
        }

        public void CheckIsAddedToMyMiniProgram(Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public void GetInviteCount(Action<int> action)
        {
            throw new NotImplementedException();
        }
    }
#endif
}