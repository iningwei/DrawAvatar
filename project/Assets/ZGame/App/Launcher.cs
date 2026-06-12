using UnityEngine;
using UnityEngine.SceneManagement;
using ZGame;
using ZGame.Ress.AB;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering;
using ZGame.Window;
using System.Collections.Generic;
using System.Text;
using ZGame.TimerTween;
using System.Net;
using System.IO;

public class Launcher : MonoBehaviour
{
    private void Start()
    {
        byte[] a = { 0b1010010 };
        byte[] b = { 0x52 };
        byte[] c = { 82 };
        string s1 = Encoding.Default.GetString(a);
        string s2 = Encoding.Default.GetString(b);
        string s3 = Encoding.Default.GetString(c);
        Debug.Log($" ------>{s1},{s2},{s3}");
        DontDestroyOnLoad(gameObject);

        string aa = "";
        string[] bb = aa.Split(',');
        Debug.LogError("bb length:" + bb.Length);


        AppManager.Instance.Init(this.transform);
    }
}