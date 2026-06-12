
#if NET_DATA_PB
using Google.Protobuf;
using System.Text;
using System;
using UnityEngine;
using BestHTTP.WebSocket;
using ZGame.Net;
using System.Threading;
using System.Net;
using System.IO;
using ZGame;
using ZGame.TimerTween;
using Timer = ZGame.TimerTween.Timer;
using System.Collections.Generic;

public enum WebSocketStatus
{
    Disconnect = 0,
    Connecting = 1,

    Connected = 2,
    Error = 3,
}
public class WebSocketManager : SingletonMonoBehaviour<WebSocketManager>
{
    string url = "";

    private WebSocket webSocket;
    WebSocketStatus status = WebSocketStatus.Disconnect;

    CircularBuffer recvBuffer;
    BlockingQueue<Message> msgQueue;

    public Action onOpen;
    public Action onError;
    public Action onClose;


    Dictionary<string, string> headerDic;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">url地址形如：ws://127.0.0.1:9999</param>
    void init(string url, Dictionary<string, string> headerDic)
    {
        this.url = url;
        this.headerDic = headerDic;
        webSocket = new WebSocket(new Uri(url));
        if (headerDic != null)
        {
            foreach (KeyValuePair<string, string> header in this.headerDic)
            {
                //当前版本的besthttp插件webgl版本无InternalRequest
#if !UNITY_WEBGL
                webSocket.InternalRequest.AddHeader(header.Key, header.Value);
                DebugExt.Log("add header:" + header.Key + "," + header.Value);
#endif

            }
        }

        webSocket.OnOpen += OnOpen;
        //webSocket.OnMessage += OnMessageReceived;
        webSocket.OnBinary += OnBinary;
        webSocket.OnError += OnError;
        webSocket.OnClosed += OnClosed;
        recvBuffer = new CircularBuffer(1024 * 1024 * 2);

        if (msgQueue == null)
        {
            msgQueue = new BlockingQueue<Message>();
        }
        else
        {
            msgQueue.Clear();
        }
    }


    bool calculateFlag = false;
    long receivedBytes = 0;
    string beginReceivedTime;
    private void OnBinary(WebSocket webSocket, byte[] data)
    {
        recvBuffer.Put(data, 0, data.Length);

        if (calculateFlag)
        {
            receivedBytes += data.Length;
        }

    }

    public void StartCalculateReceivedBytes()
    {
        calculateFlag = true;
        receivedBytes = 0;
        beginReceivedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        Debug.LogError("StartCalculateReceivedBytes");
    }
    public void EndCalculateReceivedBytes()
    {
        calculateFlag = false;
        string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        //打印
        Debug.LogError($"totalRecived:{receivedBytes} bytes({receivedBytes / 1024f / 1024}MB), startTime:{beginReceivedTime},endTime:{endTime}");
    }

    public void Connect(string url, Dictionary<string, string> headerDic)
    {
        this.init(url, headerDic);
        Debug.Log("begin connect to ws");
        status = WebSocketStatus.Connecting;
        webSocket.Open();
    }

    public bool IsConnected()
    {
        return status == WebSocketStatus.Connected;
    }
    public bool IsNull()
    {
        return webSocket == null;
    }




    private void Update()
    {
        if (IsNull())
        {
            return;
        }

        if (IsConnected() == false && NetConfig.handleBlockMsgAfterUnConnected == false)
        {
            return;
        }

        if (recvBuffer != null && recvBuffer.Count > 0)
        {
            this.loopPickupMessage();
        }
        ////if (msgQueue.Count() > 0)
        ////{ 
        ////    Message msg = msgQueue.Dequeue();
        ////    if (msg != null)
        ////    {
        ////        ProtobufMessage.HandleMessage(msg.mainCmdId.ToString(), msg.Content);
        ////    }
        ////}

        //if (msgQueue.Count() > 1)
        //{
        //    Debug.LogError("msgCount:" + msgQueue.Count());
        //}
        while (msgQueue.Count() > 0)
        {
            Message msg = msgQueue.Dequeue();
            if (msg != null)
            {
                ProtobufMessage.HandleMessage(msg.mainCmdId.ToString(), msg.Content);
            }
        }
    }

