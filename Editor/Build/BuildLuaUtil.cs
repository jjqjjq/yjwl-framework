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
using System.IO;
using System.Text;
using JQCore.tFileSystem;
using JQCore.tJson;
using JQCore.tUtil;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Build
{
    public static class BuildLuaUtil
    {
        //        private static string executablePath = Application.dataPath.Replace("Assets", "Tools");//路径;
        //        private static string basePath = Application.dataPath.Replace("Assets", "");//路径;
        //        private static string _cmd;

        private static Action _exeEndAction;

        private static Stopwatch _stopwatchAll;
        private static bool _showUI;

        private static readonly List<string> _luaList = new();

        /// <summary>
        ///     复制Lua并且给Lua设置AbName
        /// </summary>
        /// <param name="showUI"></param>
        /// <param name="exeEndAction"></param>
        public static void build(bool showUI, Action exeEndAction = null)
        {
            _showUI = showUI;
            Console.WriteLine($"BuildLuaUtil {showUI}");
            _exeEndAction = exeEndAction;
            Console.WriteLine($"buildLua-start:    {DateTime.Now.ToString()}");

            //            _stopwatchAll = new Stopwatch();
            //            _stopwatchAll.Start();
            //            _cmd = getCmd();

//            FileUtil.deleteDirectory($"Assets/{PathUtil.RES_FOLDER}/{PathUtil.LUA_FOLDER}");
            var preFileList = new List<string>();
            JQFileUtil.getAllFile(ref preFileList, $"Assets/{PathUtil.RES_FOLDER}/{PathUtil.LUA_FOLDER}", s => s.EndsWith(".lua"));
            foreach (var filePath in preFileList) JQFileUtil.deleteFile(filePath);

            CheckCommonTools.Search<TextAsset>("Lua文件处理", $"Assets/{PathUtil.LUA_FOLDER}", copyLua, null, onCopyLuaEnd, showUI, ".lua", "");
            //
            //            string streamAssetPath = Application.dataPath + $"/{folderName}/{luaFolder}";
            //
            //            FileUtil.deleteDirectory(streamAssetPath);
            //            List<string> fileList = new List<string>();
            //            FileUtil.getAllFile(ref fileList, $"Assets/{luaFolder}", isLuaFile);
            //
            //            foreach (string file in fileList)
            //            {
            //               
            //            }
            //
            //            BuildThread<string> buildThread = new BuildThread<string>("编译Lua执行中...（请关闭360等软件提速）", fileList, 12, doNonLockFun, exeEndAction);
            //            buildThread.Start();
        }

        private static void onCopyLuaEnd()
        {
            Console.WriteLine($"buildLua-end:    {DateTime.Now.ToString()}");
            AssetDatabase.Refresh();
            _luaList.Clear();
            // BuildAssetBundleUtil.setLua(_showUI, _luaList , setLuaEnd);
        }

        private static void setLuaEnd()
        {
            var jsonObject = new JSONObject();
            foreach (var file in _luaList)
            {
//                Debug.Log(file);
                var shortPath = file.Replace($"{PathUtil.LUA_FOLDER}/", "").Replace(".ab", "").Replace("\\", "/");
                jsonObject.Add(shortPath.ToLower());
            }

//            Debug.Log(jsonObject.ToString());
            var bytes = Encoding.UTF8.GetBytes(jsonObject.ToString());
            var path = $"Assets/{PathUtil.RES_FOLDER}/Data/LuaInfo.txt";
            JQFileUtil.SaveFile(bytes, path);

            AssetDatabase.Refresh();
            var assetImporter = AssetImporter.GetAtPath(path);
            assetImporter.assetBundleName = "data/luainfo.txt.ab";
            assetImporter.SaveAndReimport();

            if (_exeEndAction != null) _exeEndAction();
        }

        private static bool copyLua(string path, TextAsset luaAsset, object obj1)
        {
            var newFile = path.Replace($"/{PathUtil.LUA_FOLDER}", $"/{PathUtil.RES_FOLDER}/{PathUtil.LUA_FOLDER}").Replace(".lua", ".bytes");
//            Debug.Log("path:"+path);
//            Debug.Log("newFile:" + newFile);
            var folder = JQFileUtil.getParentFolderPath(newFile);
            JQFileUtil.CreateDirectory(folder);
            File.Copy(path, newFile, true);
            return true;
//            AssetDatabase.CopyAsset(path, newFile);
        }


        //        private static string getCmd()
        //        {
        //            switch (EditorUserBuildSettings.activeBuildTarget)
        //            {
        //                case BuildTarget.StandaloneWindows:
        //                case BuildTarget.StandaloneWindows64:
        //                case BuildTarget.Android:
        //                    return executablePath + "\\lua-5.3.4_Win32_bin\\luac53 ";
        //                case BuildTarget.iOS:
        //                    return executablePath + "\\lua-5.3.4_Win64_bin\\luac53 ";
        //            }
        //            return null;
        //        }
        //
        //        private static void doNonLockFun(string fileName)
        //        {
        //            string targetFileName = fileName.Replace($"{_luaFolder}", $"{_luaFolder}\\{_luaFolder}").Replace(".lua", ".bytes");
        //            string folderName = FileUtil.getParentFolderPath(targetFileName);
        //            FileUtil.CreateDirectory(folderName);
        //            string fullFileName = (basePath + fileName).Replace("/", "\\");
        //            string fullTargetFileName = (basePath + targetFileName).Replace("/", "\\");
        //            string paramStr = "-o " + fullTargetFileName + " " + fullFileName;
        //            string outPut = startcmd(_cmd, paramStr);//用luac编译
        //        }
        //
        //
        //        public static string startcmd(string cmd, string paramStr)
        //        {
        //            string output = "";
        //            Process process = new Process();
        //            process.StartInfo.FileName = cmd;
        //            process.StartInfo.Arguments = paramStr;
        //            process.StartInfo.UseShellExecute = false;
        //            process.StartInfo.RedirectStandardInput = true;
        //            process.StartInfo.RedirectStandardOutput = true;
        //            process.StartInfo.RedirectStandardError = true;
        //            process.StartInfo.CreateNoWindow = true;
        //            process.Start();
        //            output = process.StandardOutput.ReadToEnd();
        //            process.Close();
        //            return output;
        //        }
    }
}