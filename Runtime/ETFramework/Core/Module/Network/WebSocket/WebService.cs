using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
#if WEBSOCKET
using System.Text;
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


    public class WebService: AService
    {
        private Dictionary<long, WebChannel> idChannels = new Dictionary<long, WebChannel>();
        private Dictionary<long, WebChannel> connectChannels = new Dictionary<long, WebChannel>();
        private readonly Dictionary<long, IPEndPoint> channelAddress = new Dictionary<long, IPEndPoint>();
        
        private WebSocketServer webSocketServer = null;

        public ConcurrentQueue<WArgs> Queue = new ConcurrentQueue<WArgs>();

        //客户端使用，不启动服务，仅连接
        public WebService(AddressFamily addressFamily, ServiceType serviceType)
        {
            this.ServiceType = serviceType;
        }

        //服务端使用，启动一个WebSocket服务
        public WebService(IPEndPoint ipEndPoint, ServiceType serviceType)
        {
            this.ServiceType = serviceType;
            //var wssv = new WebSocketServer ("ws://127.0.0.1:4649");
            string url = $"ws://{ipEndPoint}";
            this.webSocketServer = new WebSocketServer(url);
            #if WebLog
            #endif
            Log.Info($"[WebService] WebSocketServer: {url} {serviceType}");
            
            // this.webSocketServer.SslConfiguration.ServerCertificate = new X509Certificate2("ssl/cdn.jijianquan.cn.pfx", "n0scpksa");
            this.webSocketServer.Log.Level = LogLevel.Error;
            this.webSocketServer.AddWebSocketService<WebBehavior>($"/WebBehavior{serviceType}", behavior =>
            {
                behavior.OnOpenAction = OnAcceptOpen;
            });
            this.webSocketServer.Start();
            // LogServiceMsg();
        }
        
        private void OnAcceptOpen(WebBehavior webBehavior, IPEndPoint ipEndPoint)
        {
            // Log.Console($"[WebService] OnOpen: {webBehavior.ID} {ipEndPoint}");
            this.CreateByAccept(webBehavior, ipEndPoint);
        }

        private void LogServiceMsg()
        {
            if (webSocketServer.IsListening)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"WebService Listening on port {webSocketServer.Port}, and providing WebSocket services:");
                foreach (var path in webSocketServer.WebSocketServices.Paths)
                {
                    stringBuilder.AppendLine($"- {path}");
                }

                Log.Console(stringBuilder.ToString());
            }
        }
        
        //NetClient专用
        public override void RegisterChannelAddress(long channelId, IPEndPoint realIpEndPoint)
        {
            this.channelAddress[channelId] = realIpEndPoint;
        }
        
        //NetClient专用
        public IPEndPoint GetChannelRealAddress(long channelId)
        {
            IPEndPoint ipEndPoint;
            if (this.channelAddress.TryGetValue(channelId, out ipEndPoint))
            {
                return ipEndPoint;
            }

            return null;
        }

        //移除一个链接
        public override void Remove(long id, int error = 0)
        {
            WebChannel channel;
            if (!this.idChannels.TryGetValue(id, out channel))
            {
                return;
            }

            channel.Error = error;
            this.channelAddress.Remove(id);
            this.idChannels.Remove(id);
            this.connectChannels.Remove(id);
            channel.Dispose();
        }

        //创建一个链接（accept）
        public void CreateByAccept(WebBehavior webBehavior, IPEndPoint address)
        {
            long id = NetServices.Instance.CreateAcceptChannelId();
            WebChannel channel = new WebChannel(id, this, webBehavior, address);
            this.idChannels.Add(channel.Id, channel);
            long channelId = channel.Id;
            NetServices.Instance.OnAccept(this.Id, channelId, channel.RemoteAddress);
        }

        //创建一个链接（connect)
        public override void Create(long id, IPEndPoint address)
        {
            if (this.idChannels.TryGetValue(id, out WebChannel _))
            {
                return;
            }

            WebChannel channel = new WebChannel(id, this, address);
            this.idChannels.Add(channel.Id, channel);
            this.connectChannels.Add(channel.Id, channel);
        }

        private WebChannel Get(long id)
        {
            WebChannel channel = null;
            this.idChannels.TryGetValue(id, out channel);
            return channel;
        }

        //对外发送消息
        public override void Send(long channelId, long actorId, object message)
        {
            try
            {
#if WebLog
                Log.Console($"[WebService] Send: {channelId} {actorId} {message.GetType().Name}");
#endif
                WebChannel aChannel = this.Get(channelId);
                if (aChannel == null)
                {
                    NetServices.Instance.OnError(this.Id, channelId, ErrorCore.ERR_SendMessageNotFoundTChannel);
                    return;
                }

                MemoryStream memoryStream = this.GetMemoryStream(message);
                aChannel.Send(actorId, memoryStream);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public override void Update()
        {
            while (true)
            {
                if (!this.Queue.TryDequeue(out var result))
                {
                    break;
                }

                switch (result.Op)
                {
                    case WebOp.StartSend:
                    {
                        WebChannel tChannel = this.Get(result.ChannelId);
                        if (tChannel != null)
                        {
                            tChannel.StartSend();
                        }

                        break;
                    }
                    case WebOp.StartRecv:
                    {
                        // WebChannel tChannel = this.Get(result.ChannelId);
                        // if (tChannel != null)
                        // {
                        //     tChannel.StartRecv();
                        // }

                        break;
                    }
                    case WebOp.Connect:
                    {
                        WebChannel tChannel = this.Get(result.ChannelId);
                        if (tChannel != null)
                        {
                            tChannel.Connect();
                        }

                        break;
                    }
                }
            }
        }

        public override bool IsDispose()
        {
            return this.webSocketServer == null;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (this.idChannels != null)
            {
                foreach (var webChannel in this.idChannels.Values)
                {
                    webChannel.Dispose();
                }

                this.idChannels.Clear();
                this.idChannels = null;
            }

            if (this.connectChannels != null)
            {
                this.connectChannels.Clear();
                this.connectChannels = null;
            }

            if (this.webSocketServer != null)
            {
                Log.Console($"webSocketServer.Stop()");
                this.webSocketServer.Stop();
                this.webSocketServer = null;
            }
        }
    }
}
#endif