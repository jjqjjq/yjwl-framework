using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JQCore.tFileSystem;
using JQFramework.tUGUI;
using JQCore.tUtil;
using JQEditor.Build;
using JQEditor.Check.tCheckTexture;
using JQCore.DynamicTexture;
using JQCore.tRes;
using JQFramework.JQFramework.tUGUI;
#if TextMeshPro
using TMPro;
using TMPro.EditorUtilities;
#endif
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace JQEditor.Check
{
    public static class CheckUITools
    {
        private static CheckTextureTool _checkTextureTool = new CheckTextureTool();

        private static Sprite _commonUnknownSprite;
        private static Sprite _mainUnknownSprite;

        private static Sprite commonUnknownSprite
        {
            get
            {
                if (_commonUnknownSprite == null)
                {
                    _commonUnknownSprite =
                        AssetDatabase.LoadAssetAtPath($"Assets/{PathUtil.RES_FOLDER}/UI/CommonIcon/unknown.png",
                            typeof(Sprite)) as Sprite;
                }

                return _commonUnknownSprite;
            }
        }

        private static Sprite mainUnknownSprite
        {
            get
            {
                if (_mainUnknownSprite == null)
                {
                    _mainUnknownSprite =
                        AssetDatabase.LoadAssetAtPath($"Assets/{PathUtil.RES_FOLDER}/UI/Main/main_unknown.png",
                            typeof(Sprite)) as Sprite;
                }

                return _mainUnknownSprite;
            }
        }

        private static void checkSpritePath(string spritePath)
        {
            if (!string.IsNullOrEmpty(spritePath))
            {
                if (spritePath.Contains($"Assets/{PathUtil.RES_FOLDER}/"))
                {
                    spritePath = spritePath.Replace($"Assets/{PathUtil.RES_FOLDER}/", "");
                }

                string path = $"Assets/{PathUtil.RES_FOLDER}/" + spritePath;
                Sprite readySprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
                if (readySprite == null)
                {
                    string fileName = Path.GetFileName(spritePath);
                    //关联的图片不存在，从UnCheck目录找，找到则移动到对应目录下
                    string picPath = $"Assets/{PathUtil.RES_FOLDER}/UI/UnCheck/" + fileName;
                    Debug.Log($"移动图片：{picPath} =========> {path}");
                    string error = AssetDatabase.MoveAsset(picPath, path);
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError(error);
                    }
                }
            }
        }

        public static void DynToStatic(string name, Action endAction)
        {
            CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views", dynToStatic, null, endAction);
        }

        private static bool dynToStatic(string assetPath, GameObject cloneGo, object obj1)
        {
            // bool change = false;
            DynamicImage[] dynamicImages = cloneGo.GetComponentsInChildren<DynamicImage>(true);
            foreach (DynamicImage dynamicImage in dynamicImages)
            {
                checkSpritePath(dynamicImage.spritePath);
            }

            DynamicSpriteRenderer[] dynamicSpriteRenderers = cloneGo.GetComponentsInChildren<DynamicSpriteRenderer>(true);
            foreach (DynamicSpriteRenderer dynamicSpriteRenderer in dynamicSpriteRenderers)
            {
                checkSpritePath(dynamicSpriteRenderer.spritePath);
            }

            return true;
        }


        public static void unlinkDynIamge(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views", unlinkAllDynIamge, null, endAction);
        }

        private static bool unlinkAllDynIamge(string assetPath, GameObject cloneGo, object obj1)
        {
            if (assetPath.Contains("LaucherLoading")) return false;
            bool change = false;
            DynamicImage[] dynamicImages = cloneGo.GetComponentsInChildren<DynamicImage>(true);
            foreach (DynamicImage dynamicImage in dynamicImages)
            {
                bool success = dynamicImage.removeSprite();

                if (success)
                {
                    change = true;
                }
            }

            DynamicSpriteRenderer[] dynamicSpriteRenderers = cloneGo.GetComponentsInChildren<DynamicSpriteRenderer>(true);
            foreach (DynamicSpriteRenderer dynamicSpriteRenderer in dynamicSpriteRenderers)
            {
                bool success = dynamicSpriteRenderer.removeSprite();

                if (success)
                {
                    change = true;
                }
            }

            return change;
        }

        public static void linkDynIamge(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views", linkAllDynIamge, null, endAction);
        }

        private static bool linkAllDynIamge(string assetPath, GameObject cloneGo, object obj1)
        {
            if (assetPath.Contains("LaucherLoading")) return false;
            bool change = false;
            Debug.Log("linkAllDynIamge:" + assetPath);
            DynamicImage[] dynamicImages = cloneGo.GetComponentsInChildren<DynamicImage>(true);
            foreach (DynamicImage dynamicImage in dynamicImages)
            {
                bool success = dynamicImage.restoreSprite();
                if (success)
                {
                    change = true;
                }
            }

            DynamicSpriteRenderer[] dynamicSpriteRenderers = cloneGo.GetComponentsInChildren<DynamicSpriteRenderer>(true);
            foreach (DynamicSpriteRenderer dynamicSpriteRenderer in dynamicSpriteRenderers)
            {
                bool success = dynamicSpriteRenderer.restoreSprite();
                if (success)
                {
                    change = true;
                }
            }

            return change;
        }


#if TextMeshPro
        public static void updateFontTexture(string name, Action endAction)
        {
            string path = $"Assets/{PathUtil.RES_FOLDER}/Common/Font_TMP";
            CheckCommonTools.Search<TMP_FontAsset>(name, path, optimizeFont, null, endAction, true, ".asset", "");
        }

        public static void removeFontExtraTexture(string name, Action endAction)
        {
            string path = $"Assets/{PathUtil.RES_FOLDER}/Common/Font_TMP";
            CheckCommonTools.Search<TMP_FontAsset>(name, path, removeFontTexture, null, endAction, true, ".asset", "");
        }

//
//         /// <summary>
//         /// 处理字体文件，贴图移动到内部
//         /// 如果把贴图塞回字体文件内部，无法取消掉可读写属性
//         /// </summary>
//         /// <param name="fontAssetPath">字体文件Assets路径</param>
//         /// <param name="texturePath">目标字体贴图</param>
//         /// <param name="isDelete">操作完成后是否移除旧的目标贴图</param>
//         public static void MoveInside(string fontAssetPath, string texturePath, bool isDelete = false)
//         {
//             AssetDatabase.Refresh(); //刷新一下，确保新的文件导入到了Unity中
//             InitMatDic();
//             TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);
//             Texture2D tarTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
//             Texture2D copy = TextureClone(tarTexture);
//             copy.name = tarTexture.name;
//             AssetDatabase.AddObjectToAsset(copy, fontAsset);
// //AssetDatabase.SaveAssets();
// //Texture2D inside2D = AssetDatabase.LoadAssetAtPath<Texture2D>(fontAssetPath);
//
//             fontAsset.atlasTextures[0] = copy;
//             SetMat(tarTexture.name, copy);
//             if (isDelete)
//             {
//                 File.Delete(texturePath);
//             }
//
//             AssetDatabase.SaveAssets();
//         }

        public static bool removeFontTexture(string fontAssetPath, TMP_FontAsset fontAsset, object obj1)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fontAssetPath);
            foreach (Object asset in assets)
            {
                if (asset is Texture2D)
                {
                    Texture2D texture = asset as Texture2D;
                    if (!texture.name.EndsWith("Atlas"))
                    {
                        AssetDatabase.RemoveObjectFromAsset(texture); //移除内部资源
                    }
                }
            }

            AssetDatabase.SaveAssets();
            return true;
        }

        public static bool optimizeFont(string fontAssetPath, TMP_FontAsset fontAsset, object obj1)
        {
            string copyUrl = fontAssetPath.Replace(".asset", "_Atlas.png");
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(fontAssetPath);
            if (texture) //如果字体文件的内部存在贴图
            {
                Texture2D tarTexture = TextureClone(texture, copyUrl);
//AssetDatabase.Refresh();//要立马使用新创建的文件，需要刷新一下，确保Unity识别到新文件
                int maxSize = texture.width > texture.height ? texture.width : texture.height;
                fontAsset.material.SetTexture(ShaderUtilities.ID_MainTex, tarTexture);
                SetTextureAttr(copyUrl, maxSize);
                fontAsset.atlasTextures[0] = tarTexture;
                AssetDatabase.RemoveObjectFromAsset(texture); //移除内部资源
                // AssetDatabase.AddObjectToAsset(tarTexture, fontAsset);
                AssetDatabase.SaveAssets();
                return true;
            }
            else
            {
                Debug.LogError("目标字体文件不存在内部贴图---" + fontAssetPath);
                return false;
            }
        }
