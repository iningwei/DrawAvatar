using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.DanMu
{
    //礼物
    public class DanMuGiftData : DanMuBaseData
    {
        /// <summary>
        /// 抖音加密后的礼物Id
        /// </summary>
        public string giftId;
        /// <summary>
        /// 礼物数量
        /// </summary>
        public long giftNum;
        /// <summary>
        /// 礼物总价值（单位：分）
        /// </summary>
        public long giftTotalValue;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nickName"></param>
        /// <param name="headerUrl"></param>
        /// <param name="timeStamp"></param>
        /// <param name="secGiftId"></param>
        /// <param name="giftNum">礼物个数</param>
        /// <param name="giftTotalValue">礼物总价值</param>
        public DanMuGiftData(string id, string nickName, string headerUrl, long timeStamp, string secGiftId, long giftNum, long giftTotalValue, bool isTest) : base(id, nickName, headerUrl, timeStamp, isTest)
        {
            this.giftId = secGiftId;
            this.giftNum = giftNum;
            this.giftTotalValue = giftTotalValue;
        }
    }
}