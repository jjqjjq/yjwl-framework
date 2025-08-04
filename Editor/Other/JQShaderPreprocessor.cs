using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JQCore.tFileSystem;
using JQCore.tUtil;
using JQEditor.Build;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using YooAsset.Editor;

namespace JQEditor.Other
{
    public class JQShaderPreprocessor : IPreprocessShaders
    {
        #region Shader变体剔除配置

        #region 全局黑名单

        //变体剔除shader列表
        private static string[] filterShaderArr = new[]
        {
            // "Universal Render Pipeline/Lit",
            // "Universal Render Pipeline/Terrain/Lit",
            // "Universal Render Pipeline/Particles/Lit",
            // "Universal Render Pipeline/Particles/Simple Lit",
            // "Universal Render Pipeline/Nature/SpeedTree7",
            // "Hidden/Universal Render Pipeline/Terrain/Lit (Base Pass)",
            // "Hidden/Universal Render Pipeline/Terrain/Lit (Add Pass)",
            // // "Hidden/spi/Universal Render Pipeline/UberPost",
            // "Hidden/Universal Render Pipeline/FinalPost",
            // // "Hidden/TerrainEngine/Details/UniversalPipeline/BillboardWavingDoublePass",
            // // "Hidden/TerrainEngine/Details/UniversalPipeline/WavingDoublePass",
            // // "Hidden/TerrainEngine/Details/UniversalPipeline/Vertexlit",
            // "Shader Graphs/PhysicalMaterial3DsMaxTransparent",
            // "Shader Graphs/PhysicalMaterial3DsMax",
            // "Shader Graphs/ArnoldStandardSurface",
            // "AVProVideo/Lit/Diffuse (texture+color+fog+stereo support)",
            // "Raygeas/AZURE Surface",
            "Raygeas/AZURE Vegetation",
        };

        //变体剔除shader关键字列表
        private static ShaderKeyword[] filterShaderKeywordArr = new ShaderKeyword[]
        {
            //雾效选项
            // new GlobalKeyword("FOG_LINEAR"),
            new ShaderKeyword("FOG_EXP"),
            new ShaderKeyword("FOG_EXP2"),

            //白名单
            // new GlobalKeyword("_ALPHATEST_ON"),
            // new ShaderKeyword("_EMISSION"),
            // new ShaderKeyword("_ALPHAPREMULTIPLY_ON"),//不能关，否则无法透明叠加
            // new ShaderKeyword("_REFLECTION_PROBE_BLENDING"),//不能关，否则没阴影
            // new ShaderKeyword("_METALLICSPECGLOSSMAP"),   //金属光泽度贴图
            // new ShaderKeyword("_SPECGLOSSMAP"),          //光泽度贴图
            // new ShaderKeyword("_SURFACE_TYPE_TRANSPARENT"),//透明材质   
            // new ShaderKeyword("_RECEIVE_SHADOWS_OFF"), //关闭接收阴影  

            //Lit优化
            new ShaderKeyword("_SPECULAR_SETUP"),
            new ShaderKeyword("_DBUFFER_MRT3"),
            new ShaderKeyword("_DETAIL_MULX2"),
            new ShaderKeyword("_DETAIL_SCALED"),
            new ShaderKeyword("_REQUIRE_UV2"),
            new ShaderKeyword("_FADING_ON"),
            new ShaderKeyword("EFFECT_BUMP"),
            new ShaderKeyword("_COLOROVERLAY_ON"),
            new ShaderKeyword("_COLORADDSUBDIFF_ON"),
            new ShaderKeyword("_ADDITIONAL_LIGHTS_VERTEX"),
            new ShaderKeyword("LIGHTMAP_SHADOW_MIXING"),
            new ShaderKeyword("DYNAMICLIGHTMAP_ON"),
            new ShaderKeyword("_OCCLUSIONMAP"), //环境光遮蔽贴图
            new ShaderKeyword("_PARALLAXMAP"), //Height Map 视差贴图，高度图

            // new ShaderKeyword("_ALPHAPREMULTIPLY_ON"),//预乘Alpha
            // new ShaderKeyword("_ALPHATEST_ON"),//Alpha测试

            //spi/Universal Render Pipeline/Lit

            //阴影模式
            // new GlobalKeyword("_MAIN_LIGHT_SHADOWS"),
            // new GlobalKeyword("_MAIN_LIGHT_SHADOWS_CASCADE"),
        };

        #endregion

        #region 单shader特性黑名单

        private Dictionary<string, List<ShaderKeyword>> _shaderFilterDic;

