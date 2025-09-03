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
        private Dictionary<long, JQEntity> _entityDic = new Dictionary<long, JQEntity>();
        
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

        public T getEntity<T>(long id) where T : JQEntity
        {
            return _entityDic[id] as T;
        }

        public void addEntity(JQEntity entity)
        {
            entity.entityMgr = this;
            entity.worldMgr = _worldMgr;
            entity.onAwake();
            _entityList.Add(entity);
            _entityDic.Add(entity.ID, entity);

            if (_isPlaying)
            {
                entity.onStart();
            }

            _listChange = true;
        }

        public T removeEntity<T>(long id) where T : JQEntity
        {
            _entityDic.TryGetValue(id, out JQEntity entity);
            if (entity == null) return null;
            _entityList.Remove(entity);
            _entityDic.Remove(id);
            entity.Dispose();
            _listChange = true;
            return entity as T;
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
                entity.Dispose();
                // ObjectPool.Instance.Recycle(entity);
            }
            _entityDic.Clear();
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