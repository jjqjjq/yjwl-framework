using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JQCore.tLog;
using UnityEngine;

namespace JQCore.tFileSystem
{
    public class JQFileUtil
    {
        //确保传入文件路径的文件夹存在
        public static void ensureDirectory(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }


        //取得所在文件夹名字
        public static string getParentFolderPath(string path, bool fullPath = true)
        {
            path = path.Replace("\\", "/");
            var lasts = path.LastIndexOf('/');
            var xxx = path.Substring(0, lasts);
            if (!fullPath)
            {
                lasts = xxx.LastIndexOf('/');
                var yyy = xxx.Substring(lasts + 1, xxx.Length - lasts - 1);
                return yyy;
            }

            return xxx;
        }

        public static string getCurrFolderOrFileName(string path, bool withExtens = true)
        {
            path = path.Replace("\\", "/");
            var lasts = path.LastIndexOf('/');
            var xxx = path.Substring(lasts + 1, path.Length - 1 - lasts);
            if (!withExtens) xxx = xxx.Substring(0, xxx.LastIndexOf('.'));

            return xxx;
        }

        public static string getExtension(string path)
        {
            var lasts = path.LastIndexOf('.');
            var xxx = path.Substring(lasts + 1, path.Length - 1 - lasts);
            return xxx;
        }

        public static string removeExtension(string path)
        {
            var lasts = path.LastIndexOf('.');
            var xxx = path.Substring(0, lasts);
            return xxx;
        }

        public static void SaveFile(string data, string path)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            SaveFile(bytes, path);
        }

        public static void SaveFile(byte[] data, string path)
        {
            SaveFile(data, 0, data.Length, path);
        }

        public static void SaveFile(byte[] data, int offset, int count, string path)
        {
            path = path.Replace("\\", "/");
            //HyDebug.Log("save file:" + path);
            var floder = path.Substring(0, path.LastIndexOf("/") + 1);
            if (!Directory.Exists(floder)) Directory.CreateDirectory(floder);

            var stream = new FileStream(path, FileMode.Create);
            stream.Write(data, offset, count);
            stream.Flush();
            stream.Close();
        }


        public static void deleteFile(string filePath)
        {
            var file = new FileInfo(filePath);
            if (!file.Exists)
                JQLog.Log("删除文件不存在：" + filePath);
            file.Delete();
            //            HyDebug.Log("文件：" + filePath + " 已删除");
        }

        
        

        public static void getAllFile(ref List<string> passList, string directory, Predicate<string> predicate = null, bool depth = true, bool containTemp = false)
        {
            var dir = new DirectoryInfo(directory);
            if (!dir.Exists)
            {
                JQLog.Log("目录不存在：" + directory);
                return;
            }

            var childs = Directory.GetFileSystemEntries(directory);
            foreach (var child in childs)
            {
                if (child.EndsWith(".meta")) continue;

                if (!containTemp && child.Contains("Temp")) continue;

                if (child.Contains(".svn")) continue;

                if (child.Contains(".DS_Store")) continue;

                if (File.Exists(child))
                {
                    if (predicate != null)
                    {
                        if (predicate(child)) passList.Add(child);
                    }
                    else
                    {
                        passList.Add(child);
                    }
                }
                else if (depth)
                {
                    getAllFile(ref passList, child, predicate, depth, containTemp);
                }
            }
        }


		#region 路径
		/// <summary>
		/// 获取规范的路径
		/// </summary>
		public static string GetRegularPath(string path)
		{
			return path.Replace('\\', '/').Replace("\\", "/"); //替换为Linux路径格式
		}

		/// <summary>
		/// 获取项目工程路径
		/// </summary>
		public static string GetProjectPath()
		{
			string projectPath = Path.GetDirectoryName(Application.dataPath);
			return GetRegularPath(projectPath);
		}

