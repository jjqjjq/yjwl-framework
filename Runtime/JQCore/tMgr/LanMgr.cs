
// using I2.Loc;

using JQCore.tMgr;
using JQCore.tRes;
using UnityEngine;
using UnityEngine.UI;


namespace JQCore.tMgr
{

    public class LanMgr
    {

        public static void setTextAsset(Object o )
        {
            TextAsset text = (TextAsset)o;
            if (text != null)
            {
                // LocalizationManager.Sources[0].Import_CSV(string.Empty, text.text);
                // LocalizationManager.CurrentLanguage = "Value";
            }
        }

        public static void setAtlas(Object o)
        {

            GameObject text = (GameObject)o;
            if (text != null)
            {
                LanMgr.SetAtlas(text);
            }
        }


        private static string _currLan = "zh_CN";
        private static GameObject curAtlas;

        public static void SetAtlas(GameObject go)
        {
            curAtlas = go;
        }

        public static Sprite GetImage(string name)
        {
            if (curAtlas == null)
            {
//                Debug.LogError("多语言图集缺失" + name);
                return null;
            }

            Sprite img = curAtlas.GetComponent<AssetObjectLib>().GetAsset(name) as Sprite;
            if (img == null)
            {
//                Debug.LogError("素材缺失" + name);
                return null;
            }
            else
            {
                return img;
            }
        }

        public static string GetCurrLan()
        {
            return CurrLan;
        }

        public const string LOCAL_LANGUAGE = "LOCAL_LANGUAGE";//当前本地语言
        public static string CurrLan //当前语言种类
        {
            set
            {
                LocalVarManager.SetString(LOCAL_LANGUAGE, value);
                //string[] Languages = LocalizationManager.GetAllLanguages().ToArray();
                //LocalizationManager.CurrentLanguage = Languages[value];
            }
            get
            {
                //if (_currLan == "")
                //{
                //    _currLan = LocalVarManager.GetString(LocalVarConst.LOCAL_LANGUAGE, "zh_CN"); ;
                //}
                LocalVarManager.GetString(LOCAL_LANGUAGE, "zh_CN");
                return _currLan;
            }
        }

        public static void setStr(Text text, string key)
        {
            // LanguageSource.ValidateFullTerm(ref key);
            // Localize localize = text.GetComponent<Localize>();
            // if (localize == null)
            // {
            //     localize = text.gameObject.AddComponent<Localize>();
            // }
            //
            // localize.SetTerm(key, key);
        }

        public static string GetStr(string key)
        {
            return key;
            // TermData termData = LocalizationManager.GetTermData(key);
            // if (termData == null)
            // {
            //     return key;
            // }
            //
            // return termData.Languages[0]; //只加载一中语言
        }
    }
}
