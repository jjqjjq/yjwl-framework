using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JQCore;
using JQCore.tCoroutine;
using JQCore.tLog;
using JQCore.tSingleton;
using JQCore.tUtil;
using JQCore.tCfg;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace JQCore.tYooAssets
{
    public class YooAssetMgr : JQSingleton<YooAssetMgr>
    {
        //记录引用次数
        private static readonly Dictionary<string, YooAssetAssetInfo> _assetDic = new();

        private Action _initFinishAction;

        public static ResourcePackage Package { get; private set; }

        public static YooAssetAssetInfo GetAsset(string shortAssetPath)
        {
            if (!_assetDic.ContainsKey(shortAssetPath)) _assetDic[shortAssetPath] = new YooAssetAssetInfo(shortAssetPath);

            return _assetDic[shortAssetPath];
        }

        public override int GetDisposePriority()
        {
            return 100;
        }


        public static void UnloadAssets()
        {
            Package.UnloadUnusedAssets();
        }

        public static void SetDontRelease(string relativePath)
        {
            var resource = GetAsset(relativePath);
            resource.setDontRelease();
        }

        public static void ClearAllUnUse()
        {
            JQLog.LogWarning("IrfResourcesMgr ClearAllUnUse");
            foreach (var keyValuePair in _assetDic)
            {
                var assetInfo = keyValuePair.Value;
                if (assetInfo.canClear()) assetInfo.releaseAsset();
            }
        }

        public void Initialize(string packageName, EPlayMode playMode, Action initFinishAction)
        {
#if !UNITY_EDITOR
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                playMode = EPlayMode.OfflinePlayMode;
            }
            
#if UNITY_WEBGL
            playMode = EPlayMode.WebPlayMode;
#endif
#endif
            JQLog.Log($"YooAsset初始化：{packageName} {playMode}");
#if SDK_WEIXIN
            YooAssets.SetCacheSystemDisableCacheOnWebGL();
#endif
            // BetterStreamingAssets.Initialize();
            YooAssets.Initialize(new YooAssetLog());
            YooAssets.SetOperationSystemMaxTimeSlice(10); //每帧提供给资源管理器的加载耗时
            YooAssets.SetDownloadSystemBreakpointResumeFileSize(1024 * 1024 * 10); //断点续传文件大小

            _initFinishAction = initFinishAction;

            JQCoroutineHandler.Start(InitPackage(packageName, playMode));
        }

        private static void showAllAsset()
        {
            var assetInfos = Package.GetAssetInfos(string.Empty);
            foreach (var assetInfo in assetInfos) JQLog.Log("xxx:" + assetInfo.AssetPath);
        }

        public static string ShortAssetPath(AssetInfo assetInfo)
        {
            return assetInfo.AssetPath.Replace($"Assets/{PathUtil.RES_FOLDER}/", "");
        }

        public static string LongAssetPath(string shortAssetPath)
        {
            return $"Assets/{PathUtil.RES_FOLDER}/" + shortAssetPath;
        }

        public static AssetInfo[] GetAssetInfos(string tag)
        {
            return Package.GetAssetInfos(tag);
        }

        public static void LoadRawFileAsync(string shortAssetPath, Action<RawFileOperationHandle> loaded)
        {
            JQLog.LogWarning("LoadResource:" + shortAssetPath);
            addRecord(shortAssetPath);
            var fullRelativePath = LongAssetPath(shortAssetPath);
            var handle = Package.LoadRawFileAsync(fullRelativePath);
            handle.Completed += loaded;
        }

        /// <summary>
        ///     UrlPrefabLoader调用
        /// </summary>
        /// <param name="shortAssetPath"></param>
        /// <param name="loaded"></param>
        /// <typeparam name="T"></typeparam>
        public static void LoadAssetAsync<T>(string shortAssetPath, Action<AssetOperationHandle> loaded) where T : Object
        {
            JQLog.LogWarning("LoadResource:" + shortAssetPath);
            addRecord(shortAssetPath);
            var fullRelativePath = LongAssetPath(shortAssetPath);
            var handle = Package.LoadAssetAsync<T>(fullRelativePath);
            handle.Completed += loaded;
        }

        /// <summary>
        ///     AssetLoader调用
        /// </summary>
        /// <param name="shortAssetPath"></param>
        /// <param name="loaded"></param>
        public static void LoadAssetAsync(string shortAssetPath, Action<AssetOperationHandle> loaded)
        {
            JQLog.LogWarning("LoadResource:" + shortAssetPath);
            addRecord(shortAssetPath);
            var fullRelativePath = LongAssetPath(shortAssetPath);
            var assetInfo = Package.GetAssetInfo(fullRelativePath);
            var handle = Package.LoadAssetAsync(assetInfo);
            handle.Completed += loaded;
        }


        /// <summary>
        ///     加载场景
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="loadSceneMode"></param>
        /// <param name="loaded"></param>
        /// <param name="activateOnLoad"></param>
        public static void LoadSceneAsync(string shortAssetPath, LoadSceneMode loadSceneMode, Action<SceneOperationHandle> loaded, bool suspendLoad = false)
        {
            JQLog.LogWarning("LoadResource:" + shortAssetPath);
            addRecord(shortAssetPath);
            var fullRelativePath = LongAssetPath(shortAssetPath);
            var assetInfo = Package.GetAssetInfo(fullRelativePath);
            var handle = Package.LoadSceneAsync(assetInfo, loadSceneMode, suspendLoad);
            handle.Completed += loaded;
        }

        public override void Dispose()
        {
            if (Package != null)
            {
                foreach (var keyValuePair in _assetDic)
                {
                    var assetInfo = keyValuePair.Value;
                    assetInfo.releaseAsset();
                }

                _assetDic.Clear();
                //该版本不支持
                #if !UNITY_WEBGL
                Package.ForceUnloadAllAssets();
                #endif
            }

            Package = null;
            _loadList.Clear();
            YooAssets.Destroy();
        }

        private IEnumerator InitPackage(string packageName, EPlayMode playMode)
        {
            // yield return new WaitForSeconds(0f);
            // 创建默认的资源包
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(package);
            }

            Package = package;

            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            switch (playMode)
            {
                case EPlayMode.EditorSimulateMode:
                    var editorSimulateModeParameters = new EditorSimulateModeParameters();
                    editorSimulateModeParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
                    initializationOperation = package.InitializeAsync(editorSimulateModeParameters);
                    break;
                case EPlayMode.OfflinePlayMode:// 单机运行模式
                    var offlinePlayModeParameters = new OfflinePlayModeParameters();
                    offlinePlayModeParameters.DecryptionServices = new GameDecryptionServices();
                    initializationOperation = package.InitializeAsync(offlinePlayModeParameters);
                    break;
                case EPlayMode.HostPlayMode:// 联机运行模式
                    var hostPlayModeParameters = new HostPlayModeParameters();
                    hostPlayModeParameters.BuildinQueryServices = new GameQueryServices();
                    hostPlayModeParameters.DecryptionServices = new GameDecryptionServices();
                    hostPlayModeParameters.RemoteServices = new RemoteServices();
                    initializationOperation = package.InitializeAsync(hostPlayModeParameters);
                    JQLog.Log($"HostPlayModeParameters: {hostPlayModeParameters.RemoteServices.GetRemoteMainURL("??")}");
                    break;
                case EPlayMode.WebPlayMode:// Web运行模式
                    var webPlayModeParameters = new WebPlayModeParameters();
                    webPlayModeParameters.BuildinQueryServices = new GameQueryServices(); //太空战机DEMO的脚本类，详细见StreamingAssetsHelper
                    webPlayModeParameters.RemoteServices = new RemoteServices();
                    initializationOperation = package.InitializeAsync(webPlayModeParameters);
                    break;
            }

            yield return initializationOperation;
            if (initializationOperation.Status == EOperationStatus.Succeed)
            {
                JQLog.Log("YooAsset初始化完成");
                showAllAsset();
                _initFinishAction?.Invoke();
            }
            else
            {
                JQLog.LogError($"YooAsset初始化失败：{initializationOperation.Error}");
            }
        }


        // 内置文件查询服务类
        private class GameQueryServices : IBuildinQueryServices
        {
            public bool QueryStreamingAssets(string packageName, string fileName)
            {
                return Sys.sdkMgr.IsStreamingAssetsExist();
            }
        }

        private class RemoteServices : IRemoteServices
        {
            string IRemoteServices.GetRemoteMainURL(string fileName)
            {
                var gameVersion = "v" + AppConfig.AppVersion;
                return $"{AppConfig.CDN}/CDN/{PathUtil.platformName}/{gameVersion}/{fileName}";
            }

            string IRemoteServices.GetRemoteFallbackURL(string fileName)
            {
                var gameVersion = "v" + AppConfig.AppVersion;
                return $"{AppConfig.CDN}/CDN/{PathUtil.platformName}/{gameVersion}/{fileName}";
            }
        }


        /// <summary>
        ///     资源文件解密服务类
        /// </summary>
        private class GameDecryptionServices : IDecryptionServices
        {
            public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
            {
#if UNITY_WEBGL
                return 0;
#else
                return PathUtil.EncryptHeadLen;
#endif
            }

            public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
            {
                throw new NotImplementedException();
            }

            public Stream LoadFromStream(DecryptFileInfo fileInfo)
            {
                throw new NotImplementedException();
            }

            public uint GetManagedReadBufferSize()
            {
                return 1024;
            }
        }


        #region recorder

        private static readonly List<string> _loadList = new();

        public static void addRecord(string url)
        {
            url = $"Assets/{PathUtil.RES_FOLDER}/{url}";
            if (!_loadList.Contains(url)) _loadList.Add(url);
        }

