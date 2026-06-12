using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor.Callbacks;
using UnityEngine;
using ZGame.ZTable;

//see AutoGenServiceFetch.cs

public class AutoGenTableFetch
{
    [DidReloadScripts]
    static void genTableFetch()
    {
        Debug.Log("Gen TableFetch");
        genPropertyAutoTableFetch();
    }

    private static void genPropertyAutoTableFetch()
    {
        string str = "using ZGame.ZTable;\r\n";
        str += "public class TableFetch{\r\n";

        List<string> classDes = new List<string>();


        string path = "./Library/ScriptAssemblies/Assembly-CSharp.dll";
        byte[] buffer = File.ReadAllBytes(path);
        var assembly = Assembly.Load(buffer);
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsClass && t.BaseType != null && t.BaseType.Name == typeof(TableReader).Name && t.Name.EndsWith("Reader"))
            {
                classDes.Add(t.Name);
            }
        }

        //LoadAllTables()
        str += "\tpublic static void LoadAllTables(){\r\n";
        for (int i = 0; i < classDes.Count; i++)
        {
            str += "\t\t" + classDes[i] + ".Instance.CallEmpty();\r\n";
        }
        str += "\t}\r\n";

        //IsAllTablesLoaded();
        str += "\tpublic static bool IsAllTablesLoaded(){\r\n";
        string isLoadedStr = "\t\treturn ";
        if (classDes.Count == 0)
        {
            isLoadedStr += "true;\r\n";
        }
        else
        {
            isLoadedStr += classDes[0] + ".Instance.IsLoaded()";
            for (int i = 1; i < classDes.Count; i++)
            {
                isLoadedStr += "&&\r\n\t\t\t\t " + classDes[i] + ".Instance.IsLoaded()";
            }
            isLoadedStr += ";\r\n";
        }
        str += isLoadedStr;
        str += "\t}\r\n";

        str += "}";
        string writePath = Application.dataPath + "/Scripts/Table/TableFetch.cs";
        string d = writePath.Substring(0, writePath.LastIndexOf('/'));
        if (!Directory.Exists(d))
        {
            Directory.CreateDirectory(d);
        }


        if (!File.Exists(writePath))
        {
            File.Create(writePath).Dispose();
        }
        File.WriteAllText(writePath, str);
    }
}
