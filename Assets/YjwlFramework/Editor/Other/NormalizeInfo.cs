using System.Collections.Generic;
using System.Linq;
using System.Text;
using JQCore.tFileSystem;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Other
{
    public class NormalizeInfo
    {
        public delegate bool MoveInCondition(Object obj, string path);

        private readonly string _folderName;

        private readonly MoveInCondition _moveInConditionFun;
        private readonly bool _needRename = false;
        private string _folderFullName;
        private string _renameFormat;
        private string _renameFormat2;

        public NormalizeInfo(string folderName, MoveInCondition moveInCondition, bool needRename = true)
        {
            _needRename = needRename;
            _folderName = folderName;
            _moveInConditionFun = moveInCondition;
        }

        public void setParam(string parentFolder, string renameFormat, string renameFormat2)
        {
            _folderFullName = string.Concat(parentFolder, "/", _folderName);
            if (!AssetDatabase.IsValidFolder(_folderFullName))
            {
                AssetDatabase.CreateFolder(parentFolder, _folderName);
            }
            _renameFormat = renameFormat;
            _renameFormat2 = renameFormat2;
        }

        public bool TryMoveIn(Object obj, string path)
        {
            bool canMoveIn = _moveInConditionFun(obj, path);
            if (canMoveIn)
            {
                string fromPath = AssetDatabase.GetAssetPath(obj);
                string parentPath = JQFileUtil.getParentFolderPath(fromPath);
                if (parentPath.Equals(_folderFullName))
                {
                    //HyDebug.Log("同目录");
                    return false;
                }
                string fileName = fromPath.Substring(fromPath.LastIndexOf('/'));
                StringBuilder irfStringBuilder = new StringBuilder();
                string toPath = irfStringBuilder.Append(_folderFullName).Append(fileName).ToString();
                //HyDebug.Log(fromPath + "  -  " + toPath);
                if (!fromPath.Equals(toPath))
                {
                    AssetDatabase.MoveAsset(fromPath, toPath);
                }
            }
            return canMoveIn;
        }

        public void SortAndReName()
        {
            if (!_needRename) return;
            string[] lookFor = new string[] {_folderFullName};
            string[] guidArr = AssetDatabase.FindAssets("", lookFor);
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            foreach (string guid in guidArr)
            {
                //HyDebug.Log(guid);
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                //Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                //HyDebug.Log(obj);
                if (AssetDatabase.IsValidFolder(assetPath)) continue;
                string extens = JQFileUtil.getExtension(assetPath);
                List<string> fileList = null;
                if (!dic.ContainsKey(extens))
                {
                    fileList = new List<string>();
                    dic[extens] = fileList;
                }
                else
                {
                    fileList = dic[extens];
                }
                fileList.Add(assetPath);
            }
            foreach (KeyValuePair<string, List<string>> keyValuePair in dic)
            {
                string extension = keyValuePair.Key;
                List<string> oldPathList = keyValuePair.Value;
                List<string> tempPathList = new List<string>();
                List<string> newPathList = new List<string>();
                List<string> newPath2List = new List<string>();
                oldPathList = new List<string>(oldPathList.Distinct());
                oldPathList.Sort(SpriteNameCompare);
                for (int i = 0; i < oldPathList.Count; i++)
                {
                    string oldPath = oldPathList[i];
                    string folderFullName = _folderFullName;
                    string fileName = JQFileUtil.getCurrFolderOrFileName(oldPath, false);
                    if (oldPath.Contains(_folderFullName))
                    {
                        string subFolderName = oldPath.Substring(0, oldPath.LastIndexOf('/'));
                        folderFullName = subFolderName;
                    }
                    tempPathList.Add(folderFullName + "/" + "temp" + i + "." + extension);
                    newPathList.Add(folderFullName + "/" + string.Format(_renameFormat, fileName) + "." + extension);
                    newPath2List.Add(folderFullName + "/" + string.Format(_renameFormat2, i,  fileName) + "." + extension);
                }
                for (int i = 0; i < oldPathList.Count; i++)
                {
                    AssetDatabase.MoveAsset(oldPathList[i], tempPathList[i]);
                }
                for (int i = 0; i < oldPathList.Count; i++)
                {
                    string newPath = newPathList[i];
                    string newPath2 = newPath2List[i];
                    string error =  AssetDatabase.MoveAsset(tempPathList[i], newPath);
                    if (!string.IsNullOrEmpty(error))
                    {
                        AssetDatabase.MoveAsset(tempPathList[i], newPath2);
                    }
//                    Object mainAssets = AssetDatabase.LoadAssetAtPath<Object>(newPath);
                    //处理Sprite
//                    if (mainAssets is Texture2D)
//                    {
//                        TextureImporter importer = AssetImporter.GetAtPath(newPath) as TextureImporter;
//                        SpriteMetaData[] metaDatas = importer.spritesheet;
//                        importer.spritePackingTag = "";
//                        for (int j = 0; j < metaDatas.Length; j++)
//                        {
//                            int index = 0;
//                            SpriteMetaData subAssets = metaDatas[j];
//                            subAssets.name = FileUtil.getCurrFolderOrFileName(newPath, false) + "_" + index;
//                            index++;
//                        }
//                        importer.SaveAndReimport();
//                    }
                }
            }

        }

        public int SpriteNameCompare(string s1, string s2)
        {
            if (s1.Length > s2.Length) return 1;
            if (s1.Length < s2.Length) return -1;
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] > s2[i]) return 1;
                if (s1[i] < s2[i]) return -1;
            }
            return 0;
        }
    }
}