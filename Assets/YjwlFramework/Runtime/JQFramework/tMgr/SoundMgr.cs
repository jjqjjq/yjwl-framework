using JQCore.AudioEngine.Code;
using JQCore;
using JQCore.tMgr;
using JQCore.tLog;
using JQCore.tSingleton;
using UnityEngine;

namespace JQFramework.tMgr
{
    public class ESoundLocalVar
    {
        public const string SoundVolumeVal = "SoundVolumeVal"; //音效音量
        public const string MusicVolumeVal = "MusicVolumeVal"; //音效音量
        public const string MasterVolumeVal = "MasterVolumeVal"; //音效音量
        public const string Setting_Quality = "Setting_Quality"; //画质
    }
    
    public class SoundMgr : JQSingleton<SoundMgr>
    {
        
        public void init()
        {
            //纯本地设置存储
            float curMasterVolumeVal = LocalVarManager.GetGlobalFloat(ESoundLocalVar.MasterVolumeVal, 1f);
            setVolume("MasterVolume", curMasterVolumeVal);
            float curMusicVolumeVal = LocalVarManager.GetGlobalFloat(ESoundLocalVar.MusicVolumeVal, 1f);
            setVolume("BGMVolume", curMusicVolumeVal);
            float curSFXVolumeVal = LocalVarManager.GetGlobalFloat(ESoundLocalVar.SoundVolumeVal, 1f);
            setVolume("SFXVolume", curSFXVolumeVal);
        }

        public void setMasterVolume(float volume)
        {
            setVolume("MasterVolume", volume);
            LocalVarManager.SetGlobalFloat(ESoundLocalVar.MasterVolumeVal, volume);
        }
        
        public void setBgmVolume(float volume)
        {
            setVolume("BGMVolume", volume);
            LocalVarManager.SetGlobalFloat(ESoundLocalVar.MusicVolumeVal, volume);
        }

        public void setSfxVolume(float volume)
        {
            setVolume("SFXVolume", volume);
            LocalVarManager.SetGlobalFloat(ESoundLocalVar.SoundVolumeVal, volume);
        }

        public void setVolume(string volumeType, float volume)
        {
            //把0.0001-1 转为 -80-0
            volume = Mathf.Max(volume, 0.0001f);
            volume = Mathf.Min(volume, 1f);
            float busVolume = Mathf.Log10(volume) * 20;
            HyAudioEngine.SetBusVolume(volumeType, busVolume);
        }
        
        public void PostEvent(string eventName, GameObject gameObject = null)
        {
            JQLog.LogWarning("SoundMgr.PostEvent: " + eventName);
            HyAudioEngine.PostEvent(eventName, gameObject);
        }
        
        public void ClearEventParam(string eventName)
        {
            JQLog.LogWarning("SoundMgr.ClearEventParam: " + eventName);
            HyAudioEngine.ClearEventParam(eventName);
        }

        public override void Dispose()
        {
            
        }
    }
}