using System;
using UnityEngine;
using System.Text;

namespace ZGame.ZTable{
public class SummonData{
	 /// <summary>
	 /// 序号
	 /// </summary>
	 public int ID;

	 /// <summary>
	 /// 
	 /// </summary>
	 public string name;

	 /// <summary>
	 /// 指令类型
	 /// 1,  加入指令      2，666      3，点赞      4，查询      5，喇叭      6，上阵英雄      100.礼物 
	 /// ….
	 /// </summary>
	 public ZGame.DanMu.DanMuCommandType commandType;

	 /// <summary>
	 /// 礼物召唤类型
	 /// 0，无； 1，单位； 2，天帝令
	 /// </summary>
	 public int giftSummonType;

	 /// <summary>
	 /// 指令名称
	 /// </summary>
	 public string[] commandStrs;

	 /// <summary>
	 /// 
	 /// 1,红 ；2，蓝色
	 /// </summary>
	 public string associateCompType;

	 /// <summary>
	 /// 是否启用设置
	 /// 主要针对礼物：是否把该礼物放入设置界面，允许主播开启或者该礼物特效
	 /// </summary>
	 public bool enableSetting;

	 /// <summary>
	 /// 礼物ID
	 /// 与GiftData配置表中的ID要对应上
	 /// </summary>
	 public string giftId;

	 /// <summary>
	 /// 召唤角色规则
	 /// 类型(1为普通召唤以及组刷指定数量召唤指定unitID；
	 /// 2为可组刷触发BOSS解锁、召唤；
	 /// 3为根据当前上阵英雄出对应飞行单位;
	 /// 4召唤将军令)
	 /// 
	 ///  类型1：礼物召唤角色ID,单个礼物召唤数量:组刷个数，召唤的高级角色UnitID，单个礼物召唤数量:组刷个数，解锁的更高级角色UnitID，单个礼物召唤数量
	 /// 
	 ///  类型2：单刷召唤角色ID,召唤个数:组刷个数,解锁bossUnitID:boss解锁后单刷召唤的角色ID,召唤的个数
	 /// 
	 /// 类型3：单刷召唤角色ID,召唤个数:当前上阵英雄ID,召唤角色ID，召唤个数…
	 /// 类型4：每个持续时长,每个增加单位的攻击力百分比,每个增加单位的血量百分比(只对新单位有效，且持续时间结束，血量不变动)
	 /// </summary>
	 public string summonActorRule;

	 /// <summary>
	 /// 键盘快捷键
	 /// 快捷键为0则表示无快捷键;  数字1对应的ascii码为49。Q:113;W:119;E:101;R:114;T:116;Y:121;U:117;I:105;O:111
	 /// </summary>
	 public string keyShortcut;

	 public static string FileName = "tb_summondata";
}
}