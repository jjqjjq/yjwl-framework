using System;
using System.Collections.Generic;
using System.IO;
using JQCore;
using JQCore.Log;
using JQCore.tLog;
using JQCore.tUtil;
using JQFramework.tUGUI;
using JQFramework.tUGUI.tDynamicScrollingList;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

//Lua调用C#使用到的U3D底层类  (如GameObject Component Transform Vector3等)
namespace JQFramework
{
    public static class CommonUtil
    {
        public static bool IsIphone()
        {
#if UNITY_IPHONE
            return true;
#else
            return false;
#endif
        }

        public static bool IsAndroid()
        {
#if UNITY_ANDROID
            return true;
#else
            return false;
#endif
        }

        public static bool HasSDK()
        {
            return Sys.hasSDK;
        }

        public static bool IsLog()
        {
            return HyDebug.Instance.EnableLog;
        }

        public static bool IsEditor()
        {
            return Sys.isEditor;
        }

        public static void GizmosDrawLine(float fromX, float fromY, float fromZ, float toX, float toY, float toZ)
        {
            Gizmos.DrawLine(new Vector3(fromX, fromY, fromZ), new Vector3(toX, toY, toZ));
        }

        #region RectTransform

        #endregion

        public static float Sin_EX(float angle)
        {
            return Mathf.Sin(angle * Mathf.Deg2Rad);
        }

        public static float getRad2Deg_EX()
        {
            return Mathf.Rad2Deg;
        }

        public static float getDeg2Rad_EX()
        {
            return Mathf.Deg2Rad;
        }


        //        public static void setIsTrigger_EX(this BoxCollider boxCollider, bool isTrigger)
        //        {
        //            boxCollider.isTrigger = isTrigger;
        //        }
        //
        //        public static void setEnabled_EX(this BoxCollider boxCollider, bool enabled)
        //        {
        //            boxCollider.enabled = enabled;
        //        }

        public static float Cos_EX(float angle)
        {
            return Mathf.Cos(angle * Mathf.Deg2Rad);
        }

        //    public static void Destory_EX(this RectTransform obj)
        //    {
        //        Object.Destroy(obj.gameObject);
        //    }
        //
        //    public static void Destory_EX(this GameObject obj)
        //    {
        //        Object.Destroy(obj);
        //    }
        //
        //    public static void Destory_EX(this Transform obj)
        //    {
        //        Object.Destroy(obj.gameObject);
        //    }
        //    public static void Destory_EX(this Component obj)
        //    {
        //        Object.Destroy(obj);
        //    }


        public static RenderTexture GetTemporary_EX(string name, int width, int height, int depthBuffer, int msaa)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, depthBuffer);
#if UNITY_EDITOR

            if (!string.IsNullOrEmpty(name))
            {
                renderTexture.name = Sys.stringBuilder.Append("TempRenderTexture_").Append(name).Gap().Append(width).Append('*')
                    .Append(height).ToString();
            }
#endif
            renderTexture.antiAliasing = msaa;
            renderTexture.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
            renderTexture.depthStencilFormat = GraphicsFormat.D16_UNorm;


