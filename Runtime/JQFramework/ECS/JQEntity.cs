using System;
using System.Collections.Generic;
using JQCore.tEvent;
using JQCore.tLog;
using JQCore.tTime;
using JQFramework.ECS;

namespace JQCore.ECS
{
    public enum EntityType
    {
        Entity,
        MapGrid,
        Building,
    }

    public class JQEntity
    {
        public static string Entity_RESET = "RESET";
        public static string Entity_DISPOSE = "DISPOSE";
        private static int instanceId = 1;

        private int _instanceId;
        private long _id;

        public long ID
        {
            get => _id;
            set => _id = value;
        }

        public JQEntityMgr entityMgr;
        public IWorldMgr worldMgr;

        public int InstanceId => _instanceId;

        public JQEventDispatcher Event => _eventDispatcher;

        private JQEventDispatcher _eventDispatcher;
        private GameTime _gameTime;
        private List<JQComponent> _componentList;
        private List<IComponentUpdate> _componentUpdateList;
        private List<IComponentLateUpdate> _componentLateUpdateList;
        private List<IComponentDisplayUpdate> _componentDisplayUpdateList;
        private Dictionary<Type, JQComponent> _componentDic;

        public JQEntity()
        {
            instanceId++;
            _instanceId = instanceId;
            _eventDispatcher = new JQEventDispatcher();

            _gameTime = null;
            _componentList = new List<JQComponent>();
            _componentDic = new Dictionary<Type, JQComponent>();
        }

        public T AddComponent<T>() where T : JQComponent, new()
        {
            T component = new T();
            component.SetEntity(this);
            _componentList.Add(component);
            _componentDic.Add(typeof(T), component);
            return component;
        }

        public T GetComponent<T>() where T : JQComponent
        {
            if (_componentDic == null)
            {
                JQLog.LogError($"Disposed Entity:{_instanceId}");
                return null;
            }
            _componentDic.TryGetValue(typeof(T), out JQComponent component);
            return component as T;
        }

        protected void SortAllComponent()
        {
            _componentList.Sort(SortComponent);
            for (int i = 0; i < _componentList.Count; i++)
            {
                JQComponent component = _componentList[i];
                if (component is IComponentUpdate)
                {
                    if (_componentUpdateList == null)
                    {
                        _componentUpdateList = new List<IComponentUpdate>();
                    }

                    _componentUpdateList.Add(component as IComponentUpdate);
                }

                if (component is IComponentLateUpdate)
                {
                    if (_componentLateUpdateList == null)
                    {
                        _componentLateUpdateList = new List<IComponentLateUpdate>();
                    }

                    _componentLateUpdateList.Add(component as IComponentLateUpdate);
                }

                if (component is IComponentDisplayUpdate)
                {
                    if (_componentDisplayUpdateList == null)
                    {
                        _componentDisplayUpdateList = new List<IComponentDisplayUpdate>();
                    }

                    _componentDisplayUpdateList.Add(component as IComponentDisplayUpdate);
                }
            }
        }


        // --优先级越大越往后放
        private int SortComponent(JQComponent a, JQComponent b)
        {
            if (a.ExecutePriority != b.ExecutePriority)
            {
                return a.ExecutePriority - b.ExecutePriority;
            }

            if (a.NameByteSum != b.NameByteSum)
            {
                return a.NameByteSum - b.NameByteSum;
            }

            return 0;
        }

        public void SetGameTime(GameTime gameTime)
        {
            _gameTime = gameTime;
            for (int i = 0; i < _componentList.Count; i++)
            {
                JQComponent component = _componentList[i];
                component.SetGameTime(gameTime);
            }
        }

        public void OnAwake()
        {
        }

        public void OnStart()
        {
            for (int i = 0; i < _componentList.Count; i++)
            {
                JQComponent component = _componentList[i];
                component.onStart();
            }
        }

        public void OnPause()
        {
            for (int i = 0; i < _componentList.Count; i++)
            {
                JQComponent component = _componentList[i];
                component.onPause();
            }
        }

        public void OnUpdate()
        {
            if (_componentUpdateList == null) return;
            for (int i = 0; i < _componentUpdateList.Count; i++)
            {
                IComponentUpdate component = _componentUpdateList[i];
                component.OnUpdate();
                if (_componentUpdateList == null)
                {
                    break;
                }
            }
        }

        public void OnDisplayUpdate()
        {
            if (_componentDisplayUpdateList == null) return;
            for (int i = 0; i < _componentDisplayUpdateList.Count; i++)
            {
                IComponentDisplayUpdate component = _componentDisplayUpdateList[i];
                component.OnDisplayUpdate();
                if (_componentDisplayUpdateList == null)
                {
                    break;
                }
            }
        }

        public void OnLateUpdate()
        {
            if (_componentLateUpdateList == null) return;
            for (int i = 0; i < _componentLateUpdateList.Count; i++)
            {
                IComponentLateUpdate component = _componentLateUpdateList[i];
                component.onLateUpdate();
                if (_componentLateUpdateList == null)
                {
                    break;
                }
            }
        }


        public void OnReset()
        {
            for (int i = 0; i < _componentList.Count; i++)
            {
                JQComponent component = _componentList[i];
                component.onReset();
            }

            _gameTime = null;
            // entityMgr = null;
            // worldMgr = null;
            if (_eventDispatcher != null)
            {
                _eventDispatcher.TriggerEvent(Entity_RESET);
                _eventDispatcher.EventDispose();
            }
        }

        public void ClearEvent()
        {
            _eventDispatcher.EventDispose();
            _eventDispatcher = null;
        }

        public virtual void Dispose()
        {
            // JQLog.Log($"Dispose Entity:{_instanceId}");
            _eventDispatcher.TriggerEvent(Entity_DISPOSE);
            for (int i = 0; i < _componentList.Count; i++)
            {
                JQComponent component = _componentList[i];
                component.Dispose();
            }

            if (_componentList != null)
            {
                _componentList.Clear();
                _componentList = null;
            }

            if (_componentUpdateList != null)
            {
                _componentUpdateList.Clear();
                _componentUpdateList = null;
            }

            if (_componentLateUpdateList != null)
            {
                _componentLateUpdateList.Clear();
                _componentLateUpdateList = null;
            }

            if (_componentDisplayUpdateList != null)
            {
                _componentDisplayUpdateList.Clear();
                _componentDisplayUpdateList = null;
            }

            _componentDic.Clear();
            _componentDic = null;
            
            ClearEvent();

            // entityMgr = null;
            // worldMgr = null;
        }
    }
}