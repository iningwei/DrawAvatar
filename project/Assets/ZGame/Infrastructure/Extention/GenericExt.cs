using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame;

public class GenericExt
{
    public static List<T> ParseList<T>(object listObj)
    {

        if (!listObj.GetType().IsGenericType)
            throw new Exception("error, parameter is not generic type");

        List<T> result = new List<T>();

        if (listObj as System.Collections.ICollection != null)
        {
            var list = (System.Collections.ICollection)listObj;
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (typeof(T).Name == item.GetType().Name)
                    {
                        result.Add((T)item);
                    }
                    else
                    {
                        Debug.LogError("error,T type and List elements type is not the same");
                        break;
                    }
                }
            }
        }
        return result;
    }

   
}
