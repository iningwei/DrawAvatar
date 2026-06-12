using System;
using UnityEngine;
using System.Text;

namespace ZGame.ZTable{
public class EffectData{
	 /// <summary>
	 /// 
	 /// </summary>
	 public long ID;

	 /// <summary>
	 /// 
	 /// 带.prefab后缀的为预制件名。
	 /// 不带后缀的为批处理Key
	 /// </summary>
	 public string prefabName;

	 /// <summary>
	 /// 是否启用特效降频
	 /// 会根据到相机的距离。降低特效的播放概率。
	 /// 一般子弹特效、英雄攻击特效都会开启
	 /// </summary>
	 public bool fxCull;

	 public static string FileName = "tb_effectdata";
}
}