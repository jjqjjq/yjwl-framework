//unity before version 5 is old

//#define USE_OLD_UNITY

//using System;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JQCore.tLog;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class Images
{
    public Texture2D clearImage;
    public Texture2D collapseImage;
    public Texture2D clearOnNewSceneImage;
    public Texture2D showTimeImage;
    public Texture2D showSceneImage;
    public Texture2D userImage;
    public Texture2D showMemoryImage;
    public Texture2D softwareImage;
    public Texture2D dateImage;
    public Texture2D showFpsImage;
    public Texture2D showGraphImage;
    public Texture2D graphImage;
    public Texture2D infoImage;
    public Texture2D searchImage;
    public Texture2D closeImage;

    public Texture2D buildFromImage;
    public Texture2D systemInfoImage;
    public Texture2D graphicsInfoImage;
    public Texture2D backImage;
    public Texture2D cameraImage;

    public Texture2D logImage;
    public Texture2D warningImage;
    public Texture2D errorImage;

    public Texture2D barImage;
    public Texture2D button_activeImage;
    public Texture2D even_logImage;
    public Texture2D odd_logImage;
    public Texture2D selectedImage;

    public GUISkin reporterScrollerSkin;
}

//just drop this script to empty game object on first scene you game start at, this all what you have to do
//no coding is required 
//then you can view logs , warnings , errors and exceptions inside your game
//just draw circle on your game to view all logs

public class Reporter : MonoBehaviour
{
    public enum _LogType
    {
        Assert = LogType.Assert,
        Error = LogType.Error,
        Exception = LogType.Exception,
        Log = LogType.Log,
        Warning = LogType.Warning
    }

    //used to check if you have In Game Logs multiple time in different scene
    //only one should work and other should be deleted
    public static bool created;

    [HideInInspector] //show hide In Game Logs
    public bool show;

    public string UserData = "";

    //fram rate per seconds
    public float fps;

    public string fpsText;
    //public delegate void OnLogHandler( string condition, string stacktrace, LogType type );
    //public event OnLogHandler OnLog ;

    public Images images;

    public Vector2 size = new(46, 46);
    public float maxSize = 20;
    public int numOfCircleToShow = 1;

    public bool Initialized;

    private bool _canShowGUI;
    private int _frameCount;
    private GUIContent backContent;
    private GUIStyle backStyle;
    private GUIStyle barStyle;

    private string buildDate = "webgl none";


    private GUIContent buildFromContent;
    private Rect buttomRect = Rect.zero;

    private GUIStyle buttonActiveStyle;

    //to save memory
    private readonly Dictionary<string, string> cachedString = new();

    private GUIContent cameraContent;

    // gui
    private GUIContent clearContent;

    private GUIContent clearOnNewSceneContent;

    //to deside if you want to clean logs for new loaded scene
    private bool clearOnNewSceneLoaded;
    private GUIContent closeContent;

    //collapse logs
    private bool collapse;

    private GUIContent collapseContent;

    //contains all collapsed logs
    private readonly List<Log> collapsedLogs = new();

    private Rect countRect = Rect.zero;

    private int currentFrame;

    //contain logs which should only appear to user , for example if you switch off show logs + switch off show warnings
    //and your mode is collapse,then this list will contains only collapsed errors
    private readonly List<Log> currentLog = new();

    //string[] scenes ;
    private string currentScene;

    private ReportView currentView = ReportView.Logs;
    private GUIContent dateContent;
    private Rect detailRect = Rect.zero;

    private string deviceModel;
    private string deviceName;
    private string deviceType;

    //calculate  pos of first click on screen
    //Vector2 startPos ;

    private Vector2 downPos;
    private GUIContent errorContent;
    private GUIStyle evenLogStyle;
    private string filterText = "";
    private Rect fpsLabelRect;
    private Rect fpsRect;

    private float gcTotalMemory;
    private int gestureCount;

    private readonly List<Vector2> gestureDetector = new();
    private float gestureLength;
    private Vector2 gestureSum = Vector2.zero;
    private GUIContent graphContent;
    private GUIContent graphicsInfoContent;
    private string graphicsMemorySize;
    private Rect graphMaxRect;
    private float graphMemUsage;
    private Rect graphMinRect;

    private Rect graphRect;

    //Vector3 tempVector1 ;
    //Vector3 tempVector2 ;
    private Vector2 graphScrollerPos;
    private GUISkin graphScrollerSkin;

    private readonly float graphSize = 4f;
    private GUIContent infoContent;

    private Vector2 infoScrollPosition;

    private float lastClickTime = -1;

    private float lastUpdate;
    private float lastUpdate2 = 0;
    private GUIStyle logButtonStyle;


    private GUIContent logContent;
    private string logDate;

    //contains all uncollapsed log
    private readonly List<Log> logs = new();
    private GUISkin logScrollerSkin;

    //used to check if the new coming logs is already exist or new one
    private readonly MultiKeyDictionary<string, string, Log> logsDic = new();
    private float logsMemUsage;
    private Rect logsRect;
    private GUIStyle lowerLeftFontStyle;
    private float maxFpsValue;
    private float maxMemoryValue;
#if !USE_OLD_UNITY
    private string maxTextureSize;
#endif
    private Rect memoryLabelRect;
    private Rect memoryRect;
    private float minFpsValue;
    private float minMemoryValue;

    //calculate drag amount , this is used for scrolling

    private Vector2 mousePosition;

    private GUIStyle nonStyle;

    //total number of collapsed logs
    private int numOfCollapsedLogs;

    //total number of collapsed errors
    private int numOfCollapsedLogsError;

    //total number of collapsed warnings
    private int numOfCollapsedLogsWarning;

    //total number of logs
    private int numOfLogs;

    //total number of errors
    private int numOfLogsError;

    //total number of warnings
    private int numOfLogsWarning;
    private GUIStyle oddLogStyle;
    private float oldDrag;
    private float oldDrag2;
    private float oldDrag3;
    private Vector2 oldInfoDrag;

    private readonly List<Sample> samples = new(60 * 60 * 60);
    private Rect sceneLabelRect;
    private Rect sceneRect;

    private Rect screenRect = Rect.zero;
    private GUIStyle scrollerStyle;

    private Vector2 scrollPosition;
    private Vector2 scrollPosition2;
    private GUIContent searchContent;
    private GUIStyle searchStyle;

    //int 	selectedIndex = -1;
    private Log selectedLog;
    private GUIStyle selectedLogFontStyle;
    private GUIStyle selectedLogStyle;

    //maximum number of allowed logs to view
    //public int maxAllowedLog = 1000 ;

    private bool showClearOnNewSceneLoadedButton = true;

    //show or hide errors
    private bool showError = true;

    private bool showFps;
    private bool showFpsButton = true;
    private GUIContent showFpsContent;

    private bool showGraph;

    //show or hide logs
    private bool showLog = true;
    private bool showMemButton = true;

    private bool showMemory;
    private GUIContent showMemoryContent;

    private bool showScene;
    private bool showSceneButton = true;
    private GUIContent showSceneContent;
    private bool showSearchText = true;

    private bool showTime;
    private bool showTimeButton = true;

    private GUIContent showTimeContent;

