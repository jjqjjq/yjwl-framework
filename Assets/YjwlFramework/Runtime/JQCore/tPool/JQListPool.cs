using System.Collections.Generic;
using UnityEngine.Events;

namespace JQCore.tPool
{
    public static class JQListPool<T>
    {
        private static readonly JQObjectPool<List<T>> s_ListPool = new JQObjectPool<List<T>>(null, ReleaseAction);

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }

        private static void ReleaseAction(List<T> toRelease)
        {
            toRelease.Clear();
        }
    }

}