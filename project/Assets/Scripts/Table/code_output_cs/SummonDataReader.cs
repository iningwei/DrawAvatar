using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class SummonDataReader:TableReader{
		private Dictionary<int,SummonData> entityMap = new Dictionary<int,SummonData>();

		static SummonDataReader instance=null;
		public static SummonDataReader Instance{
			get{
				if(instance==null){
					instance=new SummonDataReader();
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
							Debug.LogError("error,already exist key: "+key+" in SummonData,line content:"+line);
							continue;
						}
						var entity=new SummonData();
						entity.ID=int.Parse(vals[0].Trim());
						entity.name=vals[1];
						if (Enum.TryParse(vals[2].Trim(), out ZGame.DanMu.DanMuCommandType e0))
						{
							entity.commandType = e0;
						}
						else
						{
							Debug.LogError("Failed parse {vals[2].Trim()} to enum ZGame.DanMu.DanMuCommandType ");
						}
						entity.giftSummonType=int.Parse(vals[3].Trim());
						entity.commandStrs=string.IsNullOrEmpty(vals[4].Trim())?null:vals[4].Trim().Split(',');
						entity.associateCompType=vals[5];
						entity.enableSetting=bool.Parse(vals[6].Trim());
						entity.giftId=vals[7];
						entity.summonActorRule=vals[8];
						entity.keyShortcut=vals[9];
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=SummonData.FileName;
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
		public SummonData GetEntity(int key){
			SummonData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in SummonData");
				return default(SummonData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<int,SummonData> GetEntityMap(){
			return this.entityMap;
		}
	}
}