#if UNITY_IOS && UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif

namespace JQEditor.Build
{
    public static class XCodePostProcessBuild
    {
#if UNITY_IOS && UNITY_EDITOR
    private static readonly string[] csAddFrameworks = new string[]{
        "Security.framework","WebKit.framework", "CoreGraphics.framework"
    };
 
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if(BuildTarget.iOS != buildTarget)
        {
            return;
        }
        string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        SetFrameworksAndBuildSettings(projectPath);
        SetInfoList(pathToBuiltProject, "wxUrlName", "wxScheme");
        SetAssociatedDomains(projectPath, "ssz2.gz-spi.com/ssz2-bucket/ios");
    }
 
    private static void SetFrameworksAndBuildSettings(string path)
    {
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(path));
        string target = proj.GetUnityMainTargetGuid();
        Debug.Log("Target Name is " + target);
        // 设置 BuildSettings
        proj.AddBuildProperty(target, "Other Linker Flags", "-Objc -all_load");
        proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
 
        //根据微信SDK文档的要求，加入相关的Frameworks
        for (int i = 0; i < csAddFrameworks.Length; ++i)
        {
            if (!proj.ContainsFramework(target, csAddFrameworks[i]))
                proj.AddFrameworkToProject(target, csAddFrameworks[i], false);
        }
 
        File.WriteAllText(path, proj.WriteToString());        
    }
 
    private static void SetInfoList(string buildPath, string wxUrlName, string wxScheme)
    {
        string listPath = buildPath + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(listPath));
 
        plist.root.SetString("MOBAppkey", "appKey");
        plist.root.SetString("MOBAppSecret", "appSecret");
        
        // 在“info”标签栏的“URL type“添加“URL scheme”,值为你在微信后台注册的应用程序的 AppID
        PlistElementArray urlArray = plist.root.CreateArray("CFBundleURLTypes");
        PlistElementDict dict = urlArray.AddDict();
        dict.SetString("CFBundleTypeRole", "Editor");
        dict.SetString("CFBundleURLName", wxUrlName);
        PlistElementArray urlSchemes = dict.CreateArray("CFBundleURLSchemes");
        urlSchemes.AddString(wxScheme);
        
        // 在 “info”标签栏的“LSApplicationQueriesSchemes“添加weixin wechat和weixinULAPI
        PlistElementArray wxArray = plist.root.CreateArray("LSApplicationQueriesSchemes");
        wxArray.AddString("weixin");
        wxArray.AddString("wechat");
        wxArray.AddString("weixinULAPI");
 
 
        File.WriteAllText(listPath, plist.WriteToString());
    }
 
    // 设置Associated Domains
    private static void SetAssociatedDomains(string pbxProjectPath, string domainUrl)
    {
        //默认 Target Name, 你自己的可能不一样
        string targetName = "Unity-iPhone";
        //Set the entitlements file name to what you want but make sure it has this extension
        string entitlementsFileName = "poo";
 
        var entitlements = new ProjectCapabilityManager(pbxProjectPath, entitlementsFileName, targetName);
        entitlements.AddAssociatedDomains(new string[] { "applinks:" + domainUrl });
        
        entitlements.WriteToFile();
    }

#endif
    }
}