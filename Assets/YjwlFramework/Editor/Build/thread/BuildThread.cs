/*----------------------------------------------------------------
// 文件名：BuildThread.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/10/15 22:09:52
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;

namespace JQEditor.Build
{
    public class BuildThread<T>
    {
        private static readonly object _locker = new();
        private readonly Action _endAciton;
        private readonly Action<T> _executeAction;
        private int _finishCount;
        private readonly Queue<T> _queue = new();
        private readonly List<Thread> _threadList = new();
        private readonly string _titleName;
        private readonly int _total;
        private readonly int _threadCount;

        public BuildThread(string titleName, List<T> list, int threadCount, Action<T> executeAction, Action endAction)
        {
            _titleName = titleName;
            _total = list.Count;
            foreach (var t in list) _queue.Enqueue(t);
            _executeAction = executeAction;
            _endAciton = endAction;
            _threadCount = threadCount;
        }

        public void Start()
        {
            _finishCount = 0;
            for (var i = 0; i < _threadCount; i++)
            {
                //创建一个新的线程
                var thread = new Thread(SetProcessFiles);
                _threadList.Add(thread);
                //设置为后台线程
                thread.IsBackground = true;
                //开始线程
                thread.Start(i);
            }

            EditorApplication.update = SearchUpdate;
        }

        private void SetProcessFiles(object index)
        {
            while (_queue.Count > 0)
            {
                var info = default(T);
                lock (_locker)
                {
                    if (_queue.Count > 0)
                    {
                        info = _queue.Dequeue();
                        _finishCount = _total - _queue.Count;
                    }
                }

                if (info != null) _executeAction(info);
                //                    Debug.Log($"thread: {index}        {_queue.Count}      {_finishCount}/{_total}");
                Thread.Sleep(1);
            }
        }


        private void SearchUpdate()
        {
            lock (_locker)
            {
                var process = _finishCount;
//                Debug.Log("SearchUpdate:" + $"{process}/{_total}");
                var isCancle = EditorUtility.DisplayCancelableProgressBar(_titleName, $"{process}/{_total}", (float)process / _total);
                if (isCancle || process >= _total)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    Thread.Sleep(10);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    if (_endAciton != null) _endAciton();
                }
            }
        }
    }
}