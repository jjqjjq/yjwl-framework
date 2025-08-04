using System;
using JQCore.tLog;
using JQCore.tSingleton;
using JQCore.tPool.Loader;

namespace JQCore.tLoader
{
    public class LoaderMgr : JQSingleton<LoaderMgr>
    {
        private LoadTask _currLoadTask; //当前存在加载任务

        public override void Dispose()
        {
        }

        public bool StartTaskCollect(string taskName, Action endFunction)
        {
            if (_currLoadTask != null)
            {
                JQLog.LogError("有其他任务正在执行：" + _currLoadTask.name);
                return false;
            }

            //HyDebug.LogError("任务Start：" + taskName);
            _currLoadTask = new LoadTask(taskName, endFunction, this);
            return true;
        }

        public void AddTask(BaseLoader baseLoader)
        {
            if (_currLoadTask == null)
            {
                JQLog.LogError("任务不存在,请使用StartTaskCollect添加一个新的任务");
                return;
            }

            _currLoadTask.AddLoader(baseLoader);
        }

        public void AddLowTask(BaseLoader baseLoader)
        {
            if (_currLoadTask == null)
            {
                JQLog.LogError("任务不存在,请使用StartTaskCollect添加一个新的任务");
                return;
            }

            _currLoadTask.AddLowLoader(baseLoader);
        }


        public void AddTask(UrlPrefabLoader prefabLoader)
        {
            if (_currLoadTask == null)
            {
                prefabLoader.Load();
                return;
            }

            _currLoadTask.AddLoader(prefabLoader);
        }


        public void RemoveTask(UrlPrefabLoader prefabLoader)
        {
            if (_currLoadTask == null) return;
            _currLoadTask.RemoveLoader(prefabLoader);
        }

        public bool EndTaskCollectAndLoad(string taskName, int count = 0)
        {
            if (_currLoadTask == null)
            {
                JQLog.LogError("任务不存在:" + taskName);
                return false;
            }

            if (_currLoadTask.name != taskName)
            {
                JQLog.LogError("任务交叉处理了？！" + taskName);
                return false;
            }

            //ready 准备加载信息
            _currLoadTask.Ready();

            if (_currLoadTask.Total == 0 && count == 0)
            {
                Finish(_currLoadTask.name);
                return false;
            }

            //开始加载
            _currLoadTask.Load();
            return true;
        }

        public void Finish(string taskName)
        {
            if (_currLoadTask == null)
            {
                JQLog.LogError("任务不存在:" + taskName);
                return;
            }

            if (_currLoadTask.name != taskName)
            {
                JQLog.LogError("任务交叉处理了？！" + taskName);
                return;
            }

            //HyDebug.LogError("任务Finish:" + _currLoadTask.name);
            var tempTask = _currLoadTask;
            _currLoadTask = null;
            tempTask.Finish();
        }

        public void Clear()
        {
            _currLoadTask = null;
        }

        public void Stop(string taskName)
        {
            if (_currLoadTask == null) return;

            if (_currLoadTask.name != taskName) return;

            _currLoadTask.Dispose();
            _currLoadTask = null;
        }
    }
}