using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JQCore;
using JQCore.tLog;
using UnityEngine;
#if UNITY_ANDROID
using JQAppStart.Platform.Android.Util;
using JQAppStart.Platform.Taptap;
using AppStart.framework.Sdk;
using JQAppStart.Platform.Android;
using JQAppStart.tEnum;
using JQCore.tCfg;
using JQFramework.tMVC;
using UnityEngine.Android;
#endif

namespace JQFramework.Platform
{
#if UNITY_ANDROID
    public class AndroidSdkMgr :BaseSdkMgr, ISdkMgr
    {
        private ISdkPlatformMgr _sdkPlatformMgr;

        private AndroidJavaClass javaClass;
        private AndroidJavaObject javaActive;
        private AndroidJavaObject _oaidJavaObject;
        public string javaClassStr = "com.unity3d.player.UnityPlayer";
        public string javaActiveStr = "currentActivity";
        private List<string> waitPermissions;

        public AndroidSdkMgr(SdkPlatform sdkPlatform)
        {
            switch (sdkPlatform)
            {
                case SdkPlatform.taptap:
#if PLATFORM_TAPTAP
                    _sdkPlatformMgr = new TapSdkMgr();
#endif
                    break;
            }


            javaClass = new AndroidJavaClass(javaClassStr);
            javaActive = javaClass.GetStatic<AndroidJavaObject>(javaActiveStr);
        }

        public void InitAfterPermission()
        {
            JQLog.Log($"注册oaid 初始化Ad");
            _sdkPlatformMgr.InitAfterPermission();
        }

        public bool IsNeedAddPermissions()
        {
            List<string> tempPermissions = new List<string>();
            tempPermissions.Add(Permission.ExternalStorageRead);
            tempPermissions.Add(Permission.ExternalStorageWrite);
            //获取渠道需要的权限
            tempPermissions.AddRange(_sdkPlatformMgr.GetPermissions());
            waitPermissions = new List<string>();
            foreach (string tempPermission in tempPermissions)
            {
                if (waitPermissions.Contains(tempPermission)) continue;
                if (Permission.HasUserAuthorizedPermission(tempPermission)) continue;
                waitPermissions.Add(tempPermission);
            }

            return waitPermissions.Count > 0;
        }

        public void AddPermissions()
        {
            // 申请权限
            PermissionCallbacks callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += (str) =>
            {
                JQLog.Log($"获取权限失败{str}");
                waitPermissions.Remove(str);
                if (waitPermissions.Count == 0)
                {
                    Sys.gameDispatcher.TriggerEvent(EAppStart.Event_AddPermissionsFinish);
                }
            };
            callbacks.PermissionGranted += (str) =>
            {
                JQLog.Log($"获取权限成功:{str}");
                waitPermissions.Remove(str);
                if (waitPermissions.Count == 0)
                {
                    Sys.gameDispatcher.TriggerEvent(EAppStart.Event_AddPermissionsFinish);
                }
            };
            string[] permissions = waitPermissions.ToArray();
            JQLog.Log($"请求权限:{string.Join(",", permissions)}");
            Permission.RequestUserPermissions(permissions, callbacks);
        }

        public SdkPlatform GetPlatform()
        {
            return _sdkPlatformMgr.GetPlatform();
        }

        public void StartAntiAddiction(string accountName)
        {
            _sdkPlatformMgr.StartAntiAddiction(accountName);
        }

        public void Login(Action loginCallback)
        {
            _sdkPlatformMgr.Login(loginCallback);
        }

        public void Logout()
        {
            _sdkPlatformMgr.Logout();
            javaActive.Call("logout");
        }

        public async Task<bool> isLogined()
        {
            return await _sdkPlatformMgr.isLogined();
        }

        public string GetAccessToken()
        {
            return _sdkPlatformMgr.GetAccessToken();
        }

        public (float, float, float, float) GetSafeAreaInfo()
        {
            return SafeAreaUtil.GetSafeAreaInfo();
        }

        public void SetParam(string str, object obj)
        {
            
        }
    }
#endif
}