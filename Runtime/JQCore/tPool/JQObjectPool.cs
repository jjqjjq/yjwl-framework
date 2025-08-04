using System.Collections.Generic;
using UnityEngine.Events;

namespace JQCore.tPool
{
  
    // Object pool to avoid allocations.
    public class JQObjectPool<T> where T : new()
    {
        private readonly Stack<T> sk = new Stack<T>();
        private readonly UnityAction<T> getAction;
        private readonly UnityAction<T> rAction;
        private static JQObjectPool<T> _instance;

        public static JQObjectPool<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new JQObjectPool<T>();
                }
                return _instance;
            }
        }

        public int countAll { get; private set; }

        public int countActive
        {
            get { return countAll - countInactive; }
        }

        public int countInactive
        {
            get { return sk.Count; }
        }
        
        public JQObjectPool()
        {
        }

        public JQObjectPool(UnityAction<T> _getAction, UnityAction<T> _rAction)
        {
            getAction = _getAction;
            rAction = _rAction;
        }

        public T Get()
        {
            T e;
            if (sk.Count == 0)
            {
                e = new T();
                countAll++;
            }
            else
            {
                e = sk.Pop();
            }

            if (getAction != null)
                getAction(e);
            return e;
        }

        public void Release(T element)
        {
            if (sk.Count > 0 && ReferenceEquals(sk.Peek(), element))
            {
                return;
            }

            if (rAction != null)
                rAction(element);
            sk.Push(element);
        }
    }
}