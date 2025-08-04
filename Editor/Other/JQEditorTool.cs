using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class JQEditorTool
{
    /// <summary>
    /// 获取带继承关系的所有类的类型
    /// </summary>
    public static List<Type> GetAssignableTypes(System.Type parentType)
    {
        TypeCache.TypeCollection collection = TypeCache.GetTypesDerivedFrom(parentType);
        return collection.ToList();
    }
}
