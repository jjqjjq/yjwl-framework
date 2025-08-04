/*----------------------------------------------------------------
// 文件名：HyAudioEvent_Stop.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/8 11:39:08
//----------------------------------------------------------------*/

using JQCore.AudioEngine.Code.Data;
using UnityEngine;

namespace JQCore.AudioEngine.Code.Event
{
    public class HyAudioEvent_Stop : HyAudioEvent
    {
        private float _startFadeTime;

        public HyAudioEvent_Stop(AudioEventData audioEventData) : base(audioEventData)
        {
        }

        public override void reset()
        {
            base.reset();
            _startFadeTime = 0;
        }

        protected override void execute()
        {
            var data = _audioEventData.targetData;
            if (_audioEventData.stopFadeTime == 0)
            {
                _isFinish = true;
                data.stop(this);
            }
            else if (_startFadeTime == 0)
            {
                _startFadeTime = Time.time;
                data.collectUnFreeChannel(this, _channelList);
            }

            //淡入淡出
            if (_audioEventData.stopFadeTime > 0)
            {
                var fadePassTime = Time.time - _startFadeTime;
                var precent = fadePassTime / _audioEventData.stopFadeTime;
                if (precent >= 1)
                {
//                    Debug.Log("stop fade finish");
                    stop();
                    _isFinish = true;
                }
                else
                {
//                    Debug.Log("stop fade precent:" + (1 - precent));
                    setVolume(1 - precent);
                }
            }
        }
    }
}