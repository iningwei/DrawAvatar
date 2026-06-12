#if NET_DATA_PB
using Google.Protobuf; 
using Pb; 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame;
using ZGame.TimerTween;

public class PingManager : Singleton<PingManager>
{
    Timer sendTimer;
    long sendTimerId;

    int sendPingIndex = 0;
    int rcvPingIndex = 0;
    long curPingTimeStamp;
    float intervalTime = 30f;

    /// <summary>
    /// 精确到毫秒， 客户端和服务端的时差（非精确值，未考虑到网络延迟）， 大于0表示服务端时间快于客户端
    /// </summary>
    public float timeStampOffset;

    byte pingStatus = 0;//1为开，0为关

    ProtobufMsgID c2sMsgId;
    ProtobufMsgID s2cMsgId;

    Func<int, long, IMessage> c2sAssemble;
    Func<byte[], S2CPing> s2cHandle;
    public void StartPing(ProtobufMsgID c2sID, ProtobufMsgID s2cID, Func<int, long, IMessage> c2sAssemble, Func<byte[], S2CPing> s2cHandle)
    {
        Debug.Log("start Ping");
        ProtobufMessage.AddListener(s2cID, onS2CPing);

        this.c2sMsgId = c2sID;
        this.s2cMsgId = s2cID;
        this.c2sAssemble = c2sAssemble;
        this.s2cHandle = s2cHandle;

        sendTimer = TimerTween.Repeat(intervalTime, () =>
       {
           C2SPing();
           return true;
       }, true).SetTag("Send-Ping");
        sendTimer.Start(out sendTimerId);

        pingStatus = 1;
    }

    /// <summary>
    /// 发送心跳
    /// </summary>
    private void C2SPing()
    {
        if (rcvPingIndex != sendPingIndex)
        {
            pingTimeError();
            return;
        }
        sendPingIndex++;
        curPingTimeStamp = TimeTool.GetNowStamp();

        if (c2sAssemble != null)
        {
            SocketManager.Instance.Send(c2sMsgId, c2sAssemble(sendPingIndex, curPingTimeStamp));
        }
    }

    /// <summary>
    /// 收到心跳响应
    /// </summary>
    /// <param name="data"></param>
    private void onS2CPing(byte[] data)
    {
        if (this.s2cHandle != null)
        {
#if NET_DATA_PB
            S2CPing s2cData = this.s2cHandle(data);
            rcvPingIndex = s2cData.Index;
            timeStampOffset = s2cData.TimeStamp - TimeTool.GetNowStamp();
            //Debug.LogError("时间差：" + timeStampOffset + " 毫秒");
#endif
        }
    }

    private void pingTimeError()
    {
        Debug.LogError($"ping error, begin reconnect ！send:{sendPingIndex}，rcv:{rcvPingIndex}");

        this.StopPing();
        SocketManager.Instance.Reconnect();
    }


    /// <summary>
    /// 停止心跳
    /// </summary>
    public void StopPing()
    {
        if (pingStatus == 0)
        {
            return;
        }

        Debug.Log("stop ping");
        TimerTween.Cancel(sendTimer, sendTimerId);

        ProtobufMessage.RemoveListener(s2cMsgId, onS2CPing);
        this.c2sAssemble = null;
        this.s2cHandle = null;
        pingStatus = 0;
        sendPingIndex = 0;
        rcvPingIndex = 0;
    }
}
#endif