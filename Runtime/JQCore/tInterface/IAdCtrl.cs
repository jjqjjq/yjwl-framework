using System;
using System.Collections.Generic;

namespace JQCore
{
    public interface IAdCtrl
    {
        public void ReadyAd(string adUnitId);
        public bool PlayAd(AdType type, string adUnitId, Action<bool> onAdClose, Action<int, string> onAdError, Action onAdLoad, Action onAdShowSuccess, Action onAdShowFail);
        public void RemoveAd(string adUnitId);
        
        void ShareApp();
    }
    
    public enum AdType
    {
        None = 0,
        Splash,
        RewardVideo,
        Banner,
        Interstitial
    }
}