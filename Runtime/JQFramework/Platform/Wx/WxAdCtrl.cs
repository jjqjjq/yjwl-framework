#if SDK_WEIXIN
using System;
using System.Collections.Generic;
using JQCore;
using JQCore.tString;
using UnityEngine.Profiling;
using WeChatWASM;

namespace JQFramework.Platform
{
    public class WxAdCtrl : IAdCtrl
    {
        private Dictionary<string, WxAdBean> _wxAdBeanDic = new Dictionary<string, WxAdBean>();

        private string[] _shareCfgs;
        private string _currOpenId = null;
        private string _encodeOpenId = null;

        public WxAdCtrl(string[] shareCfgs)
        {
            _shareCfgs = shareCfgs;
            WX.UpdateShareMenu(new UpdateShareMenuOption()
            {
                withShareTicket = true,
                isPrivateMessage = true
            });
            
            WX.OnShareAppMessage(new WXShareAppMessageParam()
            {
                title = "差一点就过了，帮帮我!",
                imageUrlId = _shareCfgs[0],
                imageUrl = _shareCfgs[1],
            });
        }

        public string OpenId
        {
            set
            {
                _currOpenId = value;
                _encodeOpenId = StringUtil.EncryptDES(_currOpenId);
                WX.OnShareAppMessage(new WXShareAppMessageParam()
                {
                    title = "差一点就过了，帮帮我!",
                    imageUrlId = _shareCfgs[2],
                    imageUrl = _shareCfgs[3],
                    query = $"SharerId={_encodeOpenId}"
                });
            }
        }

        private WxAdBean GetWxAdBean(string adUnitId)
        {
            _wxAdBeanDic.TryGetValue(adUnitId, out WxAdBean wxAdBean);
            if (wxAdBean == null)
            {
                wxAdBean = new WxAdBean(adUnitId);
                _wxAdBeanDic[adUnitId] = wxAdBean;
            }

            return wxAdBean;
        }

        public void ReadyAd(string adUnitId)
        {
            Profiler.BeginSample($"WxAdCtrl.ReadyAd:{adUnitId}");
            WxAdBean wxAdBean = GetWxAdBean(adUnitId);
            wxAdBean.Load();
            Profiler.EndSample();
        }

        public void RemoveAd(string adUnitId)
        {
            WxAdBean wxAdBean = GetWxAdBean(adUnitId);
            wxAdBean.Dispose();
        }

        public bool PlayAd(AdType type, string adUnitId, Action<bool> onAdClose, Action<int, string> onAdError, Action onAdLoad,
            Action onAdShowSuccess, Action onAdShowFail)
        {
            WxAdBean wxAdBean = GetWxAdBean(adUnitId);
            wxAdBean.SetFunc(onAdClose, onAdError, onAdLoad, onAdShowSuccess, onAdShowFail);
            wxAdBean.Show();
            return !wxAdBean.IsLoaded;
        }


        public void ShareApp()
        {
            ShareAppMessageOption option = new ShareAppMessageOption();
            option.title = "求你帮帮，还差一点就过关";
            option.imageUrlId = _shareCfgs[2];
            option.imageUrl = _shareCfgs[3];
            option.query = $"SharerId={_encodeOpenId}";
            WX.ShareAppMessage(option);
        }
    }
}
#endif