        private const string
            KEYWORD_LIGHT_COOKIES = "_LIGHT_COOKIES"; //Light Cookies 是一种使用Light组件的纹理或 Cubemap 来实现遮蔽或过滤投射光以产生图案照明的技术。它可以更改投射光的外观颜色、形状和强度，以实现一些复杂照明场景的模拟，同时对运行时性能的影响是最小的。我们可以使用Light Cookies实现很多炫酷的效果，可用于模拟焦散、假阴影图案和复杂的光照形状，从而为这些场景增加烘托处一种氛围。

        private const string KEYWORD_REFLECTION_PROBE_BLENDING = "_REFLECTION_PROBE_BLENDING"; //反射探针混合

        private void initShaderFilterDic()
        {
            if (_shaderFilterDic != null) return;
            _shaderFilterDic = new Dictionary<string, List<ShaderKeyword>>();

            // AddOneShaderKeywordFilter("spi/Universal Render Pipeline/Lit",
            //     new[] { KEYWORD_LIGHT_COOKIES, KEYWORD_REFLECTION_PROBE_BLENDING });
            // AddOneShaderKeywordFilter("spi/Universal Render Pipeline/Terrain/Lit",
            //     new[] { KEYWORD_LIGHT_COOKIES, KEYWORD_REFLECTION_PROBE_BLENDING });
            // AddOneShaderKeywordFilter("spi/Universal Render Pipeline/Particles/Lit",
            //     new[] { KEYWORD_LIGHT_COOKIES, KEYWORD_REFLECTION_PROBE_BLENDING });
            // AddOneShaderKeywordFilter("spi/Universal Render Pipeline/Nature/SpeedTree7",
            //     new[] { KEYWORD_LIGHT_COOKIES, KEYWORD_REFLECTION_PROBE_BLENDING });
            // AddOneShaderKeywordFilter("spi/GrassMesh",
            //     new[] { KEYWORD_LIGHT_COOKIES, KEYWORD_REFLECTION_PROBE_BLENDING });
            // AddOneShaderKeywordFilter("spi/Legacy/Rings",
            //     new[] { KEYWORD_LIGHT_COOKIES, KEYWORD_REFLECTION_PROBE_BLENDING });
            // AddOneShaderKeywordFilter("spi/Scan",
            //     new[] { KEYWORD_LIGHT_COOKIES, KEYWORD_REFLECTION_PROBE_BLENDING });
            // AddOneShaderKeywordFilter("spi/Water",
            //     new[] { KEYWORD_LIGHT_COOKIES, KEYWORD_REFLECTION_PROBE_BLENDING });
            // AddOneShaderKeywordFilter("spi/WaterFall",
            //     new[] { KEYWORD_LIGHT_COOKIES, KEYWORD_REFLECTION_PROBE_BLENDING });
            //     AddOneShaderKeywordFilter("spi/Universal Render Pipeline/Autodesk Interactive/AutodeskInteractive",
            //         new[] { KEYWORD_LIGHT_COOKIES, KEYWORD_REFLECTION_PROBE_BLENDING });
        }

        private void AddOneShaderKeywordFilter(string name, string[] keywords)
        {
            List<ShaderKeyword> shaderKeywords = new List<ShaderKeyword>();
            for (int i = 0; i < keywords.Length; i++)
            {
                shaderKeywords.Add(new ShaderKeyword(keywords[i]));
            }

            _shaderFilterDic.Add(name, shaderKeywords);
        }

        #endregion

        #endregion


        public int callbackOrder
        {
            get { return 100; }
        }


