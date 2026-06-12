using ZGame.ZTable;
public class TableFetch{
	public static void LoadAllTables(){
		AudioDataReader.Instance.CallEmpty();
		ClientRankDataReader.Instance.CallEmpty();
		ConfigDataReader.Instance.CallEmpty();
		EffectDataReader.Instance.CallEmpty();
		GiftDataReader.Instance.CallEmpty();
		ItemDataReader.Instance.CallEmpty();
		LanguageDataReader.Instance.CallEmpty();
		MoneyUseDataReader.Instance.CallEmpty();
		PublicDataReader.Instance.CallEmpty();
		RankDataReader.Instance.CallEmpty();
		RankEnterDataReader.Instance.CallEmpty();
		RobotDataReader.Instance.CallEmpty();
		SummonDataReader.Instance.CallEmpty();
		VideoDataReader.Instance.CallEmpty();
		VipDataReader.Instance.CallEmpty();
		ZhuBoDataReader.Instance.CallEmpty();
	}
	public static bool IsAllTablesLoaded(){
		return AudioDataReader.Instance.IsLoaded()&&
				 ClientRankDataReader.Instance.IsLoaded()&&
				 ConfigDataReader.Instance.IsLoaded()&&
				 EffectDataReader.Instance.IsLoaded()&&
				 GiftDataReader.Instance.IsLoaded()&&
				 ItemDataReader.Instance.IsLoaded()&&
				 LanguageDataReader.Instance.IsLoaded()&&
				 MoneyUseDataReader.Instance.IsLoaded()&&
				 PublicDataReader.Instance.IsLoaded()&&
				 RankDataReader.Instance.IsLoaded()&&
				 RankEnterDataReader.Instance.IsLoaded()&&
				 RobotDataReader.Instance.IsLoaded()&&
				 SummonDataReader.Instance.IsLoaded()&&
				 VideoDataReader.Instance.IsLoaded()&&
				 VipDataReader.Instance.IsLoaded()&&
				 ZhuBoDataReader.Instance.IsLoaded();
	}
}