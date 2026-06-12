using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetService
{

    public NetService()
    {
        this.RegisterRcvMsg();
    }

    public virtual void Init()
    {
    }
    public virtual void RegisterRcvMsg()
    {
    }

    public virtual void UnregisterRcvMsg()
    {
    }
    public virtual void Dispose()
    {
        this.UnregisterRcvMsg();
    }
}
