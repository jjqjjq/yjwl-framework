/*----------------------------------------------------------------
// 文件名：HyAudioEngineEnum.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 15:36:24
//----------------------------------------------------------------*/

namespace JQCore.AudioEngine
{
    public class HyAudioConst
    {
        public enum AM_CONTAINER
        {
            None = 0,
            Random = 1,
            Queue = 2,
        }

        public enum AM_EVENT_TYPE
        {
            Play = 1,
            Stop = 2,
            StopAll = 3,
            Pause = 4,
            PauseAll = 5,
            Resume = 6,
            ResumeAll = 7,
            Break = 8,

            SetBusVolume = 9,
            SetVoiceVolume = 10,
            Mute = 11,
            UnMute = 12
        }

        public enum AM_SCOPE
        {
            GameObject = 1,
            Global = 2
        }

        public enum CURVE_TYPE
        {
            Linear = 1
        }
        
        public const string Param_Pitch = "Pitch";
        public const string Param_QueueIndex = "QueueIndex";
    }
}