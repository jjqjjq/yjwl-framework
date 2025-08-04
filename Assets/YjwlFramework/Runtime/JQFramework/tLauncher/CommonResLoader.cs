// using System.Collections.Generic;
// using Assets.Scripts.framework.Pool.Loader;
// using Assets.Scripts.framework.Res;
// using Assets.Scripts.framework.Res.tShader;
// using Assets.Scripts.framework.YooAsset;
// using Assets.Scripts.Plugins.Loader;
// using ET.Manager.UIModel;
// using Game.Enum;
// using YooAsset;
//
// namespace ET.Manager
// {
//     public class CommonResLoader:BaseLoader
//     {
//         private List<string> _resList = new List<string>();
//         private List<string> _prefabList = new List<string>();
//         
//         public CommonResLoader() : base("CommonResLoader")
//         {
//             //asset --有顺序要求
//             _resList.Add(EAsset.CommonShaders);
//             _resList.Add(EAsset.CommonFont);
//             _resList.Add(EAsset.CommonFontTMP);
//             // _resList.Add(EAsset.CommonFontImg);
//             // _resList.Add(EAsset.CommonAnimaLib);
//             // _resList.Add(EAsset.AtlasCard);
//             // _resList.Add(EAsset.AtlasEmoji);
//             _resList.Add(EAsset.AtlasHead);
//             _resList.Add(EAsset.AtlasScene);
//                 
//             // _resList.Add(EAsset.AtlasItem);
//             // _resList.Add(EAsset.AtlasBubble);
//             // _resList.Add(EAsset.AtlasSkill);
//             // _resList.Add(EAsset.AtlasTitle);
//             _resList.Add(EAsset.UICommonIcon);
//             _resList.Add(EAsset.UICommon);
//             _resList.Add(EAsset.UIMain);
//             
//             //prefab
//             // _prefabList.Add(EAsset.BattleAttr);
//             // _prefabList.Add(EAsset.BattleNumber);
//             // _prefabList.Add(EAsset.BattleSkill);
//             // _prefabList.Add(EAsset.HUDItem);
//             // _prefabList.Add(EAsset.GuideItem);
//             // _prefabList.Add(EAsset.NPCDialogueBubble);
//             _prefabList.Add(EAsset.CommonPrefab);
//             // _prefabList.Add(EAsset.ItemContainer);
//             // _prefabList.Add(EAsset.ItemShow);
//             // _prefabList.Add(EAsset.HeadContainer);
//             // _prefabList.Add(EAsset.CardContainer);
//             // _prefabList.Add(EAsset.MessageItem);
//             // _prefabList.Add(EAsset.EquipAttr1);
//             // _prefabList.Add(EAsset.EquipAttr2);
//             // _prefabList.Add(EAsset.CardShow);
//             // _prefabList.Add(EAsset.TransTitle);
//             // _prefabList.Add(EAsset.TransSlider);
//             // _prefabList.Add(EAsset.ItemBubble);
//             // _prefabList.Add(EAsset.DropItem);
//             _prefabList.Add(EAsset.LoadingView);
//             _prefabList.Add(EAsset.MainView);
//             _prefabList.Add(EAsset.DefaultModel);
//             
//             initTotal(_resList.Count + _prefabList.Count);
//         }
//
//         public override void start()
//         {
//             base.start();
//             loadNextRes();
//         }
//
//         private void loadNextRes()
//         {
//             if (_resList.Count > 0)
//             {
//                 string resUrl = _resList[0];
//                 // IrfResourcesMgr.setDontRelease(resUrl);
//                 // IrfResourcesMgr.LoadResource(resUrl, onResLoadCallback);
//                 YooAssetMgr.LoadAssetAsync(resUrl, onResLoadCallback);
//             }
//             else
//             {
//                 LoadNextPrefab();
//             }
//         }
//
//         private void LoadNextPrefab()
//         {
//             if (_prefabList.Count > 0)
//             {
//                 string prefabUrl = _prefabList[0];
//                 // IrfResourcesMgr.setDontRelease(prefabUrl);
//                 UrlPrefabLoaderUtil.LoadAsset(prefabUrl, onPrefabLoadCallback);
//             }
//             else
//             {
//                 Game.AddSingleton<CommonResMgr>();
//                 Game.AddSingleton<UIModelCtrlMgr>();
//                 Game.AddSingleton<CameraMgr>();
//                 finishAll();
//             }
//         }
//
//         private void onResLoadCallback(AssetOperationHandle obj)
//         {
//             string resUrl = _resList[0];
//             
//             ShaderManager.Instance.clear();
//                 
//             if (resUrl == EAsset.CommonShaders)
//             {
//                 foreach (var shader in EAsset.HyShaders)  
//                 {
//                     ShaderManager.Instance.AddShader(shader);
//                 }
//             }
//             _resList.RemoveAt(0);
//             finishOne();
//             loadNextRes();
//         }
//
//         private void onPrefabLoadCallback(PrefabLoader prefabLoader)
//         {
//             string prefabUrl = _prefabList[0];
//             UrlPrefabLoaderUtil.CancelLoadAsset(prefabUrl, onPrefabLoadCallback);
//             _prefabList.RemoveAt(0);
//             finishOne();
//             LoadNextPrefab();
//         }
//     }
// }