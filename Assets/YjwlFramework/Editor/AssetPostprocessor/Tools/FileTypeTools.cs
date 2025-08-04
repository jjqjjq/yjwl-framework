using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace JQEditor.tAssetPostprocessor
{
    public class FileTypeTools
    {
        private static List<string> fbx_type = new List<string>() { ".fbx", ".FBX" };
        private static List<string> texture_type = new List<string>() { ".jpg", ".png", ".exr" };

        private static List<string> config_type = new List<string>() { ".xml" };

        private static List<string> audio_type = new List<string>() { ".mp3", ".wav" };

        private static List<string> anim_type = new List<string>() { ".anim" };
        
        private static List<string> atlas_type = new List<string>() { ".spriteatlasv2", ".spriteatlas" };

        //判断是否是项目文件夹
        public static bool IsFBX(string assetsPath)
        {
            string ext = Path.GetExtension(assetsPath);
            return fbx_type.Contains(ext);
        }

        //判断是否是项目文件夹
        public static bool IsTexture(string assetsPath)
        {
            string ext = Path.GetExtension(assetsPath);
            return texture_type.Contains(ext);
        }

        //判断是否是xml配置
        public static bool IsConfig(string assetsPath)
        {
            string ext = Path.GetExtension(assetsPath);
            return config_type.Contains(ext);
        }

        //判断是否是xml配置
        public static bool IsAudio(string assetsPath)
        {
            string ext = Path.GetExtension(assetsPath);
            return audio_type.Contains(ext);
        }

        public static bool IsAnim(string assetPath)
        {
            string ext = Path.GetExtension(assetPath);
            return anim_type.Contains(ext);
        }

        public static bool IsAtlas(string assetPath)
        {
            
            string ext = Path.GetExtension(assetPath);
            return atlas_type.Contains(ext);
        }
    }
}