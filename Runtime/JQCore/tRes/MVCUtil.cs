using System.IO;
using JQCore.tFileSystem;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using System;
using System.Collections.Generic;
using System.Text;
using JQCore.tUtil;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace JQCore.tRes
{
    public static class MVCUtil
    {
        public static void setAllSubObjectOrder(GameObject viewGo, int sortingOrder, bool isAdd, GameObject searchGo = null)
        {
            searchGo = searchGo == null ? viewGo : searchGo;

            setSortingOrderCanvas(searchGo, sortingOrder, isAdd);

            setSortingOrder<ParticleSystemRenderer>(searchGo, sortingOrder, isAdd);
            setSortingOrder<SpriteRenderer>(searchGo, sortingOrder, isAdd);
            setSortingOrder<MeshRenderer>(searchGo, sortingOrder, isAdd);
            setSortingOrder<SkinnedMeshRenderer>(searchGo, sortingOrder, isAdd);
            setSortingOrder<TrailRenderer>(searchGo, sortingOrder, isAdd);

            setSortingOrderSortingGroup(searchGo, sortingOrder, isAdd);
        }

        public static void setSortingOrder<T>(GameObject searchGo, int sortingOrder, bool isAdd) where T : Renderer
        {
            if (searchGo == null) return;
            T[] componentsInChildren = searchGo.GetComponentsInChildren<T>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                T renderer = componentsInChildren[i];
                if (isAdd)
                {
                    renderer.sortingOrder += sortingOrder;
                }
                else
                {
                    renderer.sortingOrder = sortingOrder;
                }
            }
        }

        public static void setSortingOrderSortingGroup(GameObject searchGo, int sortingOrder, bool isAdd)
        {
            if (searchGo == null) return;
            SortingGroup[] componentsInChildren = searchGo.GetComponentsInChildren<SortingGroup>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                SortingGroup canvas = componentsInChildren[i];
                if (isAdd)
                {
                    canvas.sortingOrder += sortingOrder;
                }
                else
                {
                    canvas.sortingOrder = sortingOrder;
                }
            }
        }

        public static void setSortingOrderCanvas(GameObject searchGo, int sortingOrder, bool isAdd)
        {
            if (searchGo == null) return;
            Canvas[] componentsInChildren = searchGo.GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Canvas canvas = componentsInChildren[i];
                if (isAdd)
                {
                    canvas.sortingOrder += sortingOrder;
                }
                else
                {
                    canvas.sortingOrder = sortingOrder;
                }
            }
        }


