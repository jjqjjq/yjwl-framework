using System;
using System.Collections.Generic;

namespace JQCore.tEvent
{
    public class JQEventDispatcher
    {
        private List<IIrfEventAction> _eventActionList;
        private List<IIrfEventAction> _eventActionTList;
        private List<IIrfEventAction> _eventActionTUList;
        private List<IIrfEventAction> _eventActionTUVList;
        private List<IIrfEventAction> _eventActionTUVWList;

        private List<IIrfEventAction> EventListDispose(List<IIrfEventAction> list)
        {
            if (list != null)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var irfEventAction = list[i];
                    irfEventAction.Dispose();
                }

                list.Clear();
            }

            return null;
        }

        public void EventDispose()
        {
            _eventActionList = EventListDispose(_eventActionList);
            _eventActionTList = EventListDispose(_eventActionTList);
            _eventActionTUList = EventListDispose(_eventActionTUList);
            _eventActionTUVList = EventListDispose(_eventActionTUVList);
            _eventActionTUVWList = EventListDispose(_eventActionTUVWList);
        }

        private IIrfEventAction getEventAction(string eventType, bool create = false)
        {
            if (_eventActionList == null)
            {
                if (!create) return null;

                _eventActionList = new List<IIrfEventAction>();
            }

            for (var i = 0; i < _eventActionList.Count; i++)
            {
                var tempAction = _eventActionList[i];
                if (tempAction.Key.Equals(eventType)) return tempAction as IrfEventAction;
            }

            if (!create) return null;

            var irfEventAction = new IrfEventAction(eventType);
            _eventActionList.Add(irfEventAction);
            return irfEventAction;
        }

        private IIrfEventAction getEventAction<T>(string eventType, bool create = false)
        {
            if (_eventActionTList == null)
            {
                if (!create) return null;

                _eventActionTList = new List<IIrfEventAction>();
            }

            for (var i = 0; i < _eventActionTList.Count; i++)
            {
                var tempAction = _eventActionTList[i];
                if (tempAction.Key.Equals(eventType)) return tempAction as IrfEventAction<T>;
            }

            if (!create) return null;

            var irfEventAction = new IrfEventAction<T>(eventType);
            _eventActionTList.Add(irfEventAction);
            return irfEventAction;
        }

        private IIrfEventAction getEventAction<T, U>(string eventType, bool create = false)
        {
            if (_eventActionTUList == null)
            {
                if (!create) return null;

                _eventActionTUList = new List<IIrfEventAction>();
            }

            for (var i = 0; i < _eventActionTUList.Count; i++)
            {
                var tempAction = _eventActionTUList[i];
                if (tempAction.Key.Equals(eventType)) return tempAction as IrfEventAction<T, U>;
            }

            if (!create) return null;

            var irfEventAction = new IrfEventAction<T, U>(eventType);
            _eventActionTUList.Add(irfEventAction);
            return irfEventAction;
        }


        private IIrfEventAction getEventAction<T, U, V>(string eventType, bool create = false)
        {
            if (_eventActionTUVList == null)
            {
                if (!create) return null;

                _eventActionTUVList = new List<IIrfEventAction>();
            }

            for (var i = 0; i < _eventActionTUVList.Count; i++)
            {
                var tempAction = _eventActionTUVList[i];
                if (tempAction.Key.Equals(eventType)) return tempAction as IrfEventAction<T, U, V>;
            }

            if (!create) return null;

            var irfEventAction = new IrfEventAction<T, U, V>(eventType);
            _eventActionTUVList.Add(irfEventAction);
            return irfEventAction;
        }

        private IIrfEventAction getEventAction<T, U, V, W>(string eventType, bool create = false)
        {
            if (_eventActionTUVWList == null)
            {
                if (!create) return null;

                _eventActionTUVWList = new List<IIrfEventAction>();
            }

            for (var i = 0; i < _eventActionTUVWList.Count; i++)
            {
                var tempAction = _eventActionTUVWList[i];
                if (tempAction.Key.Equals(eventType)) return tempAction as IrfEventAction<T, U, V, W>;
            }

            if (!create) return null;

            var irfEventAction = new IrfEventAction<T, U, V, W>(eventType);
            _eventActionTUVWList.Add(irfEventAction);
            return irfEventAction;
        }


        public void TriggerEvent(string eventType)
        {
            var irfEventAction = getEventAction(eventType) as IrfEventAction;
            if (irfEventAction != null) irfEventAction.TriggerEvent();
        }

        public void AddEventListener(string eventType, Action handler)
        {
            var irfEventAction = getEventAction(eventType, true) as IrfEventAction;
            irfEventAction.AddEventListener(handler);
        }

        public void RemoveEventListener(string eventType, Action handler)
        {
            var irfEventAction = getEventAction(eventType) as IrfEventAction;
            if (irfEventAction != null) irfEventAction.RemoveEventListener(handler);
        }

        public void TriggerEvent<T>(string eventType, T t, bool clear = false)
        {
            var irfEventAction = getEventAction<T>(eventType) as IrfEventAction<T>;
            if (irfEventAction != null)
            {
                irfEventAction.TriggerEvent(t);
                if (clear) irfEventAction.ClearAllEvent();
            }
        }

        public void AddEventListener<T>(string eventType, Action<T> handler)
        {
            var irfEventAction = getEventAction<T>(eventType, true) as IrfEventAction<T>;
            irfEventAction.AddEventListener(handler);
        }

        public void RemoveEventListener<T>(string eventType, Action<T> handler)
        {
            var irfEventAction = getEventAction<T>(eventType) as IrfEventAction<T>;
            if (irfEventAction != null) irfEventAction.RemoveEventListener(handler);
        }

        public void TriggerEvent<T, U>(string eventType, T t, U u)
        {
            var irfEventAction = getEventAction<T, U>(eventType) as IrfEventAction<T, U>;
            if (irfEventAction != null) irfEventAction.TriggerEvent(t, u);
        }

        public void AddEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            var irfEventAction = getEventAction<T, U>(eventType, true) as IrfEventAction<T, U>;
            irfEventAction.AddEventListener(handler);
        }

        public void RemoveEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            var irfEventAction = getEventAction<T, U>(eventType) as IrfEventAction<T, U>;
            if (irfEventAction != null) irfEventAction.RemoveEventListener(handler);
        }

        public void TriggerEvent<T, U, V>(string eventType, T t, U u, V v)
        {
            var irfEventAction = getEventAction<T, U, V>(eventType) as IrfEventAction<T, U, V>;
            if (irfEventAction != null) irfEventAction.TriggerEvent(t, u, v);
        }

        public void AddEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            var irfEventAction =
                getEventAction<T, U, V>(eventType, true) as IrfEventAction<T, U, V>;
            irfEventAction.AddEventListener(handler);
        }

        public void RemoveEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            var irfEventAction = getEventAction<T, U, V>(eventType) as IrfEventAction<T, U, V>;
            irfEventAction.RemoveEventListener(handler);
        }


        public void TriggerEvent<T, U, V, W>(string eventType, T t, U u, V v, W w)
        {
            var irfEventAction =
                getEventAction<T, U, V, W>(eventType) as IrfEventAction<T, U, V, W>;
            if (irfEventAction != null) irfEventAction.TriggerEvent(t, u, v, w);
        }

        public void AddEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            var irfEventAction =
                getEventAction<T, U, V, W>(eventType, true) as IrfEventAction<T, U, V, W>;
            irfEventAction.AddEventListener(handler);
        }

        public void RemoveEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            var irfEventAction =
                getEventAction<T, U, V, W>(eventType) as IrfEventAction<T, U, V, W>;
            irfEventAction.RemoveEventListener(handler);
        }

        private interface IIrfEventAction
        {
            string Key { get; }
            void Dispose();
        }

        private class IrfEventAction : IIrfEventAction
        {
            private List<Action> _actionList = new();
            private List<Action> _tempList = new();

            public IrfEventAction(string key)
            {
                Key = key;
            }

            public string Key { get; private set; }

            public virtual void Dispose()
            {
                _actionList.Clear();
                _actionList = null;
                _tempList.Clear();
                _tempList = null;
                Key = null;
            }

            public void AddEventListener(Action action)
            {
                if (!_actionList.Contains(action)) _actionList.Add(action);
            }

            public void RemoveEventListener(Action action)
            {
                _actionList.Remove(action);
            }

            public void TriggerEvent()
            {
                _tempList.Clear();
                _tempList.AddRange(_actionList);
                for (var i = 0; i < _tempList.Count; i++)
                {
                    var action = _tempList[i];
                    action();
                }
            }
        }

        private class IrfEventAction<T> : IIrfEventAction
        {
            private List<Action<T>> _actionList = new();
            private List<Action<T>> _tempList = new();

            public IrfEventAction(string key)
            {
                Key = key;
            }

            public string Key { get; }


            public void Dispose()
            {
                _actionList.Clear();
                _actionList = null;
                _tempList.Clear();
                _tempList = null;
            }

            public void ClearAllEvent()
            {
                _actionList.Clear();
                _tempList.Clear();
            }

            public void AddEventListener(Action<T> action)
            {
                if (!_actionList.Contains(action)) _actionList.Add(action);
            }

            public void RemoveEventListener(Action<T> action)
            {
                _actionList.Remove(action);
            }

            public void TriggerEvent(T t)
            {
                _tempList.Clear();
                _tempList.AddRange(_actionList);
                for (var i = 0; i < _tempList.Count; i++)
                {
                    var action = _tempList[i];
                    action(t);
                }
            }
        }

        private class IrfEventAction<T, U> : IIrfEventAction
        {
            private List<Action<T, U>> _actionList = new();
            private List<Action<T, U>> _tempList = new();

            public IrfEventAction(string key)
            {
                Key = key;
            }

            public string Key { get; }

            public void Dispose()
            {
                _actionList.Clear();
                _actionList = null;
                _tempList.Clear();
                _tempList = null;
            }

            public void AddEventListener(Action<T, U> action)
            {
                if (!_actionList.Contains(action)) _actionList.Add(action);
            }

            public void RemoveEventListener(Action<T, U> action)
            {
                _actionList.Remove(action);
            }

            public void TriggerEvent(T t, U u)
            {
                _tempList.Clear();
                _tempList.AddRange(_actionList);
                for (var i = 0; i < _tempList.Count; i++)
                {
                    var action = _tempList[i];
                    action(t, u);
                }
            }
        }


        public class IrfEventAction<T, U, V> : IIrfEventAction
        {
            private List<Action<T, U, V>> _actionList = new();
            private List<Action<T, U, V>> _tempList = new();

            public IrfEventAction(string key)
            {
                Key = key;
            }

            public string Key { get; }

            public void Dispose()
            {
                _actionList.Clear();
                _actionList = null;
                _tempList.Clear();
                _tempList = null;
            }

            public void AddEventListener(Action<T, U, V> action)
            {
                if (!_actionList.Contains(action)) _actionList.Add(action);
            }

            public void RemoveEventListener(Action<T, U, V> action)
            {
                _actionList.Remove(action);
            }

            public void TriggerEvent(T t, U u, V v)
            {
                _tempList.Clear();
                _tempList.AddRange(_actionList);
                for (var i = 0; i < _tempList.Count; i++)
                {
                    var action = _tempList[i];
                    action(t, u, v);
                }
            }
        }

        public class IrfEventAction<T, U, V, W> : IIrfEventAction
        {
            private List<Action<T, U, V, W>> _actionList = new();
            private List<Action<T, U, V, W>> _tempList = new();

            public IrfEventAction(string key)
            {
                Key = key;
            }

            public string Key { get; }

            public void Dispose()
            {
                _actionList.Clear();
                _actionList = null;
                _tempList.Clear();
                _tempList = null;
            }

            public void AddEventListener(Action<T, U, V, W> action)
            {
                if (!_actionList.Contains(action)) _actionList.Add(action);
            }

            public void RemoveEventListener(Action<T, U, V, W> action)
            {
                _actionList.Remove(action);
            }

            public void TriggerEvent(T t, U u, V v, W w)
            {
                _tempList.Clear();
                _tempList.AddRange(_actionList);
                for (var i = 0; i < _tempList.Count; i++)
                {
                    var action = _tempList[i];
                    action(t, u, v, w);
                }
            }
        }
    }
}