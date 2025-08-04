/*----------------------------------------------------------------
// 文件名：BuildAtlasUtil.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/10/20 14:30:07
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using JQCore.tFileSystem;
using JQCore.tUtil;
using UnityEditor;
using UnityEditor.U2D;

namespace JQEditor.Build
{
    public static class BuildAtlasUtil
    {
        private static readonly Dictionary<string, Action<TextureImporter, string>> _handleDic = new();
        private static bool _showUI;

        /// <summary>
        ///     设置图集
        /// </summary>
        /// <param name="showUI"></param>
        /// <param name="exeEndAction"></param>
        public static void build(bool showUI, Action exeEndAction = null)
        {
            _showUI = showUI;
            _handleDic.Clear();
            // _handleDic.Add($"Assets/{PathUtil.RES_FOLDER}/UI/Atlas", setFolderPathWithoutAtlasType);
            _handleDic.Add($"Assets/{PathUtil.RES_FOLDER}/UI/CommonIcon", setFolderPathWithoutAtlasType);
//            _handleDic.Add($"Assets/{PathUtil.RES_FOLDER}/UI/SingleUse", setFolderPath);
//            _handleDic.Add($"Assets/{PathUtil.RES_FOLDER}/UI/Views", setDeep2FolderPath);
            _handleDic.Add($"Assets/{PathUtil.RES_FOLDER}/UI/Language", setFolderPathWithoutAtlasType);
            _handleDic.Add($"Assets/{PathUtil.RES_FOLDER}/UI/SingleUse", setOldTagPath);
            _handleDic.Add($"Assets/{PathUtil.RES_FOLDER}/UI/Common", setFolderPathWithoutAtlasType);
            _handleDic.Add($"Assets/{PathUtil.RES_FOLDER}/Common/Bottom", setFolderPathWithoutAtlasType);
            _handleDic.Add($"Assets/{PathUtil.RES_FOLDER}/UI/Main", setMainAtlas);
            _handleDic.Add($"Assets/{PathUtil.RES_FOLDER}/UI/MainIcon", setMainAtlas);
            //            _handleDic.Add($"Assets/{PathUtil.RES_FOLDER}/Scene", setNull);

            buildAll();
            exeEndAction?.Invoke();
        }

        public static void packer(bool showUI, Action exeEndAction = null)
        {
            SpriteAtlasUtility.PackAllAtlases(EditorUserBuildSettings.activeBuildTarget);
            // Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, showUI);
            exeEndAction?.Invoke();
        }

        private static bool isPic(string path)
        {
            return path.EndsWith("png") || path.EndsWith("jpg");
        }

        private static void buildAll()
        {
            var pathList = new List<string>();
            JQFileUtil.getAllFile(ref pathList, $"Assets/{PathUtil.RES_FOLDER}", isPic);
            for (var i = 0; i < pathList.Count; i++)
            {
                var path = pathList[i];
                if (_showUI) EditorUtility.DisplayProgressBar($"设置图集({removePathHead(path)})...", $"{i}/{pathList.Count}", (float)i / pathList.Count);
                var newPath = path.Replace("\\", "/");
                var assetImporter = AssetImporter.GetAtPath(newPath);
                var textureImporter = assetImporter as TextureImporter;
                if (assetImporter == null || textureImporter == null) continue;
                var preTag = textureImporter.spritePackingTag;
                textureImporter.spritePackingTag = null;
                foreach (var keyValuePair in _handleDic)
                {
                    var checkPath = keyValuePair.Key;
                    if (newPath.Contains(checkPath))
                    {
                        var handle = keyValuePair.Value;
                        handle(textureImporter, preTag);
                        break;
                    }
                }

                if (!preTag.Equals(textureImporter.spritePackingTag))
                {
                    if (textureImporter.spriteImportMode != SpriteImportMode.Multiple) textureImporter.spriteImportMode = SpriteImportMode.Single;
                    //                    Debug.Log($"设置图集：{textureImporter.assetPath}          {textureImporter.spritePackingTag}");
                    textureImporter.SaveAndReimport();
                }
            }

            if (_showUI) EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        private static void setNull(TextureImporter textureImporter)
        {
            textureImporter.spritePackingTag = null;
        }

        private static void setSelf(TextureImporter textureImporter)
        {
            string folderPath = JQFileUtil.getParentFolderPath(textureImporter.assetPath);
            string fileName = JQFileUtil.getCurrFolderOrFileName(textureImporter.assetPath);
            folderPath = removePathHead(folderPath);
            textureImporter.spritePackingTag = folderPath.ToLower() + "/" + fileName;
        }

        private static void setOldTagPath(TextureImporter textureImporter, string oldTag)
        {
            textureImporter.spritePackingTag = oldTag;
        }

        private static void setMainAtlas(TextureImporter textureImporter, string oldTag)
        {
            textureImporter.spritePackingTag = "ui/main";
        }

        private static void setFolderPathWithoutAtlasType(TextureImporter textureImporter, string oldTag)
        {
            string folderPath = JQFileUtil.getParentFolderPath(textureImporter.assetPath);
            folderPath = removePathHead(folderPath);
            textureImporter.spritePackingTag = folderPath.ToLower();
        }

        private static void setFolderPath(TextureImporter textureImporter, string oldTag)
        {
            string folderPath = JQFileUtil.getParentFolderPath(textureImporter.assetPath);
            folderPath = removePathHead(folderPath);
            textureImporter.spritePackingTag = folderPath.ToLower() + getAtlasType(textureImporter);
        }

        private static void setDeep2FolderPath(TextureImporter textureImporter)
        {
            string folderPath = JQFileUtil.getParentFolderPath(textureImporter.assetPath);
            folderPath = JQFileUtil.getParentFolderPath(folderPath);
            folderPath = removePathHead(folderPath);
            textureImporter.spritePackingTag = folderPath.ToLower() + getAtlasType(textureImporter);
        }

        private static string removePathHead(string path)
        {
            return path.Replace("\\", "/").Replace($"Assets/{PathUtil.RES_FOLDER}/", "");
        }

        public static string getAtlasType(TextureImporter textureImporter)
        {
            if (textureImporter.DoesSourceTextureHaveAlpha()) return "_rgba";
            return "_rgb";
        }
    }
}