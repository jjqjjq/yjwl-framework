/*----------------------------------------------------------------
// 文件名：AudioSFXData.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 16:59:03
//----------------------------------------------------------------*/

using System.Collections.Generic;
using framework.AudioEngine.Code.Param;
using JQCore.AudioEngine.Code.Event;
using JQCore.AudioEngine.Code.Object;
using JQCore.tJson;
using UnityEngine;

namespace JQCore.AudioEngine.Code.Data
{
    public class AudioSFXData : AudioData
    {
        private readonly int[] _randomArr;

        private readonly AudioSFXData[] _subDatas;

        public string audioClipPath;
        public AudioBusData busData;
        public HyAudioConst.AM_CONTAINER containerType = HyAudioConst.AM_CONTAINER.None;
        public uint id;
        public uint limitInstance = 99;
        public bool loop;
        public int randomWeight;
        //音调变化
        public float pitchIncrease = 0f;

        public AudioSFXData(JSONObject jsonObject)
        {
            id = JSONUtil.getUintField(jsonObject, "id");
            var busId = JSONUtil.getUintField(jsonObject, "busId");
            busData = AudioDataMgr.Instance.GetAudioBusData(busId);
            if (busData == null) Debug.LogError("bus is null：" + jsonObject);
            containerType = (HyAudioConst.AM_CONTAINER)JSONUtil.getIntField(jsonObject, "containerType");
            randomWeight = JSONUtil.getIntField(jsonObject, "randomWeight");
            loop = JSONUtil.getBoolField(jsonObject, "loop");
            pitchIncrease = JSONUtil.getFloatField(jsonObject, "pitchIncrease");

            audioClipPath = JSONUtil.getStrField(jsonObject, "audioClipPath");

            var childJsonObject = jsonObject.GetField("child");
            if (childJsonObject != null)
            {
                _subDatas = new AudioSFXData[childJsonObject.Count];
                _randomArr = new int[childJsonObject.Count];
                for (var i = 0; i < childJsonObject.Count; i++)
                {
                    var subJsonObject = childJsonObject.list[i];
                    var subAudioSfxData = new AudioSFXData(subJsonObject);
                    _subDatas[i] = subAudioSfxData;
                    _randomArr[i] = subAudioSfxData.randomWeight;
                    AudioDataMgr.Instance.addAudioData(subAudioSfxData);
                }
            }

            limitInstance = JSONUtil.getUintField(jsonObject, "limitInstance");
        }

        public override void play(HyAudioEvent hyAudioEvent)
        {
            switch (containerType)
            {
                case HyAudioConst.AM_CONTAINER.None:
                    var hyAudioBus1 = HyAudioObjectMgr.Instance.GetAudioBus(busData.path);
                    if (hyAudioBus1 != null) hyAudioBus1.Play(this, hyAudioEvent);
                    break;
                case HyAudioConst.AM_CONTAINER.Random:
                    var index2 = randomByQuality(_randomArr);
                    var subAudioSfxData2 = _subDatas[index2];
                    var hyAudioBus2 = HyAudioObjectMgr.Instance.GetAudioBus(busData.path);
                    if (hyAudioBus2 != null) hyAudioBus2.Play(subAudioSfxData2, hyAudioEvent);
                    break;
                case HyAudioConst.AM_CONTAINER.Queue:
                    int index3 = HyAudioParamMgr.Instance.GetParamInt(hyAudioEvent.AudioEventData.eventName, HyAudioConst.Param_QueueIndex, 0);
                    var subAudioSfxData3 = _subDatas[index3];
                    index3++;
                    if(index3 >= _subDatas.Length) index3 = _subDatas.Length - 1;
                    HyAudioParamMgr.Instance.SetParamInt(hyAudioEvent.AudioEventData.eventName, HyAudioConst.Param_QueueIndex, index3);
                    var hyAudioBus3 = HyAudioObjectMgr.Instance.GetAudioBus(busData.path);
                    if (hyAudioBus3 != null) hyAudioBus3.Play(subAudioSfxData3, hyAudioEvent);
                    
                    break;
                default:
                    Debug.LogError("未实现的container：" + containerType);
                    break; 
            }
        }

        public override void stop(HyAudioEvent hyAudioEvent)
        {
            var hyAudioBus = HyAudioObjectMgr.Instance.GetAudioBus(busData.path);
            if (hyAudioBus != null) hyAudioBus.Stop(this);
        }

        public override void collectUnFreeChannel(HyAudioEvent hyAudioEvent, List<HyAudioChannel> channelList)
        {
            var hyAudioBus = HyAudioObjectMgr.Instance.GetAudioBus(busData.path);
            if (hyAudioBus != null) hyAudioBus.collectUnFreeChannel(this, channelList);
        }

        public override void setVolume(HyAudioEvent hyAudioEvent, float volume)
        {
            var hyAudioBus = HyAudioObjectMgr.Instance.GetAudioBus(busData.path);
            if (hyAudioBus != null) hyAudioBus.SetVolume(this, volume);
        }

        public static int randomByQuality(int[] qualityWeight)
        {
            var maxWeight = 0;
            for (var i = 0; i < qualityWeight.Length; i++) maxWeight += qualityWeight[i];

            int randomVal;
            randomVal = Random.Range(0, maxWeight);
            var temp = 0;
            for (var i = 0; i < qualityWeight.Length; i++)
            {
                temp += qualityWeight[i];
                if (randomVal < temp) return i;
            }

            Debug.LogError("奇怪了");
            return 0;
        }
    }
}