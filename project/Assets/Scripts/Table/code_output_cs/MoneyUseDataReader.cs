using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class MoneyUseDataReader:TableReader{
		private Dictionary<long,MoneyUseData> entityMap = new Dictionary<long,MoneyUseData>();

		static MoneyUseDataReader instance=null;
		public static MoneyUseDataReader Instance{
			get{
				if(instance==null){
					instance=new MoneyUseDataReader();
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
							Debug.LogError("error,already exist key: "+key+" in MoneyUseData,line content:"+line);
							continue;
						}
						var entity=new MoneyUseData();
						entity.ID=long.Parse(vals[0].Trim());
						entity.isShowProbability=bool.Parse(vals[1].Trim());
						entity.randomPool=string.IsNullOrEmpty(vals[2].Trim())?null:vals[2].Trim().Split(',').ToFloatArray();
						entity.pityRandomPool=string.IsNullOrEmpty(vals[3].Trim())?null:vals[3].Trim().Split(',').ToFloatArray();
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=MoneyUseData.FileName;
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
		public MoneyUseData GetEntity(long key){
			MoneyUseData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in MoneyUseData");
				return default(MoneyUseData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<long,MoneyUseData> GetEntityMap(){
			return this.entityMap;
		}
	}
}