using System;
using UnityEngine;
using System.Text;

namespace ZGame.ZTable{
public class GiftData{
	 /// <summary>
	 /// 编号
	 /// </summary>
	 public string ID;

	 /// <summary>
	 /// 礼物名称
	 /// </summary>
	 public string gift_name;

	 /// <summary>
	 /// 
	 /// </summary>
	 public string gift_shortName;

	 /// <summary>
	 /// 加密礼物id
	 /// 对应平台的礼物id
	 /// </summary>
	 public string sec_gift_id;

	 /// <summary>
	 /// 送礼视频id
	 /// </summary>
	 public long video_id;

	 /// <summary>
	 /// 礼物价值（分）
	 /// 单个礼物价值。注意单位是： 分
	 /// </summary>
	 public long gift_value;

	 /// <summary>
	 /// 每个礼物，玩家获得到的英雄经验
	 /// </summary>
	 public long heroExp_added;

	 public static string FileName = "tb_giftdata";
}
}