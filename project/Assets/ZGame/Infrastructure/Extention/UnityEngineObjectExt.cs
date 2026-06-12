using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class UnityEngineObjectExt
{
    public static bool IsNull(this UnityEngine.Object target)
    {
        return target == null;
    }
}
