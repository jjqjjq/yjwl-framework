using System.Collections.Generic;
using System.IO;
using System.Text;
using JQCore.tFileSystem;
using JQCore.tRes;
using UnityEditor;
using UnityEngine;

namespace JQEditor.MVC
{
    public class MVCEditorUtil
    {
       
        
        [MenuItem("CONTEXT/Component/添加到BindObjLib")]
        public static void AddToBindObjLib(MenuCommand menuCommand)
        {
            Component component = menuCommand.context as Component;
            BindObjLib bindObjLib = component.transform.parent.GetComponentInParent<BindObjLib>();
            if (bindObjLib == null)
            {
                Debug.LogError($"未找到BindObjLib组件,请检查{component.name}的父节点是否有BindObjLib组件");
                return;
            }

            bindObjLib.AddObjEditor(component, component.transform, false);
        }

        [MenuItem("CONTEXT/Component/添加到BindObjLib(附加组件类型)")]
        public static void AddToBindObjLibType(MenuCommand menuCommand)
        {
            Component component = menuCommand.context as Component;
            BindObjLib bindObjLib = component.transform.parent.GetComponentInParent<BindObjLib>();
            if (bindObjLib == null)
            {
                Debug.LogError($"未找到BindObjLib组件,请检查{component.name}的父节点是否有BindObjLib组件");
                return;
            }

            bindObjLib.AddObjEditor(component, component.transform, true);
        }

        [MenuItem("CONTEXT/Component/添加Go到BindObjLib")]
        public static void AddGoToBindObjLib(MenuCommand menuCommand)
        {
            Component component = menuCommand.context as Component;
            BindObjLib bindObjLib = component.transform.parent.GetComponentInParent<BindObjLib>();
            if (bindObjLib == null)
            {
                Debug.LogError($"未找到BindObjLib组件,请检查{component.name}的父节点是否有BindObjLib组件");
                return;
            }

            bindObjLib.AddObjEditor(component.gameObject, component.transform, true);
        }
        
    }
}