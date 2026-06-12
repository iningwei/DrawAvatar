using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class VipDataReader:TableReader{
		private Dictionary<long,VipData> entityMap = new Dictionary<long,VipData>();

		static VipDataReader instance=null;
		public static VipDataReader Instance{
			get{
				if(instance==null){
					instance=new VipDataReader();
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
							Debug.LogError("error,already exist key: "+key+" in VipData,line content:"+line);
							continue;
						}
						var entity=new VipData();
						entity.ID=long.Parse(vals[0].Trim());
						entity.vipExp=long.Parse(vals[1].Trim());
						entity.reward=string.IsNullOrEmpty(vals[2].Trim())?null:vals[2].Trim().Split(',').ToFloatArray();
						entity.vipDailyGift=string.IsNullOrEmpty(vals[3].Trim())?null:vals[3].Trim().Split(',').ToFloatArray();
						entity.vipDailyGiftCost=string.IsNullOrEmpty(vals[4].Trim())?null:vals[4].Trim().Split(',').ToFloatArray();
						entity.vipGift=string.IsNullOrEmpty(vals[5].Trim())?null:vals[5].Trim().Split(',').ToFloatArray();
						entity.vipGiftCost=string.IsNullOrEmpty(vals[6].Trim())?null:vals[6].Trim().Split(',').ToFloatArray();
						entity.desc=vals[7];
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=VipData.FileName;
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
		public VipData GetEntity(long key){
			VipData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in VipData");
				return default(VipData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<long,VipData> GetEntityMap(){
			return this.entityMap;
		}
	}
}