using System;
using System.Collections.Generic;
using JQCore.tString;
using JQEditor.Build;
using JQCore;
using UnityEditor;
using UnityEngine;
using YooAsset;


namespace JQEditor.MainSubPackage
{
    public class OssUploadTask
    {
        List<OssUploadBean> _filePathList;
        AliyunOSSMgr _aliyunOssMgr = null;
        private long _totalSize;
        private long _currentSize;
        private Action _endAction;

        public OssUploadTask(List<OssUploadBean> filePathList)
        {
            _filePathList = filePathList;
            _aliyunOssMgr = new AliyunOSSMgr(
                BuildAppInfo.aliyunOSS_endpoint,
                BuildAppInfo.aliyunOSS_accessKeyId,
                BuildAppInfo.aliyunOSS_accessKeySecret,
                BuildAppInfo.aliyunOSS_bucketName);
        }

        public void Run(Action endAction)
        {
            _endAction = endAction;
            for (int i = 0; i < _filePathList.Count; i++)
            {
                OssUploadBean ossUploadBean = _filePathList[i];
                _totalSize += ossUploadBean.FileSize;
            }

            _currentSize = 0;

            BuildThread<OssUploadBean> buildThread = new BuildThread<OssUploadBean>("上传OSS中...", _filePathList, 12, doUploadFileFun, startTaskFinishAction);
            buildThread.Start();
        }

        private void doUploadFileFun(OssUploadBean ossUploadBean)
        {
            ossUploadBean.Upload(_aliyunOssMgr, onProgressCallback);
        }

        private void onProgressCallback(long incrementTransferred)
        {
            lock (this)
            {
                _currentSize += incrementTransferred;
                // Debug.Log($"上传进度:{_currentSize}/{_totalSize}");
            }

        }

        private void startTaskFinishAction()
        {
            Debug.Log("上传任务提交完成");
            EditorApplication.update = SearchUpdate;
        }


        private void SearchUpdate()
        {
            bool isCancle = EditorUtility.DisplayCancelableProgressBar("上传中...", $"{StringUtil.GetByteString(_currentSize)}/{StringUtil.GetByteString(_totalSize)}", (float)_currentSize / _totalSize);
            if (isCancle || _currentSize >= _totalSize)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                MainSubPackageUtil.pushCDNUpdateFileList(_filePathList);
                _endAction?.Invoke();
            }
        }
    }
}