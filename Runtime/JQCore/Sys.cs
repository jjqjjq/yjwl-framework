using System;
using System.Collections.Generic;
using System.Diagnostics;
using JQCore.tEvent;
using JQCore.tTime;
using JQCore.tLoader;
using JQCore.tLog;
using JQCore.tPerformance;
using UnityEngine;
using YooAsset;

namespace JQCore
{
    public enum AppCfgType
    {
        local, //本地
        trunk, //内网
        outTest, //外网测试
        release, //正式版本
    }
    
    public static class Sys
    {
        
        public static float GAME_WIDTH = 1024f;
        public static float GAME_HEIGHT = 2048f;

        public static ISdkMgr sdkMgr;
        public static IViewTool viewTool;
        public static IAdCtrl adCtrl;
        public static ISaveCtrl saveCtrl;
        
        public static EPlayMode yooAssetPlayMode;
        public static ILoadingUI loadingUi;
        public static JQEventDispatcher gameDispatcher = new();
        public static JQStringBuilder stringBuilder = new JQStringBuilder();
        public static Int2StrLib Int2StrLib = new Int2StrLib(99999);
        private static Dictionary<string, Stopwatch> _stopwatchDic = new Dictionary<string, Stopwatch>();

        public static bool hasSDK = false;
        public static bool LOG_RES = false;
        public static bool isEditor = true;
        public static int svnVersion; 
        
        public delegate void VoidDelegateIntGameobject(int index, GameObject go);

        public delegate object VoidDelegateL(long o);

        public delegate object VoidDelegateO(object o);

        public delegate object VoidDelegateOO(object o1, object o2);

        public delegate object VoidDelegateFF(float o1, float o2);

        public delegate object VoidDelegateOOO(object o1, object o2, object o3);

        public delegate object VoidDelegateOOOO(object o1, object o2, object o3, object o4);

        public static JQTimerMgr sysTimerMgr = new JQTimerMgr("sysTimerMgr");
        public static JQTimerMgr timerMgr = new JQTimerMgr("timerMgr");
        public static JQTimerMgr highTimerMgr = new JQTimerMgr("highTimerMgr");

        public static void Update()
        {
            sysTimerMgr.onTick();
            timerMgr.onTick();
            highTimerMgr.onTick();
        }
        
        public static void StartStopwatch(string str)
        {
            _stopwatchDic.TryGetValue(str, out Stopwatch stopwatch);
            if (stopwatch == null)
            {
                stopwatch = new Stopwatch();
                _stopwatchDic.Add(str, stopwatch);
            }
            stopwatch.Reset();
            stopwatch.Start();
            // JQLog.LogError($"加载耗时 {str}.Start");
        }
        
        public static void StopStopwatch(string str)
        {
            _stopwatchDic.TryGetValue(str, out Stopwatch stopwatch);
            if (stopwatch == null)
            {
                JQLog.LogError($"加载耗时 {str}：_stopwatch == null");
                return;
            }
            stopwatch.Stop();
            JQLog.LogError($"加载耗时 {str}：{stopwatch.ElapsedMilliseconds}ms");
        }
        
        public static void openLoadingUI()
        {
            // HyDebug.LogError("openLoadingUI");
            if (loadingUi != null) loadingUi.open();
        }

        public static void closeLoadingUI()
        {
            // HyDebug.LogError("closeLoadingUI");
            if (loadingUi != null)
            {
                loadingUi.close();
                loadingUi = null;
            }
        }

        public static void showMessageBox(string titleStr, string textStr, string leftStr, Action leftAction, string rightStr, Action rightAction)
        {
            if (loadingUi != null) loadingUi.showMessageBox(titleStr, textStr, leftStr, leftAction, rightStr, rightAction);
        }

        public static void setLoadingText(string text)
        {
            JQLog.Log("setLoadingText:" + text);
            if (loadingUi != null) loadingUi.setLoadingText(text);
        }

        public static void setTipsText(string text)
        {
            //            HyDebug.Log("setLoadingText:" + text);
            if (loadingUi != null) loadingUi.setTipsText(text);
        }

        public static void setVersionText(string text)
        {
//            HyDebug.Log("setLoadingText:" + text);
            if (loadingUi != null) loadingUi.setVersionText(text);
        }

        public static void setLoadingProgress(long total, long remain)
        {
            if (loadingUi != null) loadingUi.setLoadingProgress(total, remain);
        }
        
        public static int apkInit
        {
            get { return PlayerPrefs.GetInt("System.apkInit", 0); }
            set
            {
                PlayerPrefs.SetInt("System.apkInit", value);
                PlayerPrefs.Save();
            }
        }
        public static int apkLog
        {
            get { return PlayerPrefs.GetInt("System.apkLog", 0); }
            set
            {
                PlayerPrefs.SetInt("System.apkLog", value);
                PlayerPrefs.Save();
            }
        }
        
    }
}