            return renderTexture;
        }

        public static void ReleaseTemporary_Ex(RenderTexture renderTexture)
        {
            RenderTexture.ReleaseTemporary(renderTexture);
        }

        #region GameObject

        public static GameObject StaticFind_EX(string path)
        {
            return GameObject.Find(path);
        }


        public static GameObject Find_EX(this GameObject gameObject, string path)
        {
            //            if (gameObject.transform.parent != null)
            //            {
            //                JQLog.LogError($"Find:{gameObject.transform.parent.name}/{gameObject.name}   {path} ");
            //            }
            //            else
            //            {
            //                JQLog.LogError($"Find:{gameObject.name}   {path} ");
            //            }
            Transform childTransform = gameObject.transform.Find(path);
            if (childTransform != null)
                return childTransform.gameObject;
            return null;
        }

        private static bool _isEditLog;

        public static void setEditLog(bool isEditLog)
        {
            _isEditLog = isEditLog;
        }


        //RectTransform.anchoredPosition3D
        public static GameObject SetRectTransformAnchoredPosition3D_EX(this GameObject go, float x, float y, float z)
        {
            (go.transform as RectTransform).anchoredPosition3D = CreateVector3(x, y, z);
            return go;
        }

        //设置宽高
        public static GameObject SetSizeDelta_EX(this GameObject go, float x, float y)
        {
            (go.transform as RectTransform).sizeDelta = CreateVector2(x, y);
            return go;
        }

        //设置大小
        public static GameObject SetRectTransformLocalScale_EX(this GameObject go, float x, float y, float z)
        {
            (go.transform as RectTransform).localScale = CreateVector3(x, y, z);
            return go;
        }

        //设置锚点对齐
        public static GameObject SetPivot_EX(this GameObject go, float x, float y)
        {
            (go.transform as RectTransform).pivot = CreateVector2(x, y);
            return go;
        }


        public static GameObject setAnchorMin_EX(this GameObject go, float x, float y)
        {
            (go.transform as RectTransform).anchorMin = CreateVector2(x, y);
            return go;
        }

        public static GameObject setAnchorMax_EX(this GameObject go, float x, float y)
        {
            (go.transform as RectTransform).anchorMax = CreateVector2(x, y);
            return go;
        }

        public static GameObject SetInsetAndSizeFromParentEdge_EX(this GameObject go, byte type, float inset, float size)
        {
            switch (type)
            {
                case 1:
                    (go.transform as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, inset, size);
                    (go.transform as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, inset, size);
                    break;
                case 2:
                    (go.transform as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, inset, size);
                    (go.transform as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, inset, size);
                    break;
                case 3:
                    (go.transform as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, inset, size);
                    break;
                case 4:
                    (go.transform as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, inset, size);
                    break;
                case 5:
                    (go.transform as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, inset, size);
                    (go.transform as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, inset, size);
                    break;
                case 6:
                    (go.transform as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, inset, size);
                    break;
                case 7:
                    (go.transform as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, inset, size);
                    (go.transform as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, inset, size);
                    break;
            }

            return go;
        }

        public static GridLayoutGroup SetCellSize(this GridLayoutGroup grid, float size_x, float size_y)
        {
            grid.cellSize = CreateVector2(size_x, size_y);
            return grid;
        }

        #endregion

        #region Vector2

        private static Vector2 CreateVector2(float x, float y)
        {
            if (x == 0f && y == 0f)
            {
                return Vector2.zero;
            }

            if (x == 1f && y == 1f)
            {
                return Vector2.one;
            }

            return new Vector2(x, y);
        }

        public static float Distance(float x1, float y1, float x2, float y2)
        {
            return Vector2.Distance(CreateVector2(x1, y1), CreateVector2(x2, y2));
        }

        #endregion

        #region Vector3

        public static Vector4 CreateVector4(float x, float y, float w, float z)
        {
            if (x == 0f && y == 0f && w == 0f && z == 0f)
            {
                return Vector4.zero;
            }

            if (x == 1f && y == 1f && w == 1f && z == 1f)
            {
                return Vector4.one;
            }

            return new Vector4(x, y, w, z);
        }

        public static Vector3 CreateVector3(float x, float y, float z)
        {
            if (x == 0f && y == 0f && z == 0f)
            {
                return Vector3.zero;
            }

            if (x == 1f && y == 1f && z == 1f)
            {
                return Vector3.one;
            }

            return new Vector3(x, y, z);
        }

        public static float Distance(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            return Vector3.Distance(CreateVector3(x1, y1, z1), CreateVector3(x2, y2, z2));
        }

        #endregion

        public static void SetImageColor_EX(this Image img, float r, float g, float b, float a)
        {
            img.color = new Color(r, g, b, a);
        }

        public static Color GetColor(float r, float g, float b, float a)
        {
            return new Color(r, g, b, a);
        }

        public static void SetTextColorTransparency_EX(this Text text, float a)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, a);
        }

        public static void SetTextColor_EX(this Text text, float r, float g, float b, float a)
        {
            text.color = new Color(r, g, b, a);
        }


        public static void SetImageNormal_EX(this Image img)
        {
            img.material = null;
        }

        public static void SetLayer(GameObject go, int layer)
        {
            go.layer = layer;

            Transform t = go.transform;

            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                SetLayer(child.gameObject, layer);
            }
        }

        public static T GetComponentInParent<T>(Transform transform) where T : UnityEngine.Component
        {
            T t = transform.GetComponent<T>();
            if (t == null && transform.parent != null)
            {
                return GetComponentInParent<T>(transform.parent);
            }

            return t;
        }

        // private static int GetMaskOrRectMask2D(Transform transform, out Mask mask, out RectMask2D rectMask2D, out SoftMask softMask)
        private static int GetMaskOrRectMask2D(Transform transform, out Mask mask, out RectMask2D rectMask2D,
            out RectMask2D softMask)
        {
            mask = transform.GetComponent<Mask>();
            if (mask != null)
            {
                //                mask = null;
                rectMask2D = null;
                softMask = null;
                return 1;
            }

            rectMask2D = transform.GetComponent<RectMask2D>();
            if (rectMask2D != null)
            {
                mask = null;
                //                rectMask2D = null;
                softMask = null;
                return 2;
            }

            softMask = null;
            // softMask = transform.GetComponent<SoftMask>();
            // if (softMask != null)
            // {
            //     mask = null;
            //     rectMask2D = null;
            //     //                softMask = null;
            //     return 3;
            // }

            if (transform.parent != null)
            {
                return GetMaskOrRectMask2D(transform.parent, out mask, out rectMask2D, out softMask);
            }

            return 0;
        }


        private static Vector4 fullScreenVec = new Vector4(-10000f, -10000f, 10000f, 10000f);
        private static MaterialPropertyBlock _materialPropertyBlock = new MaterialPropertyBlock();

        public static RectTransform findNearestMask(Transform trans)
        {
            int result = GetMaskOrRectMask2D(trans, out var mask, out var rectMask2D, out var softMask);
            if (result == 1)
            {
                return mask.rectTransform;
            }

            if (result == 2)
            {
                return rectMask2D.rectTransform;
            }

            if (result == 3)
            {
                return softMask.rectTransform;
            }

            return null;
        }


        // 设置粒子系统的UI遮罩值

        //1层的mask处理-一次性更新遮罩区域
        public static void SetConstParticleMask(GameObject particleGo)
        {
            ParticleSystem[] psList = particleGo.GetComponentsInChildren<ParticleSystem>(true);
            RectTransform maskRectTrans = findNearestMask(particleGo.transform);
            if (maskRectTrans == null) return;

            Vector3[] cornersList = new Vector3[4];
            if (maskRectTrans != null)
            {
                maskRectTrans.GetWorldCorners(cornersList);
            }

            Vector4 clipRectVec = new Vector4(cornersList[0].x, cornersList[0].y, cornersList[2].x, cornersList[2].y);
            for (int i = 0; i < psList.Length; i++)
            {
                // 考虑到性能暂时使用sharedMaterial,对于共用材质的对象可能出错
                Renderer renderer = psList[i].GetComponent<Renderer>();
                renderer.GetPropertyBlock(_materialPropertyBlock);
                _materialPropertyBlock.SetVector("_ClipRect", clipRectVec);
                _materialPropertyBlock.SetVector("_ClipRect2", fullScreenVec);
                _materialPropertyBlock.SetFloat("_UseClipRect", 1.0f);
                renderer.SetPropertyBlock(_materialPropertyBlock);
            }
        }

        public static void ResetClipRect(GameObject particleGo)
        {
            ParticleSystem[] psList = particleGo.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < psList.Length; i++)
            {
                // 考虑到性能暂时使用sharedMaterial,对于共用材质的对象可能出错
                Renderer renderer = psList[i].GetComponent<Renderer>();
                renderer.GetPropertyBlock(_materialPropertyBlock);
                _materialPropertyBlock.SetVector("_ClipRect", fullScreenVec);
                _materialPropertyBlock.SetVector("_ClipRect2", fullScreenVec);
                _materialPropertyBlock.SetFloat("_UseClipRect", 1.0f);
                renderer.SetPropertyBlock(_materialPropertyBlock);
            }
        }

        private static RaycastHit[] hitList = new RaycastHit[20];

        public static void RaycastNonAlloc(out RaycastHit[] hitResult, out int count)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.green, 1);

            for (int i = 0; i < 20; i++)
            {
                hitList[i] = default(RaycastHit);
            }

            count = Physics.RaycastNonAlloc(ray, hitList);
            hitResult = hitList;
        }

        private static int GraphicRaycasterCompare(Canvas a, Canvas b)
        {
            return b.sortingOrder - a.sortingOrder;
        }

        public static List<RaycastResult> RaycastUI(Transform canvas)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            Canvas[] canvases = canvas.GetComponentsInChildren<Canvas>();
            Array.Sort(canvases, GraphicRaycasterCompare);
            List<RaycastResult> results = new List<RaycastResult>();
            //            int index = 0;
            foreach (Canvas oneCanvas in canvases)
            {
                GraphicRaycaster graphicRaycaster = oneCanvas.GetComponent<GraphicRaycaster>();
                if (graphicRaycaster == null) continue;
                graphicRaycaster.Raycast(pointerEventData, results);
                //                if (results.Count > index)
                //                {
                //                    JQLog.Log("canvas:" + oneCanvas.name + "    " + oneCanvas.sortingOrder);
                //                }
                //                for (int i = index; i < results.Count; i++)
                //                {
                //                    JQLog.Log("add:"+results[i].gameObject.name);
                //                }
                //                index = results.Count;
            }

            return results;
        }

        public static Transform selectOneChildIndex(Transform root, string name)
        {
            return selectOneChild(root, name);
        }

        public static Transform selectOneChild(Transform root, string name)
        {
            Transform selectTransform = null;
            if (!string.IsNullOrEmpty(name))
            {
                selectTransform = root.Find(name);
            }

            for (int i = 0; i < root.childCount; i++)
            {
                Transform transform = root.GetChild(i);
                if (transform != selectTransform)
                {
                    transform.gameObject.SetActive_EX(false);
                }
            }

            if (selectTransform != null)
            {
                selectTransform.SetActive_EX(true);
            }

            return selectTransform;
        }

        #region ScrollRect

        public static ScollViewOnValueChange AddCallBackOnValueChange(this ScrollRect sc, Action callback)
        {
            ScollViewOnValueChange scrollevent = sc.gameObject.AddMissingComponent<ScollViewOnValueChange>();
            scrollevent.sc = sc;
            scrollevent.callback = callback;
            return scrollevent;
        }

        public static void SetScHorizontalNormalizedPosition(this ScrollRect sc, float val)
        {
            sc.horizontalNormalizedPosition = val;
        }

        public static void SetScVerticalNormalizedPosition(this ScrollRect sc, float val)
        {
            sc.verticalNormalizedPosition = val;
        }

        #endregion

        public static void WorldToScreen(Camera sceneCamera, Vector3 worldTransformPos, float offsetX, float offsetY,
            float offsetZ, RectTransform uiRectTransform,
            bool outScreenHide, float hideSize)
        {
            // JQLog.LogError("pos:"+ worldTransformPos + " Camera:"+sceneCamera.name);
            Vector3 offsetVector3 = CreateVector3(offsetX, offsetY, offsetZ);
            Vector3 vector3 = sceneCamera.WorldToViewportPoint(worldTransformPos + offsetVector3);
            uiRectTransform.SetAnchorMax_EX(vector3.x, vector3.y);
            uiRectTransform.SetAnchorMin_EX(vector3.x, vector3.y);
            uiRectTransform.anchoredPosition3D = Vector3.zero;
            if (outScreenHide)
            {
                bool active = true;
                float x = uiRectTransform.localPosition.x;
                float y = uiRectTransform.localPosition.y;
                if (x < -Screen.width * hideSize || Screen.width * hideSize < x)
                {
                    active = false;
                }

                if (y < -Screen.height * hideSize || Screen.height * hideSize < y)
                {
                    active = false;
                }

                if (active)
                {
                    uiRectTransform.localScale = Vector3.one;
                }
                else
                {
                    uiRectTransform.localScale = Vector3.zero;
                }
            }
        }


        public static void WorldToScreen(Camera sceneCamera, Transform worldTransform, float offsetX, float offsetY,
            float offsetZ, RectTransform uiRectTransform,
            bool outScreenHide, float hideSize)
        {
            WorldToScreen(sceneCamera, worldTransform.position, offsetX, offsetY, offsetZ, uiRectTransform, outScreenHide,
                hideSize);
        }

        public static Vector3 ScreenToWorld(Camera camera, float x, float y, float z)
        {
            return camera.ScreenToWorldPoint(CreateVector3(x, y, z));
        }

        public static bool IsNull_EX(this UnityEngine.Object o) // 或者名字叫IsDestroyed等等
        {
            return o == null;
        }

        public static bool IsNull_EX(this System.Object o) // 或者名字叫IsDestroyed等等
        {
            return o == null;
        }

        public static Vector3 GetMousePos(GameObject canObj)
        {
            Canvas canvas = canObj.GetComponent<Canvas>();
            RectTransform rectCanvas = canvas.gameObject.transform as RectTransform;
            Vector2 v = Vector2.zero;
            //if (canvas == null || rectCanvas == null)
            //{
            //    return v;
            //}
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectCanvas, Input.mousePosition, canvas.worldCamera, out v);
            return v;
        }

        //坐标转换
        public static Vector2 GetRectangleContainsScreenPoint(this GameObject canvasOBJ, float x, float y)
        {
            Vector2 _pos = Vector2.zero;
            Canvas canvas = canvasOBJ.GetComponent<Canvas>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, CreateVector2(x, y),
                canvas.worldCamera, out _pos);
            return _pos;
        }

        //设置窗口到鼠标的的位置，要传一个Canvas的物体

        public static void SetViewPositionToMousePosition_EX(this GameObject view, GameObject canvasOBJ)
        {
            Vector2 _pos = Vector2.zero;
            Canvas canvas = canvasOBJ.GetComponent<Canvas>();
            RectTransform rectCanvas = canvas.transform as RectTransform;
            RectTransform rectView = view.transform as RectTransform;
            if (rectCanvas == null || rectView == null)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectCanvas, Input.mousePosition, canvas.worldCamera,
                out _pos);
            view.transform.localPosition = new Vector3(_pos.x, _pos.y, 0);

            //Bounds viewBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rectView);

            //float anchorX = rectView.anchoredPosition3D.x;
            //float anchorY = rectView.anchoredPosition3D.y;

            //if (anchorX + viewBounds.size.x > UnityEngine.Screen.width)
            //{
            //    _pos.x -= viewBounds.size.x;
            //}

            //if (anchorY + viewBounds.size.y > UnityEngine.Screen.height)
            //{
            //    _pos.y -= viewBounds.size.y;
            //}
            view.transform.localPosition = new Vector3(_pos.x, _pos.y, 0);
        }

        #region string

        // public static string announce(string text, string key)
        // {
        //     string temp = "真的很牛皮";
        //     XMLNode xml = XMLParser.Parse(text);
        //     //            JQLog.Log(xml);
        //     temp = xml.GetValue(key);
        //     return temp;
        // }

        /// <summary>
        /// 计算字符串在指定text控件中的长度
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int CalculateLengthOfText(this Text tex, string message)
        {
            int totalLength = 0;
            Font myFont = tex.font;
            myFont.RequestCharactersInTexture(message, tex.fontSize, tex.fontStyle);
            CharacterInfo characterInfo = new CharacterInfo();
            char[] arr = message.ToCharArray();

            foreach (char c in arr)
            {
                myFont.GetCharacterInfo(c, out characterInfo, tex.fontSize);

                totalLength += characterInfo.advance;
            }

            return totalLength;
        }

        public static void callGC()
        {
            JQLog.Log("GC");
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        public static void LoadFilterZhCallback(string filterText)
        {
            //        Debug.Log("屏蔽字加载：" + filterText);
            FilterString = filterText.Split('、');
            JQLog.Log("屏蔽字数量:" + FilterString.Length);
        }

        public static string[] FilterString; //需要过滤的敏感词汇(聊天相关） 

        public static string FilterContent(string str)
        {
            string temp = str.Replace(" ", "").Replace("&", "");
            temp = temp.ToLower();
            bool filter = false;
            for (int i = 0; i < FilterString.Length; i++)
            {
                if (temp.Contains(FilterString[i]))
                {
                    string starStr = "";
                    for (int j = 0; j < FilterString[i].Length; j++)
                        starStr += "*";
                    temp = temp.Replace(FilterString[i], starStr);
                    filter = true;
                }
            }

            bool check1 = true, check2 = true;
            if (filter)
            {
                int index = -1;
                int index2 = -1;
                do
                {
                    if (check1)
                    {
                        index = str.IndexOf(" ", index + 1);
                        if (index == -1)
                        {
                            check1 = false;
                        }
                    }

                    if (check2)
                    {
                        index2 = str.IndexOf("&", index2 + 1);
                        if (index2 == -1)
                        {
                            check2 = false;
                        }
                    }

                    if (index != -1 && index2 != -1)
                    {
                        if (index < index2)
                        {
                            temp = temp.Insert(index, " ");
                            index2 = index;
                        }
                        else
                        {
                            temp = temp.Insert(index2, "&");
                            index = index2;
                        }
                    }
                    else
                    {
                        if (index != -1)
                        {
                            temp = temp.Insert(index, " ");
                        }
                        else if (index2 != -1)
                        {
                            temp = temp.Insert(index2, "&");
                        }
                    }
                } while (index != -1 || index2 != -1);
            }

            return filter ? temp : str;
        }

        //是否包含敏感字（聊天相关）
        public static bool ContainsFilter(string str)
        {
            string temp = str.Replace(" ", "").Replace("&", "");
            temp = temp.ToLower();
            if (FilterString == null)
            {
                return false;
            }

            for (int i = 0; i < FilterString.Length; i++)
            {
                if (temp.Contains(FilterString[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static string md5(string str)
        {
            return Md5Util.CreateMd5(str);
        }

        #endregion


        //复制东西
        public static void CopyString(string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }

        #region math

        public static void AddPosForward(Transform transform, float multiply, out float x, out float y, out float z)
        {
            Vector3 result = transform.position + transform.forward * multiply;
            x = result.x;
            y = result.y;
            z = result.z;
        }

        #endregion

        public static ParticleSystem[] initParticleSystem(GameObject model)
        {
            ParticleSystem[] particleSystems = model.GetComponentsInChildren<ParticleSystem>(true);
            startParticleSystem(particleSystems);
            return particleSystems;
        }

        public static void startParticleSystem(ParticleSystem[] particleSystems)
        {
            if (particleSystems == null) return;
            for (int i = 0; i < particleSystems.Length; i++)
            {
                ParticleSystem particleSystem = particleSystems[i];
                particleSystem.Play();
            }
        }

        // public static SkeletonAnimation[] initSkeletonAniamtion(GameObject model)
        // {
        //     SkeletonAnimation[] animations = model.GetComponentsInChildren<SkeletonAnimation>(true);
        //     startSkeletonAniamtion(animations);
        //     return animations;
        // }

        // public static void startSkeletonAniamtion(SkeletonAnimation[] animations)
        // {
        //     if (animations == null) return;
        //     for (int i = 0; i < animations.Length; i++)
        //     {
        //         SkeletonAnimation animator = animations[i];
        //         animator.enabled = true;
        //     }
        // }


        public static Animator[] initAniamtorIndex(GameObject model, bool playAnima, string animaName)
        {
            return initAniamtor(model, playAnima, animaName);
        }

        public static Animator[] initAniamtor(GameObject model, bool playAnima, string animaName)
        {
            Animator[] animators = model.GetComponentsInChildren<Animator>(true);
            startAniamtor(animators, playAnima, animaName);
            return animators;
        }

        public static void startAniamtorIndex(Animator[] animators, bool playAnima, string animaName)
        {
            if (animators == null) return;
            startAniamtor(animators, playAnima, animaName);
        }

        public static void startAniamtor(Animator[] animators, bool playAnima, string animaName)
        {
            if (animators == null) return;
            for (int i = 0; i < animators.Length; i++)
            {
                Animator animator = animators[i];
                animator.enabled = true;
                if (playAnima && animator.gameObject.activeInHierarchy)
                {
                    if (string.IsNullOrEmpty(animaName))
                    {
                        animator.Play(COMMON_ANIMA_NAME, 0, 0);
                    }
                    else
                    {
                        //                      Debug.LogWarning(animator.name + "    animator play:" + animaName);
                        animator.Play(animaName, 0, 0);
                    }
                }
            }
        }

        public static TrailRenderer[] initTrailRenderers(GameObject model)
        {
            TrailRenderer[] trailRenderers = model.GetComponentsInChildren<TrailRenderer>(true);
            startTrailRenderers(trailRenderers);
            return trailRenderers;
        }

        public static void startTrailRenderers(TrailRenderer[] trailRenderers)
        {
            if (trailRenderers == null) return;
            //            Debug.LogError("startTrailRenderers");
            for (int i = 0; i < trailRenderers.Length; i++)
            {
                TrailRenderer trailRenderer = trailRenderers[i];
                trailRenderer.enabled = true;
                trailRenderer.Clear();
            }
        }

        public static Animation[] initAnimationIndex(GameObject model, bool playAnima, string animaName)
        {
            return initAnimation(model, playAnima, animaName);
        }

        public static Animation[] initAnimation(GameObject model, bool playAnima, string animaName)
        {
            Animation[] animations = model.GetComponentsInChildren<Animation>(true);
            startAnimation(animations, playAnima, animaName);
            return animations;
        }

        private const string COMMON_ANIMA_NAME = "play";

        public static void startAnimationIndex(Animation[] animations, bool playAnima, string animaName)
        {
            startAnimation(animations, playAnima, animaName);
        }

        public static void startAnimation(Animation[] animations, bool playAnima, string animaName)
        {
            if (animations == null) return;
            for (int i = 0; i < animations.Length; i++)
            {
                Animation animation = animations[i];
                animation.enabled = true;
                if (playAnima && animation.gameObject.activeInHierarchy)
                {
                    if (string.IsNullOrEmpty(animaName))
                    {
                        animation.Play(COMMON_ANIMA_NAME);
                    }
                    else
                    {
                        animation.Play(animaName);
                    }
                }
            }
        }


        public static void unInitParticleSystem(ParticleSystem[] particleSystems)
        {
            if (particleSystems == null)
            {
                return;
            }

            for (int i = 0; i < particleSystems.Length; i++)
            {
                ParticleSystem particleSystem = particleSystems[i];
                if (particleSystem != null)
                {
                    particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }

        public static void unInitAniamtion(Animation[] animations)
        {
            if (animations == null)
            {
                return;
            }

            for (int i = 0; i < animations.Length; i++)
            {
                Animation animation = animations[i];
                if (animation != null)
                {
                    animation.enabled = false;
                }
            }
        }

        // public static void unInitSkeletonAniamtion(SkeletonAnimation[] animations)
        // {
        //     if (animations == null) return;
        //     for (int i = 0; i < animations.Length; i++)
        //     {
        //         SkeletonAnimation animation = animations[i];
        //         if (animation != null)
        //         {
        //             animation.enabled = false;
        //         }
        //     }
        // }

        public static void unInitTrailRenderers(TrailRenderer[] trailRenderers)
        {
            if (trailRenderers == null) return;
            for (int i = 0; i < trailRenderers.Length; i++)
            {
                TrailRenderer trailRenderer = trailRenderers[i];
                if (trailRenderer != null)
                {
                    trailRenderer.enabled = false;
                }
            }
        }

        public static void unInitAniamtor(Animator[] animators)
        {
            if (animators == null) return;
            for (int i = 0; i < animators.Length; i++)
            {
                Animator animator = animators[i];
                if (animator != null)
                {
                    animator.enabled = false;
                }
            }
        }

        // static List<Graphic> graphics = new List<Graphic>();
        // public static void addSoftMaskable(GameObject go)
        // {
        //     graphics.Clear();
        //     go.GetComponentsInChildren<Graphic>(true, graphics);
        //     //            JQLog.Log("addSoftMaskable:"+ graphics.Count);
        //     for (int i = 0; i < graphics.Count; i++)
        //     {
        //         Graphic graphic = graphics[i];
        //         graphic.gameObject.AddMissingComponent<SoftMaskable>();
        //     }
        // }
        //
        // static List<SoftMaskable> softMaskables = new List<SoftMaskable>();
        // public static void removeSoftMaskable(GameObject go)
        // {
        //     softMaskables.Clear();
        //     go.GetComponentsInChildren<SoftMaskable>(true, softMaskables);
        //     //            JQLog.Log("removeSoftMaskable:" + graphics.Count);
        //     for (int i = 0; i < softMaskables.Count; i++)
        //     {
        //         SoftMaskable softMaskable = softMaskables[i];
        //         Object.Destroy(softMaskable);
        //     }
        // }


        public static void SetPosition_EX(this LineRenderer lineRenderer, int index, float x, float y, float z)
        {
            Vector3 vector3 = CreateVector3(x, y, z);
            lineRenderer.SetPosition(index, vector3);
        }

        public static void CopyPosition_EX(this Transform toTransform, Transform fromTransform)
        {
            if (fromTransform)
            {
                toTransform.position = fromTransform.position;
            }
        }

        static byte[] tempByte = new byte[1024 * 1024 * 1];

        // public static string CompressString(string originalStr)
        // {
        //     string resultStr = null;
        //     try
        //     {
        //         byte[] originalBytes = System.Text.Encoding.UTF8.GetBytes(originalStr);
        //         byte[] resultBytes = CompressByte(originalBytes);
        //         if (resultBytes == null)
        //         {
        //             JQLog.LogError("压缩失败");
        //             return string.Empty;
        //         }
        //
        //         resultStr = Convert.ToBase64String(resultBytes);
        //     }
        //     catch (Exception e)
        //     {
        //         JQLog.LogError(e);
        //     }
        //     return resultStr;
        // }

        // public static string DeCompressString(string originalStr)
        // {
        //     string resultStr = null;
        //     try
        //     {
        //         byte[] originalBytes = Convert.FromBase64String(originalStr);
        //         Array.Clear(tempByte, 0, tempByte.Length);
        //         byte[] resultBytes = DeCompressByte(originalBytes);
        //         if (resultBytes == null)
        //         {
        //             JQLog.LogError("解压失败");
        //             return string.Empty;
        //         }
        //         resultStr = System.Text.Encoding.UTF8.GetString(resultBytes);
        //     }
        //     catch (Exception e)
        //     {
        //         JQLog.LogError(e);
        //     }
        //     return resultStr;
        // }

        // public static byte[] CompressByte(byte[] buffer)
        // {
        //     byte[] result;
        //     using (MemoryStream mNs = new MemoryStream())
        //     {
        //         using (ZOutputStream zs = new ZOutputStream(mNs, zlibConst.Z_DEFAULT_COMPRESSION))
        //         {
        //             zs.Write(buffer, 0, buffer.Length);
        //             zs.finish();
        //         }
        //         result = mNs.ToArray();
        //     }
        //     return result;
        // }

        // public static byte[] DeCompressByte(byte[] buffer)
        // {
        //     byte[] result;
        //     using (MemoryStream outMemoryStream = new MemoryStream())
        //     using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
        //     using (Stream inMemoryStream = new MemoryStream(buffer))
        //     {
        //         CopyMem(inMemoryStream, outZStream);
        //         outZStream.finish();
        //         result = outMemoryStream.ToArray();
        //     }
        //     return result;
        // }

        static byte[] _tempBuffer = new byte[256 * 1024];

        public static void CopyMem(Stream input, Stream outPut)
        {
            Array.Clear(_tempBuffer, 0, _tempBuffer.Length);
            int len;
            while ((len = input.Read(_tempBuffer, 0, _tempBuffer.Length)) > 0)
            {
                outPut.Write(_tempBuffer, 0, len);
            }

            outPut.Flush();
        }

        public static void CopyLocalPosition_EX(this Transform toTransform, Transform fromTransform)
        {
            if (fromTransform)
            {
                toTransform.localPosition = fromTransform.localPosition;
            }
        }

        /// <summary>
        /// 生成滚动列表
        /// </summary>
        /// <param name="go">grid</param>
        /// <param name="renderGO">子物体预制体</param>
        /// /// <param name="mScrollRect">ScrollRect</param>
        /// <param name="CellSize">子物体宽高</param>
        /// <param name="SpacingSize">间隔x,y</param>
        /// <param name="ColumnCount">列数</param>
        /// <param name="dataCount">数据总数量</param>
        /// <param name="onUpdateFunc">更新格子回调</param>
        /// <param name="isVertical">是否是上下拖拽更新</param>
        /// <param name="onCreateFunc">初始化各种组件</param>
        /// <returns></returns>
        public static DynamicScrollingListRenderer CreateDynamicScrollingListRenderer(this GridLayoutGroup go,
            GameObject renderGO, ScrollRect mScrollRect, int dataCount,
            Action<DynamicScrollingItem> onUpdateFunc, bool isVertical = true, Action<DynamicScrollingItem> onCreateFunc = null)
        {
            GridLayoutGroup grid = go;
            grid.enabled = false;
            DynamicScrollingListRenderer dslr = go.gameObject.AddMissingComponent<DynamicScrollingListRenderer>();
            dslr.CellSize = new Vector2(grid.cellSize.x, grid.cellSize.y);
            dslr.SpacingSize = new Vector2(grid.spacing.x, grid.spacing.y);
            dslr.isVertical = isVertical;
            if (isVertical)
            {
                if (grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                    dslr.RowColumnCount = grid.constraintCount;
                else
                    dslr.RowColumnCount = dataCount;
            }
            else
            {
                if (grid.constraint == GridLayoutGroup.Constraint.FixedRowCount)
                    dslr.RowColumnCount = grid.constraintCount;
                else
                    dslr.RowColumnCount = dataCount;
            }

            dslr.RenderGO = renderGO;
            dslr.mScrollRect = mScrollRect;
            dslr.InitRendererList(onCreateFunc, onUpdateFunc);
            dslr.SetData(dataCount);
            return dslr;
        }

        /// <summary>
        /// 滚动列表跳转到指定的下标
        /// </summary>
        /// <param name="DSLR">滚动列表</param>
        /// <param name="index">下标</param>
        /// <param name="delay">时间</param>
        public static void DSLRMoveToTargetByIndex(this DynamicScrollingListRenderer DSLR, int index, float delay = 1)
        {
            if (DSLR.GetDataCount() != 0)
            {
                DSLR.LocateRenderItemAtIndex(index, delay);
            }
            else
            {
                JQLog.LogWarning("暂无设置数据");
            }
        }
    }
}