    bool isHandHeader = true;
    Int16 curMainCmdId = 0;
    Int32 curBodySize = 0;
    void loopPickupMessage()
    {
        while (true)
        {
            //////Thread.Sleep(3); 
            bool readRet = false;
            try
            {
                if (isHandHeader)
                {
                    readRet = readHeader();
                }
                else
                {
                    readRet = readBody();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("loopPickUpMessage error:" + ex.ToString());
                return;
            }

            if (!readRet)
            {
                //////Debug.LogError("xxxxxx");
                //////recvBuffer.Clear();//感觉这里没必要clear啊
                return;
            }

        }
    }
    bool readHeader()
    {
        if (recvBuffer.Count < 6)
        {
            //////Debug.LogError("read header fail,  recv count:" + recvBuffer.Count);
            return false;
        }

        int len = recvBuffer.ReadInt32();
        curMainCmdId = recvBuffer.ReadInt16();
        len = NetConfig.useNetworkOrder ? IPAddress.NetworkToHostOrder(len) : len;

        curMainCmdId = NetConfig.useNetworkOrder ? IPAddress.NetworkToHostOrder(curMainCmdId) : curMainCmdId;



        if (len >= 2)//curBodySize是可能为0的，即空消息体
        {
            curBodySize = (len - 2);
            isHandHeader = false;
        }
        return true;
    }
    bool readBody()
    {
        byte[] bytes = null;
        if (recvBuffer.Count < curBodySize)
        {
            Debug.LogError("read body fail, count:" + recvBuffer.Count + ", needSize:" + curBodySize + ", msgId:" + curMainCmdId);
            return false;
        }

        bytes = recvBuffer.ReadBytes(curBodySize);

        processPacket(bytes);
        isHandHeader = true;
        return true;
    }
    byte[] emptyBuf = new byte[0];
    void processPacket(byte[] data)
    {
        if (data == null)
            data = emptyBuf;

        if (msgQueue == null)
            return;

        msgQueue.Enqueue(new Message((ushort)curMainCmdId, data));
    }

    private void antiInit()
    {
        if (webSocket != null)
        {
            webSocket.OnOpen = null;
            webSocket.OnMessage = null;
            webSocket.OnError = null;
            webSocket.OnClosed = null;
            webSocket = null;
        }
        if (recvBuffer != null)
        {
            recvBuffer.Clear();
            recvBuffer = null;
        }
    }



    private byte[] getBytes(string message)
    {
        byte[] buffer = Encoding.Default.GetBytes(message);
        return buffer;
    }
    public void Send(string str)
    {
        webSocket.Send(str);
    }

    public void Send(ProtobufMsgID msgId, IMessage msg)
    {
        if (webSocket != null)
        {
            var des = ProtobufMsgIDDesUtils.GetIDDes(msgId);
            using (var ms = new MemoryStream())
            {
                ProtobufMessage.PrintMessage(msgId, msg);
                msg.WriteTo(ms);
                var data = ms.ToArray();
                send(des.msgId, data);
            }
        }
    }

    void send(UInt16 msgId, byte[] data)
    {
        Int32 totalLen = (data.Length + 2);
        byte[] sendBytes = new byte[totalLen + 4];
        Int32 totalLenOrdered = NetConfig.useNetworkOrder ? IPAddress.HostToNetworkOrder(totalLen) : totalLen;

        //Write length
        Array.Copy(BitConverter.GetBytes(totalLenOrdered), 0, sendBytes, 0, 4);
        //Write msgId
        Int16 msgIdOrdered = NetConfig.useNetworkOrder ? IPAddress.HostToNetworkOrder((Int16)msgId) : (Int16)msgId;
        Array.Copy(BitConverter.GetBytes(msgIdOrdered), 0, sendBytes, 4, 2);
        //Write data
        if (data != null)
        {
            Array.Copy(data, 0, sendBytes, 6, data.Length);
        }
        //Debug.Log("send msgId:" + msgId + ", length:" + data.Length);
        webSocket.Send(sendBytes);
    }


    public void Close()
    {
        webSocket.Close();
        status = WebSocketStatus.Disconnect;
    }
    void OnOpen(WebSocket ws)
    {
        WindowUtil.HideNetMask();
        status = WebSocketStatus.Connected;
        Debug.Log("ws connect suc");
        this.cancelReconnect();
        reconnectCount = 0;

        if (this.onOpen != null)
        {
            this.onOpen();
        }
    }
    /// <summary>
    /// 接收信息
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="msg">接收内容</param>
    void OnMessageReceived(WebSocket ws, string msg)
    {
        Debug.Log(msg);
    }



    /// <summary>
    /// 关闭连接
    /// </summary>
    void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        status = WebSocketStatus.Disconnect;
        Debug.LogError("ws close:" + message);

        if (this.onClose != null)
        {
            this.onClose();
        }

        antiInit();

    }

    private void OnDestroy()
    {
        if (webSocket != null && webSocket.IsOpen)
        {
            Debug.Log("ws OnDestroy");
            webSocket.Close();
            antiInit();
        }
    }

    private void OnError(WebSocket ws, string reason)
    {
        status = WebSocketStatus.Error;
        string errorMsg = string.Empty;
#if !UNITY_WEBGL || UNITY_EDITOR
        if (ws.InternalRequest.Response != null)
            errorMsg = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
#endif
        Debug.LogError("ws error:" + errorMsg + ",reason:" + reason);

        if (this.onError != null)
        {
            this.onError();
        }
        antiInit();
    }





    bool canReconnect = true;
    Timer delayReconnectTimer = null;
    long delayReconnectTimerId;
    int reconnectCount = 0;
    public void Reconnect()
    {
        if (canReconnect == false)
        {
            return;
        }
        reconnectCount = 0;
        WindowUtil.ShowNetMaskWithRoll("Reconnect2");
        TimerTween.Cancel(delayReconnectTimer, delayReconnectTimerId);

        //重连加个延迟，避免服务器或者网络通道还没有清理干净导致的重连不上
        delayReconnectTimer = TimerTween.Delay(3f, () =>
        {
            if (canReconnect)
            {
                Debug.LogError("Do reconnect!");
                reconnectCount++;
                if (reconnectCount >= 10)
                {
                    Debug.LogError("reconnect reach max value!");
                    WindowUtil.ShowTip(100001);//连接超时，请检查网络！
                    return;
                }
                this.antiInit();
                NetTool.socketConnectedStyle = SocketConnectedStyle.ReConnect;
                this.Connect(this.url, this.headerDic);
            }

        });
        delayReconnectTimer.Start(out delayReconnectTimerId);
    }

    void cancelReconnect()
    {
        TimerTween.Cancel(delayReconnectTimer, delayReconnectTimerId);
    }

    public void ForbidReconnet()
    {
        this.canReconnect = false;
        this.cancelReconnect();
    }
}

#endif
