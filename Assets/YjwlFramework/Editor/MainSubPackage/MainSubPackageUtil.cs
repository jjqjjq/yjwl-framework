using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JQCore.tFileSystem;
using JQCore.tUtil;
using JQEditor.Build;
using JQEditor.Excel;
using JQEditor.ThirdParty.Aliyun;
using JQCore;
using JQCore.tYooAssets;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;
using EditorTools = YooAsset.Editor.EditorTools;

namespace JQEditor.MainSubPackage
{
    public static class MainSubPackageUtil
    {
        private static string ROUTER_FILE = $"{Application.dataPath}/{PathUtil.RES_FOLDER}/Build/routers/router.info";
        private static string APP_VERSION = $"{Application.dataPath}/{PathUtil.RES_FOLDER}/Build/MainSubPackage/AppVersion.version";
        private static string FIRST_PACKAGE_LIST = $"{Application.dataPath}/{PathUtil.RES_FOLDER}/Build/MainSubPackage/ResLoadRecorder.txt";
        private static string PACKAGE_CONFIG_EXCEL = $"{Application.dataPath}/{PathUtil.RES_FOLDER}/Build/MainSubPackage/AssetConfig.xlsx";


        public static string getOutputPackageFolder(string version)
        {
            string defaultOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            string packageFolder = $"{defaultOutputRoot}/{EditorUserBuildSettings.activeBuildTarget}/{PathUtil.YOOASSET_PACKAGE_NAME}/{version}";
            // Debug.Log(packageFolder);
            return packageFolder;
        }

        public static string getAllPackageManifestPath(string version)
        {
            string packageFolder = getOutputPackageFolder(version);
            string manifestFilePath = $"{packageFolder}/{YooAssetSettingsData.Setting.ManifestFileName}_{PathUtil.YOOASSET_PACKAGE_NAME}_{version}.bytes";
            // Debug.Log(manifestFilePath);
            return manifestFilePath;
        }


        public static void saveAppVersionFile()
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(BuildAppInfo.version);
            JQFileUtil.SaveFile(byteArray, APP_VERSION);
            AssetDatabase.Refresh();
            Debug.Log("saveAppVersionFile Finish!");
        }
        
        public static void saveRouterFile()
        {
            string fromFilePath = $"{Application.dataPath}/{PathUtil.RES_FOLDER}/Build/routers/router-{BuildAppInfo.sysCfgType}.txt";
            JQFileUtil.CopyFile(fromFilePath, ROUTER_FILE);
            AssetDatabase.Refresh();
            Debug.Log("saveRouterFile Finish!");
        }

        public static void uploadAppVersionFileOSS()
        {
            uploadToOss(APP_VERSION);
        }
        
        public static void uploadRouterFileOSS()
        {
            uploadToOss(ROUTER_FILE);
        }

        public static void uploadAppVersionFileLocalWeb()
        {
            uploadToLocalWeb(APP_VERSION);
        }
        
        public static void uploadRouterFileLocalWeb()
        {
            uploadToLocalWeb(ROUTER_FILE);
        }

