using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame;

public static class ListExt
{
    /// <summary>
    /// 从 List 中随机取出指定数量的不重复元素（不修改原列表）
    /// </summary>
    public static List<T> TakeRandom<T>(this List<T> source, int count)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (count <= 0) return new List<T>();
        if (count >= source.Count) return new List<T>(source);
       
        // 为了不修改原列表，如果数据量稍大就复制一份
        // 200 是经验值，小列表直接操作原列表更快
        var list = source.Count > 200 ? new List<T>(source) : source;

        int n = list.Count;
        int take = Math.Min(count, n);

        // Fisher-Yates 部分洗牌
        for (int i = 0; i < take; i++)
        {
            int j = RandomTool.NextInt(i, n);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        var result = new List<T>(take);
        for (int i = 0; i < take; i++)
        {
            result.Add(list[i]);
        }

        return result;
    }
}
