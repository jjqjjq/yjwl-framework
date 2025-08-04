/*----------------------------------------------------------------
// 文件名：HyAudioEvent.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 17:48:51
//----------------------------------------------------------------*/

using System.Collections.Generic;
using JQCore.AudioEngine.Code.Data;
using JQCore.AudioEngine.Code.Object;
using UnityEngine;

namespace JQCore.AudioEngine.Code.Event
{
    public class HyAudioEvent
    {
        protected AudioEventData _audioEventData;
        protected GameObject _bindGo;
        protected List<HyAudioChannel> _channelList = new();
        protected bool _isFinish;
        protected float _resetTime;

        private float _startFadeTime;


        public HyAudioEvent(AudioEventData audioEventData)
        {
            _audioEventData = audioEventData;
        }

        public GameObject BindGo => _bindGo;

        public float ResetTime => _resetTime;

        public bool isAllAudioEnd => _channelList.Count == 0;

        public AudioEventData AudioEventData => _audioEventData;

        public void AddChannel(HyAudioChannel hyAudioChannel)
        {
            _channelList.Add(hyAudioChannel);
        }

        public void RemoveChannel(HyAudioChannel hyAudioChannel)
        {
            _channelList.Remove(hyAudioChannel);
        }

        public virtual void reset()
        {
            _bindGo = null;
            _isFinish = false;
            _resetTime = Time.time;
            _channelList.Clear();
        }

        public void start(GameObject gameObject)
        {
            _bindGo = gameObject;
            if (_audioEventData.delay == 0) execute();
        }

        public void LateUpdate()
        {
            //已执行
            if (_isFinish) return;
            //延时执行
            if (Time.time - _resetTime < _audioEventData.delay) return;
            execute();
        }

        protected virtual void execute()
        {
            Debug.LogError("[音频]未实现的事件类型：" + _audioEventData.eventType);
        }

        public void stop()
        {
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                hyAudioChannel.Stop();
            }
        }

        public void setVolume(float volume)
        {
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                hyAudioChannel.SetVolume(volume);
            }
        }
    }
}