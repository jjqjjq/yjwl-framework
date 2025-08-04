// using System;
// using System.Collections.Generic;
// using JQCore.tSingleton;
// using JQFramework.tMVC.Base;
//
// namespace JQFramework.Video
// {
//     public class VideoMgr : JQSingleton<VideoMgr>
//     {
//         private Dictionary<RootView, VideoBean> _uiVideoBeanDic = new Dictionary<RootView, VideoBean>();
//         private Dictionary<ApplyToMesh, VideoBean> _meshVideoBeanDic = new Dictionary<ApplyToMesh, VideoBean>();
//
//         public void clear()
//         {
//             foreach (KeyValuePair<RootView,VideoBean> keyValuePair in _uiVideoBeanDic)
//             {
//                 keyValuePair.Value.Dispose();
//             }
//             _uiVideoBeanDic.Clear();
//             
//             foreach (KeyValuePair<ApplyToMesh,VideoBean> keyValuePair in _meshVideoBeanDic)
//             {
//                 keyValuePair.Value.Dispose();
//             }
//             _meshVideoBeanDic.Clear();
//         }
//         
//         public override void Dispose()
//         {
//             clear();
//         }
//
//         public void releaseVideo(RootView openView)
//         {
//             VideoBean videoBean = null;
//             if (!_uiVideoBeanDic.TryGetValue(openView, out videoBean))
//             {
//                 return;
//             }
//             videoBean.Dispose();
//             _uiVideoBeanDic.Remove(openView);
//         }
//         
//         public void initVideo(RootView openView, DisplayUGUI displayUGUI, SubtitlesUGUI subtitlesUGUI, int videoId, int subTitlesId, MediaPlayer mediaPlayer, Action startFun, Action endFun, bool notAutoPlay = false)
//         {
//             VideoBean videoBean = null;
//             if (!_uiVideoBeanDic.TryGetValue(openView, out videoBean))
//             {
//                 videoBean = new VideoBean(mediaPlayer, openView, displayUGUI, subtitlesUGUI);
//                 _uiVideoBeanDic.Add(openView, videoBean);
//             }
//             videoBean.initVideo(videoId, subTitlesId, startFun, endFun, notAutoPlay);
//         }
//
//         public void initVideo(ApplyToMesh applyToMesh, int videoId, int subTitlesId, MediaPlayer mediaPlayer, Action startFun, Action endFun, bool notAutoPlay = false)
//         {
//             VideoBean videoBean = null;
//             if (!_meshVideoBeanDic.TryGetValue(applyToMesh, out videoBean))
//             {
//                 videoBean = new VideoBean(mediaPlayer, applyToMesh);
//                 _meshVideoBeanDic.Add(applyToMesh, videoBean);
//             }
//             videoBean.initVideo(videoId, subTitlesId, startFun, endFun, notAutoPlay);
//         }
//     }
// }