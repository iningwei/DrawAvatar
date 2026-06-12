using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using ZGame;
using ZGame.Event;
using ZGame.Obfuscation;

public class PlayerAvatarManager : Singleton<PlayerAvatarManager>
{
    public bool isLocalUserInfoFileEncrypt = false;
    public string robotUidPrefix = "robot_";//机器人uid前缀

    string userInfoFoldPath;
    string localUserInfoFilePath;
    string localUserHeadFilePath;
    public string localrobotHeadFilePath;
    static string KEY = "Qy+_)(12";//图片缓存说明文件加密key
    string avatarKey = "whosyourdaddy";
    /// <summary>
    /// key为用户Uid，value为用户信息
    /// </summary>
    Dictionary<string, PlayerAvatarData> PlayerAvatarDatas = new Dictionary<string, PlayerAvatarData>();

    public PlayerAvatarManager()
    {
        init();
    }
    void init()
    {
        string gameChannelId = ConfigUtility.Data.GameChannelId.ToString();
        userInfoFoldPath = Application.persistentDataPath + "/" + gameChannelId + "/UserInfo";
        localUserInfoFilePath = userInfoFoldPath + "/LocalUserInfo.json";
        localUserHeadFilePath = userInfoFoldPath + "/Head";
        localrobotHeadFilePath = Application.streamingAssetsPath + "/robot_avatar";
        IOTools.CreateDirectorySafe(userInfoFoldPath);
        IOTools.CreateFileSafe(localUserInfoFilePath);
        IOTools.CreateDirectorySafe(localUserHeadFilePath);

        byte[] bData = File.ReadAllBytes(localUserInfoFilePath);

        if (bData.Length > 0 && isLocalUserInfoFileEncrypt)
        {
            bData = DES.DecryptByte(bData, KEY);
        }


        //从本地读取缓存信息
        string contentStr = Encoding.Default.GetString(bData);
        StringReader reader = new StringReader(contentStr);
        while (true)
        {
            string line = reader.ReadLine();
            if (!string.IsNullOrEmpty(line))
            {
                var item = JsonUtility.FromJson<PlayerAvatarData>(line);
                if (item.UserId == null || item.UserId == "") continue;
                if (string.IsNullOrEmpty(item.LocalPath))
                {
                    continue;//过滤掉localPath无数据的
                }
                PlayerAvatarDatas.Add(item.UserId, item);
            }
            else
            {
                break;
            }
        }


        EventDispatcher.Instance.AddListener(EventID.OnSaveAvatar, this.onSaveAvatar);
    }

    private void onSaveAvatar(string evtId, object[] paras)
    {
        this.SaveToFile();
    }

    public void AddPlayerAvatarData(string uid, string name, string headImgUrlOrPath)
    {
        if (PlayerAvatarDatas.ContainsKey(uid))
        {
            var data = this.PlayerAvatarDatas[uid];
            if (data.IsRobot == false && data.ServerURL != headImgUrlOrPath)
            {
                data.ServerURL = headImgUrlOrPath;
                data.LocalPath = "";//清空LocalPath
            }
        }
        else
        {
            bool isRobot = false;
            if (uid.StartsWith(robotUidPrefix))
            {
                isRobot = true;
            }

            PlayerAvatarData data = null;
            if (isRobot)
            {
                if (headImgUrlOrPath.StartsWith("http"))
                {
                    data = new PlayerAvatarData(uid, name, isRobot, headImgUrlOrPath, "");
                }
                else
                {
                    data = new PlayerAvatarData(uid, name, isRobot, "", headImgUrlOrPath);
                }
            }
            else
            {
                data = new PlayerAvatarData(uid, name, isRobot, headImgUrlOrPath, "");
            }
            //Debug.LogError("add player avatar data:" + uid + ", nick:" + name + ", isRobot:" + isRobot + ", url or path:" + headImgUrlOrPath);
            this.PlayerAvatarDatas.Add(uid, data);
        }
    }


    //TODO,需要做个玩家离线标识，玩家离线后清除其tex引用，防止主播长时直播导致的内存问题
    //或者根据tex对应rawimg是否为null决定销毁与否
    Dictionary<string, Texture2D> cachedPlayerImageDic = new Dictionary<string, Texture2D>();

