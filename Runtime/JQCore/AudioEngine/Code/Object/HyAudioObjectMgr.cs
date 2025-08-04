/*----------------------------------------------------------------
// 文件名：HyAudioObjectMgr.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 16:12:59
//----------------------------------------------------------------*/

using System.Collections.Generic;
using JQCore.AudioEngine.Code.Data;
using JQCore.tSingleton;
using JQCore.tRes;
using UnityEngine;
using UnityEngine.Audio;

namespace JQCore.AudioEngine.Code.Object
{
    public class HyAudioObjectMgr : JQSingleton<HyAudioObjectMgr>
    {
        private readonly Dictionary<string, HyAudioBus> _audioBusDic = new();
        private readonly Dictionary<string, AudioClip> _audioClipDic = new();
        private AudioMixer _audioMixer;
        private Transform _root;

        public HyAudioObjectMgr()
        {
            var root = new GameObject("HyAudioObjectRoot");
            _root = root.transform;
            UnityEngine.Object.DontDestroyOnLoad(root);
        }

        public override void Dispose()
        {
            _audioBusDic.Clear();
            _audioClipDic.Clear();
            _audioMixer = null;
            _root = null;
        }

        public void LateUpdate()
        {
            foreach (var keyValuePair in _audioBusDic)
            {
                var hyAudioBus = keyValuePair.Value;
                hyAudioBus.LateUpdate();
            }
        }

        public void initMixer(AssetObjectLib mixerResLib)
        {
            _audioMixer = mixerResLib.assets[0] as AudioMixer;
        }

        public void SetBusVolume(string paramName, float val)
        {
            _audioMixer.SetFloat(paramName, val);
        }

        public void initAudio(AssetObjectLib resLib)
        {
            for (var i = 0; i < resLib.assets.Length; i++)
            {
                if (resLib.assets[i] == null) continue;
                var audioClip = resLib.assets[i] as AudioClip;
                _audioClipDic[audioClip.name] = audioClip;
            }
        }

        public void initBus(AudioBusData audioBusData)
        {
            var audioMixerGroups = _audioMixer.FindMatchingGroups(audioBusData.path);
            if (audioMixerGroups.Length == 0)
            {
                Debug.LogError("找不到总线AudioMixerGroup：" + audioBusData.path);
                return;
            }

            var hyAudioBus = new HyAudioBus(audioBusData, _root, audioMixerGroups[0]);
            _audioBusDic[audioBusData.path] = hyAudioBus;
        }

        public HyAudioBus GetAudioBus(string path)
        {
            HyAudioBus hyAudioBus = null;
            if (!_audioBusDic.TryGetValue(path, out hyAudioBus))
            {
                Debug.LogError("can not find bus path:" + path);
                return null;
            }

            return hyAudioBus;
        }

        public AudioClip GetAudioClip(string name)
        {
            AudioClip audioClip = null;
            if (!_audioClipDic.TryGetValue(name, out audioClip)) Debug.LogError("查无AudioClip:" + name);

            return audioClip;
        }
    }
}