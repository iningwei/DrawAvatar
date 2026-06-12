using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.DanMu
{
    public class DanMuBaseData
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string uid;

        /// <summary>
        /// 用户名称
        /// </summary>
        public string nickName;

        /// <summary>
        /// 头像Url
        /// </summary>
        public string headerUrl;

        /// <summary>
        /// 消息时间戳（毫秒级）
        /// </summary>
        public long timeStamp;

        /// <summary>
        /// 是否是测试
        /// </summary>
        public bool isTest;

        public DanMuBaseData(string id, string nickName, string headerUrl, long timeStamp,bool isTest)
        {
            this.uid = id;
            this.nickName = nickName;
            this.headerUrl = headerUrl;
            this.timeStamp = timeStamp;
            this.isTest = isTest;
        }
    }
}