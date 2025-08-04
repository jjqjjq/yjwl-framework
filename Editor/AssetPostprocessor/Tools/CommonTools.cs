using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

namespace JQEditor.tAssetPostprocessor
{
    public class CommonTools
    {
        public static string[] ChangeListToArray(List<string> mounts)
        {
            int len = mounts.Count;
            string[] s = new string[len];
            for (int i = 0; i < len; i++)
            {
                s[i] = mounts[i];
            }

            return s;
        }

        //获取fullPath目录的上step层目录
        public static string getParentPath(string fullPath, int step)
        {
            string findPath = "";
            int nowStep = 0;
            while (step > 0)
            {
                nowStep = nowStep + 1;
                int index = fullPath.LastIndexOf("/");
                Debug.Log(fullPath.Length + "--" + index);
                if (index == -1)
                {
                    break;
                }

                fullPath = fullPath.Substring(0, index);
                if (nowStep == step)
                {
                    findPath = fullPath;
                    break;
                }
            }

            return findPath;
        }

        //复制文件夹SourcePath到DestinationPath文件夹中
        public static bool copyFolder(string SourcePath, string DestinationPath, bool overwriteexisting)
        {
            bool ret = false;
            try
            {
                SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
                DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                        Directory.CreateDirectory(DestinationPath);

                    foreach (string fls in Directory.GetFiles(SourcePath))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        if (flinfo.FullName.Contains(".meta") == false)
                        {
                            flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                        }
                    }

                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        if (copyFolder(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)
                            ret = false;
                    }
                }

                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
            }

            return ret;
        }
    }
}