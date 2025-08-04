using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace JQEditor.Check.tCheckTexture
{
    public class CheckSpriteAtlasInfo
    {
        private bool isIncludeInBuild = true;
        private int maxSpriteAtlasSize = 2048;

        public SpriteAtlasAsset spriteAtlas;

        public List<Sprite> spriteList = new List<Sprite>();
        private string _atlasPath;

        public CheckSpriteAtlasInfo(string folderPath, string fileName)
        {
            string path = $"{folderPath}/{fileName}.spriteatlasv2";
            _atlasPath = path;
            //spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);// v1 是SpriteAtlas v2 是SpriteAtlasAsset
            spriteAtlas = SpriteAtlasAsset.Load(path);
            if (spriteAtlas != null)
            {
                AssetDatabase.DeleteAsset(path);
            }

            spriteAtlas = new SpriteAtlasAsset();
            
            SetUpAtlasInfo(ref spriteAtlas);
            // SpriteAtlasAsset.Save(spriteAtlas, path);
            // AssetDatabase.CreateAsset(spriteAtlas, path);
        }


        private void SetUpAtlasInfo(ref SpriteAtlasAsset atlas)
        {
            atlas.SetIncludeInBuild(isIncludeInBuild);
            
            //A区域参数设定
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                // enableAlphaDilation = true,
                padding = 4,
            };
            //B区域参数设定
            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            atlas.SetTextureSettings(textureSetting);
            //C区域参数设定
            SetTextureSettings(atlas, "WeixinMiniGame", true, 2048, TextureImporterFormat.ASTC_6x6, 80, false, true);
            SetTextureSettings(atlas, "Android", true, 2048, TextureImporterFormat.ASTC_4x4, 80, false, true);
            SetTextureSettings(atlas, "iPhone", true, 2048, TextureImporterFormat.PVRTC_RGB4, 100, false, true);
            SetTextureSettings(atlas, "WebGL", true, 2048, TextureImporterFormat.ASTC_6x6, 80, false, true);
        }

        private static void SetTextureSettings(SpriteAtlasAsset importer, string name, bool isDivisibleOf4,
            int maxTextureSize, TextureImporterFormat format, int compressionQuality, bool checkFormat, bool isAlpha)
        {
//        Debug.Log(isDivisibleOf4);

//        Debug.Log(format);
            TextureImporterPlatformSettings settings = importer.GetPlatformSettings(name);
            settings.overridden = true;
            settings.maxTextureSize = maxTextureSize;
            settings.format = TextureImporterFormat.ASTC_6x6;
            settings.compressionQuality = compressionQuality;
            settings.allowsAlphaSplitting = true;
            importer.SetPlatformSettings(settings);
        }

        public void AddSpirte(Sprite sprite)
        {
            // if (spriteAtlas.GetSprite(sprite.name) == null)
            // {
            if (!spriteList.Contains(sprite))
            {
                spriteList.Add(sprite);
            }
            // }
        }

        public void AddSprites(Sprite[] sprites)
        {
            spriteList.Clear();
            foreach (Sprite sprite in sprites)
            {
                AddSpirte(sprite);
            }

            Save();
        }

        public void Save()
        {
            if (spriteList.Count > 0)
            {
                spriteAtlas.Add(spriteList.ToArray());
            }

            SpriteAtlasAsset.Save(spriteAtlas, _atlasPath);
        }
    }
}