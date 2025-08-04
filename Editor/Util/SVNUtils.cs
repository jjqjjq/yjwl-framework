using System;
using System.Collections.Generic;
using System.IO;
using JQEditor.Tool;
using JQCore.tUtil;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Util
{
    public class SVNUtils
    {
        private static List<string> drives = new List<string>() { "c:", "d:", "e:", "f:" };
        private static string svnPath = @"\Program Files\TortoiseSVN\bin\";
        private static string svnProc = @"TortoiseProc.exe";
        private static string svn = @"svn.exe";

        [MenuItem("Assets/SVN/SVN更新 %&e")]
        public static void UpdateFromSVN()
        {
            var dir = new DirectoryInfo(Application.dataPath);
            var path = dir.Parent.FullName.Replace('/', '\\');
            var para = "/command:update /path:\"" + path + "\" /closeonend:0";
            System.Diagnostics.Process.Start(GetSvnProcPath(), para);
        }

        // [MenuItem("IrfTools/SVN/SVN提交")]
        public static void SVNTest()
        {
            var path = Application.dataPath.Replace('/', '\\');
            var para = "/command:commit /path:\"" + path + "\"";
            System.Diagnostics.Process.Start(GetSvnProcPath(), para);
        }

        [MenuItem("Assets/SVN/SVN提交 %&r")]
        public static void CommitToSVN()
        {
            var path = Application.dataPath.Replace('/', '\\');
            var para = "/command:commit /path:\"" + path + "\"";
            System.Diagnostics.Process.Start(GetSvnProcPath(), para);
        }

        [MenuItem("Assets/SVN/回滚本地修改（更新时看到红色字请点我！） %&t")]
        public static void RevertFromSVN()
        {
            var path = Application.dataPath.Replace('/', '\\');
            var para = "/command:revert -R /path:\"" + path + "\"";
            System.Diagnostics.Process.Start(GetSvnProcPath(), para);
        }

        [MenuItem("Assets/SVN/清理SVN %&y")]
        public static void CleanUpFromSVN()
        {
            var path = Application.dataPath.Replace('/', '\\');
            var para = "/command:cleanup /path:\"" + path + "\"";
            System.Diagnostics.Process.Start(GetSvnProcPath(), para);
        }


        public static void DelAndUpdateSvnFile()
        {
            var path = Application.dataPath.Replace('/', '\\');
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            SystemProcessUtil.ExecuteProcess(GetSvnProcPath(), string.Format("/command:update /path:{0} /closeonend:2", path));
        }

        public static void RevertAndUpdateSvnDirectory(Action endAction = null)
        {
            var path = Application.dataPath.Replace('/', '\\');
            SystemProcessUtil.ExecuteProcess(GetSvnPath(), "revert -R .", path);
            SystemProcessUtil.ExecuteProcess(GetSvnPath(), "update", path);
            if (endAction != null)
            {
                endAction();
            }
        }

        public static void UpdateSvnDirectory()
        {
            var path = Application.dataPath.Replace('/', '\\');
            SystemProcessUtil.ExecuteProcess(GetSvnProcPath(), string.Format("/command:update /path:{0} /closeonend:2", path));
        }

        public static void CommitSvnDirectory()
        {
            var path = Application.dataPath.Replace('/', '\\');
            SystemProcessUtil.ExecuteProcess(GetSvnProcPath(), string.Format("/command:commit /path:{0} /closeonend:0", path));
        }

        public static void ProcessSvnCommand(string command)
        {
            SystemProcessUtil.ExecuteProcess(GetSvnProcPath(), command);
        }

        private static string GetSvnProcPath()
        {
            return SVNTools.GetSvnPath(svnProc);
        }

        private static string GetSvnPath()
        {
            return SVNTools.GetSvnPath(svn);
        }
    }
}