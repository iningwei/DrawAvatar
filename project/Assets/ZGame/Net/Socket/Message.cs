
using System;

namespace ZGame.Net
{
    public class Message
    {
        public UInt16 mainCmdId;

        public byte[] Content { get; set; }
        public Message(UInt16 mainCmdId, byte[] data)
        {
            this.mainCmdId = mainCmdId;


            this.Content = data;
        }
    }
}