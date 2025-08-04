namespace ET
{
    public class WebSocketUtil
    {
    }
    
    public enum WebOp
    {
        StartSend,
        StartRecv,
        Connect,
    }

    public struct WArgs
    {
        public WebOp Op;
        public long ChannelId;
    }
}