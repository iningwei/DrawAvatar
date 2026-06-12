using System;
using System.Collections.Generic;
using UnityEngine;


public enum PaymentChannelId
{
    Google = 1,
    AppleStore = 2
}

public enum GameChannelId
{
    DouYin = 100,
    KuaiShou = 200
}
public enum ScreenOrientation
{
    Landscape = 0,
    Portrait = 1,
}

public enum ResLoadType
{
    AssetBundle = 0,
    Resources = 1,
    Addressable = 2
}

public class LoginType
{
    public const string PUB = "PUB";
    public const string PUBTEST = "PUBTEST";
    public const string TEST = "TEST";
    public const string DEV = "DEV";
    public const string DEV_GW = "DEV_GW";
}

public class PackType
{
    public const string PUB = "PUB";
    public const string TEST = "TEST";
    public const string DEV = "DEV";
}


[Serializable]
public class Config
{
    public string ProductName;
    public string AppVersion;//和Project Settings中的Version一致
    public string AppBundleVersion;//和Project Setting中的Bundle Version Code / Version对应  格式如：1.0.5
    public string ResVersion;
    public string PackTimeStamp;
    public int GameChannelId;
    public int PaymentChannelId;
    public bool IsABResNameCrypto;//AB资源名是否加密了
    public string ABResNameCryptoKey;
    public int ABResByteOffset;//AB资源二进制偏移位数（二进制前增加的位数）
    public string GameDesignRatio;//游戏设计分辨率,格式：1080,1920
    public int ScreenOrientation;
    public int GameInputType;//1为鼠标键盘、2为触屏
    public int ResLoadType;
    public string FirstOpenWindowName;

    public List<ChannelInfo> ChannelInfo;

    public string LoginType;
    public string PackType;//热更资源CDN地址类型，诸如：PUB;DEV;TEST

    public bool IsRealPurchase;//是否走真实购买流程
    public bool IsShowProtoMsgLog;//是否显示消息协议日志
    public bool IsShowDebugBtn;//是否显示Debug上传按钮
    public bool IsEnableLogTrace;//是否开启日志追踪，开启后，点击debug上传按钮，才会有上报的日志   ,（目前开启后，在ios上会遇到崩溃     ）
    public bool IsEnableLogRealtimeWriteToLocal;//IsEnableLogTrace为true的情况下，开启实时写入的话，会把日志实时写入本地
    public bool IsEnableLogUpdate2Server;
    public bool IsShowReporter;
}

[Serializable]
public class ChannelInfo
{
    public int ChannelID;
    public List<LoginItem> LoginData;
    public List<PackItem> PackData;
}

[Serializable]
public class LoginItem
{
    public string LoginType;
    public string PostURL;
}

[Serializable]
public class PackItem
{
    public string PackType;
    public string FtpTxtFileURL;
    public string FtpZipFileURL;
}

//////public class SubGameConfig
//////{
//////    public string name;
//////    public string abName;
//////    public long size;
//////    public string md5;
//////    public bool install;
//////} 