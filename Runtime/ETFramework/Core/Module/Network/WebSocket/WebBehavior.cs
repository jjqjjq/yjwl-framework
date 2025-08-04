using System;
using System.IO;
using System.Net;
#if WEBSOCKET
#if UNITY 
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;
#else
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;
#endif
using Log = ET.Log;

namespace ET
{
    /// <summary>
    /// 单个链接实例
    /// </summary>
    public class WebBehavior: WebSocketBehavior
    {
        public WebChannel channel;
        public Action<WebBehavior, IPEndPoint> OnOpenAction;

        public void SendMsg(string data)
        {
            this.Send(data);
        }

        public void SendMsg(byte[] bytes)
        {
            this.Send(bytes);
        }

        protected override void OnOpen()
        {
#if WebLog
            ET.Log.Console($"[WebBehavior] OnOpen: {this.ID} {this.UserEndPoint}");
#endif
            OnOpenAction?.Invoke(this, this.UserEndPoint);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
#if WebLog
            ET.Log.Console($"[WebBehavior] OnMessage: {this.ID}");
#endif
            this.channel.OnBehaviorMessage(this.webSocket, e);
        }

        protected override void OnClose(CloseEventArgs e)
        {
#if WebLog
            ET.Log.Console($"[WebBehavior] OnClose: {this.ID} {e.Code} {e.Reason}");
#endif
            this.channel.OnClose(this.webSocket, e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
#if WebLog
            ET.Log.Console($"[WebBehavior] OnError: {this.ID} {e.Message}");
#endif
            this.channel.OnError(this.webSocket, e);
        }
        
        
        public WebSocket webSocket
        {
            get
            {
                return (this as IWebSocketSession).WebSocket;
            }
        }
    }
}
#endif