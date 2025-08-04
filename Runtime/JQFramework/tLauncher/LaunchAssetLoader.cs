
using JQCore.tEnum;
using JQCore.tLoader;
using JQCore.tPool.Loader;
using JQCore.tYooAssets;
using JQFramework.tMgr;
using YooAsset;

namespace JQFramework.tLauncher
{
    public class LaunchAssetLoader : BaseLoader
    {
        private AssetInfo[] _assetInfos;

        /// <summary>
        /// 一些常用的预制体，可以在启动时提前加载，并设置不释放
        /// </summary>
        private string[] _prefabUrls = new[]
        {
            EFrameworkAsset.CommonPrefab,
            // EFrameworkAsset.DefaultModel,
            EFrameworkAsset.LoadingView,
            EFrameworkAsset.StageView,
        };

        public LaunchAssetLoader() : base("LaunchAssetLoader")
        {
            _assetInfos = YooAssetMgr.GetAssetInfos("Launch");
            initTotal(_assetInfos.Length + _prefabUrls.Length);
        }

        public override void start()
        {
            base.start();
            for (int i = 0; i < _assetInfos.Length; i++)
            {
                AssetInfo assetInfo = _assetInfos[i];
                // HyDebug.LogError($"load asset {assetInfo.AssetPath}");
                string assetPath = YooAssetMgr.ShortAssetPath(assetInfo);
                AssetLoaderUtil.LoadAsset(assetPath, onAssetLoadCallback, true);
            }

            for (int i = 0; i < _prefabUrls.Length; i++)
            {
                string url = _prefabUrls[i];
                UrlPrefabLoaderUtil.LoadAsset(url, onPrefabLoadCallback, true);
            }
        }

        private void onAssetLoadCallback(AssetLoader assetLoader)
        {
            finishOne();
            checkFinish();
        }

        private void onPrefabLoadCallback(UrlPrefabLoader urlPrefabLoader)
        {
            switch (urlPrefabLoader.Key)
            {
                case EFrameworkAsset.CommonPrefab:
                    CommonResMgr.Instance.SetAssetObjectLib(urlPrefabLoader);
                    break;
                // case EFrameworkAsset.DefaultModel:
                //     CommonResMgr.Instance.SetDefaultModelPrefabLoader(urlPrefabLoader);
                //     break;
            }

            finishOne();
            checkFinish();
        }

        private void checkFinish()
        {
            if (_remain == 0)
            {
                // Game.AddSingleton<UIModelCtrlMgr>();
                // Game.AddSingleton<UIMeshCtrlMgr>();
                // Game.AddSingleton<ViewStackMgr>();
                // Game.AddSingleton<ViewSortingOrderMgr>();
                // Game.AddSingleton<CameraMgr>();
                finishAll();
            }
        }
    }
}