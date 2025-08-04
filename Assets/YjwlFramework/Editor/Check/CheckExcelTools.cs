using System;
using System.Collections.Generic;
using JQCore.tString;
using JQCore.tUtil;
using JQEditor.Build;
using JQEditor.Excel;
#if YJWL_P3
using JQEditor.Game;
#endif
using UnityEngine;

namespace JQEditor.Check
{
    public static class CheckExcelTools
    {
        private static string MAP_CONFIG_EXCEL = $"../{BuildAppInfo.ProductNick}-Design/Excel/Map#地图表.xlsx";
        private static string CARD_CONFIG_EXCEL = $"../{BuildAppInfo.ProductNick}-Design/Excel/Card#卡牌表.xlsx";

        private class MapConfigExcelData
        {
            [JQExcelColumn] public int id { get; set; } //资源名
            [JQExcelColumn] public string Info { get; set; }
        }

        private class CardConfigExcelData
        {
            [JQExcelColumn] public int id { get; set; } //资源名
            [JQExcelColumn] public int chapter { get; set; }
            [JQExcelColumn] public string img { get; set; }
            [JQExcelColumn] public int quality { get; set; }
            [JQExcelColumn] public int reward { get; set; }
        }

        public static void Map2Excel(string name, Action endAction)
        {
#if YJWL_P3
            JQExcelMapper<MapConfigExcelData> jqExcelMapper = new JQExcelMapper<MapConfigExcelData>(MAP_CONFIG_EXCEL, 5, 4);
            jqExcelMapper.ReadFromExcel();

            List<MapConfigExcelData> newDataList = new List<MapConfigExcelData>();

            CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}/Map/Info", loadOneMapInfo, newDataList, () =>
            {
                newDataList.Sort(((a, b) => a.id.CompareTo(b.id)));
                jqExcelMapper.DataList = newDataList;
                jqExcelMapper.WriteToExcel();
                endAction?.Invoke();
            });
#endif
        }

        private static bool loadOneMapInfo(string assetPath, GameObject cloneGo, object obj1)
        {
#if YJWL_P3
            int id = StringUtil.ExtractEndNumber(cloneGo.name);

            List<MapConfigExcelData> newDataList = obj1 as List<MapConfigExcelData>;
            CubePillarInfoEditor cubePillarInfoEditor = cloneGo.GetComponentInChildren<CubePillarInfoEditor>();
            if (cubePillarInfoEditor != null)
            {
                string mapInfo = cubePillarInfoEditor.PrintMap();
                newDataList.Add(new MapConfigExcelData()
                {
                    id = id,
                    Info = mapInfo
                });
            }
#endif
            return true;
        }


        private static int idIndex = 0;
        public static void Card2Excel(string name, Action endAction)
        {
            JQExcelMapper<CardConfigExcelData> jqExcelMapper = new JQExcelMapper<CardConfigExcelData>(CARD_CONFIG_EXCEL, 5, 4);
            jqExcelMapper.ReadFromExcel();

            List<CardConfigExcelData> newDataList = new List<CardConfigExcelData>();
            idIndex = 0;
            CheckCommonTools.Search<Texture2D>(name, $"Assets/{PathUtil.RES_FOLDER}/UI/ProgramLoad/UITextureLoaderCode/Egg", loadOneCardInfo,
                newDataList, () =>
                {
                    newDataList.Sort(((a, b) => a.id.CompareTo(b.id)));
                    jqExcelMapper.DataList = newDataList;
                    jqExcelMapper.WriteToExcel();
                    endAction?.Invoke();
                }, true, ".png", "t:texture2D");
        }

        private static bool loadOneCardInfo(string assetPath, Texture2D texture2D, object obj1)
        {
            idIndex++;
            Debug.Log($"index: {idIndex}");
            List<CardConfigExcelData> newDataList = obj1 as List<CardConfigExcelData>;
            newDataList.Add(new CardConfigExcelData()
            {
                id = idIndex,
                chapter = (idIndex-1)/9+1,
                img = texture2D.name,
                quality = (idIndex-1)%9 > 2?2:1,
                reward = (idIndex-1)/9+1,
            });


            return true;
        }
    }
}