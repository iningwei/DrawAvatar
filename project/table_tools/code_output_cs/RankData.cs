using System;
using UnityEngine;
using System.Text;

namespace ZGame.ZTable{
public class RankData{
	 /// <summary>
	 /// 编号
	 /// 备注
	 /// </summary>
	 public long ID;

	 /// <summary>
	 /// 备注
	 /// </summary>
	 public string desc;

	 /// <summary>
	 /// 排行榜结算奖励
	 /// 名次区间，奖励id1，数量1，奖励id2，数量2，。。。；
	 /// 例：第一名奖励2个道具，配置为：1,1,道具1，数量1，道具2，数量2
	 /// 第二至10名奖励1个道具，配置为：2,10,道具1，数量1；
	 /// 多个名次区间以分号隔开
	 /// </summary>
	 public string rankReward;

	 /// <summary>
	 /// 排行榜描述内容对应语言表ID
	 /// </summary>
	 public long desContentId;

	 /// <summary>
	 /// 全服飘屏检测名次
	 /// 仅对玩家的榜单生效，配置0，则无全服飘屏
	 /// </summary>
	 public long announceRankNum;

	 /// <summary>
	 /// 全服飘屏提示文字
	 /// （仅客户端）
	 /// 玩家名字和名次用占位符{0}和{1}
	 /// </summary>
	 public string tipTxt;

	 public static string FileName = "tb_rankdata";
}
}