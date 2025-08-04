using JQCore.tLoader;
using JQCore.tPool.Loader;
using JQCore.tRes;
using JQCore.tYooAssets;
using JQFramework.tMgr;
using UnityEngine.U2D;
using YooAsset;

namespace JQFramework.tLauncher
{
    public class IconAtlasLibLoader:BaseLoader
    {
        private AssetInfo[] _assetInfos;
        
        public IconAtlasLibLoader() : base("IconAtlasLibLoader")
        {
            _assetInfos = YooAssetMgr.GetAssetInfos("Atlas");
            initTotal(_assetInfos.Length);
        }

        public override void start()
        {
            base.start();
            for (int i = 0; i < _assetInfos.Length; i++)
            {
                AssetInfo assetInfo = _assetInfos[i];
                string iconType = YooAssetMgr.ShortAssetPath(assetInfo);
                AssetLoaderUtil.LoadAsset(iconType, onLoadAssetCallback, true);
                // UrlPrefabLoaderUtil.LoadAsset(iconType, onLoadCallback, true);
            }
            
        }

        private void onLoadAssetCallback(AssetLoader assetLoader)
        {
            SpriteAtlas spriteAtlas = assetLoader.getSpriteAtlas();
            SpriteAtlasMgr.Instance.addIconAtlas(spriteAtlas.name, spriteAtlas);
            finishOne();
            checkRemain();
        }

       

    }
}