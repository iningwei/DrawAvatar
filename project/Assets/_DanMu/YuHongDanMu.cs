using BestHTTP.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;
using ZGame.DanMu;
using ZGame;
using Google.Protobuf.WellKnownTypes;
using ZGame.ZTable;
using System.Linq;

//////public class YuHongDanMu : MonoBehaviour
//////{
//////    public RoomLoginWindow roomLoginWindow;
//////    public string userID;//主播ID

//////    private string url = "http://127.0.0.1:3000";

//////    private WebSocket webSocket;

//////    private bool isConnected = false; // 是否已连接

//////    private void Start()
//////    {

//////        DontDestroyOnLoad(this);
      


//////#if  UNITY_EDITOR
//////        roomLoginWindow.Init(userID);
//////#else
//////        var paramArgs = System.Environment.GetCommandLineArgs();
//////        DebugExt.LogE("comandLineArgs count:" + paramArgs.Length);
//////        for (int i = 0; i < paramArgs.Length; i++)
//////        {
//////            Debug.Log("paramArg:" + i + ":" + paramArgs[i]);
//////        }
//////        if (paramArgs[0].EndsWith(".exe")) //正常启动第一个参数都是可执行程序名
//////        {
//////            if (paramArgs.Length >= 2)
//////            {
//////                userID = paramArgs[1];
//////                DebugExt.Log("YuHongDanMu userID:" + userID);
//////            }
//////            else
//////            {
//////                DebugExt.LogE("YuHongDanMu error, can not get userID");
//////            }
//////        }
//////        else //羽鸿平台的启动，第一个不是可执行程序名，而是userId
//////        {
//////            if (paramArgs.Length >= 1)
//////            {
//////                userID = paramArgs[0];
//////                DebugExt.Log("2222 YuHongDanMu userID:" + userID);
//////            }
//////            else
//////            {
//////                DebugExt.LogE("YuHongDanMu error, can not get userID");
//////            }
//////        }


//////        Init();
//////        Connect();
//////#endif

//////    }


//////    //tk模式下,礼物很多，需要按照礼物价值从高到低拆分匹配。
//////    //然后再AddDanMuData 
//////    //礼物ID:单价
//////    //5655:1  //虾哥一开始给的是5269，改成 5655，因为5655是他们盒子测试工具默认礼物
//////    //5282:10
//////    //5585:100
//////    //6007:299
//////    //12463:799
//////    //6414:1500
//////    Dictionary<int, int> giftPriceDic = new Dictionary<int, int>();

//////    public Dictionary<int, int> SplitGiftByMoney(int money)
//////    {
//////        if (money <= 0)
//////        {
//////            return new Dictionary<int, int>(); // 无效金额返回空
//////        }

//////        // 假设 giftPriceDic 已经是你的类成员 Dictionary<int, int>（id -> price）
//////        // 如果不是成员，可以把 Dictionary<int, int> giftPriceDic 作为参数传进来

//////        // 1. 按 price 从大到小排序
//////        var sortedGifts = giftPriceDic
//////            .OrderByDescending(kv => kv.Value)
//////            .ToList();

//////        Dictionary<int, int> result = new Dictionary<int, int>();
//////        int remaining = money;

//////        // 2. 贪心从最大价格开始拆
//////        foreach (var gift in sortedGifts)
//////        {
//////            if (remaining <= 0) break;

//////            int price = gift.Value;
//////            if (price <= 0) continue; // 防止异常（实际你的数据都是正数）

//////            int count = remaining / price; // 整数除法，直接取最大能买的数量
//////            if (count > 0)
//////            {
//////                result[gift.Key] = count;           // 记录这个 id 要买多少个
//////                remaining -= count * price;         // 扣除金额
//////            }
//////        }


//////        return result;
//////    }


//////    private void Init()
//////    {
//////        giftPriceDic.Add(5655, 1);
//////        giftPriceDic.Add(5282, 10);
//////        giftPriceDic.Add(5585, 100);
//////        giftPriceDic.Add(6007, 299);
//////        giftPriceDic.Add(12463, 799);
//////        giftPriceDic.Add(6414, 1500);


//////        webSocket = new WebSocket(new Uri(url));
//////        webSocket.OnOpen += OnOpen;
//////        webSocket.OnMessage += OnMessageReceived;
//////        webSocket.OnError += OnError;
//////        webSocket.OnClosed += OnClosed;
//////    }
//////    public void Connect()
//////    {
//////        if (!isConnected)
//////            webSocket.Open();
//////    }
//////    void OnOpen(WebSocket ws)
//////    {
//////        isConnected = true;
//////        DebugExt.Log("YuHongDanMu WebSocket 连接成功");
//////        //羽鸿那边返回来的已经不是userID了，而是一个json格式数据如下：，需要再处理一遍
//////        //[{"platform":"3","tv_anchor":"usj6pfcc926"}]


