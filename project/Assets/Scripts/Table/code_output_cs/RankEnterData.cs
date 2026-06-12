using System;
using UnityEngine;
using System.Text;

namespace ZGame.ZTable{
public class RankEnterData{
	 /// <summary>
	 /// 序号
	 /// </summary>
	 public long ID;

	 /// <summary>
	 /// 排名范围
	 /// </summary>
	 public long[] rankRange;

	 /// <summary>
	 /// 入场表现类型
	 /// 0：无；1：特效；2：视频
	 /// </summary>
	 public int enterEffectType;

	 /// <summary>
	 /// 入场特效ID
	 /// </summary>
	 public long effectId;

	 /// <summary>
	 /// 入场视频ID
	 /// </summary>
	 public long videoId;

	 public static string FileName = "tb_rankenterdata";
}
}