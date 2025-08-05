using System;
using System.Collections;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JQCore.tCoroutine;
using JQCore.tLog;
using UnityEngine;
using UnityEngine.Networking;

namespace JQCore.tUtil
{
    public static class HttpUtil
    {
        #region API

        public static void HttpsGet(string url, Action<string, string> onLoadEnd)
        {
            JQCoroutineHandler.Start(httpsGet(url, onLoadEnd));
        }

        public class WebRequestCertificate : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                //return base.ValidateCertificate(certificateData);
                return true;
            }
        }

        public static async Task<string> httpsGetTask(string requestURL, bool appendTimeTicks = true)
        {
            // 在URL末尾添加时间戳
            if (appendTimeTicks)
            {
                requestURL = $"{requestURL}?{System.DateTime.UtcNow.Ticks}";
            }

            var request = new UnityWebRequest(requestURL, UnityWebRequest.kHttpVerbGET);
            request.certificateHandler = new WebRequestCertificate();
            DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
            request.downloadHandler = downloadHandlerBuffer;
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                JQLog.LogError("HttpsGet error:" + request.result);
                return null;
            }

            return downloadHandlerBuffer.text;
        }

        /// <summary>
        /// 异步的扩展方法
        /// </summary>
        public static TaskAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }

        private static IEnumerator httpsGet(string requestURL, Action<string, string> onLoadEnd, bool appendTimeTicks = true)
        {
            string finalRequestURL = requestURL;
            // 在URL末尾添加时间戳
            if (appendTimeTicks)
            {
                finalRequestURL = $"{requestURL}?time={System.DateTime.UtcNow.Ticks}";
            }

            var request = new UnityWebRequest(finalRequestURL, UnityWebRequest.kHttpVerbGET);
            request.certificateHandler = new WebRequestCertificate();
            DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
            request.downloadHandler = downloadHandlerBuffer;
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                onLoadEnd?.Invoke(downloadHandlerBuffer.text, requestURL);
            }
            else
            {
                JQLog.LogError("HttpsGet error:" + request.result + " \n" + finalRequestURL);
            }
        }

        public static void httpGet(string url, Action<string> onLoadEnd)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(
                (object sender, DownloadStringCompletedEventArgs e) =>
                {
                    if (e.Error != null)
                    {
                        JQLog.LogError("httpGet error:" + e.Error);
                        onLoadEnd?.Invoke(null);
                    }
                    else
                    {
                        onLoadEnd?.Invoke(e.Result);
                    }
                });
            webClient.DownloadStringAsync(new Uri(url));
        }

        public static void httpGetSprite(string url, int width, int height, Action<Sprite> onLoadEnd)
        {
            var webClient = new WebClient();
            webClient.DownloadDataCompleted += new DownloadDataCompletedEventHandler(
                (object sender, DownloadDataCompletedEventArgs e) =>
                {
                    if (e.Error != null)
                    {
                        JQLog.LogError("httpGetSprite error:" + e.Error);
                        onLoadEnd?.Invoke(null);
                    }
                    else
                    {
                        Texture2D tex = new Texture2D(width, height);
                        tex.LoadImage(e.Result);
                        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                            new Vector2(0.5f, 0.5f));
                        onLoadEnd?.Invoke(sprite);
                    }
                });
            webClient.DownloadDataAsync(new Uri(url));
        }

        public static void wwwRequestGet(string uri, Action<string> callbackAction)
        {
            JQCoroutineHandler.Start(GetRequest(uri, callbackAction));
        }


        static IEnumerator GetRequest(string uri, Action<string> callbackAction)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                if (webRequest.isNetworkError)
                {
                    JQLog.Log(pages[page] + ": Error: " + webRequest.error);
                }
                else
                {
                    //                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    callbackAction(webRequest.downloadHandler.text);
                }
            }
        }

        public static void wwwRequestPost(string uri, WWWForm form, Action<string> callbackAction)
        {
            JQCoroutineHandler.Start(PostRequest(uri, form, callbackAction));
        }

        static IEnumerator PostRequest(string uri, WWWForm form, Action<string> callbackAction)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    JQLog.Log(www.error);
                }
                else
                {
                    callbackAction(
                        www.downloadHandler.text);
                    JQLog.Log("Form upload complete!");
                }
            }
        }
#if !UNITY_WEBGL
        public static void RealStepLog(string url)
        {
            JQCoroutineHandler.Start(realStepLog(url));
        }

        private static IEnumerator realStepLog(string url)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.error != null)
            {
#if UNITY_ANDROID || UNITY_EDITOR
                JQLog.Log("step日志错误：" + www.error);
#endif
            }
        }
#endif

        #endregion
    }
}