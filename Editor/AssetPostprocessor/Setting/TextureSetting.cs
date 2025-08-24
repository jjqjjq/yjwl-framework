using UnityEngine;
using UnityEditor;

namespace JQEditor.tAssetPostprocessor
{
    public class TextureSetting
    {
        private static void setHighQuality(TextureImporter importer, int targetTextureMaxSize, bool checkFormat,
            bool checkAlpha = true)
        {
            bool isDivisibleOf4 = AssetTools.IsDivisibleOf4(importer, targetTextureMaxSize);
            if (!checkAlpha || importer.DoesSourceTextureHaveAlpha())
            {
//            importer.alphaIsTransparency = true;
#if SDK_WEIXIN
#if WEIXINMINIGAME
                SetTextureSettings(importer, "WeixinMiniGame", isDivisibleOf4, targetTextureMaxSize,TextureImporterFormat.ASTC_6x6, 100, checkFormat, true);
#else
                SetTextureSettings(importer, "WebGL", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.ASTC_6x6, 80, checkFormat, true);
#endif
#endif
                SetTextureSettings(importer, "Android", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.ASTC_6x6, 80,
                    checkFormat, true);
                SetTextureSettings(importer, "iPhone", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.PVRTC_RGBA4,
                    100, checkFormat, true);
            }
            else
            {
                importer.alphaIsTransparency = false;
#if SDK_WEIXIN
#if WEIXINMINIGAME
                SetTextureSettings(importer, "WeixinMiniGame", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.ASTC_6x6, 100, checkFormat, true);
#else
                SetTextureSettings(importer, "WebGL", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.ASTC_6x6, 80,
                    checkFormat, true);
#endif
#endif
                SetTextureSettings(importer, "Android", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.ASTC_6x6, 80,
                    checkFormat, true);
                SetTextureSettings(importer, "iPhone", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.PVRTC_RGB4,
                    100, checkFormat, true);
            }
        }

        private static void setLowQuality(TextureImporter importer, int targetTextureMaxSize, bool checkFormat,
            bool checkAlpha = true)
        {
            bool isDivisibleOf4 = AssetTools.IsDivisibleOf4(importer, targetTextureMaxSize);
            if (!checkAlpha || importer.DoesSourceTextureHaveAlpha())
            {
//            importer.alphaIsTransparency = true;


#if SDK_WEIXIN
#if WEIXINMINIGAME
                SetTextureSettings(importer, "WeixinMiniGame", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.ASTC_10x10, 100, checkFormat, true);
#else
                SetTextureSettings(importer, "WebGL", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.ASTC_10x10, 100,
                    checkFormat, true);

#endif
#endif
                SetTextureSettings(importer, "Android", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.ASTC_8x8, 100,
                    checkFormat, true);
                SetTextureSettings(importer, "iPhone", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.PVRTC_RGBA4,
                    100, checkFormat, true);
            }
            else
            {
                importer.alphaIsTransparency = false;
#if SDK_WEIXIN
#if WEIXINMINIGAME
                SetTextureSettings(importer, "WeixinMiniGame", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.ASTC_10x10, 100, checkFormat, true);
#else
                SetTextureSettings(importer, "WebGL", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.ASTC_10x10, 100,
                    checkFormat, true);
#endif
#endif
                SetTextureSettings(importer, "Android", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.ASTC_6x6, 100,
                    checkFormat, true);
                SetTextureSettings(importer, "iPhone", isDivisibleOf4, targetTextureMaxSize, TextureImporterFormat.PVRTC_RGBA4,
                    100, checkFormat, true);
            }
        }