//////        var list = MiniJSON.Json.Deserialize(userID) as List<object>;
//////        var dic = (Dictionary<string, object>)list[0];
//////        string finalUserId = (string)dic["tv_anchor"];
//////        Debug.LogError("--->处理羽鸿传过来的userID, 原始userID:" + userID + ", finalUserId:" + finalUserId);


//////        roomLoginWindow.Init(finalUserId);
//////    }
//////    void OnMessageReceived(WebSocket ws, string msg)
//////    {
//////        DebugExt.Log("YuHongDanMu, recv msg:" + msg);

//////        long stamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
//////        //解析json数据
//////        var dic = Json.Deserialize(msg) as Dictionary<string, object>;

//////        long msgType;
//////        try
//////        {
//////            msgType = (long)dic["type"];
//////        }
//////        catch (Exception ex)
//////        {
//////            return;
//////        }

//////        if (msgType == 2)//发言
//////        {
//////            string id = (string)dic["uid"];
//////            string nick = (string)dic["name"];
//////            string avatarUrl = (string)dic["url"];
//////            string content = (string)dic["msg"];
//////            DanMuInput.AddDanMuData(new DanMuChatData(id, nick, avatarUrl, stamp, content));

//////            //发言信息上传给服务端
//////            DyService.SendChatDataToServer(id, content, avatarUrl, nick, stamp);
//////        }
//////        else if (msgType == 3)//点赞
//////        {
//////            string id = (string)dic["uid"];
//////            string nick = (string)dic["name"];
//////            string avatarUrl = (string)dic["url"];
//////            DanMuInput.AddDanMuData(new DanMuLikeData(id, nick, avatarUrl, stamp, 1));
//////        }
//////        else if (msgType == 4)//礼物
//////        {
//////            string id = (string)dic["uid"];
//////            string nick = (string)dic["name"];
//////            string avatarUrl = (string)dic["url"];
//////            string giftId = ((int)(long)dic["giftid"]).ToString();//tk的礼物id都是数字
//////            int giftNum = (int)(long)dic["num"];//礼物数量
//////            int giftValue = (int)(long)dic["price"];//price是总价
//////            string giftName = (string)dic["gift"];//礼物名称

//////            //礼物信息上传给服务端
//////            DyService.SendGiftDataToServer(id, giftId, giftName, (UInt32)giftNum, (UInt32)giftValue, avatarUrl, nick, stamp);


//////            bool isNeedSplit = true;
//////            int singleValue = giftValue / giftNum;
//////            if (singleValue == 1)//单价为1的不需要拆分
//////            {
//////                isNeedSplit = false;
//////            }

//////            Debug.Log("singleValue:" + singleValue + ", isNeedSplit:" + isNeedSplit);

//////            if (isNeedSplit)
//////            {
//////                var splitResult = SplitGiftByMoney(giftValue);
//////                if (splitResult != null && splitResult.Count > 0)
//////                {
//////                    foreach (var item in splitResult)
//////                    {
//////                        string tmpGiftId = item.Key.ToString();
//////                        int tmpGiftNum = item.Value;
//////                        int tmpTotalGiftValue = tmpGiftNum * giftPriceDic[item.Key];
//////                        Debug.Log($"礼物giftId:{giftId},num:{giftNum},totalPrice:{giftValue}; 被拆分为：id->{tmpGiftId},num->{tmpGiftNum},totalPrice:{tmpTotalGiftValue}");

//////                        DanMuInput.AddDanMuData(new DanMuGiftData(id, nick, avatarUrl, stamp, tmpGiftId, tmpGiftNum, tmpTotalGiftValue));
//////                    }
//////                }

//////            }
//////            else
//////            {
//////                DanMuInput.AddDanMuData(new DanMuGiftData(id, nick, avatarUrl, stamp, giftId, giftNum, giftValue));
//////            }

//////        }
//////    }
//////    void OnClosed(WebSocket ws, UInt16 code, string message)
//////    {
//////        DebugExt.Log("YuHongDanMu WebSocket 连接断开1:" + message);
//////        webSocket.Close();
//////        isConnected = false;
//////    }
//////    private void OnDestroy()
//////    {
//////        if (webSocket != null && webSocket.IsOpen)
//////        {
//////            DebugExt.Log("YuHongDanMu WebSocket 连接断开2");
//////            webSocket.Close();
//////            isConnected = false;
//////        }
//////    }
//////    void OnError(WebSocket ws, Exception ex)
//////    {
//////        DebugExt.Log("YuHongDanMu WebSocket出错:" + ex.Message);
//////        webSocket.Close();
//////        isConnected = false;
//////    }

//////    //发送指令，wss返回 消息：0 主播信息
//////    public void SendGetZBMsg1()
//////    {
//////        string jsonData = "{\"Action\":\"GetWebcastInfo\",\"Platform\":\"Tiktok\"}";
//////        // 发送 JSON 数据
//////        webSocket.Send(jsonData);
//////    }
//////}
