/*----------------------------------------------------------------
// 文件名：AudioData.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/5 16:27:56
//----------------------------------------------------------------*/

using System.Collections.Generic;
using JQCore.AudioEngine.Code.Event;
using JQCore.AudioEngine.Code.Object;

namespace JQCore.AudioEngine.Code.Data
{
    public class AudioData
    {
        public virtual void play(HyAudioEvent hyAudioEvent)
        {
        }

        public virtual void stop(HyAudioEvent hyAudioEvent)
        {
        }

        public virtual void setVolume(HyAudioEvent hyAudioEvent, float volume)
        {
        }

        public virtual void collectFreeChannel(HyAudioEvent hyAudioEvent, List<HyAudioChannel> channelList)
        {
        }

        public virtual void collectUnFreeChannel(HyAudioEvent hyAudioEvent, List<HyAudioChannel> channelList)
        {
        }
    }
}