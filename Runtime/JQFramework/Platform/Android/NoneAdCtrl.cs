using System;
using System.Collections.Generic;
using JQCore;

namespace JQFramework.Platform
{
    public class NoneAdCtrl : IAdCtrl
    {
        private Action<AdType, Dictionary<string, object>> _playAdAction;
        
        private Dictionary<string, object> _rewardConfigDic;

        public NoneAdCtrl(Action<AdType, Dictionary<string, object>> playAdAction)
        {
        }


        public void ReadyAd(string adUnitId)
        {
        }

        public bool PlayAd(AdType type, string adUnitId, Action<bool> onAdClose, Action<int, string> onAdError, Action onAdLoad, Action onAdShowSuccess, Action onAdShowFail)
        {
            _playAdAction?.Invoke(type, _rewardConfigDic); //直接跳过广告商SDK，回调服务器URL
            //1秒后假装广告播放完毕
            Sys.openWaitView();
            Sys.timerMgr.addOnce(1, this, () =>
            {
                Sys.closeWaitView();
                onAdClose(true);
            });
            return false;
        }

        public void RemoveAd(string adUnitId)
        {
            
        }

        public void ShareApp()
        {
        }

        public void LoadAd(AdType type, Dictionary<string, object> rewardConfigDic)
        {
            _rewardConfigDic = rewardConfigDic;
        }
    }
}