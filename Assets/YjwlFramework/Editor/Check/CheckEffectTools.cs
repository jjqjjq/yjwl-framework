using System;
using System.Collections.Generic;
using System.Linq;
using JQCore.tUtil;
using JQEditor.Build;
using JQCore.Log;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using YooAsset;
using YooAsset.Editor;
using Object = UnityEngine.Object;

namespace JQEditor.Check
{
    public static class CheckEffectTools
    {
        
        public static void checkUsePackageMaterials(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/Effect", checkPackageMaterial, null, endAction);
            Debug.Log("xxx");
        }
        
        private static bool checkPackageMaterial(string assetPath, GameObject handleGameObject, object obj1)
        {
            CheckGlobalTools.replaceUsePackageMaterials(handleGameObject);
            return true;
        }

        public static void handleAnimator(string name, Action endAction)
        {
            CheckCommonTools.Search<AnimatorController>(name + "_动画名称", $"Assets/{PathUtil.RES_FOLDER}/Effect", changeAnimation, null, endAction, true, ".controller", "t:AnimatorController");
            CheckCommonTools.Search<GameObject>(name + "_动画帧", $"Assets/{PathUtil.RES_FOLDER}/Effect", checkEffect, null, endAction);
        }
        private static bool changeAnimation(string assetPath, AnimatorController animatorController, object obj1)
        {
            if (animatorController.layers.Length > 0)
            {
                AnimatorStateMachine animatorStateMachine = animatorController.layers[0].stateMachine;
                if (animatorStateMachine.defaultState.name != "play")
                {
                    animatorStateMachine.defaultState.name = "play";
                    Debug.LogFormat($"[特效优化]设置默认动画名称为play:{0}", assetPath);
                }
            }
            return true;
        }

        private static bool checkEffect(string assetPath, GameObject handleGameObject, object obj1)
        {
            checkAnimator(handleGameObject);
            checkUnusedBehavior(handleGameObject);
            return true;
        }

        private static void checkAnimator(GameObject handleGameObject)
        {
            Animator[] animatorArr = handleGameObject.GetComponentsInChildren<Animator>(true);
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
                        if (binding.propertyName == "m_IsActive")
                        {
                            AnimationCurve animationCurve = AnimationUtility.GetEditorCurve(clip, binding);
                            Debug.LogError("[动画内存在设置Active]" + clip.name + ":" + binding.path + "/" + binding.propertyName + ", Keys: " + animationCurve.keys.Length);
                        }
                    }
                }
            }
        }

        private static void checkUnusedBehavior(GameObject handleGameObject)
        {
            Collider[] colliders = handleGameObject.GetComponentsInChildren<Collider>(true);
            foreach (Collider collider in colliders)
            {
                Debug.LogError("[特效内无用组件-Collider]" + collider.name + ":" + handleGameObject);
            }
            Animation[] animations = handleGameObject.GetComponentsInChildren<Animation>(true);
            foreach (Animation animation in animations)
            {
                Debug.LogError("[特效内无用组件-Animation]" + animation.name + ":" + handleGameObject);
            }
            Animator[] animators = handleGameObject.GetComponentsInChildren<Animator>(true);
            foreach (Animator animator in animators)
            {
                if (animator.runtimeAnimatorController == null)
                {
                    Debug.LogError("[特效内无用组件-Animator 中animatorContoller为空]" + animator.name + ":" + handleGameObject);
                }
            }
            MeshFilter[] meshFilters = handleGameObject.GetComponentsInChildren<MeshFilter>(true);
            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (meshFilter.sharedMesh == null)
                {
                    Debug.LogError("[特效内无用组件-MeshFilter 中Mesh为空]" + meshFilter.name + ":" + handleGameObject);
                }
            }
        }


        //[MenuItem("IrfCheck/粒子特效性能优化")]
        public static void CheckParticleEffect(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/Effect", checkOneParticle, null, endAction);
        }

        private static bool checkOneParticle(string assetPath, GameObject obj, object obj1)
        {
            ParticleSystem[] particleSystems = obj.GetComponentsInChildren<ParticleSystem>(true);
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                ParticleSystem.CollisionModule collisionModule = particleSystem.collision;
                if (collisionModule.enabled)
                {
                    Debug.LogFormat("[关闭collision模组]path:{0} obj:{1}", assetPath, particleSystem.name);
                    collisionModule.enabled = false;
                }

                ParticleSystem.LightsModule lightsModule = particleSystem.lights;
                if (lightsModule.enabled)
                {
                    Debug.LogFormat("[关闭light模组]path:{0} obj:{1}", assetPath, particleSystem.name);
                    lightsModule.enabled = false;
                }
            }

            Rigidbody[] rigidbodies = obj.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rigidbody in rigidbodies)
            {
                Debug.LogFormat("[移除Rigidbody]path:{0} obj:{1}", assetPath, rigidbody.name);
                Object.DestroyImmediate(rigidbody);
            }

            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                Debug.LogFormat("[移除Collider]path:{0} obj:{1}", assetPath, collider.name);
                Object.DestroyImmediate(collider);
            }

            return true;
        }


        //[MenuItem("IrfCheck/Particle/xxxx -> Mobile/Particle/xxxx")]
        public static void ChangeShader(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/Effect_al", changeOneShader, null, endAction);
        }

        public static bool changeOneShader(string assetPath, GameObject obj, object obj1)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                {
                    Debug.LogFormat("[renderer null]path:{0} obj:{1}", assetPath, renderer.name);
                    continue;
                }
                if (renderer.sharedMaterial == null)
                {
                    Debug.LogFormat("[sharedMaterial null]path:{0} obj:{1}", assetPath, renderer.name);
                    continue;
                }
                Shader shader = renderer.sharedMaterial.shader;
                switch (shader.name)
                {
                    case "Particles/Additive":
                        changeShader(assetPath, renderer, "Mobile/Particles/Additive");
                        break;
                    case "Particles/Additive (Soft)":
                        changeShader(assetPath, renderer, "Mobile/Particles/Additive");
                        break;
                    case "Particles/Alpha Blended":
                        changeShader(assetPath, renderer, "Mobile/Particles/Alpha Blended");
                        break;
                    case "Particles/VertexLit Blended":
                        changeShader(assetPath, renderer, "Mobile/Particles/VertexLit Blended");
                        break;
                    case "Particles/Multiply":
                        changeShader(assetPath, renderer, "Mobile/Particles/Multiply");
                        break;
                }
            }
            return true;
        }

        private static void changeShader(string assetPath, Renderer renderer, string newShaderName)
        {
            Debug.LogFormat("[替换Shader]path:{0} obj:{1}", assetPath, renderer.name);
            Texture texture = renderer.sharedMaterial.GetTexture("_MainTex");
            renderer.sharedMaterial.shader = Shader.Find(newShaderName);
            renderer.sharedMaterial.SetTexture("_MainTex", texture);
        }
    }
}
