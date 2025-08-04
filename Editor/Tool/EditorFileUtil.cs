using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JQCore.tFileSystem;
using JQCore.tLog;
using JQCore.tUtil;
using UnityEditor;

namespace JQEditor.Tool
{
    public class EditorFileUtil
    {

        public static void RemoveEmptyFolder(string folderFullName)
        {
            string[] lookFor = new string[] { folderFullName };
            string[] guidArr = AssetDatabase.FindAssets("", lookFor);
            List<string> guidList = new List<string>(guidArr);
            guidList = new List<string>(guidList.Distinct());
            foreach (string guid in guidList) //资源文件归类
            {
                string subAssetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(subAssetPath))
                {
                    string[] childs = Directory.GetFileSystemEntries(subAssetPath);
                    if (childs.Length <= 0)
                    {
                        UnityEditor.FileUtil.DeleteFileOrDirectory(subAssetPath);
                    }
                }
            }
        }

        //取得选定文件
        public static List<string> GetSelectAssetl(Predicate<string> predicate)
        {
            List<string> pathList = new List<string>();
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof (UnityEngine.Object), SelectionMode.Assets);
            if (selection.Length > 0)
            {
                string path = AssetDatabase.GetAssetPath(selection[0]);
                JQLog.Log(path);
                JQFileUtil.getAllFile(ref pathList, path, predicate);
            }
            return pathList;
        }
        public static List<string> GetAllAssetl(Predicate<string> predicate)
        {
            List<string> pathList = new List<string>();
            JQFileUtil.getAllFile(ref pathList, $"Assets/{PathUtil.RES_FOLDER}/", predicate);
            return pathList;
        }
    }
}