using System;
using System.Collections.Generic;
using Tea;
using UnityEngine;

namespace JQEditor.ThirdParty.Aliyun
{
    public static class CdnTool
    {
        // /// <summary>
        // /// 配置 STS Token
        // /// 通过安全令牌服务（Security Token Service，简称 STS），申请临时安全凭证（Temporary Security Credentials，简称 TSC），创建临时安全凭证。
        // /// </summary>
        // /// <param name="AccessKeyId"></param>
        // /// <param name="AccessKeySecret"></param>
        // /// <param name="SecurityToken"></param>      
        // private static void SetSTSToken(string[] lines)
        // {
        //     string inAccessKeyId = lines[1];
        //     string inAccessKeySecret = lines[2];
        //     string inSecurityToken = lines[3];
        //     
        //     Config config = new Config()
        //     {
        //         Type = "sts", // 凭证类型
        //         AccessKeyId = inAccessKeyId, // AccessKeyId
        //         AccessKeySecret = inAccessKeySecret, // AccessKeySecret
        //         SecurityToken = inSecurityToken // STS Token
        //     };
        //     var stsCredential = new Aliyun.Credentials.Client(config);
        //
        //     string accessKeyId = stsCredential.GetAccessKeyId();
        //     string accessSecret = stsCredential.GetAccessKeySecret();
        //     string credentialType = stsCredential.GetType();
        //     string securityToken = stsCredential.GetSecurityToken();
        //
        //     
        //     Debug.Log($"accessKeyId:{accessKeyId}");
        //     Debug.Log($"accessSecret:{accessSecret}");
        //     Debug.Log($"credentialType:{credentialType}");
        //     Debug.Log($"securityToken:{securityToken}");
        // }

        // This file is auto-generated, don't edit it. Thanks.
        /**
             * 使用AK&SK初始化账号Client
             * @param accessKeyId
             * @param accessKeySecret
             * @return Client
             * @throws Exception
             */
        private static AlibabaCloud.SDK.Cdn20180510.Client CreateClient(string accessKeyId, string accessKeySecret)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                // 必填，您的 AccessKey ID
                AccessKeyId = accessKeyId,
                // 必填，您的 AccessKey Secret
                AccessKeySecret = accessKeySecret
                // //sts token
                // SecurityToken = securityToken
            };
            // 访问的域名
            config.Endpoint = "cdn.aliyuncs.com";
            return new AlibabaCloud.SDK.Cdn20180510.Client(config);
        }
        //
        // private static string getFileListStr()
        // {
        //     string[] fileListLines = File.ReadAllLines("../CDNFileList.config");
        //     StringBuilder stringBuilder = new StringBuilder();
        //     for (int i = 0; i < fileListLines.Length; i++)
        //     {
        //         
        //         stringBuilder.Append(fileListLines[i]);
        //         if (i != fileListLines.Length - 1)
        //         {
        //             stringBuilder.Append("\n");
        //         }
        //     }
        //
        //     return stringBuilder.ToString();
        // }

        /// <summary>
        /// 预热
        /// </summary>
        /// <param name="lines"></param>
        public static void PushObjectCacheRequest(string accessKeyId, string accessKeySecret, string fileList)
        {
            Debug.Log("PushObjectCache:\n" + fileList);

            // 工程代码泄露可能会导致AccessKey泄露，并威胁账号下所有资源的安全性。以下代码示例仅供参考，建议使用更安全的 STS 方式，更多鉴权访问方式请参见：https://help.aliyun.com/document_detail/378671.html
            AlibabaCloud.SDK.Cdn20180510.Client client = CreateClient(accessKeyId, accessKeySecret);
            AlibabaCloud.SDK.Cdn20180510.Models.PushObjectCacheRequest pushObjectCacheRequest =
                new AlibabaCloud.SDK.Cdn20180510.Models.PushObjectCacheRequest
                {
                    Area = "domestic", //仅限中国内地
                    ObjectPath = fileList,
                };
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            AlibabaCloud.SDK.Cdn20180510.Models.PushObjectCacheResponse pushObjectCacheResponse = null;
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                pushObjectCacheResponse = client.PushObjectCacheWithOptions(pushObjectCacheRequest, runtime);
                Debug.Log("PushObjectCache statusCode:" + pushObjectCacheResponse.StatusCode);
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
                Debug.Log(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object> { { "message", _error.Message } });
                // 如有需要，请打印 error
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
                Debug.Log(error.Message);
            }
        }


        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="accessKeyId"></param>
        /// <param name="accessKeySecret"></param>
        /// <param name="fileList"></param>
        /// <param name="objectType">File | Directory | Regex | IgnoreParams</param>
        public static void RefreshObjectCachesRequest(string accessKeyId, string accessKeySecret, string fileList, string objectType = "File")
        {
            Debug.Log("RefreshObjectCaches:\n" + fileList);

            // 工程代码泄露可能会导致AccessKey泄露，并威胁账号下所有资源的安全性。以下代码示例仅供参考，建议使用更安全的 STS 方式，更多鉴权访问方式请参见：https://help.aliyun.com/document_detail/378671.html
            AlibabaCloud.SDK.Cdn20180510.Client client = CreateClient(accessKeyId, accessKeySecret);
            AlibabaCloud.SDK.Cdn20180510.Models.RefreshObjectCachesRequest refreshObjectCachesRequest =
                new AlibabaCloud.SDK.Cdn20180510.Models.RefreshObjectCachesRequest
                {
                    // ObjectPath = "ssz2.gz-spi.com/ssz2-bucket/test.txt",
                    ObjectPath = fileList,
                    ObjectType = objectType
                };
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            AlibabaCloud.SDK.Cdn20180510.Models.RefreshObjectCachesResponse refreshObjectCachesResponse = null;
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                refreshObjectCachesResponse = client.RefreshObjectCachesWithOptions(refreshObjectCachesRequest, runtime);
                Debug.Log("RefreshObjectCaches statusCode:" + refreshObjectCachesResponse.StatusCode);
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
                Debug.Log(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object> { { "message", _error.Message } });
                // 如有需要，请打印 error
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
                Debug.Log(error.Message);
            }
        }
    }
}