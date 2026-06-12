using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.DanMu
{
    //点赞
    public class DanMuLikeData : DanMuBaseData
    {
        public long likeCount;

        public DanMuLikeData(string id, string nickName, string headerUrl, long timeStamp, long likeCount, bool isTest) : base(id, nickName, headerUrl, timeStamp, isTest)
        {
            this.likeCount = likeCount;
        }
    }
}
