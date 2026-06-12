using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerAvatarData
{
    public string UserId;
    public string Name;
    public bool IsRobot; 
    public string ServerURL;
    public string LocalPath;

    public PlayerAvatarData(string userId, string name, bool isRobot, string serverUrl,string localPath)
    {
        UserId = userId;
        Name = name;
        IsRobot = isRobot;
        ServerURL = serverUrl;
        LocalPath = localPath;
    }
}