#if UNITY_EDITOR

        public static byte[] GetFileBytes()
        {
            var stringBuilder = new StringBuilder();
            foreach (var url in _loadList) stringBuilder.Append(url).Append("\n");

            var allStr = stringBuilder.ToString();
            var byteArray = Encoding.UTF8.GetBytes(allStr);
            return byteArray;
        }

#endif

        #endregion


        #region 资源更新

        public string PackageVersion => Package.GetPackageVersion();

        /// <summary>
        ///     获取资源版本
        /// </summary>
        /// <param name="onFinish"></param>
        public void UpdatePackageVersion(Action onFinish)
        {
            JQCoroutineHandler.Start(DoUpdatePackageVersion(onFinish));
        }

        private string _newPackageVersion;

        private IEnumerator DoUpdatePackageVersion(Action onFinish)
        {
            //UOS_CDN  需要关闭在URL末尾添加时间戳
            var operation = Package.UpdatePackageVersionAsync(false);

            yield return operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                _newPackageVersion = operation.PackageVersion;
                Debug.Log($"Updated package Version : {_newPackageVersion}");
                onFinish?.Invoke();
            }
            else
            {
                //更新失败
                Debug.LogError(operation.Error);
            }
        }

        /// <summary>
        ///     获取资源清单
        /// </summary>
        /// <param name="onFinish"></param>
        public void UpdatePackageManifest(Action onFinish)
        {
            JQCoroutineHandler.Start(DoUpdatePackageManifest(onFinish));
        }

        private IEnumerator DoUpdatePackageManifest(Action onFinish)
        {
            // 更新成功后自动保存版本号，作为下次初始化的版本。
            // 也可以通过operation.SavePackageVersion()方法保存。
            // _package.ForceUnloadAllAssets();
            var savePackageVersion = true;
            var operation = Package.UpdatePackageManifestAsync(_newPackageVersion, savePackageVersion);
            yield return operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                onFinish?.Invoke();
                Sys.gameDispatcher.TriggerEvent("version");
            }
            else
            {
                //更新失败
                JQLog.LogError(operation.Error);
            }
        }

        #endregion
    }
}