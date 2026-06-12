using System;
using UnityEngine;
using System.Text;

namespace ZGame.ZTable{
public class ItemData{
	 /// <summary>
	 /// 编号
	 /// 备注
	 /// </summary>
	 public long ID;

	 /// <summary>
	 /// 名称
	 /// </summary>
	 public string name;

	 /// <summary>
	 /// 类型
	 /// 1-货币、道具（money）
	 /// 2-称号（title）
	 /// 3-头像框(avatarFrame)
	 /// 4-入场特效(avatarFrame)
	 /// </summary>
	 public int itemType;

	 /// <summary>
	 /// 子类型
	 /// 子类型：
	 /// 0，常规类型
	 /// 1，货币
	 /// 2，vip头像框
	 /// </summary>
	 public int itemSubType;

	 /// <summary>
	 /// 同组ID
	 /// 相同组使用相同ID
	 /// </summary>
	 public int groupId;

	 /// <summary>
	 /// 是否可在商店上架
	 /// 0-否
	 /// 1-是
	 /// 货币子类型非0：不可上架
	 /// </summary>
	 public int canSaleOn;

	 /// <summary>
	 /// 品质
	 /// </summary>
	 public long quality;

	 /// <summary>
	 /// 使用特效ID
	 /// 对应客户端ItemUseEffectData表中的ID
	 /// </summary>
	 public long useEffectId;

	 /// <summary>
	 /// 头部hud节点缩放
	 /// 只针对itemType为2有效
	 /// </summary>
	 public float hudScale;

	 /// <summary>
	 /// 描述名称id
	 /// 对应语言表id
	 /// </summary>
	 public long desNameId;

	 /// <summary>
	 /// UI图标
	 /// 图集名:资源名
	 /// </summary>
	 public string icon;

	 /// <summary>
	 /// UI序列帧数量
	 /// --如果配置了数量就代表是序列帧图标。
	 /// --例如该字段配置的4，icon字段的值为“Icon:icon_ys”
	 /// --则序列帧对应的图就是：Icon:icon_ys_01，Icon:icon_ys_02
	 /// Icon:icon_ys_03
	 /// Icon:icon_ys_04
	 /// </summary>
	 public int iconCount;

	 /// <summary>
	 /// 对应的actorId
	 /// 多于1个的随机取一个
	 /// </summary>
	 public long[] bindedActorIds;

	 /// <summary>
	 /// 英雄解锁UI图标
	 /// 图集名:资源名,图集名:资源名
	 /// 
	 /// 第一个为红色，第二个为蓝色
	 /// </summary>
	 public string[] heroUnlockIcon;

	 /// <summary>
	 /// 英雄解锁名称图标
	 /// 图集名:资源名
	 /// </summary>
	 public string heroUnlockNameImg;

	 /// <summary>
	 /// 名称图标
	 /// 图集名:资源名
	 /// </summary>
	 public string nameIcon;

	 /// <summary>
	 /// 描述
	 /// </summary>
	 public string desc1;

	 /// <summary>
	 /// 详细描述
	 /// </summary>
	 public string desc2;

	 public static string FileName = "tb_itemdata";
}
}