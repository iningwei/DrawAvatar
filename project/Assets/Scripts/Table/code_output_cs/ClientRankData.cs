using System;
using UnityEngine;
using System.Text;

namespace ZGame.ZTable{
public class ClientRankData{
	 /// <summary>
	 /// 编号
	 /// </summary>
	 public int ID;

	 /// <summary>
	 /// 排名范围
	 /// </summary>
	 public int[] rankRange;

	 /// <summary>
	 /// 入场特效
	 /// 只对视频特效有用，第一个为战力榜视频，第二个为战功榜视频
	 /// </summary>
	 public string[] joinEffect;

	 /// <summary>
	 /// 持续时长
	 /// 只针对视频特效有用，第一个为战力榜视频时长，第二个为战功榜视频时长
	 /// </summary>
	 public float[] duration;

	 /// <summary>
	 /// 特效类型
	 /// 1.纯UI表现
	 /// 2.视频特效，对应的视频文件名
	 /// </summary>
	 public int effectType;

	 public static string FileName = "tb_clientrankdata";
}
}