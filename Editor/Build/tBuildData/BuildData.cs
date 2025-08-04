using System;
using System.IO;
using System.Text;
using JQCore.tUtil;
using UnityEngine;

namespace JQEditor.Build
{
    public class BuildData : BuildBase
    {
        public BuildData(string name, string saveKey) : base(name, saveKey)
        {
        }

        public override void build(Action endAction)
        {
            base.build(endAction);
            BuildAll();
        }

        public static void BuildAll()
        {
        }

        //[MenuItem("Tools/build data/配置数据")]
        public static void BuildDataZip()
        {
//        //合并大文件
//        string path = mergeDatas();
//        // + BuildAppInfo.lanType + 
//        //string lanType = "zh"; //BuildAppInfo.lanType
//        //大文件压缩  
//        string outputPath = PathUtil.getStreamingDataPath("Data", false);
//        //string outputPath = EditorParams.getOutPutPath("Data.zip", typePath, false);
//
//        LZMAUtil.CompressFileLZMA(path, outputPath);
//        Debug.Log("生成数据压缩文件：" + outputPath);

            //md5
            //UpdateVersion(outputPath, VersionLoader.DATA);
        }

        #region 数据

        private static string mergeDatas()
        {
            //string lanType = "zh"; //BuildAppInfo.lanType
            //压缩Data
            var directoryInfo = new DirectoryInfo(string.Concat(Application.dataPath, $"/{PathUtil.RES_FOLDER}/Data/"));
            var fileInfos = directoryInfo.GetFiles();

            var path = string.Concat(Application.dataPath, $"/{PathUtil.RES_FOLDER}/Data/", "Data.temp");
            var stream = new FileStream(path, FileMode.Create);
            //组合所有数据
            for (var i = 0; i < fileInfos.Length; i++)
            {
                var fileInfo = fileInfos[i];
                if (!fileInfo.FullName.EndsWith(".dat")) continue;
                var tabName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                var context = File.ReadAllText(fileInfo.FullName);

                if (context != "")
                {
                    writeString(stream, tabName);
                    writeLongString(stream, context);
                }
            }

            stream.Flush();
            stream.Close();

            return path;
        }

        public static void writeString(FileStream fs, string value)
        {
            writeUint16(fs, (ushort)value.Length);
            var strBuf = Encoding.UTF8.GetBytes(value);
            fs.Write(strBuf, 0, strBuf.Length);
        }

        public static void writeLongString(FileStream fs, string value)
        {
            var strBuf = Encoding.UTF8.GetBytes(value);
            writeUint32(fs, (uint)strBuf.Length);
            fs.Write(strBuf, 0, strBuf.Length);
        }


        public static void writeUint16(FileStream fs, ushort value)
        {
            var len = new byte[2];
            len[0] = (byte)(value >> 8);
            len[1] = (byte)(value & 0xFF);
            fs.Write(len, 0, 2);
        }

        public static void writeUint32(FileStream fs, uint value)
        {
            var values = new byte[4];
            values[0] = (byte)((value & 0xFF000000) >> 24);
            values[1] = (byte)((value & 0xFF0000) >> 16);
            values[2] = (byte)((value & 0xFF00) >> 8);
            values[3] = (byte)(value & 0xFF);
            fs.Write(values, 0, 4);
        }

        public static void writeInt32(FileStream fs, int value)
        {
            var values = new byte[4];
            values[0] = (byte)((value & 0xFF000000) >> 24);
            values[1] = (byte)((value & 0xFF0000) >> 16);
            values[2] = (byte)((value & 0xFF00) >> 8);
            values[3] = (byte)(value & 0xFF);
            fs.Write(values, 0, 4);
        }


        /*
        private static void UpdateVersion(string filePath, string key)
        {
            string baseVersion = "1";
            string codeMd5 = MD5Hash.GetFileMD5(filePath);
            string preMd5 = null;
            bool exist = _versionJson.GetField(ref preMd5, key);
            if (!exist || !preMd5.Equals(codeMd5))
            {
                string version = "1.0";
                bool exict = _versionJson.GetField(ref version, key + "Version");
                if (exict && version !=null)
                {
                    int versionInt = int.Parse(version.Split('.')[1]);
                    versionInt++;
                    version = string.Concat(baseVersion, "." + versionInt);
                }
                else
                {
                    version = string.Concat(baseVersion, ".0");
                }
                _versionJson.SetField(key + "Version", version);
                _versionJson.SetField(key, codeMd5);
            }
        }*/

        #endregion

        /*
        private static void LoadVersionFile()
        {
            string path = EditorParams.getOutPutPath("Version.dat", BuildHelper.typePath, false);
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                byte[] by = new byte[fs.Length];
                fs.Read(by, 0, (int)fs.Length);
                string str = Encoding.UTF8.GetString(by);
                fs.Close();
                _versionJson = new JSONObject(str);
            }
            else
            {
                _versionJson = new JSONObject();
            }
        }

        private static void SaveVersionFile()
        {
            string path = EditorParams.getOutPutPath("Version.dat", BuildHelper.typePath, false);
            string jsonStr = _versionJson.ToString();
            byte[] data = Encoding.UTF8.GetBytes(jsonStr);
            HyDebug.Log("save:"+path);
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
        }*/
    }
}