/*----------------------------------------------------------------
// 文件名：HyAudioEvent.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 16:35:48
//----------------------------------------------------------------*/

using JQCore.AudioEngine.Code.Data;
using UnityEngine;

namespace JQCore.AudioEngine.Code.Event
{
    public class HyAudioEventGroup
    {
        private static uint globalInstanceId;
        private readonly AudioEventGroupData _audioEventGroupData;
        private readonly HyAudioEvent[] _audioEvents;
        public uint instanceId;

        public HyAudioEventGroup(AudioEventGroupData audioEventGroupData)
        {
            _audioEventGroupData = audioEventGroupData;

            _audioEvents = new HyAudioEvent[audioEventGroupData.audioEventDatas.Length];
            for (var i = 0; i < audioEventGroupData.audioEventDatas.Length; i++)
            {
                var audioEventData = audioEventGroupData.audioEventDatas[i];
                HyAudioEvent hyAudioEvent = null;
                switch (audioEventData.eventType)
                {
                    case HyAudioConst.AM_EVENT_TYPE.Play:
                        hyAudioEvent = new HyAudioEvent_Play(audioEventData);
                        break;
                    case HyAudioConst.AM_EVENT_TYPE.Stop:
                        hyAudioEvent = new HyAudioEvent_Stop(audioEventData);
                        break;
                }

                _audioEvents[i] = hyAudioEvent;
            }
        }

        public string eventName => _audioEventGroupData.eventName;

        public bool isAllAudioEnd
        {
            get
            {
                var isAllAudioEnd = true;
                for (var i = 0; i < _audioEvents.Length; i++)
                {
                    var hyAudioEvent = _audioEvents[i];
                    if (!hyAudioEvent.isAllAudioEnd) isAllAudioEnd = false;
                }

                return isAllAudioEnd;
            }
        }

        public void reset()
        {
            instanceId = globalInstanceId++;
            for (var i = 0; i < _audioEvents.Length; i++)
            {
                var hyAudioEvent = _audioEvents[i];
                hyAudioEvent.reset();
            }
        }

        public void start(GameObject gameObject)
        {
            for (var i = 0; i < _audioEvents.Length; i++)
            {
                var hyAudioEvent = _audioEvents[i];
                hyAudioEvent.start(gameObject);
            }
        }

        public void LateUpdate()
        {
            for (var i = 0; i < _audioEvents.Length; i++)
            {
                var hyAudioEvent = _audioEvents[i];
                hyAudioEvent.LateUpdate();
            }
        }

        public void stop()
        {
            for (var i = 0; i < _audioEvents.Length; i++)
            {
                var hyAudioEvent = _audioEvents[i];
                hyAudioEvent.stop();
            }
        }
    }
}