    //show or hide warnings
    private bool showWarning = true;
    private GUIStyle sliderBackStyle;
    private GUIStyle sliderThumbStyle;
    private GUIContent softwareContent;
    private GUIStyle stackLabelStyle;
    private Rect stackRect;
    private Vector2 stackRectTopLeft;
    private int startFrame;
    private int startIndex;
    private GUIContent systemInfoContent;
    private string systemMemorySize;
    private readonly GUIContent tempContent = new();

    private Rect tempRect;

    private readonly List<Log> threadedLogs = new();
    private Rect timeLabelRect;
    private Rect timeRect;

    private float toolbarOldDrag;
    private Rect toolBarRect = Rect.zero;
    private GUISkin toolbarScrollerSkin;
    private Vector2 toolbarScrollPosition;
    private GUIContent userContent;
    private GUIContent warningContent;

    public float TotalMemUsage => logsMemUsage + graphMemUsage;

    private void Awake()
    {
        if (!Initialized)
            Initialize();
    }

    private void Start()
    {
        logDate = DateTime.Now.ToString();
#if !SDK_WEIXIN
        StartCoroutine("readInfo");
#endif
    }

    private void Update()
    {
        _frameCount = Time.frameCount;
        if (show) fpsText = fps.ToString("0.000");
        gcTotalMemory = (float)GC.GetTotalMemory(false) / 1024 / 1024;
        //addSample();
        //if( string.IsNullOrEmpty( scenes[ Application.loadedLevel ] ))
        //	scenes[ Application.loadedLevel ] = Application.loadedLevelName ;

        var elapsed = Time.realtimeSinceStartup - lastUpdate;
        fps = 1f / elapsed;
        lastUpdate = Time.realtimeSinceStartup;
        calculateStartIndex();
        if (!show && isGestureDone() && _canShowGUI)
        {
            Debug.Log("_canShowGUI:" + _canShowGUI);
            doShow();
        }


        if (threadedLogs.Count > 0)
            lock (threadedLogs)
            {
                for (var i = 0; i < threadedLogs.Count; i++)
                {
                    var l = threadedLogs[i];
                    AddLog(l.condition, l.stacktrace, (LogType)l.logType);
                }

                ClearLogList(threadedLogs);
            }
    }


    private void OnEnable()
    {
        if (logs.Count == 0) //if recompile while in play mode
            clear();
    }

    private void OnDisable()
    {
    }

    //save user config
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Reporter_currentView", (int)currentView);
        PlayerPrefs.SetInt("Reporter_show", show ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_collapse", collapse ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_clearOnNewSceneLoaded", clearOnNewSceneLoaded ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showTime", showTime ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showScene", showScene ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showMemory", showMemory ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showFps", showFps ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showGraph", showGraph ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showLog", showLog ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showWarning", showWarning ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showError", showError ? 1 : 0);
        PlayerPrefs.SetString("Reporter_filterText", filterText);
        PlayerPrefs.SetFloat("Reporter_size", size.x);

        PlayerPrefs.SetInt("Reporter_showClearOnNewSceneLoadedButton", showClearOnNewSceneLoadedButton ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showTimeButton", showTimeButton ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showSceneButton", showSceneButton ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showMemButton", showMemButton ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showFpsButton", showFpsButton ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showSearchText", showSearchText ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void setCanShowGUI(bool canShowGUI)
    {
        _canShowGUI = canShowGUI;
    }

    private void addSample()
    {
        var sample = new Sample();
        sample.fps = fps;
        sample.fpsText = fpsText;
        sample.loadedScene = (byte)SceneManager.GetActiveScene().buildIndex; //Application.loadedLevel
        sample.time = Time.realtimeSinceStartup;
        sample.memory = gcTotalMemory;
        samples.Add(sample);

        graphMemUsage = samples.Count * Sample.MemSize() / 1024 / 1024;
    }

    //new scene is loaded
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        /*
        if( clearOnNewSceneLoaded )
            clear();
        */
        currentScene = SceneManager.GetActiveScene().name;
        //Debug.Log( "Scene " + Application.loadedLevelName + " is loaded");
    }

