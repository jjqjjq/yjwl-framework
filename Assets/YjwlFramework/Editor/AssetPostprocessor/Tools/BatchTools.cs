using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace JQEditor.tAssetPostprocessor
{
    public class BatchTools
    {
        [MenuItem("S3-Tools/批量检查/Effect")]
        static void CheckEffect()
        {
            string effectPath = ProjectTypeTools.effectPath;
            string[] allFilePath = System.IO.Directory.GetFiles(effectPath + "/Fbx", "*.fbx", System.IO.SearchOption.AllDirectories);
            List<string> list = new List<string>();
            for (int i = 0; i < allFilePath.Length; i++)
            {
                //if (allFilePath[i].ToLower().EndsWith(".fbx"))
                //{
                string path = allFilePath[i].Replace(Application.dataPath, "Assets");
                path = path.Replace("\\", "/");

                ModelImporter importer = (ModelImporter)ModelImporter.GetAtPath(path);
                ModelSetting.ApplyEffectModelSettings(importer);
                //}
            }

            allFilePath = System.IO.Directory.GetFiles(effectPath + "/Texture", "*.png", System.IO.SearchOption.AllDirectories);
            list = new List<string>();
            for (int i = 0; i < allFilePath.Length; i++)
            {
                //if (allFilePath[i].ToLower().EndsWith(".png"))
                //{
                string path = allFilePath[i].Replace(Application.dataPath, "Assets");
                path = path.Replace("\\", "/");

                TextureImporter importer = (TextureImporter)ModelImporter.GetAtPath(path);
                TextureSetting.ApplyEffectTextureSettings(importer);
                //}
            }

            Debug.Log("优化完成");
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("检查特效", "检查并设置完毕!", "OK", "");
        }

        [MenuItem("S3-Tools/批量检查/Model")]
        static void CheckModel()
        {
            string effectPath = ProjectTypeTools.soundPath;
            string[] allFilePath = System.IO.Directory.GetFiles(effectPath + "/Fbx", "*.fbx", System.IO.SearchOption.AllDirectories);
            List<string> list = new List<string>();
            for (int i = 0; i < allFilePath.Length; i++)
            {
                //if (allFilePath[i].ToLower().EndsWith(".fbx"))
                //{
                string path = allFilePath[i].Replace(Application.dataPath, "Assets");
                path = path.Replace("\\", "/");

                ModelImporter importer = (ModelImporter)ModelImporter.GetAtPath(path);
                ModelSetting.ApplyFBXModelSettings(importer);

                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                ModelSetting.ApplyExtraExposedTransformPaths(importer, obj);
                //}
            }

            allFilePath = System.IO.Directory.GetFiles(effectPath + "/Texture", "*.png", System.IO.SearchOption.AllDirectories);
            list = new List<string>();
            for (int i = 0; i < allFilePath.Length; i++)
            {
                //if (allFilePath[i].ToLower().EndsWith(".png"))
                //{
                string path = allFilePath[i].Replace(Application.dataPath, "Assets");
                path = path.Replace("\\", "/");

                TextureImporter importer = (TextureImporter)ModelImporter.GetAtPath(path);
                TextureSetting.ApplyModelTextureSettings(importer);
                //}
            }

            Debug.Log("优化完成");
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("检查角色", "检查并设置完毕!", "OK", "");
        }

        [MenuItem("S3-Tools/批量检查/Music")]
        static void CheckMusic()
        {
            string effectPath = ProjectTypeTools.soundPath;
            string[] allFilePath = System.IO.Directory.GetFiles(effectPath, "*", System.IO.SearchOption.AllDirectories);
            List<string> list = new List<string>();
            for (int i = 0; i < allFilePath.Length; i++)
            {
                string tolower = allFilePath[i].ToLower();
                if (tolower.EndsWith(".mp3") || tolower.EndsWith(".wav"))
                {
                    string path = allFilePath[i].Replace(Application.dataPath, "Assets");
                    path = path.Replace("\\", "/");

                    AudioImporter importer = (AudioImporter)ModelImporter.GetAtPath(path);
                    AudioSetting.ApplyMusicAudio(importer);
                }
            }

            Debug.Log("优化完成");
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("检查音乐音效", "检查并设置完毕!", "OK", "");
        }

        [MenuItem("S3-Tools/批量检查/Anim")]
        static void CheckAnim()
        {
            string effectPath = ProjectTypeTools.modelPath + "Anim";
            string[] allFilePath = System.IO.Directory.GetFiles(effectPath, "*.anim", System.IO.SearchOption.AllDirectories);
            List<string> list = new List<string>();
            for (int i = 0; i < allFilePath.Length; i++)
            {
                //string tolower = allFilePath[i].ToLower();
                //if (tolower.EndsWith(".anim"))
                //{
                string path = allFilePath[i].Replace(Application.dataPath, "Assets");
                path = path.Replace("\\", "/");

                AnimationClip anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (anim != null)
                {
                    AnimSetting.ApplyAnimSettings(anim);
                }
                //}
            }

            Debug.Log("优化完成");
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("检查动作", "检查并设置完毕!", "OK", "");
        }
    }
}