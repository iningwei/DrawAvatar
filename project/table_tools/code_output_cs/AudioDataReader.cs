using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class AudioDataReader:TableReader{
		private Dictionary<long,AudioData> entityMap = new Dictionary<long,AudioData>();

		static AudioDataReader instance=null;
		public static AudioDataReader Instance{
			get{
				if(instance==null){
					instance=new AudioDataReader();
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
							Debug.LogError("error,already exist key: "+key+" in AudioData,line content:"+line);
							continue;
						}
						var entity=new AudioData();
						entity.ID=long.Parse(vals[0].Trim());
						entity.name=vals[1];
						entity.extension=vals[2];
						entity.groupType=int.Parse(vals[3].Trim());
						entity.volume=float.Parse(vals[4].Trim());
						entity.existCountLimit=int.Parse(vals[5].Trim());
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=AudioData.FileName;
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
		public AudioData GetEntity(long key){
			AudioData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in AudioData");
				return default(AudioData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<long,AudioData> GetEntityMap(){
			return this.entityMap;
		}
	}
}