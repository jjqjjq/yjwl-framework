/*----------------------------------------------------------------
// 文件名：HySoundMgr.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 15:55:05
//----------------------------------------------------------------*/

using System.Collections.Generic;
using framework.AudioEngine.Code.Param;
using JQCore.AudioEngine.Code.Data;
using JQCore.tSingleton;
using UnityEngine;

namespace JQCore.AudioEngine.Code.Event
{
    public class HyAudioEventGroupMgr : JQSingleton<HyAudioEventGroupMgr>
    {
        private readonly List<HyAudioEventGroup> _list = new();
        private readonly Dictionary<string, Queue<HyAudioEventGroup>> _poolDic = new();
        private readonly List<HyAudioEventGroup> _removeList = new();

        public override void Dispose()
        {
            _list.Clear();
            _poolDic.Clear();
            _removeList.Clear();
        }

        public uint PostEvent(string eventName, GameObject gameObject)
        {
            var hyAudioEventGroup = spawnEventGroup(eventName);
            if (hyAudioEventGroup != null)
            {
                hyAudioEventGroup.reset();
                hyAudioEventGroup.start(gameObject);
                _list.Add(hyAudioEventGroup);
                return hyAudioEventGroup.instanceId;
            }

            return 0;
        }
        
        public void ClearEventParam(string eventName)
        {
            HyAudioParamMgr.Instance.ClearAllParams(eventName);
        }

        public void StopPlayingID(uint in_playingID, int in_uTransitionDuration, HyAudioConst.CURVE_TYPE in_eFadeCurve)
        {
            for (var i = 0; i < _list.Count; i++)
            {
                var hyAudioEventGroup = _list[i];
                if (hyAudioEventGroup.instanceId == in_playingID) hyAudioEventGroup.stop();
            }
        }

        public void LateUpdate()
        {
            for (var i = 0; i < _list.Count; i++)
            {
                var hyAudioEventGroup = _list[i];
                hyAudioEventGroup.LateUpdate();
                if (hyAudioEventGroup.isAllAudioEnd) _removeList.Add(hyAudioEventGroup);
            }

            for (var i = 0; i < _removeList.Count; i++)
            {
                var hyAudioEventGroup = _removeList[i];
                DespawnEventGroup(hyAudioEventGroup);
                _list.Remove(hyAudioEventGroup);
            }

            _removeList.Clear();
        }


        private Queue<HyAudioEventGroup> getPool(string eventName)
        {
            Queue<HyAudioEventGroup> pool = null;
            if (!_poolDic.TryGetValue(eventName, out pool))
            {
                pool = new Queue<HyAudioEventGroup>();
                _poolDic[eventName] = pool;
            }

            return pool;
        }

        private HyAudioEventGroup spawnEventGroup(string eventName)
        {
//            Debug.Log("spawnEventGroup:" + eventName);
            var pool = getPool(eventName);
            if (pool.Count == 0)
            {
                var audioEventGroupData = AudioDataMgr.Instance.GetEventGroupData(eventName);
                if (audioEventGroupData == null)
                {
                    Debug.LogError("[HyAudio]未知的事件名称：" + eventName);
                    return null;
                }

                return new HyAudioEventGroup(audioEventGroupData);
            }

            return pool.Dequeue();
        }

        private void DespawnEventGroup(HyAudioEventGroup hyAudioEventGroup)
        {
//            Debug.Log("DespawnEventGroup:"+hyAudioEventGroup.eventName);
            var pool = getPool(hyAudioEventGroup.eventName);
            pool.Enqueue(hyAudioEventGroup);
        }
    }
}