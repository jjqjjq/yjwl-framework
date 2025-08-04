using System;
using System.Collections.Generic;
using JQCore.tEnum;
using JQCore.tLog;
using JQCore.tMgr;
using JQCore.tRes;
using JQCore.tSingleton;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace JQFramework.tMgr
{
    public class SpriteAtlasMgr : JQSingleton<SpriteAtlasMgr>
    {
        private Dictionary<string, SpriteAtlas> _iconLib = new Dictionary<string, SpriteAtlas>();
        private Sprite _unknown;
        private Sprite _unknownIcon;

        public SpriteAtlasMgr()
        {
        }

        public void init()
        {
            SpriteAtlasManager.atlasRequested += SpriteAtlasManagerOnatlasRequested;
        }


        public void addIconAtlas(string iconType, SpriteAtlas assetObjectLib)
        {
            _iconLib[iconType] = assetObjectLib;
            if (iconType == EFrameworkAsset.IconType_CommonUI)
            {
                _unknown = assetObjectLib.GetSprite("unknown");
                _unknownIcon = assetObjectLib.GetSprite("unknownIcon");
            }
        }
        
        private void SpriteAtlasManagerOnatlasRequested(string arg1, Action<SpriteAtlas> arg2)
        {
            JQLog.LogWarning($"SpriteAtlas Requested:{arg1}");
            arg2(_iconLib[arg1]);
        }

        public override void Dispose()
        {
            _iconLib.Clear();
        }

        public Sprite getIcon(string iconType, string iconName)
        {
            SpriteAtlas iconTypeLib = null;
            if (!_iconLib.TryGetValue(iconType, out iconTypeLib))
            {
                JQLog.LogError("图集类型不存在：" + iconType);
                return null;
            }

            if (string.IsNullOrEmpty(iconName))
            {
                JQLog.LogError("图标名称为null");
                return null;
            }

            return iconTypeLib.GetSprite(iconName);
        }

        private bool checkAndLog(Sprite asset, string iconType, string iconName)
        {
            if (asset == null)
            {
                JQLog.LogError("获取图标失败：" + iconName + " type:" + iconType);
                return false;
            }

            return true;
        }

        public void setUnknow(Image img)
        {
            img.sprite = _unknown;
        }

        public void setUnknowIcon(Image img)
        {
            img.sprite = _unknownIcon;
        }

        public bool setSprite(Image img, Sprite asset)
        {
            if (asset == null)
            {
                asset = _unknownIcon;
            }

            img.sprite = asset;
            return asset != _unknownIcon;
        }
        
        public void setCommonIcon(Image img, string iconName)
        {
            Sprite asset = getIcon(EFrameworkAsset.IconType_CommonUI, iconName);
            if (asset == null)
            {
                asset = LanMgr.GetImage(iconName);
            }
            checkAndLog(asset, EFrameworkAsset.IconType_CommonUI, iconName);
            setSprite(img, asset);
        }
        
        public void setItemIcon(Image img, string iconName)
        {
            Sprite asset = getIcon(EFrameworkAsset.IconType_Item, iconName);

            if (asset == null)
            {
                asset = getIcon(EFrameworkAsset.IconType_CommonUI, iconName);
            }

            if (asset == null)
            {
                asset = LanMgr.GetImage(iconName);
            }

            checkAndLog(asset, EFrameworkAsset.IconType_Item, iconName);
            checkAndLog(asset, EFrameworkAsset.IconType_CommonUI, iconName);
            setSprite(img, asset);
        }

        public void setHeadIcon(Image img, string iconName)
        {
            Sprite asset = getIcon(EFrameworkAsset.IconType_Head, iconName);
            if (asset == null)
            {
                asset = getIcon(EFrameworkAsset.IconType_Item, iconName);
            }

            if (asset == null)
            {
                asset = getIcon(EFrameworkAsset.IconType_CommonUI, iconName);
            }

            if (asset == null)
            {
                asset = LanMgr.GetImage(iconName);
            }

            checkAndLog(asset, EFrameworkAsset.IconType_Head, iconName);
            checkAndLog(asset, EFrameworkAsset.IconType_Item, iconName);
            checkAndLog(asset, EFrameworkAsset.IconType_CommonUI, iconName);
            setSprite(img, asset);
        }

        public void setLanguage(Image img, string iconName)
        {
            if (string.IsNullOrEmpty(iconName))
            {
                JQLog.LogError("图标名称为null");
            }

            Sprite asset = LanMgr.GetImage(iconName);
            checkAndLog(asset, "Lan", iconName);
            setSprite(img, asset);
        }
    }
}