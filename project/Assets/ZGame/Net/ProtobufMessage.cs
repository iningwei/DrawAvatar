

#if NET_DATA_PB

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ZGame;
using Google.Protobuf;

[AttributeUsage(AttributeTargets.Field)]
public class ProtobufMsgIDDes : Attribute
{
    public string name { get; private set; }
    public UInt16 msgId { get; private set; }

    public ProtobufMsgIDDes(string name, UInt16 msgId)
    {
        this.name = name;
        this.msgId = msgId;
    }
}

public class ProtobufMsgIDDesUtils
{
    public static ProtobufMsgIDDes GetIDDes(ProtobufMsgID p)
    {
        Type type = p.GetType();
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo field in fields)
        {
            if (field.Name.Equals(p.ToString()))
            {
                object[] objs = field.GetCustomAttributes(typeof(ProtobufMsgIDDes), true);
                if (objs != null && objs.Length > 0)
                {
                    ProtobufMsgIDDes des = ((ProtobufMsgIDDes)objs[0]);
                    return des;
                }
            }
        }
        Debug.LogError("No Such field: " + p);
        return null;
    }

    public static string GetIDStr(ProtobufMsgID p)
    {
        Type type = p.GetType();
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo field in fields)
        {
            if (field.Name.Equals(p.ToString()))
            {
                object[] objs = field.GetCustomAttributes(typeof(ProtobufMsgIDDes), true);
                if (objs != null && objs.Length > 0)
                {
                    ProtobufMsgIDDes des = ((ProtobufMsgIDDes)objs[0]);
                    return des.msgId.ToString();
                }
            }
        }
        Debug.LogError("No Such field: " + p);
        return null;
    }

    public static string GetNameById(UInt16 msgId)
    {
        Type type = typeof(ProtobufMsgID);
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo field in fields)
        {

            object[] objs = field.GetCustomAttributes(typeof(ProtobufMsgIDDes), true);
            if (objs != null && objs.Length > 0)
            {

                ProtobufMsgIDDes des = ((ProtobufMsgIDDes)objs[0]);
                if (des.msgId == msgId)
                {
                    return des.name;
                }
            }
        }
        Debug.LogError($"No Such field, with msgId:{msgId} ");
        return null;
    }
}


public class ProtobufMessage
{
    static Dictionary<string, Action<byte[]>> eventHandlers = new Dictionary<string, Action<byte[]>>();


    static List<ProtobufMsgID> socketIgnoreLogMsgIds = null;
    public static List<ProtobufMsgID> SocketIgnoreLogMsgIds
    {
        get
        {
            if (socketIgnoreLogMsgIds == null)
            {
                socketIgnoreLogMsgIds = BeanManager.Instance.GetSocketIgnoreLogMsgIds();
            }
            return socketIgnoreLogMsgIds;

        }
    }

    public static void AddListener(ProtobufMsgID msgId, Action<byte[]> handler)
    {
        string idStr = ProtobufMsgIDDesUtils.GetIDStr(msgId);
        addListener(idStr, handler);
    }

    static void addListener(string idStr, Action<byte[]> handler)
    {
        if (eventHandlers.TryGetValue(idStr, out var h))
        {
            if (!h.GetInvocationList().Contains(handler))
            {
                h += handler;
            }
        }
        else
        {
            eventHandlers[idStr] = handler;
        }
    }

    public static void RemoveListener(ProtobufMsgID msgId, Action<byte[]> handler)
    {
        string idStr = ProtobufMsgIDDesUtils.GetIDStr(msgId);
        removeListener(idStr, handler);
    }
    static void removeListener(string idStr, Action<byte[]> handler)
    {
        if (eventHandlers.TryGetValue(idStr, out var h))
        {
            if (h.GetInvocationList().Contains(handler))
            {
                h -= handler;
                eventHandlers[idStr] = h;
            }

            if (h == null || h.GetInvocationList().Length == 0)
            {
                eventHandlers.Remove(idStr);
            }
        }
    }

    public static void HandleMessage(string cmdId, byte[] data)
    {
        string idStr = cmdId;

        if (eventHandlers.TryGetValue(idStr, out var handler))
        {
            if (handler != null)
            {
                handler(data);
            }
        }
    }

    public static void PrintMessage(ProtobufMsgID msgId, IMessage msg)
    {
        //#if !UNITY_EDITOR
        //                if (ConfigUtility.Data.IsShowProtoMsgLog == false&&ConfigUtility.Data.LoginType==LoginType.PUB)
        //                {
        //                    return;
        //                }
        //#endif

        if (SocketIgnoreLogMsgIds.Contains(msgId) && ConfigUtility.Data.LoginType == LoginType.PUB)//外网正式服过滤消息日志
        {
            return;
        }

        var des = ProtobufMsgIDDesUtils.GetIDDes(msgId);
        Debug.Log($"{des.name}:{des.msgId}=>    {msg.ToString()}    ;frame:" + Time.frameCount + ", stamp(ms):" + TimeTool.GetNowStamp() + ";time:" + DateTime.Now.ToString("yyyyMMddHHmmss.ffff"));
    }
}
#endif


