using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class ZhuBoDataReader:TableReader{
		private Dictionary<long,ZhuBoData> entityMap = new Dictionary<long,ZhuBoData>();

		static ZhuBoDataReader instance=null;
		public static ZhuBoDataReader Instance{
			get{
				if(instance==null){
					instance=new ZhuBoDataReader();
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
							Debug.LogError("error,already exist key: "+key+" in ZhuBoData,line content:"+line);
							continue;
						}
						var entity=new ZhuBoData();
						entity.ID=long.Parse(vals[0].Trim());
						entity.avatarUrl=vals[1];
						entity.nickName=vals[2];
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=ZhuBoData.FileName;
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
		public ZhuBoData GetEntity(long key){
			ZhuBoData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in ZhuBoData");
				return default(ZhuBoData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<long,ZhuBoData> GetEntityMap(){
			return this.entityMap;
		}
	}
}