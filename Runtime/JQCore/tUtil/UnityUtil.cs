using System.Collections.Generic;
using DG.Tweening;
using JQCore;
using JQCore.tLog;
using JQCore.tUtil;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using EventSystem = UnityEngine.EventSystems.EventSystem;
using Object = UnityEngine.Object;

namespace JQCore.tUtil
{
    public static class UnityUtil
    {
        private static bool _isEditLog;

        public static void setEditLog(bool isEditLog)
        {
            _isEditLog = isEditLog;
        }

        #region Base

        public static Color RGBMultiplied(this Color color, float multiplier)
        {
            return new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier, color.a);
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

        #endregion


        #region GameObject

        public static GameObject StaticFind_EX(string path)
        {
            return GameObject.Find(path);
        }

        public static GameObject NewGameObject(string name)
        {
            return new GameObject(name);
        }

        public static void Destroy(GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }

        /// <summary>
        /// 移除GameObject组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="go"></param>
        public static void RemoveComponent<T>(this GameObject go) where T : MonoBehaviour
        {
            T componet = go.GetComponent<T>();
            if (componet != null)
                GameObject.Destroy(componet);
        }

        /// <summary>
        /// 查找某路径的GameObject 的 Component T ，如果子GameObject存在，一定会返回 UIWidgetContainer 
        /// Create By D.S.Qiu
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="parentGo">查找的根节点</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static T FindInChild<T>(this GameObject parentGo, string path = null) where T : Component
        {
            if (string.IsNullOrEmpty(path))
            {
                return parentGo.GetComponent<T>();
            }

            Transform trans = parentGo.transform.Find(path);
            if (trans == null)
                return null;
            else
                return trans.GetComponent<T>();
        }

        public static Transform FindChild(this Transform parent, string path)
        {
            Transform trans = parent;
            if (trans.Find(path))
                return trans.Find(path);
            else
                return null;
        }

        public static GameObject FindChild(this GameObject parent, string path)
        {
            Transform trans = parent.transform;
            Transform childTrans = trans.Find(path);
            if (childTrans)
                return childTrans.gameObject;
            else
                return null;
        }
        // <summary>
        // Extension for the game object that checks to see if the component already exists before adding a new one.
        //If the component is already present it will be returned instead.
        // </summary>

        public static T AddMissingComponent<T>(this GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }

            return comp;
        }

        public static GameObject SetActive_EX(this GameObject gameObject, bool activity)
        {
            if (_isEditLog)
            {
                string fullPath = PathUtil.getFullPath(gameObject);
                if (!fullPath.Contains("HUD") && !fullPath.Contains("NPCDialogueBubble"))
                {
                    JQLog.LogError($"SetActive:{fullPath}   {activity} ");
                }
            }

            if (gameObject.activeSelf != activity)
            {
                gameObject.SetActive(activity);
            }

            return gameObject;
        }