        public static void SetTextureSettings(TextureImporter importer, string name, bool isDivisibleOf4, int maxTextureSize,
            TextureImporterFormat format, int compressionQuality, bool checkFormat, bool isAlpha)
        {
//        Debug.Log(isDivisibleOf4);
            // if (checkFormat)
            // {
            //     if (name == "WeixinMiniGame" && !isDivisibleOf4)
            //     {
            //         format = isAlpha ? TextureImporterFormat.ASTC_10x10 : TextureImporterFormat.ASTC_10x10;
            //     }
            //     
            //     if (name == "WebGL" && !isDivisibleOf4)
            //     {
            //         format = isAlpha ? TextureImporterFormat.ASTC_10x10 : TextureImporterFormat.ASTC_10x10;
            //     }
            //
            //     if (name == "Android" && !isDivisibleOf4)
            //     {
            //         format = isAlpha ? TextureImporterFormat.ASTC_6x6 : TextureImporterFormat.ASTC_6x6;
            //     }
            //
            //     if (name == "iPhone" && !isDivisibleOf4)
            //     {
            //         format = isAlpha ? TextureImporterFormat.ASTC_6x6 : TextureImporterFormat.ASTC_6x6;
            //     }
            // }

//        Debug.Log(format);
            TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings(name);
            settings.overridden = true;
            settings.maxTextureSize = maxTextureSize;
            settings.format = format;
            settings.compressionQuality = compressionQuality;
            settings.allowsAlphaSplitting = true;
            importer.SetPlatformTextureSettings(settings);
        }


        //设置特效贴图默认参数
        public static void ApplyEffectTextureSettings(TextureImporter importer)
        {
            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
            }

            if (importer.isReadable)
            {
                importer.isReadable = false;
            }

            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear; //线性采样
            }

            if (importer.anisoLevel != 0)
            {
                importer.anisoLevel = 0;
            }

