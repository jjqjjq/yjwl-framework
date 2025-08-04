using System;
using System.Collections.Generic;
using JQCore.tUtil;
using JQEditor.Build;
using JQCore.Log;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Check
{
    public static class CheckModelTools
    {
        public static void checkUsePackageMaterials(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/Model", checkPackageMaterial, null, endAction);
        }
        
        public static void removeMeshCollider(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/Model", removeMeshCollider, null, endAction);
        }
        
        private static bool removeMeshCollider(string assetPath, GameObject handleGameObject, object obj1)
        {
            MeshCollider[] meshColliders = handleGameObject.GetComponentsInChildren<MeshCollider>(true);
            for (int i = 0; i < meshColliders.Length; i++)
            {
                MeshCollider meshCollider = meshColliders[i];
                GameObject.DestroyImmediate(meshCollider);
            }
            // Avatar[] avatars = handleGameObject.GetComponentsInChildren<Avatar>(true);
            // for (int i = 0; i < avatars.Length; i++)
            // {
            //     Avatar avatar = avatars[i];
            //     GameObject.DestroyImmediate(avatar);
            // }
            Animator[] animators = handleGameObject.GetComponentsInChildren<Animator>(true);
            for (int i = 0; i < animators.Length; i++)
            {
                Animator animator = animators[i];
                GameObject.DestroyImmediate(animator);
            }
            return true;
        }
        
        private static bool checkPackageMaterial(string assetPath, GameObject handleGameObject, object obj1)
        {
            CheckGlobalTools.replaceUsePackageMaterials(handleGameObject);
            return true;
        }
        
        
        //检查设置不对的模型
        //[MenuItem("IrfCheck/Model模型动画默认状态和循环统一设置 %g")]
       


            //
            //            //设置defaultState为stand_left
            //            for (int i = 0; i < animatorStateMachine.states.Count(); i++)
            //            {
            //                animatorStateMachine
            //                ChildAnimatorState childState = animatorStateMachine.states[i];
            //                if (childState.state.name.Contains("stand_left"))
            //                {
            //                    if (animatorStateMachine.defaultState != childState.state)
            //                    {
            //                        HyDebug.LogForrmat("[设置defaultState]:{0}", assetPath);
            //                        animatorStateMachine.defaultState = childState.state;
            //                    }
            //                    break;
            //                }
            //            }
            //
            //            //设置需要loop的状态
            //            foreach (AnimationClip animationClip in animatorController.animationClips)
            //            {
            //                if (animationClip.name.Contains("stand") || animationClip.name.Contains("move") || animationClip.name.Contains("loop"))
            //                {
            //                    AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(animationClip);
            //                    if (!clipSetting.loopTime )
            //                    {
            //                        HyDebug.LogForrmat("[设置AnimationClip.loop]path:{0} name:{1}", assetPath, animationClip.name);
            //                        clipSetting.loopTime = true;
            //                        AnimationUtility.SetAnimationClipSettings(animationClip, clipSetting);
            //                    }
            //                }
            //            }
            //            return true;
            //        }

            //[MenuItem("IrfTemp/Model/模型批处理")]
            public static void checkModel(string name, Action endAction)
        {
            CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}/Model", checkModel, null, endAction);
        }
        
        private static bool checkModel(string assetPath, GameObject handleGameObject, object obj1)
        {
            SpriteRenderer[] spriteRenderers = handleGameObject.GetComponentsInChildren<SpriteRenderer>(true);
            Animator[] animators = handleGameObject.GetComponentsInChildren<Animator>(true);
            //动画处理
            removeUnuseKeyFromAnimation(handleGameObject, animators);
            //替换材质
            changeSpriteMaterial(handleGameObject, spriteRenderers);
            //z处理
            SortSprite(handleGameObject, spriteRenderers);
            return true;
        }

        private static void removeUnuseKeyFromAnimation(GameObject gameObject, Animator[] animatorArr)
        {
            for (int i = 0; i < animatorArr.Length; i++)
            {
                Animator animator = animatorArr[i];
                if (animator.runtimeAnimatorController == null) continue;
                AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
                for (int j = 0; j < clips.Length; j++)
                {
                    AnimationClip clip = clips[j];
                    //if (!clip.name.Contains("head_attack2_down")) continue;
                    EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
                    foreach (EditorCurveBinding binding in bindings)
                    {
                        if (binding.propertyName == "m_LocalPosition.z")
                        {
                            AnimationCurve animationCurve = AnimationUtility.GetEditorCurve(clip, binding);
                            float lastValue = 0;
                            bool canDel = true;
                            for (int k = 0; k < animationCurve.keys.Length; k++)
                            {
                                Keyframe keyframe = animationCurve.keys[k];
                                if (k == 0)
                                {
                                    lastValue = keyframe.value;
                                }
                                else
                                {
                                    if (lastValue != keyframe.value)
                                    {
                                        Debug.LogError("[z值不一样]" + clip.name + ":" + binding.path + "/" + binding.propertyName + ", Keys: " + animationCurve.keys.Length);
                                         //canDel = false;
                                        break;
                                    }
                                }
                            }
                            if (canDel)
                            {
                                Debug.Log("[移除动画帧]" + clip.name + ":" + binding.path + "/" + binding.propertyName + ", Keys: " + animationCurve.keys.Length);
                                while (animationCurve.keys.Length > 0)
                                {
                                    animationCurve.RemoveKey(0);
                                }
                                AnimationUtility.SetEditorCurve(clip, binding, null);
                            }
                        }

                    }
                }
            }
        }

        private static void changeSpriteMaterial(GameObject gameObject, SpriteRenderer[] spriteRendererArr)
        {
            Material spriteDiffuseMat = Resources.Load("Shaders/SpriteDiffuse") as Material;
            for (int i = 0; i < spriteRendererArr.Length; i++)
            {
                SpriteRenderer spriteRenderer = spriteRendererArr[i];
                if (spriteRenderer.gameObject.name != "shadow")
                {
                    if (spriteRenderer.sharedMaterial.shader.name.Equals("Sprites/Default"))
                    {
                        Debug.LogFormat("[Sprites/Default -> Sprites/Diffuse]");
                        spriteRenderer.sharedMaterial = spriteDiffuseMat;
                    }
                }

            }
        }

        private static void SortSprite(GameObject obj, Renderer[] rendererArr)
        {
            bool hadNotZeroZ = false;
            for (int i = 0; i < rendererArr.Length; i++)
            {
                Renderer spriteRenderer = rendererArr[i];
                if (spriteRenderer.gameObject.name == "shadow") continue;
                Vector3 pos = spriteRenderer.transform.position;
                if (pos.z != 0)
                {
                    hadNotZeroZ = true;
                }
            }
            if (hadNotZeroZ)
            {
                List<Renderer> sortInfoList = new List<Renderer>(rendererArr);
                sortInfoList.Sort(CompareZ);
                for (int i = 0; i < sortInfoList.Count; i++)
                {
                    Renderer spriteRenderer = sortInfoList[i];
                    if (spriteRenderer.gameObject.name == "shadow") continue;
                    spriteRenderer.sortingOrder = i;
                }
                SetAllChildToZeroZ(obj.transform);
            }
        }

        private static int CompareZ(Renderer sortInfoA, Renderer sortInfoB)
        {
            if (sortInfoA.transform.position.z > sortInfoB.transform.position.z)
            {
                return -1;
            }
            if (sortInfoA.transform.position.z < sortInfoB.transform.position.z)
            {
                return 1;
            }
            return 0;
        }

        private static void SetAllChildToZeroZ(Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.position = new Vector3(child.position.x, child.position.y, 0);
                if (child.childCount > 0)
                {
                    SetAllChildToZeroZ(child);
                }
            }
        }
    }
}