		/// <summary>
		/// 转换文件的绝对路径为Unity资源路径
		/// 例如 D:\\YourPorject\\Assets\\Works\\file.txt 替换为 Assets/Works/file.txt
		/// </summary>
		public static string AbsolutePathToAssetPath(string absolutePath)
		{
			string content = GetRegularPath(absolutePath);
			return Substring(content, "Assets/", true);
		}

		/// <summary>
		/// 转换Unity资源路径为文件的绝对路径
		/// 例如：Assets/Works/file.txt 替换为 D:\\YourPorject/Assets/Works/file.txt
		/// </summary>
		public static string AssetPathToAbsolutePath(string assetPath)
		{
			string projectPath = GetProjectPath();
			return $"{projectPath}/{assetPath}";
		}

		/// <summary>
		/// 递归查找目标文件夹路径
		/// </summary>
		/// <param name="root">搜索的根目录</param>
		/// <param name="folderName">目标文件夹名称</param>
		/// <returns>返回找到的文件夹路径，如果没有找到返回空字符串</returns>
		public static string FindFolder(string root, string folderName)
		{
			DirectoryInfo rootInfo = new DirectoryInfo(root);
			DirectoryInfo[] infoList = rootInfo.GetDirectories();
			for (int i = 0; i < infoList.Length; i++)
			{
				string fullPath = infoList[i].FullName;
				if (infoList[i].Name == folderName)
					return fullPath;

				string result = FindFolder(fullPath, folderName);
				if (string.IsNullOrEmpty(result) == false)
					return result;
			}
			return string.Empty;
		}

		/// <summary>
		/// 截取字符串
		/// 获取匹配到的后面内容
		/// </summary>
		/// <param name="content">内容</param>
		/// <param name="key">关键字</param>
		/// <param name="includeKey">分割的结果里是否包含关键字</param>
		/// <param name="searchBegin">是否使用初始匹配的位置，否则使用末尾匹配的位置</param>
		public static string Substring(string content, string key, bool includeKey, bool firstMatch = true)
		{
			if (string.IsNullOrEmpty(key))
				return content;

			int startIndex = -1;
			if (firstMatch)
				startIndex = content.IndexOf(key); //返回子字符串第一次出现位置		
			else
				startIndex = content.LastIndexOf(key); //返回子字符串最后出现的位置

			// 如果没有找到匹配的关键字
			if (startIndex == -1)
				return content;

			if (includeKey)
				return content.Substring(startIndex);
			else
				return content.Substring(startIndex + key.Length);
		}
		#endregion
		#region 文件
		/// <summary>
		/// 创建文件所在的目录
		/// </summary>
		/// <param name="filePath">文件路径</param>
		public static void CreateFileDirectory(string filePath)
		{
			string destDirectory = Path.GetDirectoryName(filePath);
			CreateDirectory(destDirectory);
		}

