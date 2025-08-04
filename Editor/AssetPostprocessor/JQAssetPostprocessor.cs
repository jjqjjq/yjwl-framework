using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;

namespace JQEditor.tAssetPostprocessor
{
    public class JQAssetPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessAsset()
        {
            if (assetImporter.assetPath.Contains("Temp")) return;
            if (!FileTypeTools.IsAtlas(assetPath)) return;
            SpriteAtlasImporter spriteAtlasImporter = assetImporter as SpriteAtlasImporter;
            spriteAtlasImporter.includeInBuild = true;
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                // enableAlphaDilation = true,
                padding = 4,
            };
            spriteAtlasImporter.packingSettings = packSetting;
            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            spriteAtlasImporter.textureSettings = textureSetting;
            
            
            SetTextureSettings(spriteAtlasImporter, "WeixinMiniGame", true, 2048, TextureImporterFormat.ASTC_8x8, 80, false, true);
            SetTextureSettings(spriteAtlasImporter, "Android", true, 2048, TextureImporterFormat.ASTC_4x4, 80, false, true);
            SetTextureSettings(spriteAtlasImporter, "iPhone", true, 2048, TextureImporterFormat.PVRTC_RGB4, 100, false, true);
            SetTextureSettings(spriteAtlasImporter, "WebGL", true, 4096, TextureImporterFormat.ASTC_10x10, 80, false, true);
        }
        
        private static void SetTextureSettings(SpriteAtlasImporter importer, string name, bool isDivisibleOf4,
            int maxTextureSize, TextureImporterFormat format, int compressionQuality, bool checkFormat, bool isAlpha)
        {
//        Debug.Log(isDivisibleOf4);

//        Debug.Log(format);
            TextureImporterPlatformSettings settings = importer.GetPlatformSettings(name);
            settings.overridden = true;
            settings.maxTextureSize = maxTextureSize;
            settings.format = format;
            settings.compressionQuality = compressionQuality;
            settings.allowsAlphaSplitting = isAlpha;
            importer.SetPlatformSettings(settings);
        }
        
        //贴图处理
        private void OnPreprocessTexture()
        {
            if (assetImporter.assetPath.Contains("Temp")) return;
            if (!FileTypeTools.IsTexture(assetPath)) return;
            //        Debug.Log("处理贴图。。。。。");
            if (ProjectTypeTools.IsLightmap(assetPath))
            {
                TextureSetting.ApplyLightmapSettings(assetImporter as TextureImporter);
            }
            else if (ProjectTypeTools.IsTerrainResource(assetPath))
            {
                TextureSetting.ApplyTerrainTextureSettings(assetImporter as TextureImporter);
            }
            else if (ProjectTypeTools.IsEffectResource(assetPath))
            {
                TextureSetting.ApplyEffectTextureSettings(assetImporter as TextureImporter);
            }
            else if (ProjectTypeTools.IsModelResource(assetPath))
            {
                TextureSetting.ApplyModelTextureSettings(assetImporter as TextureImporter);
            }
            else if (ProjectTypeTools.IsUIResource(assetPath))
            {
                TextureSetting.ApplyUITextureSettings(assetImporter as TextureImporter);
            }
            else if (ProjectTypeTools.isSceneResource(assetPath))
            {
                TextureSetting.ApplySceneTextureSettings(assetImporter as TextureImporter);
            }
            else if (ProjectTypeTools.isLanResource(assetPath))
            {
                TextureSetting.ApplyUITextureSettings(assetImporter as TextureImporter);
            }
            else if (ProjectTypeTools.IsCardResource(assetPath))
            {
                TextureSetting.ApplyCardTextureSettings(assetImporter as TextureImporter);
            }
            else if (ProjectTypeTools.IsTMPFontResource(assetPath))
            {
                // TextureSetting.ApplyCommonTextureSettings(assetImporter as TextureImporter);
            }
            else if (ProjectTypeTools.IsCommonResource(assetPath))
            {
                TextureSetting.ApplyCommonTextureSettings(assetImporter as TextureImporter);
            }
            else if (ProjectTypeTools.IsResourcesResource(assetPath))
            {
                TextureSetting.ApplyResourceTextureSettings(assetImporter as TextureImporter);
            }
        }

        //模型导入前处理
        private void OnPreprocessModel()
        {
            if (assetImporter.assetPath.Contains("Temp")) return;
            if (ProjectTypeTools.IsEffectResource(assetPath) && FileTypeTools.IsFBX(assetPath))
            {
                ModelSetting.ApplyEffectModelSettings(assetImporter as ModelImporter);
                return;
            }

            if (ProjectTypeTools.IsModelResource(assetPath) && FileTypeTools.IsFBX(assetPath))
            {
                ModelSetting.ApplyFBXModelSettings(assetImporter as ModelImporter);
                return;
            }

            if (ProjectTypeTools.IsCardResource(assetPath) && FileTypeTools.IsFBX(assetPath))
            {
                ModelSetting.ApplyEffectModelSettings(assetImporter as ModelImporter);
                return;
            }
        }

        //模型导入后处理
        private void OnPostprocessModel(GameObject obj)
        {
            if (assetImporter.assetPath.Contains("Temp")) return;
            //        Debug.Log("处理挂载点!!!");
            // if (ProjectTypeTools.IsModelResource(assetPath) && FileTypeTools.IsFBX(assetPath))
            // {
            //
            //     ModelSetting.ApplyExtraExposedTransformPaths(assetImporter as ModelImporter, obj);
            // }
        }

        private void OnPreprocessAudio()
        {
            if (assetImporter.assetPath.Contains("Temp")) return;
            //        Debug.Log("处理音乐音效!!!");
            if (ProjectTypeTools.IsSoundResource(assetPath) && FileTypeTools.IsAudio(assetPath))
            {
                AudioSetting.ApplyMusicAudio(assetImporter as AudioImporter);
            }
        }

        private void OnPostprocessAnimation(GameObject go, AnimationClip clip)
        {
            //        Debug.Log("处理动作文件!!!");
            if (ProjectTypeTools.IsModelResource(assetPath) && FileTypeTools.IsAnim(assetPath))
            {
                AnimSetting.ApplyAnimSettings(clip);
            }
        }
    }
}