#if UNITY_EDITOR
        public struct MVCAttrInfo
        {
            public string attrName;
            public string AttrName;
            public string attrTypeStr;
            public string param1;
            public string param2;
            public string param3;
            public string param4;
        }

        private static List<MVCAttrInfo> buildMvcAttrInfos(BindObjLib bindObjLib)
        {
            List<MVCAttrInfo> list = new();
            for (int i = 0; i < bindObjLib.objs.Length; i++)
            {
                BindObjLib.BindObj bindObj = bindObjLib.objs[i];
                string name = bindObj.key;

                string firstWord = name.Substring(0, 1);
                string otherWord = name.Substring(1, name.Length - 1);
                string attrName = $"{firstWord.ToLower()}{otherWord}";
                string AttrName = $"{firstWord.ToUpper()}{otherWord}";
                string attrTypeStr = bindObj.obj.GetType().Name;
                string param1 = bindObj.param1;
                string param2 = bindObj.param2;
                string param3 = bindObj.param3;
                string param4 = bindObj.param4;
                list.Add(new MVCAttrInfo()
                {
                    attrName = attrName,
                    AttrName = AttrName,
                    attrTypeStr = attrTypeStr,
                    param1 = param1,
                    param2 = param2,
                    param3 = param3,
                    param4 = param4,
                });
            }

            return list;
        }

        private const string ROOTVIEW_TEMPLATE = "Assets/Editor/JQEditor/MVC/ViewTemplate.txt";
        private const string ROOTVIEW_TEMPLATE_EXTRA = "Assets/Editor/JQEditor/MVC/ExtraViewTemplate.txt";
        private const string SUBVIEW_TEMPLATE = "Assets/Editor/JQEditor/MVC/SubViewTemplate.txt";
        private const string SUBVIEW_TEMPLATE_EXTRA = "Assets/Editor/JQEditor/MVC/ExtraSubViewTemplate.txt";

        private static string _templatePath;
        private static string _extraTemplatePath;

        public static void CreateViewExtraCls(BindObjLib bindObjLib, bool isNew, bool isSubView, bool justPrint = false)
        {
            if (isSubView)
            {
                _templatePath = SUBVIEW_TEMPLATE;
                _extraTemplatePath = SUBVIEW_TEMPLATE_EXTRA;
            }
            else
            {
                _templatePath = ROOTVIEW_TEMPLATE;
                _extraTemplatePath = ROOTVIEW_TEMPLATE_EXTRA;
            }

            TextAsset textAsset =
                AssetDatabase.LoadAssetAtPath<TextAsset>(_extraTemplatePath);
            string templateStr = textAsset.ToString();

            List<MVCAttrInfo> mvcAttrInfos = buildMvcAttrInfos(bindObjLib);
            string moduleName = "Module";
            string viewClassName = null;
            string eViewName = null;
            if (isSubView)
            {
                string rootViewName = bindObjLib.transform.parent.GetComponentInParent<BindObjLib>().gameObject.name;
                moduleName = rootViewName.Replace("View", "");
                string subViewName = bindObjLib.gameObject.name;
                string firstWord = subViewName.Substring(0, 1);
                string otherWord = subViewName.Substring(1, subViewName.Length - 1);
                viewClassName = $"{moduleName}{firstWord.ToUpper()}{otherWord}View";
            }
            else
            {
                viewClassName = bindObjLib.gameObject.name;
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null)
                {
                    JQFileUtil.getParentFolderPath(prefabStage.assetPath);
                    moduleName = JQFileUtil.getParentFolderPath(prefabStage.assetPath, false);
                }
                else
                {
                    moduleName = viewClassName.Replace("View", "").Replace("Item", "");
                }
            }

            string viewAttr = getViewAttr(mvcAttrInfos);
            string onBindAttr = getOnBindAttr(mvcAttrInfos);
            string onUnBindAttr = getOnUnBindAttr(mvcAttrInfos);
            string eventFunc = getEventFunc(mvcAttrInfos, true);
            templateStr = templateStr.Replace("$viewClassName$", viewClassName);
            templateStr = templateStr.Replace("$viewAttr$", viewAttr);
            templateStr = templateStr.Replace("$onBindAttr$", onBindAttr);
            templateStr = templateStr.Replace("$onUnBindAttr$", onUnBindAttr);
            templateStr = templateStr.Replace("$eventFunc$", eventFunc);

            if (justPrint)
            {
                // templateStr = templateStr.Replace("override ", "");
                templateStr = templateStr.Replace("AddBtnListener", "ViewUtil.AddBtnListener");
                Debug.Log(templateStr);
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(templateStr);

            string folder = Application.dataPath + $"\\Scripts\\JQGame\\MVC\\Generate\\{moduleName}\\";
            JQFileUtil.CreateDirectory(folder);
            string xxViewExtraCsFilePath = folder + $"{viewClassName}Extra.cs";
            JQFileUtil.SaveFile(bytes, xxViewExtraCsFilePath);
            Debug.Log($"生成代码完成：{xxViewExtraCsFilePath}");

            if (isNew)
            {
                CreateViewCls(mvcAttrInfos, viewClassName, moduleName);
            }

            AssetDatabase.Refresh();
        }

        private static void CreateViewCls(List<MVCAttrInfo> mvcAttrInfos, string viewClassName, string moudleName)
        {
            string folder = Application.dataPath + $"\\Scripts\\JQGame\\MVC\\Module\\{moudleName}\\";
            string xxViewCsFilePath = folder + $"{viewClassName}.cs";
            if (File.Exists(xxViewCsFilePath))
            {
                return; //已经生成过了就不再生成了
            }

            TextAsset textAsset =
                AssetDatabase.LoadAssetAtPath<TextAsset>(_templatePath);
            string templateStr = textAsset.ToString();

            string eventFunc = getEventFunc(mvcAttrInfos, false);
            string eViewName = viewClassName.Replace("View", "");
            templateStr = templateStr.Replace("$viewClassName$", viewClassName);
            templateStr = templateStr.Replace("$moudleName$", moudleName);
            templateStr = templateStr.Replace("$eventFunc$", eventFunc);
            templateStr = templateStr.Replace("$eViewName$", eViewName);
            byte[] bytes = Encoding.UTF8.GetBytes(templateStr);

            JQFileUtil.CreateDirectory(folder);
            JQFileUtil.SaveFile(bytes, xxViewCsFilePath);
            Debug.Log($"生成代码完成：{xxViewCsFilePath}");
        }


        private static string getViewAttr(List<MVCAttrInfo> mvcAttrInfos)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < mvcAttrInfos.Count; i++)
            {
                MVCAttrInfo mvcAttrInfo = mvcAttrInfos[i];
                stringBuilder.AppendLine($"        private {mvcAttrInfo.attrTypeStr} _{mvcAttrInfo.attrName} = null;");
            }

            return stringBuilder.ToString();
        }

        private static string getOnBindAttr(List<MVCAttrInfo> mvcAttrInfos)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < mvcAttrInfos.Count; i++)
            {
                MVCAttrInfo info = mvcAttrInfos[i];
                stringBuilder.AppendLine(
                    $"            _{info.attrName} = ({info.attrTypeStr})_bindObjLib.GetObjByKey(\"{info.attrName}\");");
                if (info.attrTypeStr == "Button")
                {
                    //按钮点击延时
                    float secondClickTime = 0.1f;
                    if (!string.IsNullOrEmpty(info.param1))
                    {
                        float.TryParse(info.param1, out secondClickTime);
                    }

                    //按钮音效
                    string sound = "0";
                    if (!string.IsNullOrEmpty(info.param2))
                    {
                        sound = info.param2;
                    }

                    stringBuilder.AppendLine(
                        $"            AddBtnListener(_{info.attrName}, On{info.AttrName}Click, {secondClickTime}f, \"{sound}\");");
                }

                if (info.attrTypeStr == "TMP_InputField")
                {
                    stringBuilder.AppendLine(
                        $"            AddInputListener(_{info.attrName}, On{info.AttrName}ValueChanged);");
                }
            }

            return stringBuilder.ToString();
        }

        private static string getOnUnBindAttr(List<MVCAttrInfo> mvcAttrInfos)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < mvcAttrInfos.Count; i++)
            {
                MVCAttrInfo info = mvcAttrInfos[i];
                if (info.attrTypeStr == "Button")
                {
                    stringBuilder.AppendLine($"            _{info.attrName}.onClick.RemoveAllListeners();");
                }

                if (info.attrTypeStr == "TMP_InputField")
                {
                    stringBuilder.AppendLine($"            _{info.attrName}.onValueChanged.RemoveAllListeners();");
                }

                stringBuilder.AppendLine($"            _{info.attrName} = null;");
            }

            return stringBuilder.ToString();
        }

        private static string getEventFunc(List<MVCAttrInfo> mvcAttrInfos, bool annotation)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < mvcAttrInfos.Count; i++)
            {
                MVCAttrInfo info = mvcAttrInfos[i];
                if (info.attrTypeStr == "Button")
                {
                    if (annotation)
                    {
                        stringBuilder.AppendLine($"        // private void On{info.AttrName}Click()");
                        stringBuilder.AppendLine("        // {");
                        stringBuilder.AppendLine("        //     throw new System.NotImplementedException();");
                        stringBuilder.AppendLine("        // }");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"         private void On{info.AttrName}Click()");
                        stringBuilder.AppendLine("         {");
                        stringBuilder.AppendLine("             throw new System.NotImplementedException();");
                        stringBuilder.AppendLine("         }");
                    }
                }

                if (info.attrTypeStr == "TMP_InputField")
                {
                    if (annotation)
                    {
                        stringBuilder.AppendLine($"        // private void On{info.AttrName}ValueChanged(string value)");
                        stringBuilder.AppendLine("        // {");
                        stringBuilder.AppendLine("        //     throw new System.NotImplementedException();");
                        stringBuilder.AppendLine("        // }");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"         private void On{info.AttrName}ValueChanged(string value)");
                        stringBuilder.AppendLine("         {");
                        stringBuilder.AppendLine("             throw new System.NotImplementedException();");
                        stringBuilder.AppendLine("         }");
                    }
                }
            }

            return stringBuilder.ToString();
        }
#endif
    }
}