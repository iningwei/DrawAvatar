using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class CompInfo
    {
        public Transform tran;
        public CompInfo(Transform tran)
        {
            this.tran = tran;
        }
    }
}
