using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class ClientRankDataReader:TableReader{
		private Dictionary<int,ClientRankData> entityMap = new Dictionary<int,ClientRankData>();

		static ClientRankDataReader instance=null;
		public static ClientRankDataReader Instance{
			get{
				if(instance==null){
					instance=new ClientRankDataReader();
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
						int key=int.Parse(vals[0].Trim());
						if(entityMap.ContainsKey(key)){
							Debug.LogError("error,already exist key: "+key+" in ClientRankData,line content:"+line);
							continue;
						}
						var entity=new ClientRankData();
						entity.ID=int.Parse(vals[0].Trim());
						entity.rankRange=string.IsNullOrEmpty(vals[1].Trim())?null:vals[1].Trim().Split(',').ToIntArray();
						entity.joinEffect=string.IsNullOrEmpty(vals[2].Trim())?null:vals[2].Trim().Split(',');
						entity.duration=string.IsNullOrEmpty(vals[3].Trim())?null:vals[3].Trim().Split(',').ToFloatArray();
						entity.effectType=int.Parse(vals[4].Trim());
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=ClientRankData.FileName;
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
		public ClientRankData GetEntity(int key){
			ClientRankData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in ClientRankData");
				return default(ClientRankData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<int,ClientRankData> GetEntityMap(){
			return this.entityMap;
		}
	}
}