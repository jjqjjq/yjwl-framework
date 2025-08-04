/*----------------------------------------------------------------
// 文件名：BuildLuaDataUtil.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/10/20 14:44:39
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;
using JQCore.tFileSystem;
using JQCore.tJson;
using JQCore.tUtil;

namespace JQEditor.Build
{
    public static class BuildLuaDataUtil
    {
        private static bool isAB(string fileName)
        {
            return fileName.EndsWith(".ab");
        }


        public static void build(Action exeEndAction)
        {
            var rootPath = "Assets/StreamingAssets/resource/luascripts/";
            var fileList = new List<string>();
            JQFileUtil.getAllFile(ref fileList, rootPath, isAB);
            var jsonObject = new JSONObject();
            foreach (var file in fileList)
            {
                var shortPath = file.Replace(rootPath, "").Replace(".ab", "");
                jsonObject.Add(shortPath);
            }

//            Debug.Log(jsonObject.ToString());
            var bytes = Encoding.UTF8.GetBytes(jsonObject.ToString());
            var path = PathUtil.getStreamingDataPath("LuaInfo", false);
            JQFileUtil.SaveFile(bytes, path);
            //            string outputPath = PathUtil.getStreamingDataPath("LuaInfo", false);
            //            LZMAUtil.CompressFileLZMA(path, outputPath);
            //            FileUtil.deleteFile(path);
//            Debug.Log("生成Lua AB列表：" + path);

            if (exeEndAction != null) exeEndAction();
        }
    }
}