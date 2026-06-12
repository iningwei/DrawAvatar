using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace ZGame.ZTable{
	public class ItemDataReader:TableReader{
		private Dictionary<long,ItemData> entityMap = new Dictionary<long,ItemData>();

		static ItemDataReader instance=null;
		public static ItemDataReader Instance{
			get{
				if(instance==null){
					instance=new ItemDataReader();
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
							Debug.LogError("error,already exist key: "+key+" in ItemData,line content:"+line);
							continue;
						}
						var entity=new ItemData();
						entity.ID=long.Parse(vals[0].Trim());
						entity.name=vals[1];
						entity.itemType=int.Parse(vals[2].Trim());
						entity.itemSubType=int.Parse(vals[3].Trim());
						entity.groupId=int.Parse(vals[4].Trim());
						entity.canSaleOn=int.Parse(vals[5].Trim());
						entity.quality=long.Parse(vals[6].Trim());
						entity.useEffectId=long.Parse(vals[7].Trim());
						entity.hudScale=float.Parse(vals[8].Trim());
						entity.desNameId=long.Parse(vals[9].Trim());
						entity.icon=vals[10];
						entity.iconCount=int.Parse(vals[11].Trim());
						entity.bindedActorIds=string.IsNullOrEmpty(vals[12].Trim())?null:vals[12].Trim().Split(',').ToLongArray();
						entity.heroUnlockIcon=string.IsNullOrEmpty(vals[13].Trim())?null:vals[13].Trim().Split(',');
						entity.heroUnlockNameImg=vals[14];
						entity.nameIcon=vals[15];
						entity.desc1=vals[16];
						entity.desc2=vals[17];
						entityMap[key]=entity;
					}
					catch (Exception lineEx){
						Debug.LogError($"lineData:{lines[i]},  lineEx:{lineEx}");
					}
				}
				isLoaded = true;
			};

			string fileName=ItemData.FileName;
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
		public ItemData GetEntity(long key){
			ItemData data;
			if(entityMap.TryGetValue(key,out data)){
				return data;
			}
			else{
				Debug.LogError("no entity with key:"+key+" in ItemData");
				return default(ItemData);
			}
		}
		/// <summary>
		/// get all datas
		/// </summary>
		public Dictionary<long,ItemData> GetEntityMap(){
			return this.entityMap;
		}
	}
}