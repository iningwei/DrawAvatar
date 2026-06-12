using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetConfig
{
    public static ClientSocketType clientSocketType = ClientSocketType.WEBSOCKET;
    public static bool useNetworkOrder = true;//是否使用网络字节序
    public static bool handleBlockMsgAfterUnConnected = true;
}
