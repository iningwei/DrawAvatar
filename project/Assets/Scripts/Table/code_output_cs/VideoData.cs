using System;
using UnityEngine;
using System.Text;

namespace ZGame.ZTable{
public class VideoData{
	 /// <summary>
	 /// 
	 /// </summary>
	 public long ID;

	 /// <summary>
	 /// 视频名
	 /// </summary>
	 public string name;

	 /// <summary>
	 /// 视频文件后缀
	 /// </summary>
	 public string extension;

	 /// <summary>
	 /// 音量大小
	 /// 0-1
	 /// </summary>
	 public float volume;

	 public static string FileName = "tb_videodata";
}
}