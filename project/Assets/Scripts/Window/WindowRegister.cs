using ZGame.Window;
public class WindowRegister
{
	public static void Register()
	{
		var winManager=WindowManager.Instance;
		winManager.RegisterWindowType(WindowNames.CursorWindow,typeof(CursorWindow).ToString(),"CursorWindow");
		winManager.RegisterWindowType(WindowNames.DynamicCfgWindow,typeof(DynamicCfgWindow).ToString(),"DynamicCfgWindow");
		winManager.RegisterWindowType(WindowNames.LoadingWindow,typeof(LoadingWindow).ToString(),"LoadingWindow");
		winManager.RegisterWindowType(WindowNames.LoginWindow,typeof(LoginWindow).ToString(),"LoginWindow");
		winManager.RegisterWindowType(WindowNames.MessageBoxWindow,typeof(MessageBoxWindow).ToString(),"MessageBoxWindow");
		winManager.RegisterWindowType(WindowNames.SettingWindow,typeof(SettingWindow).ToString(),"SettingWindow");
		winManager.RegisterWindowType(WindowNames.NetMaskWindow,typeof(NetMaskWindow).ToString(),"NetMaskWindow");
		winManager.RegisterWindowType(WindowNames.TipWindow,typeof(TipWindow).ToString(),"TipWindow");
	}
}