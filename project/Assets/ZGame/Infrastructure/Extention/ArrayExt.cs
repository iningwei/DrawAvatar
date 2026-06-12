using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExt    
{
    public static bool Contain(this long[] array,long target)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i]==target)
            {
                return true;
            }
        }
        return false;
    }
}
