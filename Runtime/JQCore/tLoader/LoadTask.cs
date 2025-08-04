using System;
using System.Collections;
using System.Collections.Generic;
using JQCore.tCoroutine;
using JQCore.tLog;
using JQCore;
using JQCore.tPool.Loader;
using UnityEngine;

namespace JQCore.tLoader
{
    public class LoadTask
    {
        private const int LOAD_COUNT_PRE_FRAME = 1;
        private List<BaseLoader> _baseLoaderList = new();
        private BatchPrefabLoader _batchPrefabLoader = new();

        private int _bigTaskRunCount;

        private BaseLoader _currBaseLoader;
        private Action _endFunction;

        private readonly List<BaseLoader> _finishLoaderList = new();

        private LoaderMgr _loaderMgr;
        private List<BaseLoader> _lowLoaderList = new();

        private int _preAiCount;
        private int _preStreamLoadCount;
        private int _startAiCount;
        private int _startStreamLoadCount;

        public string name;

        public LoadTask(string taskName, Action endFunction, LoaderMgr loaderMgr)
        {
            name = taskName;
            _endFunction = endFunction;
            _loaderMgr = loaderMgr;
        }

        public int Total { get; private set; }

        private void initTotal()
        {
            var total = 0;
            for (var i = 0; i < _baseLoaderList.Count; i++)
            {
                var baseLoader = _baseLoaderList[i];
                total += baseLoader.Total;
            }

            total += _batchPrefabLoader.Total;

            for (var i = 0; i < _lowLoaderList.Count; i++)
            {
                var baseLoader = _lowLoaderList[i];
                total += baseLoader.Total;
            }

            Total = total;
        }

        public void Dispose()
        {
            if (_baseLoaderList != null)
            {
                for (var i = 0; i < _baseLoaderList.Count; i++)
                {
                    var baseLoader = _baseLoaderList[i];
                    baseLoader.dispose();
                }

                _baseLoaderList.Clear();
                _baseLoaderList = null;
            }

            if (_batchPrefabLoader != null)
            {
                _batchPrefabLoader.dispose();
                _batchPrefabLoader = null;
            }

            if (_lowLoaderList != null)
            {
                for (var i = 0; i < _lowLoaderList.Count; i++)
                {
                    var baseLoader = _lowLoaderList[i];
                    baseLoader.dispose();
                }

                _lowLoaderList.Clear();
                _lowLoaderList = null;
            }

            _currBaseLoader = null;
            _endFunction = null;
            _loaderMgr = null;
        }

        public void AddLoader(BaseLoader baseLoader)
        {
            if (Sys.LOG_RES) JQLog.Log("[++++++]Loader:" + baseLoader.Name);
            _baseLoaderList.Add(baseLoader);
        }

        public void AddLowLoader(BaseLoader baseLoader)
        {
            if (Sys.LOG_RES) JQLog.Log("[++++++]LowLoader:" + baseLoader.Name);
            _lowLoaderList.Add(baseLoader);
        }

        public void AddLoader(UrlPrefabLoader prefabLoader)
        {
            _batchPrefabLoader.AddLoader(prefabLoader);
        }

        public void RemoveLoader(UrlPrefabLoader prefabLoader)
        {
            _batchPrefabLoader.RemoveLoader(prefabLoader);
        }

        //1.开始加载
        public void Load()
        {
            LoadNext();
        }

        public void FinalEnd()
        {
            for (var i = 0; i < _finishLoaderList.Count; i++)
            {
                var baseLoader = _finishLoaderList[i];
                baseLoader.finalEnd();
            }

            _finishLoaderList.Clear();

            _batchPrefabLoader.finalEnd();
        }


        public void Ready()
        {
            for (var i = 0; i < _baseLoaderList.Count; i++)
            {
                var baseLoader = _baseLoaderList[i];
                baseLoader.ready();
                baseLoader.readyEnd();
            }

            for (var i = 0; i < _lowLoaderList.Count; i++)
            {
                var baseLoader = _lowLoaderList[i];
                baseLoader.ready();
                baseLoader.readyEnd();
            }

            _batchPrefabLoader.ready();
            _batchPrefabLoader.readyEnd();

            initTotal();
        }

        //2.加载非资源项
        private void LoadNext()
        {
            _currBaseLoader = null;
            if (_baseLoaderList.Count > 0)
                _currBaseLoader = _baseLoaderList[0];
            else if (_batchPrefabLoader.Remain != 0)
                _currBaseLoader = _batchPrefabLoader;
            else if (_lowLoaderList.Count > 0) _currBaseLoader = _lowLoaderList[0];

            if (_currBaseLoader != null)
            {
                _currBaseLoader.onComplete += onLoadComplete;
                _currBaseLoader.onProcessUpdate += onProcessUpdate;
                _currBaseLoader.start();
            }
            else
            {
                _loaderMgr.Finish(name);
            }
        }

        public void Finish()
        {
            FinalEnd();
            if (Total > 0) Sys.setLoadingProgress(Total, 0);
            JQCoroutineHandler.Start(closeLoadingUI());
        }

        private IEnumerator closeLoadingUI()
        {
            yield return 0;
            if (_endFunction != null)
            {
                _endFunction();
                _endFunction = null;
            }

            Dispose();
            // JQLog.LogError("GC");
            // GC.Collect();
        }

        private void check()
        {
            if (Sys.isEditor)
            {
                var bigTaskRun = false;
                if (bigTaskRun) _bigTaskRunCount++;
                if (_bigTaskRunCount >= 2)
                {
                    _bigTaskRunCount = 0;
                    JQCoroutineHandler.Start(loadNextFrame());
                }
                else
                {
                    LoadNext();
                }
            }
            else
            {
                LoadNext();
            }
        }

        private IEnumerator loadNextFrame()
        {
            yield return new WaitForEndOfFrame();
            LoadNext();
        }

        private void onLoadComplete(BaseLoader baseLoader)
        {
            baseLoader.onComplete -= onLoadComplete;
            baseLoader.onProcessUpdate -= onProcessUpdate;
            if (_baseLoaderList.Contains(baseLoader))
            {
                _finishLoaderList.Add(baseLoader);
                _baseLoaderList.Remove(baseLoader);
            }
            else if (_batchPrefabLoader == baseLoader)
            {
            }
            else if (_lowLoaderList.Contains(baseLoader))
            {
                _finishLoaderList.Add(baseLoader);
                _lowLoaderList.Remove(baseLoader);
            }

            check();
        }

        private void onProcessUpdate()
        {
            var remain = 0;
            for (var i = 0; i < _baseLoaderList.Count; i++)
            {
                var baseLoader = _baseLoaderList[i];
                remain += baseLoader.Remain;
            }

            remain += _batchPrefabLoader.Remain;
            //remain += _batchAiLoader.Remain;

            for (var i = 0; i < _lowLoaderList.Count; i++)
            {
                var baseLoader = _lowLoaderList[i];
                remain += baseLoader.Remain;
            }

            Sys.setLoadingProgress(Total, remain);
        }
    }
}