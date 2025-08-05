using System;
using System.Threading.Tasks;
using JQCore;
using JQCore.tCfg;

namespace JQFramework.Platform
{
    public class DySdkMgr: ISdkMgr
    {
        public SdkPlatform GetPlatform()
        {
            throw new NotImplementedException();
        }

        public bool NeedShowSubscribeTips(string tmplId)
        {
            throw new NotImplementedException();
        }

        public void RequestSubscribeMessage(string[] tmplIds)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SetParam(string str, object obj)
        {
            throw new NotImplementedException();
        }

        public void ReportEvent<T>(string eventId, T data)
        {
            throw new NotImplementedException();
        }

        public void GC()
        {
            throw new NotImplementedException();
        }

        public void ClearData(string key)
        {
            throw new NotImplementedException();
        }

        public bool IsDataExist(string key)
        {
            throw new NotImplementedException();
        }

        public void VibrateShort(int type)
        {
            throw new NotImplementedException();
        }

        public void LoadDataAsync(string key, Action<string> callback)
        {
            throw new NotImplementedException();
        }

        public void SaveDataAsync(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SaveCloudDataAsync(string key)
        {
            throw new NotImplementedException();
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
}