    public void Initialize()
    {
        if (!created)
        {
            try
            {
                gameObject.SendMessage("OnPreStart");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            //scenes = new string[ Application.levelCount ];
            currentScene = SceneManager.GetActiveScene().name; //Application.loadedLevelName;
            SceneManager.sceneLoaded += OnSceneLoad;
            DontDestroyOnLoad(gameObject);
            //Application.logMessageReceived += CaptureLog;
            Application.logMessageReceivedThreaded += CaptureLogThread;
            created = true;
            //addSample();
        }
        else
        {
            Debug.LogWarning("tow manager is exists delete the second");
            DestroyImmediate(gameObject, true);
            return;
        }


        //initialize gui and styles for gui porpose

        clearContent = new GUIContent("", images.clearImage, "Clear logs");
        collapseContent = new GUIContent("", images.collapseImage, "Collapse logs");
        clearOnNewSceneContent = new GUIContent("", images.clearOnNewSceneImage, "Clear logs on new scene loaded");
        showTimeContent = new GUIContent("", images.showTimeImage, "Show Hide Time");
        showSceneContent = new GUIContent("", images.showSceneImage, "Show Hide Scene");
        showMemoryContent = new GUIContent("", images.showMemoryImage, "Show Hide Memory");
        softwareContent = new GUIContent("", images.softwareImage, "Software");
        dateContent = new GUIContent("", images.dateImage, "Date");
        showFpsContent = new GUIContent("", images.showFpsImage, "Show Hide fps");
        graphContent = new GUIContent("", images.showGraphImage, "Show Graph");
        infoContent = new GUIContent("", images.infoImage, "Information about application");
        searchContent = new GUIContent("", images.searchImage, "Search for logs");
        closeContent = new GUIContent("", images.closeImage, "Hide logs");
        userContent = new GUIContent("", images.userImage, "User");

        buildFromContent = new GUIContent("", images.buildFromImage, "Build From");
        systemInfoContent = new GUIContent("", images.systemInfoImage, "System Info");
        graphicsInfoContent = new GUIContent("", images.graphicsInfoImage, "Graphics Info");
        backContent = new GUIContent("", images.backImage, "Back");

        cameraContent = new GUIContent("", images.cameraImage, "Select Photo");


        //snapshotContent = new GUIContent("",images.cameraImage,"show or hide logs");
        logContent = new GUIContent("", images.logImage, "show or hide logs");
        warningContent = new GUIContent("", images.warningImage, "show or hide warnings");
        errorContent = new GUIContent("", images.errorImage, "show or hide errors");


        currentView = (ReportView)PlayerPrefs.GetInt("Reporter_currentView", 1);
        show = false;
        collapse = PlayerPrefs.GetInt("Reporter_collapse") == 1 ? true : false;
        clearOnNewSceneLoaded = PlayerPrefs.GetInt("Reporter_clearOnNewSceneLoaded") == 1 ? true : false;
        showTime = PlayerPrefs.GetInt("Reporter_showTime") == 1 ? true : false;
        showScene = PlayerPrefs.GetInt("Reporter_showScene") == 1 ? true : false;
        showMemory = PlayerPrefs.GetInt("Reporter_showMemory") == 1 ? true : false;
        showFps = PlayerPrefs.GetInt("Reporter_showFps") == 1 ? true : false;
        showGraph = PlayerPrefs.GetInt("Reporter_showGraph") == 1 ? true : false;
        showLog = PlayerPrefs.GetInt("Reporter_showLog", 1) == 1 ? true : false;
        showWarning = PlayerPrefs.GetInt("Reporter_showWarning", 1) == 1 ? true : false;
        showError = PlayerPrefs.GetInt("Reporter_showError", 1) == 1 ? true : false;
        filterText = PlayerPrefs.GetString("Reporter_filterText");
        size.x = size.y = PlayerPrefs.GetFloat("Reporter_size", 48);


        showClearOnNewSceneLoadedButton = PlayerPrefs.GetInt("Reporter_showClearOnNewSceneLoadedButton", 1) == 1 ? true : false;
        showTimeButton = PlayerPrefs.GetInt("Reporter_showTimeButton", 1) == 1 ? true : false;
        showSceneButton = PlayerPrefs.GetInt("Reporter_showSceneButton", 1) == 1 ? true : false;
        showMemButton = PlayerPrefs.GetInt("Reporter_showMemButton", 1) == 1 ? true : false;
        showFpsButton = PlayerPrefs.GetInt("Reporter_showFpsButton", 1) == 1 ? true : false;
        showSearchText = PlayerPrefs.GetInt("Reporter_showSearchText", 1) == 1 ? true : false;


        initializeStyle();

        Initialized = true;

        if (show) doShow();

        deviceModel = SystemInfo.deviceModel;
        deviceType = SystemInfo.deviceType.ToString();
        deviceName = SystemInfo.deviceName;
        graphicsMemorySize = SystemInfo.graphicsMemorySize.ToString();
#if !USE_OLD_UNITY
        maxTextureSize = SystemInfo.maxTextureSize.ToString();
#endif
        systemMemorySize = SystemInfo.systemMemorySize.ToString();
    }

    private void initializeStyle()
    {
        var paddingX = (int)(size.x * 0.2f);
        var paddingY = (int)(size.y * 0.2f);
        nonStyle = new GUIStyle();
        nonStyle.clipping = TextClipping.Clip;
        nonStyle.border = new RectOffset(0, 0, 0, 0);
        nonStyle.normal.background = null;
        nonStyle.fontSize = (int)(size.y / 2);
        nonStyle.alignment = TextAnchor.MiddleCenter;

        lowerLeftFontStyle = new GUIStyle();
        lowerLeftFontStyle.clipping = TextClipping.Clip;
        lowerLeftFontStyle.border = new RectOffset(0, 0, 0, 0);
        lowerLeftFontStyle.normal.background = null;
        lowerLeftFontStyle.fontSize = (int)(size.y / 2);
        lowerLeftFontStyle.fontStyle = FontStyle.Bold;
        lowerLeftFontStyle.alignment = TextAnchor.LowerLeft;


        barStyle = new GUIStyle();
        barStyle.border = new RectOffset(1, 1, 1, 1);
        barStyle.normal.background = images.barImage;
        barStyle.active.background = images.button_activeImage;
        barStyle.alignment = TextAnchor.MiddleCenter;
        barStyle.margin = new RectOffset(1, 1, 1, 1);

        //barStyle.padding = new RectOffset(paddingX,paddingX,paddingY,paddingY); 
        //barStyle.wordWrap = true ;
        barStyle.clipping = TextClipping.Clip;
        barStyle.fontSize = (int)(size.y / 2);


        buttonActiveStyle = new GUIStyle();
        buttonActiveStyle.border = new RectOffset(1, 1, 1, 1);
        buttonActiveStyle.normal.background = images.button_activeImage;
        buttonActiveStyle.alignment = TextAnchor.MiddleCenter;
        buttonActiveStyle.margin = new RectOffset(1, 1, 1, 1);
        //buttonActiveStyle.padding = new RectOffset(4,4,4,4);
        buttonActiveStyle.fontSize = (int)(size.y / 2);

        backStyle = new GUIStyle();
        backStyle.normal.background = images.even_logImage;
        backStyle.clipping = TextClipping.Clip;
        backStyle.fontSize = (int)(size.y / 2);

        evenLogStyle = new GUIStyle();
        evenLogStyle.normal.background = images.even_logImage;
        evenLogStyle.fixedHeight = size.y;
        evenLogStyle.clipping = TextClipping.Clip;
        evenLogStyle.alignment = TextAnchor.UpperLeft;
        evenLogStyle.imagePosition = ImagePosition.ImageLeft;
        evenLogStyle.fontSize = (int)(size.y / 2);
        //evenLogStyle.wordWrap = true;

        oddLogStyle = new GUIStyle();
        oddLogStyle.normal.background = images.odd_logImage;
        oddLogStyle.fixedHeight = size.y;
        oddLogStyle.clipping = TextClipping.Clip;
        oddLogStyle.alignment = TextAnchor.UpperLeft;
        oddLogStyle.imagePosition = ImagePosition.ImageLeft;
        oddLogStyle.fontSize = (int)(size.y / 2);
        //oddLogStyle.wordWrap = true ;

        logButtonStyle = new GUIStyle();
        //logButtonStyle.wordWrap = true;
        logButtonStyle.fixedHeight = size.y;
        logButtonStyle.clipping = TextClipping.Clip;
        logButtonStyle.alignment = TextAnchor.UpperLeft;
        //logButtonStyle.imagePosition = ImagePosition.ImageLeft ;
        //logButtonStyle.wordWrap = true;
        logButtonStyle.fontSize = (int)(size.y / 2);
        logButtonStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        selectedLogStyle = new GUIStyle();
        selectedLogStyle.normal.background = images.selectedImage;
        selectedLogStyle.fixedHeight = size.y;
        selectedLogStyle.clipping = TextClipping.Clip;
        selectedLogStyle.alignment = TextAnchor.UpperLeft;
        selectedLogStyle.normal.textColor = Color.white;
        //selectedLogStyle.wordWrap = true;
        selectedLogStyle.fontSize = (int)(size.y / 2);

        selectedLogFontStyle = new GUIStyle();
        selectedLogFontStyle.normal.background = images.selectedImage;
        selectedLogFontStyle.fixedHeight = size.y;
        selectedLogFontStyle.clipping = TextClipping.Clip;
        selectedLogFontStyle.alignment = TextAnchor.UpperLeft;
        selectedLogFontStyle.normal.textColor = Color.white;
        //selectedLogStyle.wordWrap = true;
        selectedLogFontStyle.fontSize = (int)(size.y / 2);
        selectedLogFontStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        stackLabelStyle = new GUIStyle();
        stackLabelStyle.wordWrap = true;
        stackLabelStyle.fontSize = (int)(size.y / 2);
        stackLabelStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        scrollerStyle = new GUIStyle();
        scrollerStyle.normal.background = images.barImage;

        searchStyle = new GUIStyle();
        searchStyle.clipping = TextClipping.Clip;
        searchStyle.alignment = TextAnchor.LowerCenter;
        searchStyle.fontSize = (int)(size.y / 2);
        searchStyle.wordWrap = true;


        sliderBackStyle = new GUIStyle();
        sliderBackStyle.normal.background = images.barImage;
        sliderBackStyle.fixedHeight = size.y;
        sliderBackStyle.border = new RectOffset(1, 1, 1, 1);

        sliderThumbStyle = new GUIStyle();
        sliderThumbStyle.normal.background = images.selectedImage;
        sliderThumbStyle.fixedWidth = size.x;

        var skin = images.reporterScrollerSkin;

        toolbarScrollerSkin = Instantiate(skin);
        toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        toolbarScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
        toolbarScrollerSkin.verticalScrollbarThumb.fixedWidth = 0f;
        toolbarScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

        logScrollerSkin = Instantiate(skin);
        logScrollerSkin.verticalScrollbar.fixedWidth = size.x * 2f;
        logScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
        logScrollerSkin.verticalScrollbarThumb.fixedWidth = size.x * 2f;
        logScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

        graphScrollerSkin = Instantiate(skin);
        graphScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        graphScrollerSkin.horizontalScrollbar.fixedHeight = size.x * 2f;
        graphScrollerSkin.verticalScrollbarThumb.fixedWidth = 0f;
        graphScrollerSkin.horizontalScrollbarThumb.fixedHeight = size.x * 2f;
        //inGameLogsScrollerSkin.verticalScrollbarThumb.fixedWidth = size.x * 2;
        //inGameLogsScrollerSkin.verticalScrollbar.fixedWidth = size.x * 2;
    }

    private void ClearLogList(List<Log> list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var log = list[i];
            log.dispose();
        }

        list.Clear();
    }

