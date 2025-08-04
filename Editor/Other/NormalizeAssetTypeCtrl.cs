using System.Collections.Generic;
using System.IO;
using System.Linq;
using JQCore.tFileSystem;
using JQCore.tLog;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Other
{
    public class NormalizeAssetTypeCtrl
    {
        private readonly List<NormalizeInfo> _infoList = new List<NormalizeInfo>();
        private readonly string _type;

        public NormalizeAssetTypeCtrl(string type, List<NormalizeInfo> infoList)
        {
            _type = type;
            _infoList = infoList;
        }

        public void Do()
        {
            Object[] objects = Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets);
            int count = 0;
            for (int i = 0; i < objects.Length; i++)
            {
                Object obj = objects[i];
                if (obj is GameObject)
                {
                    count++;
                    handleOneAsset(obj, _infoList);
                }
            }
            AssetDatabase.Refresh();
            JQLog.Log(count + " done!");
        }


        private void handleOneAsset(Object obj, List<NormalizeInfo> infoList)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            //HyDebug.Log("prefab:" + path);
            string folderFullName = JQFileUtil.getParentFolderPath(path);
            //HyDebug.Log(folderFullName);
            string folderName = JQFileUtil.getCurrFolderOrFileName(folderFullName);
            //HyDebug.Log(folderName);
            string[] lookFor = new string[] {folderFullName};
            string[] guidArr = AssetDatabase.FindAssets("", lookFor);
            List<string> guidList = new List<string>(guidArr);
            guidList = new List<string>(guidList.Distinct());
            foreach (string guid in guidList) //资源文件归类
            {
                string subAssetPath = AssetDatabase.GUIDToAssetPath(guid);
                //HyDebug.Log("guid:" + guid + " path:" + subAssetPath);
                if (subAssetPath.Contains("Temp"))continue;
                if (AssetDatabase.IsValidFolder(subAssetPath)) continue;
                Object subAsset = AssetDatabase.LoadAssetAtPath<Object>(subAssetPath);
                for (int i = 0; i < infoList.Count; i++)
                {
                    NormalizeInfo normalizeInfo = infoList[i];
                    normalizeInfo.setParam(folderFullName, _type + "_" + folderName + "_{0}", _type + "_" + folderName + "_{0}_{1}");
                    if (normalizeInfo.TryMoveIn(subAsset, subAssetPath))
                    {
                        break;
                    }
                }
            }
            AssetDatabase.Refresh();
            for (int i = 0; i < infoList.Count; i++)
            {
                NormalizeInfo normalizeInfo = infoList[i];
                normalizeInfo.SortAndReName();
            }
            removeEmptyFolder(folderFullName, folderName);
            AssetDatabase.MoveAsset(path, folderFullName + "/" + _type + "_" + folderName + ".prefab");
        }

        private void removeEmptyFolder(string folderFullName, string folderName)
        {
            string[] lookFor = new string[] {folderFullName};
            string[] guidArr = AssetDatabase.FindAssets("", lookFor);
            List<string> guidList = new List<string>(guidArr);
            guidList = new List<string>(guidList.Distinct());
            foreach (string guid in guidList) //资源文件归类
            {
                string subAssetPath = AssetDatabase.GUIDToAssetPath(guid);
                JQLog.Log("guid:" + guid + " path:" + subAssetPath);
                if (AssetDatabase.IsValidFolder(subAssetPath))
                {
                    if (subAssetPath.StartsWith(folderFullName + "/" + folderName + "_") || subAssetPath.StartsWith(folderFullName + "/#")) continue;
                    string[] childs = Directory.GetFileSystemEntries(subAssetPath);
                    if (childs.Length <= 0)
                    {
                        JQFileUtil.DeleteDirectory(subAssetPath);
                    }
                }
            }
        }
    }
}