using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#if UNITY
using JQCore.tCfg;
#endif
using Newtonsoft.Json.Linq;

namespace ET.UOS
{
    public static class UosUtil
    {
        //
        // [StaticField]
        // private static string appId = "4db73d3d-302c-41ef-9460-adf5f60c0378"; // 替换为您的应用ID
        // [StaticField]
        // private static string appServiceSecret = "042a4c51c52743649c57b48ba6a60816"; // 替换为您的应用服务秘钥
        
        
        [StaticField]
        private static string url = "https://p.unity.cn"; // 替换为您要请求的URL
        
        //正式环境
        [StaticField]
        private static string appId = "eae56e8f-be2f-4204-b936-07807d7b2fb6"; // 替换为您的应用ID
        [StaticField]
        private static string appServiceSecret = "4bdbb33c9aea460181d6e78491e05b4c"; // 替换为您的应用服务秘钥
        
        //测试环境
        [StaticField]
        private static string develop_appId = "62a9fcfd-7ea6-4663-8c1c-76862752371d"; // 替换为您的应用ID
        [StaticField]
        private static string develop_appServiceSecret = "2be68a5d54b1435986d8a064b36747f5"; // 替换为您的应用服务秘钥
        
        
        [StaticField]
        private static HttpClient _httpClient;

        private static HttpClient httpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
#if UNITY
                    if (!AppConfig.IsDebug) //非开发模式
#else
                    if (Options.Instance.Develop == 0) //非开发模式
#endif
                    
                    {
                        string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(appId + ":" + appServiceSecret));
                        _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + token);
                    }
                    else
                    {
                        string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(develop_appId + ":" + develop_appServiceSecret));
                        _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + token);
                    }
                }
                return _httpClient;
            }
        }

        public static async Task<T> HttpPost<T>(string apiPath, Dictionary<string, string> requestParamsDic)
        {
            T responseModel = await httpClient.PostAsyncExtens<T>($"{url}{apiPath}", requestParamsDic);
            return responseModel;
        }

        public static async Task<T> HttpGet<T>(string apiPath, Dictionary<string, string> requestParamsDic)
        {
            T responseModel = await httpClient.GetAsyncExtens<T>($"{url}{apiPath}", requestParamsDic);
            return responseModel;
        }

        public static async Task<V> HttpPut<T, V>(string apiPath, T body)
        {
            V responseModel = await httpClient.PutAsyncExtens<T, V>($"{url}{apiPath}", body);
            return responseModel;
        }

        public static async Task<T> HttpDelete<T>(string apiPath)
        {
            T responseModel = await httpClient.DeleteAsyncExtens<T>($"{url}{apiPath}");
            return responseModel;
        }
    }
}