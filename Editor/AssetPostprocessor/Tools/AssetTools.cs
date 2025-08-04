using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

namespace JQEditor.tAssetPostprocessor
{
    public class AssetTools
    {
        //被4整除
        public static bool IsDivisibleOf4(TextureImporter importer, int maxSize)
        {
            Vector2 v2 = GetTextureImporterSize(importer, maxSize);
            return (v2.x % 4 == 0 && v2.y % 4 == 0);
        }

        //2的整数次幂
        public static bool IsPowerOfTwo(TextureImporter importer, int maxSize)
        {
            Vector2 v2 = GetTextureImporterSize(importer, maxSize);
            return (v2.x == v2.y) && (v2.x > 0) && ((int.Parse(v2.x.ToString()) & (int.Parse(v2.x.ToString()) - 1)) == 0);
        }

        //贴图不存在、meta文件不存在、图片尺寸发生修改需要重新导入
        public static bool IsFirstImport(TextureImporter importer)
        {
            Vector2 v2 = GetTextureImporterSize(importer);
            Texture tex = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
            bool hasMeta = File.Exists(AssetDatabase.GetAssetPathFromTextMetaFilePath(importer.assetPath));
            return tex == null || !hasMeta || (tex.width != v2.x && tex.height != v2.y);
        }

        //获取导入图片的宽高
        public static Vector2 GetTextureImporterSize(TextureImporter importer, int maxSize = 1024, bool showError = true)
        {
            Vector2 v2 = new Vector2(0, 0);
            if (importer != null)
            {
                object[] args = new object[2];
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);
                v2.x = (int)args[0];
                v2.y = (int)args[1];
                if (showError && (v2.x > maxSize || v2.y > maxSize))
                {
                    Debug.LogError($"图片大小超过限制：{importer.assetPath} 限制为：{maxSize} 当前为：{v2.x}*{v2.y}");
                }
            }

            return v2;
        }
    }
}