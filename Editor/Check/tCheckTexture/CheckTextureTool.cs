/*----------------------------------------------------------------
// 文件名：CheckTexture.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/1/20 20:13:20
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JQCore.tFileSystem;
using JQFramework.tUGUI;
using JQCore.tUtil;
using JQEditor.Build;
using JQEditor.tAssetPostprocessor;
using JQCore.DynamicTexture;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace JQEditor.Check.tCheckTexture
{
    public class CheckTextureTool
    {
        private string uiPath = $"Assets/{PathUtil.RES_FOLDER}/UI"; //所有图片的路径

        string viewsFolder = $"Assets/{PathUtil.RES_FOLDER}/UI/Views"; //界面目录


        //图集
        public static string spriteAtlasFolder = $"Assets/{PathUtil.RES_FOLDER}/UI/SpriteAtlas"; //图集目录
        private string MainAtlas = "MainAtlas"; //主界面图集
        private string UseOneAtlas = "UseOneAtlas"; //引用计数1的图集
        private string UseTwoAtlas = "UseTwoAtlas"; //引用计数2的图集
        private string UseThreeAtlas = "UseThreeAtlas"; //引用计数3的图集
        private string UseMoreAtlas = "UseMoreAtlas"; //引用计数大于3的图集-通用图集

        //非图集
        private string uiTextureLoader = $"Assets/{PathUtil.RES_FOLDER}/UI/NotProgramLoad/UITextureLoader"; //动态加载图

        Dictionary<string, CheckTextureInfo> picInfoDic = new Dictionary<string, CheckTextureInfo>();
        Dictionary<string, CheckSpriteAtlasInfo> spriteAtlasInfoDic = new Dictionary<string, CheckSpriteAtlasInfo>();
        List<CheckTextureInfo> unusePicList = new List<CheckTextureInfo>();
        Dictionary<string, string> moveRecordDic = new Dictionary<string, string>();


        public void build(string name, Action endAction, bool handleBigPic, string[] extraPath = null)
        {
            picInfoDic.Clear();
            moveRecordDic.Clear();
            spriteAtlasInfoDic.Clear();
            unusePicList.Clear();

            CollectAllTexture(extraPath);
            CaculateDependensNums();
            //清空图集目录
            CleanSpriteAtlasFolder();

            //分类
            ClassifyTexture(handleBigPic);
            // MoveTexture();
            // SetSingleTag();


            EditorUtility.ClearProgressBar();
            if (handleBigPic)
            {
                CheckCommonTools.SearchAndDo(name, viewsFolder, handlerOneView, null, endAction);
            }

            // endAction?.Invoke();
        }


        private void CleanSpriteAtlasFolder()
        {
            JQFileUtil.DeleteDirectory(spriteAtlasFolder);
            JQFileUtil.CreateDirectory(spriteAtlasFolder);
            AssetDatabase.Refresh();
        }


        private CheckSpriteAtlasInfo GetSpriteAtlasInfo(string atlasFileName)
        {
            CheckSpriteAtlasInfo spriteAtlasInfo = null;
            if (spriteAtlasInfoDic.ContainsKey(atlasFileName))
            {
                spriteAtlasInfo = spriteAtlasInfoDic[atlasFileName];
            }
            else
            {
                spriteAtlasInfo = new CheckSpriteAtlasInfo(spriteAtlasFolder, atlasFileName);
                spriteAtlasInfoDic[atlasFileName] = spriteAtlasInfo;
            }

            return spriteAtlasInfo;
        }

        private void AddToSpriteAtlas(string atlasFileName, Sprite sprite)
        {
            CheckSpriteAtlasInfo spriteAtlas = GetSpriteAtlasInfo(atlasFileName);
            spriteAtlas.AddSpirte(sprite);
        }

        private void ClassifyTexture(bool handleBigPic)
        {
            int count2 = 0;
            int total2 = picInfoDic.Count;
            foreach (CheckTextureInfo checkTextureInfo in picInfoDic.Values)
            {
                count2++;
                EditorUtility.DisplayProgressBar("UI图片整理-归类图片", $"{count2}/{total2}", (float)count2 / total2);
                string picPath = checkTextureInfo.path;
                string fileName = JQFileUtil.getCurrFolderOrFileName(picPath);
                int refCount = checkTextureInfo.refCount;

                TextureImporter picTextureImporter = AssetImporter.GetAtPath(picPath) as TextureImporter;
                Vector2 size = AssetTools.GetTextureImporterSize(picTextureImporter, 2048, false);

                if (refCount == 0) //未使用
                {
                    if (!unusePicList.Contains(checkTextureInfo))
                    {
                        unusePicList.Add(checkTextureInfo);
                    }
                }
                else if (size.x * size.y > 256 * 256 || size.x > 512 || size.y > 512) //图片过大，放到动态加载目录
                {
                    if (handleBigPic)
                    {
                        string newPath = uiTextureLoader + "/" + fileName;
                        if (newPath == picPath) continue;

                        Debug.Log($"移动图片：{picPath} =========> {newPath}");
                        string folderPath = JQFileUtil.getParentFolderPath(newPath);
                        JQFileUtil.CreateDirectory(folderPath);
                        string error = AssetDatabase.MoveAsset(picPath, newPath);
                        if (!string.IsNullOrEmpty(error))
                        {
                            Debug.LogError(error);
                        }

                        moveRecordDic[picPath] = newPath;
                    }
                }
                else if (checkTextureInfo.isMainViewRef()) //主界面用到的图片
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(checkTextureInfo.path);
                    //放到主界面图集中
                    AddToSpriteAtlas(MainAtlas, sprite);
                }
                else if (refCount == 1) //引用1次
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(checkTextureInfo.path);
                    AddToSpriteAtlas(UseOneAtlas, sprite);
                }
                else if (refCount == 2) //引用2次
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(checkTextureInfo.path);
                    AddToSpriteAtlas(UseTwoAtlas, sprite);
                }
                else if (refCount == 3) //引用3次
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(checkTextureInfo.path);
                    AddToSpriteAtlas(UseThreeAtlas, sprite);
                }
                else //引用大于3次
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(checkTextureInfo.path);
                    AddToSpriteAtlas(UseMoreAtlas, sprite);
                }
            }

            foreach (CheckSpriteAtlasInfo checkSpriteAtlasInfo in spriteAtlasInfoDic.Values)
            {
                checkSpriteAtlasInfo.Save();
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < unusePicList.Count; i++)
            {
                CheckTextureInfo checkTextureInfo = unusePicList[i];
                sb.Append($"{checkTextureInfo.path}\n");
            }
            string content = sb.ToString();
            JQFileUtil.SaveFile(content, $"{spriteAtlasFolder}/unUsePic.txt");
            
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 收集所有图片
        /// </summary>
        private void CollectAllTexture(string[] extraPath)
        {
            List<string> list = new List<string>();
            list.Add(uiPath);
            if (extraPath != null)
            {
                list.AddRange(extraPath);
            }
            
            string[] picGuids = AssetDatabase.FindAssets("t:texture2D", list.ToArray());
            int count0 = 0;
            int total0 = picGuids.Length;
            List<string> mulList = new List<string>();
            for (int i = 0; i < picGuids.Length; i++)
            {
                count0++;
                EditorUtility.DisplayProgressBar("UI图片整理-收集所有图片", $"{count0}/{total0}", (float)count0 / total0);
                string path = AssetDatabase.GUIDToAssetPath(picGuids[i]); //获取到UI下面的所有图片路径
                if(!isCheckPic(path))continue;
                if (picInfoDic.ContainsKey(path))
                {
                    if (!mulList.Contains(path))
                    {
                        mulList.Add(path);
                    }
                }
                else
                {
                    //                    Debug.Log("PIC: "+path);
                    picInfoDic[path] = new CheckTextureInfo(path);
                }
            }

            if (mulList.Count > 0)
            {
                Debug.LogError("存在重复资源");
                foreach (string s in mulList)
                {
                    Debug.LogError("    " + s);
                }
            }
        }
        
        private bool isCheckPic(string path)
        {
            if (path.Contains("Temp")) return false;
            if (path.Contains("ProgramLoad") && !path.Contains("NotProgramLoad")) return false; //程序设置的图片
            if (path.StartsWith($"Assets/{PathUtil.RES_FOLDER}/UI/Language/")) return false; //未定义
            if (path.Contains("AVProVideoIcon.png")) return false; //插件组件关联图片
            if (path.Contains("SoftMaskableIcon.png")) return false; //插件组件关联图片
            if (path.Contains("SoftMaskIcon.png")) return false; //插件组件关联图片
            if (path.Contains("Font")) return false; //字体图片
            if (path.EndsWith(".jpg") || path.EndsWith(".png"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 遍历界面prefab，计算所有图片的引用次数
        /// </summary>
        private void CaculateDependensNums()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new string[] { viewsFolder });
            int count1 = 0;
            int total1 = prefabGuids.Length;
            for (int i = 0; i < prefabGuids.Length; i++)
            {
                count1++;
                EditorUtility.DisplayProgressBar("UI图片整理-计算引用次数", $"{count1}/{total1}", (float)count1 / total1);
                string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                // Debug.Log($"收集图片引用次数，界面：{path}");
                if (path.Contains("Temp")) continue;
                if (!path.EndsWith(".prefab")) continue;
                List<string> dependenPicList = new List<string>();
                string[] dependencies = AssetDatabase.GetDependencies(path); //获取每个预制体所用的的图片的路径
                foreach (string dependency in dependencies)
                {
                    if (isCheckPic(dependency))
                    {
                        dependenPicList.Add(dependency);
                    }
                }

                //兼容DynamicImage组件
                GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                DynamicImage[] dynamicImages = prefab.GetComponentsInChildren<DynamicImage>(true);
                foreach (DynamicImage dynamicImage in dynamicImages)
                {
                    string spritePath = null;
                    if (!string.IsNullOrEmpty(dynamicImage.spritePath))
                    {
                        spritePath = $"Assets/{PathUtil.RES_FOLDER}/{dynamicImage.spritePath}";
                    }
                    else
                    {
                        spritePath = AssetDatabase.GetAssetPath(dynamicImage.sprite);
                    }


                    if (isCheckPic(spritePath))
                    {
                        dependenPicList.Add(spritePath);
                    }
                }

                //兼容DynamicSpriteRenderer组件
                DynamicSpriteRenderer[] dynamicSpriteRenderers = prefab.GetComponentsInChildren<DynamicSpriteRenderer>(true);
                foreach (DynamicSpriteRenderer dynamicSpriteRenderer in dynamicSpriteRenderers)
                {
                    string spritePath = null;
                    if (!string.IsNullOrEmpty(dynamicSpriteRenderer.spritePath))
                    {
                        spritePath = $"Assets/{PathUtil.RES_FOLDER}/{dynamicSpriteRenderer.spritePath}";
                    }
                    else
                    {
                        spritePath = AssetDatabase.GetAssetPath(dynamicSpriteRenderer.sprite);
                    }

                    if (isCheckPic(spritePath))
                    {
                        dependenPicList.Add(spritePath);
                    }
                }

                dependenPicList = dependenPicList.Distinct().ToList();

                foreach (string dependency in dependenPicList)
                {
                    //                    Debug.Log("xx:"+ dependency);
                    Object asset = AssetDatabase.LoadAssetAtPath<Object>(dependency);
                    if (asset is Texture2D)
                    {
                        if (picInfoDic.ContainsKey(dependency))
                        {
                            picInfoDic[dependency].addRefPath(path);
                        }
                        else
                        {
                            Debug.LogError($"图片路径异常：{path}  {dependency}");
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 根据引用次数和图片大小移动到指定目录
        /// </summary>
        // private void MoveTexture()
        // {
        //     int count2 = 0;
        //     int total2 = picCountDic.Count;
        //     foreach (KeyValuePair<string, int> keyValuePair in picCountDic)
        //     {
        //         count2++;
        //         EditorUtility.DisplayProgressBar("UI图片整理-移动图片", $"{count2}/{total2}", (float)count2 / total2);
        //         string picPath = keyValuePair.Key;
        //         string viewPath = null;
        //         textue2DViewDic.TryGetValue(picPath, out viewPath);
        //         string fileName = FileUtil.getCurrFolderOrFileName(picPath);
        //         string newPath = null;
        //         int picCount = keyValuePair.Value;
        //
        //         TextureImporter picTextureImporter = AssetImporter.GetAtPath(picPath) as TextureImporter;
        //         Vector2 size = AssetTools.GetTextureImporterSize(picTextureImporter, 2048);
        //
        //         if (picCount == 0) //未使用
        //         {
        //             newPath = unuseFolder + "/" + fileName;
        //         }
        //         else if (size.x * size.y > 256 * 256 || size.x > 512 || size.y > 512) //图片过大，放到动态加载目录
        //         {
        //             newPath = uiTextureLoader + "/" + fileName;
        //         }
        //         else if (isMainAtlas(viewPath)) //主界面用到的图片都算公用
        //         {
        //             newPath = mainViewFolder + "/" + fileName;
        //         }
        //         else if (picCount == 1) //专用
        //         {
        //             string atlasTag = "ui/views/" + FileUtil.getCurrFolderOrFileName(viewPath);
        //             newPath = singleUse + "/" + fileName;
        //             picTagDic[newPath] = atlasTag;
        //         }
        //         else if (picCount > 1) //公用
        //         {
        //             newPath = commonFolder + "/" + fileName;
        //         }
        //         else //未使用
        //         {
        //             newPath = unuseFolder + "/" + fileName;
        //         }
        //
        //         if (!newPath.Equals(picPath))
        //         {
        //             Debug.Log($"移动图片：{picPath} =========> {newPath}");
        //             string folderPath = FileUtil.getParentFolderPath(newPath);
        //             FileUtil.CreateDirectory(folderPath);
        //             string error = AssetDatabase.MoveAsset(picPath, newPath);
        //             if (!string.IsNullOrEmpty(error))
        //             {
        //                 Debug.LogError(error);
        //             }
        //
        //             moveRecordDic[picPath] = newPath;
        //         }
        //     }
        //
        //     AssetDatabase.Refresh();
        // }

        /// <summary>
        /// 设置专用图集Tag
        /// </summary>
        // private void SetSingleTag()
        // {
        //     int count3 = 0;
        //     int total3 = picTagDic.Count;
        //     foreach (KeyValuePair<string, string> valuePair in picTagDic)
        //     {
        //         count3++;
        //         EditorUtility.DisplayProgressBar("UI图片整理-设置专用图集", $"{count3}/{total3}", (float)count3 / total3);
        //         string path = valuePair.Key;
        //         string atlasTag = valuePair.Value;
        //         TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        //         string preTag = textureImporter.spritePackingTag;
        //         atlasTag = atlasTag.Replace($"Assets/{PathUtil.RES_FOLDER}/", "");
        //         string newTag = atlasTag.ToLower();
        //         if (!preTag.Equals(newTag))
        //         {
        //             textureImporter.spritePackingTag = newTag;
        //             textureImporter.SaveAndReimport();
        //         }
        //     }
        //
        //     AssetDatabase.Refresh();
        // }
        public bool handlerOneView(string assetPath, GameObject cloneGo, object obj1)
        {
            if (assetPath.Contains("LaucherLoading")) return false;
            bool change = false;
            Image[] images = cloneGo.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                Image image = images[i];
                //大图处理 DynamicImage->Image   Image ->DynamicImage
                if (handleDynamicImage(image, cloneGo))
                {
                    change = true;
                }
            }

            SpriteRenderer[] spriteRenderers = cloneGo.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = spriteRenderers[i];
                //大图处理 DynamicImage->Image   Image ->DynamicImage
                if (handleDynamicSpriteRenderer(spriteRenderer, cloneGo))
                {
                    change = true;
                }
            }

            //移除动态加载图关联
            DynamicImage[] dynamicImages = cloneGo.GetComponentsInChildren<DynamicImage>(true);
            for (int i = 0; i < dynamicImages.Length; i++)
            {
                DynamicImage image = dynamicImages[i];
                //                image.checkJpg();
                //                                image.restoreSprite();
                bool success = image.removeSprite();

                if (success)
                {
                    change = true;
                }
            }

            DynamicSpriteRenderer[] dynamicSpriteRenderers = cloneGo.GetComponentsInChildren<DynamicSpriteRenderer>(true);
            for (int i = 0; i < dynamicSpriteRenderers.Length; i++)
            {
                DynamicSpriteRenderer dynamicSpriteRenderer = dynamicSpriteRenderers[i];
                //                image.checkJpg();
                //                dynamicSpriteRenderer.restoreSprite();
                bool success = dynamicSpriteRenderer.removeSprite();

                if (success)
                {
                    change = true;
                }
            }

            return change;
        }

        //是否需要动图组件
        private bool needDynamicBehaviour(string url)
        {
            return url.Contains("UITextureLoader") || url.Contains("UITextureLoaderCode");
        }

        //sprite是否由程序设置
        private bool isCodeSetImage(string url)
        {
            return url.Contains("UITextureLoaderCode");
        }

        private bool handleDynamicSpriteRenderer(SpriteRenderer sprteRenderer, GameObject cloneGo)
        {
            bool change = false;

            DynamicSpriteRenderer dynamicSpriteRenderer = sprteRenderer.GetComponent<DynamicSpriteRenderer>();
            string url = null;
            if (sprteRenderer.sprite != null && sprteRenderer.sprite.name != "unknown")
            {
                url = AssetDatabase.GetAssetPath(sprteRenderer.sprite); //图片直接关联，使用Sprite的Url
            }
            else if (dynamicSpriteRenderer != null && !string.IsNullOrEmpty(dynamicSpriteRenderer.spritePath))
            {
                url = $"Assets/{PathUtil.RES_FOLDER}/{dynamicSpriteRenderer.spritePath}";
                if (moveRecordDic.ContainsKey(url)) //关联的图片被移动过，使用新的url
                {
                    url = moveRecordDic[url];
                }
            }
            else
            {
                Debug.LogError("[图片是默认图，路径为空]" + cloneGo.name + "----" + sprteRenderer.name);
            }

            //如果图片不在UITextureLoader目录下，DynamicSpriteRenderer
            if (url != null)
            {
                if (!needDynamicBehaviour(url) && dynamicSpriteRenderer != null)
                {
                    dynamicSpriteRenderer.restoreSprite();
                    Object.DestroyImmediate(dynamicSpriteRenderer);
                    change = true;
                }

                if (needDynamicBehaviour(url) && dynamicSpriteRenderer == null)
                {
                    sprteRenderer.gameObject.AddComponent<DynamicSpriteRenderer>();
                    change = true;
                }

                if (isCodeSetImage(url))
                {
                    sprteRenderer.sprite = null;
                }
            }

            return change;
        }

        private bool handleDynamicImage(Image image, GameObject cloneGo)
        {
            bool change = false;

            if (image is DynamicImage)
            {
                DynamicImage dynamicImage = image as DynamicImage;
                string url = null;
                if (image.sprite != null && image.sprite.name != "unknown")
                {
                    url = AssetDatabase.GetAssetPath(image.sprite); //图片直接关联，使用Sprite的Url
                }
                else if (!string.IsNullOrEmpty(dynamicImage.spritePath))
                {
                    url = $"Assets/{PathUtil.RES_FOLDER}/{dynamicImage.spritePath}";
                    if (moveRecordDic.ContainsKey(url)) //关联的图片被移动过，使用新的url
                    {
                        url = moveRecordDic[url];
                    }
                }
                else
                {
                    Debug.LogError("[图片是默认图，路径为空]" + cloneGo.name + "----" + image.name);
                }

                //如果图片不在UITextureLoader目录下，则DynamicImage替换为Image
                if (url != null && !needDynamicBehaviour(url))
                {
                    GameObject gameObject = dynamicImage.gameObject;
                    Sprite sprite = AssetDatabase.LoadAssetAtPath(url, typeof(Sprite)) as Sprite;
                    Image.Type imgType = dynamicImage.type;
                    Image.FillMethod fillMethod = dynamicImage.fillMethod;
                    bool raycastTarget = dynamicImage.raycastTarget;
                    int fillOrigin = dynamicImage.fillOrigin;
                    float fillAmount = dynamicImage.fillAmount;
                    Object.DestroyImmediate(dynamicImage);

                    Image newImage = gameObject.AddComponent<Image>();
                    if (isCodeSetImage(url))
                    {
                        newImage.sprite = null;
                    }
                    else
                    {
                        newImage.sprite = sprite;
                    }

                    newImage.type = imgType;
                    newImage.fillMethod = fillMethod;
                    newImage.fillOrigin = fillOrigin;
                    newImage.fillAmount = fillAmount;
                    newImage.raycastTarget = raycastTarget;
                    change = true;
                }
            }
            else
            {
                string url = AssetDatabase.GetAssetPath(image.sprite);
                //如果图片是大图（在UITextureLoader目录下），则Image替换为DynamicImage
                if (needDynamicBehaviour(url))
                {
                    NumerousImage numerousImage = image.GetComponent<NumerousImage>();
                    if (numerousImage == null)
                    {
                        Debug.Log("[大图未设置DynamicImage]" + cloneGo.name + "----" + image.name);
                        GameObject gameObject = image.gameObject;
                        Sprite sprite = image.sprite;
                        Image.Type imgType = image.type;
                        Image.FillMethod fillMethod = image.fillMethod;
                        bool raycastTarget = image.raycastTarget;
                        int fillOrigin = image.fillOrigin;
                        float fillAmount = image.fillAmount;
                        Object.DestroyImmediate(image);

                        DynamicImage dynamicImage = gameObject.AddComponent<DynamicImage>();
                        dynamicImage.sprite = sprite;
                        dynamicImage.type = imgType;
                        dynamicImage.fillMethod = fillMethod;
                        dynamicImage.fillOrigin = fillOrigin;
                        dynamicImage.fillAmount = fillAmount;
                        dynamicImage.raycastTarget = raycastTarget;
                        change = true;
                    }
                    else
                    {
                        Debug.LogError("图片在动图目录，但是组件有NumerousImage，请更换制作方式：" + url + "   " + cloneGo.name);
                    }
                }
                else
                {
                    if (isCodeSetImage(url))
                    {
                        image.sprite = null;
                    }
                }
            }

            return change;
        }
    }
}