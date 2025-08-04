/*----------------------------------------------------------------
// 文件名：HyAudioEvent_Play.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/8 11:35:56
//----------------------------------------------------------------*/

using JQCore.AudioEngine.Code.Data;
using UnityEngine;

namespace JQCore.AudioEngine.Code.Event
{
    public class HyAudioEvent_Play : HyAudioEvent
    {
        private bool _hadPlay;
        private float _startFadeTime;

        public HyAudioEvent_Play(AudioEventData audioEventData) : base(audioEventData)
        {
        }

        public override void reset()
        {
            base.reset();
            _startFadeTime = 0;
            _hadPlay = false;
        }

        protected override void execute()
        {
            //还未播放
            if (_channelList.Count == 0)
            {
                var data = _audioEventData.targetData;
                if (!_hadPlay)
                {
                    data.play(this);
                    _hadPlay = true;
                }

                if (_audioEventData.playFadeTime == 0)
                    _isFinish = true;
                else if (_startFadeTime == 0) _startFadeTime = Time.time;
            }

            //淡入淡出
            if (_audioEventData.playFadeTime > 0)
            {
                var fadePassTime = Time.time - _startFadeTime;
                var precent = fadePassTime / _audioEventData.playFadeTime;
                if (precent >= 1)
                {
//                    Debug.Log("play fade finish");
                    setVolume(1);
                    _isFinish = true;
                }
                else
                {
//                    Debug.Log("play fade precent:" + precent);
                    setVolume(precent);
                }
            }
        }
    }
}