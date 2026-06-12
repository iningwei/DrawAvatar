using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Window
{
    public class WindowLayer
    {
        public static string Hidden = "hidden";//Some windows may always be used,so after close window,we inactive them and move them to this layer for reuse

        public static string Basic = "basic";//especially used for game's main ui window
        public static string Basic2 = "basic2";
        public static string Hud = "hud";//used for some pop up window
        public static string Hud2 = "hud2";
        public static string Hud3 = "hud3";
        public static string Msg = "msg";//used for some message box or message tips
        public static string SceneChange = "scenechange";//used for scene change  
        public static string NetMask = "netmask";//used for net connect delay
        public static string InteractMask = "interactmask";//used for mask all UI interacts,except windows in Top Layer
        public static string Notify = "notify";// for notify
        public static string Top = "top";//toppest layer,used something you always want player to see.

        public static List<string> LayerList = new List<string>()
    {
        Hidden,
        Basic,
        Basic2,
        Hud,
        Hud2,
        Hud3,
        Msg,
        SceneChange,
        NetMask,
        InteractMask,
        Notify,
        Top
    };

        static Transform rootCanvasTran => WindowManager.Instance.GetRootCanvasTran();

        static Dictionary<string, Transform> layerDic = null;
        public static Transform GetTran(string layerName)
        {
            if (layerDic == null)
            {
                layerDic = new Dictionary<string, Transform>();
            }
            if (layerDic.ContainsKey(layerName))
            {
                return layerDic[layerName];
            }
            else
            {
                var tran = rootCanvasTran.Find(layerName);
                layerDic[layerName] = tran;
                return tran;
            }
        }
    }


}