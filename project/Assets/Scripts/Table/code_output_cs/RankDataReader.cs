using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class RankDataReader:TableReader{
		private Dictionary<long,RankData> entityMap = new Dictionary<long,RankData>();

		static RankDataReader instance=null;
		public static RankDataReader Instance{
			get{
				if(instance==null){
					instance=new RankDataReader();
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
						long key=long.Parse(vals[0].Trim());
						if(entityMap.ContainsKey(key)){
							Debug.LogError("error,already exist key: "+key+" in RankData,line content:"+line);
							continue;
						}
						var entity=new RankData();
						entity.ID=long.Parse(vals[0].Trim());
						entity.desc=vals[1];
						entity.rankReward=vals[2];
						entity.desContentId=long.Parse(vals[3].Trim());
						entity.announceRankNum=long.Parse(vals[4].Trim());
						entity.tipTxt=vals[5];
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=RankData.FileName;
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
		public RankData GetEntity(long key){
			RankData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in RankData");
				return default(RankData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<long,RankData> GetEntityMap(){
			return this.entityMap;
		}
	}
}