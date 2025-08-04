using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ET.UOS
{
    public static class NewtonsoftHttpClientExtensions
    {
        [StaticField]
        private static TimeSpan requestTimeout = TimeSpan.FromSeconds(1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="uri"></param>
        /// <param name="requestParamsDic"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> GetAsyncExtens<T>(this HttpClient httpClient, string uri, IDictionary<string, string> requestParamsDic)
        {
            using (var tokenSource = new CancellationTokenSource(requestTimeout))
            {
                uri += ToQueryString(requestParamsDic);

                ThrowIfInvalidParams(httpClient, uri);

                var response = await httpClient.GetAsync(uri, tokenSource.Token);
                if (!response.IsSuccessStatusCode)
                {
                    Log.Console($"error:{response.StatusCode}");
                }
                else
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Log.Console($"Get Finish:{json}");
                    try
                    {
                        T t = JsonConvert.DeserializeObject<T>(json);
                        return t;
                    }
                    catch (Exception e)
                    {
                        Log.Console($"JsonError:{e}");
                    }
                }
            }

            return default;
        }

        /// <summary>
        /// 发起一个http put请求
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="uri"></param>
        /// <param name="body">更新示例</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public static async Task<V> PutAsyncExtens<T, V>(this HttpClient httpClient, string uri, T body)
        {
            using (var tokenSource = new CancellationTokenSource(requestTimeout))
            {
                ThrowIfInvalidParams(httpClient, uri);

                //请求参数转换为json
                string requestJsonStr = JsonConvert.SerializeObject(body);
                HttpContent httpContent = new StringContent(requestJsonStr, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync(uri, httpContent, tokenSource.Token);
                if (!response.IsSuccessStatusCode)
                {
                    Log.Console($"error:{response.StatusCode}");
                }
                else
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<V>(responseJson);
                }
            }

            return default;
        }

        /// <summary>
        /// 发起一个http put请求
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="uri"></param>
        /// <param name="requestParamsDic">更新字典</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> PutAsyncExtens<T>(this HttpClient httpClient, string uri, IDictionary<string, string> requestParamsDic)
        {
            using (var tokenSource = new CancellationTokenSource(requestTimeout))
            {
                ThrowIfInvalidParams(httpClient, uri);

                //请求参数转换为json
                var requestJson = new JObject();
                foreach (var requestParam in requestParamsDic) //遍历字典
                {
                    requestJson.Add(requestParam.Key, requestParam.Value);
                }

                string requestJsonStr = requestJson.ToString();
                HttpContent httpContent = new StringContent(requestJsonStr, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync(uri, httpContent, tokenSource.Token);
                if (!response.IsSuccessStatusCode)
                {
                    Log.Console($"error:{response.StatusCode}");
                }
                else
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseJson);
                }
            }

            return default;
        }

        /// <summary>
        /// 发起一个http delete请求 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="uri">key在uri里面</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> DeleteAsyncExtens<T>(this HttpClient httpClient, string uri)
        {
            using (var tokenSource = new CancellationTokenSource(requestTimeout))
            {
                ThrowIfInvalidParams(httpClient, uri);
                var response = await httpClient.DeleteAsync(uri, tokenSource.Token);
                if (!response.IsSuccessStatusCode)
                {
                    Log.Console($"error:{response.StatusCode}");
                }
                else
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseJson);
                }
            }

            return default;
        }

        /// <summary>
        /// 发起一个http post请求
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="uri"></param>
        /// <param name="requestParamsDic"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> PostAsyncExtens<T>(this HttpClient httpClient, string uri, IDictionary<string, string> requestParamsDic)
        {
            using (var tokenSource = new CancellationTokenSource(requestTimeout))
            {
                ThrowIfInvalidParams(httpClient, uri);

                //请求参数转换为json
                var requestJson = new JObject();
                foreach (var requestParam in requestParamsDic) //遍历字典
                {
                    requestJson.Add(requestParam.Key, requestParam.Value);
                }

                string requestJsonStr = requestJson.ToString();
                HttpContent httpContent = new StringContent(requestJsonStr, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(uri, httpContent, tokenSource.Token);
                if (!response.IsSuccessStatusCode)
                {
                    Log.Console($"error:{response.StatusCode}");
                }
                else
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseJson);
                }
            }

            return default;
        }

        private static string ToQueryString(IDictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return "";
            }

            var builder = new UriBuilder();
            var query = new System.Collections.Specialized.NameValueCollection();
            foreach (var parameter in parameters)
            {
                query[parameter.Key] = parameter.Value;
            }

            builder.Query = string.Join("&", query.AllKeys.Select(key => $"{key}={query[key]}"));
            return builder.Query;
        }

        private static void ThrowIfInvalidParams(HttpClient httpClient, string uri)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof (httpClient));
            }

            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentException("Can't be null or empty", nameof (uri));
            }
        }
    }
}