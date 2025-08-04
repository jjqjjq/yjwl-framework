/*----------------------------------------------------------------
// 文件名：HyAndioInitializer.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 16:04:03
//----------------------------------------------------------------*/

using UnityEngine;

namespace JQCore.AudioEngine.Code
{
    public class HyAudioInitializer : MonoBehaviour
    {
        public static HyAudioInitializer instance;

        private void Awake()
        {
            instance = this;
            //
        }

        private void LateUpdate()
        {
            HyAudioEngineController.Instance.LateUpdate();
        }
    }
}