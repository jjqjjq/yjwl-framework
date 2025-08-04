/*----------------------------------------------------------------
// 文件名：SVNTools.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/10/28 20:41:55
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using JQCore;
using JQCore.tLog;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JQCore.tUtil
{
    public static class SVNTools
    {
        private static readonly List<string> drives = new() { "c:", "d:", "e:", "f:" };
        private static readonly string SvnCachePath = "Assets/Scripts/framework/Tool/Util/SvnCache";
        private static readonly List<string> svnPaths = new() { @"\Program Files\TortoiseSVN\bin\", @"\TortoiseSVN\bin\", @"\svn\bin\", @"\SVN\bin\" };

        private static readonly List<string> localPaths = new();

        // private static string svnProc = @"TortoiseProc.exe";
        private static readonly string svnExe = @"svn.exe";
        private static string svnProcPath = "";

        public static void GetVersionFromSVN()
        {
            try
            {
                if (string.IsNullOrEmpty(svnProcPath))
                    svnProcPath = GetSvnPath(svnExe);
                var dir = new DirectoryInfo(Application.dataPath);
                var path = dir.Parent.FullName.Replace('/', '\\');
                var para = "info " + path;
                var process = new Process();
                process.StartInfo.FileName = svnProcPath;
                process.StartInfo.Arguments = para;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                var xxxx = process.StandardOutput.ReadToEnd();
                var xx = MidStrEx_New(xxxx, "Revision: ", "Node Kind: directory");
                Sys.svnVersion = int.Parse(xx);
                JQLog.Log("SVN Version:" + xxxx);
            }
            catch (Exception e)
            {
                Sys.svnVersion = 0;
            }
        }

        private static void WriteSvnPath(string str)
        {
            File.WriteAllText(SvnCachePath, str);
        }

        private static string ReadSvnPath()
        {
            if (!File.Exists(SvnCachePath)) return "";

            var readText = File.ReadAllText(SvnCachePath);
            return readText;
        }

        public static string MidStrEx_New(string sourse, string startstr, string endstr)
        {
            var rg = new Regex("(?<=(" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(sourse).Value;
        }

        public static string GetSvnPath(string svnExestr)
        {
#if UNITY_EDITOR
            var dir = ReadSvnPath();
            var dir2 = string.Concat(dir, "\\");
            foreach (var item in drives)
            foreach (var svnPath in svnPaths)
            {
                var tpath = string.Concat(item, svnPath);
                localPaths.Add(tpath);
            }

            localPaths.Add(dir2);
            foreach (var svnPath in localPaths)
            {
                var pathread = string.Concat(svnPath, svnExestr);
                if (File.Exists(pathread))
                    return pathread;
            }

            ;
            var realexe = EditorUtility.OpenFilePanel("Select svn.exe", "c:\\", "exe");
            var realpath = Path.GetDirectoryName(realexe);
            WriteSvnPath(realpath);
            return realexe;
#else
            return "0";
#endif
        }
    }
}