using System;
using UnityEngine;
using System.Text;

namespace ZGame.ZTable{
public class AudioData{
	 /// <summary>
	 /// 技能编号
	 /// </summary>
	 public long ID;

	 /// <summary>
	 /// 音效名
	 /// </summary>
	 public string name;

	 /// <summary>
	 /// 音效文件后缀
	 /// </summary>
	 public string extension;

	 /// <summary>
	 /// 分组
	 /// 0：无分组
	 /// 1：僵尸环境音
	 /// 2：队伍脚步声
	 /// </summary>
	 public int groupType;

	 /// <summary>
	 /// 音量大小
	 /// 0-1
	 /// </summary>
	 public float volume;

	 /// <summary>
	 /// 该音效资源同一时刻播放的个数
	 /// 该音效资源同一时刻播放的个数
	 /// </summary>
	 public int existCountLimit;

	 public static string FileName = "tb_audiodata";
}
}