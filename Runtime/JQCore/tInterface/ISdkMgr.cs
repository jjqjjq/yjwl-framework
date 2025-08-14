using System;
using System.Threading.Tasks;
using JQCore.tCfg;
using LitJson;

namespace JQCore
{
    public interface ISdkMgr
    {
        bool IsStreamingAssetsExist();
        public SdkPlatform GetPlatform();
        bool NeedShowSubscribeTips(string tmplId);
        void RequestSubscribeMessage(string[] tmplIds);

        bool HasCloudEnv();
        
        void StartAntiAddiction(string accountName);

        bool IsSubscribed(string tmplId);

        void Logout();

        void AddTouchEnd(string type);

        void RemoveTouchEnd(string type);

        void AddPermissions();

        bool IsNeedAddPermissions();

        void InitAfterPermission();

        Task<bool> isLogined();

        string GetAccessToken();

        (float, float, float, float) GetSafeAreaInfo();

        void SetParam(string str, object obj);

        void ReportEvent<T>(string eventId, T data);

        void GC();

        void ClearData(string key);
        bool IsDataExist(string key);

        void VibrateShort(int type);
        void LoadDataAsync(string key, Action<string> callback);
        void SaveDataAsync(string key, string value);

        public void SaveCloudDataAsync(string key);

        void CheckIsAddedToMyMiniProgram(Action<bool> callback);
        void GetInviteCount(Action<int> action);


#if SDK_DOUYIN
        bool CanShowSideBar();
        bool IsLocationSideBar();
        void NavigateToSideBarScene();

        void ReportScene(int sceneId);
#endif
    }
}