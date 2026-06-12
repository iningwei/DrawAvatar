using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class RobotDataReader:TableReader{
		private Dictionary<string,RobotData> entityMap = new Dictionary<string,RobotData>();

		static RobotDataReader instance=null;
		public static RobotDataReader Instance{
			get{
				if(instance==null){
					instance=new RobotDataReader();
					instance.Load();
				}
				return instance;
			}
		}

		void Load(){
			Action<string> onTableLoad=(txt)=>{
				string[] lines=txt.Split(new string[]{"\r\n"},System.StringSplitOptions.RemoveEmptyEntries);
				int count=lines.Length;
				if(count<0){
					return;
				}
				for(int i=0;i<count;i++){
					try
					{
						string line=lines[i];
						if(string.IsNullOrEmpty(line)){
							Debug.LogError("data error, line "+i+" is null");
							continue;
						}
						string[] vals=line.Split("@~ExcelTool!@");
						string key=vals[0].Trim();
						if(entityMap.ContainsKey(key)){
							Debug.LogError("error,already exist key: "+key+" in RobotData,line content:"+line);
							continue;
						}
						var entity=new RobotData();
						entity.ID=vals[0];
						entity.nickName=vals[1];
						entity.avatarUrl=vals[2];
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=RobotData.FileName;
			try{
				FileMgr.ReadFile(fileName,onTableLoad);
			}
			catch(System.Exception ex){
				Debug.LogError("error while read:"+fileName+ ", ex:" + ex.ToString());
			}
		}

		/// <summary>
		/// get data by primary key
		/// </summary>
		public RobotData GetEntity(string key){
			RobotData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in RobotData");
				return default(RobotData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<string,RobotData> GetEntityMap(){
			return this.entityMap;
		}
	}
}