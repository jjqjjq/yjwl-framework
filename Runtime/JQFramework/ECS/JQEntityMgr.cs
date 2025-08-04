using System.Collections.Generic;
using ET;
using JQCore.ECS;
using JQCore.tLog;

namespace JQFramework.ECS
{
    public class JQEntityMgr
    {
        private IWorldMgr _worldMgr;
        private List<JQEntity> _entityList = new List<JQEntity>();
        private bool _isPlaying;
        private bool _listChange;
        private List<JQEntity> _tempList = new List<JQEntity>();
        
        public JQEntityMgr(IWorldMgr worldMgr)
        {
            _worldMgr = worldMgr;
            _isPlaying = false;
            _listChange = true;
        }

        public List<JQEntity> getAllEntity()
        {
            return _entityList;
        }

        public JQEntity getEntity(int instanceId)
        {
            for (int i = 0; i < _entityList.Count; i++)
            {
                JQEntity entity = _entityList[i];
                if (entity.InstanceId == instanceId)
                {
                    return entity;
                }
            }
            return null;
        }

        public void addEntity(JQEntity entity)
        {
            entity.entityMgr = this;
            entity.worldMgr = _worldMgr;
            entity.onAwake();
            _entityList.Add(entity);

            if (_isPlaying)
            {
                entity.onStart();
            }

            _listChange = true;
        }

        public void removeEntity(JQEntity entity)
        {
            if (entity == null) return;
            _entityList.Remove(entity);
            entity.onReset();
            ObjectPool.Instance.Recycle(entity);
            _listChange = true;
        }

        public void start()
        {
            if (_isPlaying) return;
            _isPlaying = true;
            for (int i = 0; i < _entityList.Count; i++)
            {
                JQEntity entity = _entityList[i];
                entity.onStart();

            }
        }

        public void pause()
        {
            for (int i = 0; i < _entityList.Count; i++)
            {
                JQEntity entity = _entityList[i];
                entity.onPause();
            }
        }

        public void rerfreshTempList()
        {
            if (_listChange)
            {
                _tempList.Clear();
                _tempList.AddRange(_entityList);
                _listChange = false;
            }
        }

        public void onUpdate()
        {
            rerfreshTempList();
            for (int i = 0; i < _tempList.Count; i++)
            {
                JQEntity entity = _tempList[i];
                entity.onUpdate();
            }
        }
        
        public void onLateUpdate()
        {
            for (int i = 0; i < _tempList.Count; i++)
            {
                JQEntity entity = _tempList[i];
                entity.onLateUpdate();
            }
        }
        
        public void onDisplayUpdate()
        {
            for (int i = 0; i < _tempList.Count; i++)
            {
                JQEntity entity = _tempList[i];
                entity.onDisplayUpdate();
            }
        }

        public void clear()
        {
            JQLog.Log("JQEntityMgr clear");
            for (int i = 0; i < _entityList.Count; i++)
            {
                JQEntity entity = _entityList[i];
                entity.onReset();
                ObjectPool.Instance.Recycle(entity);
            }
            _tempList.Clear();
            _entityList.Clear();
            _isPlaying = false;
            _listChange = true;
            _worldMgr = null;
        }

        public void Dispose()
        {
            clear();
        }
    }
}