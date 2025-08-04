#if WEBSOCKET
using System;
using System.IO;

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
    public class WebChannel: AChannel
    {
        private readonly WebService Service;

        private readonly CircularBuffer recvBuffer = new CircularBuffer();
        private readonly CircularBuffer sendBuffer = new CircularBuffer();

        private readonly PacketParser parser;
        private readonly byte[] sendCache = new byte[Packet.OpcodeLength + Packet.ActorIdLength];
        private WebBehavior webBehavior;

        private WebSocket webSocket;
        private string connectUrl;

        private bool isSending;
        private bool isConnected;

        // private long pingTime;
        // private long pingVal; //延时 

        /// <summary>
        /// 连接通道-accept
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <param name="webBehavior"></param>
        /// <param name="ipEndPoint"></param>
        public WebChannel(long id, WebService service, WebBehavior webBehavior, IPEndPoint ipEndPoint)
        {
#if WebLog
            Log.Console($"[WebChannel] Accept: {id} {ipEndPoint}");
#endif
            this.ChannelType = ChannelType.Accept;
            this.Id = id;
            this.Service = service;
            this.parser = new PacketParser(this.recvBuffer, this.Service);

            this.RemoteAddress = ipEndPoint;

            this.webBehavior = webBehavior;
            this.webBehavior.channel = this;
            this.isConnected = true;
            this.isSending = false;

            this.Service.Queue.Enqueue(new WArgs() { Op = WebOp.StartSend, ChannelId = this.Id });
            this.Service.Queue.Enqueue(new WArgs() { Op = WebOp.StartRecv, ChannelId = this.Id });
        }

        /// <summary>
        /// 链接通道-connect
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <param name="ipEndPoint"></param>
        public WebChannel(long id, WebService service, IPEndPoint ipEndPoint)
        {
#if WebLog
            Log.Console($"[WebChannel] Connect: {id} {ipEndPoint}");
#endif
            this.ChannelType = ChannelType.Connect;
            this.Id = id;
            this.Service = service;
            this.parser = new PacketParser(this.recvBuffer, this.Service);

            this.RemoteAddress = ipEndPoint;

            switch (this.Service.ServiceType)
            {
                case ServiceType.None:
                    this.connectUrl = $"wss://{ipEndPoint}/WebRouterBehavior";
                    break;
                case ServiceType.Inner:
                    this.connectUrl = $"ws://{ipEndPoint}/WebBehavior{ServiceType.Inner}";
                    break;
                case ServiceType.Outer://用不到
                    break;
            }

            this.webSocket = new WebSocket(connectUrl);
            this.webSocket.OnOpen += OnOpen;
            this.webSocket.OnMessage += OnMessage;
            this.webSocket.OnError += OnError;
            this.webSocket.OnClose += OnClose;
            this.webSocket.EmitOnPing = true; // 收到ping消息也触发OnMessage

            this.isConnected = false;
            this.isSending = false;

            this.Service.Queue.Enqueue(new WArgs() { Op = WebOp.Connect, ChannelId = this.Id });
        }

        // private void checkPing()
        // {
        //     if (this.ChannelType != ChannelType.Connect) return;
        //     if (!isConnected) return;
        //     long nowTime = TimeHelper.ClientNow();
        //     //2秒一次
        //     if (nowTime - this.pingTime < 2000) return;
        //     this.pingTime = nowTime;
        //     this.webSocket.Ping();
        //     Log.Console("Ping");
        // }

        public void Connect()
        {
            if (this.IsDisposed)
            {
                return;
            }
#if WebLog
            Log.Console($"[WebChannel] Connect: {this.Id} {this.connectUrl}");
#endif
            this.webSocket.Connect();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }


            long id = this.Id;
            this.Id = 0;
            this.Service.Remove(id);
#if WebLog
            Log.Console($"[WebChannel] dispose 1: {this.Id} {this.RemoteAddress} {this.Error}");
#endif
            if (this.webSocket != null)
            {
                this.webSocket.Close();
                this.webSocket = null;
            }

            recvBuffer.Dispose();
            sendBuffer.Dispose();
            this.webBehavior = null;
#if WebLog
            Log.Console($"[WebChannel] dispose 2: {this.Id} {this.RemoteAddress} {this.Error}");
#endif
        }

        //对外链接的接收connect
        public void OnMessage(object sender, MessageEventArgs e)
        {
            NetServices.Instance.OnRecvBytes(this.Service.Id, this.Id);
            if (e.IsPing)
            {
#if WebLog
                Log.Console($"[WebChannel] OnPing");
#endif
                return;
            }

            if (e.IsBinary)
            {
#if WebLog
                Log.Console($"[WebChannel] OnMessage: {e.RawData.Length}");
#endif
                StartRecv(e.RawData);
            }

            if (e.IsText)
            {
#if WebLog
                Log.Console($"[WebChannel] OnText");
#endif
            }
        }

        public void OnBehaviorMessage(object sender, MessageEventArgs e)
        {
            NetServices.Instance.OnRecvBytes(this.Service.Id, this.Id);
            if (e.IsPing)
            {
#if WebLog
                Log.Console($"[WebChannel] OnBehaviorPing");
#endif
                return;
            }

            if (e.IsBinary)
            {
#if WebLog
                Log.Console($"[WebChannel] OnBehaviorMessage: {e.RawData.Length}");
#endif
                StartRecv(e.RawData);
            }

            if (e.IsText)
            {
#if WebLog
                Log.Console($"[WebChannel] OnBehaviorText:{e.Data}");
#endif
            }
        }

        private void HandlerError(int error)
        {
#if WebLog
            Log.Console($"[WebChannel] error: {error} {this.RemoteAddress}");
#endif

            long channelId = this.Id;

            this.Service.Remove(channelId);

            NetServices.Instance.OnError(this.Service.Id, channelId, error);
        }

        public void OnError(object sender, ErrorEventArgs e)
        {
#if WebLog
            Log.Console($"[WebChannel] OnError: {e.Message} {this.RemoteAddress}");
#endif

            HandlerError(ErrorCore.ERR_WebsocketError);
        }

        private void OnOpen(object sender, EventArgs e)
        {
            if (this.isConnected)
            {
                return;
            }
#if WebLog
            Log.Console($"[WebChannel] OnOpen: {this.connectUrl} {e.ToString()}");
#endif

            //如果是客户端发起的链接，需要先发送一个真实访问地址消息
            if (this.Service.ServiceType == ServiceType.None)
            {
                SendRealAddress();
            }
            else
            {
                OnConnectFinish(true);
            }
        }

        private void SendMsg(string data)
        {
            if (this.ChannelType == ChannelType.Accept)
            {
                this.webBehavior.SendMsg(data);
            }
            else
            {
                this.webSocket.Send(data);
            }
        }

        private void SendMsg(byte[] data)
        {
            if (this.ChannelType == ChannelType.Accept)
            {
                this.webBehavior.SendMsg(data);
            }
            else
            {
                this.webSocket.Send(data);
            }
        }

        private void SendRealAddress()
        {
            IPEndPoint realAddress = this.Service.GetChannelRealAddress(this.Id);
            SendMsg(realAddress.ToString());
            OnConnectFinish(true);
        }

        private void OnConnectFinish(bool success)
        {
            if (!success)
            {
                //发送失败，重发
                SendRealAddress();
                return;
            }

            this.isConnected = true;
            this.Service.Queue.Enqueue(new WArgs() { Op = WebOp.StartSend, ChannelId = this.Id });
            this.Service.Queue.Enqueue(new WArgs() { Op = WebOp.StartRecv, ChannelId = this.Id });
        }

        public void OnClose(object sender, CloseEventArgs e)
        {
#if WebLog
            Log.Console($"[WebChannel] OnClose: {e.Code} {e.Reason}");
#endif
            this.Dispose();
        }

        public void StartSend()
        {
            if (!this.isConnected)
            {
                return;
            }

            if (this.isSending)
            {
                return;
            }

            while (true)
            {
                try
                {
                    if (this.IsDisposed)
                    {
                        this.isSending = false;
                        return;
                    }

                    // 没有数据需要发送
                    if (this.sendBuffer.Length == 0)
                    {
                        this.isSending = false;
                        return;
                    }

                    this.isSending = true;

                    int sendSize = this.sendBuffer.ChunkSize - this.sendBuffer.FirstIndex;
                    if (sendSize > this.sendBuffer.Length)
                    {
                        sendSize = (int)this.sendBuffer.Length;
                    }

                    byte[] tmpBuffer = new byte[sendSize];
                    Array.Copy(this.sendBuffer.First, this.sendBuffer.FirstIndex, tmpBuffer, 0, sendSize);

#if WebLog
                    Log.Console($"[WebChannel] Send:{sendSize} {this.ChannelType}");
#endif
                    SendMsg(tmpBuffer);
                    this.sendBuffer.FirstIndex += sendSize;
                    if (this.sendBuffer.FirstIndex == this.sendBuffer.ChunkSize)
                    {
                        this.sendBuffer.FirstIndex = 0;
                        this.sendBuffer.RemoveFirst();
                    }

                    OnSendComplete(true);
                }
                catch (Exception e)
                {
                    throw new Exception($"socket set buffer error: {this.sendBuffer.First.Length}, {this.sendBuffer.FirstIndex}", e);
                }
            }
        }

        public void OnSendComplete(bool success)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.isSending = false;

            this.Service.Queue.Enqueue(new WArgs() { Op = WebOp.StartSend, ChannelId = this.Id });
        }

        public void Send(long actorId, MemoryStream stream)
        {
            if (this.IsDisposed)
            {
                throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }

            switch (this.Service.ServiceType)
            {
                case ServiceType.Inner:
                {
                    int messageSize = (int)(stream.Length - stream.Position);
                    if (messageSize > ushort.MaxValue * 16)
                    {
                        throw new Exception($"send packet too large: {stream.Length} {stream.Position}");
                    }

                    this.sendCache.WriteTo(0, messageSize);
                    this.sendBuffer.Write(this.sendCache, 0, PacketParser.InnerPacketSizeLength);

                    // actorId
                    stream.GetBuffer().WriteTo(0, actorId);
                    this.sendBuffer.Write(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position));
                    break;
                }
                case ServiceType.None:
                case ServiceType.Outer:
                {
                    stream.Seek(Packet.ActorIdLength, SeekOrigin.Begin); // 外网不需要actorId
                    ushort messageSize = (ushort)(stream.Length - stream.Position);

                    this.sendCache.WriteTo(0, messageSize);
                    this.sendBuffer.Write(this.sendCache, 0, PacketParser.OuterPacketSizeLength);

                    this.sendBuffer.Write(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position));
                    break;
                }
            }

            if (!this.isSending)
            {
                //this.StartSend();
                this.Service.Queue.Enqueue(new WArgs() { Op = WebOp.StartSend, ChannelId = this.Id });
            }
        }

        public void StartRecv(byte[] recvBytes)
        {
            int startIndex = 0;
            while (true)
            {
                int recvLen = 0;
                try
                {
                    if (this.IsDisposed)
                    {
                        return;
                    }

                    //当前块剩余空间
                    int chunkCanRecvLen = this.recvBuffer.ChunkSize - this.recvBuffer.LastIndex;

                    //待接收数据剩余长度
                    int waitRecvLen = recvBytes.Length - startIndex;

                    //可接收的数据长度
                    recvLen = Math.Min(chunkCanRecvLen, waitRecvLen);


                    //接收
                    Array.Copy(recvBytes, startIndex, this.recvBuffer.Last, this.recvBuffer.LastIndex, recvLen);

                    //更新待接收数据的起始下标
                    startIndex += recvLen;
                }
                catch (Exception e)
                {
                    Log.Error($"[WebChannel]tchannel error: {this.Id}\n{e}");
                    this.HandlerError(ErrorCore.ERR_TChannelRecvError);
                    return;
                }

                this.HandleRecv(recvLen);
#if WebLog
                Log.Console($"[WebChannel]startIndex：{startIndex} recvLen：{recvLen} recvBytes.Length：{recvBytes.Length}");
#endif
                //如果待接收数据已经全部接收完毕，跳出循环
                if (startIndex == recvBytes.Length)
                {
                    return;
                }

            }
        }

        private void HandleRecv(int recvLen)
        {
            if (this.IsDisposed)
            {
                return;
            }

            // 后移当前块的起始下标
            this.recvBuffer.LastIndex += recvLen;
            // 如果当前块写满了，添加一个新的块
            if (this.recvBuffer.LastIndex == this.recvBuffer.ChunkSize)
            {
                this.recvBuffer.AddLast();
                this.recvBuffer.LastIndex = 0;
            }

            // 收到消息回调
            while (true)
            {
                // 这里循环解析消息执行，有可能，执行消息的过程中断开了session
                if (this.IsDisposed)
                {
                    return;
                }

                try
                {
                    bool ret = this.parser.Parse();
                    if (!ret)
                    {
                        break;
                    }

                    //收集齐完整的一个包
                    this.OnRead(this.parser.MemoryStream);
                }
                catch (Exception ee)
                {
                    Log.Error($"ip: {this.RemoteAddress} {ee}");
                    HandlerError(ErrorCore.ERR_SocketError);
                    return;
                }
            }
        }

        private void OnRead(MemoryStream memoryStream)
        {
            try
            {
                long channelId = this.Id;
                object message = null;
                long actorId = 0;
                switch (this.Service.ServiceType)
                {
                    case ServiceType.None:
                    case ServiceType.Outer:
                    {
                        ushort opcode = BitConverter.ToUInt16(memoryStream.GetBuffer(), Packet.KcpOpcodeIndex);
                        Type type = NetServices.Instance.GetType(opcode);
                        message = SerializeHelper.Deserialize(type, memoryStream);
                        break;
                    }
                    case ServiceType.Inner:
                    {
                        actorId = BitConverter.ToInt64(memoryStream.GetBuffer(), Packet.ActorIdIndex);
                        ushort opcode = BitConverter.ToUInt16(memoryStream.GetBuffer(), Packet.OpcodeIndex);
                        Type type = NetServices.Instance.GetType(opcode);
                        message = SerializeHelper.Deserialize(type, memoryStream);
                        break;
                    }
                }

                NetServices.Instance.OnRead(this.Service.Id, channelId, actorId, message);
            }
            catch (Exception e)
            {
                Log.Error($"{this.RemoteAddress} {memoryStream.Length} {e}");
                // 出现任何消息解析异常都要断开Session，防止客户端伪造消息
                this.HandlerError(ErrorCore.ERR_PacketParserError);
            }
        }
    }
}
#endif