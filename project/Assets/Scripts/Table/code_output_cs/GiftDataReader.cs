using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class GiftDataReader:TableReader{
		private Dictionary<string,GiftData> entityMap = new Dictionary<string,GiftData>();

		static GiftDataReader instance=null;
		public static GiftDataReader Instance{
			get{
				if(instance==null){
					instance=new GiftDataReader();
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
							Debug.LogError("error,already exist key: "+key+" in GiftData,line content:"+line);
							continue;
						}
						var entity=new GiftData();
						entity.ID=vals[0];
						entity.gift_name=vals[1];
						entity.gift_shortName=vals[2];
						entity.sec_gift_id=vals[3];
						entity.video_id=long.Parse(vals[4].Trim());
						entity.gift_value=long.Parse(vals[5].Trim());
						entity.heroExp_added=long.Parse(vals[6].Trim());
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=GiftData.FileName;
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
		public GiftData GetEntity(string key){
			GiftData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in GiftData");
				return default(GiftData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<string,GiftData> GetEntityMap(){
			return this.entityMap;
		}
	}
}