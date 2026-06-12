using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowMsgID : WindowMsgBaseID
{
    //前100号为系统框架使用的ID
    public const int OnSceneLoadSuccess = 101;

    public const int OnAddHudMsg = 200;
    public const int OnRemoveHudMsg = 201;

    public const int OnLoginS2CSuccess = 300;
}
