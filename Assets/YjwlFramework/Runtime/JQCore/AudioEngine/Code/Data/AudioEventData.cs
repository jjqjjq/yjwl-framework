/*----------------------------------------------------------------
// 文件名：AudioEventData.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 16:49:08
//----------------------------------------------------------------*/

using JQCore.tJson;

namespace JQCore.AudioEngine.Code.Data
{
    public class AudioEventData
    {
        public float delay;
        public HyAudioConst.AM_EVENT_TYPE eventType;
        public HyAudioConst.AM_SCOPE scope;
        public string eventName;

        //事件对象id
        public AudioData targetData;


        public AudioEventData(string eventName, JSONObject jsonObject)
        {
            this.eventName = eventName;
            eventType = (HyAudioConst.AM_EVENT_TYPE)JSONUtil.getIntField(jsonObject, "type");
            scope = (HyAudioConst.AM_SCOPE)JSONUtil.getIntField(jsonObject, "scope");
            delay = JSONUtil.getFloatField(jsonObject, "delay");

            switch (eventType)
            {
                case HyAudioConst.AM_EVENT_TYPE.Play:
                    var playTargetId = JSONUtil.getUintField(jsonObject, "playTargetId");
                    targetData = AudioDataMgr.Instance.GetAudioSfxData(playTargetId, true);
                    playProbability = JSONUtil.getIntField(jsonObject, "playProbability");
                    playFadeTime = JSONUtil.getFloatField(jsonObject, "playFadeTime");
                    playFadeInCurveType = (HyAudioConst.CURVE_TYPE)JSONUtil.getIntField(jsonObject, "playFadeInCurveType");
                    break;
                case HyAudioConst.AM_EVENT_TYPE.Stop:
                    var stopSfxTargetId = JSONUtil.getUintField(jsonObject, "stopSfxTargetId");
                    var stopBusTargetId = JSONUtil.getUintField(jsonObject, "stopBusTargetId");
                    if (stopSfxTargetId != 0)
                        targetData = AudioDataMgr.Instance.GetAudioSfxData(stopSfxTargetId, true);
                    else
                        targetData = AudioDataMgr.Instance.GetAudioBusData(stopBusTargetId, true);
                    stopFadeTime = JSONUtil.getFloatField(jsonObject, "stopFadeTime");
                    stopFadeOutCurveType = (HyAudioConst.CURVE_TYPE)JSONUtil.getIntField(jsonObject, "stopFadeOutCurveType");
                    break;
            }
        }

        #region Play事件属性

        //执行概率
        public int playProbability = 100;

        //淡入淡出事件
        public float playFadeTime;

        //淡入淡出曲线
        public HyAudioConst.CURVE_TYPE playFadeInCurveType = HyAudioConst.CURVE_TYPE.Linear;

        #endregion

        #region Stop事件属性

        //淡入淡出事件
        public float stopFadeTime;

        //淡入淡出曲线
        public HyAudioConst.CURVE_TYPE stopFadeOutCurveType = HyAudioConst.CURVE_TYPE.Linear;

        #endregion
    }
}