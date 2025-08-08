using System;
using System.Collections.Generic;
using System.Reflection;
using JQEditor.Check;
using JQEditor.Check.tCheckApp;
using JQCore.tSingleton;
using UnityEditor.PackageManager;

namespace JQEditor.Check.tCheckApp
{
    public class CheckApp : JQSingleton<CheckApp>
    {
        private List<CheckAppGroupInfo> _list = new List<CheckAppGroupInfo>();

        public List<CheckAppGroupInfo> List => _list;

        public override void Dispose()
        {
            _list.Clear();
        }

        public CheckApp()
        {
            var customTypes = JQEditorTool.GetAssignableTypes(typeof(ICheckApp));
            foreach (var type in customTypes)
            {
                ICheckApp iCheckApp = (ICheckApp)Activator.CreateInstance(type);
                iCheckApp.AddGroup(_list);
            }


            CheckAppGroupInfo sceneGroup = new CheckAppGroupInfo("场景");
            AddInfo(sceneGroup, "场景层级处理", CheckSceneTools.setOrder);
            AddInfo(sceneGroup, "查找引用丢失-场景", CheckGlobalTools.CheckSceneMissingReferences);
            AddInfo(sceneGroup, "插件Materials替换为项目Materials-场景", CheckSceneTools.checkUsePackageMaterials);
            _list.Add(sceneGroup);

            CheckAppGroupInfo shaderGroup = new CheckAppGroupInfo("Shader");
            AddInfo(shaderGroup, "收集的Shader是否已登记，并设置buildInShader", CheckShaderTools.checkShaderRegisterFromSVC);
            AddInfo(shaderGroup, "插件Shader替换为项目Shader", CheckShaderTools.replaceShaderFromPackageToProject);
            AddInfo(shaderGroup, "打印shader变体数量", CheckShaderTools.GetAllShaderVariantCount);
            AddInfo(shaderGroup, "打包资源引用到插件Mat检查", CheckShaderTools.checkAllAssetUsePackageMat);
            // AddInfo(shaderGroup, "检查Shader是否已登记", CheckEffectTools.checkShaders);
            _list.Add(shaderGroup);


            CheckAppGroupInfo effectGroup = new CheckAppGroupInfo("特效");
//            AddInfo(CheckGroup.特效, "IrfEffect脚本检查", CheckEffectTools.checkEffect);
            AddInfo(effectGroup, "【×】粒子特效性能优化", CheckEffectTools.CheckParticleEffect);
//            AddInfo(effectGroup, "【×】Particle/xxxx -> Mobile/Particle/xxxx", CheckEffectTools.ChangeShader);
            AddInfo(effectGroup, "查找引用丢失-特效", CheckGlobalTools.CheckEffectMissingReferences);
            AddInfo(effectGroup, "特效动画检查", CheckEffectTools.handleAnimator);
            AddInfo(effectGroup, "插件Materials替换为项目Materials-特效", CheckEffectTools.checkUsePackageMaterials);
            _list.Add(effectGroup);

            //            AddInfo(CheckGroup.模型, "{Monster}Animator设置默认State和Loop", CheckModelTools.handleAnimator);
            //            AddInfo(CheckGroup.模型, "引用丢失查找", CheckModelTools.checkModel);
            //            AddInfo(CheckGroup.模型, "All", CheckModelTools.checkAll);

            CheckAppGroupInfo modelGroup = new CheckAppGroupInfo("模型");
            AddInfo(modelGroup, "插件Materials替换为项目Materials-模型", CheckModelTools.checkUsePackageMaterials);
            AddInfo(modelGroup, "移除模型MeshCollider组件", CheckModelTools.removeMeshCollider);
            _list.Add(modelGroup);


            CheckAppGroupInfo commonGroup = new CheckAppGroupInfo("通用图集");
            AddInfo(commonGroup, "图片集合到Prefab", CheckAltasTools.PutIconsToPrefab);
            AddInfo(commonGroup, "图标设置为buttom锚点", CheckAltasTools.PutIconsButtomAnchor);
            _list.Add(commonGroup);
            //            AddInfo(CheckGroup.通用图集, "道具图标是否存在(window)", CheckAltasTools.CheckGoodsData);
            //            AddInfo(CheckGroup.通用图集, "过大图集显示(需打好图集)", CheckAltasTools.CheckSpritePacker);

            //            AddInfo(CheckGroup.数据表, "检查AI掉落和use_actions参数合法性", CheckDataTools.checkDropData);

            //            AddInfo(CheckGroup.AI, "查找怪物表没用到的AI文件", CheckAiTools.ShowUusedAi);

            CheckAppGroupInfo soundGroup = new CheckAppGroupInfo("音频");
            AddInfo(soundGroup, "音频集合到Prefab", CheckSoundTools.PutSoundToPrefab);
            _list.Add(soundGroup);

            CheckAppGroupInfo uiGroup = new CheckAppGroupInfo("UI");

            // AddInfo(uiGroup, "xxxxx", CheckUITools.tempDo);

#if TextMeshPro
            AddInfo(uiGroup, "字体图集优化", CheckUITools.updateFontTexture);
            AddInfo(uiGroup, "移除字体多余图集", CheckUITools.removeFontExtraTexture);
#endif

            AddInfo(uiGroup, "UI图片整理", CheckUITools.filterCommonPic); //√
            AddInfo(uiGroup, "UI图片整理(处理大图)", CheckUITools.filterCommonPic2); //√
//            AddInfo(uiGroup, "Image组件优化", CheckUITools.checkAllImage);//√
            AddInfo(uiGroup, "Button组件优化", CheckUITools.checkAllButton); //√
            AddInfo(uiGroup, "Text组件优化", CheckUITools.checkAllText); //√
#if TextMeshPro
            AddInfo(uiGroup, "TMP->Text", CheckUITools.convertTextFromTMP); //√
#endif
            AddInfo(uiGroup, "合并战斗UI图片", CheckUITools.updateBattleImg); //√
            AddInfo(uiGroup, "关联资源越界检查", CheckUITools.checkDependenceRes); //√
            AddInfo(uiGroup, "查找引用丢失-UI", CheckGlobalTools.CheckUIMissingReferences);
            AddInfo(uiGroup, "动态图关联还原", CheckUITools.linkDynIamge);
            AddInfo(uiGroup, "动态图关联断开", CheckUITools.unlinkDynIamge);
            AddInfo(uiGroup, "空Image设置默认值", CheckUITools.checkEmptyImage);
            AddInfo(uiGroup, "Main界面专用资源替换", CheckUITools.checkMainViewImage);
            AddInfo(uiGroup, "WX-InputField转换", CheckUITools.changeToUGUIInputField);
            AddInfo(uiGroup, "界面结构转换", CheckUITools.changeViewHierarchy);

            // AddInfo(uiGroup, "Test", CheckUITools.DynToStatic);
            _list.Add(uiGroup);


            CheckAppGroupInfo globalGroup = new CheckAppGroupInfo("全局检查");
            AddInfo(globalGroup, "【×】无用组件检查", CheckGlobalTools.CheckUnuseCompnent);
            AddInfo(globalGroup, "无用资源检查", CheckGlobalTools.CheckUnuseRes);
            AddInfo(globalGroup, "移除所有AB-Name", CheckGlobalTools.RemoveABName);
//            AddInfo(globalGroup, "检查Canvas", CheckGlobalTools.CheckRootCanvas);

            //            AddInfo(CheckGroup.全局检查, "引用资源未整理分析", CheckGlobalTools.CheckTexture2DFrom);
            AddInfo(globalGroup, "查找引用丢失", CheckGlobalTools.CheckMissingReferences);
            AddInfo(globalGroup, "检查missCompnent(window)", CheckGlobalTools.SearchMissComponents);
            AddInfo(globalGroup, "图片格式检查", CheckGlobalTools.CheckImgFormat);
            AddInfo(globalGroup, "打印Art使用到的Dll", CheckGlobalTools.LogArtUseDll);
            AddInfo(globalGroup, "生成link.xml", CheckGlobalTools.BuildLinkXml);
            // AddInfo(globalGroup, "清空所有AB-Name", CheckGlobalTools.clearAllABName); //√
            AddInfo(globalGroup, "打印所有Unity类", CheckGlobalTools.showAllU3DClass); //√
            AddInfo(globalGroup, "团结转Unity-收集", CheckGlobalTools.fixGUIDCollect); //√
            AddInfo(globalGroup, "团结转Unity-处理", CheckGlobalTools.fixGUIDExecute); //√
            AddInfo(globalGroup, "打印错误码", CheckGlobalTools.PrintErrorCode); //√
            AddInfo(globalGroup, "批量重命名", CheckGlobalTools.RenameAssets); //√
            AddInfo(globalGroup, "清除存档", CheckGlobalTools.DeleteSaves); //√
            AddInfo(globalGroup, "截图", CheckGlobalTools.CaptureScreenshot); //√
            AddInfo(globalGroup, "软著代码搜集", CheckGlobalTools.CollectCode); //√
            AddInfo(globalGroup, "检查挂载脚本", CheckGlobalTools.FindUnityClass); //√

            //            AddInfo(CheckGroup.全局检查, "AI节点和MapConfiger配置参数检查", CheckGlobalTools.CheckAIAndMapRes);
            //            AddInfo(CheckGroup.全局检查, "AudioSource检查", CheckGlobalTools.checkSound);
            //            AddInfo(globalGroup, "All", CheckGlobalTools.checkAll);
            _list.Add(globalGroup);
        }

        public static void AddInfo(CheckAppGroupInfo group, string name, Action<string, Action> action)
        {
            CheckAppInfo checkAppInfo = new CheckAppInfo(group, name, action);
            group.AddInfo(checkAppInfo);
        }
    }
}