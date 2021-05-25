using Com.Tongtech.Tlq7Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TongLinkQ_SDK
{
    public class SDKClient
    {
        public interface IReceiver
        {
            void receiveData(string message);
        }
        private string host;
        private int port;
        private string qcuName;
        private string queueName;
        private string topicName;
        private string rcvQueName;
        private int pTlqConnFlag = 0;
        private int cancelpubFlag;
        private int cancelsubFlag;
        private TLQCONNCONTEXT client;
        private int ret = 0;
        private string CorrMsgId = null;
        private TLQ_ID gid = default(TLQ_ID);
        private TLQ_QCUHDL qcuId = default(TLQ_QCUHDL);
        private TLQError errstru = default(TLQError);
        private int pWaitInterval;
        private const int MaxSize = 100;
        private const int RETRY_COUNT = 3;
        private const int ERR_PROC_RETURN = 0;
        private const int ERR_PROC_RECONN = 1;
        private const int ERR_PROC_RETRY = 2;
        public Encoding encoding = Encoding.UTF8;
        public SDKClient(string host, int port, string qcuName, string topicName, string queueName, string rcvQueName, int pWaitInterval)
        {
            this.host = host;
            this.port = port;
            this.qcuName = qcuName;
            this.queueName = queueName;
            this.topicName = topicName;
            this.rcvQueName = rcvQueName;
            this.pWaitInterval = pWaitInterval;
            this.client = default(TLQCONNCONTEXT);
            TLQInterface.Tlq_InitTlqConnContext(ref this.client);
            this.client.BrokerId = -1;
            this.client.HostName = host;
            this.client.ListenPort = port;
        }
        public void setSubFlag(int _cancelsubFlag)
        {
            this.cancelsubFlag = _cancelsubFlag;
        }
        public int openConnection()
        {
            this.ret = TLQInterface.Tlq_SetConnContext(ref this.gid, ref this.client, ref this.errstru);
            bool flag = this.ret < 0;
            if (flag)
            {
                throw new Exception("Client connect context failed! Error string is " + this.errstru.errstr);
            }
            this.ret = TLQInterface.Tlq_Conn(ref this.gid, ref this.errstru);
            bool flag2 = this.ret < 0;
            if (flag2)
            {
                throw new Exception("Client connect failed! Error string is " + this.errstru.errstr);
            }
            this.ret = TLQInterface.Tlq_OpenQCU(ref this.gid, ref this.qcuId, this.qcuName, ref this.errstru);
            bool flag3 = this.ret < 0;
            if (flag3)
            {
                throw new Exception("Client qcu failed! Error string is " + this.errstru.errstr);
            }
            this.pTlqConnFlag = 1;
            return 0;
        }
        public void closeConnection()
        {
            TLQError tLQError = default(TLQError);
            bool flag = this.pTlqConnFlag == 0;
            if (!flag)
            {
                int num = TLQInterface.Tlq_CloseQCU(ref this.gid, ref this.qcuId, ref tLQError);
                bool flag2 = num < 0;
                if (flag2)
                {
                    this.GetProcByTLQError("Close QCU", num, ref tLQError);
                    num = TLQInterface.Tlq_DisConn(ref this.gid, ref tLQError);
                    bool flag3 = num < 0;
                    if (flag3)
                    {
                        this.GetProcByTLQError("Disconnect server", num, ref tLQError);
                    }
                    this.pTlqConnFlag = 0;
                }
            }
        }
        public int ReConnectToTLQ()
        {
            this.closeConnection();
            int result;
            for (int i = 0; i < 3; i++)
            {
                int num = this.openConnection();
                bool flag = num == 0;
                if (flag)
                {
                    result = 0;
                    return result;
                }
                Thread.Sleep(3000);
            }
            result = -1;
            return result;
        }
        public bool sendMessage(string message)
        {
            int num = 0;
            TLQMSG_INFO tLQMSG_INFO = default(TLQMSG_INFO);
            TLQMSG_OPT tLQMSG_OPT = default(TLQMSG_OPT);
            TLQError tLQError = default(TLQError);
            TLQInterface.Tlq_InitMsgInfo(ref tLQMSG_INFO);
            TLQInterface.Tlq_InitMsgOpt(ref tLQMSG_OPT);
            tLQMSG_INFO.MsgType = 0;
            tLQMSG_INFO.Persistence = 1;
            tLQMSG_INFO.Priority = 4;
            tLQMSG_INFO.Expiry = -1;
            tLQMSG_OPT.QueName = this.queueName;
            byte[] bytes = encoding.GetBytes(message);
            string @string = encoding.GetString(bytes);
            tLQMSG_INFO.MsgSize = encoding.GetByteCount(@string);
            while (true)
            {
                int num2 = TLQInterface.Tlq_PutMsg(ref this.gid, ref this.qcuId, ref tLQMSG_INFO, ref tLQMSG_OPT, (IntPtr)0, bytes, ref tLQError);
                bool flag = num2 < 0;
                if (!flag)
                {
                    return true;
                }
                num++;
                bool flag2 = num > 3;
                if (flag2)
                {
                    break;
                }
                num2 = this.GetProcByTLQError(ref tLQError);
                bool flag3 = num2 == 2;
                if (!flag3)
                {
                    bool flag4 = num2 == 1;
                    if (!flag4)
                    {
                        goto IL_104;
                    }
                    num2 = this.ReConnectToTLQ();
                }
            }
            throw new Exception("Send message faild.Error:" + tLQError.errstr);
        IL_104:
            throw new Exception("Send message faild.Error:" + tLQError.errstr);
        }
        public string getMessage()
        {
            byte[] array = null;
            int num = 0;
            TLQMSG_INFO tLQMSG_INFO = default(TLQMSG_INFO);
            TLQMSG_OPT tLQMSG_OPT = default(TLQMSG_OPT);
            TLQError tLQError = default(TLQError);
            TLQInterface.Tlq_InitMsgInfo(ref tLQMSG_INFO);
            TLQInterface.Tlq_InitMsgOpt(ref tLQMSG_OPT);
            tLQMSG_OPT.AckMode = 1;
            tLQMSG_OPT.WaitInterval = this.pWaitInterval;
            tLQMSG_OPT.QueName = this.rcvQueName;
            while (true)
            {
                int num2 = TLQInterface.Tlq_GetMsg(ref this.gid, ref this.qcuId, ref tLQMSG_INFO, ref tLQMSG_OPT, ref array, ref tLQError);
                bool flag = num2 < 0;
                if (!flag)
                {
                    goto IL_C8;
                }
                num++;
                bool flag2 = num > 3;
                if (flag2)
                {
                    break;
                }
                num2 = this.GetProcByTLQError(ref tLQError);
                bool flag3 = num2 == 1;
                if (flag3)
                {
                    num2 = this.ReConnectToTLQ();
                }
                else
                {
                    bool flag4 = num2 == 2;
                    if (!flag4)
                    {
                        goto IL_C7;
                    }
                }
            }
            throw new Exception("Get message faild.Error:" + tLQError.errstr);
        IL_C7:
        IL_C8:
            string result = string.Empty;
            bool flag5 = array != null;
            if (flag5)
            {
                result = encoding.GetString(array);
            }
            return result;
        }
        public bool pubMessage(string message, int serviceType)
        {
            int num = 0;
            TLQMSG_INFO tLQMSG_INFO = default(TLQMSG_INFO);
            TLQMSG_OPT tLQMSG_OPT = default(TLQMSG_OPT);
            TLQError tLQError = default(TLQError);
            byte[] bytes = encoding.GetBytes(message);
            bool flag = this.cancelpubFlag == 0;
            if (flag)
            {
                TLQInterface.Tlq_InitMsgInfo(ref tLQMSG_INFO);
                TLQInterface.Tlq_InitMsgOpt(ref tLQMSG_OPT);
                tLQMSG_INFO.Persistence = 1;
                tLQMSG_INFO.Priority = 5;
                tLQMSG_INFO.Expiry = -1;
                tLQMSG_OPT.OperateType = 3;
                tLQMSG_OPT.PubSubScope = 3;
                tLQMSG_OPT.Topic = this.topicName;
                tLQMSG_INFO.MsgSize = bytes.Length;
                while (true)
                {
                    int num2 = TLQInterface.Tlq_PutMsg(ref this.gid, ref this.qcuId, ref tLQMSG_INFO, ref tLQMSG_OPT, (IntPtr)0, bytes, ref tLQError);
                    bool flag2 = num2 < 0;
                    if (!flag2)
                    {
                        return true;
                    }
                    num++;
                    bool flag3 = num > 3;
                    if (flag3)
                    {
                        break;
                    }
                    num2 = this.GetProcByTLQError(ref tLQError);
                    bool flag4 = num2 == 2;
                    if (!flag4)
                    {
                        bool flag5 = num2 == 1;
                        if (!flag5)
                        {
                            goto IL_109;
                        }
                        num2 = this.ReConnectToTLQ();
                    }
                }
                throw new Exception("Send message faild.Error:" + tLQError.errstr);
            IL_109:
                throw new Exception("Send message faild.Error:" + tLQError.errstr);
            }
            throw new Exception("Pub flag is cancel");
        }
        public void subscribe(SDKClient.IReceiver messageListener)
        {
            TLQMSG_INFO tLQMSG_INFO = default(TLQMSG_INFO);
            TLQMSG_OPT tLQMSG_OPT = default(TLQMSG_OPT);
            TLQInterface.Tlq_InitMsgInfo(ref tLQMSG_INFO);
            TLQInterface.Tlq_InitMsgOpt(ref tLQMSG_OPT);
            tLQMSG_INFO.Persistence = 1;
            tLQMSG_INFO.Priority = 5;
            tLQMSG_INFO.Expiry = -1;
            tLQMSG_INFO.MsgSize = 10;
            tLQMSG_OPT.OperateType = 5;
            tLQMSG_OPT.PubSubScope = 3;
            tLQMSG_OPT.Topic = this.topicName;
            tLQMSG_OPT.QueName = this.rcvQueName;
            bool flag = this.cancelsubFlag == 0;
            if (flag)
            {
                int num = TLQInterface.Tlq_PutMsg(ref this.gid, ref this.qcuId, ref tLQMSG_INFO, ref tLQMSG_OPT, (IntPtr)0, null, ref this.errstru);
                bool flag2 = num < 0;
                if (flag2)
                {
                    throw new Exception("Sub topic faild.Error:" + this.errstru.errstr);
                }
                this.CorrMsgId = tLQMSG_INFO.MsgId;
                this.receiveData(messageListener);
            }
            else
            {
                this.receiveData(messageListener);
            }
        }
        private void receiveData(SDKClient.IReceiver messageListener)
        {
            int num = 0;
            TLQError tLQError = default(TLQError);
            TLQMSG_INFO tLQMSG_INFO = default(TLQMSG_INFO);
            TLQMSG_OPT tLQMSG_OPT = default(TLQMSG_OPT);
            while (true)
            {
                byte[] bytes = null;
                TLQInterface.Tlq_InitMsgInfo(ref tLQMSG_INFO);
                TLQInterface.Tlq_InitMsgOpt(ref tLQMSG_OPT);
                tLQMSG_INFO.Persistence = 1;
                tLQMSG_OPT.QueName = this.rcvQueName;
                tLQMSG_OPT.AckMode = 1;
                tLQMSG_OPT.WaitInterval = this.pWaitInterval;
                while (true)
                {
                    this.ret = TLQInterface.Tlq_GetMsg(ref this.gid, ref this.qcuId, ref tLQMSG_INFO, ref tLQMSG_OPT, ref bytes, ref tLQError);
                    bool flag = this.ret < 0;
                    if (!flag)
                    {
                        goto IL_FA;
                    }
                    num++;
                    bool flag2 = num > 3;
                    if (flag2)
                    {
                        goto Block_2;
                    }
                    this.ret = this.GetProcByTLQError(ref tLQError);
                    bool flag3 = this.ret == 1;
                    if (flag3)
                    {
                        this.ret = this.ReConnectToTLQ();
                    }
                    else
                    {
                        bool flag4 = this.ret == 2;
                        if (!flag4)
                        {
                            break;
                        }
                    }
                }
                continue;
            IL_FA:
                string @string = encoding.GetString(bytes);
                messageListener.receiveData(@string);
            }
        Block_2:
            throw new Exception("Receive message faild.Error:" + tLQError.errstr);
        }
        public void unSubscribe()
        {
            TLQMSG_INFO tLQMSG_INFO = default(TLQMSG_INFO);
            TLQMSG_OPT tLQMSG_OPT = default(TLQMSG_OPT);
            TLQError tLQError = default(TLQError);
            tLQMSG_INFO.CorrMsgId = this.CorrMsgId;
            tLQMSG_OPT.OperateType = 6;
            this.ret = TLQInterface.Tlq_PutMsg(ref this.gid, ref this.qcuId, ref tLQMSG_INFO, ref tLQMSG_OPT, (IntPtr)0, null, ref tLQError);
            bool flag = this.ret < 0;
            if (flag)
            {
                throw new Exception("Unsub topic faild.Error:" + tLQError.errstr);
            }
        }
        private int GetProcByTLQError(ref TLQError pTlqError)
        {
            int tlq_errno = pTlqError.tlq_errno;
            int result;
            if (tlq_errno > 59)
            {
                if (tlq_errno <= 1017)
                {
                    if (tlq_errno == 1000)
                    {
                        goto IL_A0;
                    }
                    if (tlq_errno != 1005)
                    {
                        if (tlq_errno != 1017)
                        {
                            goto IL_D9;
                        }
                        Thread.Sleep(3000);
                        result = 2;
                        return result;
                    }
                }
                else
                {
                    if (tlq_errno <= 2102)
                    {
                        if (tlq_errno == 1029)
                        {
                            goto IL_A0;
                        }
                        if (tlq_errno != 2102)
                        {
                            goto IL_D9;
                        }
                    }
                    else
                    {
                        if (tlq_errno == 2104)
                        {
                            result = 0;
                            return result;
                        }
                        if (tlq_errno != 2603)
                        {
                            goto IL_D9;
                        }
                        result = 0;
                        return result;
                    }
                }
                result = 0;
                return result;
            }
            if (tlq_errno <= 10)
            {
                if (tlq_errno == 4 || tlq_errno == 9)
                {
                    result = 0;
                    return result;
                }
                if (tlq_errno != 10)
                {
                    goto IL_D9;
                }
                result = 0;
                return result;
            }
            else
            {
                if (tlq_errno != 54 && tlq_errno != 58 && tlq_errno != 59)
                {
                    goto IL_D9;
                }
            }
        IL_A0:
            Thread.Sleep(3000);
            result = 1;
            return result;
        IL_D9:
            result = 0;
            return result;
        }
        private void GetProcByTLQError(string pHeadInfo, int pRet, ref TLQError pTlqError)
        {
            throw new Exception(string.Concat(new object[]
            {
                "[Error]",
                pHeadInfo,
                " failed!ret=",
                pRet,
                ",tlqErrno=",
                pTlqError.tlq_errno,
                ",sysErrno=",
                pTlqError.sys_errno,
                ",errsStr=",
                pTlqError.errstr
            }));
        }
    }
}