        public static void saveRecorder()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("请先运行游戏跑主包资源加载");
                return;
            }

            byte[] byteArray = YooAssetMgr.GetFileBytes();
            JQFileUtil.SaveFile(byteArray, FIRST_PACKAGE_LIST);
            AssetDatabase.Refresh();
            Debug.Log("saveRecorder Finish!");
        }

        //山水志临时处理
        private static bool isInMainPackage2(string[] mainPackageArray, string assetPath)
        {
            return !assetPath.Contains($"Assets/{PathUtil.RES_FOLDER}/Scene");
        }

        //正常处理
        private static bool isInMainPackage(string[] mainPackageArray, string assetPath)
        {
            return mainPackageArray.Contains(assetPath);
        }

        private static PackageManifest getPackageManifest(string packageVersion)
        {
            // 加载补丁清单
            string manifestFilePath = getAllPackageManifestPath(packageVersion);
            byte[] bytesData = FileUtility.ReadAllBytes(manifestFilePath);
            PackageManifest manifest = ManifestTools.DeserializeFromBinary(bytesData);
            return manifest;
        }

        public static void updateResCfgExcel()
        {
            //取得补丁包的资源列表
            PackageManifest manifest = getPackageManifest(BuildAppInfo.PackageVersion);

            //取得首包资源列表
            string[] mainPackageArray = File.ReadAllLines(FIRST_PACKAGE_LIST);
            Debug.Log("跑首包资源数量:" + mainPackageArray.Length);

            //取得资源配置excel
            JQExcelMapper<MainSubPackageData> jqExcelMapper = new JQExcelMapper<MainSubPackageData>(PACKAGE_CONFIG_EXCEL, 3, 1);
            jqExcelMapper.ReadFromExcel();

            List<MainSubPackageData> newDataList = new List<MainSubPackageData>();
            Dictionary<string, MainSubPackageData> dataDic = new Dictionary<string, MainSubPackageData>();
            for (int i = 0; i < jqExcelMapper.DataList.Count; i++)
            {
                MainSubPackageData data = jqExcelMapper.DataList[i];

                if (manifest.AssetDic.ContainsKey(data.Name))
                {
                    newDataList.Add(data);
                    dataDic.Add(data.Name, data);
                }
                else //移除已经不存在的资源
                {
                    Debug.Log($"移除已经不使用的资源:{data.Name}");
                }
            }

            //添加新的资源资源
            foreach (var asset in manifest.AssetDic)
            {
                if (!dataDic.ContainsKey(asset.Key))
                {
                    MainSubPackageData data = new MainSubPackageData();
                    data.Name = asset.Key;
                    newDataList.Add(data);
                }
            }

            //设置是否在首包
            for (int i = 0; i < newDataList.Count; i++)
            {
                MainSubPackageData mainSubPackageData = newDataList[i];
                if (isInMainPackage2(mainPackageArray, mainSubPackageData.Name))
                {
                    mainSubPackageData.isInMainPackage = 1;
                }
            }

            jqExcelMapper.DataList = newDataList;
            jqExcelMapper.WriteToExcel();
        }

        public static void CopyMainPackageToStreamAsseting(Action callbackFun)
        {
            string buildPackageVersion = BuildAppInfo.PackageVersion;
            string packageOutputDirectory = getOutputPackageFolder(buildPackageVersion);
            string streamingAssetsDirectory = $"{AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot()}/{PathUtil.YOOASSET_PACKAGE_NAME}";

            // 加载补丁清单
            PackageManifest manifest = getPackageManifest(buildPackageVersion);

            // 清空流目录
            EditorTools.ClearFolder(streamingAssetsDirectory);

            List<string> copyFileList = new List<string>();

            //添加其他文件
            List<string> otherFileList = addOtherFile(buildPackageVersion);
            copyFileList.AddRange(otherFileList);

            //获取需要拷贝的BundleId
            List<int> bundleIdList = collectBundleId(manifest);

            //拷贝Bundle文件
            for (int i = 0; i < bundleIdList.Count; i++)
            {
                int bundleId = bundleIdList[i];
                PackageBundle packageBundle = manifest.BundleList[bundleId];
                copyFileList.Add(packageBundle.FileName);
            }

            int total = copyFileList.Count;
            //拷贝文件
            for (int i = 0; i < copyFileList.Count; i++)
            {
                bool isCancle = EditorUtility.DisplayCancelableProgressBar("内置文件拷贝", $"{i+1}/{total}", ((float)i+1) / total);
                string fileName = copyFileList[i];
                string sourcePath = $"{packageOutputDirectory}/{fileName}";
                string destPath = $"{streamingAssetsDirectory}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }
            EditorUtility.ClearProgressBar();

            // 刷新目录
            AssetDatabase.Refresh();
            BuildLogger.Log($"内置文件拷贝完成：{packageOutputDirectory} =》 {streamingAssetsDirectory}");

            
            //拷贝配置文件到streamingAssetsDir
            string cfgPath = $"{Application.streamingAssetsPath}/cfg";
            JQFileUtil.CopyDirectory(BuildAppInfo.LocalCfgFloder, cfgPath);
            Debug.Log($"复制cfg目录:{BuildAppInfo.LocalCfgFloder} => {cfgPath}");
            
            callbackFun?.Invoke();
        }

        private static List<int> collectBundleId(PackageManifest manifest)
        {
            List<int> bundleIdList = new List<int>();
            
            if (BuildAppInfo.fullPackage)
            {
                for (int i = 0; i < manifest.BundleList.Count; i++)
                {
                    bundleIdList.Add(i);
                }
            }
            else
            {
                //取得资源配置excel
                JQExcelMapper<MainSubPackageData> jqExcelMapper = new JQExcelMapper<MainSubPackageData>(PACKAGE_CONFIG_EXCEL, 3, 1);
                jqExcelMapper.ReadFromExcel();
                
                //从补丁清单里面获得需要拷贝的Bundle文件路径
                List<string> mainAssetList = new List<string>();
                for (int i = 0; i < jqExcelMapper.DataList.Count; i++)
                {
                    MainSubPackageData data = jqExcelMapper.DataList[i];
                    if (data.isInMainPackage == 1 || BuildAppInfo.fullPackage)
                    {
                        mainAssetList.Add(data.Name);
                    }
                }
                
                for (int i = 0; i < mainAssetList.Count; i++)
                {
                    string assetName = mainAssetList[i];
                    if (manifest.AssetDic.ContainsKey(assetName))
                    {
                        PackageAsset assetInfo = manifest.AssetDic[assetName];
                        if (!bundleIdList.Contains(assetInfo.BundleID))
                        {
                            bundleIdList.Add(assetInfo.BundleID);
                        }

                        for (int j = 0; j < assetInfo.DependIDs.Length; j++)
                        {
                            int dependId = assetInfo.DependIDs[j];
                            if (!bundleIdList.Contains(dependId))
                            {
                                bundleIdList.Add(dependId);
                            }
                        }
                    }
                }
            }
            
            return bundleIdList;
        }


        public static void uploadAllPackageToOSS(string currPackageVersion)
        {
            List<OssUploadBean> ossUploadBeans = GetAllPackage(currPackageVersion);
            Debug.Log("上传文件：" + ossUploadBeans.Count);
            OssUploadTask ossUploadTask = new OssUploadTask(ossUploadBeans);
            ossUploadTask.Run(onUploadPackageFinish);
        }


        public static void uploadComparePackageToOSS(string oldPackageVersion, string currPackageVersion)
        {
            if (oldPackageVersion.Equals(currPackageVersion))
            {
                Debug.LogError($"版本号相同，不需要上传：{oldPackageVersion}");
                return;
            }

            List<OssUploadBean> ossUploadBeans = ComparePackage(oldPackageVersion, currPackageVersion);
            Debug.Log("上传文件：" + ossUploadBeans.Count);
            OssUploadTask ossUploadTask = new OssUploadTask(ossUploadBeans);
            ossUploadTask.Run(onUploadPackageFinish);
        }

        private static void onUploadPackageFinish()
        {
            BuildAppInfo.OldPackageVersion = BuildAppInfo.PackageVersion;
        }

        public static void uploadToOss(string filePath)
        {
            List<OssUploadBean> ossUploadBeans = new List<OssUploadBean>();
            string fileName = JQFileUtil.getCurrFolderOrFileName(filePath);
            string ossPath = getOssPath(fileName);
            FileInfo fileInfo = new FileInfo(filePath);
            OssUploadBean ossUploadBean = new OssUploadBean(fileInfo.Length, ossPath, filePath);
            ossUploadBeans.Add(ossUploadBean);
            OssUploadTask ossUploadTask = new OssUploadTask(ossUploadBeans);
            ossUploadTask.Run(onUploadFileFinish);
        }

        public static void uploadToOss(List<string> fileList)
        {
            List<OssUploadBean> ossUploadBeans = new List<OssUploadBean>();
            for (int i = 0; i < fileList.Count; i++)
            {
                string filePath = fileList[i];
                string fileName = JQFileUtil.getCurrFolderOrFileName(filePath);
                string ossPath = getOssPath(fileName);
                FileInfo fileInfo = new FileInfo(filePath);
                OssUploadBean ossUploadBean = new OssUploadBean(fileInfo.Length, ossPath, filePath);
                ossUploadBeans.Add(ossUploadBean);
            }

            OssUploadTask ossUploadTask = new OssUploadTask(ossUploadBeans);
            ossUploadTask.Run(onUploadFileFinish);
        }

        private static void onUploadFileFinish()
        {
            Debug.Log("上传完成");
        }

        public static void uploadToLocalWeb(string filePath)
        {
            string targetDic = $"{BuildAppInfo.LocalWebCdn}/CDN/{PathUtil.platformName}";
            if (!Directory.Exists(targetDic))
            {
                Directory.CreateDirectory(targetDic);
            }

            string fileName = JQFileUtil.getCurrFolderOrFileName(filePath);
            string destPath = $"{targetDic}/{fileName}";
            EditorTools.CopyFile(filePath, destPath, true);
        }

        public static void uploadAllPackageToLocalWeb(string currPackageVersion)
        {
            string packageOutputDirectory = getOutputPackageFolder(currPackageVersion);
            string targetDic = $"{BuildAppInfo.LocalWebCdn}/CDN/{PathUtil.platformName}/v{BuildAppInfo.version}";
            if (!Directory.Exists(targetDic))
            {
                Directory.CreateDirectory(targetDic);
            }

            List<string> fileList = GetAllFilesByVersion(currPackageVersion);
            int total = fileList.Count;
            for (int i = 0; i < fileList.Count; i++)
            {
                bool isCancle = EditorUtility.DisplayCancelableProgressBar("上次到本地Web", $"{i+1}/{total}", ((float)i+1) / total);
                string fileName = fileList[i];
                string sourcePath = $"{packageOutputDirectory}/{fileName}";
                string destPath = $"{targetDic}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }
            EditorUtility.ClearProgressBar();
        }

        private static List<string> GetAllFilesByVersion(string buildPackageVersion)
        {
            List<string> fileList = new List<string>();

            // 加载补丁清单
            PackageManifest manifest = getPackageManifest(buildPackageVersion);

            //从补丁清单里面获得需要拷贝的Bundle文件路径
            for (int i = 0; i < manifest.BundleList.Count; i++)
            {
                PackageBundle packageBundle = manifest.BundleList[i];
                fileList.Add(packageBundle.FileName);
            }

            //添加其他文件
            List<string> otherFileList = addOtherFile(buildPackageVersion);
            fileList.AddRange(otherFileList);
            return fileList;
        }

        private static string getOssPath(string subPath)
        {
            string sysCfgFileName = BuildAppInfo.sysCfgType.ToString();
            string ossPath = $"{sysCfgFileName}/CDN/{PathUtil.platformName}/{subPath}";
            return ossPath;
        }

        private static (string, string) getOssPackagePath(string packageOutputDirectory, string fileName)
        {
            string sysCfgFileName = BuildAppInfo.sysCfgType.ToString();
            string ossPath = $"{sysCfgFileName}/CDN/{PathUtil.platformName}/v{BuildAppInfo.version}/{fileName}";
            string localPath = $"{packageOutputDirectory}/{fileName}";
            return (ossPath, localPath);
        }

        private static List<OssUploadBean> GetAllPackage(string currPackageVersion)
        {
            // 加载补丁清单
            PackageManifest manifest = getPackageManifest(currPackageVersion);
            string packageOutputDirectory = getOutputPackageFolder(currPackageVersion);

            List<OssUploadBean> filePathList = new List<OssUploadBean>();
            foreach (PackageBundle packageBundle in manifest.BundleList)
            {
                (string ossPath, string localPath) = getOssPackagePath(packageOutputDirectory, packageBundle.FileName);
                OssUploadBean ossUploadBean = new OssUploadBean(packageBundle.FileSize, ossPath, localPath);
                filePathList.Add(ossUploadBean);
            }

            List<string> otherFileList = addOtherFile(currPackageVersion);

            for (int i = 0; i < otherFileList.Count; i++)
            {
                string fileName = otherFileList[i];
                (string ossPath, string localPath) = getOssPackagePath(packageOutputDirectory, fileName);
                FileInfo fileInfo = new FileInfo(localPath);
                OssUploadBean ossUploadBean = new OssUploadBean(fileInfo.Length, ossPath, localPath);
                filePathList.Add(ossUploadBean);
            }

            return filePathList;
        }

        /// <summary>
        /// 对比取得需要上传的文件列表
        /// </summary>
        /// <param name="oldManifestPath"></param>
        /// <param name="newManifestPath"></param>
        /// <returns></returns>
        private static List<OssUploadBean> ComparePackage(string oldPackageVersion, string currPackageVersion)
        {
            List<OssUploadBean> needUploadList = new List<OssUploadBean>();

            // 加载补丁清单1
            PackageManifest manifest1 = getPackageManifest(oldPackageVersion);

            // 加载补丁清单1
            PackageManifest manifest2 = getPackageManifest(currPackageVersion);


            string packageOutputDirectory = getOutputPackageFolder(currPackageVersion);

            // 拷贝文件列表
            foreach (var bundle2 in manifest2.BundleList)
            {
                if (manifest1.TryGetPackageBundle(bundle2.BundleName, out PackageBundle bundle1))
                {
                    if (bundle2.FileHash != bundle1.FileHash)
                    {
                        (string ossPath, string localPath) = getOssPackagePath(packageOutputDirectory, bundle2.FileName);
                        OssUploadBean ossUploadBean = new OssUploadBean(bundle2.FileSize, ossPath, localPath);
                        needUploadList.Add(ossUploadBean);
                    }
                }
                else
                {
                    (string ossPath, string localPath) = getOssPackagePath(packageOutputDirectory, bundle2.FileName);
                    OssUploadBean ossUploadBean = new OssUploadBean(bundle2.FileSize, ossPath, localPath);
                    needUploadList.Add(ossUploadBean);
                }
            }

            //添加其他文件
            List<string> otherFileList = addOtherFile(currPackageVersion);

            for (int i = 0; i < otherFileList.Count; i++)
            {
                string fileName = otherFileList[i];
                (string ossPath, string localPath) = getOssPackagePath(packageOutputDirectory, fileName);
                FileInfo fileInfo = new FileInfo(localPath);
                OssUploadBean ossUploadBean = new OssUploadBean(fileInfo.Length, ossPath, localPath);
                needUploadList.Add(ossUploadBean);
            }

            Debug.Log("资源包差异比对完成！");
            return needUploadList;
        }

        private static List<string> addOtherFile(string packageVersion)
        {
            List<string> otherFileList = new List<string>();
            // 拷贝补丁清单文件
            {
                string fileName = YooAssetSettingsData.GetManifestBinaryFileName(PathUtil.YOOASSET_PACKAGE_NAME, packageVersion);
                otherFileList.Add(fileName);
            }

            // 拷贝补丁清单哈希文件
            {
                string fileName = YooAssetSettingsData.GetPackageHashFileName(PathUtil.YOOASSET_PACKAGE_NAME, packageVersion);
                otherFileList.Add(fileName);
            }

            // 拷贝补丁清单版本文件
            {
                string fileName = YooAssetSettingsData.GetPackageVersionFileName(PathUtil.YOOASSET_PACKAGE_NAME);
                otherFileList.Add(fileName);
            }

            return otherFileList;
        }

        public static List<OssUploadBean> cacheUploadList;
        //预热
        public static void pushCDNUpdateFileList(List<OssUploadBean> beanList)
        {
            cacheUploadList = beanList;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < beanList.Count; i++)
            {
                OssUploadBean ossUploadBean = beanList[i];
                string url = $"https://{BuildAppInfo.aliyunCDN_host}/{ossUploadBean.CdnPath}";
                stringBuilder.Append(url).Append("\n");
            }
            string allStr = stringBuilder.ToString();
            CdnTool.PushObjectCacheRequest(BuildAppInfo.aliyunOSS_accessKeyId, BuildAppInfo.aliyunOSS_accessKeySecret, allStr);
            Debug.Log("Update CDN-updateList Finish!");
        }

        //刷新
        public static void refreshCDNUpdateDirectory(List<OssUploadBean> beanList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < beanList.Count; i++)
            {
                OssUploadBean ossUploadBean = beanList[i];
                string url = $"https://{BuildAppInfo.aliyunCDN_host}/{ossUploadBean.CdnPath}";
                stringBuilder.Append(url).Append("\n");
            }
            string allStr = stringBuilder.ToString();
            CdnTool.RefreshObjectCachesRequest(BuildAppInfo.aliyunOSS_accessKeyId, BuildAppInfo.aliyunOSS_accessKeySecret, allStr, "File");
            Debug.Log("Update CDN-updateList Finish!");
        }
    }
}