        public static T AddComponent_EX<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.AddComponent<T>();
        }

        #endregion

        #region UGUI

        public static void RemoveAllListeners_EX(this Button button)
        {
            button.onClick.RemoveAllListeners();
        }

        public static void RemoveAllListeners_EX(this Toggle toggle)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }

        public static void RemoveAllListeners_EX(this InputField inputField)
        {
            inputField.onValueChanged.RemoveAllListeners();
        }

        public static void RemoveAllListeners_EX(this Slider slider)
        {
            slider.onValueChanged.RemoveAllListeners();
        }


        public static void AddListener_EX(this Button button, UnityAction action, float secondClickTime = 0f,
            string sound = "0")
        {
            button.onClick.AddListener(action);
            if (sound != "-1")
            {
                // button.onClick.AddListener(() => { HyXLua.CallLuaOO(HyXLua.MAIN_LUA, "CCL_PlayBtnSound", 1, sound); });
            }
        }

        public static void AddListener_EX(this Slider slider, UnityAction<float> action, string sound = "0")
        {
            slider.onValueChanged.AddListener(action);
        }

        public static void AddListener_EX(this InputField inputField, UnityAction<string> action, string sound = "0")
        {
            inputField.onValueChanged.AddListener(action);
        }

        public static void AddListener_EX(this Toggle toggle, UnityAction<bool> action, string sound = "0")
        {
            toggle.onValueChanged.AddListener(action);
            toggle.onValueChanged.AddListener((b) =>
                {
                    if (b)
                    {
                        if (toggle.gameObject == EventSystem.current.currentSelectedGameObject)
                        {
                            JQLog.Log("播放按钮音效");
                            // HyXLua.CallLuaOO(HyXLua.MAIN_LUA, "CCL_PlayBtnSound", 2, sound);
                        }
                    }
                }
            );
        }

        #endregion


        #region Camera

        public static void WorldToViewportPoint_EX(this Camera camera, float inX, float inY, float inZ, out float x,
            out float y, out float z)
        {
            Vector3 pos = camera.WorldToViewportPoint(CreateVector3(inX, inY, inZ));
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }

        public static Vector3 WorldToScreenPoint_EX(this Camera camera, Vector3 inV3)
        {
            return camera.WorldToScreenPoint(inV3);
        }

        public static void WorldToScreenPoint_EX(this Camera camera, float inX, float inY, float inZ, out float x,
            out float y, out float z)
        {
            Vector3 pos = camera.WorldToScreenPoint(CreateVector3(inX, inY, inZ));
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }

        public static void SetMSAA_EX(this Camera camera, bool allowMSAA)
        {
            camera.allowMSAA = allowMSAA;
        }

        #endregion

        #region Transform/RectTransform

        public static RectTransform SetRectTransformAnchoredPosition3D_EX(this RectTransform transform, float x,
            float y,
            float z)
        {
            transform.anchoredPosition3D = CreateVector3(x, y, z);
            return transform;
        }

        //设置宽高
        public static Transform SetSizeDelta_EX(this RectTransform rectTransform, float x, float y)
        {
            rectTransform.sizeDelta = CreateVector2(x, y);
            return rectTransform;
        }

        public static T AddComponent_EX<T>(this Transform transform) where T : Component
        {
            return transform.gameObject.AddComponent_EX<T>();
        }

        public static void SetOffsetMax_EX(this RectTransform transform, float x, float y)
        {
            transform.offsetMax = CreateVector2(x, y);
        }

        public static void SetOffsetMin_EX(this RectTransform transform, float x, float y)
        {
            transform.offsetMin = CreateVector2(x, y);
        }

        public static void SetAnchorMax_EX(this RectTransform transform, float x, float y)
        {
            transform.anchorMax = CreateVector2(x, y);
        }

        public static void SetAnchorMin_EX(this RectTransform transform, float x, float y)
        {
            transform.anchorMin = CreateVector2(x, y);
        }


        public static Transform SetActive_EX(this Transform transform, bool activity)
        {
            transform.gameObject.SetActive_EX(activity);
            return transform;
        }


        public static Transform Find_EX(this Transform transform, string path)
        {
            //            GameObject gameObject = transform.gameObject;
            //            if (gameObject.transform.parent != null)
            //            {
            //                JQLog.LogError($"Find:{gameObject.transform.parent.name}/{gameObject.name}   {path} ");
            //            }
            //            else
            //            {
            //                JQLog.LogError($"Find:{gameObject.name}   {path} ");
            //            }
            return transform.Find(path);
        }

        // transform.localScale 
        public static void GetLocalScale_EX(this Transform transform, out float x, out float y, out float z)
        {
            Vector3 scale = transform.localScale;
            x = scale.x;
            y = scale.y;
            z = scale.z;
        }

        // transform.localScale = 
        public static Transform SetLocalScale_EX(this Transform transform, float x, float y, float z)
        {
            Vector3 vector3 = CreateVector3(x, y, z);
            if (_isEditLog)
            {
                string fullPath = PathUtil.getFullPath(transform.gameObject);
                JQLog.LogError($"SetLocalScale :{fullPath}   x:{x}  y:{y}  z:{z}");
            }

            transform.localScale = vector3;
            return transform;
        }
        
        public static Transform SetLocalScale_EX(this Transform transform, Vector3 vector3)
        {
            if (_isEditLog)
            {
                string fullPath = PathUtil.getFullPath(transform.gameObject);
                JQLog.LogError($"SetLocalScale :{fullPath}   {vector3} ");
            }

            transform.localScale = vector3;
            return transform;
        }

        public static Transform SetLocalScale_EX(this Transform transform, float3 a)
        {
            if (_isEditLog)
            {
                string fullPath = PathUtil.getFullPath(transform.gameObject);
                JQLog.LogError($"SetLocalScale :{fullPath}   scale:{a}");
            }

            transform.localScale = a;
            return transform;
        }

        public static Transform SetLocalScaleFromTrans_EX(this Transform transform, Transform fromTrans)
        {
            transform.localScale = fromTrans.localScale;
            return transform;
        }

        public static Transform SetLocalScaleMultiply_EX(this Transform transform, float multiplyX, float multiplyY,
            float multiplyZ)
        {
            Vector3 localScale = transform.localScale;
            transform.localScale =
                CreateVector3(multiplyX * localScale.x, multiplyY * localScale.y, multiplyZ * localScale.z);
            return transform;
        }

        public static void SetParentFull_EX(this Transform transform, Transform parent)
        {
            RectTransform rectTransform = transform.GetComponent<RectTransform>();
            rectTransform.SetParent(parent);
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        public static Transform GetRootTransform(this Transform transform)
        {
            while (transform.parent!=null)
            {
                transform = transform.parent;
            }
            return transform;
        }

        // transform.SetParent(parent)
        public static Transform SetParent_EX(this Transform transform, Transform parent)
        {
            if (_isEditLog)
            {
                if (parent != null)
                {
                    string fullPath = PathUtil.getFullPath(parent.gameObject);
                    JQLog.LogError($"SetParent:{transform.name}   {fullPath}");
                }
                else
                {
                    JQLog.LogError($"SetParent:{transform.name}   null");
                }
            }


            if (transform.parent != parent)
            {
                transform.SetParent(parent);
            }

            return transform;
        }

        // transform.position = 
        public static Transform SetPosOffset_EX(this Transform transform, float x, float y, float z)
        {
            transform.position = transform.position + CreateVector3(x, y, z);
            return transform;
        }

        public static Transform SetPos_EX(this Transform transform, float x, float y, float z)
        {
            transform.position = CreateVector3(x, y, z);
            return transform;
        }

        // transform.localPosition = 
        public static Transform SetLocalPos_EX(this Transform transform, float x, float y, float z)
        {
            transform.localPosition = CreateVector3(x, y, z);
            return transform;
        }

        public static Transform SetLocalPos_EX(this Transform transform, float3 pos)
        {
            transform.localPosition = pos;
            return transform;
        }


        // transform.rotation = 
        public static Transform SetRotation_EX(this Transform transform, float x, float y, float z)
        {
            transform.rotation = Quaternion.Euler(x, y, z);
            return transform;
        }

        public static void SelfRotation(this Transform trans, float x, float y, float z, float speed)
        {
            trans.Rotate(
                new Vector3(x * speed * Time.deltaTime, y * speed * Time.deltaTime, z * speed * Time.deltaTime));
        }

        // transform.localRotation = 
        public static Transform SetLocalRotation_EX(this Transform transform, float x, float y, float z)
        {
            // Log.Error($"name: {transform.name} rotate:{x} {y} {z}");
            transform.localRotation = Quaternion.Euler(x, y, z);
            return transform;
        }

        public static Transform SetLocalRotation_EX(this Transform transform, quaternion q)
        {
            // Log.Error($"name: {transform.name} rotate:{x} {y} {z}");
            transform.localRotation = q;
            return transform;
        }

        // transform.eulerAngles
        public static void GetEulerAngles_EX(this Transform transform, out float x, out float y, out float z)
        {
            Vector3 angle = transform.eulerAngles;
            x = angle.x;
            y = angle.y;
            z = angle.z;
        }

        // transform.localEulerAngles
        public static void GetLocalEulerAngles_EX(this Transform transform, out float x, out float y, out float z)
        {
            Vector3 angle = transform.localEulerAngles;
            x = angle.x;
            y = angle.y;
            z = angle.z;
        }

        public static RectTransform SetRectTransformAnchoredPosition3D_EX(this Transform transform, float x, float y,
            float z)
        {
            RectTransform rectTransform = transform as RectTransform;
            rectTransform.anchoredPosition3D = CreateVector3(x, y, z);
            return rectTransform;
        }

        public static RectTransform SetEulerAngles_EX(this Transform transform, float x, float y, float z)
        {
            RectTransform rectTransform = transform as RectTransform;
            rectTransform.eulerAngles = new Vector3(x, y, z);
            return rectTransform;
        }


        public static void SetPivot_EX(this RectTransform rectTransform, float x, float y)
        {
            rectTransform.pivot = CreateVector2(x, y);
        }

        public static RectTransform toRect(this Transform transform)
        {
            return transform as RectTransform;
        }

        #endregion

        #region Animation/Animator

        private const string COMMON_ANIMA_NAME = "play";

        public static void PlayIndex_EX(this Animation animation, string animationName)
        {
            animation.Play_EX(animationName);
        }

        public static void Play_EX(this Animation animation, string animationName)
        {
            animation.Play(animationName);
        }

        public static void Stop_EX(this Animation animation)
        {
            animation.Stop();
        }

        public static void PlayIndex_EX(this Animator animator, string animationName, int layer, float normalizeTime)
        {
            // 查Animator.GotoState: State could not be found打开
            //            Debug.Log($"{JQLog.getFullPath(animator.gameObject)}     animName:{animationName}");
            animator.Play_EX(animationName, layer, normalizeTime);
        }

        public static void PlayDefault_EX(this Animator animator, int layer, float normalizeTime)
        {
            animator.enabled = true;
            if (animator.gameObject.activeInHierarchy)
            {
                animator.Play(COMMON_ANIMA_NAME, layer, normalizeTime);
            }
        }

        public static void Play_EX(this Animator animator, string animationName, int layer = -1,
            float normalizeTime = 0)
        {
            animator.enabled = true;
            if (animator.gameObject.activeInHierarchy)
            {
                animator.Play(animationName, layer, normalizeTime);
            }
        }

        public static void CrossFade_EX(this Animator animator, string animationName, int layer, float duration)
        {
            animator.enabled = true;
            if (animator.gameObject.activeInHierarchy)
            {
                animator.CrossFade(animationName, duration, layer);
            }
        }

        public static void Stop_EX(this Animator animator)
        {
            animator.enabled = false;
        }

        #endregion

        public static void SetQualityLevel(int level)
        {
            
            int currQualityLevel = QualitySettings.GetQualityLevel();
            if (currQualityLevel != level)
            {
                JQLog.Log($"WX modelLevel change {currQualityLevel} -> {level}");
                QualitySettings.SetQualityLevel(level, false);
            }
        }
        
        public static RenderTexture GetTemporary_EX(string name, int width, int height, int depthBuffer, int msaa)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, depthBuffer);
#if UNITY_EDITOR

            if (!string.IsNullOrEmpty(name))
            {
                renderTexture.name = Sys.stringBuilder.Append("TempRenderTexture_").Append(name).Gap().Append(width)
                    .Append('*')
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
    }
}