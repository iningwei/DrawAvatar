using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABBuildLogData
{
    /// <summary>
    /// 资源路径，Assets/...
    /// </summary>
    public string originResPath;

    /// <summary>
    /// 资源md5值
    /// </summary>
    public string md5Value;

    public string timeDes;
    public ABBuildLogData(string resPath, string md5)
    {
        this.originResPath = resPath;
        this.md5Value = md5;

        this.timeDes = TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now, true);
    }
}
