using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Window
{
    //忽略名字收集标签
    [AttributeUsage(AttributeTargets.Class,Inherited =false)]
    public class IgnoreWindowNameGatherAttribute : System.Attribute
    {

    }
}