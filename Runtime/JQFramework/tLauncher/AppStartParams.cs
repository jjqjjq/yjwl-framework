using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppStartParams : MonoBehaviour
{
    [Serializable]
    public class AppStartParam
    {
        public string key;
        public string value;
    }

    [SerializeField] public AppStartParam[] appStartParams;

    public Dictionary<string, string> GetParams()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        foreach (var param in appStartParams)
        {
            dic[param.key] = param.value;
        }

        return dic;
    }
}