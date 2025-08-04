using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JQCore.tFileSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveRT2PNG : MonoBehaviour
{
    public GameObject[] prefabs;
    public Camera camera;
    public Transform parent;
    public RawImage rawImage;

    public string typeStr;

    public GameObject objGo;

    public int index = -1;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            index++;
            if(index >= prefabs.Length)
            {
                index = 0;
            }
            ShowNextObj();
            SavePng();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            index--;
            if(index < 0)
            {
                index = prefabs.Length - 1;
            }
            ShowNextObj();
            SavePng();
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SavePng();
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartSave();
        }
    }

    private void ShowNextObj()
    {
        if(objGo != null)
        {
            Object.DestroyImmediate(objGo);
        }
        objGo = Object.Instantiate(prefabs[index], Vector3.zero, Quaternion.identity);
        objGo.transform.SetParent(parent);
        objGo.transform.localPosition = Vector3.zero;
        objGo.transform.localScale = Vector3.one;
        objGo.transform.localRotation = Quaternion.identity;
        Selection.activeGameObject = parent.gameObject;
    }

    private string toFullString100(int index)
    {
        if (index < 10)
        {
            return "00" + index;
        }
        else if (index < 100)
        {
            return "0" + index;
        }
        else
        {
            return index.ToString();
        }
    }

    private string toFullString10(int index)
    {
        if (index < 10)
        {
            return "0" + index;
        }
        else
        {
            return index.ToString();
        }
    }


    private void SavePng()
    {
        JQFileUtil.CreateDirectory(Application.dataPath + "/../" + typeStr);
        camera.Render();
        string indexStr = "";
        if (prefabs.Length > 100)
        {
            indexStr = toFullString10(index+1);
        }
        else
        {
            indexStr = toFullString100(index+1);
        }
        SaveAsPng(camera.targetTexture, $"{Application.dataPath}/../{typeStr}/{typeStr}_{indexStr}.png");
        
    }

    private void StartSave()
    {
        JQFileUtil.CreateDirectory(Application.dataPath + "/../" + typeStr);
        for (int i = 0; i < prefabs.Length; i++)
        {
            if(objGo != null)
            {
                Object.DestroyImmediate(objGo);
            }
            objGo = Object.Instantiate(prefabs[i], Vector3.zero, Quaternion.identity);
            objGo.transform.SetParent(parent);
            objGo.transform.localPosition = Vector3.zero;
            objGo.transform.localScale = Vector3.one;
            camera.Render();
            if (i < 9)
            {
                SaveAsPng(camera.targetTexture, $"{Application.dataPath}/../{typeStr}/{typeStr}_0{i+1}.png");
            }
            else
            {
                SaveAsPng(camera.targetTexture, $"{Application.dataPath}/../{typeStr}/{typeStr}_{i+1}.png");
            }
        }
    }


    private void rawImg2Png(RawImage rawImage)
    {
        SaveAsPng(rawImage.mainTexture as RenderTexture, Application.dataPath + "/../222.png");
    }

    public static bool SaveAsPng(RenderTexture source, string filename)
    {
        // 备份当前的RenderTexture
        RenderTexture previous = RenderTexture.active;

        // 创建一个新的RenderTexture实例
        RenderTexture tempRT = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            source.format
        );

        // 激活新的RenderTexture
        Graphics.Blit(source, tempRT);
        RenderTexture.active = tempRT;

        // 创建一个新的2D纹理来保存RenderTexture的像素数据
        Texture2D screenShot = new Texture2D(tempRT.width, tempRT.height, TextureFormat.ARGB32, false);
        screenShot.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        screenShot.Apply();

        // 恢复之前的RenderTexture
        RenderTexture.active = previous;

        // 释放临时的RenderTexture
        RenderTexture.ReleaseTemporary(tempRT);

        // 将Texture2D保存为PNG文件
        byte[] bytes = screenShot.EncodeToPNG();
        if (bytes != null)
        {
            File.WriteAllBytes(filename, bytes);
            return true;
        }
        else
        {
            return false;
        }
    }
}