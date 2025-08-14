using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ET.ETFramework.Model.Client.Game;
using JQCore;
using JQCore.tCfg;
using JQCore.tLog;
#if SDK_DOUYIN
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using TTSDK;
using TTSDK.UNBridgeLib.LitJson;
#endif

namespace JQFramework.Platform
{
#if SDK_DOUYIN
    [Serializable]
    public class DySaveData
    {
        public string json;

        public override string ToString()
        {
            return json;
        }
    }
    
    public class DySdkMgr : ISdkMgr
    {
        private DyAdCtrl _dyAdCtrl;
        private TTFileSystemManager _dyFileSystemMgr;
        private DySaveData _dySaveData;
        private TTSystemInfo _dySystemInfo;

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
                TT.GetAppLifeCycle().OnShow += OnOnShow;
                TT.GetAppLifeCycle().OnHide += OnAppHide;
                CheckSideBarScene();
                _dySaveData = new DySaveData();
                initFinishAction?.Invoke();
                _dySystemInfo = TT.GetSystemInfo();
                UpdateQualitySettingByDeviceModelLevel();
            });
        }

        private string _scene;
        private string _launch_from;
        private string _location;

        private void OnOnShow(Dictionary<string, object> param)
        {
            foreach (var kv in param)
            {
                JQLog.Log(kv.Key + "=" + kv.Value);
            }
            _scene = (string)param["scene"];
            _location = (string)param["location"];
            _launch_from = (string)param["launchFrom"];
            JQLog.Log($"scene:{_scene}, location:{_location}, launch_from:{_launch_from}");
        }

        private void OnAppHide()
        {
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
            JQLog.Log("Need Mkdir:" + dirPath);
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
            return true;
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public void AddTouchEnd(string type)
        {
        }

        public void RemoveTouchEnd(string type)
        {
        }

        public void AddPermissions()
        {
        }

        public bool IsNeedAddPermissions()
        {
            return false;
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
        
        private void UpdateQualitySettingByDeviceModelLevel()
        {
            var info = _dySystemInfo;
            double score = info.deviceScore.overall;
            if (score >= 8.51f || score <0f)
            {
                JQCore.tUtil.UnityUtil.SetQualityLevel(0);
            }else if (score >= 7.30f)
            {
                
                JQCore.tUtil.UnityUtil.SetQualityLevel(1);
            }
            else 
            {
                JQCore.tUtil.UnityUtil.SetQualityLevel(2);
                
            }
        }

        public (float, float, float, float) GetSafeAreaInfo()
        {
            var info = _dySystemInfo;
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

        public bool HasCloudEnv()
        {
            return false;
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
        
        public void ReportScene(int sceneId)
        {
            JQLog.Log($"ReportScene:{sceneId}");
            JsonData jsonData = new JsonData();
            jsonData["sceneId"] = sceneId;
            jsonData["costTime"] = UnityEngine.Time.time;
            TT.ReportScene(jsonData, null, ((i, s) =>
            {
                JQLog.LogError($"ReportScene Fail:{i} {s}");
            }));
        }

        public void GC()
        {
            MemoryUnloader.instance.addUnloadTask(null);
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
            JQLog.Log($"LoadDataAsync:{key}");
            _dySaveData = TT.LoadSaving<DySaveData>(key);
            callback(_dySaveData.json);
        }

        public void SaveDataAsync(string key, string value)
        {
            JQLog.Log($"SaveDataAsync1:{key}:{value}");
            DySaveData dySaveData = new DySaveData();
            dySaveData.json = value;;
            JQLog.Log($"SaveDataAsync2:{key} {dySaveData==null}");
            TT.Save(dySaveData, key);
            JQLog.Log($"SaveDataAsync3:{key}");
        }
        
        public void ClearData(string key)
        {
            TT.DeleteSaving<DySaveData>(key);
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

        #region 侧边栏

        private bool _canShowSideBar;

        public bool CanShowSideBar()
        {
            return _canShowSideBar;
        }

        public bool IsLocationSideBar()
        {
            if (_scene == "021036" && _launch_from == "homepage" && _location == "sidebar_card")
            {
                return true;
            }
            return false;
        }

        public void NavigateToSideBarScene()
        {
            JQLog.Log("NavigateToSideBarScene");
            JsonData jsonData = new JsonData();
            jsonData["scene"] = "sidebar";
            TT.NavigateToScene(jsonData, () => { JQLog.Log($"NavigateToSideBarScene Success"); },
                () => { }, ((i, s) => { JQLog.LogError($"NavigateToSideBarScene Error:{i} {s}"); }));
        }

        private void CheckSideBarScene()
        {
            TT.CheckScene(TTSideBar.SceneEnum.SideBar, isExist => { _canShowSideBar = isExist; }, () => { },
                ((i, s) => { JQLog.LogError($"checkScene fail:{i} {s}"); }));
        }

        #endregion
    }
#endif
}