    //clear all logs
    public void clear()
    {
        ClearLogList(logs);
        ClearLogList(collapsedLogs);
        ClearLogList(currentLog);
        logsDic.MultiKeyClear();
        cachedString.Clear();
        //selectedIndex = -1;
        selectedLog = null;
        numOfLogs = 0;
        numOfLogsWarning = 0;
        numOfLogsError = 0;
        numOfCollapsedLogs = 0;
        numOfCollapsedLogsWarning = 0;
        numOfCollapsedLogsError = 0;
        logsMemUsage = 0;
        graphMemUsage = 0;
        for (var i = 0; i < samples.Count; i++)
        {
            var sample = samples[i];
            sample.dispose();
        }

        samples.Clear();
//        System.GC.Collect();
        selectedLog = null;
    }

    //calculate what is the currentLog : collapsed or not , hide or view warnings ......
    private void calculateCurrentLog()
    {
        var filter = !string.IsNullOrEmpty(filterText);
        var _filterText = "";
        if (filter)
            _filterText = filterText.ToLower();
        currentLog.Clear();
        if (collapse)
            for (var i = 0; i < collapsedLogs.Count; i++)
            {
                var log = collapsedLogs[i];
                if (log.logType == _LogType.Log && !showLog)
                    continue;
                if (log.logType == _LogType.Warning && !showWarning)
                    continue;
                if (log.logType == _LogType.Error && !showError)
                    continue;
                if (log.logType == _LogType.Assert && !showError)
                    continue;
                if (log.logType == _LogType.Exception && !showError)
                    continue;

                if (filter)
                {
                    if (log.condition.ToLower().Contains(_filterText))
                        currentLog.Add(log);
                }
                else
                {
                    currentLog.Add(log);
                }
            }
        else
            for (var i = 0; i < logs.Count; i++)
            {
                var log = logs[i];
                if (log.logType == _LogType.Log && !showLog)
                    continue;
                if (log.logType == _LogType.Warning && !showWarning)
                    continue;
                if (log.logType == _LogType.Error && !showError)
                    continue;
                if (log.logType == _LogType.Assert && !showError)
                    continue;
                if (log.logType == _LogType.Exception && !showError)
                    continue;

                if (filter)
                {
                    if (log.condition.ToLower().Contains(_filterText))
                        currentLog.Add(log);
                }
                else
                {
                    currentLog.Add(log);
                }
            }

        if (selectedLog != null)
        {
            var newSelectedIndex = currentLog.IndexOf(selectedLog);
            if (newSelectedIndex == -1)
            {
                var collapsedSelected = logsDic[selectedLog.condition][selectedLog.stacktrace];
                newSelectedIndex = currentLog.IndexOf(collapsedSelected);
                if (newSelectedIndex != -1)
                    scrollPosition.y = newSelectedIndex * size.y;
            }
            else
            {
                scrollPosition.y = newSelectedIndex * size.y;
            }
        }
    }