#endif


        /// <summary>
        /// 克隆贴图
        /// </summary>
        /// <param name="sourceTexture">被克隆贴图</param>
        /// <param name="outFile">空，返回的是未持久化的贴图。非空，返回的是持久化的贴图</param>
        /// <returns>新的贴图对象</returns>
        public static Texture2D TextureClone(Texture2D sourceTexture, string outFile = null)
        {
            Texture2D copy = new Texture2D(sourceTexture.width, sourceTexture.height, sourceTexture.format, false);
            Graphics.CopyTexture(sourceTexture, copy);
            if (outFile == null)
            {
                copy.Apply();
                return copy;
            }
            else
            {
                File.WriteAllBytes(outFile, copy.EncodeToPNG());
                AssetDatabase.ImportAsset(outFile);
                return AssetDatabase.LoadAssetAtPath<Texture2D>(outFile);
            }
        }


        /// <summary>
        /// 设置贴图属性
        /// </summary>
        /// <param name="textureUrl">贴图Assets路径</param>
        public static void SetTextureAttr(string textureUrl, int maxSize)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(textureUrl);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(textureUrl);
            if (importer.isReadable)
            {
                importer.isReadable = false;
            }


            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
            }

            if (!importer.alphaIsTransparency)
            {
                importer.alphaIsTransparency = true;
            }

            TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("Android");
            settings.overridden = true;
            settings.maxTextureSize = maxSize;
            settings.format = TextureImporterFormat.ASTC_6x6;
            importer.SetPlatformTextureSettings(settings);

            TextureImporterPlatformSettings settings2 = importer.GetPlatformTextureSettings("Standalone");
            settings2.overridden = true;
            settings2.maxTextureSize = maxSize;
            settings2.format = TextureImporterFormat.Alpha8;
            importer.SetPlatformTextureSettings(settings2);

            TextureImporterPlatformSettings settings3 = importer.GetPlatformTextureSettings("WeixinMiniGame");
            settings3.overridden = true;
            settings3.maxTextureSize = maxSize;
            settings3.format = TextureImporterFormat.ASTC_6x6;
            importer.SetPlatformTextureSettings(settings3);
            importer.SaveAndReimport();
            AssetDatabase.ImportAsset(textureUrl);
        }


        public static void filterCommonPic(string name, Action endAction)
        {
            string[] extraPath = new string[]
            {
                "Assets/Plugins/Art/Layer Lab/GUI Pro-SuperCasual/ResourcesData/Sprites",
                "Assets/Art/Build",
                "Assets/Art/Effect",
                "Assets/Plugins/Art/CartoonVFX9X/UIEffectCollecion_Megapack/Textures"
            };
            _checkTextureTool.build(name, endAction, false, extraPath);
        }

        public static void filterCommonPic2(string name, Action endAction)
        {
            string[] extraPath = new string[]
            {
                "Assets/Plugins/Art/Layer Lab/GUI Pro-SuperCasual/ResourcesData/Sprites",
                "Assets/Art/Build",
                "Assets/Art/Effect",
                "Assets/Plugins/Art/CartoonVFX9X/UIEffectCollecion_Megapack/Textures"
            };
            _checkTextureTool.build(name, endAction, true, extraPath);
        }

        public static void changeToUGUIInputField(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views", changeInputField, null, endAction);
        }


        private static bool changeInputField(string assetPath, GameObject cloneGo, object obj1)
        {
            int count = 0;
#if TextMeshPro
            // Font font = AssetDatabase.LoadAssetAtPath($"Assets/{PathUtil.RES_FOLDER}/Common/Font/CommonFont.ttf", typeof(Font)) as Font;
            TMP_InputField[] inputFields = cloneGo.GetComponentsInChildren<TMP_InputField>(true);
            foreach (TMP_InputField inputField in inputFields)
            {
                WXTMPInputField wxtmpInputField = inputField.GetComponent<WXTMPInputField>();
                if (wxtmpInputField == null)
                {
                    wxtmpInputField = inputField.gameObject.AddComponent<WXTMPInputField>();
                    wxtmpInputField.input = inputField;
                    count++;
                }
            }
#endif

            InputField[] inputFields2 = cloneGo.GetComponentsInChildren<InputField>(true);
            foreach (InputField inputField in inputFields2)
            {
                WXInputField wxInputField = inputField.GetComponent<WXInputField>();
                if (wxInputField == null)
                {
                    wxInputField = inputField.gameObject.AddComponent<WXInputField>();
                    wxInputField.input = inputField;
                    count++;
                }
            }

            if (count > 0)
            {
                Debug.Log($"微信输入框处理：{assetPath} {count}");
            }

            return count > 0;
        }


        public static void changeViewHierarchy(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views", changeOneViewHierarchy, null, endAction);
        }

        private static bool changeOneViewHierarchy(string assetPath, GameObject cloneGo, object obj1)
        {
            if (!cloneGo.name.EndsWith("View")) return false;
            RectTransform rectTransform = cloneGo.AddMissingComponent<RectTransform>();
            bool hasPanelFunction = false;
            bool hasPanelNoAdapt = false;
            foreach (Transform child in cloneGo.transform)
            {
                if (child.name == "panel_function")
                {
                    hasPanelFunction = true;
                }
                else if (child.name == "panel_no_adapt")
                {
                    hasPanelNoAdapt = true;
                }
            }

            if (!hasPanelNoAdapt)
            {
                GameObject adaptGo = new GameObject("panel_no_adapt");
                adaptGo.transform.SetParent(rectTransform);
                RectTransform adaptRectTrans = adaptGo.AddComponent<RectTransform>();
                adaptRectTrans.localPosition = Vector3.zero;
                adaptRectTrans.localScale = Vector3.one;
                adaptRectTrans.anchorMin = Vector2.zero;
                adaptRectTrans.anchorMax = Vector2.one;
                adaptRectTrans.sizeDelta = Vector2.zero;
            }

            RectTransform funcRectTrans = null;
            if (!hasPanelFunction)
            {
                GameObject funcGo = new GameObject("panel_function");
                funcGo.transform.SetParent(rectTransform);
                funcRectTrans = funcGo.AddComponent<RectTransform>();
                funcRectTrans.localPosition = Vector3.zero;
                funcRectTrans.localScale = Vector3.one;
                funcRectTrans.anchorMin = Vector2.zero;
                funcRectTrans.anchorMax = Vector2.one;
                funcRectTrans.sizeDelta = Vector2.zero;
            }
            else
            {
                funcRectTrans = rectTransform.Find("panel_function").GetComponent<RectTransform>();
            }

            BindObjLib bindObjLib = cloneGo.GetComponent<BindObjLib>();
            if (bindObjLib == null)
            {
                bindObjLib = cloneGo.AddComponent<BindObjLib>();
            }

            if (funcRectTrans != null && !bindObjLib.IsKeyExist(funcRectTrans, "panel_function"))
            {
                BindObjLib.BindObj[] objs = bindObjLib.objs;
                List<BindObjLib.BindObj> bindObjs = new List<BindObjLib.BindObj>(objs);
                bindObjs.Add(new BindObjLib.BindObj() { key = "panel_function", obj = funcRectTrans });
                bindObjLib.objs = bindObjs.ToArray();
            }

            return true;
        }


        public static void checkMainViewImage(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views/Main", setMainViewImage, null, endAction);
        }

        private static bool setMainViewImage(string assetPath, GameObject cloneGo, object obj1)
        {
            bool change = false;
            Image[] images = cloneGo.GetComponentsInChildren<Image>(true);
            Sprite commonHhhdSprite =
                AssetDatabase.LoadAssetAtPath($"Assets/{PathUtil.RES_FOLDER}/UI/CommonIcon/hhhd.png", typeof(Sprite)) as Sprite;
            Sprite mainHhhdSprite =
                AssetDatabase.LoadAssetAtPath($"Assets/{PathUtil.RES_FOLDER}/UI/Main/main_hhhd.png", typeof(Sprite)) as Sprite;
            for (int i = 0; i < images.Length; i++)
            {
                Image image = images[i];
                if (image.sprite == commonHhhdSprite)
                {
                    image.sprite = mainHhhdSprite;
                    change = true;
                }
            }

            return change;
        }

        public static void checkEmptyImage(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views", setEmptyImage, null, endAction);
        }


        private static bool setEmptyImage(string assetPath, GameObject cloneGo, object obj1)
        {
            bool change = false;
            Image[] images = cloneGo.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                Image image = images[i];
                if (image.sprite == null)
                {
                    Debug.Log($"设置空Image默认值 {PathUtil.getFullPath(image.gameObject)} ");
                    if (assetPath.Contains("MainView.prefab"))
                    {
                        image.sprite = mainUnknownSprite;
                    }
                    else
                    {
                        image.sprite = commonUnknownSprite;
                    }

                    change = true;
                }
            }

            return change;
        }


        //[MenuItem("IrfTools/加工流程/取消勾选image图片Raycast Target")]
        //        public static void checkAllImage(string name, Action endAction)
        //        {
        //            //            CheckCommonTools.SearchAndDo($"Assets/{CheckApp.RES_FOLDER}/UI/Views/Equip", replaceTextureLoader, null);
        //            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views", setAllImage, null, endAction);
        //        }


        //        private static bool setAllImage(string assetPath, GameObject cloneGo, object obj1)
        //        {
        //            bool change = false;
        //            Image[] images = cloneGo.GetComponentsInChildren<Image>(true);
        //            for (int i = 0; i < images.Length; i++)
        //            {
        //                Image image = images[i];
        //                bool oldRaycastTarget = image.raycastTarget;
        //                bool newRaycastTarget = hadInteractElemenet(image);//raycastTarget处理
        //                if (oldRaycastTarget != newRaycastTarget)
        //                {
        //                    image.raycastTarget = newRaycastTarget;
        //                    change = true;
        //                }
        //            }
        //            
        //            return change;
        //        }

        //        private static bool hadInteractElemenet(Image image)
        //        {
        //            if (image.name.Contains("_nrt"))
        //            {
        //                return false;
        //            }
        //            if (image.name.Contains("_rt"))
        //            {
        //                return true;
        //            }
        //            if (image.name.ToLower().Contains("click"))
        //            {
        //                return true;
        //            }
        //            if (image.name.Contains("back_mask"))
        //            {
        //                return true;
        //            }
        //            if (image.name.Contains("bg"))
        //            {
        //                return true;
        //            }
        //            if (checkImageBehavior<Button>(image))
        //            {
        //                return true;
        //            }
        //            if (checkImageBehavior<Toggle>(image))
        //            {
        //                return true;
        //            }
        //            if (checkImageBehavior<Slider>(image))
        //            {
        //                return true;
        //            }
        //            ScrollRect[] scrollRects = image.transform.GetComponentsInParent<ScrollRect>(true);
        //            foreach (ScrollRect scrollRect in scrollRects)
        //            {
        //                if (scrollRect.viewport == image.transform)
        //                {
        //                    return true;
        //                }
        //            }
        //            if (checkImageBehavior<JoystickManager>(image))
        //            {
        //                return true;
        //            }
        //            if (checkImageBehavior<EventTrigger>(image, false))
        //            {
        //                return true;
        //            }
        //            return false;
        //        }


        //        private static bool checkImageBehavior<T>(Image image, bool checkParent = true) where T : MonoBehaviour
        //        {
        //            T trigger = image.transform.GetComponent<T>();
        //            if (trigger != null)
        //            {
        //                return true;
        //            }
        //
        //            if (checkParent)
        //            {
        //                T[] triggers = image.transform.GetComponentsInParent<T>(true);
        //                if (triggers.Length > 0)
        //                {
        //                    return true;
        //                }
        //            }
        //            return false;
        //        }


        //[MenuItem("IrfCheck/UI/按钮批处理")]
        public static void checkAllButton(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views", setAllButton, null, endAction);
        }

        private static List<string> iconList = new List<string>
        {
            "icon_sc", "sl_icon_tubiao", "icon_sd", "icon_fszr", "icon_fl", "zjm_icon_dtyl", "icon_wfzd", "icon_zf",
            "zjm_icon_hd", "zjm_icon_cnzt",
            "zjm_icon_cz", "zjm_icon_phb", "zjm_icon_kaifuzhekou", "zjm_icon_xz", "zjm_icon_xymj", "zjm_icon_ghgh",
            "zjm_icon_ryzz", "zjm_icon_shiz", "zjm_icon_yj", "zjm_icon_yuanz", "yw_btn_phb",
            "zjm_btn_VIP", "zjm_icon_zhanb", "zjm_icon_zaixianjiangli", "jz_2", "icon_zb2", "icon_zb", "icon_tj", "icon_bianiqng",
            "icon_cw", "icon_jz", "icon_rw", "icon_bb", "bb_icon_dz",
            "common_icon_ksgj", "common_icon_zs", "icon_sjboss", "icon_rc", "icon_flqd", "icon_sz", "dj_baoxiang9", "icon_xshd",
            "icon_xslb", "icon_yj", "cj_btn_sj", "zjm_icon_sjdt", "zb_icon_bs",
            "icon_zb3", "btn_jiahao", "btn_jianhao", "btn_jiantou", "btn_main_9_2", "btn_sousuo1", "btn_main_tj",
            "common_btn_main", "btn_main_bq", "btn_ts_fx", "btn_ts_fx2", "cb_btn_mb", "common_btn_gw", "common_btn_jiantou",
            "common_btn_shuaxin1", "common_btn_zl", "common_btn_yc2", "common_btn_yc1", "common_btn_xz", "icon_wenhao",
            "jjc_icon_fl", "jjc_icon_ks", "jjc_icon_sd", "js_btn_huifang", "js_btn_hf",
            "jjc_icon_zr", "jjc_icon_zb", "js_btn_tj", "js_btn_tj", "jz_btn_cha", "jz_btn_gou", "yw_btn_cz", "mg_icon_bx",
            "rc_icon_rw01", "kp_btn_ly", "mg_icon_sd", "mg_icon_zskp", "mg_icon_zylb",
            "tyg_btn_yj", "btn_comment", "yw_btn_fd", "zjm_icon_zp", "zjm_icon_map", "btn_get", "btn_active", "btn_infinity_draw",
            "yw_btn_sm", "btn_god_domain", "w_btn_hyd", "btn_week", "yw_btn_phb",
            "btn_challengeOther", "icon_phb", "icon_phbjl", "icon_gxgg", "mdjy_icon_xx", "mdjy_btn_03", "lbzt_btn_02",
            "lbzt_btn_01", "nszt_icon_01", "nszt_icon_02", "nszt_icon_03", "btn_gb01",
            "btn_main3", "btn_main5", "btn_main2", "cz_btn_bg", "btn_main4", "btn_main2_hd2", "btn_main2_hd1", "btn_exit"
        };


        private static bool setAllButton(string assetPath, GameObject res, object obj1)
        {
            Button[] buttons = res.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                Button button = buttons[i];
                if (button.targetGraphic == null)
                {
                    //                    Debug.LogError($"button未设置targetGraphic：{assetPath} - {button.name}");
                    continue;
                }

                Image img = button.targetGraphic.GetComponent<Image>();
                if (img != null)
                {
                    if (iconList.Contains(img.mainTexture.name))
                    {
                        //Debug.LogError("1111111111111");
                        button.transition = Selectable.Transition.None;
                        Animator ani = button.gameObject.AddMissingComponent<Animator>();
                        ani.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                        ani.runtimeAnimatorController =
                            AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                                $"Assets/{PathUtil.RES_FOLDER}/UI/Common/UIButtonAnimaCtrl.controller");
                        button.gameObject.AddMissingComponent<ButtonTween>();
                    }
                }
            }

            return true;
        }

        //        static TMP_FontAsset defaultFont1 =
        //            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"Assets/{PathUtil.RES_FOLDER}/Common/Font_TMP/Title SDF.asset");
        //        static TMP_FontAsset defaultFont2 =
        //            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"Assets/{PathUtil.RES_FOLDER}/Common/Font_TMP/Title SDF BlueOutline.asset");
        //        static TMP_FontAsset defaultFont3 =
        //            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"Assets/{PathUtil.RES_FOLDER}/Common/Font_TMP/Title SDF RedOutline.asset");
        //
        //        public static void checkAllText2(string name, Action endAction)
        //        {
        //            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}", setAllText2, null, endAction);
        //        }
        //        private static void setAllText2(string assetPath, GameObject res, object obj1)
        //        {
        //            TextMeshProUGUI[] texts = res.GetComponentsInChildren<TextMeshProUGUI>(true);
        //            for (int i = 0; i < texts.Length; i++)
        //            {
        //                TextMeshProUGUI text = texts[i];
        //                string oldFontName = text.font.name;
        //                if (text.font == defaultFont1 || text.font == defaultFont2 || text.font == defaultFont3)
        //                {
        //                    continue;
        //                }
        //                HyDebug.Log($"【替换未报备字体-】:{oldFontName}->{defaultFont1.name}   assetPath:{assetPath}     obj:{text.name} ");
        //                text.font = defaultFont1;
        //            }
        //
        //
        //            TextMeshPro[] texts2 = res.GetComponentsInChildren<TextMeshPro>(true);
        //            for (int i = 0; i < texts2.Length; i++)
        //            {
        //                TextMeshPro text = texts2[i];
        //                string oldFontName = text.font.name;
        //                if (text.font == defaultFont1 || text.font == defaultFont2 || text.font == defaultFont3)
        //                {
        //                    continue;
        //                }
        //                HyDebug.Log($"【替换未报备字体-】:{oldFontName}->{defaultFont1.name}   assetPath:{assetPath}     obj:{text.name} ");
        //                text.font = defaultFont1;
        //            }
        //        }

        private static void SetTextureImporter(string subPngPath)
        {
            bool isChange = false;
            TextureImporter ti = AssetImporter.GetAtPath(subPngPath) as TextureImporter;
            if (ti.textureType != TextureImporterType.Sprite)
            {
                ti.textureType = TextureImporterType.Sprite;
                isChange = true;
            }

            if (ti.mipmapEnabled)
            {
                ti.mipmapEnabled = false;
                isChange = true;
            }

            if (ti.isReadable != true)
            {
                ti.isReadable = true;
                isChange = true;
            }

            if (ti.filterMode != FilterMode.Trilinear)
            {
                ti.filterMode = FilterMode.Trilinear;
                isChange = true;
            }

            if (ti.textureCompression != TextureImporterCompression.Uncompressed)
            {
                ti.textureCompression = TextureImporterCompression.Uncompressed;
                isChange = true;
            }

            if (isChange)
            {
                AssetDatabase.ImportAsset(subPngPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            }
        }

        internal class BattleAtlasImgInfo
        {
            public int x;
            public int y;
            public string fileName;

            public BattleAtlasImgInfo(int x, int y, string fileName)
            {
                this.x = x;
                this.y = y;
                this.fileName = fileName;
            }
        }

        public static void updateBattleImg(string name, Action endAction)
        {
#if TextMeshPro
            List<SpriteMetaData> spriteMetaDataList = new List<SpriteMetaData>();
            string texturePath = $"Assets/{PathUtil.RES_FOLDER}/Common/Font_TMP/BattleFont_Num.png";
            int maxWidth = 1024;
            int maxHeight = 1024;
            Texture2D fullTexture = new Texture2D(maxWidth, maxHeight, TextureFormat.RGBA32, false);

            Color[] fullTcolors = fullTexture.GetPixels();

            for (int i = 0; i < fullTcolors.Length; i++)
            {
                fullTcolors[i].a = 0;
                fullTcolors[i].r = 0;
                fullTcolors[i].g = 0;
                fullTcolors[i].b = 0;
            }

            fullTexture.SetPixels(fullTcolors);

            int posX = 0;
            int posY = 0;
            int currLineHeight = 0;
            string[] childs =
                Directory.GetFileSystemEntries($"{Application.dataPath}/{PathUtil.RES_FOLDER}/Common/Font_TMP/Temp/Battle");

            string luaScript = "local class = base(...)\n" + "EBattleAtlas = class\n" + "class.battleAtlasDic = {\n";
            List<BattleAtlasImgInfo> imgScriptList = new List<BattleAtlasImgInfo>();

            for (int i = 0; i < childs.Length; i++)
            {
                string path = childs[i];
                if (!path.EndsWith(".png"))
                {
                    continue;
                }

                string subPngPath = path.Replace(Application.dataPath, "Assets");
                SetTextureImporter(subPngPath);
                string fileName = JQFileUtil.getCurrFolderOrFileName(path, false);

                Texture2D tex2D = AssetDatabase.LoadAssetAtPath(subPngPath, typeof(Texture2D)) as Texture2D;
                Color[] colors = tex2D.GetPixels();
                if (posX + tex2D.width > maxWidth)
                {
                    posX = 0;
                    posY += currLineHeight;
                    currLineHeight = 0;
                }

                int spriteY = maxHeight - posY - tex2D.height;
                fullTexture.SetPixels(posX, spriteY, tex2D.width, tex2D.height, colors);
                SpriteMetaData spriteMetaData = new SpriteMetaData();
                spriteMetaData.name = fileName;
                spriteMetaData.rect = new Rect(posX, spriteY, tex2D.width, tex2D.height);
                spriteMetaDataList.Add(spriteMetaData);

                posX += tex2D.width;
                currLineHeight = Math.Max(currLineHeight, tex2D.height);
                imgScriptList.Add(new BattleAtlasImgInfo(posX, spriteY, fileName));
            }

            imgScriptList = imgScriptList.OrderByDescending(battleAtlasImgInfo => battleAtlasImgInfo.y)
                .ThenBy(battleAtlasImgInfo => battleAtlasImgInfo.x).ToList();

            int pngCount = 0;
            for (int i = 0; i < imgScriptList.Count; i++)
            {
                BattleAtlasImgInfo battleAtlasImgInfo = imgScriptList[i];
                luaScript += $"    {battleAtlasImgInfo.fileName} = {pngCount},\n";
                pngCount++;
            }


            luaScript += "}";

            byte[] full = fullTexture.EncodeToPNG();
            File.WriteAllBytes(texturePath, full);

            AssetDatabase.Refresh();

            texturePath = texturePath.Replace(Application.dataPath, "Assets");
            TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            Texture2D texture2D = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.SaveAndReimport();

            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.spritesheet = spriteMetaDataList.ToArray();
            textureImporter.SaveAndReimport();
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

            Selection.activeObject = texture2D;
            //            AssetDatabase.DeleteAsset(tmpSpriteAssetPath);
            TMP_SpriteAssetMenu.CreateSpriteAsset();

            string spriteAssetPath = $"Assets/{PathUtil.RES_FOLDER}/Common/Font_TMP/BattleFont_Num.asset";
            TMP_SpriteAsset tmpSpriteAsset =
                AssetDatabase.LoadAssetAtPath(spriteAssetPath, typeof(TMP_SpriteAsset)) as TMP_SpriteAsset;
            for (int i = 0; i < tmpSpriteAsset.spriteGlyphTable.Count; i++)
            {
                TMP_SpriteGlyph tmpSpriteGlyph = tmpSpriteAsset.spriteGlyphTable[i];
                GlyphRect glyphRect = tmpSpriteGlyph.glyphRect;
                tmpSpriteGlyph.metrics = new GlyphMetrics(glyphRect.width, glyphRect.height, 0, glyphRect.height,
                    glyphRect.width);
                tmpSpriteAsset.spriteGlyphTable[i] = tmpSpriteGlyph;
            }

            tmpSpriteAsset.UpdateLookupTables();
            EditorUtility.SetDirty(tmpSpriteAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(spriteAssetPath);

            byte[] luaScriptBytes = Encoding.UTF8.GetBytes(luaScript);
            string luaPath = Application.dataPath + $"/{PathUtil.LUA_FOLDER}/global/enum/EBattleAtlas.lua";
            JQFileUtil.SaveFile(luaScriptBytes, luaPath);
#endif
        }

#if TextMeshPro
        public static void convertTextFromTMP(string name, Action endAction)
        {
            Font defaultFont = AssetDatabase.LoadAssetAtPath<Font>($"Assets/{PathUtil.RES_FOLDER}/Common/Font/CommonFont.ttf");
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views", convertTMP2Text, defaultFont,
                endAction);
        }

        private static bool convertTMP2Text(string assetPath, GameObject res, object obj1)
        {
            Font defaultFont = obj1 as Font;
            BindObjLib bindObjLib = res.GetComponent<BindObjLib>();
            Dictionary<Object, BindObjLib.BindObj> bindObjDic = new Dictionary<Object, BindObjLib.BindObj>();
            foreach (BindObjLib.BindObj bindObj in bindObjLib.objs)
            {
                bindObjDic[bindObj.obj] = bindObj;
            }

            bool needSynViewScript = false;

            TextMeshProUGUI[] textMeshProUguis = res.GetComponentsInChildren<TextMeshProUGUI>(true);
            for (int i = 0; i < textMeshProUguis.Length; i++)
            {
                TextMeshProUGUI textMeshProUgui = textMeshProUguis[i];
                bindObjDic.TryGetValue(textMeshProUgui, out BindObjLib.BindObj bindObj);
                Text text = textMeshProUgui.ToText(defaultFont);
                if (text != null)
                {
                    if (bindObj != null)
                    {
                        bindObj.obj = text;
                        needSynViewScript = true;
                    }

                    ImageSolidColorOutline imageSolidColorOutline = text.gameObject.AddMissingComponent<ImageSolidColorOutline>();
                    imageSolidColorOutline.OutlineColor = Color.black;
                    imageSolidColorOutline.OutlineWidth = 3;
                }
            }

            if (needSynViewScript)
            {
                bindObjLib.SynViewScript();
            }

            return true;
        }

        public class TMPAssetEdit
        {
            public TMP_FontAsset defaultFontTMP;
            public Material defaultMaterial;
        }
#endif

        public static void checkAllText(string name, Action endAction)
        {
            Font defaultFont = AssetDatabase.LoadAssetAtPath<Font>($"Assets/{PathUtil.RES_FOLDER}/Common/Font/CommonFont.ttf");
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI", setAllText, defaultFont, endAction);

#if TextMeshPro
            TMPAssetEdit tmpAssetEdit = new TMPAssetEdit()
            {
                defaultFontTMP = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                    $"Assets/{PathUtil.RES_FOLDER}/Common/Font_TMP/CommonFont_TMP.asset"),
                defaultMaterial = AssetDatabase.LoadAssetAtPath<Material>(
                    $"Assets/{PathUtil.RES_FOLDER}/Common/Font_TMP/CommonFont_TMP_Outline0.2_Underlay0.50.0.mat")
            };
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views/Subscribe", setAllTextTMP, tmpAssetEdit,
                endAction);
