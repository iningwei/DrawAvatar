

#if NET_DATA_PB
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ZGame.Net.Tcp;
using Google.Protobuf;
public class SocketManager : Singleton<SocketManager>
{
    public bool IsConnected()
    {
        switch (NetConfig.clientSocketType)
        {
            case ClientSocketType.TCPIP:
                Debug.LogError("not implementation yet!");
                break;
            case ClientSocketType.WEBSOCKET:
                return WebSocketManager.Instance.IsConnected();
            default:
                Debug.LogError("not implementation yet!");
                break;
        }
        return false;
    }
    public void Send(ProtobufMsgID msgId, IMessage msg)
    {
        switch (NetConfig.clientSocketType)
        {
            case ClientSocketType.TCPIP:
                break;
            case ClientSocketType.WEBSOCKET:
                WebSocketManager.Instance.Send(msgId, msg);
                break;
            default:
                Debug.LogError("not implementation yet!");
                break;
        }
    }

    public T Deserialize<T>(byte[] data) where T : IMessage<T>, new()
    {
        //ProtoType:
        //S2CAccountLogin msg = S2CAccountLogin.Parser.ParseFrom(data);

        Type type = typeof(T);
        PropertyInfo pInfo = type.GetProperty("Parser", BindingFlags.Public | BindingFlags.Static);
        object parserInstance = pInfo.GetValue(null);//获取Parser对象,  因为 Parser对象 是静态属性，传入 null

        var methods = parserInstance.GetType().GetMethods();
        var parseFromMethod = methods.Where(m => m.Name == "ParseFrom");
        var parseFromByteArrMethod = parseFromMethod.Single(m => m.GetParameters().Length == 1 && m.GetParameters().First().ParameterType == typeof(byte[]) && m.ReturnType.Name == type.Name);
        T result = (T)parseFromByteArrMethod.Invoke(parserInstance, new object[] { data }); ;

        if (Enum.TryParse(type.Name, out ProtobufMsgID msgId))
        {
            ProtobufMessage.PrintMessage(msgId, result);
        }
        return result;
    }


    public void Reconnect()
    {
        switch (NetConfig.clientSocketType)
        {
            case ClientSocketType.TCPIP:
#if !UNITY_WEBGL
                TcpIpSocketManager.Instance.Close();
                TcpIpSocketManager.Instance.Reconnect();
#endif
                break;
            case ClientSocketType.WEBSOCKET:
                WebSocketManager.Instance.Close();
                WebSocketManager.Instance.Reconnect();
                break;
            default:
                Debug.LogError("not implementation yet!");
                break;
        }
    }

    public void Close()
    {
        Debug.Log("socket close");
        switch (NetConfig.clientSocketType)
        {
            case ClientSocketType.TCPIP:
#if !UNITY_WEBGL
                TcpIpSocketManager.Instance.Close();
#endif
                break;
            case ClientSocketType.WEBSOCKET:
                WebSocketManager.Instance.Close();
                break;
        }
    }

    public void ForbidReconnet()
    {
        switch (NetConfig.clientSocketType)
        {
            case ClientSocketType.TCPIP:
#if !UNITY_WEBGL
                TcpIpSocketManager.Instance.ForbidReconnet();
#endif
                break;
            case ClientSocketType.WEBSOCKET:
                WebSocketManager.Instance.ForbidReconnet();
                break;

        }
    }
}

#endif


