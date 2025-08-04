using System.Collections.Generic;
using UnityEngine;

namespace JQFramework.tMgr
{
    public static class LayerMgr
    {
        public static int[] NormalLayers;
        public static int[] UILayers;
        public static int[] UIWorld;

        public static void init(string[] gameLayers)
        {
            ELayer.Default = LayerMask.NameToLayer("Default");
            ELayer.Ground = LayerMask.NameToLayer("Ground");
            ELayer.Transparent = LayerMask.NameToLayer("TransparentFX");
            ELayer.UI = LayerMask.NameToLayer("UI");
            ELayer.Water = LayerMask.NameToLayer("Water");
            ELayer.Story = LayerMask.NameToLayer("Story");
            ELayer.UIWorld = LayerMask.NameToLayer("UIWorld");
            ELayer.UIMesh = LayerMask.NameToLayer("UIMesh");

            List<int> normalLayers = new List<int>();
            normalLayers.Add(ELayer.Default);
            foreach (var layer in gameLayers)
            {
                int layerInt = LayerMask.NameToLayer(layer);
                ELayer.layerDic[layer] = layerInt;
                normalLayers.Add(layerInt);
            }

            ELayer.RenderHigh = LayerMask.NameToLayer("RenderHigh");
            ELayer.RenderMiddle = LayerMask.NameToLayer("RenderMiddle");
            ELayer.RenderLow = LayerMask.NameToLayer("RenderLow");
            ELayer.NotRender = LayerMask.NameToLayer("NotRender");
            NormalLayers = normalLayers.ToArray();
            UILayers = new[] { ELayer.UI };
            UIWorld = new[] { ELayer.UIWorld };
        }


        public static int GetMaskValue(int[] layerArr)
        {
            int maskLayer = 0;
            for (int i = 0; i < layerArr.Length; i++)
            {
                maskLayer = maskLayer | 1 << layerArr[i];
            }

            return maskLayer;
        }
    }

    public static class ELayer
    {
        public static Dictionary<string, int> layerDic = new Dictionary<string, int>();
        public static int Default;
        public static int Ground;
        public static int Transparent;
        public static int UI;
        public static int Water;
        public static int Story;
        public static int UIWorld;
        public static int UIMesh;


        public static int RenderHigh;
        public static int RenderMiddle;
        public static int RenderLow;
        public static int NotRender;
    }
}