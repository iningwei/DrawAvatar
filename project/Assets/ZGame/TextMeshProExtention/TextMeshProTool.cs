using System;
using TMPro;
using UnityEngine;
using ZGame.TextMeshProExtention;

namespace ZGame
{
    public class TextMeshProTool
    {
        public static void FillTextMeshProUGUIWithContent(GameObject obj, string text)
        {
            var pro = obj.GetComponent<TextMeshProUGUI>();
            if (pro != null)
            {
                pro.text = text;
            }
            else
            {
                Debug.LogError(obj.GetHierarchy() + " have no TextMeshProUGUI comp");
            }
        }

        public static void RegisterTextMeshProUGUILinkAction(GameObject tmpObj, Camera cam, Action<string> callback)
        {
            var link = tmpObj.GetComponent<TextMeshProUGUILink>();
            if (link != null)
            {
                link.cam = cam;
                link.onLinkClicked = null;
                link.onLinkClicked = callback;
            }
        }
    }
}
