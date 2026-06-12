using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOTMetaAssembly
{
    public static List<string> GetOriginDllNames()
    {
        List<string> list = new List<string>()
        {
             "mscorlib.dll",
            "System.dll",
            "System.Core.dll",//// 如果使用了Linq，需要这个
            //"Unity.VisualScripting.Core.dll",////使用了VisualScripting.Core.LinqUtility的AddRange
        };

        return list;
    }

    public static List<string> GetOriginBytesNames()
    {
        List<string> list = new List<string>()
        {
             "mscorlib.dll.bytes",
            "System.dll.bytes",
            "System.Core.dll.bytes",
            //"Unity.VisualScripting.Core.dll.bytes",
        };

        return list;
    }


    public static List<string> GetCryptedBytesNames()
    {
        List<string> results = new List<string>();
        List<string> dllLists = GetOriginDllNames();
        for (int i = 0; i < dllLists.Count; i++)
        {
            results.Add(HybridCLRDES.EncryptStrToHex(dllLists[i], ConfigUtility.Data.ABResNameCryptoKey) + ".bytes");
        }
        return results;
    }
}
