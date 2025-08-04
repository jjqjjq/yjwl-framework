using System;
using JQCore.tFileSystem;
using JQCore.tSingleton;
using JQCore.tMgr;
using JQFramework.tMgr;
using UnityEditor;
using UnityEngine;

namespace JQFramework.tMVC
{
    public class FilterManager : JQSingleton<FilterManager>
    {
        private static string[] SentiWords = null; //定义一个接受文件内容的字符串数组

        public FilterManager()
        {
            //
        }

        public void LoadWWW()
        {
            string fileTxt = JQFileUtil.ReadFileAllText(Application.streamingAssetsPath + "/" + "SensitiveWords1.txt");
            SentiWords = fileTxt.Split(',');
        }

        public void RemoveWWW()
        {
            if (SentiWords != null)
                SentiWords = null;
        }


        //在Start()函数中开启协程加载文件
        private void Start()
        {
            //添加输入事件监听
            //transform.GetComponent<InputField>().onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 监听方法，该方法会在监测到输入值改变时被触发
        /// </summary>
        /// <param name="t"></param> 参数为当前输入的值
        public string OnValueChanged(string t)
        {
            if (SentiWords == null)
            {
                object temp = CommonResMgr.Instance.GetAsset("SensitiveWords1");

                SentiWords = temp.ToString().Split(',');
            }


            foreach (string ssr in SentiWords)
            {
                if (t.Contains(ssr))
                {
                    if (!ssr.Equals(""))
                    {
                        Debug.Log("包含敏感词汇:" + ssr + ",需要进行替换");

                        //将敏感词替换*
                        string stt = "";
                        int length = ssr.ToCharArray().Length;
                        string s = "";
                        for (int i = 0; i < length; i++)
                            s += "*";
                        stt = ssr.Replace(ssr, s);
                        return stt;
                    }
                }
            }

            return t;
        }
    }
}