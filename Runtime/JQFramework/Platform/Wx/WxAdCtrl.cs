using System;
using System.Collections.Generic;
using JQCore;
using JQCore.tString;
using UnityEngine.Profiling;
using WeChatWASM;

namespace JQAppStart.Platform
{
    public class WxAdCtrl : IAdCtrl
    {
        private Dictionary<string, WxAdBean> _wxAdBeanDic = new Dictionary<string, WxAdBean>();

        private const string IMG_URL1 =
            "https://mmocgame.qpic.cn/wechatgame/QZcggKbynZ9xpjgKWIes1icbb2YDicNdQe60SFcG0kajV71v3ibh37pns4yXEg64lLq/0";

        private const string IMG_URL2 =
            "https://mmocgame.qpic.cn/wechatgame/IwBFVVhZF58UDVczNXOM8EXb3ic4iaZUzb3sWgpdWEk9hTeW8sPL8IILM8BwR91YpP/0";

        private const string IMG_ID1 = "0VlL/SAZQ8KYcdK+BcrOBA==";
        private const string IMG_ID2 = "lTFPgrh6S82yOAjHuMKu0g==";
        private string _currOpenId = null;
        private string _encodeOpenId = null;

        public WxAdCtrl()
        {
            WX.UpdateShareMenu(new UpdateShareMenuOption()
            {
                withShareTicket = true,
                isPrivateMessage = true
            });
            
            WX.OnShareAppMessage(new WXShareAppMessageParam()
            {
                title = "差一点就过了，帮帮我!",
                imageUrl = IMG_URL1,
                imageUrlId = IMG_ID1
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
                    imageUrl = IMG_URL1,
                    imageUrlId = IMG_ID1,
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
            option.imageUrl = IMG_URL2;
            option.imageUrlId = IMG_ID2;
            option.query = $"SharerId={_encodeOpenId}";
            WX.ShareAppMessage(option);
        }
    }
}