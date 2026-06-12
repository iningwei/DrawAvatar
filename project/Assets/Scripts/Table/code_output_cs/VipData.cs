using System;
using UnityEngine;
using System.Text;

namespace ZGame.ZTable{
public class VipData{
	 /// <summary>
	 /// vip等级
	 /// </summary>
	 public long ID;

	 /// <summary>
	 /// 当前等级的最低经验值
	 /// </summary>
	 public long vipExp;

	 /// <summary>
	 /// 奖励
	 /// </summary>
	 public float[] reward;

	 /// <summary>
	 /// 每日礼包
	 /// </summary>
	 public float[] vipDailyGift;

	 /// <summary>
	 /// 购买每日礼包消耗
	 /// </summary>
	 public float[] vipDailyGiftCost;

	 /// <summary>
	 /// 特权礼包
	 /// </summary>
	 public float[] vipGift;

	 /// <summary>
	 /// 购买特权礼包消耗
	 /// </summary>
	 public float[] vipGiftCost;

	 /// <summary>
	 /// 特权描述
	 /// </summary>
	 public string desc;

	 public static string FileName = "tb_vipdata";
}
}