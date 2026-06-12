using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class RankEnterDataReader:TableReader{
		private Dictionary<long,RankEnterData> entityMap = new Dictionary<long,RankEnterData>();

		static RankEnterDataReader instance=null;
		public static RankEnterDataReader Instance{
			get{
				if(instance==null){
					instance=new RankEnterDataReader();
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
							Debug.LogError("error,already exist key: "+key+" in RankEnterData,line content:"+line);
							continue;
						}
						var entity=new RankEnterData();
						entity.ID=long.Parse(vals[0].Trim());
						entity.rankRange=string.IsNullOrEmpty(vals[1].Trim())?null:vals[1].Trim().Split(',').ToLongArray();
						entity.enterEffectType=int.Parse(vals[2].Trim());
						entity.effectId=long.Parse(vals[3].Trim());
						entity.videoId=long.Parse(vals[4].Trim());
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=RankEnterData.FileName;
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
		public RankEnterData GetEntity(long key){
			RankEnterData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in RankEnterData");
				return default(RankEnterData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<long,RankEnterData> GetEntityMap(){
			return this.entityMap;
		}
	}
}