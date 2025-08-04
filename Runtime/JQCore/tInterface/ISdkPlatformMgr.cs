using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JQCore.tCfg;

namespace JQCore
{
    public interface ISdkPlatformMgr
    {
        public SdkPlatform GetPlatform();
        void InitAfterPermission();
        
        void AddPermissions();
        List<string> GetPermissions();
        
        void StartAntiAddiction(string accountName);
        string GetAccessToken();
        void Login(Action loginCallback);
        
        void Logout();

        Task<bool> isLogined();

    }
}