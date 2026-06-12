using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using ZGame.TimerTween;

namespace ZGame.Net.Tcp
{

    public enum SocketState
    {
        UnConnect = 0,
        Connecting,
        Success,
        Error,
        Close
    }

    //消息结构
    //|header:4|msgNo:2|proto|
    //header消息长度4字节：msgNo消息号2字节：proto消息体
    //hearder中记录的长度 = msgNo的2字节长度 + proto消息体的字节长度
    //消息号msgNo定义不能太大，uint范围

    public class ClientSocket
    {
        public int socketId = 0;
        /// <summary>
        /// 是否使用网络字节序处理
        /// 即对于头数据(只针对多字节整数值；对于byte类型由于只有一个字节，不存在大端小端问题)，在上行是否要从主机字节序转换为网络字节序，下行是否要从网络字节序转换为主机字节序
        /// </summary>
        bool useNetworkOrder = true;

        Socket socket;

        SocketState socketState = SocketState.UnConnect;
        public SocketState State
        {
            get
            {
                return this.socketState;
            }
            set
            {
                DebugExt.Log("Set Socket State:" + value);
                this.socketState = value;
            }
        }

        public bool IsConnected()
        {
            return this.socketState == SocketState.Success;
        }

        CircularBuffer recvBuffer;
        byte[] recvBytes;
        private long timeoutCheckId = -1;
        BlockingQueue<Message> msgQueue;

        public ClientSocket(int id, BlockingQueue<Message> msgQueue)
        {
            this.socketId = id;
            this.msgQueue = msgQueue;
            recvBuffer = new CircularBuffer(1024 * 1024 * 2);
            recvBytes = new byte[1024];
        }



