using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.DanMu
{
    //玩家进入直播间
    public class DanMuMemberData : DanMuBaseData
    {
        public DanMuMemberData(string id, string nickName, string headerUrl, long timeStamp, string content,bool isTest) : base(id, nickName, headerUrl, timeStamp,isTest)
        {
        }
    }
}