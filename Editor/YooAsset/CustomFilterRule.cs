using System.IO;
using JQCore.tRes;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace YooAsset.Editor
{
    public class CustomFilterRule
    {
        [DisplayName("收集所有资源(JQ)")]
        public class JQCollectAll : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }

                return true;
            }
        }

        [DisplayName("收集场景(JQ)")]
        public class JQCollectScene : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }

#if TUANJIE_WEIXINMINIGAME
                return Path.GetExtension(data.AssetPath) == ".scene";
#else
                return Path.GetExtension(data.AssetPath) == ".unity";
#endif
            }
        }

        [DisplayName("收集音效(JQ)")]
        public class JQCollectSound : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }

                string extension = Path.GetExtension(data.AssetPath);
                return extension == ".mp3";
            }
        }
        
        [DisplayName("收集Fbx(JQ)")]
        public class JQCollectFbx : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }

                return Path.GetExtension(data.AssetPath) == ".fbx";
            }
        }

        [DisplayName("收集预制体(JQ)")]
        public class JQCollectPrefab : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }

                return Path.GetExtension(data.AssetPath) == ".prefab";
            }
        }
        
        [DisplayName("收集资源集合(JQ)")]
        public class JQAssetObjectLib : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(data.AssetPath);
                if (gameObject == null)
                {
                    return false;
                }
                AssetObjectLib assetObjectLib = gameObject.GetComponent<AssetObjectLib>();
                if (assetObjectLib == null || assetObjectLib.assets.Length == 0)
                {
                    return false;
                }

                return Path.GetExtension(data.AssetPath) == ".prefab";
            }
        }
        
        [DisplayName("收集光照贴图(JQ)")]
        public class JQCollectLightmap : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }
                if (!data.AssetPath.Contains("Lightmap"))
                {
                    return false;
                }

                var mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath);
                if (mainAssetType == typeof(Texture2D))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        [DisplayName("收集非精灵类型的纹理(JQ)")]
        public class JQCollectTexture : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }
                if (data.AssetPath.Contains("Lightmap"))
                {
                    return false;
                }

                var mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath);
                if (mainAssetType == typeof(Texture2D))
                {
                    var texImporter = AssetImporter.GetAtPath(data.AssetPath) as TextureImporter;
                    if (texImporter != null && texImporter.textureType != TextureImporterType.Sprite)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
        }

        [DisplayName("收集精灵类型的纹理(JQ)")]
        public class JQCollectSprite : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }
                if (data.AssetPath.Contains("Lightmap"))
                {
                    return false;
                }

                var mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath);
                if (mainAssetType == typeof(Texture2D))
                {
                    var texImporter = AssetImporter.GetAtPath(data.AssetPath) as TextureImporter;
                    if (texImporter != null && texImporter.textureType == TextureImporterType.Sprite)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
        }

        [DisplayName("收集着色器变种集合(JQ)")]
        public class JQCollectShaderVariants : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }

                return Path.GetExtension(data.AssetPath) == ".shadervariants";
            }
        }
        
        [DisplayName("收集图集(JQ)")]
        public class JQCollectSprtieAtlas : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp")) return false;
                if (!Path.GetExtension(data.AssetPath).Contains(".spriteatlas")) return false;
                SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(data.AssetPath);
                if (spriteAtlas == null) return false;
                if (spriteAtlas.spriteCount == 0) return false;
                return true;
            }
        }

        [DisplayName("收集着色器(JQ)")]
        public class JQCollectShader : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }

                return Path.GetExtension(data.AssetPath) == ".shader";
            }
        }

        [DisplayName("收集JSON(JQ)")]
        public class JQCollectJson : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (data.AssetPath.Contains("Temp"))
                {
                    return false;
                }

                return Path.GetExtension(data.AssetPath) == ".json";
            }
        }
    }
}