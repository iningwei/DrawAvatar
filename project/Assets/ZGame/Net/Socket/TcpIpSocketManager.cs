#if NET_DATA_PB
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using ZGame;
using ZGame.Net;
using ZGame.Net.Tcp;
using ZGame.TimerTween;

public class TcpIpSocketManager : SingletonMonoBehaviour<TcpIpSocketManager>
{
    public int socketId;
    public ClientSocket clientSocket;

    SocketState state = SocketState.UnConnect;
    public BlockingQueue<Message> msgQueue = new BlockingQueue<Message>();


    string ip;
    int port;
    public Action onConnected;
    public Action onClose;
    public Action onError;

    public bool IsConnected()
    {
        return clientSocket != null && state == SocketState.Success;
    }

    public bool IsClientSocketConnected()
    {
        if (clientSocket != null)
        {
            return clientSocket.IsConnected();
        }
        return false;
    }



    public void Update()
    {
        if (IsConnected() == false && NetConfig.handleBlockMsgAfterUnConnected == false)
        {
            return;
        }


        if (msgQueue.Count() > 0)
        {
            Message msg = msgQueue.Dequeue();
            if (msg != null)
            {
                //Debug.Log("begin hadle:" + msg.mainCmdId);
                ProtobufMessage.HandleMessage(msg.mainCmdId.ToString(), msg.Content);
            }
        }
    }

    public void LateUpdate()
    {
        if (clientSocket == null)
        {
            state = SocketState.Connecting;
            return;
        }
        if (clientSocket.socketId != socketId)
        {
            state = SocketState.UnConnect;
            Close();
            return;
        }

        if (state == SocketState.Connecting)
        {
            if (clientSocket.State == SocketState.Success)
            {
                state = SocketState.Success;
                onConnected?.Invoke();
            }

            if (clientSocket.State == SocketState.Close)
            {
                state = SocketState.Close;
                clientSocket.Close();
                clientSocket = null;
                onClose?.Invoke();

            }

            if (clientSocket.State == SocketState.Error)
            {
                state = SocketState.Error;
                clientSocket.Close();
                clientSocket = null;
                onError?.Invoke();

            }
        }
        else if (state == SocketState.Success)
        {
            if (clientSocket.State == SocketState.Close)
            {
                state = SocketState.Close;
                clientSocket.Close();
                clientSocket = null;
                onClose?.Invoke();


            }
            if (clientSocket.State == SocketState.Error)
            {
                state = SocketState.Error;
                clientSocket.Close();
                clientSocket = null;
                onError?.Invoke();

            }
        }
    }
    bool canReconnect = true;
    Timer delayReconnectTimer;
    long delayReconnectTimerId;
    int reconnectCount = 0;
    public void Reconnect(float timeOut = 5)
    {
        if (canReconnect == false)
        {
            return;
        }
        WindowUtil.ShowNetMaskWithRoll("Reconnect");

        TimerTween.Cancel(delayReconnectTimer, delayReconnectTimerId);

        //重连加个延迟，避免服务器或者网络通道还没有清理干净导致的重连不上
        delayReconnectTimer = TimerTween.Delay(2f, () =>
        {
            if (canReconnect)
            {
                reconnectCount++;
                if (reconnectCount > 5)
                {
                    Debug.LogError("reconnect reach max count!");

                    WindowUtil.ShowTip(100001);//连接超时，请检查网络！
                    WindowUtil.HideNetMask();
                    TcpIpSocketManager.Instance.Close();
                    return;
                }
                this.checkAndRemoveConnectedCallback(this.connectedCallback);
                this.onConnected += connectedCallback;
                this.Connect(this.ip, this.port, timeOut);
            }

        });
        delayReconnectTimer.Start(out delayReconnectTimerId);
    }

    void checkAndRemoveConnectedCallback(Action callbackAction)
    {
        if (this.onConnected != null && this.onConnected.GetInvocationList().Contains(callbackAction))
        {
            this.onConnected -= callbackAction;
        }
    }
    void connectedCallback()
    {
        WindowUtil.HideNetMask();
        Debug.LogError("连上了，设置 reconnectCount 为0");
        this.reconnectCount = 0;
        this.cancelReconnect();
    }

    public void Connect(string ip, int port, float timeOut = 5)
    {
        this.ip = ip;
        this.port = port;

        if (clientSocket != null)
        {
            clientSocket.Close();
        }
        msgQueue.Clear();
        state = SocketState.Connecting;
        clientSocket = new ClientSocket(++socketId, msgQueue);
        clientSocket.Connect(ip, port, timeOut);
    }


    public void Send(UInt16 mainId, IMessage msg)
    {
        using (var ms = new MemoryStream())
        {

            string name = ProtobufMsgIDDesUtils.GetNameById(mainId);

            //Debug.Log($"send {name},{mainId} :{msg.ToString()}");

            msg.WriteTo(ms);
            var data = ms.ToArray();
            Send(mainId, data);
        }
    }


    public void Send(UInt16 mainId, byte[] data)
    {
        if (clientSocket != null)
        {
            clientSocket.Send(mainId, data);
        }
    }



    public void Close()
    {
        if (clientSocket != null)
        {
            socketId++;
            clientSocket.Close();
            clientSocket = null;
            state = SocketState.UnConnect;
        }
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

    public void Destroy()
    {
        this.Close();
    }
}
#endif