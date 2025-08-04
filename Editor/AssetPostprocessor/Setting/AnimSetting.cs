using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace JQEditor.tAssetPostprocessor
{
    public class AnimSetting
    {
        private static List<string> need_scale = new List<string>
        {
            "model_50004_attack1",
            "model_50004_idel",
            "model_50004_run",
            "model_50004_skill1",
            "model_50005_attack1",
            "model_50005_idel",
            "model_50005_run",
            "model_50005_skill1",
            "model_50004_low_attack1",
            "model_50004_low_idel",
            "model_50004_low_run",
            "model_50004_low_skill1",
            "model_50005_low_attack1",
            "model_50005_low_idel",
            "model_50005_low_run",
            "model_50005_low_skill1",
            "model_11006_attack1",
            "model_11006_attack2",
            "model_11006_die",
            "model_11006_hit",
            "model_11006_idel",
            "model_11006_run",
            "model_11006_skill1",
            "model_11007_attack1",
            "model_11007_die",
            "model_11007_hit",
            "model_11007_idel",
            "model_11007_run",
            "model_11007_skill1",
            "model_20032_idel",
            "model_20036_idel",
            "model_20037_idel",
            "model_37031_idel",
            "model_37031_run",
            "model_37031_skill1",
            "model_37032_idel",
            "model_37032_run",
            "model_37032_skill",
            "model_15011_attack1",
            "model_15011_die",
            "model_15011_idel",
            "model_15011_run",
            "model_15011_skill1",
            "model_15011_skill2",
            "model_15010_attack1",
            "model_15010_die",
            "model_15010_idel",
            "model_15010_run",
            "model_15010_skill1",
            "model_10013_attack1",
            "model_10013_die",
            "model_10013_idel",
            "model_10013_run",
            "model_10019_attack1",
            "model_10019_die",
            "model_10019_idel",
            "model_10019_run",
            "model_10020_attack1",
            "model_10020_die",
            "model_10020_idel",
            "model_10020_run",
            "model_50009_skill1",
            "model_10022_die",
            "model_10019_die10025",
            "model_11003_skill1",
            "model_11003_skill2",
            "model_20007_skill1",
        };

        public static bool IsNeedScale(string path)
        {
            for (int i = 0; i < need_scale.Count; i++)
            {
                if (path.Contains(need_scale[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static void ApplyAnimSettings(UnityEngine.Object obj)
        {
            //  MethodInfo methodInfo = typeof(AnimationUtility).GetMethod("GetStorageMemorySize", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            //Debug.Log("内存占用：" + EditorUtility.FormatBytes(Profiler.GetRuntimeMemorySize(obj)));
            // Debug.Log("硬盘占用：" + EditorUtility.FormatBytes((int)methodInfo.Invoke(null, new object[] { obj })));
            // List<AnimationClip> animationClipList = new List<AnimationClip>(AnimationUtility.GetAnimationClips(g));
            List<AnimationClip> animationClipList = new List<AnimationClip> { obj as AnimationClip };
            if (animationClipList.Count == 0)
            {
                AnimationClip[] objectList = UnityEngine.Object.FindObjectsOfType(typeof(AnimationClip)) as AnimationClip[];
                animationClipList.AddRange(objectList);
            }

            foreach (AnimationClip theAnimation in animationClipList)
            {
                try
                {
                    //去除scale曲线
                    foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(theAnimation))
                    {
                        string name = theCurveBinding.propertyName.ToLower();
                        if (name.Contains("scale"))
                        {
                            AnimationUtility.SetEditorCurve(theAnimation, theCurveBinding, null);
                        }
                    }

                    //浮点数精度压缩到f3
                    AnimationClipCurveData[] curves = null;
                    curves = AnimationUtility.GetAllCurves(theAnimation);
                    Keyframe key;
                    Keyframe[] keyFrames;
                    for (int ii = 0; ii < curves.Length; ++ii)
                    {
                        AnimationClipCurveData curveDate = curves[ii];
                        if (curveDate.curve == null || curveDate.curve.keys == null)
                        {
                            continue;
                        }

                        keyFrames = curveDate.curve.keys;
                        for (int i = 0; i < keyFrames.Length; i++)
                        {
                            key = keyFrames[i];
                            key.value = float.Parse(key.value.ToString("f3"));
                            key.inTangent = float.Parse(key.inTangent.ToString("f3"));
                            key.outTangent = float.Parse(key.outTangent.ToString("f3"));
                            keyFrames[i] = key;
                        }

                        curveDate.curve.keys = keyFrames;
                        theAnimation.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(string.Format("CompressAnimationClip Failed !!! animationPath : {0} error: {1}", e));
                }
            }
        }
    }
}