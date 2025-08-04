using System.Collections;
using System.Collections.Generic;
using JQCore.tCoroutine;
using JQCore.tLog;
using JQCore;
using JQCore.tPool.Loader;
#if PERFORMANCE_LOG
using System.Diagnostics;
#endif

namespace JQCore.tLoader
{
    public class BatchPrefabLoader : BaseLoader
    {
        private List<UrlPrefabLoader> _prefabLoaderList = new();
        private List<UrlPrefabLoader> _finishLoaderList = new();

        private UrlPrefabLoader _currPrefabLoader;
#if PERFORMANCE_LOG
        Stopwatch _stopwatch = new Stopwatch();
#endif


        public BatchPrefabLoader() : base("BatchPrefabLoader")
        {
        }

        public override void start()
        {
            base.start();
            LoadNext();
        }

        public override void ready()
        {
            base.ready();
            initTotal(_prefabLoaderList.Count);
        }

        private void LoadNext()
        {
            if (_prefabLoaderList.Count > 0)
            {
                _currPrefabLoader = _prefabLoaderList[0];
#if PERFORMANCE_LOG
                JQLog.Log("[执行Prefab]" + _currPrefabLoader.Url);
                _stopwatch.Reset();
            _stopwatch.Start();
#endif
                _currPrefabLoader.AddEventListener<UrlPrefabLoader>(UrlPrefabLoader.COMPLETE, onPrefabComplete);
                _currPrefabLoader.Load();
            }
            else
            {
                //释放WebStream,u3d内在bug，会导致图片丢失。需要等待1-2帧
                JQCoroutineHandler.Start(waitForOneBug());
            }
        }

        protected virtual IEnumerator waitForOneBug()
        {
            yield return 2;
            ReleaseAll();
            finishAll();
        }

        private void ReleaseAll()
        {
            for (var i = 0; i < _finishLoaderList.Count; i++)
            {
                var prefabLoader = _finishLoaderList[i];
                prefabLoader.ReleaseAllAssetsBundle();
            }

            _finishLoaderList.Clear();
        }


        private void onPrefabComplete(UrlPrefabLoader prefabLoader)
        {
            _currPrefabLoader.RemoveEventListener<UrlPrefabLoader>(UrlPrefabLoader.COMPLETE, onPrefabComplete);
            _prefabLoaderList.Remove(prefabLoader);
            _finishLoaderList.Add(prefabLoader);
            finishOne();
#if PERFORMANCE_LOG
             _stopwatch.Stop();
             JQLog.LogErrorFormat("---------------------------[执行完毕Prefab]:{0}ms", _stopwatch.ElapsedMilliseconds);
#endif
            LoadNext();
        }

        public void AddLoader(UrlPrefabLoader prefabLoader)
        {
            if (_prefabLoaderList.Contains(prefabLoader)) return;

            if (Sys.LOG_RES) JQLog.Log("[++++++]Prefab:" + prefabLoader.Key);
            _prefabLoaderList.Add(prefabLoader);
        }

        public void RemoveLoader(UrlPrefabLoader prefabLoader)
        {
            if (_prefabLoaderList.Remove(prefabLoader))
                if (Sys.LOG_RES)
                    JQLog.Log("[-------]Prefab:" + prefabLoader.Key);
        }

        /*
        public override void finalEnd()
        {
            base.finalEnd();
            ReleaseAll();
        }*/

        public override void dispose()
        {
            base.dispose();
            if (_finishLoaderList != null)
            {
                _finishLoaderList.Clear();
                _finishLoaderList = null;
            }

            if (_prefabLoaderList != null)
            {
                _prefabLoaderList.Clear();
                _prefabLoaderList = null;
            }

            _currPrefabLoader = null;
        }
    }
}