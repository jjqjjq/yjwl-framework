#if SDK_DOUYIN
using System;
using System.Collections.Generic;
using JQCore;
using JQCore.tLog;
using JQCore.tString;
using TTSDK;
using TTSDK.UNBridgeLib.LitJson;
using UnityEngine.Profiling;

namespace JQFramework.Platform
{
    public class DyAdCtrl: IAdCtrl
    {
        private string _currOpenId = null;
        private string _encodeOpenId = null;
        private Dictionary<string, DyAdBean> _adBeanDic = new Dictionary<string, DyAdBean>();

        public DyAdCtrl()
        {
            // TT.GetUserInfo(OnGetUserInfoSuccess, OnGetUserInfoFail);
            TT.OnShareAppMessage(OnShareApp);
        }

        // private void OnGetUserInfoFail(string errmsg)
        // {
        //     JQLog.LogError($"GetUserInfoFail: {errmsg}");
        // }
        //
        // private void OnGetUserInfoSuccess(ref TTUserInfo scuserinfo)
        // {
        //     JQLog.Log($"GetUserInfoSuccess: {scuserinfo}");
        // }

        private DyAdBean GetAdBean(string adUnitId)
        {
            _adBeanDic.TryGetValue(adUnitId, out DyAdBean adBean);
            if (adBean == null)
            {
                adBean = new DyAdBean(adUnitId);
                _adBeanDic[adUnitId] = adBean;
            }

            return adBean;
        }
        
        public void ReadyAd(string adUnitId)
        {
            Profiler.BeginSample($"DyAdCtrl.ReadyAd:{adUnitId}");
            DyAdBean wxAdBean = GetAdBean(adUnitId);
            wxAdBean.Load();
            Profiler.EndSample();
        }

        public bool PlayAd(AdType type, string adUnitId, Action<bool> onAdClose, Action<int, string> onAdError, Action onAdLoad, Action onAdShowSuccess,
            Action onAdShowFail)
        { 
            DyAdBean adBean = GetAdBean(adUnitId);
            adBean.SetFunc(onAdClose, onAdError, onAdLoad, onAdShowSuccess, onAdShowFail);
            adBean.Show();
            return !adBean.IsLoaded;
        }

        public void RemoveAd(string adUnitId)
        { 
            DyAdBean wxAdBean = GetAdBean(adUnitId);
            wxAdBean.Dispose();
        }
        
        public string OpenId
        {
            set
            {
                _currOpenId = value;
                _encodeOpenId = StringUtil.EncryptDES(_currOpenId);
            }
        }

        private TTShare.ShareParam OnShareApp(TTShare.ShareOption shareOption)
        {
            JQLog.Log("OnShareApp");
            JsonData jsonData = new JsonData();
            jsonData["templateId"] = "";
            // jsonData["query"]= $"SharerId={_encodeOpenId}";
            return new TTShare.ShareParam(jsonData, OnShareAppSuccess, OnShareAppFail, OnShareAppCancel);
        }

        private void OnShareAppCancel()
        {
            JQLog.Log("OnShareAppCancel");
        }

        private void OnShareAppFail(string errmsg)
        {
            JQLog.LogError("OnShareAppFail:"+errmsg);
        }

        private void OnShareAppSuccess(Dictionary<string, object> data)
        {
            JQLog.Log("OnShareAppSuccess");
        }

        public void ShareApp()
        {
            JsonData jsonData = new JsonData();
            jsonData["templateId"] = "";
            jsonData["query"]= $"SharerId={_encodeOpenId}";
            TT.ShareAppMessage(jsonData, OnShareAppSuccess, OnShareAppFail);
        }
    }
}
#endif