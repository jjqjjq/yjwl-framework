/*----------------------------------------------------------------
// 文件名：AudioDataMgr.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 16:47:41
//----------------------------------------------------------------*/

using System.Collections.Generic;
using JQCore.AudioEngine.Code.Object;
using JQCore.tJson;
using JQCore.tSingleton;
using UnityEngine;

namespace JQCore.AudioEngine.Code.Data
{
    public class AudioDataMgr : JQSingleton<AudioDataMgr>
    {
        //id -> data
        private readonly Dictionary<uint, AudioBusData> _busDataDic = new();

        //id -> data
        private readonly Dictionary<uint, AudioEventGroupData> _eventGroupDataDic = new();

        //eventName -> eventId
        private readonly Dictionary<string, uint> _eventNameDic = new();

        //id -> data
        private readonly Dictionary<uint, AudioSFXData> _sfxDataDic = new();

        public override void Dispose()
        {
            _busDataDic.Clear();
            _sfxDataDic.Clear();
            _eventGroupDataDic.Clear();
            _eventNameDic.Clear();
        }

        public void initData(JSONObject jsonObject)
        {
            //bus
            var busJsonObject = jsonObject.GetField("bus");
            for (var i = 0; i < busJsonObject.Count; i++)
            {
                var oneBusJsonObj = busJsonObject.list[i];
                var audioBusData = new AudioBusData(oneBusJsonObj);
                _busDataDic[audioBusData.id] = audioBusData;
                HyAudioObjectMgr.Instance.initBus(audioBusData);
            }

            //audio
            var audioJsonObject = jsonObject.GetField("audio");
            for (var i = 0; i < audioJsonObject.Count; i++)
            {
                var oneAudioJsonObj = audioJsonObject.list[i];
                var audioSfxData = new AudioSFXData(oneAudioJsonObj);
                Instance.addAudioData(audioSfxData);
            }

            //event
            var eventJsonObject = jsonObject.GetField("event");
            for (var i = 0; i < eventJsonObject.Count; i++)
            {
                var oneEventJsonObject = eventJsonObject.list[i];
                var audioEventGroupData = new AudioEventGroupData(oneEventJsonObject);
                _eventGroupDataDic[audioEventGroupData.id] = audioEventGroupData;
                _eventNameDic[audioEventGroupData.eventName] = audioEventGroupData.id;
            }
        }

        public void addAudioData(AudioSFXData audioSfxData)
        {
            _sfxDataDic[audioSfxData.id] = audioSfxData;
        }


        public AudioEventGroupData GetEventGroupData(string name)
        {
            if (!_eventNameDic.TryGetValue(name, out var id)) return null;
            if (!_eventGroupDataDic.TryGetValue(id, out var eventData)) return null;
            return eventData;
        }

        public AudioSFXData GetAudioSfxData(uint id, bool showError = false)
        {
            if (!_sfxDataDic.TryGetValue(id, out var sfxData))
            {
                if (showError) Debug.LogError("查无SFXData:" + id);
                return null;
            }

            return sfxData;
        }

        public AudioBusData GetAudioBusData(uint id, bool showError = false)
        {
            if (!_busDataDic.TryGetValue(id, out var busData))
            {
                if (showError) Debug.LogError("查无BusData:" + id);
                return null;
            }

            return busData;
        }
    }
}