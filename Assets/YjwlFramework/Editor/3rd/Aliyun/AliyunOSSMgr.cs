using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Aliyun.OSS;
using Aliyun.OSS.Common;

public class AliyunOSSMgr
{
    // yourEndpoint填写Bucket所在地域对应的Endpoint。以华东1（杭州）为例，Endpoint填写为https://oss-cn-hangzhou.aliyuncs.com。
    private string _endpoint = "yourEndpoint";

    // 阿里云账号AccessKey拥有所有API的访问权限，风险很高。强烈建议您创建并使用RAM用户进行API访问或日常运维，请登录RAM控制台创建RAM用户。
    private string _accessKeyId = "yourAccessKeyId";
    private string _accessKeySecret = "yourAccessKeySecret";

    // yourBucketName填写Bucket名称。
    private string _bucketName = "yourBucketName";
    
    private OssClient _client;

    public AliyunOSSMgr(string endpoint, string accessKeyId, string accessKeySecret, string bucketName)
    {
        _endpoint = endpoint;
        _accessKeyId = accessKeyId;
        _accessKeySecret = accessKeySecret;
        _bucketName = bucketName;
        // 创建OSSClient实例。
        _client = new OssClient(_endpoint, _accessKeyId, _accessKeySecret);
    }

    

    public void CreateBucket()
    {
        try
        {
            // 创建存储空间。
            var bucket = _client.CreateBucket(_bucketName);
            Console.WriteLine("Create bucket succeeded, {0} ", bucket.Name);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Create bucket failed, {0}", ex.Message);
        }
    }

    // 填写Object完整路径，完整路径中不能包含Bucket名称，例如exampledir/exampleobject.txt。
    // var objectName = "exampledir/exampleobject.txt";
    // 填写本地文件完整路径，例如D:\\localpath\\examplefile.txt。如果未指定本地路径，则默认从示例程序所属项目对应本地路径中上传文件。
    // var localFilename = "D:\\localpath\\examplefile.txt";
    public void Upload(string ossFilePath, string localFilePath, EventHandler<StreamTransferProgressArgs> streamProgressCallback, Action onError)
    {
        try
        {
            // 上传文件。
            var result = _client.PutObject(_bucketName, ossFilePath, localFilePath);
            Debug.Log($"Put object succeeded, ETag: {result.ETag} ");
            
            using (var fs = File.Open(localFilePath, FileMode.Open))
            {
                var putObjectRequest = new PutObjectRequest(_bucketName, ossFilePath, fs);
                putObjectRequest.StreamTransferProgress += streamProgressCallback;
                _client.PutObject(putObjectRequest);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Put object failed, {ex.Message}");
            onError?.Invoke();
        }
    }
    


    // 填写Object完整路径，完整路径中不能包含Bucket名称，例如exampledir/exampleobject.txt。
    //var objectName = "exampledir/exampleobject.txt";
    // 下载Object到本地文件examplefile.txt，并保存到指定的本地路径中（D:\\localpath）。如果指定的本地文件存在会覆盖，不存在则新建。
    // 如果未指定本地路径，则下载后的文件默认保存到示例程序所属项目对应本地路径中。
    //var downloadFilename = "D:\\localpath\\examplefile.txt";
    public void Download(string ossFilePath, string localFilePath)
    {
        try
        {
            // 下载文件。
            var result = _client.GetObject(_bucketName, ossFilePath);
            using (var requestStream = result.Content)
            {
                using (var fs = File.Open(localFilePath, FileMode.OpenOrCreate))
                {
                    int length = 4 * 1024;
                    var buf = new byte[length];
                    do
                    {
                        length = requestStream.Read(buf, 0, length);
                        fs.Write(buf, 0, length);
                    } while (length != 0);
                }
            }

            Debug.Log("Get object succeeded");
        }
        catch (OssException ex)
        {
            Debug.LogError($"Failed with error code: {ex.ErrorCode}; Error info: {ex.Message}. \nRequestID:{ex.RequestId}\tHostID:{ex.HostId}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed with error info: {ex.Message}");
        }
    }
    
    public List<string> ListObjects(string prefix = null)
    {
        List<string> objects = new List<string>();
        try
        {
            // 列举文件。
            var req = new ListObjectsRequest(_bucketName) {Prefix = prefix, MaxKeys = 999999};
            var result = _client.ListObjects(req);
            Debug.Log("List objects succeeded 展示数量："+req.MaxKeys);
            foreach (var summary in result.ObjectSummaries)
            {
                Debug.Log($"file:{summary.Key}");
                objects.Add(summary.Key);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"List objects failed. {ex.Message}");
        }
        return objects;
    }
}