using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JQAppStart.Platform.Android;
using JQCore;
using JQCore.tCfg;
using JQCore.tLog;
using JQCore.tUtil;
using JQFramework;
using LitJson;
using UnityEngine;

namespace JQAppStart.Platform
{
    public class NoneSdkMgr : BaseSdkMgr, ISdkMgr
    {
        public NoneSdkMgr()
        {
        }

        public SdkPlatform GetPlatform()
        {
            return SdkPlatform.none;
        }

        public bool NeedShowSubscribeTips(string tmplId)
        {
            return true;
        }

        public void RequestSubscribeMessage(string[] tmplIds)
        {
        }

        public void StartAntiAddiction(string accountName)
        {
            throw new NotImplementedException();
        }

        public void FristLogin(Action<JsonData> loginCallback)
        {
        }

        public bool IsSubscribed(string tmplId)
        {
            return false;
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
            Sys.adCtrl = new NoneAdCtrl(PlayAd);
        }


        private void PlayAd(AdType adType, Dictionary<string, object> rewardConfigDic)
        {
            if (_paramDic.ContainsKey("OtherHttp") == false)
            {
                JQLog.Log("OtherHttp not exist");
                return;
            }

            string address = _paramDic["OtherHttp"] as string;
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append($"http://{address}/AdRewardVideo?");
            urlBuilder.Append($"pid={rewardConfigDic["AdId"]}");
            urlBuilder.Append($"&user_id={rewardConfigDic["UserId"]}");
            urlBuilder.Append($"&trans_id=111");
            urlBuilder.Append($"&extra={rewardConfigDic["Extra1"]}");
            urlBuilder.Append($"&sign=111");
            string url = urlBuilder.ToString();
            JQLog.Log($"url:{url}");
            HttpUtil.httpGet(url, s => { JQLog.Log("PlayAd success:" + s); });
        }

        public async Task<bool> isLogined()
        {
            return true;
        }

        public string GetAccessToken()
        {
            throw new NotImplementedException();
        }

        public (float, float, float, float) GetSafeAreaInfo()
        {
            Rect safeArea = Screen.safeArea;
            double safeAreaLeft = safeArea.y;
            double safeAreaRight = Screen.height - (safeArea.y + safeArea.height);
            double safeAreaTop = safeArea.x;
            double safeAreaBottom = Screen.width - (safeArea.x + safeArea.width);
            Debug.Log(
                $"x:{safeArea.x}, y:{safeArea.y}, width:{safeArea.width}, height:{safeArea.height} screen:{Screen.width} {Screen.height}");

            float safeLeftRate = (float)(safeAreaLeft / Screen.height);
            float safeRightRate = (float)(safeAreaRight / Screen.height);
            float safeTopRate = (float)(safeAreaTop / Screen.width);
            float safeBottomRate = (float)(safeAreaBottom / Screen.width);
            return (safeTopRate, safeBottomRate, safeRightRate, safeLeftRate);
        }

        public void ReportEvent<T>(string eventId, T data)
        {
            // if (eventId != EGame.wxdata_perf_monitor)
            // {
                // JQLog.LogError($"ReportEvent {eventId}");
            // }
        }

        public void GC()
        {
            MemoryUnloader.instance.addUnloadTask(null);
        }

        public string LoadData(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public void LoadDataAsync(string key, Action<string> callback)
        {
            string value = PlayerPrefs.GetString(key, null);
            JQLog.Log($"LoadDataAsync:{value}");
            callback?.Invoke(value);
        }

        public void SaveData(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        public void SaveDataAsync(string key, string value)
        {
            JQLog.Log($"SaveDataAsync:{key}");
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        public void SaveCloudDataAsync(string key)
        {
        }

        private bool _isAdded;
        public void CheckIsAddedToMyMiniProgram(Action<bool> callback)
        {
            callback?.Invoke(_isAdded);
            _isAdded = !_isAdded;
        }

        private int _inviteCount;
        public void GetInviteCount(Action<int> action)
        {
            action?.Invoke(_inviteCount);
            _inviteCount++;
        }

        public void ClearData(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public bool IsDataExist(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public void VibrateShort(int type)
        {
        }
    }
}