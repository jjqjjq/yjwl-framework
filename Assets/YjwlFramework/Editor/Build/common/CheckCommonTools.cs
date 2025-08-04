using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JQEditor.Build
{
    public static class CheckCommonTools
    {
        public static void SearchAndDo(string name, string searchPath, CheckCommonInfo<GameObject>.CheckCommonInfoAction action, object obj1, Action endAction, bool showUI = true, string extension = ".prefab",
            string filter = "t:gameObject")
        {
            var checkCommonInfo = new CheckCommonInfo<GameObject>(name, searchPath, action, obj1, extension, filter, showUI);
            checkCommonInfo.SearchAndDo(endAction);
        }


        public static void Search<T>(string name, string searchPath, CheckCommonInfo<T>.CheckCommonInfoAction action, object obj1, Action endAction, bool showUI = true, string extension = ".prefab", string filter = "t:gameObject")
            where T : Object
        {
            var checkCommonInfo = new CheckCommonInfo<T>(name, searchPath, action, obj1, extension, filter, showUI);
            checkCommonInfo.Search(endAction);
        }


//        [MenuItem("IrfTools/CommonTest")]
//        public static void DoOne()
//        {
//            GameObject obj = Selection.activeGameObject;
//            CheckEffectTools.changeOneShader(null, obj, null);
//        }
    }
}