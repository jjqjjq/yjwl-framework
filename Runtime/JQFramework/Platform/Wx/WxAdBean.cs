using System;
using JQCore.tLog;
using WeChatWASM;

namespace JQAppStart.Platform
{
    public class WxAdBean
    {
        private string _adUnitId;
        private WXRewardedVideoAd _wxRewardedVideoAd;
        private Action<bool> _onAdClose;
        private Action<int, string> _onAdError;
        private Action _onAdLoad;
        private Action _onAdShowSuccess;
        private Action _onAdShowFail;
        private bool _isLoaded;
        private bool _isWaitShow;

        public string AdUnitId => _adUnitId;

        public bool IsLoaded => _isLoaded;

        public WxAdBean(string adUnitId)
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
            if (_wxRewardedVideoAd != null)
            {
                return;
            }

            JQLog.Log($"WxAdBean.Load: {_adUnitId}");
            WXCreateRewardedVideoAdParam param = new WXCreateRewardedVideoAdParam()
            {
                adUnitId = _adUnitId,
                multiton = true
            };
            _wxRewardedVideoAd = WX.CreateRewardedVideoAd(param);
            _wxRewardedVideoAd.OnLoad(OnLoad);
            _wxRewardedVideoAd.OnError(OnError);
            _wxRewardedVideoAd.OnClose(OnClose);
            _wxRewardedVideoAd.Load(OnLoadSuccess, OnLoadFail);
        }

        public void Show()
        {
            JQLog.Log($"WxAdBean.Show: {_adUnitId} {_isLoaded}");
            if (_isLoaded)
            {
                _wxRewardedVideoAd.Show(OnShowSuccess, OnShowFail);
            }
            else
            {
                _isWaitShow = true;
            }
        }

        public void Dispose()
        {
            JQLog.Log($"WxAdBean.Dispose: {_adUnitId}");
            if (_wxRewardedVideoAd != null)
            {
                _wxRewardedVideoAd.OffError(OnError);
                _wxRewardedVideoAd.OffClose(OnClose);
                _wxRewardedVideoAd.OffLoad(OnLoad);
                _wxRewardedVideoAd.Destroy();
                _wxRewardedVideoAd = null;
            }

            _onAdClose = null;
            _onAdError = null;
            _onAdLoad = null;
            _onAdShowSuccess = null;
            _onAdShowFail = null;
            _isLoaded = false;
            _isWaitShow = false;
        }

        private void OnClose(WXRewardedVideoAdOnCloseResponse obj)
        {
            JQLog.Log($"RewardVideoAd[{_adUnitId}] OnClose:{obj.isEnded}");
            _onAdClose?.Invoke(obj.isEnded);
        }

        private void OnError(WXADErrorResponse obj)
        {
            JQLog.LogError($"RewardVideoAd[{_adUnitId}] OnError:{obj.errCode} {obj.errMsg}");
            _onAdError?.Invoke(obj.errCode, obj.errMsg);
        }

        private void OnLoad(WXADLoadResponse obj)
        {
            JQLog.Log($"RewardVideoAd[{_adUnitId}] OnLoad:{obj.shareValue} {obj.rewardValue}");
            _isLoaded = true;
            _onAdLoad?.Invoke();
            _onAdLoad = null;
            if (_isWaitShow)
            {
                _isWaitShow = false;
                Show();
            }
        }

        private void OnShowFail(WXTextResponse obj)
        {
            if (obj.errCode != 0)
            {
                JQLog.LogError($"RewardVideoAd[{_adUnitId}] OnShowFail:{obj.errCode} {obj.errMsg}");
            }
            _onAdShowFail?.Invoke();
        }

        private void OnShowSuccess(WXTextResponse obj)
        {
            JQLog.Log($"RewardVideoAd[{_adUnitId}] OnShowSuccess");
            _onAdShowSuccess?.Invoke();
        }

        private void OnLoadFail(WXADErrorResponse obj)
        {
            if (obj.errCode != 0)
            {
                JQLog.LogError($"RewardVideoAd[{_adUnitId}] OnLoadFail:{obj.errCode} {obj.errMsg}");
            }
        }

        private void OnLoadSuccess(WXTextResponse obj)
        {
            JQLog.Log($"RewardVideoAd[{_adUnitId}] OnLoadSuccess");
        }
    }
}