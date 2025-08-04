/*----------------------------------------------------------------
// 文件名：AkSoundEngineController.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 16:05:35
//----------------------------------------------------------------*/

using JQCore.AudioEngine.Code.Event;
using JQCore.AudioEngine.Code.Object;

namespace JQCore.AudioEngine.Code
{
    public class HyAudioEngineController
    {
        private static HyAudioEngineController ms_Instance;

        public static HyAudioEngineController Instance
        {
            get
            {
                if (ms_Instance == null)
                    ms_Instance = new HyAudioEngineController();

                return ms_Instance;
            }
        }

        public void LateUpdate()
        {
            HyAudioObjectMgr.Instance.LateUpdate();
            HyAudioEventGroupMgr.Instance.LateUpdate();
        }
    }
}