using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class EffectDataReader:TableReader{
		private Dictionary<long,EffectData> entityMap = new Dictionary<long,EffectData>();

		static EffectDataReader instance=null;
		public static EffectDataReader Instance{
			get{
				if(instance==null){
					instance=new EffectDataReader();
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
							Debug.LogError("error,already exist key: "+key+" in EffectData,line content:"+line);
							continue;
						}
						var entity=new EffectData();
						entity.ID=long.Parse(vals[0].Trim());
						entity.prefabName=vals[1];
						entity.fxCull=bool.Parse(vals[2].Trim());
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=EffectData.FileName;
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
		public EffectData GetEntity(long key){
			EffectData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in EffectData");
				return default(EffectData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<long,EffectData> GetEntityMap(){
			return this.entityMap;
		}
	}
}