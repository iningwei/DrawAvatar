namespace ZGame.Event
{
    //1-10000 为游戏框架使用ID
    public class EventID : EventBaseID
    {
        //----------------------->游戏逻辑相关 
        public const string OnUICameraOverlayAndStackSetFinished = "10001";

        //弹幕框架相关
        public const string OnSaveAvatar = "12000";
        public const string OnKSCodeReceived = "30000";


        public const string OnSelfPetQuery = "12110";
        public const string OnVersionContentUpdate = "12451";
        public const string OnScrollMsgTip = "14002";

        public const string OnSystemNotice = "14023";
        public const string OnRoomNotify = "15001";
        public const string OnSummonTip = "15002";

        public const string OnDaddyJoin = "20000";//金主加入
        public const string OnKickDaddy = "20001";//金主被T，加入到了其它直播间 
        public const string OnDaddySendGift = "20003";//玩家送礼
        public const string OnDaddyLike = "20004";//玩家点赞 

   
        public const string OnGameOver = "30001";
        public const string OnMainWindowShowed = "31000";
    }
}