		/// <summary>
		/// 创建文件夹
		/// </summary>
		public static bool CreateDirectory(string directory)
		{
			if (Directory.Exists(directory) == false)
			{
				Directory.CreateDirectory(directory);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 删除文件夹及子目录
		/// </summary>
		public static bool DeleteDirectory(string directory)
		{
			if (Directory.Exists(directory))
			{
				Directory.Delete(directory, true);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 文件重命名
		/// </summary>
		public static void FileRename(string filePath, string newName)
		{
			string dirPath = Path.GetDirectoryName(filePath);
			string destPath;
			if (Path.HasExtension(filePath))
			{
				string extentsion = Path.GetExtension(filePath);
				destPath = $"{dirPath}/{newName}{extentsion}";
			}
			else
			{
				destPath = $"{dirPath}/{newName}";
			}
			FileInfo fileInfo = new FileInfo(filePath);
			fileInfo.MoveTo(destPath);
		}
		
		/// <summary>
		/// 文件重命名
		/// </summary>
		public static void FileRenamePath(string filePath, string destPath)
		{
			FileInfo fileInfo = new FileInfo(filePath);
			fileInfo.MoveTo(destPath);
		}

		/// <summary>
		/// 移动文件
		/// </summary>
		public static void MoveFile(string filePath, string destPath)
		{
			if (File.Exists(destPath))
				File.Delete(destPath);

			FileInfo fileInfo = new FileInfo(filePath);
			fileInfo.MoveTo(destPath);
		}

		/// <summary>
		/// 拷贝文件夹
		/// 注意：包括所有子目录的文件
		/// </summary>
		public static void CopyDirectory(string sourcePath, string destPath)
		{
			sourcePath = GetRegularPath(sourcePath);

			// If the destination directory doesn't exist, create it.
			if (Directory.Exists(destPath) == false)
				Directory.CreateDirectory(destPath);

			string[] fileList = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
			foreach (string file in fileList)
			{
				string temp = GetRegularPath(file);
				string savePath = temp.Replace(sourcePath, destPath);
				CopyFile(file, savePath);
			}
		}

		/// <summary>
		/// 拷贝文件
		/// </summary>
		public static void CopyFile(string sourcePath, string destPath)
		{
			if (File.Exists(sourcePath) == false)
			{
				Debug.LogError($"文件不存在：{sourcePath}");
			}

			// 创建目录
			CreateFileDirectory(destPath);

			// 复制文件
			if (File.Exists(destPath))
			{
				File.Copy(sourcePath, destPath, true);
			}
			else
			{
				File.Copy(sourcePath, destPath);
			}
		}

		/// <summary>
		/// 清空文件夹
		/// </summary>
		/// <param name="folderPath">要清理的文件夹路径</param>
		public static void ClearFolder(string directoryPath)
		{
			if (Directory.Exists(directoryPath) == false)
				return;

			// 删除文件
			string[] allFiles = Directory.GetFiles(directoryPath);
			for (int i = 0; i < allFiles.Length; i++)
			{
				File.Delete(allFiles[i]);
			}

			// 删除文件夹
			string[] allFolders = Directory.GetDirectories(directoryPath);
			for (int i = 0; i < allFolders.Length; i++)
			{
				Directory.Delete(allFolders[i], true);
			}
		}

		/// <summary>
		/// 获取文件字节大小
		/// </summary>
		public static long GetFileSize(string filePath)
		{
			FileInfo fileInfo = new FileInfo(filePath);
			return fileInfo.Length;
		}

		/// <summary>
		/// 读取文件的所有文本内容
		/// </summary>
		public static string ReadFileAllText(string filePath)
		{
			if (File.Exists(filePath) == false)
				return string.Empty;

			return File.ReadAllText(filePath, Encoding.UTF8);
		}

		/// <summary>
		/// 读取文本的所有文本内容
		/// </summary>
		public static string[] ReadFileAllLine(string filePath)
		{
			if (File.Exists(filePath) == false)
				return null;

			return File.ReadAllLines(filePath, Encoding.UTF8);
		}

		/// <summary>
		/// 检测AssetBundle文件是否合法
		/// </summary>
		public static bool CheckBundleFileValid(byte[] fileData)
		{
			string signature = ReadStringToNull(fileData, 20);
			if (signature == "UnityFS" || signature == "UnityRaw" || signature == "UnityWeb" || signature == "\xFA\xFA\xFA\xFA\xFA\xFA\xFA\xFA")
				return true;
			else
				return false;
		}
		private static string ReadStringToNull(byte[] data, int maxLength)
		{
			List<byte> bytes = new List<byte>();
			for (int i = 0; i < data.Length; i++)
			{
				if (i >= maxLength)
					break;

				byte bt = data[i];
				if (bt == 0)
					break;

				bytes.Add(bt);
			}

			if (bytes.Count == 0)
				return string.Empty;
			else
				return Encoding.UTF8.GetString(bytes.ToArray());
		}
		#endregion
        /*
#if UNITY_ANDROID
     [DllImport("dllCrypt")]
     public static extern void decryptDll(IntPtr ptr, int buffLen);
#endif
*/
    }
}