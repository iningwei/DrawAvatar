using System;
using UnityEngine;
using System.Text;

namespace ZGame.ZTable{
public class MoneyUseData{
	 /// <summary>
	 /// 编号
	 /// 备注
	 /// </summary>
	 public long ID;

	 /// <summary>
	 /// 是否显示保底概率信息
	 /// 仅客户端用
	 /// </summary>
	 public bool isShowProbability;

	 /// <summary>
	 /// 随机池
	 /// 次数，随机池id,…
	 /// </summary>
	 public float[] randomPool;

	 /// <summary>
	 /// 保底随机池
	 /// 次数，随机池id
	 /// </summary>
	 public float[] pityRandomPool;

	 public static string FileName = "tb_moneyusedata";
}
}