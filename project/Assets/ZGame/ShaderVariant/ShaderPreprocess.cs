//打ab的时候会唤起OnProcessShader

//本来想测试一下shader处理的相关回调，看看打AB时的相关日志
//MD，这部分代码无法测试，在Unity2021.3.3f1 android中，当打ab包的时候报错：Script updater for Library\Bee\artifacts\1300b0aP.dag\Assembly-CSharp.dll failed to produce updates.txt file
//只有等后续该bug解决了再说吧
//20220630
//相关类似报错文档：https://forum.unity.com/threads/error-when-changing-scripts.1023646/

//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor.Build;
//using UnityEditor.Rendering;
//using UnityEngine;
//using UnityEngine.Rendering;

//public class ShaderPreprocess : IPreprocessShaders
//{
//    static ShaderKeyword[] uselessKeywords;

//    public int callbackOrder
//    {
//        get { return 0; }
//    }

//    static ShaderPreprocess()
//    {
//        Debug.Log("call ShaderPreprocess()");
//        uselessKeywords = new ShaderKeyword[] {
//            //new ShaderKeyword( "DIRLIGHTMAP_COMBINED" ),
//            //new ShaderKeyword( "LIGHTMAP_SHADOW_MIXING" ),
//            //new ShaderKeyword( "SHADOWS_SCREEN" ),
//        };
//    }

//    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
//    {
//        Debug.Log("----------------------------- >OnProcessShader, shader:" + shader.name + ", compilerDataCount:" + data.Count);
//        for (int i = 0; i < data.Count; i++)
//        {
//            var keywords = data[i].shaderKeywordSet.GetShaderKeywords();
//            for (int j = 0; j < keywords.Length; j++)
//            {
//                var name = keywords[i].name;
//                Debug.Log($"i:{i},j:{j},keyword name: {name}");
//            }
//        }

//        Debug.Log("< -----------------------------OnProcessFinish");
//    }


//}
