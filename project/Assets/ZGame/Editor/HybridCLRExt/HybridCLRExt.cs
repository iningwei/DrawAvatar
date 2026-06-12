using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace HybridCLR.Editor.Commands
{
#if HybridCLR_INSTALLED
    public static class HybridCLRExt
    {
        /// <summary>
        /// 按照必要的顺序，执行所有生成操作，适合打包前操作
        /// </summary>
        [MenuItem("HybridCLR/Generate/All_WebGL_Step2-ExceptLinkXML-先确保LinkXML内已移除Socket相关引用", priority = 202)]
        public static void GenerateAllExceptLinkXML()
        {
            var installer = new Installer.InstallerController();
            if (!installer.HasInstalledHybridCLR())
            {
                throw new BuildFailedException($"You have not initialized HybridCLR, please install it via menu 'HybridCLR/Installer'");
            }
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            // 生成裁剪后的aot dll
            StripAOTDllCommand.GenerateStripedAOTDlls(target);

            // 桥接函数生成依赖于AOT dll，必须保证已经build过，生成AOT dll
            MethodBridgeGeneratorCommand.GenerateMethodBridgeAndReversePInvokeWrapper(target);
            AOTReferenceGeneratorCommand.GenerateAOTGenericReference(target);
        }


        [MenuItem("HybridCLR/Generate/All_WebGL_Step1-GenLinkXML", priority = 201)]
        public static void GenerateLinkXML()
        {
            var installer = new Installer.InstallerController();
            if (!installer.HasInstalledHybridCLR())
            {
                throw new BuildFailedException($"You have not initialized HybridCLR, please install it via menu 'HybridCLR/Installer'");
            }
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            CompileDllCommand.CompileDll(target);
            Il2CppDefGeneratorCommand.GenerateIl2CppDef();

            //  这几个生成依赖HotUpdateDlls
            LinkGeneratorCommand.GenerateLinkXml(target);

            //删除Socket相关的引用
            var ls = SettingsUtil.HybridCLRSettings;
            string path = $"{Application.dataPath}/{ls.outputLinkFile}";
            Debug.Log("path:" + path);

            string[] allLines = File.ReadAllLines(path);
            for (int i = 0; i < allLines.Length; i++)
            {
                if (allLines[i].Contains("System.Net.Sockets."))
                {
                    allLines[i] = "";
                }
            }
            File.WriteAllLines(path, allLines);
            Debug.Log("modify finished");
        }
    }
#endif
}