        void getIPType(string serverIp, string serverPorts, out string newServerIp, out AddressFamily mIPType)
        {
            mIPType = AddressFamily.InterNetwork;
            newServerIp = serverIp;
            try
            {
                string mIPv6 = SDKExt.GetIPv6ByCS(serverIp, serverPorts);
                //DebugExt.LogE("mIPv6:" + mIPv6);
                if (!string.IsNullOrEmpty(mIPv6))
                {
                    string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
                    if (m_StrTemp != null && m_StrTemp.Length >= 2)
                    {
                        string IPType = m_StrTemp[1];
                        if (IPType == "ipv6")
                        {
                            newServerIp = m_StrTemp[0];
                            mIPType = AddressFamily.InterNetworkV6;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugExt.Log("GetIPv6 error:" + e);
            }

        }


        public void Connect(string ip, int port, float timeout = 5f)
        {
            if (socket != null && socket.Connected)
            {
                DebugExt.LogE("can not connect socket while socket is connected");
                return;
            }

            if (socket == null || !socket.Connected)
            {
                State = SocketState.Connecting;

                AddressFamily af = AddressFamily.InterNetwork;
                string newIp;
                getIPType(ip.Trim(), port.ToString(), out newIp, out af);
                //DebugExt.LogE("ip:" + ip + ",port:" + port.ToString() + ",newIp:" + newIp + ", af:" + af.ToString());
                if (!string.IsNullOrEmpty(newIp))
                {
                    ip = newIp;
                }

                socket = new Socket(af, SocketType.Stream, ProtocolType.Tcp);

                IAsyncResult asyncResult = socket.BeginConnect(ip, port, connectCallback, socket);

                timeoutCheckId = CoroutineManager.Instance.DelayedCall(timeout,
           () =>
           {
               timeoutCheckId = -1;
               if (State == SocketState.Connecting)
               {
                   State = SocketState.Error;
               }
           });
            }
        }

        void connectCallback(IAsyncResult r)
        {
            try
            {
                if (timeoutCheckId != -1)
                {
                    CoroutineManager.Instance.RemoveCoroutine(timeoutCheckId);
                    timeoutCheckId = -1;
                }
                socket.EndConnect(r);

                if (!socket.Connected)
                {
                    State = SocketState.Error;
                }
                else
                {
                    State = SocketState.Success;
                    asyncReceive();

                    //开新线程处理消息数据
                    var pickupMsgThread = new Thread(loopPickupMessage);
                    pickupMsgThread.IsBackground = true;
                    pickupMsgThread.Start();
                }
            }
            catch (Exception ex)
            {
                DebugExt.LogE("connectCallback error:" + ex.ToString());
                State = SocketState.Error;
            }
        }


        bool isHandHeader = true;
        Int16 curMainCmdId = 0;
        Int32 curBodySize = 0;
        void loopPickupMessage()
        {
            //TODO:i删了吧。。。
            int i = 0;
            while (true)
            {
                Thread.Sleep(3);
                i = 2;
                while (i-- >= 0)
                {
                    bool readRet = false;
                    try
                    {
                        if (isHandHeader)
                        {
                            readRet = readHeader();
                        }
                        else
                        {
                            readRet = readBody();
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugExt.LogE("loopPickUpMessage error:" + ex.ToString());
                        if (State == SocketState.Connecting)
                        {
                            State = SocketState.Error;
                        }
                        return;
                    }

                    if (!readRet && (State == SocketState.Error || State == SocketState.Close))
                    {
                        recvBuffer.Clear();
                        return;
                    }
                }
            }
        }

        bool readBody()
        {
            //Debug.LogError("readBody");
            byte[] bytes = null;

            if (recvBuffer.Count < curBodySize)
            {
                //Debug.LogError($"return!!!curBodySize:{curBodySize},recvBuffer.Count:{recvBuffer.Count}");
                return false;
            }

            bytes = recvBuffer.ReadBytes(curBodySize);



            processPacket(bytes);
            isHandHeader = true;
            return true;
        }


        byte[] emptyBuf = new byte[0];
        void processPacket(byte[] data)
        {
            if (data == null)
                data = emptyBuf;

            if (msgQueue == null)
                return;

            msgQueue.Enqueue(new Message((ushort)curMainCmdId, data));
        }

        bool readHeader()
        {
            if (recvBuffer.Count < 4)
            {
                return false;
            }

            int len = recvBuffer.ReadInt32();
            curMainCmdId = recvBuffer.ReadInt16();
            len = useNetworkOrder ? IPAddress.NetworkToHostOrder(len) : len;

            curMainCmdId = useNetworkOrder ? IPAddress.NetworkToHostOrder(curMainCmdId) : curMainCmdId;

            if (len >= 2)//curBodySize是可能为0的，即空消息体
            {
                curBodySize = (len - 2);
                isHandHeader = false;
            }
            return true;
        }

        void asyncReceive()
        {
            if (socket != null && socket.Connected)
            {
                socket.BeginReceive(recvBytes, 0, recvBytes.Length, SocketFlags.None, receiveCallback, socket);
            }
        }

        void receiveCallback(IAsyncResult r)
        {
            //android手机wifi环境下，在大厅内不做任何操作，只有心跳的上行、下行。
            //过一段时间后（时间不固定），这里会通过try catch捕获到异常，报错如下：
            //System.Net.Sockets.SocketException (0x80004005): Network subsystem is down
            //目前还没有找到造成这个异常的具体缘由
            ////目前通过通用的socket error后进行重连来解决
            try
            {
                if (State != SocketState.Success)
                {
                    return;
                }

                int len = socket.EndReceive(r);
                if (len <= 0)
                {
                    State = SocketState.Error;
                    return;
                }

                recvBuffer.Put(recvBytes, 0, len);
            }
            catch (Exception ex)
            {
                DebugExt.LogE("receive callback error:" + ex.ToString());

                State = SocketState.Error;
                return;
            }


            //continue receive
            asyncReceive();
        }


        public void Send(UInt16 mainId, byte[] data)
        {
            try
            {
                if (State != SocketState.Success)
                {
                    return;
                }

                Int32 totalLen = (data.Length + 2);
                //Debug.LogError("  send, totalLen:" + totalLen + ",mainId:" + mainId);

                byte[] sendBytes = new byte[totalLen + 4];
                Int32 totalLenOrdered = useNetworkOrder ? IPAddress.HostToNetworkOrder(totalLen) : totalLen;
                //Debug.LogError("ordered totalLen:" + totalLenOrdered);

                //Write length
                Array.Copy(BitConverter.GetBytes(totalLenOrdered), 0, sendBytes, 0, 4);
                //Write msgId
                Int16 msgIdOrdered = useNetworkOrder ? IPAddress.HostToNetworkOrder((Int16)mainId) : (Int16)mainId;
                Array.Copy(BitConverter.GetBytes(msgIdOrdered), 0, sendBytes, 4, 2);
                //Write data
                if (data != null)
                {
                    Array.Copy(data, 0, sendBytes, 6, data.Length);
                }
                socket.Send(sendBytes, sendBytes.Length, SocketFlags.None);
            }
            catch (Exception ex)
            {
                DebugExt.LogE("send error:" + ex.ToString());
                State = SocketState.Error;
            }
        }

        public void Close()
        {
            State = SocketState.Close;
            if (socket != null && socket.Connected)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    DebugExt.LogE("clientSocket shutdown, ex:" + ex.ToString());
                    State = SocketState.Error;
                }

                try
                {
                    socket.Close();
                }
                catch (Exception ex)
                {
                    DebugExt.LogE("clientSocket close, ex:" + ex.ToString());
                    State = SocketState.Error;
                }
            }

        }
    }
}