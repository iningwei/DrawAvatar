using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;
using System.Linq;
using ZGame.ZTable;
using ZGame.DanMu; 

public class Daddy
{
    public string uid;
    public long campType;
    public string nick;
    public string headUrl;

    public bool isRobot = false;
    public double totalDamage;

    long heroExp;
     
    public long GetHeroExp()
    {
        return heroExp;
    }
    

     
    public void AddDamageValue(double value)
    {
        if (value < 0)
        {
            Debug.LogError("error damage value <0,  value is:" + value);
        }
        totalDamage += value;
    }

    public void CleanDamageValue()
    {
        totalDamage = 0;
    }

    public Daddy(string id, long campType, string nick, string headUrl)
    {
        this.uid = id;
        this.campType = campType;
        this.nick = nick;
        this.headUrl = headUrl;

        if (int.TryParse(id, out int outId))
        {
            this.isRobot = true;
        }
        Debug.Log("daddy:" + uid + $",campType:{campType}，生成");
        //WindowUtil.ShowTip(uid + " 加入阵营：" + campType);


        EventDispatcher.Instance.AddListener(EventID.OnDaddySendGift, this.onSendGift);
        EventDispatcher.Instance.AddListener(EventID.OnDaddyLike, this.onDaddyLike); 
    }


    

    private void onDaddyLike(string evtId, object[] paras)
    {
        string uid = (string)paras[0];
        if (uid == this.uid)
        {
            long count = (long)paras[1];
 
        }
    }

    

     
    public void Update()
    {
     
    }


    string itemNameIcon;
    long itemCount;
    private void onSendGift(string evtId, object[] paras)
    {
        string uid = paras[0].ToString();
        if (this.uid == uid)
        {
            //Debug.LogError($"daddy {this.uid} recv gift,campType:{this.campType}");
            DanMuGiftData danmuGiftData = paras[1] as DanMuGiftData;

            //英雄经验
            var giftData = BeanManager.Instance.GetGiftDataBySecId(danmuGiftData.giftId);
            if (giftData != null)
            {
                long exp = danmuGiftData.giftNum * giftData.heroExp_added; 
            }

            var summonData = BeanManager.Instance.GetSummonDataByGiftId(giftData.ID);
            if (summonData.giftSummonType == 0)
            {
                return;
            }


            if (summonData.giftSummonType == 1)
            {
                //召唤单位
                this.summonUnit(danmuGiftData, out long outUnitId, out long outUnitCount, out long outUnlockHeroUnitId);
               
            }
            else if (summonData.giftSummonType == 2)
            {
                //召唤天帝令 
            }

            ////送礼特效、召唤提示、通用滚动提示
            var giftBean = BeanManager.Instance.GetGiftDataBySecId(danmuGiftData.giftId);

            //召唤提示
            EventDispatcher.Instance.DispatchEvent(EventID.OnSummonTip, uid, this.itemNameIcon, this.itemCount);

            //通用滚动提示
            EventDispatcher.Instance.DispatchEvent(EventID.OnScrollMsgTip, danmuGiftData.uid, $"送出了 {giftBean.gift_name}x{danmuGiftData.giftNum}");
        }
    }

    //outUnitId：这次送礼召唤的单位UnitId
    //outUnitCount: 这次送礼召唤的单位数量
    //outUnlockHeroUnitId：为0表示这次送礼没有解锁英雄，否则表示解锁的英雄UnitId
    private void summonUnit(DanMuGiftData danmuGiftData, out long outUnitId, out long outUnitCount, out long outUnlockHeroUnitId)
    {
        outUnitId = 0;
        outUnitCount = 0;
        outUnlockHeroUnitId = 0;

        var secGiftId = danmuGiftData.giftId;//加密礼物id
        var giftData = BeanManager.Instance.GetGiftDataBySecId(secGiftId);
        var giftCount = danmuGiftData.giftNum;
        if (giftData != null)
        {
            var summonData = BeanManager.Instance.GetSummonDataByGiftId(giftData.ID);
            if (summonData != null)
            {
                string[] rule = summonData.summonActorRule.Split(':');
                if (rule != null && rule.Length > 0)
                {
                    int ruleType = int.Parse(rule[0]);
                    if (ruleType == 1)//单招或组刷召唤特殊单位
                    {
                        var sub = rule[1].Split(',');
                        long unitId = long.Parse(sub[0]);//默认读取非指定组刷
                        int summonCount = int.Parse(sub[1]);

                        for (int i = 2; i < 4; i++)
                        {
                            var tmp = rule[i].Split(',');
                            if (long.Parse(tmp[0]) == giftCount)  //礼物数量匹配指定组刷数量
                            {
                                unitId = long.Parse(tmp[1]);
                                summonCount = int.Parse(tmp[2]);
                                Debug.LogError($"礼物类型1，匹配上了组刷{giftCount}召唤 ");
                                break;
                            }
                        }

                        for (int i = 0; i < giftCount; i++)
                        {
                          
                        }

                        //处理out参数
                        outUnitId = unitId;
                        outUnitCount = summonCount * giftCount;
                        outUnlockHeroUnitId = 0;
                    }
                    else if (ruleType == 2)//可触发hero解锁
                    {
                        //逻辑
                        //当前没有解锁对应hero，礼物数量达到hero解锁条件，则解锁hero 
                        //hero存在的话，则召唤对应小兵 
                        //否则召唤其它小兵


                        var sub1 = rule[1].Split(',');//hero解锁前召唤的角色ID,个数
                        var sub2 = rule[2].Split(',');//组刷个数,解锁hero unitId
                        var sub3 = rule[3].Split(',');//hero解锁后召唤的角色ID，个数


                        long targetHeroUnitId = long.Parse(sub2[1]);
                        

                    }
                    else if (ruleType == 3)//飞行单位处理
                    {
                       
                    }
                    else if (ruleType == 4)//天帝令
                    {
                        //在summonGodWand 中处理

                    }
                    else
                    {
                        Debug.LogError("not implement ruleType:" + ruleType);
                    }
                }
            }
        }
    }



    

    

    public void Destroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnDaddySendGift, this.onSendGift);
        EventDispatcher.Instance.RemoveListener(EventID.OnDaddyLike, this.onDaddyLike); 
    }

}
