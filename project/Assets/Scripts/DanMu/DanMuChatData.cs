using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.DanMu
{
    public class DanMuChatData : DanMuBaseData
    {
        /// <summary>
        /// 评论 内容
        /// </summary>
        public string content;
        public DanMuChatData(string id, string nickName, string headerUrl, long timeStamp, string content,bool isTest) : base(id, nickName, headerUrl, timeStamp,isTest)
        {
            this.content = content;
        }
    }
}