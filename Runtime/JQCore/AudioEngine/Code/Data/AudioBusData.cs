/*----------------------------------------------------------------
// 文件名：AudioBusData.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 16:58:51
//----------------------------------------------------------------*/

using System.Collections.Generic;
using JQCore.AudioEngine.Code.Event;
using JQCore.AudioEngine.Code.Object;
using JQCore.tJson;

namespace JQCore.AudioEngine.Code.Data
{
    public class AudioBusData : AudioData
    {
        public uint id;
        public uint limitInstance = 99;
        public string path;

        public AudioBusData(JSONObject jsonObject)
        {
            id = JSONUtil.getUintField(jsonObject, "id");
            path = JSONUtil.getStrField(jsonObject, "path");
            limitInstance = JSONUtil.getUintField(jsonObject, "limitInstance", 24);
        }

        public override void stop(HyAudioEvent hyAudioEvent)
        {
            var hyAudioBus = HyAudioObjectMgr.Instance.GetAudioBus(path);
            if (hyAudioBus != null) hyAudioBus.StopAll();
        }

        public override void collectFreeChannel(HyAudioEvent hyAudioEvent, List<HyAudioChannel> channelList)
        {
        }

        public override void collectUnFreeChannel(HyAudioEvent hyAudioEvent, List<HyAudioChannel> channelList)
        {
            var hyAudioBus = HyAudioObjectMgr.Instance.GetAudioBus(path);
            if (hyAudioBus != null) hyAudioBus.collectUnFreeChannel(channelList);
        }

        public override void setVolume(HyAudioEvent hyAudioEvent, float volume)
        {
            var hyAudioBus = HyAudioObjectMgr.Instance.GetAudioBus(path);
            if (hyAudioBus != null) hyAudioBus.SetVolumeAll(volume);
        }
    }
}