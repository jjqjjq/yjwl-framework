/*----------------------------------------------------------------
// 文件名：HyAudioChannel.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/5 20:22:22
//----------------------------------------------------------------*/

using framework.AudioEngine.Code.Param;
using JQCore.AudioEngine.Code.Data;
using JQCore.AudioEngine.Code.Event;
using UnityEngine;

namespace JQCore.AudioEngine.Code.Object
{
    public class HyAudioChannel
    {
        private readonly GameObject _audioGo;
        private AudioSource _audioSource;
        private readonly Transform _audioTrans;
        private GameObject _bindGo;

        public HyAudioChannel(AudioSource audioSource)
        {
            _audioSource = audioSource;
            _audioGo = audioSource.gameObject;
            _audioTrans = audioSource.transform;
        }

        public AudioSFXData SfxData { get; private set; }


        public AudioClip AudioClip { get; private set; }

        public HyAudioEvent LinkEvent { get; set; }


        public bool isFree => _audioSource.clip == null;

        public bool isMute
        {
            get => _audioSource.mute;
            set => _audioSource.mute = value;
        }

        public void SetVolume(float volume)
        {
            _audioSource.volume = volume;
        }

        public void setAudioSource(AudioSource audioSource)
        {
            if (audioSource == null) Debug.LogError("找不到AudioSource");

            _audioSource = audioSource;
        }

        //播放（继续之前的播放进度）.
        public void Resume()
        {
            _audioSource.Play();
        }

        public void Stop()
        {
            freeThis();
        }

        //播放一段新的声音.
        public void Play(AudioSFXData sfxData, HyAudioEvent hyAudioEvent)
        {
            freeThis();
            _bindGo = hyAudioEvent.BindGo;
            if (_bindGo != null) _audioTrans.position = _bindGo.transform.position;

            SfxData = sfxData;
            AudioClip = HyAudioObjectMgr.Instance.GetAudioClip(sfxData.audioClipPath);
            _audioSource.clip = AudioClip;
#if UNITY_EDITOR
            _audioGo.name = "channel_" + AudioClip.name;
#endif
            _audioSource.loop = sfxData.loop;
            if (sfxData.pitchIncrease != 0)
            {
                float currEventPitch = HyAudioParamMgr.Instance.GetParamFloat(hyAudioEvent.AudioEventData.eventName, HyAudioConst.Param_Pitch, 1f);
                currEventPitch += sfxData.pitchIncrease;
                _audioSource.pitch = currEventPitch;
                HyAudioParamMgr.Instance.SetParamFloat(hyAudioEvent.AudioEventData.eventName, HyAudioConst.Param_Pitch, currEventPitch);
            }
            else
            {
                HyAudioParamMgr.Instance.RemoveParamFloat(hyAudioEvent.AudioEventData.eventName, HyAudioConst.Param_Pitch);
            }
            
            _audioSource.Play();
        }

        public void Pause()
        {
            _audioSource.Pause();
        }

        private void freeThis()
        {
            if (LinkEvent != null)
            {
                LinkEvent.RemoveChannel(this);
                LinkEvent = null;
                _audioSource.Stop();
                _audioSource.clip = null;
                _audioSource.pitch = 1;
#if UNITY_EDITOR
                _audioGo.name = "channel";
#endif
                SfxData = null;
                _bindGo = null;
                _audioTrans.localPosition = Vector3.zero;
            }
        }

        public void LateUpdate()
        {
            if (_audioSource!=null && _audioSource.clip != null)
            {
                if (_bindGo != null) _audioTrans.position = _bindGo.transform.position;

                if (!_audioSource.isPlaying) freeThis();
            }
        }
    }
}