    public Texture2D GetHeadImgByUserID(string uid, int width = 128, int height = 128)
    {
        if (!PlayerAvatarDatas.ContainsKey(uid))
        {
            #region UNITY_EDITOR
            Debug.Log("GetHeadImgByUserID error, no avatar data with uid:" + uid);
            #endregion
            return null;
        }
        if (cachedPlayerImageDic.ContainsKey(uid))
        {
            return cachedPlayerImageDic[uid];
        }



        string localPath = this.PlayerAvatarDatas[uid].LocalPath;
        if (!string.IsNullOrEmpty(localPath))
        {
            return loadLocalTexture(uid, localPath, width, height);
        }
        string serverUrl = this.PlayerAvatarDatas[uid].ServerURL;
        if (!string.IsNullOrEmpty(serverUrl))
        {
            this.loadServerTexture(uid, serverUrl, width, height);
            return null;
        }

        //////Debug.LogError($"GetHeadImgByUserID error occur,please check:localPath:{localPath}, serverUrl:{serverUrl}" + "，uid:" + uid);
        return null;
    }
    Texture2D loadLocalTexture(string uid, string path, int width, int height)
    {
        //先处理path名
        //path是完整路径，可能每个玩家的本地路径不一致。但是StreamingAssets后的是一致的 
        if (int.TryParse(path, out int pathInt))//path只含有图片数字名字的
        {
            path = Application.streamingAssetsPath + "/robot_avatar/" + path + ".png";
        }
        else if (path.Contains("StreamingAssets/robot_avatar/"))
        {
            path = Application.streamingAssetsPath + "/" + path.Substring(path.IndexOf("robot_avatar/"));
        }
        else
        {
            //Debug.LogError("加载的是本地缓存的server下载下来的图");
        }

        if (File.Exists(path))
        {
            long lSize = new FileInfo(path).Length;
            Texture2D tex = new Texture2D(width, height);
            var b = File.ReadAllBytes(path);
            b = XOR.DecryptBytes(b, avatarKey);
            tex.LoadImage(b);
            cachedPlayerImageDic.Add(uid, tex);
            return tex;
        }
        else
        {
            //Debug.LogError("error, no texture path:" + path);
            return null;
        }
    }

    private void loadServerTexture(string uid, string url, int width, int height)
    {
        var avatarData = PlayerAvatarDatas[uid];
        string fileName = MD5.Get(Encoding.UTF8.GetBytes(uid + avatarData.Name + url));
        Debug.Log("try load " + uid + "'s tex, url:" + url + ", time stamp(s):" + TimeTool.GetNowSecondStamp());
        UnityWebRequestMgr.Instance.GetTexture(avatarData.ServerURL, (v) =>
        {
            if (v != null)
            {
                string localPath = localUserHeadFilePath + "/" + fileName;
                if (!File.Exists(localPath))
                {
                    var bytes = v.EncodeToPNG();
                    bytes = XOR.EncryptBytes(bytes, avatarKey);
                    var file = File.Open(localPath, FileMode.Create);
                    var binary = new BinaryWriter(file);
                    binary.Write(bytes);
                    file.Close();


                    cachedPlayerImageDic.Add(uid, v);
                    avatarData.LocalPath = localPath;
                    Debug.Log($"load tex success, uid:{uid},url:{url},localPath:{localPath}");
                }
                else
                {
                    //Debug.Log("already exist same!");
                    //遇到了本地头像数据存储了，但是LocalUserInfo中LocalPath为空；走到了这里。最终导致玩家头像为空。
                    //所以在这里强制把localPath置过来！！！
                    avatarData.LocalPath = localPath;
                }
            }
            else
            {
                Debug.Log("load tex is null, uid:" + uid + ",url:" + url);
            }
        });
    }

    private void SaveToFile()
    {
        StringBuilder stringBuilder = new StringBuilder();
        string jsonStr = null;
        foreach (var item in PlayerAvatarDatas)
        {
            if (item.Value.IsRobot)
            {
                continue;
            }

            jsonStr = JsonUtility.ToJson(item.Value);
            stringBuilder.AppendLine(jsonStr);//每一行都是JSON格式，整体文件不是JSON格式
        }
        string content = stringBuilder.ToString();
        if (!string.IsNullOrEmpty(content))
        {
            byte[] b = Encoding.Default.GetBytes(content);
            if (isLocalUserInfoFileEncrypt)
            {
                b = DES.EncryptByte(b, KEY);
            }
            File.WriteAllBytes(localUserInfoFilePath, b);
        }
    }
}
