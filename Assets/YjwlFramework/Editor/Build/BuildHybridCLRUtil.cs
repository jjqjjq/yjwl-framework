#if HYBRIDCLR
using System;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Build
{
    
    public static class BuildHybridCLRUtil
    {
        private static bool _showUI;
        
        public static void buildHotfix(bool showUI, Action exeEndAction = null)
        {
            _showUI = showUI;
            HybridCLRAssetsCommand.CompileHotfixDll();
            
            if (_showUI)
            {
                EditorUtility.ClearProgressBar();
            }
            AssetDatabase.Refresh();
            
            exeEndAction?.Invoke();
            Debug.Log("finish");
        }
        
        public static void buildAOT(bool showUI, Action exeEndAction = null)
        {
            _showUI = showUI;
            HybridCLRAssetsCommand.CompileAotDll();
            
            if (_showUI)
            {
                EditorUtility.ClearProgressBar();
            }
            AssetDatabase.Refresh();
            
            exeEndAction?.Invoke();
            Debug.Log("finish");
        }
        
        
        
        public static void copyAndBindAllDllToLib(bool showUI, Action exeEndAction = null)
        {
            _showUI = showUI;
            HybridCLRAssetsCommand.CopyAndBindAllDllToLib();
            
            if (_showUI)
            {
                EditorUtility.ClearProgressBar();
            }
            AssetDatabase.Refresh();
            
            exeEndAction?.Invoke();
            Debug.Log("finish");
        }
    }
}
#endif