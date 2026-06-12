using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Window
{
    public class WindowInfo
    {
        public string scriptName;

        /// <summary>
        /// resource name,without suffix
        /// </summary>
        public string resName;

        public WindowInfo(string scriptName, string resName)
        {
            this.scriptName = scriptName;
            this.resName = resName;
        }
    }
}