using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame;
using ZGame.Obfuscation;

public class ABNameQuickSearch : EditorWindow
{
    string originName = "";
    string encryptName = "";
    string showOrigin = "";
    string showEncrypt = "";


    [MenuItem("HotUpdate/AB名称快查")]
    static void Init()
    {
        ABNameQuickSearch window = EditorWindow.GetWindow(typeof(ABNameQuickSearch)) as ABNameQuickSearch;
    }
    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("输入原始名字，获得加密后名字");
        GUILayout.BeginHorizontal();
        originName = EditorGUILayout.TextField("", originName);
        if (GUILayout.Button("转换"))
        {
            if (!string.IsNullOrEmpty(originName.Trim()))
            {
                string r = DES.EncryptStrToHex(originName.Trim(), ConfigUtility.Data.ABResNameCryptoKey);
                Debug.LogError(originName.Trim() + " 加密后名字--->" + r);
                showOrigin = " 加密后名字--->" + r;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Label(showOrigin);

        GUILayout.Space(30);
        GUILayout.Label("输入加密后的名字，获得原始名字");
        GUILayout.BeginHorizontal();
        encryptName = EditorGUILayout.TextField("", encryptName);
        if (GUILayout.Button("转换"))
        {
            if (!string.IsNullOrEmpty(encryptName.Trim()))
            {
                string r = DES.DecryptStrFromHex(encryptName.Trim(), ConfigUtility.Data.ABResNameCryptoKey);
                Debug.LogError(encryptName.Trim() + " 原始名字--->" + r);
                showEncrypt = " 原始名字--->" + r;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Label(showEncrypt);

    }
}