#endif
        }

        private static List<string> fontNameList = new List<string>(new[] { "CommonFont_TMP", "CommonFont" });

        private static bool setAllText(string assetPath, GameObject res, object obj1)
        {
            Font defaultFont = obj1 as Font;
            Text[] texts = res.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                Text text = texts[i];
                string oldFontName = null;
                if (text.font == null)
                {
                    Debug.Log($"【替换未报备字体-】:{oldFontName}->{defaultFont.name}   assetPath:{assetPath}     obj:{text.name} ");
                    text.font = defaultFont;
                }

                if (!fontNameList.Contains(text.font.name) && !text.font.name.StartsWith("ImgFont_"))
                {
                    oldFontName = text.font.name;
                    Debug.Log($"【替换未报备字体-】:{oldFontName}->{defaultFont.name}   assetPath:{assetPath}     obj:{text.name} ");
                    text.font = defaultFont;
                }
            }

            return true;
        }

#if TextMeshPro
        private static bool setAllTextTMP(string assetPath, GameObject res, object obj1)
        {
            TMPAssetEdit editAsset = obj1 as TMPAssetEdit;
            TMP_FontAsset defaultFont = editAsset.defaultFontTMP;
            Material defaultMaterial = editAsset.defaultMaterial;
            TextMeshProUGUI[] texts = res.GetComponentsInChildren<TextMeshProUGUI>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                TextMeshProUGUI text = texts[i];
                string oldFontName = null;
                if (text.font == null)
                {
                    Debug.Log($"【替换未报备字体-】:{oldFontName}->{defaultFont.name}   assetPath:{assetPath}     obj:{text.name} ");
                    text.font = defaultFont;
                }

                if (!fontNameList.Contains(text.font.name) && !text.font.name.StartsWith("ImgFont_"))
                {
                    oldFontName = text.font.name;
                    Debug.Log($"【替换未报备字体-】:{oldFontName}->{defaultFont.name}   assetPath:{assetPath}     obj:{text.name} ");
                    text.font = defaultFont;
                    text.fontMaterial = defaultMaterial;
                }
            }

            return true;
        }
#endif

        public static void checkDependenceRes(string name, Action endAction)
        {
            CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}/UI/Views", checkDependence, null, endAction);
        }

        private static bool checkDependence(string assetPath, GameObject res, object obj1)
        {
            string[] strArr = AssetDatabase.GetDependencies(assetPath);
            List<string> list = new List<string>();
            for (int i = 0; i < strArr.Length; i++)
            {
                string resPath = strArr[i];
                if (resPath.EndsWith(".TTF")) continue;
                if (resPath.EndsWith(".ttf")) continue;
                if (resPath.EndsWith(".cs")) continue;
                if (!resPath.StartsWith($"Assets/{PathUtil.RES_FOLDER}/UI"))
                {
                    list.Add(resPath);
                }
            }

            if (list.Count > 0)
            {
                Debug.Log($"[UI]跨类型资源引用：{assetPath}");
                for (int i = 0; i < list.Count; i++)
                {
                    Debug.Log($"-------------{list[i]}");
                }
            }

            return true;
        }
    }
}