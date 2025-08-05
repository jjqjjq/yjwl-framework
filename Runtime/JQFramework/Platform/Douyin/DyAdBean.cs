#if SDK_DOUYIN
using System;
using JQCore.tLog;
using TTSDK;

namespace JQFramework.Platform
{
    public class DyAdBean
    {
        private string _adUnitId;
        private TTRewardedVideoAd _rewardedVideoAd;
        private Action<bool> _onAdClose;
        private Action<int, string> _onAdError;
        private Action _onAdLoad;
        private Action _onAdShowSuccess;
        private Action _onAdShowFail;
        private bool _isLoaded;
        private bool _isWaitShow;

        public string AdUnitId => _adUnitId;

        public bool IsLoaded => _isLoaded;

        public DyAdBean(string adUnitId)
        {
            _adUnitId = adUnitId;
        }

        public void SetFunc(Action<bool> onAdClose, Action<int, string> onAdError, Action onAdLoad, Action onAdShowSuccess,
            Action onAdShowFail)
        {
            JQLog.Log("WxAdBean.SetFunc：" + _adUnitId);
            _onAdClose = onAdClose;
            _onAdError = onAdError;
            _onAdLoad = onAdLoad;
            _onAdShowSuccess = onAdShowSuccess;
            _onAdShowFail = onAdShowFail;
        }

        public void Load()
        {
            if (_rewardedVideoAd != null)
            {
                return;
            }

            JQLog.Log($"DyAdBean.Load: {_adUnitId}");
            CreateRewardedVideoAdParam param = new CreateRewardedVideoAdParam()
            {
                AdUnitId = _adUnitId,
            };
            _rewardedVideoAd = TT.CreateRewardedVideoAd(param);
            _rewardedVideoAd.OnLoad += OnLoad;
            _rewardedVideoAd.OnError += OnError;
            _rewardedVideoAd.OnClose += OnClose;
            _rewardedVideoAd.Load();
        }

        public void Show()
        {
            JQLog.Log($"DyAdBean.Show: {_adUnitId} {_isLoaded}");
            if (_isLoaded)
            {
                _rewardedVideoAd.Show();
            }
            else
            {
                _isWaitShow = true;
            }
        }

        public void Dispose()
        {
            JQLog.Log($"DyAdBean.Dispose: {_adUnitId}");
            if (_rewardedVideoAd != null)
            {
                _rewardedVideoAd.OnError -= OnError;
                _rewardedVideoAd.OnClose -= OnClose;
                _rewardedVideoAd.OnLoad -= OnLoad;
                _rewardedVideoAd.Destroy();
                _rewardedVideoAd = null;
            }

            _onAdClose = null;
            _onAdError = null;
            _onAdLoad = null;
            _onAdShowSuccess = null;
            _onAdShowFail = null;
            _isLoaded = false;
            _isWaitShow = false;
        }

        private void OnClose(bool isEnded, int count)
        {
            JQLog.Log($"RewardVideoAd[{_adUnitId}] OnClose:{isEnded}");
            _onAdClose?.Invoke(isEnded);
        }

        private void OnError(int errorCode, string errorMessage)
        {
            JQLog.LogError($"RewardVideoAd[{_adUnitId}] OnError:{errorCode} {errorMessage}");
            _onAdError?.Invoke(errorCode, errorMessage);
        }

        private void OnLoad()
        {
            JQLog.Log($"RewardVideoAd[{_adUnitId}] OnLoad");
            _isLoaded = true;
            _onAdLoad?.Invoke();
            _onAdLoad = null;
            if (_isWaitShow)
            {
                _isWaitShow = false;
                Show();
            }
        }

    }
}
#endif