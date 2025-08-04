/*----------------------------------------------------------------
// 文件名：BuildLuaUtil.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/10/20 14:15:54
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JQCore.tFileSystem;
using JQCore.tUtil;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Build
{
    public static class BuildLuacUtil
    {
        private static readonly string executablePath = Application.dataPath.Replace("Assets", "Tools"); //路径;
        private static readonly string basePath = Application.dataPath.Replace("Assets", ""); //路径;
        private static string _cmd;

        public static void build(Action exeEndAction)
        {
            _cmd = getCmd();

            var streamAssetPath = $"Resources/{PathUtil.LUA_FOLDER}";

            JQFileUtil.DeleteDirectory(streamAssetPath);
            var fileList = new List<string>();
            JQFileUtil.getAllFile(ref fileList, $"Assets/{PathUtil.LUA_FOLDER}", isLuaFile);

            var buildThread = new BuildThread<string>("编译Lua执行中...（请关闭360等软件提速）", fileList, 12, doNonLockFun, exeEndAction);
            buildThread.Start();
        }


        private static bool isLuaFile(string name)
        {
            return name.EndsWith(".lua");
        }

        private static string getCmd()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.Android:
                    return executablePath + "\\lua-5.3.4_Win32_bin\\luac53 ";
                case BuildTarget.iOS:
                    return executablePath + "\\lua-5.3.4_Win64_bin\\luac53 ";
            }

            return null;
        }

        private static void doNonLockFun(string fileName)
        {
            var targetFileName = fileName.Replace($"{PathUtil.LUA_FOLDER}", $"Resources\\{PathUtil.LUA_FOLDER}").Replace(".lua", ".bytes");
            var folderName = JQFileUtil.getParentFolderPath(targetFileName);
            JQFileUtil.CreateDirectory(folderName);
            var fullFileName = (basePath + fileName).Replace("/", "\\");
            var fullTargetFileName = (basePath + targetFileName).Replace("/", "\\");
            var paramStr = "-o " + fullTargetFileName + " " + fullFileName;
            var outPut = startcmd(_cmd, paramStr); //用luac编译
        }


        public static string startcmd(string cmd, string paramStr)
        {
            var output = "";
            var process = new Process();
            process.StartInfo.FileName = cmd;
            process.StartInfo.Arguments = paramStr;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            output = process.StandardOutput.ReadToEnd();
            process.Close();
            return output;
        }
    }
}