        //输出变体报告
        public static void printReport()
        {
            string outputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            string outputDirectory = $"{outputRoot}/{EditorUserBuildSettings.activeBuildTarget}/{PathUtil.YOOASSET_PACKAGE_NAME}/{BuildAppInfo.PackageVersion}";
            string fileName = $"ShaderReport_{PathUtil.YOOASSET_PACKAGE_NAME}_{BuildAppInfo.PackageVersion}.txt";

            int totalCount = 0;
            Dictionary<string, int> keywordCountDic = new Dictionary<string, int>();
            foreach (ShaderBuildRecorder shaderBuildRecorder in recorderDic.Values)
            {
                totalCount += shaderBuildRecorder.count;
                foreach (string keyword in shaderBuildRecorder.keywords)
                {
                    keywordCountDic[keyword] = keywordCountDic.ContainsKey(keyword) ? keywordCountDic[keyword] + 1 : 1;
                }
            }

            //将recorderDic保存成List后进行排序
            List<ShaderBuildRecorder> shaderBuildRecorderList = recorderDic.Values.ToList();
            shaderBuildRecorderList.Sort((pair1, pair2) => { return pair2.count.CompareTo(pair1.count); });


            StringBuilder sb = new StringBuilder();
            foreach (ShaderBuildRecorder shaderBuildRecorder in shaderBuildRecorderList)
            {
                sb.Append($"shaderName:{shaderBuildRecorder.shaderName}\ncount:{shaderBuildRecorder.count}\ncountPercent:{1.0f * shaderBuildRecorder.count / totalCount * 100}%\nkeywords:{shaderBuildRecorder.keywords.Count}\n");
                sb.Append($"{string.Join(',', shaderBuildRecorder.keywords)}\n");
                sb.Append($"\n");
            }

            sb.Append($"Keyword Use:\n");
            //将keywordCountDic保存成List后进行排序
            List<KeyValuePair<string, int>> keywordCountList = keywordCountDic.ToList();
            keywordCountList.Sort((pair1, pair2) => { return pair2.Value.CompareTo(pair1.Value); });

            foreach (KeyValuePair<string, int> keyValuePair in keywordCountList)
            {
                sb.Append(keyValuePair.Key + ":" + keyValuePair.Value + "\n");
            }

            string content = sb.ToString();
            JQFileUtil.SaveFile(content, $"{outputDirectory}/{fileName}");
            Debug.Log($"ShaderReport:{outputDirectory}/{fileName}");
        }

        public static void ClearShaderVariantCount()
        {
            recorderDic.Clear();
        }

        static Dictionary<string, ShaderBuildRecorder> recorderDic = new Dictionary<string, ShaderBuildRecorder>();

        public class ShaderBuildRecorder
        {
            public string shaderName;
            public int count;
            public List<string> keywords = new List<string>();

            public ShaderBuildRecorder(string shaderName)
            {
                this.shaderName = shaderName;
            }


            public void addCount(int count)
            {
                this.count += count;
            }

            public void addKeyWord(string keyWord)
            {
                if (keywords.Contains(keyWord)) return;
                keywords.Add(keyWord);
            }
        }

        private static ShaderBuildRecorder getRecorder(string shaderName)
        {
            if (recorderDic.ContainsKey(shaderName))
            {
                return recorderDic[shaderName];
            }

            ShaderBuildRecorder shaderBuildRecorder = new ShaderBuildRecorder(shaderName);
            recorderDic.Add(shaderName, shaderBuildRecorder);
            return shaderBuildRecorder;
        }

        private bool isNeedFilter(Shader shader, ShaderCompilerData shaderCompilerData)
        {
            initShaderFilterDic();
            //检测全局Keyword黑名单
            for (int j = 0; j < filterShaderKeywordArr.Length; j++)
            {
                ShaderKeyword shaderKeyword = filterShaderKeywordArr[j];
                if (shaderCompilerData.shaderKeywordSet.IsEnabled(shaderKeyword))
                {
                    return true;
                }
            }

            //检测指定shader的Keyword黑名单
            List<ShaderKeyword> shaderKeywords = null;
            _shaderFilterDic.TryGetValue(shader.name, out shaderKeywords);
            if (shaderKeywords != null)
            {
                for (int j = 0; j < shaderKeywords.Count; j++)
                {
                    ShaderKeyword shaderKeyword = shaderKeywords[j];
                    if (shaderCompilerData.shaderKeywordSet.IsEnabled(shaderKeyword))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            if (data.Count == 0) return;
            //检测全局Shader黑名单
            if (filterShaderArr.Contains(shader.name))
            {
                data.Clear();
                return;
            }

            for (int i = data.Count - 1; i >= 0; i--)
            {
                ShaderCompilerData shaderCompilerData = data[i];
                if (isNeedFilter(shader, shaderCompilerData))
                {
                    data.RemoveAt(i);
                }
            }


            ShaderBuildRecorder shaderBuildRecorder = getRecorder(shader.name);
            shaderBuildRecorder.addCount(data.Count);
            // string log = $"Shader 变体编译:{shader.name} ({snippet.passName}) ({snippet.shaderType}) - 变体数量 = {data.Count}, 总数量 = {recorderDic[shader.name]}\n";

            for (int i = 0; i < data.Count; i++)
            {
                ShaderCompilerData shaderCompilerData = data[i];
                ShaderKeyword[] shaderKeywords = shaderCompilerData.shaderKeywordSet.GetShaderKeywords();
                string keywords = "";
                for (int j = 0; j < shaderKeywords.Length; j++)
                {
                    shaderBuildRecorder.addKeyWord(shaderKeywords[j].name);
                }
            }
        }
    }
}