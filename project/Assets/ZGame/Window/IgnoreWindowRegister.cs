using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Window
{
    //忽略注册标签
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class IgnoreWindowRegisterAttribute : System.Attribute
    {

    }
}