#if WEBSOCKET
using System;
using System.Collections.Generic;
using System.Net;
#if UNITY
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;
#else
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;
#endif


namespace ET
{
    public class WebRouterBehavior: WebSocketBehavior
    {
        public Action<WebRouterBehavior, IPEndPoint> OnOpenAction;
        private string connectUrl;
        private WebSocket serverSocket;
        private bool isServerConnnected;
        private string realAddress;
        private Queue<byte[]> waitServerMsg = new Queue<byte[]>();

        protected override void OnOpen()
        {
#if WebLog
            ET.Log.Console($"[WebRouterBehavior-Out] OnOpen: {this.ID} {this.UserEndPoint}");
#endif
            if (OnOpenAction != null)
            {
                this.OnOpenAction(this, this.UserEndPoint);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            //第一条信息发送过来的是真实地址
            if (string.IsNullOrEmpty(realAddress) && e.IsText)
            {
                realAddress = e.Data;
#if WebLog
                ET.Log.Console($"[WebRouterBehavior-Out] RealAddress:{this.realAddress}");
#endif
                CloseServerSocket();
                CreateServerConnect();
                return;
            }

            if (!e.IsBinary)
            {
                return;
            }
#if WebLog
           ET.Log.Console($"[WebRouterBehavior-Out] OnMessage: {this.ID} {e.RawData.Length}");
#endif
            //如果没有连接上服务器，先缓存消息
            if (!this.isServerConnnected)
            {
                ET.Log.Console("       Enqueue");
                this.waitServerMsg.Enqueue(e.RawData);
                return;
            }
            
#if WebLog
            ET.Log.Console($"[WebRouterBehavior-Inner] Send:{e.RawData.Length}");
#endif
            this.serverSocket.Send(e.RawData); //转发给服务器
        }

        protected override void OnError(ErrorEventArgs e)
        {
#if WebLog
            ET.Log.Console($"[WebRouterBehavior-Out] OnError: {this.ID} {e.Message}");
#endif
            CloseServerSocket();
            this.Close();
        }

        protected override void OnClose(CloseEventArgs e)
        {
#if WebLog
            ET.Log.Console($"[WebRouterBehavior-Out] OnClose: {this.ID} {e.Code} {e.Reason}");
#endif
            CloseServerSocket();
        }

        #region NetServer

        // private long pingTime;
        // private long pingVal; //延时 

        public void Update()
        {
            // checkPing();
        }

        public void Destroy()
        {
            CloseServerSocket(CloseStatusCode.Normal, "Destroy");
        }

        // private void checkPing()
        // {
        //     if (!this.isServerConnnected) return;
        //     long nowTime = TimeHelper.ClientNow();
        //     //2秒一次
        //     if (nowTime - this.pingTime < 2000) return;
        //     this.pingTime = nowTime;
        //     this.serverSocket.Ping();
        //     ET.Log.Console($"[WebRouterBehavior-Inner] checkPing: {this.UserEndPoint}");
        // }

        private void CreateServerConnect()
        {
            if (serverSocket != null) return;
            //从路由转发到实际服务器
            this.isServerConnnected = false;
            this.connectUrl = $"ws://{realAddress}/WebBehavior{ServiceType.Outer}";
            serverSocket = new WebSocket(this.connectUrl);
            serverSocket.OnOpen += OnServerOpen;
            serverSocket.OnMessage += OnServerMessage;
            serverSocket.OnError += OnServerError;
            serverSocket.OnClose += OnServerClose;
            // serverSocket.EmitOnPing = true;
#if WebLog
            ET.Log.Console($"[WebRouterBehavior-Inner] Connect: {this.ID} {this.UserEndPoint} {this.connectUrl}");
#endif
            serverSocket.Connect();
        }

        private void OnServerOpen(object sender, EventArgs e)
        {
            if (this.serverSocket == null) return;
            
#if WebLog
            ET.Log.Console($"[WebRouterBehavior-Inner] OnServerOpen: {this.ID}");
#endif

            this.isServerConnnected = true;
            //处理未链接时收到的客户端消息
            while (this.waitServerMsg.Count > 0)
            {
                byte[] bytes = this.waitServerMsg.Dequeue();
                
#if WebLog
                ET.Log.Console($"OnServerOpen       Send{bytes.Length}");
#endif
                this.serverSocket.Send(bytes);
            }
        }

        private void OnServerMessage(object sender, MessageEventArgs e)
        {
            if (this.serverSocket == null) return;
            if (e.IsPing)
            {
                
#if WebLog
                ET.Log.Console($"[WebRouterBehavior-Inner] OnPing");
#endif
                return;
            }

            if (e.IsBinary)
            {
                
#if WebLog
                ET.Log.Console($"[WebRouterBehavior-Inner] OnServerMessage: {this.ID} {e.RawData.Length}");
#endif
                this.Send(e.RawData); //转发给客户端
                return;
            }

            if (e.IsText)
            {
#if WebLog
                ET.Log.Console($"[WebRouterBehavior-Inner] OnServerTest: {this.ID} {e.Data}");
#endif
            }
            // ET.Log.Console($"{e.Data} {e.IsText} {e.RawData.Length}");
        }

        private void OnServerError(object sender, ErrorEventArgs e)
        {
            
#if WebLog
            ET.Log.Console($"[WebRouterBehavior-Inner] OnServerError: {this.ID} {e.Message}]");
#endif
            CloseServerSocket();
            CreateServerConnect();
        }

        private void OnServerClose(object sender, CloseEventArgs e)
        {
            
#if WebLog
            ET.Log.Console($"[WebRouterBehavior-Inner] OnServerClose {realAddress} {e.Code} {e.Reason}]");
#endif
            if (e.Code == (int)CloseStatusCode.Normal && e.Reason == "Destroy")
            {
                //主动关闭
                return;
            }
        }

        private void CloseServerSocket(CloseStatusCode code = CloseStatusCode.NoStatus, string reason = "")
        {
            this.isServerConnnected = false;
            if (this.serverSocket != null)
            {
#if WebLog
                ET.Log.Console($"CloseServerSocket {code} {reason}");
#endif
                this.serverSocket.Close(code, reason);
                this.serverSocket = null;
            }
        }

        #endregion
    }
}
#endif