            //目标贴图参数
            CheckTextureName(importer, 1024, 1024);
        }

        //设置Terrain贴图
        public static void ApplyTerrainTextureSettings(TextureImporter importer)
        {
            if (!importer.isReadable)
            {
                importer.isReadable = true;
            }

            //目标贴图参数
            CheckTextureName(importer, 1024, 1024);
        }

        //设置场景图片
        public static void ApplySceneTextureSettings(TextureImporter importer)
        {
            //目标贴图参数
            CheckTextureName(importer, 2048, 2048);
        }


        //光照贴图设置
        public static void ApplyLightmapSettings(TextureImporter importer)
        {
            // if (!importer.mipmapEnabled)
            // {
            //     importer.mipmapEnabled = true;
            // }
            // if (importer.isReadable)
            // {
            //     importer.isReadable = false;
            // }
            // if (importer.filterMode != FilterMode.Bilinear)
            // {
            //     importer.filterMode = FilterMode.Bilinear; //线性采样
            // }
            // if (importer.anisoLevel != 3)
            // {
            //     importer.anisoLevel = 3;
            // }
            // if (importer.textureType != TextureImporterType.Lightmap)
            // {
            //     importer.textureType = TextureImporterType.Lightmap;
            // }
            //目标贴图参数
            CheckTextureName(importer, 1024, 1024);
        }

        //设置UI图片
        public static void ApplyUITextureSettings(TextureImporter importer)
        {
            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
            }

            if (importer.isReadable)
            {
                importer.isReadable = false;
            }

            if (importer.npotScale != TextureImporterNPOTScale.None)
            {
                importer.npotScale = TextureImporterNPOTScale.None;
            }

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
            }

            if (importer.wrapMode != TextureWrapMode.Clamp)
            {
                importer.wrapMode = TextureWrapMode.Clamp;
            }

            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear; //线性采样
            }

            if (importer.anisoLevel != 0)
            {
                importer.anisoLevel = 0;
            }

            //目标贴图参数
            string textureName = importer.assetPath;
            if (textureName.Contains("_Best")) //RGBA32位
            {
                setHighQuality(importer, 2048, true);
            }
            else if (textureName.Contains("UITextureLoader")) //RGBA32位
            {
                setHighQuality(importer, 2048, true);
            }
            else if (textureName.Contains("UITextureLoaderCode")) //RGBA32位
            {
                setHighQuality(importer, 2048, true);
            }
            else if (textureName.Contains("UnUsed")) //RGBA32位
            {
                setHighQuality(importer, 2048, true);
            }
            else if (textureName.Contains("Atlas") || textureName.Contains("Common") || textureName.Contains("Main") ||
                     textureName.Contains("CommonIcon") || textureName.Contains("Language") ||
                     textureName.Contains("SingleUse")) //需合图集的目录
            {
                setHighQuality(importer, 1024, false, false); //小图会合图集
            }
            else if (textureName.Contains("UnCheck"))
            {
                setHighQuality(importer, 2048, true); //散图不合图集
            }
            else
            {
                setHighQuality(importer, 2048, true); //散图不合图集
            }
        }

        //设置Resource图片
        public static void ApplyResourceTextureSettings(TextureImporter importer)
        {
            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
            }

            if (importer.isReadable)
            {
                importer.isReadable = false;
            }

            if (importer.npotScale != TextureImporterNPOTScale.None)
            {
                importer.npotScale = TextureImporterNPOTScale.None;
            }

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
            }

            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear; //线性采样
            }

            if (importer.anisoLevel != 0)
            {
                importer.anisoLevel = 0;
            }

            //目标贴图参数
            
            CheckTextureName(importer, 2048, 1024);
        }
        
        private static void CheckTextureName(TextureImporter importer, int maxSize1, int maxSize2)
        {
            //目标贴图参数
            string textureName = importer.assetPath.ToLower();
            if (textureName.Contains("_best")) //RGBA32位
            {
                setHighQuality(importer, maxSize1, true);
            }
            else
            {
                setLowQuality(importer, maxSize2, true); //
            }
        }


        //设置Common图片
        public static void ApplyCommonTextureSettings(TextureImporter importer)
        {
            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
            }

            if (importer.isReadable)
            {
                importer.isReadable = false;
            }

            if (importer.npotScale != TextureImporterNPOTScale.None)
            {
                importer.npotScale = TextureImporterNPOTScale.None;
            }

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
            }

            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear; //线性采样
            }

            if (importer.anisoLevel != 0)
            {
                importer.anisoLevel = 0;
            }

            CheckTextureName(importer, 2048, 2048);
        }


        //设置Card图片
        public static void ApplyCardTextureSettings(TextureImporter importer)
        {
            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
            }

            if (importer.isReadable)
            {
                importer.isReadable = false;
            }

            if (importer.npotScale != TextureImporterNPOTScale.None)
            {
                importer.npotScale = TextureImporterNPOTScale.None;
            }

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
            }

            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear; //线性采样
            }

            if (importer.anisoLevel != 0)
            {
                importer.anisoLevel = 0;
            }

            //目标贴图参数
            CheckTextureName(importer, 2048, 1024);
        }

        //设置模型贴图默认参数
        public static void ApplyModelTextureSettings(TextureImporter importer)
        {
            string lowerAssetPath = importer.assetPath.ToLower();
            if (lowerAssetPath.EndsWith("_n.png") || lowerAssetPath.EndsWith("_n.jpg"))
            {
                if (importer.textureType != TextureImporterType.NormalMap)
                {
                    importer.textureType = TextureImporterType.NormalMap;
                }
            }
            else
            {
                if (importer.textureType != TextureImporterType.Default)
                {
                    importer.textureType = TextureImporterType.Default;
                }
            }

            if (importer.textureShape != TextureImporterShape.Texture2D)
            {
                importer.textureShape = TextureImporterShape.Texture2D;
            }

            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = true; //3D项目需要打开
            }

            if (importer.isReadable)
            {
                importer.isReadable = false;
            }

            if (importer.wrapMode != TextureWrapMode.Repeat)
            {
                importer.wrapMode = TextureWrapMode.Repeat;
            }

            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear; //线性采样
            }

            if (importer.anisoLevel != 0)
            {
                importer.anisoLevel = 0;
            }

            //目标贴图参数
            CheckTextureName(importer, 1024, 1024);
        }
    }
}