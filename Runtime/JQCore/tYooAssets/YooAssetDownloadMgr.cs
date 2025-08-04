using System;
using System.Collections.Generic;
using JQCore.tSingleton;

namespace JQCore.tYooAssets
{
    public class YooAssetDownloadMgr : JQSingleton<YooAssetDownloadMgr>
    {
        //单个主资源下载
        private readonly List<YooAssetDownloadTask> _taskList = new();

        public override void Dispose()
        {
            for (var i = 0; i < _taskList.Count; i++) _taskList[i].Dispose();

            _taskList.Clear();
        }

        public YooAssetDownloadTask GetTask(string key, bool isTag, bool autoCreateNewTask = true)
        {
            for (var i = 0; i < _taskList.Count; i++)
            {
                var task = _taskList[i];
                if (task.isSame(key, isTag)) return task;
            }

            if (!autoCreateNewTask) return null;

            var newTask = new YooAssetDownloadTask(key, isTag);
            _taskList.Add(newTask);
            return newTask;
        }

        public void RemoveTask(YooAssetDownloadTask task)
        {
            if (_taskList.Contains(task))
            {
                task.Dispose();
                _taskList.Remove(task);
            }
        }

        public static bool IsNeedDownload(string shortAssetPath)
        {
            var fullRelativePath = YooAssetMgr.LongAssetPath(shortAssetPath);
            return YooAssetMgr.Package.IsNeedDownloadFromRemote(fullRelativePath);
        }

        public float GetProgress(string shortAssetPath)
        {
            var task = GetTask(shortAssetPath, false, false);
            if (task != null) return task.GetProgress();

            if (IsNeedDownload(shortAssetPath)) return 0f;

            return 1f;
        }

        public YooAssetDownloadTask DownloadByPath(string shortAssetPath, Action<bool> onDownloadOver, Action<int, int, long, long> onProgress = null, bool rightNow = true)
        {
            var task = GetTask(shortAssetPath, false);
            task.AddEvent(onDownloadOver, onProgress);
            if (rightNow) task.StartDownload();

            return task;
        }

        public void CancelDownloadByPath(string shortAssetPath)
        {
            var task = GetTask(shortAssetPath, false, false);
            if (task != null)
            {
                task.Dispose();
                _taskList.Remove(task);
            }
        }

        public void CancelDownloadByTag(string tag)
        {
            var task = GetTask(tag, true, false);
            if (task != null)
            {
                task.Dispose();
                _taskList.Remove(task);
            }
        }

        public YooAssetDownloadTask DownloadByTag(string tag, Action<bool> onDownloadOver, Action<int, int, long, long> onProgress = null, bool rightNow = true)
        {
            var task = GetTask(tag, true);
            task.AddEvent(onDownloadOver, onProgress);
            if (rightNow) task.StartDownload();

            return task;
        }
    }
}