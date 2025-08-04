/*----------------------------------------------------------------
// 文件名：HyAudioBus.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/5 20:22:43
//----------------------------------------------------------------*/

using System.Collections.Generic;
using JQCore.AudioEngine.Code.Data;
using JQCore.AudioEngine.Code.Event;
using UnityEngine;
using UnityEngine.Audio;

namespace JQCore.AudioEngine.Code.Object
{
    public class HyAudioBus
    {
        private AudioBusData _audioBusData;
        private readonly List<HyAudioChannel> _channelList = new();

        public HyAudioBus(AudioBusData audioBusData, Transform parentTransform, AudioMixerGroup audioMixerGroup)
        {
            _audioBusData = audioBusData;
            busGo = new GameObject(audioBusData.path);
            busGo.transform.SetParent(parentTransform);
            for (var i = 0; i < audioBusData.limitInstance; i++)
            {
                var channel = new GameObject("channel");
                var audioSource = channel.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = audioMixerGroup;
                channel.transform.SetParent(busGo.transform);
                var hyAudioChannel = new HyAudioChannel(audioSource);
                _channelList.Add(hyAudioChannel);
            }
        }

        public GameObject busGo { get; }


        private HyAudioChannel getFreeChannel()
        {
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                if (hyAudioChannel.isFree) return hyAudioChannel;
            }

            return null;
        }

        public void LateUpdate()
        {
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                hyAudioChannel.LateUpdate();
            }
        }

        public void Play(AudioSFXData audioSfxData, HyAudioEvent hyAudioEvent)
        {
            //空闲音轨
            var hyAudioChannel = getFreeChannel();
            if (hyAudioChannel != null)
            {
                hyAudioChannel.Play(audioSfxData, hyAudioEvent);
                hyAudioEvent.AddChannel(hyAudioChannel);
                hyAudioChannel.LinkEvent = hyAudioEvent;
            }
            else
            {
                //总线音轨不足
                hyAudioChannel = getOldestChannel();
                if (hyAudioChannel != null)
                {
                    hyAudioChannel.Play(audioSfxData, hyAudioEvent);
                    hyAudioEvent.AddChannel(hyAudioChannel);
                    hyAudioChannel.LinkEvent = hyAudioEvent;
                }
            }

            //检查同音效是否超数量播放
            if (audioSfxData.limitInstance > 0)
            {
                var needFreeAudioChannel = getNeedFreeChannel(audioSfxData);
                if (needFreeAudioChannel != null) needFreeAudioChannel.Stop();
            }
        }

        public void collectUnFreeChannel(AudioSFXData audioSfxData, List<HyAudioChannel> channelList)
        {
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                if (hyAudioChannel.isFree) continue;
                if (hyAudioChannel.SfxData != audioSfxData) continue;
                channelList.Add(hyAudioChannel);
            }
        }


        public void collectUnFreeChannel(List<HyAudioChannel> channelList)
        {
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                if (!hyAudioChannel.isFree) channelList.Add(hyAudioChannel);
            }
        }

        private HyAudioChannel getOldestChannel()
        {
            var time = Time.time;
            HyAudioChannel oldestAudioChannel = null;
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                if (!hyAudioChannel.isFree)
                    if (hyAudioChannel.LinkEvent.ResetTime < time)
                    {
                        oldestAudioChannel = hyAudioChannel;
                        time = hyAudioChannel.LinkEvent.ResetTime;
                    }
            }

            return oldestAudioChannel;
        }

        private HyAudioChannel getNeedFreeChannel(AudioSFXData audioSfxData)
        {
            var time = Time.time;
            HyAudioChannel oldestAudioChannel = null;
            var count = 0;
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                if (hyAudioChannel.isFree) continue;
                if (hyAudioChannel.SfxData != audioSfxData) continue;
                count++;

                if (hyAudioChannel.LinkEvent.ResetTime < time)
                {
                    oldestAudioChannel = hyAudioChannel;
                    time = hyAudioChannel.LinkEvent.ResetTime;
                }
            }

            if (count > audioSfxData.limitInstance) return oldestAudioChannel;
            return null;
        }

        public void Stop(AudioSFXData audioSfxData)
        {
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                if (hyAudioChannel.AudioClip != null)
                    if (audioSfxData == hyAudioChannel.SfxData)
                        hyAudioChannel.Stop();
            }
        }

        public void SetVolume(AudioSFXData audioSfxData, float volume)
        {
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                if (hyAudioChannel.AudioClip != null)
                    if (audioSfxData == hyAudioChannel.SfxData)
                        hyAudioChannel.SetVolume(volume);
            }
        }

        public void SetVolumeAll(float volume)
        {
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                if (hyAudioChannel.AudioClip != null) hyAudioChannel.SetVolume(volume);
            }
        }

        public void StopAll()
        {
            for (var i = 0; i < _channelList.Count; i++)
            {
                var hyAudioChannel = _channelList[i];
                hyAudioChannel.Stop();
            }
        }
    }
}