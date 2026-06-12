using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor.Callbacks;
using UnityEngine;

public class AutoGenServiceFetch
{
    [DidReloadScripts]
    static void genServiceFetch()
    {
        Debug.Log("Gen ServiceFetch");
        genPropertyAutoServices();

    }


    //生成通过 RuntimeInitializeOnLoadMethod， 自动实例化各种service的ServiceFetch.cs
    //由于我是通过hybrid clr对assembly dll热更，因此会导致 RuntimeInitializeOnLoadMethod 不生效（官方使用指南->不支持的特性 一栏中有提到）
    //格式如下
    /*
    using UnityEngine;

    public class ServiceFetch
    {
        public static DyService dyService;
        public static LoginService loginService;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnBeforeSceneLoadRuntimeMethod()
        {
            dyService = new DyService();
            loginService = new LoginService();
        }
    }
    */
    static void genRuntimeInitializeOnLoadAutoServices()
    {
        string str = "using UnityEngine;\n\n";
        str += "public class ServiceFetch{\n";

        List<string> classDes = new List<string>();
        List<string> classCtor = new List<string>();

        string path = "./Library/ScriptAssemblies/Assembly-CSharp.dll";
        byte[] buffer = File.ReadAllBytes(path);
        var assembly = Assembly.Load(buffer);
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsClass && t.BaseType != null && t.BaseType.Name == typeof(NetService).Name && t.Name.EndsWith("Service"))
            {
                classDes.Add("public static " + t.Name + " " + t.Name.FirstCharToLower() + ";");
                classCtor.Add(t.Name.FirstCharToLower() + " = new " + t.Name + "();");
            }
        }

        for (int i = 0; i < classDes.Count; i++)
        {
            str += "\t" + classDes[i] + "\n";
        }
        str += "\n";

        str += "\t[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]\n";
        str += "\tpublic static void OnBeforeSceneLoadRuntimeMethod(){\n";
        for (int i = 0; i < classCtor.Count; i++)
        {
            str += "\t\t" + classCtor[i] + "\n";
        }
        str += "\t}\n";
        str += "}";



        string writePath = Application.dataPath + "/Scripts/Net/Service/ServiceFetch.cs";
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

    static void genPropertyAutoServices()
    {
        string str = "using UnityEngine;\n\n";
        str += "public class ServiceFetch{\n";

        List<string> classDes = new List<string>();
        List<string> classProperty = new List<string>();
        List<string> initFunc = new List<string>();
        List<string> disposeFunc = new List<string>();

        string path = "./Library/ScriptAssemblies/Assembly-CSharp.dll";
        byte[] buffer = File.ReadAllBytes(path);
        var assembly = Assembly.Load(buffer);
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsClass && t.BaseType != null && t.BaseType.Name == typeof(NetService).Name && t.Name.EndsWith("Service"))
            {
                classDes.Add("\tprivate static " + t.Name + " _" + t.Name.FirstCharToLower() + ";");
                initFunc.Add("\t\t" + t.Name.FirstCharToLower() + ".Init();");
                disposeFunc.Add("\t\t" + t.Name.FirstCharToLower() + ".Dispose();");
                //属性拼接
                string propertyStr = "\tpublic static " + t.Name + " " + t.Name.FirstCharToLower() + "\n";
                propertyStr += "\t{\n";
                propertyStr += "\t\tget\n";
                propertyStr += "\t\t{\n";
                propertyStr += "\t\t\tif (_" + t.Name.FirstCharToLower() + " == null)\n";
                propertyStr += "\t\t\t{\n";
                propertyStr += "\t\t\t\t_" + t.Name.FirstCharToLower() + " = new " + t.Name + "();\n";
                propertyStr += "\t\t\t}\n";
                propertyStr += "\t\t\treturn _" + t.Name.FirstCharToLower() + ";\n";
                propertyStr += "\t\t}\n";
                propertyStr += "\t}\n";
                classProperty.Add(propertyStr);
            }
        }

        for (int i = 0; i < classDes.Count; i++)
        {
            str += classDes[i] + "\n";
        }
        str += "\n";

        str += "\tpublic static void Init()\n" + "\t{\n";
        for (int i = 0; i < initFunc.Count; i++)
        {
            str += initFunc[i] + "\n";
        }
        str += "\t}\n\n";

        str += "\tpublic static void Dispose()\n" + "\t{\n";
        for (int i = 0; i < disposeFunc.Count; i++)
        {
            str += disposeFunc[i] + "\n";
        }
        str += "\t}\n\n";



        for (int i = 0; i < classProperty.Count; i++)
        {
            str += classProperty[i] + "\n";
        }

        str += "}";


        string writePath = Application.dataPath + "/Scripts/Net/Service/ServiceFetch.cs";
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
