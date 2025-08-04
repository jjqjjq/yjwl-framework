// using System;
// using JQCore.tLog;
// using JQCore.tUtil;
// using JQFramework.tMVC.Base;
// using JQFramework.tYooAssets;
// using YooAsset;
// using ErrorCode = RenderHeads.Media.AVProVideo.ErrorCode;
//
// namespace JQFramework.Video
// {
//     public class VideoBean
//     {
//         private Action _playStartCallBack;
//         private Action _playEndCallBack;
//
//         private int _type = 0; //1:ui 2:mesh
//
//         //ui
//         private RootView _rootView;
//
//         private DisplayUGUI _displayUGUI;
//
//         //mesh
//         private ApplyToMesh _applyToMesh;
//
//         private MediaPlayer _mediaPlayer;
//         private SubtitlesUGUI _subtitlesUGUI;
//
//         private int _videoId;
//         private int _subTitlesId;
//         private bool _autoPlay;
//
//
//         private bool _isDispose;
//
//         public void Dispose()
//         {
//             _playStartCallBack = null;
//             _playEndCallBack = null;
//             _rootView = null;
//             _displayUGUI = null;
//             _subtitlesUGUI = null;
//             _applyToMesh = null;
//             _isDispose = true;
//             _mediaPlayer = null;
//             _autoPlay = false;
//         }
//
//
//         public VideoBean(MediaPlayer mediaPlayer, RootView rootView, DisplayUGUI displayUGUI, SubtitlesUGUI subtitlesUGUI)
//         {
//             _mediaPlayer = mediaPlayer;
//             _rootView = rootView;
//             _displayUGUI = displayUGUI;
//             _subtitlesUGUI = subtitlesUGUI;
//             _type = 1;
//         }
//
//         public VideoBean(MediaPlayer mediaPlayer, ApplyToMesh applyToMesh)
//         {
//             _mediaPlayer = mediaPlayer;
//             _applyToMesh = applyToMesh;
//             _type = 2;
//         }
//
//
//         public void initVideo(int videoId, int subTitlesId, Action startFun, Action endFun, bool notAutoPlay = false)
//         {
//             _autoPlay = !notAutoPlay;
//             switch (_type)
//             {
//                 case 1:
//                     _displayUGUI.enabled = false;
//                     break;
//                 case 2:
//                     _applyToMesh.enabled = false;
//                     break;
//             }
//
//             _mediaPlayer.DisableSubtitles();
//             _mediaPlayer.CloseMedia();
//             _mediaPlayer.Events.RemoveAllListeners();
//             _mediaPlayer.Events.AddListener(OnMediaPlayerEvent);
//
//             _videoId = videoId;
//             _subTitlesId = subTitlesId;
//             _playStartCallBack = startFun;
//             _playEndCallBack = endFun;
//
//
//             if (subTitlesId != 0)
//             {
//                 switch (_type)
//                 {
//                     case 1:
//                         if (_subtitlesUGUI == null)
//                         {
//                             JQLog.LogError("subtitlesUGUI is null but subTitlesId is not zero!:" + subTitlesId);
//                             return;
//                         }
//
//                         _subtitlesUGUI.ChangeMediaPlayer(_mediaPlayer);
//                         break;
//                 }
//
//                 LoadSubtitles();
//             }
//             else
//             {
//                 LoadVideo();
//             }
//         }
//
//         private (string, string, string, string) getPath(int videoId, string suffix)
//         {
//             string abPath = $"Video/{videoId}.{suffix}";
//             string dataPath = $"{PathUtil.RES_FOLDER}/Video/{videoId}.{suffix}";
//             string streamingAssetPath = $"Video/{videoId}.{suffix}";
//             string persistentDataPath = $"{PathUtil.DOWNLOAD_FOLDER}/Video/{videoId}.{suffix}"; //todo: 持久化下载目录需要调试
//             return (abPath, dataPath, streamingAssetPath, persistentDataPath);
//         }
//
//         private void LoadSubtitles()
//         {
//             (string srt_abPath, string srt_dataPath, string srt_streamingAssetPath, string srt_persistentDataPath) = getPath(_videoId, "srt");
//             YooAssetMgr.LoadRawFileAsync(srt_abPath, LoadSubtitlesFinish);
//         }
//
//         private void LoadSubtitlesFinish(RawFileOperationHandle handle)
//         {
//             if (_mediaPlayer == null) return;
//             if (handle.IsDone == false)
//             {
//                 JQLog.LogError($"字幕加载失败:{handle.GetRawFilePath()}:{handle.LastError}");
//                 return;
//             }
//
//             string fileText = handle.GetRawFileText();
//             string filePath = handle.GetRawFilePath();
//             JQLog.Log($"字幕加完成：设置字幕:{filePath}");
//             _mediaPlayer.Subtitles.LoadSubtitlesSRT(fileText);
//             LoadVideo();
//         }
//
//         private void LoadVideo()
//         {
//             (string mp4_abPath, string mp4_dataPath, string mp4_streamingAssetPath, string mp4_persistentDataPath) = getPath(_videoId, "mp4");
// #if UNITY_EDITOR
//             // _mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToDataFolder, mp4_dataPath);
// #endif
//
//             YooAssetMgr.LoadRawFileAsync(mp4_abPath, LoadVideoFinish);
//         }
//
//         private void LoadVideoFinish(RawFileOperationHandle handle)
//         {
//             if (_mediaPlayer == null) return;
//             if (handle == null)
//             {
//                 JQLog.LogError($"handle is null");
//                 return;
//             }
//             if (handle.IsDone == false)
//             {
//                 JQLog.LogError($"视频加载失败:{handle.GetRawFilePath()}:{handle.LastError}");
//                 return;
//             }
//             
//             (string mp4_abPath, string mp4_dataPath, string mp4_streamingAssetPath, string mp4_persistentDataPath) = getPath(_videoId, "mp4");
//             byte[] fileData = handle.GetRawFileData();
//             string filePath = handle.GetRawFilePath();
//             JQLog.Log($"视频加载完成:{mp4_abPath} {filePath} {handle.IsValid}");
//             _mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, filePath, _autoPlay);
//         }
//
//         private void OnMediaPlayerEvent(MediaPlayer mediaPlayer, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
//         {
//             switch (eventType)
//             {
//                 case MediaPlayerEvent.EventType.FirstFrameReady:
//                     switch (_type)
//                     {
//                         case 1:
//                             _displayUGUI.enabled = true;
//                             break;
//                         case 2:
//                             _applyToMesh.enabled = true;
//                             break;
//                     }
//
//                     if (_playStartCallBack != null)
//                     {
//                         _playStartCallBack();
//                         _playStartCallBack = null;
//                     }
//
//                     break;
//                 case MediaPlayerEvent.EventType.FinishedPlaying:
//                     if (_playEndCallBack != null)
//                     {
//                         _playEndCallBack();
//                         _playEndCallBack = null;
//                     }
//
//                     break;
//             }
//         }
//     }
// }