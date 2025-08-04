using System.Collections.Generic;
using JQCore.tLog;
using JQCore.tSingleton;
using JQCore.Log;
#if TextMeshPro
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace JQAppStart.tLuancher
{
    public class ShaderManager : JQSingleton<ShaderManager>
    {
        public static string[] buildInShaders =
        {
            //内置Shader
            "Hidden/CubeBlur",
            "Hidden/CubeCopy",
            "Hidden/CubeBlend",
            "Hidden/VideoDecode",
            "Hidden/Compositing",
            "Hidden/BlitCopy",
            
            "UI/Default",
            "UI/DefaultETC1",
            
            "Sprites/Default",
            "Sprites/Diffuse",

            "Skybox/Cubemap",
            "Skybox/6 Sided",
            "Skybox/Procedural",
            "Hidden/Internal-GUITextureClip",
            "Hidden/Internal-GUITextureClipText",
            "Hidden/Internal-GUITexture",
            "Hidden/Internal-GUITextureBlit",
            "Hidden/Internal-GUIRoundedRect",
            "Hidden/Internal-GUIRoundedRectWithColorPerBorder",
            "Hidden/Internal-UIRAtlasBlitCopy",
            "Hidden/UIElements/EditorUIE",
            "Hidden/GraphView/GraphViewUIE",
            "Unlit/Texture",
            "Unlit/Transparent",
            
            "Particles/Standard Unlit",
            "Mobile/Particles/Additive",
            "Mobile/Particles/Alpha Blended",
            "Mobile/Particles/VertexLit Blended",
            "Mobile/Unlit (Supports Lightmap)",
            
            "Legacy Shaders/Diffuse",
            "Legacy Shaders/Particles/Additive",
            "Legacy Shaders/Particles/Additive (Soft)",
            "Legacy Shaders/Particles/Alpha Blended",
            "Legacy Shaders/Particles/Alpha Blended Premultiply",
            "Legacy Shaders/Particles/Multiply",
            "Legacy Shaders/Particles/Multiply (Double)",
            "Legacy Shaders/Transparent/Diffuse",
        };


        public static string[] noCheck =
        {
        };


        public static string[] HyShaders = new[]
        {
            // --第三方shader
            "TextMeshPro/Sprite",
            "TextMeshPro/Mobile/Distance Field",
            "TextMeshPro/Mobile/Distance Field - Masking",
            "TextMeshPro/Mobile/Distance Field Overlay",
            "TextMeshPro/Distance Field-BillboardFacing",
            
            //URP
            "Universal Render Pipeline/Lit",
            "Universal Render Pipeline/Particles/Unlit",
            "Universal Render Pipeline/Particles/Lit",
            "Universal Render Pipeline/Autodesk Interactive/AutodeskInteractive",
            "Universal Render Pipeline/Nature/SpeedTree7",
            "Universal Render Pipeline/Nature/SpeedTree7 Billboard",
            "Universal Render Pipeline/Terrain/Lit",
            "Hidden/Universal Render Pipeline/Blit",
            "Hidden/Universal Render Pipeline/CopyDepth",
            
            //SPI-URP
            "spi/Universal Render Pipeline/Lit",
            "spi/Universal Render Pipeline/Particles/Unlit",
            "spi/Universal Render Pipeline/Particles/Lit",
            "spi/Universal Render Pipeline/Autodesk Interactive/AutodeskInteractive",
            "spi/Universal Render Pipeline/Nature/SpeedTree7",
            "spi/Universal Render Pipeline/Nature/SpeedTree7 Billboard",
            "spi/Universal Render Pipeline/Terrain/Lit",
            "Hidden/spi/Universal Render Pipeline/FallbackError",
            "Hidden/spi/Core/FallbackError",
            
            
            "spi/Shader Graphs/PhysicalMaterial3DsMax",
            
            // "UnityChanToonShader/Toon_ShadingGradeMap_Mini", //--角色主材质
            
            
            "UI/Particles/GlowAdditiveSimple",
            "UI/Particles/Additive",
            "UI/Particles/Alpha Blended",
            "UI/Particles/Multiply",
            
            // --自编shader
            "spi/Particles/Additive",
            "spi/RimLight",
            "spi/PathGuide",
            "spi/Billboard",
            "spi/Distortion",
            "spi/Scan",
            "spi/UI/Default_No_Alpha",
            
            // 启元
            "spi/lianxi_liuguang_shader",
            "spi/Additive Intensify",
            "spi/Water",
            "spi/Add",
            "spi/Add_uv",
            "spi/GrassMesh",
            "spi/WaterFall",
            "spi/Earth1",
            "spi/Earth2",
            "spi/SaoGuang",
            "spi/Legacy/Atmosphere",
            "spi/Legacy/Rings",
            "spi/VertColorAdd",
            "spi/Legacy/Planet",
            
            // "Sprites/ClearBlackSimple",
            // "Sprites/PureColor",
            // "UI/Default_No_Alpha", //--镜头拍特效挂在RawImage上得以正常显示
            // "swan/FlowLight", //--UV遮罩流动效果
            // "swan/FlowLight (Alpha)", //--UV遮罩流动效果带Alpha透明
            // "Haoyue/UI/Mask", //--UI遮罩
            // "Haoyue/UI/Gray", //--UI
            // "Haoyue/UI/Particles/Additive", //--可实现被UI遮罩的ADD
            // "Haoyue/UI/Particles/Alpha Blended", //--可实现被UI遮罩的AlphaBlended
            // "Haoyue/Temp/ZhaiYue/miaobian", //--描边渐变透明材质
            // "Custom/Dissolve2", //--特效UV流动
            // "Custom/Mask Additive Tint Dyn UV", //--特效UV流动
            // "Custom_VFX/UV_move_Add", //--特效UV流动
            // "MC/BasicRole", //--可调色可调透明度材质
            // "FXMaker/Mask Alpha Blended Tint", //--带遮罩的AlphaBlended
            // "DZ/DoubleFaceAAlphaTest", //--可以通过贴图透明度控制部分透明
            // "Custom/WaterWave Effect" //--程序用的切图波浪
        };

        public void clear()
        {
            _shaderDic.Clear();
        }

        public ShaderManager()
        {
            InitAllShader();
        }

        public override void Dispose()
        {
            _shaderDic.Clear();
        }

        private readonly Dictionary<string, Shader> _shaderDic = new Dictionary<string, Shader>();


        public Shader GetShader(string name)
        {
            Shader shader;
            _shaderDic.TryGetValue(name, out shader);
            return shader;
        }
        private Material grayShader;
        
        public void SetImageGray_EX(Image img)
        {
            if (grayShader == null)
            {
                grayShader = new Material(GetShader("Haoyue/UI/Gray"));
            }
            img.material = grayShader;
        }

        public void InitAllShader()
        {
            return;
            InitShaderArr(buildInShaders);
            InitShaderArr(HyShaders);
        }
        
        private void InitShaderArr(string[] shaderArr)
        {
            #if UNITY_EDITOR
            foreach (var shader in shaderArr)
            {
                AddShader(shader);
            }
            #endif
        }

        public void AddShader(string name)
        {
            Shader shader = Shader.Find(name);
            if (shader != null && !_shaderDic.ContainsKey(shader.name))
            {
                JQLog.Log("++++++" + shader.name + "++++++");
                _shaderDic.Add(shader.name, shader);
            }
        }
        //
        // public void resetShader(Object asset, string name)
        // {
        //     return;
        //     if (asset is GameObject)
        //     {
        //         GameObject go = asset as GameObject;
        //         Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        //         foreach (Renderer renderer in renderers)
        //         {
        //             Material[] materials = renderer.sharedMaterials;
        //             for (int j = 0; j < materials.Length; j++)
        //             {
        //                 Material material = materials[j];
        //                 if (material != null && material.shader != null)
        //                 {
        //                     resetShader(material, renderer.name, name);
        //                 }
        //             }
        //         }
        //
        //         TMP_Text[] tmpTexts = go.GetComponentsInChildren<TMP_Text>(true);
        //         foreach (var tmpText in tmpTexts)
        //         {
        //             if (tmpText.font != null && tmpText.font.material != null)
        //             {
        //                 resetShader(tmpText.font.material, tmpText.name, name);
        //             }
        //             if(tmpText.fontSharedMaterial != null)
        //             {
        //                 resetShader(tmpText.fontSharedMaterial, tmpText.name, name);
        //             }
        //         }
        //     }
        //     else if (asset is Material)
        //     {
        //         Material material = asset as Material;
        //         resetShader(material, asset.name, name);
        //     }
        //     else if (asset is Font)
        //     {
        //         Font font = asset as Font;
        //         resetShader(font.material, asset.name, name);
        //     }
        //     else if (asset is TMP_FontAsset)
        //     {
        //         TMP_FontAsset font = asset as TMP_FontAsset;
        //         resetShader(font.material, asset.name, name);
        //     }
        //     else if (asset is TMP_SpriteAsset)
        //     {
        //         TMP_SpriteAsset font = asset as TMP_SpriteAsset;
        //         resetShader(font.material, asset.name, name);
        //     }
        //     else if (asset is TextAsset || asset is Texture2D || asset is AudioClip || asset is Sprite)
        //     {
        //     }
        //     else
        //     {
        //         JQLog.Log($"未判断的资源类型：{asset.GetType()}, name:{asset.name}");
        //     }
        // }

        // private void resetShader(Material material, string name1, string name2)
        // {
        //     if (material != null && material.shader != null)
        //     {
        //         if (_shaderDic.ContainsKey(material.shader.name))
        //         {
        //             material.shader = _shaderDic[material.shader.name];
        //         }
        //         else if (material.shader.name.Equals("GUI/Text Shader"))
        //         {
        //         }
        //         else
        //         {
        //             JQLog.LogError($"Shader未管理：shaderName:{material.shader.name}   linkName：{name1}  materialName:{name2}");
        //             material.shader = Shader.Find(material.shader.name);
        //         }
        //     }
        // }
    }
}