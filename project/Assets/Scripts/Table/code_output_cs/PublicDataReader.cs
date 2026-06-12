using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class PublicDataReader:TableReader{
		private Dictionary<int,PublicData> entityMap = new Dictionary<int,PublicData>();

		static PublicDataReader instance=null;
		public static PublicDataReader Instance{
			get{
				if(instance==null){
					instance=new PublicDataReader();
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
							Debug.LogError("error,already exist key: "+key+" in PublicData,line content:"+line);
							continue;
						}
						var entity=new PublicData();
						entity.ID=int.Parse(vals[0].Trim());
						entity.val=float.Parse(vals[1].Trim());
						entity.valList=string.IsNullOrEmpty(vals[2].Trim())?null:vals[2].Trim().Split(',').ToFloatArray();
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=PublicData.FileName;
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
		public PublicData GetEntity(int key){
			PublicData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in PublicData");
				return default(PublicData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<int,PublicData> GetEntityMap(){
			return this.entityMap;
		}
	}
}