    private void DrawInfo()
    {
        GUILayout.BeginArea(screenRect, backStyle);

        var drag = getDrag();
        if (drag.x != 0 && downPos != Vector2.zero) infoScrollPosition.x -= drag.x - oldInfoDrag.x;
        if (drag.y != 0 && downPos != Vector2.zero) infoScrollPosition.y += drag.y - oldInfoDrag.y;
        oldInfoDrag = drag;

        GUI.skin = toolbarScrollerSkin;
        infoScrollPosition = GUILayout.BeginScrollView(infoScrollPosition);
        GUILayout.Space(size.x);
        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Box(buildFromContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(buildDate, nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Box(systemInfoContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(deviceModel, nonStyle, GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(deviceType, nonStyle, GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(deviceName, nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Box(graphicsInfoContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(SystemInfo.graphicsDeviceName, nonStyle, GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(graphicsMemorySize, nonStyle, GUILayout.Height(size.y));
#if !USE_OLD_UNITY
        GUILayout.Space(size.x);
        GUILayout.Label(maxTextureSize, nonStyle, GUILayout.Height(size.y));
#endif
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Space(size.x);
        GUILayout.Space(size.x);
        GUILayout.Label("Screen Width " + Screen.width, nonStyle, GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label("Screen Height " + Screen.height, nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(systemMemorySize + " mb", nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Space(size.x);
        GUILayout.Space(size.x);
        GUILayout.Label("Mem Usage Of Logs " + logsMemUsage.ToString("0.000") + " mb", nonStyle, GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        //GUILayout.Label( "Mem Usage Of Graph " + graphMemUsage.ToString("0.000")  + " mb", nonStyle , GUILayout.Height(size.y));
        //GUILayout.Space( size.x);
        GUILayout.Label("GC Memory " + gcTotalMemory.ToString("0.000") + " mb", nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Box(softwareContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(SystemInfo.operatingSystem, nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Box(dateContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(DateTime.Now.ToString(), nonStyle, GUILayout.Height(size.y));
        GUILayout.Label(" - Application Started At " + logDate, nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Box(showTimeContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(Time.realtimeSinceStartup.ToString("000"), nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(fpsText, nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Box(userContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(UserData, nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label(currentScene, nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.Label("Unity Version = " + Application.unityVersion, nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        /*GUILayout.BeginHorizontal();
        GUILayout.Space( size.x);
        GUILayout.Box( graphContent ,nonStyle ,  GUILayout.Width(size.x) , GUILayout.Height(size.y));
        GUILayout.Space( size.x);
        GUILayout.Label( "frame " + samples.Count , nonStyle , GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();*/

        drawInfo_enableDisableToolBarButtons();

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Label("Size = " + size.x.ToString("0.0"), nonStyle, GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        var _size = GUILayout.HorizontalSlider(size.x, 16, 64, sliderBackStyle, sliderThumbStyle, GUILayout.Width(Screen.width * 0.5f));
        if (size.x != _size)
        {
            size.x = size.y = _size;
            initializeStyle();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        if (GUILayout.Button(backContent, barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) currentView = ReportView.Logs;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }


    private void drawInfo_enableDisableToolBarButtons()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Label("Hide or Show tool bar buttons", nonStyle, GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);

        if (GUILayout.Button(clearOnNewSceneContent, showClearOnNewSceneLoadedButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            showClearOnNewSceneLoadedButton = !showClearOnNewSceneLoadedButton;

        if (GUILayout.Button(showTimeContent, showTimeButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) showTimeButton = !showTimeButton;
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(tempRect, Time.realtimeSinceStartup.ToString("0.0"), lowerLeftFontStyle);
        if (GUILayout.Button(showSceneContent, showSceneButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) showSceneButton = !showSceneButton;
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(tempRect, currentScene, lowerLeftFontStyle);
        if (GUILayout.Button(showMemoryContent, showMemButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) showMemButton = !showMemButton;
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(tempRect, gcTotalMemory.ToString("0.0"), lowerLeftFontStyle);

        if (GUILayout.Button(showFpsContent, showFpsButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) showFpsButton = !showFpsButton;
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(tempRect, fpsText, lowerLeftFontStyle);
        /*if( GUILayout.Button( graphContent , (showGraph)?buttonActiveStyle:barStyle , GUILayout.Width(size.x*2) ,GUILayout.Height(size.y*2)))
        {
            showGraph = !showGraph ;
        }
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label( tempRect , samples.Count.ToString() , lowerLeftFontStyle );*/
        if (GUILayout.Button(searchContent, showSearchText ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) showSearchText = !showSearchText;
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.TextField(tempRect, filterText, searchStyle);


        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void DrawReport()
    {
        screenRect.x = 0f;
        screenRect.y = 0f;
        screenRect.width = Screen.width;
        screenRect.height = Screen.height;
        GUILayout.BeginArea(screenRect, backStyle);
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box(cameraContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.Label("Select Photo", nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Comming Soon", nonStyle, GUILayout.Height(size.y));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(backContent, barStyle, GUILayout.Width(size.x), GUILayout.Height(size.y))) currentView = ReportView.Logs;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void drawToolBar()
    {
        toolBarRect.x = 0f;
        toolBarRect.y = 0f;
        toolBarRect.width = Screen.width;
        toolBarRect.height = size.y * 2f;

        //toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        //toolbarScrollerSkin.horizontalScrollbar.fixedHeight= 0f  ;

        GUI.skin = toolbarScrollerSkin;
        var drag = getDrag();
        if (drag.x != 0 && downPos != Vector2.zero && downPos.y > Screen.height - size.y * 2f) toolbarScrollPosition.x -= drag.x - toolbarOldDrag;
        toolbarOldDrag = drag.x;
        GUILayout.BeginArea(toolBarRect);
        toolbarScrollPosition = GUILayout.BeginScrollView(toolbarScrollPosition);
        GUILayout.BeginHorizontal(barStyle);

        if (GUILayout.Button(clearContent, barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) clear();
        if (GUILayout.Button(collapseContent, collapse ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            collapse = !collapse;
            calculateCurrentLog();
        }

        if (showClearOnNewSceneLoadedButton && GUILayout.Button(clearOnNewSceneContent, clearOnNewSceneLoaded ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            clearOnNewSceneLoaded = !clearOnNewSceneLoaded;

        if (showTimeButton && GUILayout.Button(showTimeContent, showTime ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) showTime = !showTime;
        if (showSceneButton)
        {
            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, Time.realtimeSinceStartup.ToString("0.0"), lowerLeftFontStyle);
            if (GUILayout.Button(showSceneContent, showScene ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) showScene = !showScene;
            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, currentScene, lowerLeftFontStyle);
        }

        if (showMemButton)
        {
            if (GUILayout.Button(showMemoryContent, showMemory ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) showMemory = !showMemory;
            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, gcTotalMemory.ToString("0.0"), lowerLeftFontStyle);
        }

        if (showFpsButton)
        {
            if (GUILayout.Button(showFpsContent, showFps ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) showFps = !showFps;
            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, fpsText, lowerLeftFontStyle);
        }
        /*if( GUILayout.Button( graphContent , (showGraph)?buttonActiveStyle:barStyle , GUILayout.Width(size.x*2) ,GUILayout.Height(size.y*2)))
        {
            showGraph = !showGraph ;
        }
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label( tempRect , samples.Count.ToString() , lowerLeftFontStyle );*/

        if (showSearchText)
        {
            GUILayout.Box(searchContent, barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2));
            tempRect = GUILayoutUtility.GetLastRect();
            var newFilterText = GUI.TextField(tempRect, filterText, searchStyle);
            if (newFilterText != filterText)
            {
                filterText = newFilterText;
                calculateCurrentLog();
            }
        }

        if (GUILayout.Button(infoContent, barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2))) currentView = ReportView.Info;


        GUILayout.FlexibleSpace();


        var logsText = " ";
        if (collapse)
            logsText += numOfCollapsedLogs;
        else
            logsText += numOfLogs;
        var logsWarningText = " ";
        if (collapse)
            logsWarningText += numOfCollapsedLogsWarning;
        else
            logsWarningText += numOfLogsWarning;
        var logsErrorText = " ";
        if (collapse)
            logsErrorText += numOfCollapsedLogsError;
        else
            logsErrorText += numOfLogsError;

        GUILayout.BeginHorizontal(showLog ? buttonActiveStyle : barStyle);
        if (GUILayout.Button(logContent, nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            showLog = !showLog;
            calculateCurrentLog();
        }

        if (GUILayout.Button(logsText, nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            showLog = !showLog;
            calculateCurrentLog();
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(showWarning ? buttonActiveStyle : barStyle);
        if (GUILayout.Button(warningContent, nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            showWarning = !showWarning;
            calculateCurrentLog();
        }

        if (GUILayout.Button(logsWarningText, nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            showWarning = !showWarning;
            calculateCurrentLog();
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(showError ? buttonActiveStyle : nonStyle);
        if (GUILayout.Button(errorContent, nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            showError = !showError;
            calculateCurrentLog();
        }

        if (GUILayout.Button(logsErrorText, nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            showError = !showError;
            calculateCurrentLog();
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button(closeContent, barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            show = false;
            var gui = gameObject.GetComponent<ReporterGUI>();
            DestroyImmediate(gui);

            try
            {
                gameObject.SendMessage("OnHideReporter");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }


        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void DrawLogs()
    {
        GUILayout.BeginArea(logsRect, backStyle);

        GUI.skin = logScrollerSkin;
        //setStartPos();
        var drag = getDrag();

        if (drag.y != 0 && logsRect.Contains(new Vector2(downPos.x, Screen.height - downPos.y))) scrollPosition.y += drag.y - oldDrag;
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        oldDrag = drag.y;


        var totalVisibleCount = (int)(Screen.height * 0.75f / size.y);
        var totalCount = currentLog.Count;
        /*if( totalCount < 100 )
            inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 0;
        else
            inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 64;*/

        totalVisibleCount = Mathf.Min(totalVisibleCount, totalCount - startIndex);
        var index = 0;
        var beforeHeight = (int)(startIndex * size.y);
        //selectedIndex = Mathf.Clamp( selectedIndex , -1 , totalCount -1);
        if (beforeHeight > 0)
        {
            //fill invisible gap befor scroller to make proper scroller pos
            GUILayout.BeginHorizontal(GUILayout.Height(beforeHeight));
            GUILayout.Label("---");
            GUILayout.EndHorizontal();
        }

        var endIndex = startIndex + totalVisibleCount;
        endIndex = Mathf.Clamp(endIndex, 0, totalCount);
        var scrollerVisible = totalVisibleCount < totalCount;
        for (var i = startIndex; startIndex + index < endIndex; i++)
        {
            if (i >= currentLog.Count)
                break;
            var log = currentLog[i];

            if (log.logType == _LogType.Log && !showLog)
                continue;
            if (log.logType == _LogType.Warning && !showWarning)
                continue;
            if (log.logType == _LogType.Error && !showError)
                continue;
            if (log.logType == _LogType.Assert && !showError)
                continue;
            if (log.logType == _LogType.Exception && !showError)
                continue;

            if (index >= totalVisibleCount) break;

            GUIContent content = null;
            if (log.logType == _LogType.Log)
                content = logContent;
            else if (log.logType == _LogType.Warning)
                content = warningContent;
            else
                content = errorContent;
            //content.text = log.condition ;

            var currentLogStyle = (startIndex + index) % 2 == 0 ? evenLogStyle : oddLogStyle;
            if (log == selectedLog)
                //selectedLog = log ;
                currentLogStyle = selectedLogStyle;

            tempContent.text = log.count.ToString();
            var w = 0f;
            if (collapse)
                w = barStyle.CalcSize(tempContent).x + 3;
            countRect.x = Screen.width - w;
            countRect.y = size.y * i;
            if (beforeHeight > 0)
                countRect.y += 8; //i will check later why
            countRect.width = w;
            countRect.height = size.y;

            if (scrollerVisible)
                countRect.x -= size.x * 2;

            var sample = samples[log.sampleId];
            fpsRect = countRect;
            if (showFps)
            {
                tempContent.text = sample.fpsText;
                w = currentLogStyle.CalcSize(tempContent).x + size.x;
                fpsRect.x -= w;
                fpsRect.width = size.x;
                fpsLabelRect = fpsRect;
                fpsLabelRect.x += size.x;
                fpsLabelRect.width = w - size.x;
            }


            memoryRect = fpsRect;
            if (showMemory)
            {
                tempContent.text = sample.memory.ToString("0.000");
                w = currentLogStyle.CalcSize(tempContent).x + size.x;
                memoryRect.x -= w;
                memoryRect.width = size.x;
                memoryLabelRect = memoryRect;
                memoryLabelRect.x += size.x;
                memoryLabelRect.width = w - size.x;
            }

            sceneRect = memoryRect;
            if (showScene)
            {
                //tempContent.text = scenes[ sample.loadedScene];
                w = currentLogStyle.CalcSize(tempContent).x + size.x;
                sceneRect.x -= w;
                sceneRect.width = size.x;
                sceneLabelRect = sceneRect;
                sceneLabelRect.x += size.x;
                sceneLabelRect.width = w - size.x;
            }

            timeRect = sceneRect;
            if (showTime)
            {
                tempContent.text = sample.time.ToString("0.000");
                w = currentLogStyle.CalcSize(tempContent).x + size.x;
                timeRect.x -= w;
                timeRect.width = size.x;
                timeLabelRect = timeRect;
                timeLabelRect.x += size.x;
                timeLabelRect.width = w - size.x;
            }


            GUILayout.BeginHorizontal(currentLogStyle);
            if (log == selectedLog)
            {
                GUILayout.Box(content, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
                GUILayout.Label(log.condition, selectedLogFontStyle);
                //GUILayout.FlexibleSpace();
                if (showTime)
                {
                    GUI.Box(timeRect, showTimeContent, currentLogStyle);
                    GUI.Label(timeLabelRect, sample.time.ToString("0.000"), currentLogStyle);
                }

                if (showScene) GUI.Box(sceneRect, showSceneContent, currentLogStyle);
                //GUI.Label( sceneLabelRect ,scenes[ sample.loadedScene] ,currentLogStyle   );
                if (showMemory)
                {
                    GUI.Box(memoryRect, showMemoryContent, currentLogStyle);
                    GUI.Label(memoryLabelRect, sample.memory.ToString("0.000") + " mb", currentLogStyle);
                }

                if (showFps)
                {
                    GUI.Box(fpsRect, showFpsContent, currentLogStyle);
                    GUI.Label(fpsLabelRect, sample.fpsText, currentLogStyle);
                }
            }
            else
            {
                if (GUILayout.Button(content, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y)))
                    //selectedIndex = startIndex + index ;
                    selectedLog = log;
                if (GUILayout.Button(log.condition, logButtonStyle))
                    //selectedIndex = startIndex + index ;
                    selectedLog = log;
                //GUILayout.FlexibleSpace();
                if (showTime)
                {
                    GUI.Box(timeRect, showTimeContent, currentLogStyle);
                    GUI.Label(timeLabelRect, sample.time.ToString("0.000"), currentLogStyle);
                }

                if (showScene) GUI.Box(sceneRect, showSceneContent, currentLogStyle);
                //GUI.Label( sceneLabelRect, scenes[ sample.loadedScene] ,currentLogStyle  ) ;
                if (showMemory)
                {
                    GUI.Box(memoryRect, showMemoryContent, currentLogStyle);
                    GUI.Label(memoryLabelRect, sample.memory.ToString("0.000") + " mb", currentLogStyle);
                }

                if (showFps)
                {
                    GUI.Box(fpsRect, showFpsContent, currentLogStyle);
                    GUI.Label(fpsLabelRect, sample.fpsText, currentLogStyle);
                }
            }

            if (collapse)
                GUI.Label(countRect, log.count.ToString(), barStyle);
            GUILayout.EndHorizontal();
            index++;
        }

        var afterHeight = (int)((totalCount - (startIndex + totalVisibleCount)) * size.y);
        if (afterHeight > 0)
        {
            //fill invisible gap after scroller to make proper scroller pos
            GUILayout.BeginHorizontal(GUILayout.Height(afterHeight));
            GUILayout.Label(" ");
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        buttomRect.x = 0f;
        buttomRect.y = Screen.height - size.y;
        buttomRect.width = Screen.width;
        buttomRect.height = size.y;

        if (showGraph)
            drawGraph();
        else
            drawStack();
    }

    private void drawGraph()
    {
        graphRect = stackRect;
        graphRect.height = Screen.height * 0.25f; //- size.y ;


        //startFrame = samples.Count - (int)(Screen.width / graphSize) ;
        //if( startFrame < 0 ) startFrame = 0 ;
        GUI.skin = graphScrollerSkin;

        var drag = getDrag();
        if (graphRect.Contains(new Vector2(downPos.x, Screen.height - downPos.y)))
        {
            if (drag.x != 0)
            {
                graphScrollerPos.x -= drag.x - oldDrag3;
                graphScrollerPos.x = Mathf.Max(0, graphScrollerPos.x);
            }

            var p = downPos;
            if (p != Vector2.zero) currentFrame = startFrame + (int)(p.x / graphSize);
        }

        oldDrag3 = drag.x;
        GUILayout.BeginArea(graphRect, backStyle);

        graphScrollerPos = GUILayout.BeginScrollView(graphScrollerPos);
        startFrame = (int)(graphScrollerPos.x / graphSize);
        if (graphScrollerPos.x >= samples.Count * graphSize - Screen.width)
            graphScrollerPos.x += graphSize;

        GUILayout.Label(" ", GUILayout.Width(samples.Count * graphSize));
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        maxFpsValue = 0;
        minFpsValue = 100000;
        maxMemoryValue = 0;
        minMemoryValue = 100000;
        for (var i = 0; i < Screen.width / graphSize; i++)
        {
            var index = startFrame + i;
            if (index >= samples.Count)
                break;
            var s = samples[index];
            if (maxFpsValue < s.fps) maxFpsValue = s.fps;
            if (minFpsValue > s.fps) minFpsValue = s.fps;
            if (maxMemoryValue < s.memory) maxMemoryValue = s.memory;
            if (minMemoryValue > s.memory) minMemoryValue = s.memory;
        }

        //GUI.BeginGroup(graphRect);


        if (currentFrame != -1 && currentFrame < samples.Count)
        {
            var selectedSample = samples[currentFrame];
            GUILayout.BeginArea(buttomRect, backStyle);
            GUILayout.BeginHorizontal();

            GUILayout.Box(showTimeContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.time.ToString("0.0"), nonStyle);
            GUILayout.Space(size.x);

            GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            //GUILayout.Label( scenes[ selectedSample.loadedScene],nonStyle );
            GUILayout.Space(size.x);

            GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.memory.ToString("0.000"), nonStyle);
            GUILayout.Space(size.x);

            GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.fpsText, nonStyle);
            GUILayout.Space(size.x);

            GUILayout.Box(graphContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(currentFrame.ToString(), nonStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        graphMaxRect = stackRect;
        graphMaxRect.height = size.y;
        GUILayout.BeginArea(graphMaxRect);
        GUILayout.BeginHorizontal();

        GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Label(maxMemoryValue.ToString("0.000"), nonStyle);

        GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Label(maxFpsValue.ToString("0.000"), nonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        graphMinRect = stackRect;
        graphMinRect.y = stackRect.y + stackRect.height - size.y;
        graphMinRect.height = size.y;
        GUILayout.BeginArea(graphMinRect);
        GUILayout.BeginHorizontal();

        GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));

        GUILayout.Label(minMemoryValue.ToString("0.000"), nonStyle);


        GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));

        GUILayout.Label(minFpsValue.ToString("0.000"), nonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        //GUI.EndGroup();
    }

    private void drawStack()
    {
        if (selectedLog != null)
        {
            var drag = getDrag();
            if (drag.y != 0 && stackRect.Contains(new Vector2(downPos.x, Screen.height - downPos.y))) scrollPosition2.y += drag.y - oldDrag2;
            oldDrag2 = drag.y;


            GUILayout.BeginArea(stackRect, backStyle);
            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2);
            Sample selectedSample = null;
            try
            {
                selectedSample = samples[selectedLog.sampleId];
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(selectedLog.condition, stackLabelStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(size.y * 0.25f);
            GUILayout.BeginHorizontal();
            GUILayout.Label(selectedLog.stacktrace, stackLabelStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(size.y);
            GUILayout.EndScrollView();
            GUILayout.EndArea();


            GUILayout.BeginArea(buttomRect, backStyle);
            GUILayout.BeginHorizontal();

            GUILayout.Box(showTimeContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.time.ToString("0.000"), nonStyle);
            GUILayout.Space(size.x);

            GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            //GUILayout.Label( scenes[ selectedSample.loadedScene],nonStyle );
            GUILayout.Space(size.x);

            GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.memory.ToString("0.000"), nonStyle);
            GUILayout.Space(size.x);

            GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.fpsText, nonStyle);
            /*GUILayout.Space( size.x );
            GUILayout.Box( graphContent ,nonStyle, GUILayout.Width(size.x) ,GUILayout.Height(size.y));
            GUILayout.Label( selectedLog.sampleId.ToString() ,nonStyle  );*/
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        else
        {
            GUILayout.BeginArea(stackRect, backStyle);
            GUILayout.EndArea();
            GUILayout.BeginArea(buttomRect, backStyle);
            GUILayout.EndArea();
        }
    }


    public void OnGUIDraw()
    {
        if (!show) return;

        screenRect.x = 0;
        screenRect.y = 0;
        screenRect.width = Screen.width;
        screenRect.height = Screen.height;

        getDownPos();


        logsRect.x = 0f;
        logsRect.y = size.y * 2f;
        logsRect.width = Screen.width;
        logsRect.height = Screen.height * 0.75f - size.y * 2f;

        stackRectTopLeft.x = 0f;
        stackRect.x = 0f;
        stackRectTopLeft.y = Screen.height * 0.75f;
        stackRect.y = Screen.height * 0.75f;
        stackRect.width = Screen.width;
        stackRect.height = Screen.height * 0.25f - size.y;


        detailRect.x = 0f;
        detailRect.y = Screen.height - size.y * 3;
        detailRect.width = Screen.width;
        detailRect.height = size.y * 3;

        if (currentView == ReportView.Info)
        {
            DrawInfo();
        }
        else if (currentView == ReportView.Logs)
        {
            drawToolBar();
            DrawLogs();
        }
    }


    private bool isTouchInput()
    {
#if UNITY_EDITOR
        return false;
#else
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return true;
        }
        return false;
#endif
    }

    private bool isMouseInput()
    {
#if UNITY_EDITOR
        return true;
#else
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return false;
        }
        return true;
#endif
    }

    private bool isGestureDone()
    {
        if (isTouchInput())
        {
            if (Input.touches.Length != 1)
            {
                gestureDetector.Clear();
                gestureCount = 0;
            }
            else
            {
                if (Input.touches[0].phase == TouchPhase.Canceled || Input.touches[0].phase == TouchPhase.Ended)
                {
                    gestureDetector.Clear();
                }
                else if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    var p = Input.touches[0].position;
                    if (gestureDetector.Count == 0 || (p - gestureDetector[gestureDetector.Count - 1]).magnitude > 10)
                        gestureDetector.Add(p);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                gestureDetector.Clear();
                gestureCount = 0;
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    var p = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    if (gestureDetector.Count == 0 || (p - gestureDetector[gestureDetector.Count - 1]).magnitude > 10)
                        gestureDetector.Add(p);
                }
            }
        }

        if (gestureDetector.Count < 10)
            return false;

        gestureSum = Vector2.zero;
        gestureLength = 0;
        var prevDelta = Vector2.zero;
        for (var i = 0; i < gestureDetector.Count - 2; i++)
        {
            var delta = gestureDetector[i + 1] - gestureDetector[i];
            var deltaLength = delta.magnitude;
            gestureSum += delta;
            gestureLength += deltaLength;

            var dot = Vector2.Dot(delta, prevDelta);
            if (dot < 0f)
            {
                gestureDetector.Clear();
                gestureCount = 0;
                return false;
            }

            prevDelta = delta;
        }

        var gestureBase = (Screen.width + Screen.height) / 4;

        if (gestureLength > gestureBase && gestureSum.magnitude < gestureBase / 2)
        {
            gestureDetector.Clear();
            gestureCount++;
            if (gestureCount >= numOfCircleToShow)
                return true;
        }

        return false;
    }

    private bool isDoubleClickDone()
    {
        if (isTouchInput())
        {
            if (Input.touches.Length != 1)
            {
                lastClickTime = -1;
            }
            else
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    if (lastClickTime == -1)
                    {
                        lastClickTime = Time.realtimeSinceStartup;
                    }
                    else if (Time.realtimeSinceStartup - lastClickTime < 0.2f)
                    {
                        lastClickTime = -1;
                        return true;
                    }
                    else
                    {
                        lastClickTime = Time.realtimeSinceStartup;
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (lastClickTime == -1)
                {
                    lastClickTime = Time.realtimeSinceStartup;
                }
                else if (Time.realtimeSinceStartup - lastClickTime < 0.2f)
                {
                    lastClickTime = -1;
                    return true;
                }
                else
                {
                    lastClickTime = Time.realtimeSinceStartup;
                }
            }
        }

        return false;
    }

    private Vector2 getDownPos()
    {
        if (isTouchInput())
        {
            if (Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Began)
            {
                downPos = Input.touches[0].position;
                return downPos;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                downPos.x = Input.mousePosition.x;
                downPos.y = Input.mousePosition.y;
                return downPos;
            }
        }

        return Vector2.zero;
    }

    private Vector2 getDrag()
    {
        if (isTouchInput())
        {
            if (Input.touches.Length != 1) return Vector2.zero;
            return Input.touches[0].position - downPos;
        }

        if (Input.GetMouseButton(0))
        {
            mousePosition = Input.mousePosition;
            return mousePosition - downPos;
        }

        return Vector2.zero;
        //return Vector2.zero;
    }

    //calculate the start index of visible log
    private void calculateStartIndex()
    {
        startIndex = (int)(scrollPosition.y / size.y);
        startIndex = Mathf.Clamp(startIndex, 0, currentLog.Count);
    }

    //Rect temp;
    public void doShow()
    {
        show = true;
        currentView = ReportView.Logs;
        var reporterGui = gameObject.GetComponent<ReporterGUI>();
        if (reporterGui == null) gameObject.AddComponent<ReporterGUI>();


        try
        {
            gameObject.SendMessage("OnShowReporter");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }


    private void AddLog(string condition, string stacktrace, LogType type)
    {
        var memUsage = 0f;
        var _condition = "";
        if (!cachedString.TryGetValue(condition, out _condition))
        {
            _condition = condition;
            cachedString.Add(_condition, _condition);
            memUsage += string.IsNullOrEmpty(_condition) ? 0 : _condition.Length * sizeof(char);
            memUsage += IntPtr.Size;
        }

        var _stacktrace = "";
        if (!cachedString.TryGetValue(stacktrace, out _stacktrace))
        {
            _stacktrace = stacktrace;
            cachedString.Add(_stacktrace, _stacktrace);
            memUsage += string.IsNullOrEmpty(_stacktrace) ? 0 : _stacktrace.Length * sizeof(char);
            memUsage += IntPtr.Size;
        }

        var newLogAdded = false;

        addSample();
        var log = new Log { logType = (_LogType)type, condition = _condition, stacktrace = _stacktrace, sampleId = samples.Count - 1, frameCount = Time.frameCount };
        memUsage += log.GetMemoryUsage();
        //memUsage += samples.Count * 13 ;

        logsMemUsage += memUsage / 1024 / 1024;

        if (TotalMemUsage > maxSize)
        {
            clear();
            Debug.Log("Memory Usage Reach" + maxSize + " mb So It is Cleared");
            return;
        }

        var isNew = false;
        //string key = _condition;// + "_!_" + _stacktrace ;
        if (logsDic.ContainsKey(_condition, stacktrace))
        {
            isNew = false;
            logsDic[_condition][stacktrace].count++;
        }
        else
        {
            isNew = true;
            collapsedLogs.Add(log);
            logsDic[_condition][stacktrace] = log;

            if (type == LogType.Log)
                numOfCollapsedLogs++;
            else if (type == LogType.Warning)
                numOfCollapsedLogsWarning++;
            else
                numOfCollapsedLogsError++;
        }

        if (type == LogType.Log)
            numOfLogs++;
        else if (type == LogType.Warning)
            numOfLogsWarning++;
        else
            numOfLogsError++;


        logs.Add(log);
        if (!collapse || isNew)
        {
            var skip = false;
            if (log.logType == _LogType.Log && !showLog)
                skip = true;
            if (log.logType == _LogType.Warning && !showWarning)
                skip = true;
            if (log.logType == _LogType.Error && !showError)
                skip = true;
            if (log.logType == _LogType.Assert && !showError)
                skip = true;
            if (log.logType == _LogType.Exception && !showError)
                skip = true;

            if (!skip)
                if (string.IsNullOrEmpty(filterText) || log.condition.ToLower().Contains(filterText.ToLower()))
                {
                    currentLog.Add(log);
                    newLogAdded = true;
                }
        }

        if (newLogAdded)
        {
            calculateStartIndex();
            var totalCount = currentLog.Count;
            var totalVisibleCount = (int)(Screen.height * 0.75f / size.y);
            if (startIndex >= totalCount - totalVisibleCount)
                scrollPosition.y += size.y;
        }

//        try
//        {
//            gameObject.SendMessage("OnLog", log);
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogException(e);
//        }
    }

    public void CaptureLogThread(string condition, string stacktrace, LogType type)
    {
        var log = new Log { condition = condition, stacktrace = stacktrace, logType = (_LogType)type, frameCount = _frameCount };
        lock (threadedLogs)
        {
            threadedLogs.Add(log);
        }
    }

    //read build information 
    private IEnumerator readInfo()
    {
        var prefFile = "build_info.txt";
        var url = prefFile;

        if (prefFile.IndexOf("://") == -1)
        {
            var streamingAssetsPath = Application.streamingAssetsPath;
            if (streamingAssetsPath == "")
                streamingAssetsPath = Application.dataPath + "/StreamingAssets/";
            url = Path.Combine(streamingAssetsPath, prefFile);
        }

        //if (Application.platform != RuntimePlatform.OSXWebPlayer && Application.platform != RuntimePlatform.WindowsWebPlayer)
        if (!url.Contains("://"))
            url = "file://" + url;

       
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                buildDate = www.downloadHandler.text;
            }
        }
    }


    public class Sample
    {
        public float fps;
        public string fpsText;
        public byte loadedScene;
        public float memory;
        public float time;

        public static float MemSize()
        {
            float s = sizeof(float) + sizeof(byte) + sizeof(float) + sizeof(float);
            return s;
        }

        public void dispose()
        {
            fpsText = null;
        }
    }


    public class Log
    {
        public string condition;
        public int count = 1;
        public int frameCount;
        public _LogType logType;
        public int sampleId;

        public string stacktrace;
        //public string   objectName="" ;//object who send error
        //public string   rootName =""; //root of object send error

        public Log CreateCopy()
        {
            return (Log)MemberwiseClone();
        }

        public float GetMemoryUsage()
        {
            return sizeof(int) +
                   sizeof(_LogType) +
                   condition.Length * sizeof(char) +
                   stacktrace.Length * sizeof(char) +
                   sizeof(int);
        }

        public void dispose()
        {
            condition = null;
            stacktrace = null;
        }
    }

    //List<Texture2D> snapshots = new List<Texture2D>() ;

    private enum ReportView
    {
        None,
        Logs,
        Info,
        Snapshot
    }

    private enum DetailView
    {
        None,
        StackTrace,
        Graph
    }
}