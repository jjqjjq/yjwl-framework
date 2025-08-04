/*----------------------------------------------------------------
// 文件名：CheckSoundTolls.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2021/2/8 16:14:47
//----------------------------------------------------------------*/
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace JQEditor.Check
{
    public static class CheckSoundTools
    {
        public static void PutSoundToPrefab(string name, Action endAction)
        {
            CheckGlobalTools.addAssetToAtlasLib<AudioClip>("Sound", "Sound/HyAudioResLib", "t:AudioClip");
            CheckGlobalTools.addAssetToAtlasLib<AudioMixer>("Sound/Mixer", "Sound/HyAudioMixerResLib", "t:AudioMixer");
        }


      
    }
}
