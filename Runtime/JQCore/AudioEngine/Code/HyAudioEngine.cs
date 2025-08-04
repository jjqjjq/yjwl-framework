/*----------------------------------------------------------------
// 文件名：HySoundEngine.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 15:31:16
//----------------------------------------------------------------*/

using JQCore.AudioEngine.Code.Data;
using JQCore.AudioEngine.Code.Event;
using JQCore.AudioEngine.Code.Object;
using JQCore.tJson;
using JQCore.tRes;
using UnityEngine;

namespace JQCore.AudioEngine.Code
{
    public static class HyAudioEngine
    {
        public static void Init(string configJsonStr, AssetObjectLib resLib, AssetObjectLib mixerResLib)
        {
            var configJson = new JSONObject(configJsonStr);
            HyAudioObjectMgr.Instance.initMixer(mixerResLib);
            HyAudioObjectMgr.Instance.initAudio(resLib);
            AudioDataMgr.Instance.initData(configJson);
        }

        public static uint PostEvent(string eventName, GameObject gameObject)
        {
            return HyAudioEventGroupMgr.Instance.PostEvent(eventName, gameObject);
        }
        
        public static void ClearEventParam(string eventName)
        {
            HyAudioEventGroupMgr.Instance.ClearEventParam(eventName);
        }

        public static void SetBusVolume(string paramName, float val)
        {
            HyAudioObjectMgr.Instance.SetBusVolume(paramName, val);
        }

        public static void StopPlayingID(uint in_playingID, int in_uTransitionDuration = 0, HyAudioConst.CURVE_TYPE in_eFadeCurve = HyAudioConst.CURVE_TYPE.Linear)
        {
            HyAudioEventGroupMgr.Instance.StopPlayingID(in_playingID, in_uTransitionDuration, in_eFadeCurve);
        }
    }
}