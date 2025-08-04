#if UNITY && WEBSOCKET
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace ET
{

    public class UnityWebService: AService
    {
        private Dictionary<long, UnityWebChannel> idChannels = new Dictionary<long, UnityWebChannel>();
        private Dictionary<long, UnityWebChannel> connectChannels = new Dictionary<long, UnityWebChannel>();
        private readonly Dictionary<long, IPEndPoint> channelAddress = new Dictionary<long, IPEndPoint>();
        
        private WebSocketServer webSocketServer = null;

        public ConcurrentQueue<WArgs> Queue = new ConcurrentQueue<WArgs>();

        //客户端使用，不启动服务，仅连接
        public UnityWebService(AddressFamily addressFamily, ServiceType serviceType)
        {
            this.ServiceType = serviceType;
        }
        
        //NetClient专用
        public override void RegisterChannelAddress(long channelId, IPEndPoint realIpEndPoint)
        {
            // Debug.Log($"RegisterChannelAddress: {channelId} {realIpEndPoint}");
            this.channelAddress[channelId] = realIpEndPoint;
        }
        
        //NetClient专用
        public IPEndPoint GetChannelRealAddress(long channelId)
        {
            IPEndPoint ipEndPoint;
            if (this.channelAddress.TryGetValue(channelId, out ipEndPoint))
            {
                // Debug.Log($"GetChannelRealAddress: {channelId} {ipEndPoint}");
                return ipEndPoint;
            }

            // Debug.Log($"GetChannelRealAddress: {channelId} null");
            return null;
        }

        //移除一个链接
        public override void Remove(long id, int error = 0)
        {
            UnityWebChannel channel;
            if (this.idChannels == null || !this.idChannels.TryGetValue(id, out channel))
            {
                return;
            }

            channel.Error = error;
            this.channelAddress.Remove(id);
            this.idChannels.Remove(id);
            this.connectChannels.Remove(id);
            channel.Dispose();
        }


        //创建一个链接（connect)
        public override void Create(long id, IPEndPoint address)
        {
            if (this.idChannels.TryGetValue(id, out UnityWebChannel _))
            {
                return;
            }

            UnityWebChannel channel = new UnityWebChannel(id, this, address);
            this.idChannels.Add(channel.Id, channel);
            this.connectChannels.Add(channel.Id, channel);
        }

        private UnityWebChannel Get(long id)
        {
            UnityWebChannel channel = null;
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
                UnityWebChannel aChannel = this.Get(channelId);
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
                        UnityWebChannel tChannel = this.Get(result.ChannelId);
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
                        UnityWebChannel tChannel = this.Get(result.ChannelId);
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